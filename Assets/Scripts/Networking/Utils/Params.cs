using System.Net;

namespace Networking.Utils
{
    public class Params
    {
        public const int RECEIVE_BUFFER_SEZE = 256;

        private const string IP_ADDRESS = "127.0.0.1";
        
        public static readonly IPEndPoint WideFieldEndPoint = new IPEndPoint(IPAddress.Parse(IP_ADDRESS), WIDE_FIELD_PORT);
        private const int WIDE_FIELD_PORT = 11000;
    }
}