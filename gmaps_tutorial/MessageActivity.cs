using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Util;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace gmaps_tutorial
{
    [Activity(Label = "Message", MainLauncher = false, Icon = "@drawable/icon")]
    public class MessageActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.messageBaloon);
        }
    }
}