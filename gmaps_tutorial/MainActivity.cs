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
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Newtonsoft.Json;

namespace wibo
{
    [Activity(Label = "wibo", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity, IOnMapReadyCallback, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
    {
        private IGoogleApiClient apiClient;
        private GoogleMap _map;
        private Location _currentLocation;
        private LatLng _locationLatLng;
        private BitmapDescriptor redBaloon;
        private ulong lastTime;
        private List<Baloon> _followedBaloons;
        private List<Baloon> _catchedBaloons;
        private List<Baloon> _nearbyBaloons;
        private List<String> _testMessages;
        private bool _isGooglePlayServicesInstalled;
        private ImageButton createBaloonButton;
        private Button followedBaloonsMenuButton;
        private List<Marker> _baloonMarkers;
        private RelativeLayout _layout;

        //On create is called when the activity is created
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //Window.RequestFeature(WindowFeatures.NoTitle);
            // Set our view from the "main" layout resource
            //redBaloon = BitmapDescriptorFactory.FromResource(Resource.Drawable.ballon_de_base_cote_map_petit);

            // get nearest ballons and followed Baloons
            _nearbyBaloons = new List<Baloon>();
            _testMessages = new List<String>();
            _followedBaloons = new List<Baloon>();
            _catchedBaloons = new List<Baloon>();
            _baloonMarkers = new List<Marker>();

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

            _nearbyBaloons.Add(new Baloon(_testMessages, "catched1", 88, 48.8784073, 2.3540572, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched2", 82, 48.8628437, 2.3252016, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched3", 83, 48.87840, 2.35408, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched4", 14, 48.8873, 2.352, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched5", 100, 48.84073, 2.3572, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched6", 35, 48.878473, 2.3544, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched7", 93, 48.87073, 2.372, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched8", 52, 48.8768, 2.35, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched9", 11, 48.8643, 2.40572, false, 2.0, 35.6));
            _nearbyBaloons.Add(new Baloon(_testMessages, "catched10", 1, 48.8799, 2.3599, false, 2.0, 35.6));

            _followedBaloons.Add(new Baloon(_testMessages, "test1", 88, 48.8784073, 2.3540572, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test2", 82, 48.8628437, 2.3252016, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test3", 83, 48.87840, 2.35408, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test4", 14, 48.8873, 2.352, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test5", 100, 48.84073, 2.3572, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test6", 35, 48.878473, 2.3544, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test7", 93, 48.87073, 2.372, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test8", 52, 48.8768, 2.35, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test9", 11, 48.8643, 2.40572, true, 2.0, 35.6));
            _followedBaloons.Add(new Baloon(_testMessages, "test10", 1, 48.8799, 2.3599, true, 2.0, 35.6));

            createBaloonButton = FindViewById<ImageButton>(Resource.Id.createBaloonButton);
            createBaloonButton.Click += createBaloonButton_Click;
            followedBaloonsMenuButton = FindViewById<Button>(Resource.Id.followedBaloonsMenuButton);
            followedBaloonsMenuButton.Click += followedBaloonsMenuButton_Click;

            /*
            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();  
            if (_isGooglePlayServicesInstalled)
            {
                // pass in the Context, ConnectionListener and ConnectionFailedListener
                apiClient = new GoogleApiClientBuilder(this, this, this)
                    .AddApi(LocationServices.API).Build();
            }
            else
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
                Finish();
            }
            */
        }

        private void moveBaloonsOnMap()
        {
            /*
            double speedPerTick = 0.05;
            double deltaLat =  - x_current;
            delta_y = y_goal - y_current
            goal_dist = sqrt( (delta_x * delta_x) + (delta_y * delta_y) );
            if (dist > speed_per_tick)
            {
    ratio = speed_per_tick / goal_dist;
    x_move = ratio * delta_x ; 
    y_move = ratio * delta_y;
    new_x_pos = x_move + x_current ; 
    new_y_pos = y_move + y_current;
            }
            else
            {
    new_x_pos = x_goal;
    new_y_pos = y_goal;
            }
            */
        }

        //Is Called when app is back on foreground or when the return button is pressed on a child activity
        protected override void OnResume()
        {
            base.OnResume();
            //send a request to re-get nearest balloons then add it to the map and display
            SetUpMap();
            //ThreadPool.QueueUserWorkItem(o => moveBaloonsOnMap());
            /*
            Log.Debug("OnResume", "OnResume called, connecting to client...");
            if (!apiClient.IsConnected)
            {
                apiClient.Connect();
            }*/
        }

        public void removeFromMap(Marker marker)
        {
            marker.Remove();
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
                _baloonMarkers.ForEach(removeFromMap);
                _baloonMarkers.Clear();
                foreach (Baloon baloon in _nearbyBaloons)
                {
                    MarkerOptions optionsBaloon = new MarkerOptions()
                   .SetPosition(new LatLng(baloon.LatPosition, baloon.LngPosition))
                   .SetTitle(baloon.Id.ToString())
                   .SetIcon(redBaloon);
                    Marker marker = _map.AddMarker(optionsBaloon);
                    marker.HideInfoWindow();
                    _baloonMarkers.Add(marker);
                    Console.WriteLine(marker.Id);
                }
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
            //Send a request to retrieve nearby baloons
            foreach (Baloon baloon in _nearbyBaloons)
            {
                MarkerOptions optionsBaloon = new MarkerOptions()
               .SetPosition(new LatLng(baloon.LatPosition, baloon.LngPosition))
               .SetTitle(baloon.Id.ToString())
               .SetIcon(redBaloon);
                Marker marker = _map.AddMarker(optionsBaloon);
                marker.HideInfoWindow();
                _baloonMarkers.Add(marker);
                Console.WriteLine(marker.Id);
            }
            // Then add marker for each near baloons
            _locationLatLng = new LatLng(48.87873, 2.35472);
            //move the camera to the location of the user
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(_locationLatLng, 15);
            _map.MoveCamera(camera);
        }

        void followedBaloonsMenuButton_Click(object sender, EventArgs e)
        {
            var menuFollowedBaloonActivity = new Intent(this, typeof(MenuFollowedBaloonActivity));
            string jsonFollowedBaloons = JsonConvert.SerializeObject(_followedBaloons, Formatting.Indented);
            string jsonCatchedBaloons = JsonConvert.SerializeObject(_catchedBaloons, Formatting.Indented);
            Console.WriteLine(jsonFollowedBaloons);
            Console.WriteLine(jsonCatchedBaloons);
            menuFollowedBaloonActivity.PutExtra("Followed Baloons", jsonFollowedBaloons);
            menuFollowedBaloonActivity.PutExtra("Catched Baloons", jsonCatchedBaloons);
            StartActivityForResult(menuFollowedBaloonActivity, 2);
        }

        void createBaloonButton_Click(object sender, EventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            CreateBaloonFragment createBaloonFragment = new CreateBaloonFragment();
            createBaloonFragment.sendBaloonEvent += createBaloonFragment_sendBaloon;
            createBaloonFragment.Show(transaction, "Create Baloon");
        }

        private void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            Marker marker;
            UInt64 idBaloon;
            Baloon baloon;

            markerClickEventArgs.Handled = true;
            Console.WriteLine("Event : MaponMarkerClick");
            marker = markerClickEventArgs.Marker;
            UInt64.TryParse(marker.Title, out idBaloon);
            baloon = _nearbyBaloons.Find(x => x.Id == idBaloon);
            baloon.Catched = true;
            string jsonBaloon = JsonConvert.SerializeObject(baloon, Formatting.Indented);
            var seeAnswerableBaloonContentActivity = new Intent(this, typeof(SeeAnswerableBaloonContentActivity));
            seeAnswerableBaloonContentActivity.PutExtra("Baloon", jsonBaloon);
            StartActivityForResult(seeAnswerableBaloonContentActivity, 1);
        }

        private String serializeBaloon(Baloon baloonToSerialize)
        {
            string json = JsonConvert.SerializeObject(baloonToSerialize);
            return json;
        }

        //Is called when the return button is pressed or on the exit of the app
        protected override void OnDestroy()
        {
            base.OnDestroy();
            /*
            Log.Debug("OnDestroy", "OnDestroy called, stopping location update...");
            if (apiClient.IsConnected)
            {
                apiClient.Disconnect();
            }
            */
        }

        //Is called when a new Activity starts and when the app is set to background
        protected override void OnPause()
        {
            base.OnPause();
            /*
            Log.Debug("OnPause", "OnPause called, stopping location updates");
            if (apiClient.IsConnected)
            {
                apiClient.Disconnect();
            }
            */
        }

         protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            UInt64 id;
            Boolean followed;
            Boolean catched;

            Console.WriteLine("OnActivityResult called, resultCode :{0}, requestCode :{1}", resultCode, requestCode);
            //Return value from a followed baloon activity
            if (requestCode == 1)
            {
                if (data.HasExtra("baloonId"))
                {
                    UInt64.TryParse(data.GetStringExtra("baloonId"), out id);
                    Boolean.TryParse(data.GetStringExtra("followed"), out followed);
                    Boolean.TryParse(data.GetStringExtra("resend"), out catched);
                    Console.WriteLine(id);
                    Baloon baloon = _nearbyBaloons.Find(x => x.Id == id);
                    Console.WriteLine(JsonConvert.SerializeObject(_nearbyBaloons));
                    Console.WriteLine(baloon);
                    Console.WriteLine(followed);
                    Console.WriteLine(catched);
                    baloon.Followed = followed;
                    if (data.HasExtra("message"))
                    {
                        baloon.Messages.Add(data.GetStringExtra("message"));
                    }
                    if (catched)
                    {
                        _catchedBaloons.Add(baloon);
                    }
                    else
                    {
                        if (followed)
                            _followedBaloons.Add(baloon);
                    }
                    _nearbyBaloons.Remove(baloon);
                }
            }
            if (requestCode == 2)
            {
                if (data.HasExtra("FollowedBaloons"))
                {
                    string jsonFollowedBaloons = data.GetStringExtra("FollowedBaloons");
                    _followedBaloons = JsonConvert.DeserializeObject<List<Baloon>>(jsonFollowedBaloons);
                }
                if (data.HasExtra("CatchedBaloons"))
                {
                    string jsonCatchedBaloons = data.GetStringExtra("CatchedBaloons");
                    _catchedBaloons = JsonConvert.DeserializeObject<List<Baloon>>(jsonCatchedBaloons);
                }
            }
        }

