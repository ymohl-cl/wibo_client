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
using SupportFragment = Android.Support.V4.App.Fragment;

using Android.Support.V7.App;


namespace wibo
{
    [Activity(Label = "SeeBalloonContentActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
	public class SeeBalloonContentActivity : Activity
    {

        private Balloon _balloon;
        private int _type;
		private int _numTab;
		private FrameLayout _followButton;
		private FrameLayout _leftTab;
		private FrameLayout _rightTab;
		private TabBalloonFragment _contentTabFocus;
		private TabBalloonFragment _statsTabFocus;
		private TabBalloonFragment _answerTabFocus;
		private TabBalloonFragment _releaseTabFocus;
		private TabBalloonFragment _contentTabUnfocus;
		private TabBalloonFragment _statsTabUnfocus;
		private TabBalloonFragment _answerTabUnfocus;
		private TabBalloonFragment _releaseTabUnfocus;

		private const int FOCUS = 0;
		private const int UNFOCUS = 1;
		private const int CATCHED = 0;
		private const int FOLLOW = 1;
		private const int CONTENT = 0;
		private const int STATS = 1;
		private const int ANSWER = 2;
		private const int RELEASE = 3;
		private const int LEFT = 0;
		private const int RIGHT = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.contentBalloon);

            string jsonBalloon = Intent.GetStringExtra("Balloon");
            _balloon = JsonConvert.DeserializeObject<Balloon>(jsonBalloon);
            _type = Intent.GetIntExtra("type", -1);

			_contentTabFocus = new TabBalloonFragment ();
			_statsTabFocus = new TabBalloonFragment ();
			_answerTabFocus = new TabBalloonFragment ();
			_releaseTabFocus = new TabBalloonFragment ();
			_contentTabUnfocus = new TabBalloonFragment ();
			_statsTabUnfocus = new TabBalloonFragment ();
			_answerTabUnfocus = new TabBalloonFragment ();
			_releaseTabUnfocus = new TabBalloonFragment ();

			if (_type == CATCHED)
			{
				CreateTabFragment (_answerTabFocus, FOCUS, ANSWER, Resource.Id.leftTab);
				CreateTabFragment (_answerTabUnfocus, UNFOCUS, ANSWER, Resource.Id.leftTab);
				CreateTabFragment (_releaseTabFocus, FOCUS, RELEASE, Resource.Id.rightTab);
				CreateTabFragment (_releaseTabUnfocus, UNFOCUS, RELEASE, Resource.Id.rightTab);
			}
			if (_type == FOLLOW) 
			{
				CreateTabFragment (_contentTabFocus, FOCUS, CONTENT, Resource.Id.leftTab);
				CreateTabFragment (_contentTabUnfocus, UNFOCUS, CONTENT, Resource.Id.leftTab);
				CreateTabFragment (_statsTabFocus, FOCUS, STATS, Resource.Id.rightTab);
				CreateTabFragment (_statsTabUnfocus, UNFOCUS, STATS, Resource.Id.rightTab);
			}

			_leftTab = FindViewById<FrameLayout> (Resource.Id.leftTab);
			_rightTab = FindViewById<FrameLayout> (Resource.Id.rightTab);

			_leftTab.Click += _leftTab_Click;
			_rightTab.Click += _rightTab_Click;
        }

        void _rightTab_Click (object sender, EventArgs e)
        {
			if (_numTab == LEFT)
			{
				_numTab = RIGHT;
				if (_type == CATCHED)
				{
					Intent data = new Intent();
					_balloon.Catched = false;
					data.PutExtra("balloonId", _balloon.Id);
					data.PutExtra("followed", _balloon.Followed);
					data.PutExtra("resend", _balloon.Catched);
					data.PutExtra("type", _type);
					SetResult(Result.Ok, data);
					Finish();
				}
				if (_type == FOLLOW)
				{
					ShowTabFragment (CONTENT, UNFOCUS);
					ShowTabFragment (STATS, FOCUS);
				}
			}
        }

        void _leftTab_Click (object sender, EventArgs e)
        {
			if (_numTab == RIGHT)
			{
				_numTab = LEFT;
				if (_type == CATCHED)
				{
					ShowTabFragment (ANSWER, FOCUS);
				}
				if (_type == FOLLOW)
				{
					ShowTabFragment (CONTENT, FOCUS);
					ShowTabFragment (STATS, UNFOCUS);
				}
			}

        }

		private void ShowTabFragment(int type, int state)
		{
			var ft = FragmentManager.BeginTransaction();
			if (type == CONTENT)
			{
				if (state == FOCUS)
				{
					if (_contentTabFocus.IsVisible)
						return;
					_contentTabFocus.View.BringToFront ();
					ft.Hide (_contentTabUnfocus);
					ft.Show (_contentTabFocus);
				}
				else
				{
					if (_contentTabUnfocus.IsVisible)
						return;
					_contentTabUnfocus.View.BringToFront ();
					ft.Hide (_contentTabFocus);
					ft.Show (_contentTabUnfocus);
				}
			}
			if (type == STATS)
			{
				if (state == FOCUS)
				{
					if (_statsTabFocus.IsVisible)
						return;
					_statsTabFocus.View.BringToFront ();
					ft.Hide (_statsTabUnfocus);
					ft.Show (_statsTabFocus);
				}
				else
				{
					if (_statsTabUnfocus.IsVisible)
						return;
					_statsTabUnfocus.View.BringToFront ();
					ft.Hide (_statsTabFocus);
					ft.Show (_statsTabUnfocus);
				}
			}
			if (type == ANSWER)
			{
				if (state == FOCUS)
				{
					if (_answerTabFocus.IsVisible)
						return;
					_answerTabFocus.View.BringToFront ();
					ft.Hide (_answerTabUnfocus);
					ft.Show (_answerTabFocus);
				}
				else
				{
					if (_answerTabUnfocus.IsVisible)
						return;
					_answerTabUnfocus.View.BringToFront ();
					ft.Hide (_answerTabFocus);
					ft.Show (_answerTabUnfocus);
				}
			}
			if (type == RELEASE)
			{
				if (state == FOCUS)
				{
					if (_releaseTabFocus.IsVisible)
						return;
					_releaseTabFocus.View.BringToFront ();
					ft.Hide (_releaseTabUnfocus);
					ft.Show (_releaseTabFocus);
				}
				else
				{
					if (_releaseTabUnfocus.IsVisible)
						return;
					_releaseTabUnfocus.View.BringToFront ();
					ft.Hide (_releaseTabFocus);
					ft.Show (_releaseTabUnfocus);
				}
			}
			ft.AddToBackStack(null);
			ft.Commit ();
		}

		private void CreateTabFragment (Fragment fragment, int state, int type, int container)
		{
			Bundle args = new Bundle();
			var ft = FragmentManager.BeginTransaction ();
			args.PutInt ("state", state);
			args.PutInt ("type", type);
			fragment.Arguments = args;
			ft.Add (container, fragment);
			ft.Hide (fragment);
			ft.Commit ();
		}

		/*
        void _toolbar_MenuItemClick (object sender, SupportToolbar.MenuItemClickEventArgs e)
        {
			switch(e.Item.ItemId)
			{
			case Resource.Id.action_followBalloon:
				if (_balloon.Followed == true)
				{
					_balloon.Followed = false;
					//_followBalloonButton.Text = "Follow";
				}
				else
				{
					_balloon.Followed = true;
					//_followBalloonButton.Text = "Unfollow";
				}
				break;
			}
        }

        void _balloonContentTabs_MenuItemClick (object sender, SupportToolbar.MenuItemClickEventArgs e)
        {
			var ft = FragmentManager.BeginTransaction();
			switch(e.Item.ItemId)
			{
			case Resource.Id.action_contentBalloon:
				string jsonBalloon = JsonConvert.SerializeObject(_balloon);
				Bundle args = new Bundle();
				args.PutString("balloon", jsonBalloon);
				if (_type == 0)
				{
					AnswerableBalloonContentFragment answerableBalloonContentFragment = new AnswerableBalloonContentFragment();
					answerableBalloonContentFragment.Arguments = args;
					ft.Add(Resource.Id.fragmentContainer, answerableBalloonContentFragment);
				}
				else
				{
					BalloonContentFragment balloonContentFragment = new BalloonContentFragment();
					balloonContentFragment.Arguments = args;
					ft.Add(Resource.Id.fragmentContainer, balloonContentFragment);
				}
				break;
			case Resource.Id.action_statsBalloon:
				break;
			}
        }

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.toolbarBalloon, menu);
			return base.OnCreateOptionsMenu(menu);
		}*/

        protected override void OnPause()
        {
            base.OnPause();
            Console.WriteLine("On Pause Called");
            Intent data = new Intent();
            data.PutExtra("balloonId", _balloon.Id);
            data.PutExtra("followed", _balloon.Followed);
            data.PutExtra("resend", _balloon.Catched);
            data.PutExtra("type", _type);
            SetResult(Result.Ok, data);
        }

        public override void OnBackPressed()
        {
            Intent data = new Intent();
            data.PutExtra("balloonId", _balloon.Id);
            data.PutExtra("followed", _balloon.Followed);
            data.PutExtra("resend", _balloon.Catched);
            data.PutExtra("type", _type);
            SetResult(Result.Ok, data);
            Finish();
        }
	}
}