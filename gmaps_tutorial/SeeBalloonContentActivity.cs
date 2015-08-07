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
    [Activity(Label = "SeeBalloonContentActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SeeBalloonContentActivity : Activity
    {
        private Balloon _balloon;
        private ScrollView _scrollView;
        private LinearLayout _layout;
        private TextView _titleBalloonTextView;
        private Button _followBalloonButton;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            string jsonBalloon = Intent.GetStringExtra("Balloon");
            _balloon = JsonConvert.DeserializeObject<Balloon>(jsonBalloon);
            SetContentView(Resource.Layout.contentBalloon);
            _scrollView = FindViewById<ScrollView>(Resource.Id.titleBalloonList);
            _layout = FindViewById<LinearLayout>(Resource.Id.listLayout);
            _titleBalloonTextView = FindViewById<TextView>(Resource.Id.titleBalloonTextView);
            _titleBalloonTextView.Text = _balloon.Title;
            foreach (String message in _balloon.Messages)
            {
                TextView messageBalloonTextView = new TextView(this);
                messageBalloonTextView.Text = message;
                _layout.AddView(messageBalloonTextView);
            }
            _followBalloonButton = FindViewById<Button>(Resource.Id.followBalloonButton);
            if (_balloon.Followed == true)
                _followBalloonButton.Text = "Unfollow";
            else
                _followBalloonButton.Text = "Follow";
            _followBalloonButton.Click += _followBalloonButton_Click;
        }

        void _followBalloonButton_Click(object sender, EventArgs e)
        {
            if (_balloon.Followed == true)
            {
                _balloon.Followed = false;
                _followBalloonButton.Text = "Follow";
            }
            else
            {
                _balloon.Followed = true;
                _followBalloonButton.Text = "Unfollow";
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            Console.WriteLine("On Pause Called");
            Intent data = new Intent();
            data.PutExtra("balloonId", _balloon.Id.ToString());
            data.PutExtra("followed", _balloon.Followed.ToString());
            SetResult(Result.Ok, data);
        }

        public override void OnBackPressed()
        {
            Intent data = new Intent();
            data.PutExtra("balloonId", _balloon.Id.ToString());
            data.PutExtra("followed", _balloon.Followed.ToString());
            SetResult(Result.Ok, data);
            Finish();
        }
    }
}