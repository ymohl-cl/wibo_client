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
    [Activity(Label = "MenuFollowedBaloonActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MenuFollowedBaloonActivity : Activity
    {
        private List<Baloon> _followedBaloons;
        private List<Baloon> _catchedBaloons;
        private ScrollView _scrollView;
        private LinearLayout _layout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //get all serialized followed baloons and non-resend baloons
            string jsonFolloweBaloons = Intent.GetStringExtra("Followed Baloons");
            string jsonCatchedBaloons = Intent.GetStringExtra("Catched Baloons");
            //then deserialize to retrieve them
            _followedBaloons = JsonConvert.DeserializeObject<List<Baloon>>(jsonFolloweBaloons);
            _catchedBaloons = JsonConvert.DeserializeObject<List<Baloon>>(jsonCatchedBaloons);
        }

        //Is called after On ActivityResult so we can update the list if there were any changes in thez followed and catched baloons
        protected override void OnResume()
        {
            base.OnResume();
            Console.WriteLine("OnResume Called");
            SetContentView(Resource.Layout.menuFollowedBaloon);
            _scrollView = FindViewById<ScrollView>(Resource.Id.titleBaloonList);
            _layout = FindViewById<LinearLayout>(Resource.Id.listLayout);
            //Check if there is any baloon to display
            if (_followedBaloons == null && _catchedBaloons == null)
            {
                TextView noBaloons;

                noBaloons = new TextView(this);
                noBaloons.Text = "Vous n'avez aucun ballons suivis ni de ballons à renvoyer";
                _layout.AddView(noBaloons);
            }
            else
            {
                //First display the catchedBaloons if there is any
                if (_catchedBaloons.Count != 0)
                {
                    AddTextViewFromBaloons(_catchedBaloons, 0);
                }
                //Then display the followed Baloons if there is any
                if (_followedBaloons.Count != 0)
                {
                    AddTextViewFromBaloons(_followedBaloons, 1);
                }
            }
        }

        private void AddTextViewFromBaloons(List<Baloon> baloons, int type)
        {
            ContextThemeWrapper newContext;

            foreach (Baloon baloon in baloons)
            {
                if (type == 0)
                {
                   newContext = new ContextThemeWrapper(this, Resource.Style.titleCatchedBaloonTextViewStyle);
                }
                else
                {
                    newContext = new ContextThemeWrapper(this, Resource.Style.titleFollowedBaloonTextViewStyle);
                }
                TextView baloonTextView = new TextView(newContext);
                LinearLayout.LayoutParams newLayoutParameters = new LinearLayout.LayoutParams(_layout.LayoutParameters);
                newLayoutParameters.SetMargins(16, 4, 16, 4);
                newLayoutParameters.Height = 96;
                baloonTextView.LayoutParameters = newLayoutParameters;
                baloonTextView.Text = baloon.Title;
                baloonTextView.Click += (send, e) =>
                {
                    if (type == 0)
                    {
                        var seeAnswerableBaloonContentActivity = new Intent(this, typeof(SeeAnswerableBaloonContentActivity));
                        //Serialize baloon to send it through Activities using the Intent class
                        string baloonJson = JsonConvert.SerializeObject(baloon);
                        //Send the clicked baloon
                        seeAnswerableBaloonContentActivity.PutExtra("Baloon", baloonJson);
                        //Wait for result to know if the baloon has been resend and/or followed
                        StartActivityForResult(seeAnswerableBaloonContentActivity, 2);
                    }
                    else if (type == 1)
                    {
                        var seeBaloonContentActivity = new Intent(this, typeof(SeeBaloonContentActivity));
                        //Serialize baloon to send it through Activities using the Intent class
                        string baloonJson = JsonConvert.SerializeObject(baloon);
                        //Send the clicked baloon
                        seeBaloonContentActivity.PutExtra("Baloon", baloonJson);
                        //Wait for result to know if the baloon has been resend and/or followed
                        StartActivityForResult(seeBaloonContentActivity, 1);
                    }
                };
                _layout.AddView(baloonTextView);
            }
        }

        public override void OnBackPressed()
        {
            Console.WriteLine("OnPause Called");
            Intent data = new Intent();
            string jsonFollowedBaloons = JsonConvert.SerializeObject(_followedBaloons, Formatting.Indented);
            string jsonCatchedBaloons = JsonConvert.SerializeObject(_catchedBaloons, Formatting.Indented);
            data.PutExtra("FollowedBaloons", jsonFollowedBaloons);
            data.PutExtra("CatchedBaloons", jsonCatchedBaloons);
            SetResult(Result.Ok, data);
            Finish();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            UInt64 id;
            Boolean followed;
            Boolean catched;

            Console.WriteLine("OnActivityResult called, resultCode :{0}, requestCode :{1}", resultCode, requestCode);
            //Return value from a followed baloon activity
            if (requestCode == 1)
            {
                if (data.HasExtra("baloonId"))
                {
                    UInt64.TryParse(data.GetStringExtra("baloonId"), out id);
                    Boolean.TryParse(data.GetStringExtra("followed"), out followed);
                    Baloon baloon = _followedBaloons.Find(x => x.Id == id);
                    if (followed == false)
                    {
                        _followedBaloons.Remove(baloon);
                    }
                }
            }
            //Return value from a
            if (requestCode == 2)
            {
                if (data.HasExtra("baloonId"))
                {
                    UInt64.TryParse(data.GetStringExtra("baloonId"), out id);
                    Boolean.TryParse(data.GetStringExtra("followed"), out followed);
                    Boolean.TryParse(data.GetStringExtra("resend"), out catched);
                    Baloon baloon = _catchedBaloons.Find(x => x.Id == id);
                    baloon.Followed = followed;
                    if (data.HasExtra("message"))
                    {
                        baloon.Messages.Add(data.GetStringExtra("message"));
                    }
                    Console.WriteLine(catched);
                    Console.WriteLine(followed);
                    if (followed && !catched)
                    {
                        if (baloon != null)
                        {
                            _followedBaloons.Add(baloon);
                            _catchedBaloons.Remove(baloon);
                        }
                    }
                    else if (!catched)
                    {
                        if (baloon != null)
                        {
                            _catchedBaloons.Remove(baloon);
                        }
                    }
                }
            }
        }
    }
}