using System;
using System.Collections.Generic;

namespace StatsigShared
{
    public class SDKDetails
    {
        private static SDKDetails? _clientDetails;
        private static SDKDetails? _serverDetails;

        public string SDKType;
        public string SDKVersion;

        internal SDKDetails(string type)
        {
            SDKType = type;
            SDKVersion = GetType().Assembly.GetName()!.Version!.ToString();
        }

        public IReadOnlyDictionary<string, string> StatsigMetadata
        {
            get
            {
                return new Dictionary<string, string>
                {
                    ["sdkType"] = SDKType,
                    ["sdkVersion"] = SDKVersion
                };
            }
        }

        public static SDKDetails GetClientSDKDetails()
        {
            if (_clientDetails == null)
            {
                _clientDetails = new SDKDetails("dotnet-client");
            }
            return _clientDetails;
        }

        public static SDKDetails GetServerSDKDetails()
        {
            if (_serverDetails == null)
            {
                _serverDetails = new SDKDetails("dotnet-server");
            }
            return _serverDetails;
        }
    }
}
