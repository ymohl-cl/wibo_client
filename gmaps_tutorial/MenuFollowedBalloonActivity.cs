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
using Android.Content.PM;
using Newtonsoft.Json;

namespace wibo
{
	[Activity(Label = "MenuFollowedBalloonActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, Theme="@style/MyTheme")]
	public class MenuFollowedBalloonActivity : Activity
    {
        private List<Balloon> _followedBalloons;
        private List<Balloon> _catchedBalloons;
		private LinearLayout _menuFollowedBalloonTabs;
		private FrameLayout _followedBalloonsTab;
		private FrameLayout _catchedBalloonsTab;
		private int _numTab;
		private ListBalloonFragment _followedBalloonsFragment;
		private ListBalloonFragment _catchedBalloonsFragment;
		private TabMenuFollowedFragment _followedTabFocusFragment;
		private TabMenuFollowedFragment _followedTabUnfocusFragment;
		private TabMenuFollowedFragment _catchedTabFocusFragment;
		private TabMenuFollowedFragment _catchedTabUnfocusFragment;
		private string jsonCatchedBalloons;
		private string jsonFollowedBalloons;
		private const int CATCHED = 0;
		private const int FOLLOW = 1;
		private const int FOCUS = 0;
		private const int UNFOCUS = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			SetContentView(Resource.Layout.menuFollowedBalloon);
			//get all serialized followed balloons and non-resend balloons
            jsonFollowedBalloons = Intent.GetStringExtra("Followed Balloons");
            jsonCatchedBalloons = Intent.GetStringExtra("Catched Balloons");
            //then deserialize to retrieve them
            _followedBalloons = JsonConvert.DeserializeObject<List<Balloon>>(jsonFollowedBalloons);
            _catchedBalloons = JsonConvert.DeserializeObject<List<Balloon>>(jsonCatchedBalloons);

			_catchedBalloonsTab = FindViewById<FrameLayout> (Resource.Id.catchedBalloonTab);
			_followedBalloonsTab = FindViewById<FrameLayout> (Resource.Id.followedBalloonTab);

			_followedBalloonsTab.Click += _followedBalloonsTab_Click;
			_catchedBalloonsTab.Click += _catchedBalloonsTab_Click;;

			_catchedBalloonsFragment = new ListBalloonFragment ();
			_followedBalloonsFragment = new ListBalloonFragment();

			CreateListFragment (jsonCatchedBalloons, _catchedBalloonsFragment, CATCHED);
			CreateListFragment (jsonFollowedBalloons, _followedBalloonsFragment, FOLLOW);

			_catchedTabFocusFragment = new TabMenuFollowedFragment ();
			_catchedTabUnfocusFragment = new TabMenuFollowedFragment ();
			_followedTabFocusFragment = new TabMenuFollowedFragment ();
			_followedTabUnfocusFragment = new TabMenuFollowedFragment ();

			CreateTabFragment (_catchedTabFocusFragment, FOCUS, CATCHED);
			CreateTabFragment(_catchedTabUnfocusFragment, UNFOCUS, CATCHED);
			CreateTabFragment(_followedTabFocusFragment, FOCUS, FOLLOW);
			CreateTabFragment(_followedTabUnfocusFragment, UNFOCUS, FOLLOW);

        }

        void _catchedBalloonsTab_Click (object sender, EventArgs e)
        {
			if (_numTab == FOLLOW)
			{
				ShowListFragment (CATCHED);
				ShowTabFragment (CATCHED, FOCUS);
				ShowTabFragment (FOLLOW, UNFOCUS);
			}
        }

        void _followedBalloonsTab_Click (object sender, EventArgs e)
        {
			if (_numTab == CATCHED)
			{
				ShowListFragment (FOLLOW);
				ShowTabFragment (CATCHED, UNFOCUS);
				ShowTabFragment (FOLLOW, FOCUS);
			}
        }
			
        //Is called after On ActivityResult so we can update the list if there were any changes in thez followed and catched balloons
        protected override void OnResume()
        {
            base.OnResume();

			if (_catchedBalloons.Count > 0)
			{
				Console.WriteLine ("Catched first show");
				ShowTabFragment (CATCHED, FOCUS);
				ShowTabFragment (FOLLOW, UNFOCUS);
				_numTab = CATCHED;
				ShowListFragment (CATCHED);
			} 
			else 
			{
				Console.WriteLine ("Followed first show");
				ShowTabFragment (CATCHED, UNFOCUS);
				ShowTabFragment (FOLLOW, FOCUS);
				_numTab = FOLLOW;
				ShowListFragment (FOLLOW);
			}
			Console.WriteLine ("End onResume menu");
		}

		private void ShowListFragment(int type)
		{
			var ft = FragmentManager.BeginTransaction();
			_numTab = type;
			if (type == CATCHED)
			{
				if (_catchedBalloonsFragment.IsVisible)
					return ;
				_catchedBalloonsFragment.View.BringToFront ();
				ft.Hide (_followedBalloonsFragment);
				ft.Show (_catchedBalloonsFragment);
			}
			if (type == FOLLOW) 
			{
				if (_followedBalloonsFragment.IsVisible)
					return ;
				_followedBalloonsFragment.View.BringToFront ();
				ft.Hide (_catchedBalloonsFragment);
				ft.Show(_followedBalloonsFragment);
			}
			ft.AddToBackStack(null);
			ft.Commit ();
		}

		private void ShowTabFragment(int type, int state)
		{
			var ft = FragmentManager.BeginTransaction();
			if (type == CATCHED)
			{
				if (state == FOCUS)
				{
					if (_catchedTabFocusFragment.IsVisible)
						return;
					Console.WriteLine ("Tab catched focus");
					_catchedTabFocusFragment.View.BringToFront ();
					ft.Hide (_catchedTabUnfocusFragment);
					ft.Show (_catchedTabFocusFragment);
				}
				else
				{
					if (_catchedTabUnfocusFragment.IsVisible)
						return;
					Console.WriteLine ("Tab catched unfocus");
					_catchedTabUnfocusFragment.View.BringToFront ();
					ft.Hide (_catchedTabFocusFragment);
					ft.Show (_catchedTabUnfocusFragment);
				}
			}
			if (type == FOLLOW) 
			{
				if (state == FOCUS)
				{
					if (_followedTabFocusFragment.IsVisible)
						return;
					Console.WriteLine ("Tab follow focus");
					_followedTabFocusFragment.View.BringToFront ();
					ft.Hide (_followedTabUnfocusFragment);
					ft.Show (_followedTabFocusFragment);
				}
				else
				{
					if (_followedTabUnfocusFragment.IsVisible)
						return;
					Console.WriteLine ("Tab follow unfocus");
					_followedTabUnfocusFragment.View.BringToFront ();
					ft.Hide (_followedTabFocusFragment);
					ft.Show (_followedTabUnfocusFragment);
				}
			}
			ft.AddToBackStack(null);
			ft.Commit ();
		}

		private void CreateListFragment(string jsonBalloonsList, Fragment fragment, int type)
		{
			Bundle args = new Bundle ();
			var ft = FragmentManager.BeginTransaction ();
			args.PutString ("listBalloons", jsonBalloonsList);
			args.PutInt ("typeBalloons", type);
			fragment.Arguments = args;
			ft.Add (Resource.Id.fragmentContainer, fragment);
			ft.Hide (fragment);
			ft.Commit ();
		}

		private void CreateTabFragment (Fragment fragment, int state, int type)
		{
			Bundle args = new Bundle();
			var ft = FragmentManager.BeginTransaction ();
			args.PutInt ("state", state);
			args.PutInt ("type", type);
			fragment.Arguments = args;
			if (type == CATCHED)
				ft.Add (Resource.Id.catchedBalloonTab, fragment);
			else
				ft.Add(Resource.Id.followedBalloonTab, fragment);
			ft.Hide (fragment);
			ft.Commit ();
		}

        public override void OnBackPressed()
        {
            Console.WriteLine("OnPause Called");
            Intent data = new Intent();
            string jsonFollowedBalloons = JsonConvert.SerializeObject(_followedBalloons, Formatting.Indented);
            string jsonCatchedBalloons = JsonConvert.SerializeObject(_catchedBalloons, Formatting.Indented);
            data.PutExtra("FollowedBalloons", jsonFollowedBalloons);
            data.PutExtra("CatchedBalloons", jsonCatchedBalloons);
            SetResult(Result.Ok, data);
            Finish();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Int64 id;
            Int64 type;
            Boolean followed;
            Boolean catched;
            Balloon balloon;
			Boolean followedListUpdated;
			Boolean catchedListUpdated;

			followedListUpdated = false;
			catchedListUpdated = false;
            Console.WriteLine("OnActivityResult called, resultCode :{0}, requestCode :{1}", resultCode, requestCode);
            //Return value a balloon content activity
            if (requestCode == 1)
            {
                if (data.HasExtra("balloonId"))
                {
                    type = data.GetIntExtra("type", -1);
                    id = data.GetLongExtra("balloonId", -1);
                    followed = data.GetBooleanExtra("followed", false);
                    catched = data.GetBooleanExtra("resend", true);
                    if (type == CATCHED)
                    {
                        balloon = _catchedBalloons.Find(x => x.Id == (UInt64)id);
                    }
                    else
                    {
                        balloon = _followedBalloons.Find(x => x.Id == (UInt64)id);
                    }
                    balloon.Followed = followed;
                    balloon.Catched = catched;
                    if (type == CATCHED)
                    {
                        if (!catched)
                        {
                            _catchedBalloons.Remove(balloon);
							catchedListUpdated = true;
							if (followed) 
							{
								_followedBalloons.Add (balloon);
								followedListUpdated = true;
							}
                        }
                    }
                    if (type == FOLLOW)
                    {
                        if (!followed)
                        {
							followedListUpdated = true;
                            _followedBalloons.Remove(balloon);
                        }
                    }
                }
				//Mettre a jour les listes et les fragments associés
				if (catchedListUpdated == true) 
				{
					jsonCatchedBalloons = JsonConvert.SerializeObject (_catchedBalloons);
					var ft = FragmentManager.BeginTransaction ();
					ft.Remove (_catchedBalloonsFragment);
					_catchedBalloonsFragment = new ListBalloonFragment ();
					CreateListFragment (jsonCatchedBalloons, _catchedBalloonsFragment, CATCHED); 
				}
				if (followedListUpdated == true)
				{
					var ft = FragmentManager.BeginTransaction ();
					jsonFollowedBalloons = JsonConvert.SerializeObject (_followedBalloons);
					ft.Remove (_followedBalloonsFragment);
					_followedBalloonsFragment = new ListBalloonFragment ();
					CreateListFragment (jsonCatchedBalloons, _followedBalloonsFragment, FOLLOW); 
				}
            }
        }
    }
}