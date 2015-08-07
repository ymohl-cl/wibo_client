using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace textBoxMessage
{
    public class sendBaloonEvent : EventArgs
    {
        private string _titleBaloon;
        private string _messageBaloon;

        public string TitleBaloon
        {
            get { return _titleBaloon; }
            set {_titleBaloon = value ;}
        }

        public string MessageBaloon
        {
            get { return _messageBaloon; }
            set {_titleBaloon = value ;}
        }

        public sendBaloonEvent(string titleBaloon, string messageBaloon)
            : base()
        {
            MessageBaloon = messageBaloon;
            TitleBaloon = titleBaloon;
        }
    }

    [Activity(Label = "textBoxMessage", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private String _titleBaloon;
        private String _textBaloon;
        private EditText _messageBaloonEditText;
        private EditText _titleBaloonEditText;

        public event EventHandler<sendBaloonEvent> sendBaloon;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Button sendBaloonButton = FindViewById<Button>(Resource.Id.sendBaloonButton);
            _titleBaloonEditText = FindViewById<EditText>(Resource.Id.titleBaloonEditText);
            _messageBaloonEditText = FindViewById<EditText>(Resource.Id.messageBaloonEditText);

            sendBaloonButton.Click += sendBaloonButton_Click;
        }

        void sendBaloonButton_Click(object sender, EventArgs e)
        {
               
        }
    }
}

