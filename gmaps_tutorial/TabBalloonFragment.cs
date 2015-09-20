
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace wibo
{
	public class TabBalloonFragment : Fragment
	{
		private int _state;
		private int _type;

		private const int FOCUS = 0;
		private const int UNFOCUS = 1;
		private const int CONTENT = 0;
		private const int STATS = 1;
		private const int ANSWER = 2;
		private const int RELEASE = 3;


		public override void OnCreate (Bundle savedInstanceState)
		{
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			_state = Arguments.GetInt ("state");
			_type = Arguments.GetInt ("type");
			if (_type == CONTENT)
			{
				if (_state == FOCUS)
					return inflater.Inflate (Resource.Layout.contentTabFocus, container, false);
				else
					return inflater.Inflate (Resource.Layout.contentTabUnfocus, container, false);
			}
			if (_type == STATS)
			{
				if (_state == FOCUS)
					return inflater.Inflate (Resource.Layout.statsTabFocus, container, false);
				else
					return inflater.Inflate (Resource.Layout.statsTabUnfocus, container, false);
			}
			if (_type == ANSWER)
			{
				if (_state == FOCUS)
					return inflater.Inflate (Resource.Layout.answerTabFocus, container, false);
				else
					return inflater.Inflate (Resource.Layout.answerTabUnfocus, container, false);
			}
			if (_type == RELEASE)
			{
				if (_state == FOCUS)
					return inflater.Inflate (Resource.Layout.releaseTabFocus, container, false);
				else
					return inflater.Inflate (Resource.Layout.releaseTabUnfocus, container, false);
			}
			return inflater.Inflate (Resource.Layout.answerTabFocus, container, false);
		}
	}
}

