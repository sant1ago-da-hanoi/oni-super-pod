using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using TUNING;
using UnityEngine;

namespace SuperPOD
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/sant1ago-da-hanoi/oni-super-pod", "preview.png")]
    [ConfigFile("config.json", IndentOutput: true)]
    [RestartRequired]
    public sealed class SuperPODOptions : SingletonOptions<SuperPODOptions>
    {
        // ===================== Spawn =====================

        [Option(Format = "F0")]
        [Limit(0, 36000)]
        [JsonProperty]
        public int TimeBeforeSpawn { get; set; } = 1800;

        // ===================== Blueprint =====================

        [Option(Format = "F0")]
        [Limit(0, 10)]
        [JsonProperty]
        public int DuplicantNumber { get; set; } = 3;

        [Option(Format = "F0")]
        [Limit(0, 10)]
        [JsonProperty]
        public int CarePackageNumber { get; set; } = 1;

        // ===================== Interests =====================

        [Option(Format = "F0")]
        [Limit(0, 13)]
        [JsonProperty]
        public int InterestNumber { get; set; } = 3;

        [Option(Format = "F0")]
        [Limit(0, 1000)]
        [JsonProperty]
        public int InterestValue { get; set; } = 7;

        // ===================== Traits =====================

        [Option(Format = "F0")]
        [Limit(0, 34)]
        [JsonProperty]
        public int PositiveTraitsNumber { get; set; } = 3;

        [Option(Format = "F0")]
        [Limit(0, 28)]
        [JsonProperty]
        public int NegativeTraitsNumber { get; set; } = 1;

        // ===================== Stress Reactions =====================

        [Option]
        [JsonProperty]
        public bool StressAggressive { get; set; } = true;

        [Option]
        [JsonProperty]
        public bool StressVomiter { get; set; } = true;

        [Option]
        [JsonProperty]
        public bool StressUglyCrier { get; set; } = true;

        [Option]
        [JsonProperty]
        public bool StressBingeEater { get; set; } = true;

        [Option]
        [JsonProperty]
        public bool StressBanshee { get; set; } = true;

        // ===================== Overjoyed Responses =====================

        [Option]
        [JsonProperty]
        public bool JoyBalloonArtist { get; set; } = true;

        [Option]
        [JsonProperty]
        public bool JoySparkleStreaker { get; set; } = true;

        [Option]
        [JsonProperty]
        public bool JoyStickerBomber { get; set; } = true;

        [Option]
        [JsonProperty]
        public bool JoySuperProductive { get; set; } = true;

        [Option]
        [JsonProperty]
        public bool JoyHappySinger { get; set; } = true;

        // ===================== Validation helpers =====================

        private static readonly Dictionary<string, Func<SuperPODOptions, bool>> StressMap =
            new Dictionary<string, Func<SuperPODOptions, bool>>
            {
                { "Aggressive",    o => o.StressAggressive },
                { "StressVomiter", o => o.StressVomiter },
                { "UglyCrier",     o => o.StressUglyCrier },
                { "BingeEater",    o => o.StressBingeEater },
                { "Banshee",       o => o.StressBanshee },
            };

        private static readonly Dictionary<string, Func<SuperPODOptions, bool>> OverjoyedMap =
            new Dictionary<string, Func<SuperPODOptions, bool>>
            {
                { "BalloonArtist",   o => o.JoyBalloonArtist },
                { "SparkleStreaker", o => o.JoySparkleStreaker },
                { "StickerBomber",   o => o.JoyStickerBomber },
                { "SuperProductive", o => o.JoySuperProductive },
                { "HappySinger",     o => o.JoyHappySinger },
            };

        public static string GetStress()
        {
            var opts = Instance;
            var enabled = StressMap
                .Where(kv => kv.Value(opts))
                .Select(kv => kv.Key)
                .ToList();

            if (enabled.Count == 0)
                enabled = StressMap.Keys.ToList();

            return enabled[UnityEngine.Random.Range(0, enabled.Count)];
        }

        public static string GetOverjoyed()
        {
            var opts = Instance;
            var enabled = OverjoyedMap
                .Where(kv => kv.Value(opts))
                .Select(kv => kv.Key)
                .ToList();

            if (enabled.Count == 0)
                enabled = OverjoyedMap.Keys.ToList();

            return enabled[UnityEngine.Random.Range(0, enabled.Count)];
        }

        public static int GetDuplicantNumber()
        {
            var opts = Instance;
            int dup = Mathf.Clamp(opts.DuplicantNumber, 0, 10);
            int care = Mathf.Clamp(opts.CarePackageNumber, 0, 10);
            if (dup + care > 10)
                dup = 3;
            return dup;
        }

        public static int GetCarePackageNumber()
        {
            var opts = Instance;
            int dup = Mathf.Clamp(opts.DuplicantNumber, 0, 10);
            int care = Mathf.Clamp(opts.CarePackageNumber, 0, 10);
            if (dup + care > 10)
                care = 1;
            return care;
        }

        public static int GetInterestNumber()
        {
            return Mathf.Clamp(Instance.InterestNumber, 0, 13);
        }

        public static int GetInterestValue()
        {
            int value = Mathf.Clamp(Instance.InterestValue, 0, 1000);
            int multiplier = UnityEngine.Random.Range(0, 101) <= 90 ? 1 : 2;
            return value * multiplier;
        }

        public static float GetInterestValueAsFloat()
        {
            return GetInterestValue();
        }

        public static int GetPositiveTraitsNumber()
        {
            return Mathf.Clamp(Instance.PositiveTraitsNumber, 0, DUPLICANTSTATS.GOODTRAITS.Count);
        }

        public static int GetNegativeTraitsNumber()
        {
            return Mathf.Clamp(Instance.NegativeTraitsNumber, 0, DUPLICANTSTATS.BADTRAITS.Count);
        }

        public static int GetMaxTraits()
        {
            return GetPositiveTraitsNumber() + GetNegativeTraitsNumber() + 4;
        }
    }
}
