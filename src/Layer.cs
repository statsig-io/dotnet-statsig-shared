﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace StatsigShared
{
    public class Layer
    {
        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("rule_id")]
        public string RuleID { get; }

        [JsonProperty("value")]
        public Dictionary<string, JToken> Value { get; }

        [JsonProperty("secondary_exposures")]
        public List<Dictionary<string, string>> SecondaryExposures;

        [JsonProperty("undelegated_secondary_exposures")]
        public List<Dictionary<string, string>> UndelegatedSecondaryExposures;

        [JsonProperty("explicit_parameters")]
        public List<string> ExplicitParameters;

        [JsonProperty("allocated_experiment_name")]
        public string AllocatedExperimentName;

        public Action<Layer, string> OnExposure;

        static Layer? _default;

        public static Layer Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new Layer();
                }
                return _default;
            }
        }

        public Layer(string? name = null, 
            Dictionary<string, JToken>? value = null, 
            string? ruleID = null, 
            Action<Layer, string>? onExposure = null)
        {
            Name = name ?? "";
            Value = value ?? new Dictionary<string, JToken>();
            RuleID = ruleID ?? "";
            OnExposure = onExposure ?? delegate { };
            SecondaryExposures = new List<Dictionary<string, string>>();
            UndelegatedSecondaryExposures = new List<Dictionary<string, string>>();
            ExplicitParameters = new List<string>();
            AllocatedExperimentName = "";
        }

        public T? Get<T>(string key, T? defaultValue = default(T))
        {
            JToken? outVal;
            if (!this.Value.TryGetValue(key, out outVal))
            {
                return defaultValue;
            }

            try
            {
                var result = outVal.ToObject<T>();
                OnExposure(this, key);
                return result;
            }
            catch
            {
                // There are a bunch of different types of exceptions that could
                // be thrown at this point - missing converters, format exception
                // type cast exception, etc.
                return defaultValue;
            }
        }

        public static Layer? FromJObject(string configName, JObject? jobj)
        {
            if (jobj == null)
            {
                return null;
            }


            try
            {
                var layer = new Layer
                (
                    configName,
                    GetFromJSON<Dictionary<string, JToken>>(jobj, "value", new Dictionary<string, JToken>()),
                    GetFromJSON<string>(jobj, "rule_id", "")
                );

                layer.AllocatedExperimentName = GetFromJSON(jobj, "allocated_experiment_name", "");
                layer.SecondaryExposures = GetFromJSON(jobj, "secondary_exposures", new List<Dictionary<string, string>>());
                layer.UndelegatedSecondaryExposures = GetFromJSON(jobj, "undelegated_secondary_exposures", new List<Dictionary<string, string>>());
                layer.ExplicitParameters = GetFromJSON(jobj, "explicit_parameters", new List<string>());

                return layer;
            }
            catch
            {
                // Failed to parse config.  TODO: Log this
                return null;
            }
        }

        private static T GetFromJSON<T>(JObject json, string key, T defaultValue)
        {
            json.TryGetValue(key, out JToken? token);
            if (token == null)
            {
                return defaultValue;
            }
            return token == null ? defaultValue : (token.ToObject<T>() ?? defaultValue);
        }
    }
}
