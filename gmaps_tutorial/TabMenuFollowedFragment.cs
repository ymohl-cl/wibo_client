
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
	public class TabMenuFollowedFragment : Fragment
	{
		private int _state;
		private int _type;
		private const int CATCHED = 0;
		private const int FOLLOW = 1;
		private const int FOCUS = 0;
		private const int UNFOCUS = 1;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			_state = Arguments.GetInt ("state");
			_type = Arguments.GetInt ("type");
			if (_type == CATCHED) {
				if (_state == FOCUS)
					return inflater.Inflate (Resource.Layout.catchedTabFocus, container, false);
				else
					return inflater.Inflate (Resource.Layout.catchedTabUnfocus, container, false);
			}
			else
			{
				if (_state == FOCUS)
					return inflater.Inflate (Resource.Layout.followedTabFocus, container, false);
				else
					return inflater.Inflate (Resource.Layout.followedTabUnfocus, container, false);
			}
		}
	}
}

