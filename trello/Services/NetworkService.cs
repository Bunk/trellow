using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Caliburn.Micro;
using Windows.Networking.Connectivity;
using trellow.api.Data;

namespace trello.Services
{
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