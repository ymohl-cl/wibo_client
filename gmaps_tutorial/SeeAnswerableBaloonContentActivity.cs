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
    [Activity(Label = "SeeAnswerableBaloonContentActivity", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SeeAnswerableBaloonContentActivity: Activity
    {
        private Baloon _baloon;
        private ScrollView _scrollView;
        private LinearLayout _layout;
        private TextView _titleBaloonTextView;
        private Button _sendAnswerButton;
        private Button _followBaloonButton;
        private EditText _answerBaloonEditText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //unserialize baloon to get and display content
            string jsonBaloon = Intent.GetStringExtra("Baloon");
            _baloon = JsonConvert.DeserializeObject<Baloon>(jsonBaloon);
            SetContentView(Resource.Layout.contentAnswerableBaloon);
            _scrollView = FindViewById<ScrollView>(Resource.Id.titleBaloonList);
            _layout = FindViewById<LinearLayout>(Resource.Id.listLayout);
            _titleBaloonTextView = FindViewById<TextView>(Resource.Id.titleBaloonTextView);
            _titleBaloonTextView.Text = _baloon.Title;
            _sendAnswerButton = FindViewById<Button>(Resource.Id.sendAnswerButton);
            _sendAnswerButton.Click += _sendAnswerButton_Click;
            _followBaloonButton = FindViewById<Button>(Resource.Id.followBaloonButton);
            _followBaloonButton.Click += _followBaloonButoon_Click;
            _answerBaloonEditText = FindViewById<EditText>(Resource.Id.answerBaloonEditText);
            if (_baloon.Followed)
                _followBaloonButton.Text = "Unfollow";
            else
                _followBaloonButton.Text = "Follow";
            //Add all baloons to the scroll view
            foreach (String message in _baloon.Messages)
            {
                TextView messageBaloonTextView = new TextView(this);
                messageBaloonTextView.Text = message;
                _layout.AddView(messageBaloonTextView);
            }
        }

        void _followBaloonButoon_Click(object sender, EventArgs e)
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

        void _sendAnswerButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("answerBaloonEditText is clicked : {0}", _answerBaloonEditText.Text);
            if (String.IsNullOrEmpty(_answerBaloonEditText.Text))
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
                //send request to warn server that new message has to be add on current baloon
                Intent data = new Intent();
                _baloon.Catched = false;
                data.PutExtra("baloonId", _baloon.Id.ToString());
                data.PutExtra("followed", _baloon.Followed.ToString());
                data.PutExtra("resend", _baloon.Catched.ToString());
                data.PutExtra("message", _answerBaloonEditText.Text);
                SetResult(Result.Ok, data);
                this.Finish();
            }
        }

        public override void OnBackPressed()
        {
            Intent data = new Intent();
            data.PutExtra("baloonId", _baloon.Id.ToString());
            data.PutExtra("followed", _baloon.Followed.ToString());
            data.PutExtra("resend", _baloon.Catched.ToString());
            SetResult(Result.Ok, data);
            Finish();
        }
    }
}