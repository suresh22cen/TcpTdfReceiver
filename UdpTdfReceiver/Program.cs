using System;

namespace UdpTdfReceiver
{
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Serialization.Formatters.Binary;
    using RaceBusinessLogic;

    class Program
    {
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static TcpDataService service;

        static void Main(string[] args)
		{
			// arg 1 is port, default 12345
			try
			{
			    string ipAddress = GetLocalIPAddress();
				int port = 8091;

                if (args.Length > 1)
                    ipAddress = args[0];

                if (args.Length > 2)
					port = int.Parse(args[1]);

                SetConsoleCtrlHandler((e) => {
                    service.Stop();
                    return false;
                }, true);

                service = new TcpDataService(ipAddress, port, getObjectSize());

                service.DataReceived += Service_DataReceived;
                service.StartReading();            
            }
			catch (Exception ex)
			{
				Console.WriteLine("Pass port number");
			}
		}

        private static void Service_DataReceived(object sender, DataRecord e)
        {
            RaceDB raceDB = new RaceDB(ConfigurationManager.ConnectionStrings["RaceDB"].ToString(), ConfigurationManager.AppSettings["dbName"]);
            raceDB.SaveDataRecord(e);
        }

        private static string GetLocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }

        private static int getObjectSize()
        {
            long size = 0;
            DataRecord record = new DataRecord();
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, record);
                return Convert.ToInt32(stream.Length);
            }
        }      
    }
}
