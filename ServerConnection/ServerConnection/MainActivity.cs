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

using Newtonsoft.Json;

namespace ServerConnection
{
    [Activity(Label = "ServerConnection", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Connection _connection;

/*        protected override void OnDestroy()
        {
            base.OnDestroy();
            _connection.Close(false);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _connection.Close(true);
        }

        protected override void OnResume()
        {
            base.OnResume();
            _connection.Connect();
        }
*/
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            Button button1 = FindViewById<Button>(Resource.Id.button1);
            Button button3 = FindViewById<Button>(Resource.Id.button3);
            Button button4 = FindViewById<Button>(Resource.Id.button4);
            Button button5 = FindViewById<Button>(Resource.Id.button5);
            Button button6 = FindViewById<Button>(Resource.Id.button6);
            Button button7 = FindViewById<Button>(Resource.Id.button7);
            Button button8 = FindViewById<Button>(Resource.Id.button8);

            // Create socket
            _connection = new Connection();

            // Start exchange loop in another thread
            ThreadPool.QueueUserWorkItem(o => this._connection.StartLoop());

            //_connection.SyncWithServer();

            // On button1 click, 
            button1.Click += delegate {
                _connection.SyncWithServer();
            };

            // On button3 click
            button3.Click += delegate {
                ThreadPool.QueueUserWorkItem(o => this._connection.SetLocation(48.833086, 2.310655));
            };

            // On button4 click
            button4.Click += delegate {
                ThreadPool.QueueUserWorkItem(o => this._connection.SetGrab(4));
            };

            button5.Click += delegate
            {
                ThreadPool.QueueUserWorkItem(o => this._connection.SetFollow(4));
            };

            button6.Click += delegate
            {
                ThreadPool.QueueUserWorkItem(o => this._connection.SetUnfollow(4));
            };

            button7.Click += delegate
            {
                ThreadPool.QueueUserWorkItem(o => this._connection.SetNewBalloon(48.833086, 2.310655, "TOTO", "Ceci est un test"));
            };

            button8.Click += delegate
            {
                ThreadPool.QueueUserWorkItem(o => this._connection.SetAnswerBalloon(48.833086, 2.310655, 4, "Ceci est une reponse test"));
            };

            _connection._OnReceiveBalloonList += (o, s) =>
            {
                string json = JsonConvert.SerializeObject(s);
                Console.WriteLine(json);
            };

            _connection._OnReceiveBalloon += (o, s) =>
            {
                string json = JsonConvert.SerializeObject(s);
                Console.WriteLine(json);
            };
        }
    }
}