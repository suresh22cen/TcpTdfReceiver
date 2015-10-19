using System;
using System.Text;

namespace UdpTdfReceiver
{
    using System.Net;
    using System.Net.Sockets;
    using Newtonsoft.Json;
    using RaceBusinessLogic;

    public class TcpDataService
    {
        private readonly TcpListener listener;

        public event EventHandler<DataRecord> DataReceived;
        readonly int _size;

        public TcpDataService(string ipAddress, int portNum, int size)
        {
            listener = new TcpListener(IPAddress.Parse(ipAddress), portNum);
            _size = size;
        }

        public void StartReading()
        {
            byte[] bytes = new byte[_size];
            listener.Start();
            
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            while (true)
            {
                if (stream.DataAvailable && stream.CanRead)
                {                   
                    Array.Clear(bytes, 0, bytes.Length);
                    stream.Read(bytes, 0, _size);
                    var data = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    DataReceived(this, JsonConvert.DeserializeObject<DataRecord>(data));

                    client = listener.AcceptTcpClient();
                    stream = client.GetStream();
                }
            }
        }

        public void Stop()
        {
            listener.Stop();
        }

        ~TcpDataService()
        {
            listener.Stop();
        }
    }
}
