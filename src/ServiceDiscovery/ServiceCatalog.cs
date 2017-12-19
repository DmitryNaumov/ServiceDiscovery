using System;
using System.Collections.Generic;
using System.Linq;

namespace NeedfulThings.ServiceDiscovery
{
    internal sealed class ServiceCatalog : IServiceCatalog
    {
        private static readonly IReadOnlyList<ServiceDescription> Empty = new List<ServiceDescription>(0);

        public event Action<ServiceDescription> ServiceRegistered;

        // Grouped by Service WellKnownName
        private readonly Dictionary<string, List<ServiceDescription>> _catalog = new Dictionary<string, List<ServiceDescription>>();

        internal readonly string Id = $"{nameof(ServiceCatalog)}-{Guid.NewGuid():N}";

        public void Register(Uri endpoint, string wellKnownName, Version version, IReadOnlyDictionary<string, string> capabilities)
        {
            var instanceId = Guid.NewGuid().ToString("N");
            Register(new ServiceDescription(Id, endpoint, wellKnownName, version, instanceId, capabilities));
        }

        public IReadOnlyList<ServiceDescription> GetServices()
        {
            lock (_catalog)
            {
                return _catalog.Values.SelectMany(services => services).ToList();
            }
        }

        public IReadOnlyList<ServiceDescription> GetServices(string wellKnownName)
        {
            lock (_catalog)
            {
                List<ServiceDescription> services;

                if (_catalog.TryGetValue(wellKnownName, out services))
                {
                    return services;
                }

                return Empty;
            }
        }

        public void Register(string wellKnownName, Uri endpoint, Version version, IReadOnlyDictionary<string, string> capabilities)
        {
            var instanceId = Guid.NewGuid().ToString("N");
            Register(new ServiceDescription(Id, endpoint, wellKnownName, version, instanceId, capabilities));
        }

        internal bool Register(ServiceDescription service)
        {
            bool isRegistered = false;

            lock (_catalog)
            {
                List<ServiceDescription> services;
                if (!_catalog.TryGetValue(service.WellKnownName, out services))
                {
                    services = new List<ServiceDescription>();
                    _catalog.Add(service.WellKnownName, services);
                }

                if (services.All(existingService => existingService.InstanceId != service.InstanceId))
                {
                    services.Add(service);

                    isRegistered = true;
                }
            }

            if (isRegistered)
            {
                ServiceRegistered?.Invoke(service);
            }

            return isRegistered;
        }
    }
}