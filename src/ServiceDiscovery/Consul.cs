using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NeedfulThings.ServiceDiscovery
{
    internal sealed class Consul : IDisposable
    {
        private readonly ServiceCatalog _catalog;
        private readonly IDiscoveryTransport _transport;

        public Consul([NotNull] ServiceCatalog catalog, [NotNull] IDiscoveryTransport transport)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (transport == null) throw new ArgumentNullException(nameof(transport));

            _catalog = catalog;
            _transport = transport;

            catalog.ServiceRegistered += OnServiceRegistered;
            transport.ServicesRequested += OnServicesRequested;
            transport.ServiceDiscovered += OnServiceDiscovered;

            DiscoverWorld();
        }

        public void Dispose()
        {
            _catalog.ServiceRegistered -= OnServiceRegistered;
        }

        private void OnServiceRegistered(ServiceDescription serviceDescription)
        {
            _transport.Announce(serviceDescription);
        }

        private void OnServiceDiscovered(ServiceDescription serviceDescription)
        {
            if (serviceDescription.CatalogId == _catalog.Id)
            {
                // in case transport broadcasts announcements and we can receive our own
                return;
            }

            _catalog.Register(serviceDescription);
        }

        private void OnServicesRequested()
        {
            foreach (var serviceDescription in GetLocalServices())
            {
                _transport.Announce(serviceDescription);
            }
        }

        private IEnumerable<ServiceDescription> GetLocalServices()
        {
            var services = _catalog.GetServices().Where(s => s.CatalogId != _catalog.Id);
            return services;
        }

        private void DiscoverWorld()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        _transport.DiscoverRemoteServices();
                        break;
                    }
                    catch (Exception)
                    {
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            });
        }
    }
}