using System.Net;

namespace Networking.Utils
{
    public class Params
    {
        public const int RECEIVE_BUFFER_SEZE = 256;

        private const string IP_ADDRESS = "127.0.0.1";
        private const int PORT = 11000;
        
        public static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(IP_ADDRESS), PORT);
    }
}