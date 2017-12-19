using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeedfulThings.ServiceDiscovery
{
    [DebuggerDisplay("[{WellKnownName}] {Endpoint.ToString()}")]
    public sealed class ServiceDescription
    {
        internal ServiceDescription(string catalogId, Uri endpoint, string wellKnownName, Version version, string instanceId, IReadOnlyDictionary<string, string> capabilities)
        {
            if (!endpoint.IsAbsoluteUri) throw new ArgumentException("Endpoint should be absolute Uri");

            CatalogId = catalogId;
            Endpoint = endpoint;
            WellKnownName = wellKnownName;
            Version = version;
            InstanceId = instanceId;
            Capabilities = capabilities;
        }

        public Uri Endpoint { get; }
        public string WellKnownName { get; }
        public Version Version { get; }
        public IReadOnlyDictionary<string, string> Capabilities { get; }

        internal string CatalogId { get; }
        internal string InstanceId { get; }
    }
}