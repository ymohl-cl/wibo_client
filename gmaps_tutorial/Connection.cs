using System;
using System.Collections.Generic;
using System.Collections;
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
using Android.Telephony;

using Java.Nio;
using Java.Util;

/*
 *  For debug reasons, Connect() method is called when creating a request
 *  and Diconnect() method is called after receiving the server response.
 *  In the actual programm, Connect() will be called by MainActivity's OnCreate() method
 *  and the connection will only be closed when OnDestroy() or OnPause() is called.
 */

namespace wibo
{
    public class Connection
    {
        private IPEndPoint _serverEndpoint;
        private Socket _socket;
        private List<byte[]> _requestList = new List<byte[]>();
        private List<String> _msgList;
        private List<Balloon> _followedList = null;
        private int _lastMsgId = -1;

        public event EventHandler<OnReceiveBalloonArgs> _OnReceiveBalloon;
        public event EventHandler<OnReceiveBalloonListArgs> _OnReceiveBalloonList;

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
            Connect();
        }

        // Connect to server
        public bool Connect()
        {
            try
            {
                this._socket.Connect(this._serverEndpoint);
                Console.WriteLine("Connected");

                return true;
            }
            catch (Exception err)
            {
                // Print exception on console
                Console.WriteLine("Could not connect to server : {0}", err);

                return false;
            }
        }

        // Close connection
        public void Close(bool reconnect = true)
        {
            this._socket.Shutdown(SocketShutdown.Both);
            if (reconnect == true)
                this._socket.Disconnect(true);
            else
                this._socket.Close();
        }

