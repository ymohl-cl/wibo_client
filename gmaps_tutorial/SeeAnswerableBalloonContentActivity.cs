using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;


namespace wibo
{
    [Activity(Label = "SeeAnswerableBalloonContentActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SeeAnswerableBalloonContentActivity: Activity
    {
        private Balloon _balloon;
        private ScrollView _scrollView;
        private LinearLayout _layout;
        private TextView _titleBalloonTextView;
        private Button _sendAnswerButton;
        private Button _followBalloonButton;
        private EditText _answerBalloonEditText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //unserialize balloon to get and display content
            string jsonBalloon = Intent.GetStringExtra("Balloon");
            _balloon = JsonConvert.DeserializeObject<Balloon>(jsonBalloon);
            SetContentView(Resource.Layout.contentAnswerableBalloon);
            _scrollView = FindViewById<ScrollView>(Resource.Id.titleBalloonList);
            _layout = FindViewById<LinearLayout>(Resource.Id.listLayout);
            _titleBalloonTextView = FindViewById<TextView>(Resource.Id.titleBalloonTextView);
            _titleBalloonTextView.Text = _balloon.Title;
            _sendAnswerButton = FindViewById<Button>(Resource.Id.sendAnswerButton);
            _sendAnswerButton.Click += _sendAnswerButton_Click;
            _followBalloonButton = FindViewById<Button>(Resource.Id.followBalloonButton);
            _followBalloonButton.Click += _followBalloonButoon_Click;
            _answerBalloonEditText = FindViewById<EditText>(Resource.Id.answerBalloonEditText);
            if (_balloon.Followed)
                _followBalloonButton.Text = "Unfollow";
            else
                _followBalloonButton.Text = "Follow";
            //Add all balloons to the scroll view
            foreach (String message in _balloon.Messages)
            {
                TextView messageBalloonTextView = new TextView(this);
                messageBalloonTextView.Text = message;
                _layout.AddView(messageBalloonTextView);
            }
        }

        void _followBalloonButoon_Click(object sender, EventArgs e)
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

        void _sendAnswerButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("answerBalloonEditText is clicked : {0}", _answerBalloonEditText.Text);
            if (String.IsNullOrEmpty(_answerBalloonEditText.Text))
            {
                Console.WriteLine("Text is empty");
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Aucune réponse n'a été écrite");
                alert.SetPositiveButton("Ok", (senderAlert, args) =>
                {
                });
                RunOnUiThread(() =>
                {
                    alert.Show();
                });
            }
            else
            {
                //send request to warn server that new message has to be add on current balloon
                Intent data = new Intent();
                _balloon.Catched = false;
                data.PutExtra("balloonId", _balloon.Id.ToString());
                data.PutExtra("followed", _balloon.Followed.ToString());
                data.PutExtra("resend", _balloon.Catched.ToString());
                data.PutExtra("message", _answerBalloonEditText.Text);
                SetResult(Result.Ok, data);
                this.Finish();
            }
        }

        public override void OnBackPressed()
        {
            Intent data = new Intent();
            data.PutExtra("balloonId", _balloon.Id.ToString());
            data.PutExtra("followed", _balloon.Followed.ToString());
            data.PutExtra("resend", _balloon.Catched.ToString());
            SetResult(Result.Ok, data);
            Finish();
        }
    }
}