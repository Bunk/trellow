using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services;
using trellow.api.Data;

namespace Trellow.Services
{
    [UsedImplicitly]
    public class NetworkService : INetworkService
    {
        public bool IsAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }

        public NetworkService(IEventAggregator eventAggregator)
        {
            NetworkInformation.NetworkStatusChanged += sender => PublishChange(eventAggregator);
        }

        private static void PublishChange(IEventAggregator aggregator)
        {
            Task.Factory.StartNew(() =>
            {
                var message = new NetworkStatusChanged();
                aggregator.Publish(message);
            });
        }
    }
}