using System.Net;

namespace Networking.Client
{
    /// <summary>
    /// Объект, содержащий информацию переподключения в случае, если не удалось установить соединение с первой попытки
    /// </summary>
    public class ClientConnectionObject
    {
        /// <summary>
        /// Конечная точка подключениея
        /// </summary>
        public readonly IPEndPoint RemoteEndPoint;

        /// <summary>
        /// Объект многопточного асинхронного клиента
        /// </summary>
        public readonly AsynchronousClient Client;

        public ClientConnectionObject(AsynchronousClient client, IPEndPoint remoteEndPoint)
        {
            Client = client;
            RemoteEndPoint = remoteEndPoint;
        }
    }
}