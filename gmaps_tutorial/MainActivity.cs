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
    [Activity(Label = "wibo", Icon = "@drawable/ic_launcher", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity, IOnMapReadyCallback, ILocationListener
    {
        private LocationManager locMgr;
        private Connection _connection;
        private GoogleMap _map;
        private Location _currentLocation;
        private LatLng _locationLatLng;
        private BitmapDescriptor redBalloon;
        private List<Balloon> _followedBalloons;
        private List<Balloon> _catchedBalloons;
        private List<Balloon> _nearbyBalloons;
        private List<String> _testMessages;
        private bool _isGooglePlayServicesInstalled;
        private ImageButton createBalloonButton;
        private Button followedBalloonsMenuButton;
        private List<Marker> _balloonMarkers;
        private RelativeLayout _layout;
        private List<MarkerOptions> _markersOptions;
        private EventWaitHandle ewh1 = new EventWaitHandle(false, EventResetMode.AutoReset);
        private EventWaitHandle ewh2 = new EventWaitHandle(false, EventResetMode.AutoReset);

        private double _tmpLon = 48.833086;
        private double _tmpLat = 2.310655;

        //On create is called when the activity is created
		protected override void OnCreate(Bundle bundle) 
        {
            base.OnCreate(bundle);
			Console.WriteLine ("Before setContentView");
			SetContentView (Resource.Layout.Main);
			Console.WriteLine ("Before setContentView");
            //Get the connection object from the LoadingPage Activity
            string jsonConnection = Intent.GetStringExtra("Connection");
            _connection = JsonConvert.DeserializeObject<Connection>(jsonConnection);

           // Window.RequestFeature(WindowFeatures.NoTitle);
            // Set our view from the "main" layout resource
            //redBalloon = BitmapDescriptorFactory.FromResource(Resource.Drawable.ballon_de_base_cote_map_petit);

            // get nearest ballons and followed Balloons
            _nearbyBalloons = new List<Balloon>();
            _testMessages = new List<String>();
            _followedBalloons = new List<Balloon>();
            _catchedBalloons = new List<Balloon>();
            _balloonMarkers = new List<Marker>();
            _markersOptions = new List<MarkerOptions>();

            
			_testMessages.Add("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur vitae est eu ante molestie aliquam ac at ligula. Nulla justo nisi, pharetra et facilisis ut, congue et felis. Cras commodo justo sed erat porttitor, at porta ligula euismod. Curabitur non molestie arcu.");
            _testMessages.Add("Nulla sed luctus magna. Nulla diam nunc, scelerisque ac elementum eget, fermentum ut nulla. Cras vestibulum, sapien eu aliquam dignissim, sapien arcu luctus odio, eu rutrum elit sapien nec urna. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae.");
            _testMessages.Add(" Maecenas in sem ut nisl rhoncus rutrum. Fusce placerat neque iaculis, faucibus turpis quis, pretium arcu. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Ut tincidunt, elit rhoncus maximus hendrerit, ex nunc finibus nunc, ut commodo libero diam non nulla. Quisque eu sollicitudin turpis, sed convallis nulla");
            _testMessages.Add("Aliquam tempus, mauris venenatis facilisis venenatis, libero magna vulputate magna, in congue arcu sapien nec tellus. Sed in nibh et erat efficitur imperdiet. Aliquam efficitur tincidunt erat vitae pretium. ");
            _testMessages.Add("Integer lorem enim, auctor at diam nec, consequat vulputate neque. Nullam eget accumsan quam. Mauris ultricies nisi sapien, finibus fermentum libero semper at. Aliquam sed consectetur leo. Donec egestas nisl quis est tempor, eget rhoncus nunc scelerisque. Nulla rhoncus ut eros sed accumsan. Proin nibh elit, iaculis nec dolor at, imperdiet feugiat elit. Donec blandit dolor ut sagittis aliquam.");
            _testMessages.Add("Ut ut pharetra ante. Nullam mauris nulla, laoreet et volutpat vel, gravida sit amet massa. Integer purus enim, luctus eget ultrices sed, dictum id erat.");
            _testMessages.Add(" Nulla laoreet justo risus. Interdum et malesuada fames ac ante ipsum primis in faucibus. Vivamus lacus velit, dapibus vel sollicitudin eu, fermentum eu ex. Donec ornare odio dui, eu aliquam arcu gravida quis. Maecenas sit amet enim a ligula vehicula gravida nec ultrices felis. ");
            _testMessages.Add("Integer vitae arcu vel lectus interdum cursus vel sed ligula. Donec vel tellus vitae orci ultricies consectetur. Aenean tempus arcu vitae arcu faucibus tincidunt. Proin maximus ultrices mauris. Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec finibus, sapien nec commodo suscipit, nunc quam tempor ex, et iaculis lectus odio et tellus. Nunc non velit porta, maximus risus commodo, volutpat neque. ");
            _testMessages.Add("Nunc molestie, lacus eget vulputate euismod, est augue tincidunt purus, nec bibendum ligula ipsum ut lectus. Aliquam ac mollis mi. In finibus auctor congue. Pellentesque lobortis nulla nec pellentesque viverra. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.");
            _testMessages.Add("Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus leo dui, imperdiet dictum neque tristique, sollicitudin tincidunt arcu. Phasellus cursus tristique urna id facilisis. Nullam ut elit ante. Suspendisse quis lacus eu quam dapibus pharetra a sit amet diam. ");
            _testMessages.Add("In hac habitasse platea dictumst. Interdum et malesuada fames ac ante ipsum primis in faucibus");
            _testMessages.Add("Maecenas nec erat ac ligula iaculis fermentum. Aliquam venenatis mollis augue, ac mollis nisi suscipit id. Donec ut lobortis nunc, iaculis aliquam ante. Etiam maximus sagittis arcu, id pretium est imperdiet in. Vivamus ultricies urna quis vehicula imperdiet. Nulla lacus urna, pellentesque sed semper id, pulvinar at risus. Ut quis sem congue, pretium ante ac, ultrices massa. Sed eleifend laoreet orci, et varius sapien consectetur non. Aliquam tempus sollicitudin tempor. Donec tristique eros eu turpis porttitor tincidunt. ");
            
            _nearbyBalloons.Add(new Balloon(_testMessages, "catched1", 88, 48.833086, 2.310655, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched2", 82, 48.8628437, 2.3252016, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched3", 83, 48.87840, 2.35408, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched4", 14, 48.8873, 2.352, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched5", 100, 48.84073, 2.3572, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched6", 35, 48.878473, 2.3544, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched7", 93, 48.87073, 2.372, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched8", 52, 48.8768, 2.35, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched9", 11, 48.8643, 2.40572, false, 2.0, 35.6));
             _nearbyBalloons.Add(new Balloon(_testMessages, "catched10", 1, 48.8799, 2.3599, false, 2.0, 35.6));
            
            
            _followedBalloons.Add(new Balloon(_testMessages, "test1", 88, 48.8784073, 2.3540572, true, 2.0, 35.6));
            _followedBalloons.Add(new Balloon(_testMessages, "test2", 82, 48.8628437, 2.3252016, true, 2.0, 35.6));
            _followedBalloons.Add(new Balloon(_testMessages, "test3", 83, 48.87840, 2.35408, true, 2.0, 35.6));
           _followedBalloons.Add(new Balloon(_testMessages, "test4", 14, 48.8873, 2.352, true, 2.0, 35.6));
            _followedBalloons.Add(new Balloon(_testMessages, "test5", 100, 48.84073, 2.3572, true, 2.0, 35.6));
            _followedBalloons.Add(new Balloon(_testMessages, "test6", 35, 48.878473, 2.3544, true, 2.0, 35.6));
            _followedBalloons.Add(new Balloon(_testMessages, "test7", 93, 48.87073, 2.372, true, 2.0, 35.6));
            _followedBalloons.Add(new Balloon(_testMessages, "test8", 52, 48.8768, 2.35, true, 2.0, 35.6));
            _followedBalloons.Add(new Balloon(_testMessages, "test9", 11, 48.8643, 2.40572, true, 2.0, 35.6));
            _followedBalloons.Add(new Balloon(_testMessages, "test10", 1, 48.8799, 2.3599, true, 2.0, 35.6));
            

            _connection._OnReceiveFollowedList += (o, s) =>
            {
                _followedBalloons = s.Response;
            };
            createBalloonButton = FindViewById<ImageButton>(Resource.Id.createBalloonButton);
            createBalloonButton.Click += createBalloonButton_Click;
            followedBalloonsMenuButton = FindViewById<Button>(Resource.Id.followedBalloonsMenuButton);
            followedBalloonsMenuButton.Click += followedBalloonsMenuButton_Click;

            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();
            if (!_isGooglePlayServicesInstalled)
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
                Finish();
            }
        }

        private void moveBalloonsOnMap()
        {
        }

        //Is Called when app is back on foreground or when the return button is pressed on a child activity
        protected override void OnResume()
        {
            base.OnResume();
            _connection.SetLocation(_tmpLon, _tmpLat, true);
            //Set up the map
            //SetUpMap();
//            var locationCriteria = new Criteria();
//            locationCriteria.Accuracy = Accuracy.Coarse;
//            locationCriteria.PowerRequirement = Power.Medium;
//            string locationProvider = locMgr.GetBestProvider(locationCriteria, true);
//            locMgr.RequestLocationUpdates(locationProvider, 2000, 1, this);
            ThreadPool.QueueUserWorkItem(o => moveBalloonsOnMap());
            /*
            Log.Debug("OnResume", "OnResume called, connecting to client...");
            if (!apiClient.IsConnected)
            {
                apiClient.Connect();
            }*/
        }

        public void removeFromMap(Marker marker)
        {
            Console.WriteLine("removing from map");
            RunOnUiThread(() =>
            {
                marker.Remove();
            });
        }

        private void SetUpMap()
        {

            if (_map == null)
            {
                //Recupere la map via l'api google
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
            else
            {
                _balloonMarkers.ForEach(removeFromMap);
                _balloonMarkers.Clear();
            }

        }

        //Fonction appelée en callback de GetMapAsync
        public void OnMapReady(GoogleMap googleMap)
        {

            Console.WriteLine("Map is ready to be used");
            _map = googleMap;
            _map.UiSettings.SetAllGesturesEnabled(false);
            _map.MapType = GoogleMap.MapTypeTerrain;
            _map.MarkerClick += MapOnMarkerClick;

            //Send a request to retrieve nearby balloons
            Console.WriteLine("In OnMapReady.");
            ThreadPool.QueueUserWorkItem(o => _connection.SetLocation(_tmpLon, _tmpLat));
            _connection._OnReceiveBalloonList += (o, s) =>
            {
                Console.WriteLine("List of nearby baloons received.");
                _nearbyBalloons = s.Response;
                if (_balloonMarkers.Count > 0)
                {
                    _balloonMarkers.ForEach(removeFromMap);
                    _balloonMarkers.Clear();
                }
                foreach (Balloon balloon in _nearbyBalloons)
                {
                    MarkerOptions optionsBalloon = new MarkerOptions()
                   .SetPosition(new LatLng(balloon.LatPosition, balloon.LngPosition))
                   .SetTitle(balloon.Id.ToString())
                   .SetIcon(redBalloon);
                    RunOnUiThread(() =>
                    {
                        Marker marker = _map.AddMarker(optionsBalloon);
                        marker.HideInfoWindow();
                        _balloonMarkers.Add(marker);
                    });
                }
            };
            // Set user's location
            _locationLatLng = new LatLng(_tmpLon, _tmpLat);
            //move the camera to the location of the user
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(_locationLatLng, 15);
            _map.MoveCamera(camera);
        }

        void followedBalloonsMenuButton_Click(object sender, EventArgs e)
        {
            var menuFollowedBalloonActivity = new Intent(this, typeof(MenuFollowedBalloonActivity));
            string jsonFollowedBalloons = JsonConvert.SerializeObject(_followedBalloons, Formatting.Indented);
            string jsonCatchedBalloons = JsonConvert.SerializeObject(_catchedBalloons, Formatting.Indented);
            menuFollowedBalloonActivity.PutExtra("Followed Balloons", jsonFollowedBalloons);
            menuFollowedBalloonActivity.PutExtra("Catched Balloons", jsonCatchedBalloons);
            StartActivityForResult(menuFollowedBalloonActivity, 2);
        }

        void createBalloonButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            CreateBalloonFragment createBalloonFragment = new CreateBalloonFragment();
            createBalloonFragment.sendBalloonEvent += createBalloonFragment_sendBalloon;
            createBalloonFragment.Show(transaction, "Create Balloon");
        }

        private void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            Marker marker;
            UInt64 idBalloon;
            Balloon balloon;

            markerClickEventArgs.Handled = true;
            Console.WriteLine("Event : MaponMarkerClick");
            marker = markerClickEventArgs.Marker;
            UInt64.TryParse(marker.Title, out idBalloon);
            balloon = _nearbyBalloons.Find(x => x.Id == idBalloon);
            _connection.SetGrab((long)idBalloon);
            _connection._OnReceiveBalloon += (o, s) =>
            {
                balloon.Messages = s.Response.Messages;
                ewh1.Set();
            };
            ewh1.WaitOne();
            String jsonBalloon = JsonConvert.SerializeObject(balloon);
            balloon.Catched = true;
            //string jsonBalloon = JsonConvert.SerializeObject(balloon, Formatting.Indented);
            var seeAnswerableBalloonContentActivity = new Intent(this, typeof(SeeBalloonContentActivity));
            seeAnswerableBalloonContentActivity.PutExtra("Balloon", jsonBalloon);
            seeAnswerableBalloonContentActivity.PutExtra("type", 0);
            StartActivityForResult(seeAnswerableBalloonContentActivity, 1);
        }

        private String serializeBalloon(Balloon balloonToSerialize)
        {
            string json = JsonConvert.SerializeObject(balloonToSerialize);
            return json;
        }

        //Is called when the return button is pressed or on the exit of the app
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }


        //Is called when a new Activity starts and when the app is set to background
        protected override void OnPause()
        {
            base.OnPause();
//            locMgr.RemoveUpdates(this);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            UInt64 id;
            Boolean followed;
            Boolean catched;

            Console.WriteLine("OnActivityResult called, resultCode :{0}, requestCode :{1}", resultCode, requestCode);
            //Return value from a followed balloon activity
            if (requestCode == 1)
            {
                if (data.HasExtra("balloonId"))
                {
                    UInt64.TryParse(data.GetStringExtra("balloonId"), out id);
                    Boolean.TryParse(data.GetStringExtra("followed"), out followed);
                    Boolean.TryParse(data.GetStringExtra("resend"), out catched);
                    Console.WriteLine(id);
                    Balloon balloon = _nearbyBalloons.Find(x => x.Id == id);
                    Console.WriteLine(JsonConvert.SerializeObject(_nearbyBalloons));
                    Console.WriteLine(balloon);
                    Console.WriteLine(followed);
                    Console.WriteLine(catched);
                    balloon.Followed = followed;
                    if (data.HasExtra("message"))
                    {
                        balloon.Messages.Add(data.GetStringExtra("message"));
                    }
                    if (catched)
                    {
                        _catchedBalloons.Add(balloon);
                    }
                    else
                    {
                        if (followed)
                            _followedBalloons.Add(balloon);
                    }
                    _nearbyBalloons.Remove(balloon);
                }
            }
            if (requestCode == 2)
            {
                if (data.HasExtra("FollowedBalloons"))
                {
                    string jsonFollowedBalloons = data.GetStringExtra("FollowedBalloons");
                    _followedBalloons = JsonConvert.DeserializeObject<List<Balloon>>(jsonFollowedBalloons);
                }
                if (data.HasExtra("CatchedBalloons"))
                {
                    string jsonCatchedBalloons = data.GetStringExtra("CatchedBalloons");
                    _catchedBalloons = JsonConvert.DeserializeObject<List<Balloon>>(jsonCatchedBalloons);
                }
            }
        }

        private void createBalloonFragment_sendBalloon(object sender, SendBalloonEvent e)
        {
            //get datas from the balloon creation and send to server.
            _connection.SetNewBalloon(_tmpLon, _tmpLat, e.TitleBalloon, e.MessageBalloon);
        }

        bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("ManActivity", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);

                // Show error dialog to let user debug google play services
            }
            return false;
        }

        public void OnLocationChanged(Location location)
        {
            _locationLatLng.Latitude = location.Latitude;
            _locationLatLng.Longitude = location.Longitude;
            _connection.SetLocation(_locationLatLng.Longitude, _locationLatLng.Latitude, true);
            if (_map != null)
            {
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(_locationLatLng, 15);
                _map.MoveCamera(camera);
            }
        }

        public void OnProviderDisabled(string provider)
        {
            Console.WriteLine("Provider Disabled");
        }

        public void OnProviderEnabled(string provider)
        {
            Console.WriteLine("Provider Enabled");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Console.WriteLine("Status for localisation changed");
        }

        public override void OnBackPressed()
        {
            System.Environment.Exit(0);
        }
    }
}