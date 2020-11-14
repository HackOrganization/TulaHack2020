using System.Net;

namespace Networking.Utils
{
    public class Params
    {
        public const int SERVER_CONNECTIONS_COUNT = 1;
        public const int RETRY_SETTING_CONNECTION_TIMEOUT = 1000;
        
        public const int SERVER_BUFFER_SIZE = 1024;
        public const int CLIENT_BUFFER_SIZE = 64;

        private const string IP_ADDRESS = "127.0.0.1";
        
        public static readonly IPEndPoint WideFieldEndPoint = new IPEndPoint(IPAddress.Parse(IP_ADDRESS), WIDE_FIELD_PORT);
        private const int WIDE_FIELD_PORT = 51423;
    }
}