        // Set device ID using TelephonyManager
        private string getDeviceID()
        {
            var telephonyDeviceID = string.Empty;
            var telephonySIMSerialNumber= string.Empty;
            TelephonyManager telephonyManager = (TelephonyManager)Application.Context.GetSystemService(Context.TelephonyService);
            if (telephonyManager != null)
            {
                if (!string.IsNullOrEmpty(telephonyManager.DeviceId))
                    telephonyDeviceID = telephonyManager.DeviceId;
                if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber))
                    telephonySIMSerialNumber = telephonyManager.SimSerialNumber;
            }
            var androidID = Android.Provider.Settings.Secure.GetString(
                Application.Context.ContentResolver,
                Android.Provider.Settings.Secure.AndroidId);
            var deviceUuid = new UUID(androidID.GetHashCode(), ((telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode()));

            return telephonyDeviceID;
        }

        // Set the _followedList list
        public bool SyncWithServer()
        {
            //Console.WriteLine("Sync with server");
            return AddToRequestList(1, ByteBuffer.Allocate(0));
        }

        // Prepare location request content
        public bool SetLocation(double lon, double lat)
        {
            ByteBuffer data = ByteBuffer.Allocate(16);
            data.PutDouble(0, lon);
            data.PutDouble(8, lat);

            return AddToRequestList(3, data);
        }

        // Prepare grab request content
        public bool SetGrab(long id, short type = 4)
        {
            ByteBuffer data = ByteBuffer.Allocate(8);
            data.PutLong(0, id);

            return AddToRequestList(type, data);
        }

        // Prepare follow request content
        public bool SetFollow(long id)
        {
            ByteBuffer data = ByteBuffer.Allocate(8);
            data.PutLong(0, id);

            return AddToRequestList(5, data);
        }

        // Prepare unfollow request content
        public bool SetUnfollow(long id)
        {
            ByteBuffer data = ByteBuffer.Allocate(8);
            data.PutLong(0, id);

            return AddToRequestList(6, data);
        }

        // Prepare balloon creation request content
        public bool SetNewBalloon(double lon, double lat, string title, string content)
        {
            if (title.Length >= 16)
            {
                Console.WriteLine("Title is too long");
                return false;
            }
            Console.WriteLine("title : {0}", title);
            ByteBuffer data = ByteBuffer.Allocate(40 + content.Length + 10); // Added 10 here because it wouldn't generate without it
            byte[] titleArray = new byte[16];
            byte[] msgArray = Encoding.UTF8.GetBytes(content);
            titleArray = Encoding.UTF8.GetBytes(title);
            data.Position(0);
            data.Put(ByteBuffer.Wrap(titleArray));
            data.PutDouble(16, lon);
            data.PutDouble(24, lat);
            data.PutInt(32, content.Length);
            data.Position(40);
            data.Put(ByteBuffer.Wrap(msgArray));

            return AddToRequestList(7, data);
        }

        public bool SetAnswerBalloon(double lon, double lat, long id, string content)
        {
            ByteBuffer data = ByteBuffer.Allocate(32 + content.Length + 10);
            byte[] msgArray = Encoding.UTF8.GetBytes(content);
            data.PutLong(0, id);
            data.PutDouble(8, lon);
            data.PutDouble(16, lat);
            data.PutInt(24, content.Length);
            data.Position(32);
            data.Put(ByteBuffer.Wrap(msgArray));
            
            return AddToRequestList(8, data);
        }
        private bool SetAcknowledgement(int type, int flag)
        {
            ByteBuffer data = ByteBuffer.Allocate(8);
            data.PutInt(0, type);
            data.PutInt(4, flag);

            return AddToRequestList(9, data);
        }

        // Create buffer and set its header
        private ByteBuffer setHeader(short type, int nbPackets, int id, short size)
        {
            ByteBuffer buffer = ByteBuffer.Allocate(1024);
            buffer.PutShort(0, size);
            buffer.PutShort(2, type);
            buffer.PutInt(8, nbPackets);
            buffer.PutInt(12, id);
            byte[] deviceId = Encoding.ASCII.GetBytes(getDeviceID());
            buffer.Position(20);
            buffer.Put(ByteBuffer.Wrap(deviceId));

            return buffer;
        }
        
        // Prepare the request and add it to the request list
        public bool AddToRequestList(short type, ByteBuffer data)
        {
            List<byte[]> request = new List<byte[]>();
            //Console.WriteLine("Add request to list");
            ByteBuffer buffer;
            int packetSize;
            data.Position(0);
            int contentSize = data.Remaining();
            int nbPackets = (contentSize == 0 ? 1 : contentSize / (1024 - 56)); // 56 = header size
            if (contentSize % (1024 - 56) > 0)
                nbPackets += 1;
            //Console.WriteLine("nb packet : {0}", nbPackets);
            for (int i = 0; i < nbPackets; i++)
            {
                if (contentSize - ((i + 1) * (1024 - 56)) >= (1024 - 56))
                    packetSize = 1024;
                else
                    packetSize = 56 + (contentSize - (i * (1024 - 56)));
                buffer = setHeader(type, nbPackets, i, (short)packetSize);
                buffer.Position(56);
                data.Position(0);
                buffer.Put(data);
                byte[] res = new byte[1024];
                buffer.Position(0);
                buffer.Get(res);
                try 
	            {
                    request.Add(res);
	            }
	            catch (Exception err)
	            {
		            Console.WriteLine("Error while adding request to list : {0}", err);
                    return false;
            	}
            }
            _requestList.AddRange(request);
            return true;
        }

        private void printRequest(byte[] request)
        {
            ByteBuffer buffer = ByteBuffer.Wrap(request);
            short type = buffer.GetShort(2);
            //Console.Write("Size : {0}\nType : {1}\nNbPackets : {2}\nID packet : {3}\n", buffer.GetShort(0), type, buffer.GetInt(8), buffer.GetInt(12));
            switch (type)
            {
                case 3:
                    Console.Write("Longitude : {0}\nLatitude : {1}\n", buffer.GetDouble(56), buffer.GetDouble(64));
                    break;
                default:
                    break;
            }
        }

        // Send and receive algorithm between client and server
        public void StartLoop()
        {
            while (true)
            {
                if (_requestList.Count > 0 && _requestList[0] != null)
                {
                    try
                    {
                        byte[] readArray = new byte[1024];
                        printRequest(_requestList[0]);
                        Console.Write("sending : ");
                        for (int i = 0; i < 1024; i++)
                        {
                            if (i == 56 || i == 56 + 16 || i == 56 + 16 + 8 || i == 56 + 16 + 8 + 8)
                                Console.Write("|");
                            Console.Write(_requestList[0][i]);
                        }
                        int bytesSent = this._socket.Send(_requestList[0]);
                        Console.WriteLine("{0} bytes sent.", bytesSent);
//                        Thread.Sleep(5000);
                        int bytesReceived = this._socket.Receive(readArray);
                        Console.WriteLine("{0} bytes received.", bytesReceived);
                        Console.Write("received : ");
                        int j = 0;
                        for (int i = 0; i < 1024; i++)
                        {
                            if (readArray[i] == 0)
                                j++;
                            Console.Write(readArray[i]);
                        }
/*                        if (j == 1024)
                        {
                            Console.WriteLine("previous packet was empty");
                            int bReceived = this._socket.Receive(readArray);
                            Console.WriteLine("{0} bytes received in new packet.", bReceived);
                        }
*/                        Console.Write("\n");
                        ParseResponse(ByteBuffer.Wrap(readArray));
                        if (_requestList.Count > 0)
                            _requestList.RemoveAt(0);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("Error while exchanging data with server : {0}.", err);
                        return;
                    }
                }
            }
        }

        private void ParseResponse(ByteBuffer buffer)
        {
            short type = buffer.GetShort(2);
            Console.WriteLine("Response type : {0}", type);
            switch (type)
            {
                case 1:
                    // List of followed balloons.
                    CreateFollowedList(buffer);
                    break;
                case 2:
                    // Balloon content
                    FillBalloon(buffer);
                    break;
                case 3:
                    // List of nearby balloons.
                    CreateBalloonList(buffer);
                    break;
                case 4:
                    // Balloon content
                    FillBalloon(buffer);
                    break;
                case 32767 :
                    // Acknowledgement!
                    TreatAcknowledgement(buffer);
                    break;
                default:
                    Console.WriteLine("UNKNOWN TYPE {0}", type);
                    break;
            }
        }

        private void TreatAcknowledgement(ByteBuffer buffer)
        {
            short type = (short)buffer.GetInt(16);
            int flag = buffer.GetInt(20);
            Console.WriteLine("type : {0}, flag : {1}", type, flag);
            if (flag == 0)
            {
                for (int i = 0; i < _requestList.Count; i++)
                {
                    ByteBuffer request = ByteBuffer.Wrap(_requestList[i]);
                    if (request.GetShort(2) == type)
                        _requestList.RemoveAt(i);
                }
            }
            if (flag == 1)
            {
                if (type == 5)
                {
                    _followedList.Add(new Balloon((UInt64)buffer.GetLong(24), null));
                }
//                if (type == 6)
//                {
//                    _followedList.RemoveAll(x => x.Id == (UInt64)buffer.GetLong(24));
//                }
            }
        }
        private void CreateFollowedList(ByteBuffer buffer)
        {
            int nbPackets = buffer.GetInt(8);
            int PacketId = buffer.GetInt(12);
            int nbBalloons = buffer.GetInt(16);
//            Console.WriteLine("nb balloons : {0}", nbBalloons);
            if (nbBalloons == 0)
                return;
            int byteIndex = 24;
            if (PacketId == 0)
                _followedList = new List<Balloon>();
            Balloon toto = _followedList.Find(x => x.Id == (UInt64)buffer.GetLong(byteIndex));
            if (toto == null)
                _followedList.Add(new Balloon((UInt64)buffer.GetLong(byteIndex), false));
            else
                _followedList.Add(new Balloon((UInt64)buffer.GetLong(byteIndex), true));
            if (nbPackets == PacketId + 1)
            {
                foreach (var balloon in _followedList)
                {
                    SetGrab((long)balloon.Id, 2);
                }
            }
            else
                SetAcknowledgement(1, 1);
        }

        private void FillBalloon(ByteBuffer buffer)
        {
            long balloonId = buffer.GetLong(16);
            int firstId = buffer.GetInt(32);
            if (firstId == 0)
            {
                _msgList = new List<String>();
            }
            int nbMessages = buffer.GetInt(28);
            Console.WriteLine("nb messages : {0}", nbMessages);            
            int byteIndex = 32;
            int msgLength;
            for (int i = 0; i < nbMessages; i++)
            {
                msgLength = buffer.GetInt(byteIndex + 4);
                buffer.Position(byteIndex + 16);
                if (msgLength > buffer.Remaining())
                    msgLength = buffer.Remaining();
                byte[] msgArray = new byte[msgLength];
                buffer.Get(msgArray);
                if (i == 0 && firstId == _lastMsgId)
                {
                    _msgList[_msgList.Count - 1] += Encoding.UTF8.GetString(msgArray);
                }
                else
                {
                    _msgList.Add(Encoding.UTF8.GetString(msgArray));
                }
            }
            _lastMsgId = buffer.GetInt(byteIndex - 16);
            if (buffer.GetInt(8) == buffer.GetInt(12) + 1)
            {
                int j = 0;
                if (_followedList != null)
                {
                    for (j = 0; j < _followedList.Count && _followedList[j].Id == (UInt64)balloonId; j++) { }
                }
                if (_followedList != null && j < _followedList.Count)
                {
                    _followedList[j].Messages = _msgList;
                    _OnReceiveBalloon.Invoke(this, new OnReceiveBalloonArgs(_followedList[j]));
                }
                else
                {
                    Console.WriteLine("Create balloon");
                    Balloon balloon = new Balloon((UInt64)balloonId, _msgList);
                    try
                    {
                        _OnReceiveBalloon.Invoke(this, new OnReceiveBalloonArgs(balloon));
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("catched : {0}", err);
                    }
                }
                _msgList.Clear();
            }
            else
            {
                Console.WriteLine("SetAcknowledgenent");
                SetAcknowledgement(2, 1);
            }
            Console.WriteLine("end of fill balloon method");
        }

        // Create balloon list from server response
        private void CreateBalloonList(ByteBuffer buffer)
        {
            Console.WriteLine("Creating balloon list");
            String title;
            int j;
            byte[] titleArray = new byte[1024];
            int byteIndex = 24;
            int nbBalloons = buffer.GetInt(16);
            Console.WriteLine("nb balloons : {0}", nbBalloons);
            List<Balloon> balloonList = new List<Balloon>(nbBalloons);
            for (int i = 0; i < nbBalloons; i++)
            {
                Console.WriteLine("Baloon position = {0}, {1}", buffer.GetDouble(byteIndex + 32), buffer.GetDouble(byteIndex + 24));
                buffer.Position(byteIndex + 8);
                buffer.Get(titleArray, 0, 16);
                title = Encoding.UTF8.GetString(titleArray);
                for (j = 0; title[j] != 0; j++) { }
                title = title.Remove(j);
                balloonList.Add(new Balloon(
                    null,
                    title,
                    (UInt64)buffer.GetLong(byteIndex),
                    buffer.GetDouble(byteIndex + 32),
                    buffer.GetDouble(byteIndex + 24),
                    false,
                    buffer.GetDouble(byteIndex + 48),
                    buffer.GetDouble(byteIndex + 40)));
                byteIndex += 56;
            }
          _OnReceiveBalloonList.Invoke(this, new OnReceiveBalloonListArgs(balloonList));
        }
    }
}