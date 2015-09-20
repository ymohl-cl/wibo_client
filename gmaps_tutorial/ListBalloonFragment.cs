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
	public class ListBalloonFragment : Fragment
    {
        private List<Balloon> _balloonsList;
        private ScrollView _scrollView;
        private LinearLayout _layout;
        private int _type;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var view = inflater.Inflate(Resource.Layout.listBalloonFragment, container, false);
			_scrollView = view.FindViewById<ScrollView>(Resource.Id.titleBalloonList);
			_layout = view.FindViewById<LinearLayout>(Resource.Id.listLayout);
			if (Arguments != null) 
			{
				Console.WriteLine (Arguments.GetString ("listBalloons"));
				string jsonBalloons = Arguments.GetString ("listBalloons");
				_balloonsList = JsonConvert.DeserializeObject<List<Balloon>> (jsonBalloons);
				_type = Arguments.GetInt ("typeBalloons");
				AddTextViewFromBalloons (_balloonsList, _type);
			}
            return view;
        }

        private void AddTextViewFromBalloons(List<Balloon> balloons, int type)
        {
            ContextThemeWrapper newContext;

			if (_type == 0)
			{
				newContext = new ContextThemeWrapper(this.Activity, Resource.Style.titleCatchedBalloonTextViewStyle);
			}
			else
			{
				newContext = new ContextThemeWrapper(this.Activity, Resource.Style.titleFollowedBalloonTextViewStyle);
			}
            foreach (Balloon balloon in balloons)
            {
				Console.WriteLine(newContext);
                TextView balloonTextView = new TextView(newContext);
                LinearLayout.LayoutParams newLayoutParameters = new LinearLayout.LayoutParams(_layout.LayoutParameters);
                newLayoutParameters.SetMargins(16, 4, 16, 4);
                newLayoutParameters.Height = 96;
                balloonTextView.LayoutParameters = newLayoutParameters;
                balloonTextView.Text = balloon.Title;
                balloonTextView.Click += (send, e) =>
                {
                	var seeBalloonContentActivity = new Intent(this.Activity, typeof(SeeBalloonContentActivity));
                	//Serialize balloon to send it through Activities using the Intent class
                	string balloonJson = JsonConvert.SerializeObject(balloon);
                	//Send the clicked balloon
                	seeBalloonContentActivity.PutExtra("Balloon", balloonJson);
                	seeBalloonContentActivity.PutExtra("Type", _type);
                	//Wait for result to know if the balloon has been resend and/or followed
                	StartActivityForResult(seeBalloonContentActivity, 1);
				};
				_layout.AddView(balloonTextView);
            }
        }
    }
}