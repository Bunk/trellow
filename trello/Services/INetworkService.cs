using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Caliburn.Micro;
using Windows.Networking.Connectivity;

namespace trello.Services
{
    public interface INetworkService
    {
        bool IsAvailable { get; }
    }

    public class NetworkStatusChanged
    {
        
    }

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
