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
using Newtonsoft.Json;
using Android.Content.PM;



namespace wibo
{
    [Activity(Label = "SeeBaloonContentActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SeeBaloonContentActivity : Activity
    {
        private Baloon _baloon;
        private ScrollView _scrollView;
        private LinearLayout _layout;
        private TextView _titleBaloonTextView;
        private Button _followBaloonButton;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            string jsonBaloon = Intent.GetStringExtra("Baloon");
            _baloon = JsonConvert.DeserializeObject<Baloon>(jsonBaloon);
            SetContentView(Resource.Layout.contentBaloon);
            _scrollView = FindViewById<ScrollView>(Resource.Id.titleBaloonList);
            _layout = FindViewById<LinearLayout>(Resource.Id.listLayout);
            _titleBaloonTextView = FindViewById<TextView>(Resource.Id.titleBaloonTextView);
            _titleBaloonTextView.Text = _baloon.Title;
            foreach (String message in _baloon.Messages)
            {
                TextView messageBaloonTextView = new TextView(this);
                messageBaloonTextView.Text = message;
                _layout.AddView(messageBaloonTextView);
            }
            _followBaloonButton = FindViewById<Button>(Resource.Id.followBaloonButton);
            if (_baloon.Followed == true)
                _followBaloonButton.Text = "Unfollow";
            else
                _followBaloonButton.Text = "Follow";
            _followBaloonButton.Click += _followBaloonButton_Click;
        }

        void _followBaloonButton_Click(object sender, EventArgs e)
        {
            if (_baloon.Followed == true)
            {
                _baloon.Followed = false;
                _followBaloonButton.Text = "Follow";
            }
            else
            {
                _baloon.Followed = true;
                _followBaloonButton.Text = "Unfollow";
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            Console.WriteLine("On Pause Called");
            Intent data = new Intent();
            data.PutExtra("baloonId", _baloon.Id.ToString());
            data.PutExtra("followed", _baloon.Followed.ToString());
            SetResult(Result.Ok, data);
        }

        public override void OnBackPressed()
        {
            Intent data = new Intent();
            data.PutExtra("baloonId", _baloon.Id.ToString());
            data.PutExtra("followed", _baloon.Followed.ToString());
            SetResult(Result.Ok, data);
            Finish();
        }
    }
}