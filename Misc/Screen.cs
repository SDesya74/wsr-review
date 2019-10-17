using System;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Java.Interop;

namespace WSReview.Misc {
	public static class Screen {
		public static int Width { get; private set; }
		public static int Height { get; private set; }
		public static float Scale { get; private set; }
		public static int ActionBarHeight { get; private set; }
		public static int StatusBarHeight { get; private set; }
		public static int UsefulHeight {
			get {
				return Height - ActionBarHeight;
			}
		}

		public static DisplayMetrics DisplayMetrics { get; private set; }

		public static int Dip2Px(float dip) {
			return (int) Math.Floor(dip * Scale);
		}
		public static void Init(Context context) {
			IWindowManager wm = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

			DisplayMetrics = new DisplayMetrics();
			wm.DefaultDisplay.GetMetrics(DisplayMetrics);

			Point point = new Point();
			wm.DefaultDisplay.GetSize(point);
			Width = point.X;
			Height = point.Y;

			Scale = context.Resources.DisplayMetrics.Density;

			TypedArray ta = context.Theme.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.ActionBarSize });
			ActionBarHeight = (int) ta.GetDimension(0, 0);

			int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
			if(resourceId > 0) StatusBarHeight = context.Resources.GetDimensionPixelSize(resourceId);
		}

		public static int Dip(this int px) {
			return Dip2Px(px);
		}
	}
}