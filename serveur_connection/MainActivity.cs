using System;
using System.Threading.Tasks;
using System.Json;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Util;

using Newtonsoft.Json.Linq;

//using DeviceInfo.Plugin;

namespace serveur_connection
{
    [Activity(Label = "serveur_connection", MainLauncher = true, Icon = "@drawable/icon")]


    public class MainActivity : Activity, ILocationListener
    {
        Location _currentLocation;
        LocationManager _locationManager;
        String _locationProvider;
        EditText _latitude;
        EditText _longitude;

        public void OnLocationChanged(Location location) {}

        public void OnProviderDisabled(string provider) {}

        public void OnProviderEnabled(string provider) {}

        public void OnStatusChanged(string provider, Availability status, Bundle extras) {}

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get the latitude/longitude EditBox and button resources:
            _latitude = FindViewById<EditText>(Resource.Id.latText);
            _longitude = FindViewById<EditText>(Resource.Id.longText);
            Button weatherButton = FindViewById<Button>(Resource.Id.getWeatherButton);
            Button positionButton = FindViewById<Button>(Resource.Id.getCurPosButton);

            InitializeLocationManager();

            positionButton.Click += PositionButton_OnClick;

            // When the user clicks the button ...
            weatherButton.Click += async (sender, e) =>
            {
                // Get the latitude and longitude entered by the user and create a query.
               string url = "http://api.openweathermap.org/data/2.5/weather?lat=" +
                                _latitude.Text +
                                "&lon=" +
                                _longitude.Text;
 
                // Fetch the weather informations asynchronously,
                // parse the results, then update the screen:
                JsonValue json = await FetchWeatherAsync(url);
                ParseAndDisplay (json);
            };

        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria { Accuracy = Accuracy.Fine };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = String.Empty;
            }
        }

        async void PositionButton_OnClick(object sender, EventArgs eventArgs)
        {
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            _currentLocation = _locationManager.GetLastKnownLocation(_locationProvider);
            if (_currentLocation != null)
            {
                string url = "http://api.openweathermap.org/data/2.5/weather?lat=" +
                 _currentLocation.Latitude +
                 "&lon=" +
                 _currentLocation.Longitude;

                // Fetch the weather informations asynchronously,
                // parse the results, then update the screen:
                JsonValue json = await FetchWeatherAsync(url);
                ParseAndDisplay(json);

            }
        }

        // Gets weather data rom the passed URL.
        private async Task<JsonValue> FetchWeatherAsync (string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync ())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream ())
                {
                    // Use this stream to build a JSON document object:
                    JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                    // Return the JSON document:
                    return jsonDoc;
                }
            }
        }

        private void ParseAndDisplay (JsonValue json)
        {
            // Get the weather reporting fields from the layout resource:
            TextView location = FindViewById<TextView>(Resource.Id.locationText);
            TextView temperature = FindViewById<TextView>(Resource.Id.tempText);
            TextView humidity = FindViewById<TextView>(Resource.Id.humidText);
            TextView conditions = FindViewById<TextView>(Resource.Id.condText);

            var obj = JObject.Parse(json.ToString());

            // Extract the location and write it to the location TextBox:
            location.Text = obj["name"].ToString();
            
            // Extract temperature:
            string tempStr = obj["main"]["temp"].ToString();

            // Convert temperature from Kelvin to Celsius
            double temp = Convert.ToDouble(tempStr);
            temp -= 273.15;

            // Write the temperature (one decimal place) to the temperature TextBox:
            temperature.Text = temp.ToString() + "° C";

            // Get the percentage of humidity and write it to the humidity TextBox:
            humidity.Text = obj["main"]["humidity"].ToString() + "%";

            // Get description in weather array and write it to the humidity TextBox:
            conditions.Text = obj["weather"][0]["description"].ToString();
            
            //var device = DeviceInfo.Plugin.CrossDeviceInfo.Current.Id;
            //Console.Out.WriteLine("Response: {0}", device);
       }
    }
}