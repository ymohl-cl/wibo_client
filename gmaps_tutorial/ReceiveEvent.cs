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
    public class OnReceiveBalloonListArgs : EventArgs
    {
        private List<Balloon> _response;

        public List<Balloon> Response
        {
            get { return _response; }
            set { _response = value; }
        }

        public OnReceiveBalloonListArgs(List<Balloon> balloonList) : base ()
        {
            Response = balloonList;
//            string json = JsonConvert.SerializeObject(balloonList);
//            Console.WriteLine(json);
        }
    }

    public class OnReceiveBalloonArgs : EventArgs
    {
        private Balloon _response;

        public Balloon Response
        {
            get { return _response; }
            set { _response = value; }
        }

        private void print(string str)
        {
            Console.Write("{0}\n", str);
        }
        public OnReceiveBalloonArgs(Balloon balloon) : base()
        {
            Console.Write("ballon : {0}\n", balloon.Id);
            balloon.Messages.ForEach(print);
            Response = balloon;
        }
    }
}