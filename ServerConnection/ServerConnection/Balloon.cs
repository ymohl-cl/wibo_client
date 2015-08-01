using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
/*
 * using Android.Gms.Maps;
 * using Android.Gms.Maps.Model;
 * using Android.Gms.Location;
 * using Android.Gms.Common;
 * using Android.Gms.Common.Apis;
*/

namespace ServerConnection
{
    [Serializable]
    class Balloon
    {
        private UInt64 _id;
        private List<String> _messages;
        private String _title;
        private bool _catched;
        private bool _followed;
        private Double _latPosition;
        private Double _lngPosition;
        private Double _windDirection;
        private Double _windSpeed;

        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public List<String> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public UInt64 Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public bool Catched
        {
            get { return _catched; }
            set { _catched = value; }
        }

        public bool Followed
        {
            get { return _followed; }
            set { _followed = value; }
        }

        public Double WindDirection
        {
            get { return _windDirection; }
            set { _windDirection = value; }
        }

        public Double WindSpeed
        {
            get { return _windSpeed; }
            set { _windSpeed = value; }
        }

        public Double LatPosition
        {
            get { return _latPosition; }
            set { _latPosition = value; }
        }

        public Double LngPosition
        {
            get { return _lngPosition; }
            set { _lngPosition = value; }
        }

        public Balloon(List<String> messages, String title, UInt64 id, Double latPosition, Double lngPosition, bool followed, Double windDirection, Double windSpeed)
        {
            Messages = messages;
            Title = title;
            Id = id;
            Followed = followed;
            LatPosition = latPosition;
            LngPosition = lngPosition;
            WindDirection = windDirection;
            WindSpeed = windSpeed;
            Console.WriteLine("id : {0}, title : {1}, latitude : {2}, longitude : {3}, direction : {4}, speed : {5}",
                Id, Title, LatPosition, LngPosition, WindDirection, WindSpeed);
        }

        public Balloon(UInt64 id, List<String> messages)
        {
            Id = id;
            Messages = messages;
            if (Messages != null)
            {
                foreach (string msg in Messages)
                    Console.Write("msg : {0}\n\n", msg);
            }
        }

        public Balloon(UInt64 id, bool catched)
        {
            Id = id;
            Catched = catched;
        }
    }
}