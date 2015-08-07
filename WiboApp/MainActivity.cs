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
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Common.Apis;


namespace WiboApp
{
    [Activity(Label = "WiboApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnMapReadyCallback,Android.Gms.Maps.GoogleMap.IInfoWindowAdapter, Android.Gms.Maps.GoogleMap.IOnCameraChangeListener, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
    {
        private IGoogleApiClient apiClient;
        private GoogleMap _map;
        private Location _currentLocation;
        private LatLng _locationLatLng;
        private BitmapDescriptor redBaloon;
        private bool _isGooglePlayServicesInstalled;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("Toto");
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            redBaloon = BitmapDescriptorFactory.FromResource(Resource.Drawable.redBaloon);
            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();

            if (_isGooglePlayServicesInstalled)
            {
                // pass in the Context, ConnectionListener and ConnectionFailedListener
                apiClient = new GoogleApiClientBuilder(this, this, this)
                    .AddApi(LocationServices.API).Build();
                apiClient.Connect();
            }
            else
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
                Finish();
            }
            SetUpMap();
        }

        private void SetUpMap()
        {
            if (_map == null)
            {
                //Recupere la map via l'api google
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug("OnPause", "OnPause called, stopping location updates");

            if (apiClient.IsConnected)
            {
                apiClient.Disconnect();
            }
        }
   
        //Fonction appelée en callback de GetMapAsync
        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            MarkerOptions optionsBallon1 = new MarkerOptions()
            .SetPosition(new LatLng(48.85, 2.34))
            .SetIcon(redBaloon);
            MarkerOptions optionsBallon2 = new MarkerOptions()
            .SetPosition(new LatLng(48.86, 2.5))
            .SetTitle("Pigeon")
            .SetSnippet("Pigeon, Oiseau à la grise robe,Dans l'enfer des villes À mon regard tu te dérobes, Tu es vraiment le plus agile")
            .SetIcon(redBaloon);
            Marker marker1 = _map.AddMarker(optionsBallon1);
            marker1.HideInfoWindow();
            Marker marker2 = _map.AddMarker(optionsBallon2);
            marker2.HideInfoWindow();
            _map.SetInfoWindowAdapter(this);
        }

        private void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            markerClickEventArgs.Handled = true;
            Marker marker = markerClickEventArgs.Marker;
            marker.ShowInfoWindow();
        }

        public void OnLocationChanged(Location location)
        {

        }

        public View GetInfoContents(Marker marker)
        {
            return null;
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
            View view = LayoutInflater.Inflate(Resource.Layout.messageBaloon, null, false);
            TextView title = view.FindViewById<TextView>(Resource.Id.titleBaloon);
            title.Text = marker.Title;
            TextView message = view.FindViewById<TextView>(Resource.Id.textBaloon);
            message.Text = marker.Snippet;
            return view;
        }

        public void OnCameraChange(CameraPosition position)
        {
            //Checker la nouvelle position de la map et 
        }

        public void OnConnected(Bundle bundle)
        {
            // This method is called when we connect to the LocationClient. We can start location updated directly form
            // here if desired, or we can do it in a lifecycle method, as shown above 

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now connected to client");
            _currentLocation = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
            _locationLatLng = new LatLng(_currentLocation.Latitude, _currentLocation.Longitude);
            //move the camera to the location of the user
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(_locationLatLng, 10);
            _map.MoveCamera(camera);
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