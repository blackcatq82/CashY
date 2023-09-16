namespace CashY.Services
{
    public interface IConnection
    {
        bool IsConnected { get; set; }
        Action<NetworkAccess> NetworkOnChange { get; set; }
        NetworkAccess NetworkAccess { get; set; }
        void Start();
    }
    public class Connection : IConnection
    {
        private bool isConnection;
        public bool IsConnected {
            get { Start(); return isConnection; }
            set => isConnection = value; }

        private Action<NetworkAccess> networkOnChange;
        public Action<NetworkAccess> NetworkOnChange { get => networkOnChange; set => networkOnChange = value; }


        public NetworkAccess networkAccess;
        public NetworkAccess NetworkAccess { get => networkAccess; set => networkAccess = value; }

        public void Start()
        {
            // get state connection.
            NetworkAccess current = Connectivity.NetworkAccess;

            // check on started has connection to network.
            if (current == NetworkAccess.Internet)
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }

            // handler network state.
            NetworkOnChange?.Invoke(current);

            // Register for connectivity changes, be sure to unsubscribe when finished
            Connectivity.ConnectivityChanged += ConnectivityChanged;
        }

        void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            // set current networkAccess
            this.NetworkAccess = e.NetworkAccess;

            // handler change networking.
            NetworkOnChange?.Invoke(e.NetworkAccess);
        }
    }
}
