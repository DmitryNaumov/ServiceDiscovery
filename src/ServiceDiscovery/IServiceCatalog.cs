using System;
using System.Collections.Generic;

namespace NeedfulThings.ServiceDiscovery
{
    public interface IServiceCatalog
    {
        event Action<ServiceDescription> ServiceRegistered;

        IReadOnlyList<ServiceDescription> GetServices();

        IReadOnlyList<ServiceDescription> GetServices(string wellKnownName);

        void Register(string wellKnownName, Uri endpoint, Version version, IReadOnlyDictionary<string, string> capabilities);
    }
}