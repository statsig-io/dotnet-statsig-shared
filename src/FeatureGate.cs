using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace StatsigShared
{
    public class FeatureGate
    {
        [JsonProperty("name")] public string Name { get; }
        [JsonProperty("value")] public bool Value { get; }
        [JsonProperty("rule_id")] public string RuleID { get; }
        [JsonProperty("secondary_exposures")] public List<Dictionary<string, string>> SecondaryExposures { get; }

        private static FeatureGate _defaultConfig;

        public static FeatureGate Default
        {
            get
            {
                if (_defaultConfig == null)
                {
                    _defaultConfig = new FeatureGate();
                }

                return _defaultConfig;
            }
        }

        public FeatureGate(string name = null, bool value = false, string ruleID = null,
            List<Dictionary<string, string>> secondaryExposures = null)
        {
            Name = name ?? "";
            Value = value;
            RuleID = ruleID ?? "";
            SecondaryExposures = secondaryExposures ?? new List<Dictionary<string, string>>();
        }

        public static FeatureGate FromJObject(string name, JObject jobj)
        {
            if (jobj == null)
            {
                return null;
            }

            JToken ruleToken;
            if (!jobj.TryGetValue("rule_id", out ruleToken))
            {
                return null;
            }

            JToken valueToken;
            if (!jobj.TryGetValue("value", out valueToken))
            {
                return null;
            }

            JToken nameToken;
            if (!jobj.TryGetValue("name", out nameToken))
            {
                return null;
            }

            try
            {
                return new FeatureGate
                (
                    nameToken.Value<string>(),
                    valueToken.Value<bool>(),
                    ruleToken.Value<string>(),
                    jobj.TryGetValue("secondary_exposures", out JToken? exposures)
                        ? exposures.ToObject<List<Dictionary<string, string>>>()
                        : new List<Dictionary<string, string>>()
                );
            }
            catch
            {
                // Failed to parse config.  TODO: Log this
                return null;
            }
        }
    }
}