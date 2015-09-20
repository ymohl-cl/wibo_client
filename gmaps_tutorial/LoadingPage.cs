using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.OS;
using Android.Locations;
using Android.Util;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Newtonsoft.Json;


namespace wibo
{
    [Activity(Label = "wibo", MainLauncher = true, Icon = "@drawable/ic_launcher", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoadingPage : Activity
    {
        private Connection _connection;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.loadingPage);

			_connection = new Connection();
            //Loop d'echange avec le serveur
            ThreadPool.QueueUserWorkItem(o => this._connection.StartLoop());
		
            //Remplissage de la liste des ballons suivis.
            Console.WriteLine("Sync with server");
            //_connection.SyncWithServer();
            var mainActivity = new Intent(this, typeof(MainActivity));
            string jsonConnection = JsonConvert.SerializeObject(_connection, Formatting.Indented);
            mainActivity.PutExtra("Connection", jsonConnection);
            StartActivity(mainActivity);
        }
    }
}