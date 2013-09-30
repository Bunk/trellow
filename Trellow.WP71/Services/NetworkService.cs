using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Caliburn.Micro;
using trello.Services;
using trellow.api.Data;

namespace Trellow.Services
{
    public class NetworkService : INetworkService
    {
        public bool IsAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }

        public NetworkService(IEventAggregator eventAggregator)
        {
            // todo: Implement polling for WP7
            // http://stackoverflow.com/questions/5266240/how-to-use-tpl-to-manage-multiple-indefinite-tasks
            // NetworkInformation.NetworkStatusChanged += sender => PublishChange(eventAggregator);
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