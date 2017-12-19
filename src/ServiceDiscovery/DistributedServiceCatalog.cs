using System;
using JetBrains.Annotations;

namespace NeedfulThings.ServiceDiscovery
{
    public static class DistributedServiceCatalog
    {
        public static IServiceCatalog Create([NotNull] IDiscoveryTransport transport)
        {
            if (transport == null) throw new ArgumentNullException(nameof(transport));

            var catalog = new ServiceCatalog();
            var consul = new Consul(catalog, transport);

            return catalog;
        }
    }
}