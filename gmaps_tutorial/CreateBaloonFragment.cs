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

namespace wibo
{
    public class SendBaloonEvent : EventArgs
    {
        private string _titleBaloon;
        private string _messageBaloon;

        public string TitleBaloon
        {
            get { return _titleBaloon; }
            set { _titleBaloon = value; }
        }

        public string MessageBaloon
        {
            get { return _messageBaloon; }
            set { _messageBaloon = value; }
        }

        public SendBaloonEvent(string titleBaloon, string messageBaloon)
        {
            _titleBaloon = titleBaloon;
            _messageBaloon = messageBaloon;
        }
    }

    class CreateBaloonFragment : DialogFragment
    {
        private Button _sendBaloonButton;
        private EditText _titleBaloon;
        private EditText _messageBaloon;

        public EventHandler<SendBaloonEvent> sendBaloonEvent;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.messageBaloonDialogFragment, container, false);
            _titleBaloon = view.FindViewById<EditText>(Resource.Id.titleBaloonEditText);
            _messageBaloon = view.FindViewById<EditText>(Resource.Id.messageBaloonEditText);
            _sendBaloonButton = view.FindViewById<Button>(Resource.Id.sendBaloonButton);
            _messageBaloon.Text = "Toto";
            _sendBaloonButton.Click += sendBaloonButton_Click;
            return view;
        }

        void sendBaloonButton_Click(object sender, EventArgs e)
        {
            sendBaloonEvent.Invoke(this, new SendBaloonEvent(_titleBaloon.Text, _messageBaloon.Text));
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.createBaloonAnimation;
        }
    }
}