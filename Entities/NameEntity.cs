using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AzSignalR.Monitor.Storage.Entities
{
    public class NameEntity : CompoundTableEntity
    {
        public static readonly string IndexPartition = (long.MaxValue - 1).ToString("d19");

        private static readonly Regex IndexRowKeyRegex = new Regex(@"^(\d{19})\|(.*)$");

        public NameEntity() : base() { }
        public NameEntity(ResourceType type, string region, string name) : base(BuildKeys(type, region, name).Item1, BuildKeys(type, region, name).Item2)
        {
            Type = type;
            NameRegion = region;
        }

        [ConvertableEntityProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResourceType Type { get; set; }
        public string NameRegion { get; set; }
        public DateTime EffectiveTime { get; set; }

        [ConvertableEntityProperty]
        public List<string> AlternativeNames { get; set; }

        [IgnoreProperty]
        public string Name
        {
            get
            {
                if (PartitionKey == IndexPartition)
                {
                    var match = IndexRowKeyRegex.Match(RowKey);
                    return match.Success ? match.Groups[2].Value : RowKey;
                }
                return RowKey;
            }
        }

        [IgnoreProperty]
        public string Index
        {
            get
            {
                if (PartitionKey == IndexPartition)
                {
                    var match = IndexRowKeyRegex.Match(RowKey);
                    return match.Success ? match.Groups[1].Value : "";
                }
                return "";
            }
        }

        [IgnoreProperty]
        public string NameIdentifier => $"{Type}|{NameRegion}|{Name}";

        [ConvertableEntityProperty]
        public Dictionary<string, NameValidityGroup> Validity { get; set; } = new Dictionary<string, NameValidityGroup>();

        private object _validityLock = new object();

        public bool EnsureValidity(string packPartition, DateTime created, DateTime time, bool isDeleted = false)
        {
            NameValidityGroup validityGroup;
            lock (_validityLock)
            {
                if (!Validity.TryGetValue(packPartition, out validityGroup))
                {
                    validityGroup = new NameValidityGroup
                    {
                        PackPartition = packPartition,
                    };
                    Validity[packPartition] = validityGroup;
                }
            }
            bool changed = validityGroup.EnsureValidity(created, time, isDeleted);
            lock (_validityLock)
            {
                if (changed && time > EffectiveTime)
                {
                    EffectiveTime = time;
                }
            }
            return changed;
        }

        public bool IsValidInRange(DateTime validStart, DateTime validEnd)
        {
            if (Validity.Values.Any(v => v.StartTime <= validEnd && v.EndTime >= validStart))
            {
                return true;
            }
            if (Type == ResourceType.SignalR)
            {
                return Validity.Values.Any(v => v.IsValidInRange(validStart, validEnd, true));
            }
            return false;
        }

        public NameEntity ToIndex()
        {
            if (PartitionKey == IndexPartition)
            {
                throw new ArgumentException("Cannot convert a name index once again");
            }
            return new NameEntity
            {
                PartitionKey = IndexPartition,
                RowKey = $"{Utils.InversedTimeKey(EffectiveTime)}|{Name}",
                Type = Type,
                NameRegion = NameRegion,
                Validity = Validity,
                EffectiveTime = EffectiveTime,
                AlternativeNames = AlternativeNames,
            };
        }

        public void NormalizeInplace()
        {
            if (PartitionKey != IndexPartition)
            {
                return;
            }
            (PartitionKey, RowKey) = BuildKeys(Type, NameRegion, Name);
        }

        public static (string, string) BuildKeys(ResourceType type, string region, string name)
        {
            var pk = string.IsNullOrEmpty(region) ? type.ToString() : $"{type}|{region}";
            return (pk, name);
        }
    }

    public class NameValidity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsDeleted { get; set; }

        public NameValidity ExtendEndTime(DateTime endTime)
        {
            return new NameValidity
            {
                Start = Start,
                End = endTime > End ? endTime : End,
                IsDeleted = IsDeleted,
            };
        }
    }

    public class NameValidityGroup
    {
        public string PackPartition { get; set; }

        public List<NameValidity> ValidPeriods { get; set; } = new List<NameValidity>();

        public DateTime StartTime => ValidPeriods?.FirstOrDefault()?.Start ?? new DateTime();

        public DateTime EndTime => ValidPeriods?.FirstOrDefault()?.End ?? new DateTime();

        public bool EnsureValidity(DateTime created, DateTime time, bool isDeleted = false)
        {
            lock (this)
            {
                bool changed = false;
                if (ValidPeriods.Count == 0)
                {
                    ValidPeriods.Add(new NameValidity
                    {
                        Start = created < time ? created : time,
                        End = time,
                        IsDeleted = isDeleted,
                    });
                    changed = true;
                }
                else
                {
                    var last = ValidPeriods.Last();
                    if (last.IsDeleted && isDeleted)
                    {
                        changed = false;
                    }
                    else if (isDeleted)
                    {
                        if (time > last.End)
                        {
                            last.End = time;
                        }
                        last.IsDeleted = true;
                        changed = true;
                    }
                    else if (last.IsDeleted)
                    {
                        // start a new scope after previous deletion.
                        var newScope = new NameValidity
                        {
                            Start = time,
                            End = time,
                        };
                        ValidPeriods.Add(newScope);
                        changed = true;
                    }
                    else
                    {
                        if (time > last.End)
                        {
                            last.End = time;
                            changed = true;
                        }
                        else if (time < last.Start)
                        {
                            // rarely we would need an update here.
                            last.Start = time;
                            changed = true;
                        }
                    }
                }
                return changed;
            }
        }

        /// <summary>
        /// For SignalR resource, we only fetch the resource table updates to reduce full update
        /// on every check. The side effect is the name validity will not be updated if the resource
        /// was not updated since last check.
        ///
        /// We handle the resource specially here. If the resource was not deleted, we extend its lifetime
        /// to the current time.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="extend">If we need to extend the name validity end time, i.e., resource is SignalR.</param>
        /// <returns></returns>
        public bool IsValidInRange(DateTime start, DateTime end, bool extend)
        {
            if (ValidPeriods.Count == 0)
            {
                return false;
            }
            if (ValidPeriods.Any(v => v.Start <= end && v.End >= start))
            {
                return true;
            }
            var last = ValidPeriods.Last();
            if (extend && !last.IsDeleted && last.Start <= end)
            {
                return true;
            }
            return false;
        }

        public NameValidityGroup ExtendEndTime(DateTime end)
        {
            if (ValidPeriods.Count == 0)
            {
                return this;
            }
            var last = ValidPeriods.Last();
            if (last.IsDeleted)
            {
                return this;
            }
            var validityPeriods = new List<NameValidity>(ValidPeriods);
            validityPeriods[validityPeriods.Count - 1] = last.ExtendEndTime(end);
            return new NameValidityGroup
            {
                PackPartition = PackPartition,
                ValidPeriods = validityPeriods,
            };
        }
    }
}
