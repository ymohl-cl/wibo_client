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
    public class SendBalloonEvent : EventArgs
    {
        private string _titleBalloon;
        private string _messageBalloon;

        public string TitleBalloon
        {
            get { return _titleBalloon; }
            set { _titleBalloon = value; }
        }

        public string MessageBalloon
        {
            get { return _messageBalloon; }
            set { _messageBalloon = value; }
        }

        public SendBalloonEvent(string titleBalloon, string messageBalloon)
        {
            _titleBalloon = titleBalloon;
            _messageBalloon = messageBalloon;
        }
    }

    class CreateBalloonFragment : DialogFragment
    {
        private Button _sendBalloonButton;
        private EditText _titleBalloon;
        private EditText _messageBalloon;

        public EventHandler<SendBalloonEvent> sendBalloonEvent;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.messageBalloonDialogFragment, container, false);
            _titleBalloon = view.FindViewById<EditText>(Resource.Id.titleBalloonEditText);
            _messageBalloon = view.FindViewById<EditText>(Resource.Id.messageBalloonEditText);
            _sendBalloonButton = view.FindViewById<Button>(Resource.Id.sendBalloonButton);
            _messageBalloon.Text = "Toto";
            _sendBalloonButton.Click += sendBalloonButton_Click;
            return view;
        }

        void sendBalloonButton_Click(object sender, EventArgs e)
        {
            sendBalloonEvent.Invoke(this, new SendBalloonEvent(_titleBalloon.Text, _messageBalloon.Text));
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.createBalloonAnimation;
        }
    }
}