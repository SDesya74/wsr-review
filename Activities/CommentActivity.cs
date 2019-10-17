using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Specialized;

using WSReview.Misc;

namespace WSReview.Activities {
	[Activity(Label = "Отправка комментария",
		Theme = "@style/AppTheme",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		ScreenOrientation = ScreenOrientation.SensorPortrait)]
	public class CommentActivity : AppCompatActivity {
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			LinearLayout main = new LinearLayout(this);
			main.Orientation = Orientation.Vertical;
			main.SetGravity(GravityFlags.Top);
			SetContentView(main);

			EditText editor = new EditText(this);
			editor.SetHeight(Screen.Height / 4);
			editor.Gravity = GravityFlags.Left | GravityFlags.Top;
			main.AddView(editor);


			Button send = new Button(this);
			send.Text = "Отправить комментарий";
			send.Click += async delegate {
				if(String.IsNullOrWhiteSpace(editor.Text)) return;

				InputMethodManager inputManager = (InputMethodManager) GetSystemService(Activity.InputMethodService);
				inputManager.HideSoftInputFromWindow(editor.WindowToken, 0);


				var data = new NameValueCollection();
				data.Add("name", Options.ClientName);
				data.Add("comment", editor.Text);

				LoadingDialog builder = new LoadingDialog(this).SetMessage("Отправка...");
				Android.App.AlertDialog dialog = builder.Create();
				dialog.Show();

				string responce = await Connector.PostAsync("comment.add", data) ?? "{}";
				bool added = JObject.Parse(responce)?["status"] != null;
				if(added) {
					Snackbar bar = Snackbar.Make(send, "Комментарий отправлен :)", Snackbar.LengthShort);
					bar.Show();
					editor.Text = "";
				} else {
					Snackbar.Make(send, "Произошла ошибка :(", Snackbar.LengthIndefinite)
							.SetAction("Ok", delegate { })
							.Show();
				}
				dialog.Dismiss();
			};
			main.AddView(send);
		}
	}
}