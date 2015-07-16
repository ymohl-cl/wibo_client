using System;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;

using Java.Nio;

namespace serverConnection
{
    [Activity(Label = "serverConnection", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Connection _soc;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.sendButton);

            // Open socket
            _soc = new Connection();

            //Connect to server (Connect() method returns a boolean value)
            _soc.Connect();

            ThreadPool.QueueUserWorkItem(o => this._soc.Receive());

            button.Click += delegate {
                // Check if socket in connected to server
                if (_soc.Connected == true)
                {
                    _soc.SendLocation(42.2f, -42.2f);
                    _soc.SendBalloon(3.3f, 5.5f, "test", "Ceci est un ballon de test");
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
    }
}