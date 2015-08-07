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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Common.Apis;

namespace wibo
{
    public class Baloon
    {
        private UInt64 _id;
        private List<String> _messages;
        private String _title;
        private bool _catched;
        private bool _followed;
        private Double _latPosition;
        private Double _lngPosition;
        private Double _windSpeed;
        private Double _windDirection;

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

        public Double WindSpeed
        {
            get { return _windSpeed; }
            set { _windSpeed= value; }
        }

        public Double WindDirection
        {
            get { return _windDirection; }
            set { _windDirection = value; }
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

        public Baloon(List<String> messages, String title, UInt64 id, Double latPosition, Double lngPosition, bool followed, Double windDirection, Double windDegrees)
        {
            Messages = messages;
            Title = title;
            Id = id;
            LatPosition = latPosition;
            LngPosition = lngPosition;
            Followed = followed;
            WindDirection = windDirection;
            WindSpeed = windDegrees;
        }
    }
}