        private void createBaloonFragment_sendBaloon(object sender, SendBaloonEvent e)
        {
            //get datas from the baloon creation and send to server.
        }

        public void OnLocationChanged(Location location)
        {

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

        public View GetInfoWindow(Marker marker)
        {
            return null;
        }

        public void OnConnected(Bundle bundle)
        {
            // This method is called when we connect to the LocationClient. We can start location updated directly form
            // here if desired, or we can do it in a lifecycle method, as shown above
            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now connected to client");
            _currentLocation = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
            if (_currentLocation != null)
            {
                Log.Info("LocationClient", "Now find his location");
                _locationLatLng = new LatLng(43, 2);
                //move the camera to the location of the user
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(_locationLatLng, 10);
                _map.MoveCamera(camera);
            }
        }

        public void OnDisconnected()
        {
            // This method is called when we disconnect from the LocationClient.
            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now disconnected from client");
        }

        public void OnConnectionFailed(ConnectionResult bundle)
        {
            // This method is used to handle connection issues with the Google Play Services Client (LocationClient).
            // You can check if the connection has a resolution (bundle.HasResolution) and attempt to resolve it
            // You must implement this to implement the IGooglePlayServicesClientOnConnectionFailedListener Interface
            Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
        }

        public void OnConnectionSuspended(int i)
        {

        }
    }
}