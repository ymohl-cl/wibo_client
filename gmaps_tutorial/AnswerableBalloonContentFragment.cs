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
    class AnswerableBalloonContentFragment : Fragment
    {
        private ScrollView _scrollView;
        private LinearLayout _layout;
        private Button _sendAnswerButton;
        private Balloon _balloon;
        private EditText _answerBalloonEditText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.answerableBalloonContentFragment, null);
            
			string jsonBalloon = Arguments.GetString ("balloon");
			_balloon = JsonConvert.DeserializeObject<Balloon> (jsonBalloon);
            _scrollView = view.FindViewById<ScrollView>(Resource.Id.messageBalloonScrollView);
            _layout = view.FindViewById<LinearLayout>(Resource.Id.listLayout);
            foreach (String message in _balloon.Messages)
            {
                TextView messageBalloonTextView = new TextView(this.Activity);
                messageBalloonTextView.Text = message;
                _layout.AddView(messageBalloonTextView);
            }

            _answerBalloonEditText = view.FindViewById<EditText>(Resource.Id.answerBalloonEditText);
            _sendAnswerButton = view.FindViewById<Button>(Resource.Id.sendAnswerButton);
            _sendAnswerButton.Click += _sendAnswerButton_Click;

            return view;
        }

        void _sendAnswerButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("answerBalloonEditText is clicked : {0}", _answerBalloonEditText.Text);
            if (String.IsNullOrEmpty(_answerBalloonEditText.Text))
            {
                Console.WriteLine("Text is empty");
                AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);
                alert.SetTitle("Aucune réponse n'a été écrite");
                alert.SetPositiveButton("Ok", (senderAlert, args) =>
                {
                });
                this.Activity.RunOnUiThread(() =>
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
                this.Activity.SetResult(Result.Ok, data);
                this.Activity.Finish();
            }
        }
    }
}