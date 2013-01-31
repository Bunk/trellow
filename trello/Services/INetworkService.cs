using System;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;

namespace trello.Services
{
    public interface INetworkService
    {
        bool IsAvailable { get; }

        Action<object> StatusChanged { get; set; }
    }

    public class NetworkService : INetworkService
    {
        public bool IsAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }

        public Action<object> StatusChanged { get; set; }

        public NetworkService()
        {
            NetworkInformation.NetworkStatusChanged += sender => StatusChanged(sender);
        }
    }
}
