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
using Newtonsoft.Json;


namespace wibo
{
    public class BalloonContentFragment : Fragment
    {
        private ScrollView _scrollView;
        private LinearLayout _layout;
        private Balloon _balloon;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
			string jsonBalloon = Arguments.GetString ("balloon");
			_balloon = JsonConvert.DeserializeObject<Balloon> (jsonBalloon);
            var view = inflater.Inflate(Resource.Layout.balloonContentFragment, null);
            _scrollView = view.FindViewById<ScrollView>(Resource.Id.messageBalloonScrollView);
            _layout = view.FindViewById<LinearLayout>(Resource.Id.listLayout);
            foreach (String message in _balloon.Messages)
            {
                TextView messageBalloonTextView = new TextView(this.Activity);
                messageBalloonTextView.Text = message;
                _layout.AddView(messageBalloonTextView);
            }
            return view;
        }
    }
}