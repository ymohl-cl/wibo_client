using System;
using System.Net;
using System.Net.Sockets;
using System.Drawing;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Java.Nio;

namespace serverConnection
{
    [Activity(Label = "serverConnection", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Socket _soc;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.sendButton);

            _soc = createSocket();
            if (_soc.Connected == true)
                Console.Out.WriteLine("toto.");
            button.Click += delegate {
                if (_soc.Connected == true)
                {
                    ByteBuffer data = ByteBuffer.Allocate(8);
                    data.Clear();
                    data.PutFloat(0, 42.2f);
                    data.PutFloat(4, -42.2f);
                    sendData(_soc, data, 8, 1, 0, 1);
                }
                else
                {
                    Console.Out.WriteLine("Socket is not connected.");
                }
            };
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            _soc.Close();
        }
        public static Socket createSocket()
        {

            String server = "82.245.153.246";

            Socket socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            System.Net.IPAddress ipAddr =
                System.Net.IPAddress.Parse(server);

            System.Net.IPEndPoint remoteEP =
                new IPEndPoint(ipAddr, 8081);

            socket.Connect(remoteEP);

            return socket;
        }

        public static int sendData(Socket soc, ByteBuffer data, short contentSize, short type, int i, int nbPacket)
        {
            short headerSize = 24;
            
            ByteBuffer buffer = ByteBuffer.Allocate(1024);

            buffer.Clear();
            // Packet size (bytes)
            buffer.PutShort(0, (short)(headerSize + contentSize));
            Console.Out.WriteLine("size : {0}", (headerSize + contentSize));
            // Request type
            buffer.PutShort(2, type);
            Console.Out.WriteLine("type : {0}", type);
            // Checksum
            buffer.PutInt(4, 9999);
            // Total of packets
            buffer.PutInt(8, nbPacket);
            // Packet number
            buffer.PutInt(12, i);
            Console.Out.WriteLine("i : {0}", i);
            // Device ID
            buffer.PutLong(16, 424242424242);
            // Content
            buffer.Position(24);
            data.Position(0);
            buffer.Put(data);
            Console.Out.WriteLine("type : {0}", data);
            byte[] res = new byte[1024];
            buffer.Position(0);
            buffer.Get(res);
            for (int j = 0; j < 1024; j++)
                Console.Out.Write("{0}", res[j]);
            Console.Out.WriteLine("res : {0}",System.Text.Encoding.Default.GetString(res));
            int n = soc.Send(res);
            Console.Out.WriteLine("{0} bytes were sent.", n);

            return n;
        }
    }
}