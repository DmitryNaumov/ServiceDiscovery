using System;

namespace NeedfulThings.ServiceDiscovery
{
    public interface IDiscoveryTransport
    {
        event Action<ServiceDescription> ServiceDiscovered;

        event Action ServicesRequested;

        void DiscoverRemoteServices();

        void Announce(ServiceDescription serviceDescription);
    }
}