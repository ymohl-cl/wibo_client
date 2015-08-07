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
    [Activity(Label = "MenuFollowedBalloonActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MenuFollowedBalloonActivity : Activity
    {
        private List<Balloon> _followedBalloons;
        private List<Balloon> _catchedBalloons;
        private ScrollView _scrollView;
        private LinearLayout _layout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //get all serialized followed balloons and non-resend balloons
            string jsonFolloweBalloons = Intent.GetStringExtra("Followed Balloons");
            string jsonCatchedBalloons = Intent.GetStringExtra("Catched Balloons");
            //then deserialize to retrieve them
            _followedBalloons = JsonConvert.DeserializeObject<List<Balloon>>(jsonFolloweBalloons);
            _catchedBalloons = JsonConvert.DeserializeObject<List<Balloon>>(jsonCatchedBalloons);
        }

        //Is called after On ActivityResult so we can update the list if there were any changes in thez followed and catched balloons
        protected override void OnResume()
        {
            base.OnResume();
            Console.WriteLine("OnResume Called");
            SetContentView(Resource.Layout.menuFollowedBalloon);
            _scrollView = FindViewById<ScrollView>(Resource.Id.titleBalloonList);
            _layout = FindViewById<LinearLayout>(Resource.Id.listLayout);
            //Check if there is any balloon to display
            if (_followedBalloons == null && _catchedBalloons == null)
            {
                TextView noBalloons;

                noBalloons = new TextView(this);
                noBalloons.Text = "Vous n'avez aucun ballons suivis ni de ballons à renvoyer";
                _layout.AddView(noBalloons);
            }
            else
            {
                //First display the catchedBalloons if there is any
                if (_catchedBalloons.Count != 0)
                {
                    AddTextViewFromBalloons(_catchedBalloons, 0);
                }
                //Then display the followed Balloons if there is any
                if (_followedBalloons.Count != 0)
                {
                    AddTextViewFromBalloons(_followedBalloons, 1);
                }
            }
        }

        private void AddTextViewFromBalloons(List<Balloon> balloons, int type)
        {
            ContextThemeWrapper newContext;

            foreach (Balloon balloon in balloons)
            {
                if (type == 0)
                {
                   newContext = new ContextThemeWrapper(this, Resource.Style.titleCatchedBalloonTextViewStyle);
                }
                else
                {
                    newContext = new ContextThemeWrapper(this, Resource.Style.titleFollowedBalloonTextViewStyle);
                }
                TextView balloonTextView = new TextView(newContext);
                LinearLayout.LayoutParams newLayoutParameters = new LinearLayout.LayoutParams(_layout.LayoutParameters);
                newLayoutParameters.SetMargins(16, 4, 16, 4);
                newLayoutParameters.Height = 96;
                balloonTextView.LayoutParameters = newLayoutParameters;
                balloonTextView.Text = balloon.Title;
                balloonTextView.Click += (send, e) =>
                {
                    if (type == 0)
                    {
                        var seeAnswerableBalloonContentActivity = new Intent(this, typeof(SeeAnswerableBalloonContentActivity));
                        //Serialize balloon to send it through Activities using the Intent class
                        string balloonJson = JsonConvert.SerializeObject(balloon);
                        //Send the clicked balloon
                        seeAnswerableBalloonContentActivity.PutExtra("Balloon", balloonJson);
                        //Wait for result to know if the balloon has been resend and/or followed
                        StartActivityForResult(seeAnswerableBalloonContentActivity, 2);
                    }
                    else if (type == 1)
                    {
                        var seeBalloonContentActivity = new Intent(this, typeof(SeeBalloonContentActivity));
                        //Serialize balloon to send it through Activities using the Intent class
                        string balloonJson = JsonConvert.SerializeObject(balloon);
                        //Send the clicked balloon
                        seeBalloonContentActivity.PutExtra("Balloon", balloonJson);
                        //Wait for result to know if the balloon has been resend and/or followed
                        StartActivityForResult(seeBalloonContentActivity, 1);
                    }
                };
                _layout.AddView(balloonTextView);
            }
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
            UInt64 id;
            Boolean followed;
            Boolean catched;

            Console.WriteLine("OnActivityResult called, resultCode :{0}, requestCode :{1}", resultCode, requestCode);
            //Return value from a followed balloon activity
            if (requestCode == 1)
            {
                if (data.HasExtra("balloonId"))
                {
                    UInt64.TryParse(data.GetStringExtra("balloonId"), out id);
                    Boolean.TryParse(data.GetStringExtra("followed"), out followed);
                    Balloon balloon = _followedBalloons.Find(x => x.Id == id);
                    if (followed == false)
                    {
                        _followedBalloons.Remove(balloon);
                    }
                }
            }
            //Return value from a
            if (requestCode == 2)
            {
                if (data.HasExtra("balloonId"))
                {
                    UInt64.TryParse(data.GetStringExtra("balloonId"), out id);
                    Boolean.TryParse(data.GetStringExtra("followed"), out followed);
                    Boolean.TryParse(data.GetStringExtra("resend"), out catched);
                    Balloon balloon = _catchedBalloons.Find(x => x.Id == id);
                    balloon.Followed = followed;
                    if (data.HasExtra("message"))
                    {
                        balloon.Messages.Add(data.GetStringExtra("message"));
                    }
                    Console.WriteLine(catched);
                    Console.WriteLine(followed);
                    if (followed && !catched)
                    {
                        if (balloon != null)
                        {
                            _followedBalloons.Add(balloon);
                            _catchedBalloons.Remove(balloon);
                        }
                    }
                    else if (!catched)
                    {
                        if (balloon != null)
                        {
                            _catchedBalloons.Remove(balloon);
                        }
                    }
                }
            }
        }
    }
}