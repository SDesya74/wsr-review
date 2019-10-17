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

namespace WSReview.Misc {
	class LoadingDialog : AlertDialog.Builder {
		private TextView view;
		public LoadingDialog(Context context) : base(context) {
			SetCancelable(false);

			LinearLayout main = new LinearLayout(context);
			main.Orientation = Orientation.Horizontal;
			main.SetGravity(GravityFlags.Center);

			ProgressBar bar = new ProgressBar(context);
			main.AddView(bar);

			view = new TextView(context);
			view.TextSize = 18;
			view.Gravity = GravityFlags.Center | GravityFlags.Left;
			main.AddView(view);

			SetView(main);
		}


		public new LoadingDialog SetMessage(string message) {
			view.Text = message;
			return this;
		}
	}
}