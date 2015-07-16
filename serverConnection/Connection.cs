using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Json;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.Nio;

namespace serverConnection
{
    class Connection
    {
        class StateObject
        {
            internal byte[] sBuffer;
            internal Socket sSocket;
            internal StateObject(int size, Socket sock)
            {
                sBuffer = new byte[size];
                sSocket = sock;
            }
        }
        internal IPEndPoint _serverEndpoint;
        internal Socket _socket;
        public bool Connected;

        // Default constructor
        public Connection()
        {
            string ip = "82.245.153.246";
            int port = 8081;

            this._socket = new Socket(
               AddressFamily.InterNetwork,
               SocketType.Stream,
               ProtocolType.Tcp);

            IPAddress serverIP = IPAddress.Parse(ip);

            this._serverEndpoint = new IPEndPoint(serverIP, port);
        }

        public bool Connect()
        {
            try
            {
                Console.Out.WriteLine("Trying to connect to : {0}", this._serverEndpoint.ToString());
                this._socket.Connect(this._serverEndpoint);
                Console.Out.WriteLine("Connected");
                this.Connected = true;
                return true;
            }
            catch (System.Exception err)
            {
                Console.WriteLine("Could not connect : {0}", err);
                this.Connected = false;
                return false;
            }
        }

        public Socket getSocket()
        {
            return this._socket;
        }

        public void Close()
        {
            this._socket.Shutdown(SocketShutdown.Both);
            this._socket.Close();
        }

        public bool SendLocation(float lon, float lat)
        {
            ByteBuffer data = ByteBuffer.Allocate(8);

            data.Clear();

            data.PutFloat(0, lon);
            data.PutFloat(4, lat);

            return Send(1, data);
        }

        public bool SendGrab(int id)
        {
            ByteBuffer data = ByteBuffer.Allocate(8);

            data.Clear();

            data.PutInt(0, id);

            return Send(2, data);
        }

        public bool SendFollow(int id)
        {
            ByteBuffer data = ByteBuffer.Allocate(8);

           data.PutInt(0, id);

           return Send(3, data);
        }

        public bool SendUnfollow(int id)
        {
            ByteBuffer data = ByteBuffer.Allocate(8);

            data.PutInt(0, id);

            return Send(4, data);
        }

        public bool SendBalloon(float lon, float lat, string title, string content)
        {
            if (title.Length >= 16)
                return false;
            ByteBuffer data = ByteBuffer.Allocate(24 + content.Length + 10);

            byte[] name = new byte[16];

            Encoding.UTF8.GetBytes(title, 0, title.Length, name, 0);
/*            
            for (int i = title.Length; i < 16; i++)
            {
                name[i] = 0x20;
            }
 */
            //Array.Clear(name, title.Length, (16 - title.Length));
            byte[] msg = Encoding.UTF8.GetBytes(content);

            data.Put(ByteBuffer.Wrap(name));
            data.Position(name.Length);
            data.PutFloat(16, lon);
            data.PutFloat(20, lat);
            data.PutInt(24, content.Length);
            data.Position(32);
            data.Put(ByteBuffer.Wrap(msg));

            return Send(5, data);
        }
        public bool Send(short type, ByteBuffer data)
        {
            data.Position(0);
            int packetId = 0;
            int totalSize = data.Remaining();
            int nbPackets = totalSize / 1000;
            if (totalSize % 1000 > 0)
            {
                nbPackets += 1;
            }
            int packetSize;
            if (totalSize - ((packetId + 1) * 1000) >= 1000)
                packetSize = 1024;
            else
                packetSize = 24 + (totalSize - (packetId * 1000));
            long deviceId = 4242424242;

            ByteBuffer buffer = ByteBuffer.Allocate(1024);
            buffer.Clear();
            buffer.PutShort(0, (short)packetSize);
            buffer.PutShort(2, type);
            buffer.PutInt(8, nbPackets);
            buffer.PutInt(12, packetId);
            buffer.PutLong(16, deviceId);
            buffer.Position(24);
            data.Position(0);
            buffer.Put(data);

            byte[] res = new byte[1024];
            buffer.Position(0);
            buffer.Get(res);
            try
            {
/*                Console.Write("res : ");
                for (int count = 0; count < 1024; count++)
                    Console.Write("{0}", res[count]);
                Console.WriteLine("");
*/                this._socket.Send(res);
            }
            catch (System.Exception err)
            {
                Console.Out.WriteLine("Could not send : {0}", err);
                return false;
            }
            return true;
        }

        public void Receive()
        {
            byte[] array = new byte[1024];

            while (true)
            {
                if (this._socket.Available > 0)
                {
                    int bytesReceived = this._socket.Receive(array);
                    
                    Console.WriteLine("{0} received.", bytesReceived);

                    ByteBuffer buffer = ByteBuffer.Wrap(array);
    
                    parseBuffer(array);
                    
                    Array.Clear(array, 0, array.Length);
                }
            }
        }

        private void parseBuffer(byte[] array)
        {
            string json;
            float lon;
            float lat;

            ByteBuffer buffer = ByteBuffer.Wrap(array);

            short type = buffer.GetShort(2);

            switch (type)
            {
                case 1:
                    Console.WriteLine("Parse location :");
                    
                    lon = buffer.GetFloat(24);
                    lat = buffer.GetFloat(28);
                    
                    json = @"{ lon: '" + lon.ToString() + "', lat: '" + lat.ToString() + "' }";
                    break;
                case 5:
                    Console.WriteLine("Parse Balloon :");
                    
                    byte[] titleArray = new byte [16];

                    buffer.Position(24);
                    buffer.Get(titleArray, 0, 16);

                    string title = Encoding.UTF8.GetString(titleArray, 0, 4);

                    buffer.Position(0);

                    lon = buffer.GetFloat(40);
                    lat = buffer.GetFloat(44);

                    int msgLength = buffer.GetInt(48);
                    
                    byte[] msgArray = new byte[msgLength];

                    buffer.Position(56);
                    buffer.Get(msgArray, 0, msgLength);

                    string msg = Encoding.UTF8.GetString(msgArray, 0, msgLength);
                    buffer.Position(0);

                    json = @"{ lon: '" + lon.ToString() + "', lat: '" + lat.ToString() + "' , title: '" + title + "', content: '" + msg + "' }";
                    break;
                default:
                    int id = buffer.GetInt(24);
                    
                    json = @"{ id: '" + id.ToString() + "' }";
                    break;
            }
             
            Console.WriteLine("json : {0}", json);
        }
    }
}
