using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

using WSReview.Misc;

namespace WSReview.Activities {
	[Activity(Label = "Список комментариев",
		Theme = "@style/AppTheme",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		ScreenOrientation = ScreenOrientation.SensorPortrait)]
	public class CommentServiceActivity : AppCompatActivity {
		LinearLayout Main;
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);

			ScrollView scroll = new ScrollView(this);
			SetContentView(scroll);

			Main = new LinearLayout(this);
			Main.Orientation = Orientation.Vertical;
			Main.SetGravity(GravityFlags.Top);
			Main.SetDividerDrawable(new ColorDrawable(Android.Graphics.Color.Gray));
			Main.DividerPadding = 2.Dip();
			Main.ShowDividers = ShowDividers.Middle;
			scroll.AddView(Main);

			Snackbar.Make(Main, "Чтобы удалить комментарий, долго жмите на него", Snackbar.LengthIndefinite)
					.SetAction("Ok", delegate { })
					.Show();
			Load();
		}

		public async void Load() {
			LoadingDialog builder = new LoadingDialog(this).SetMessage("Загрузка...");
			Android.App.AlertDialog dialog = builder.Create();
			dialog.Show();

		
			string response = await Connector.GetAsync("comment.get") ?? "{}";
			var data = JObject.Parse(response);
			if(data?["status"] == null) {
				Toast.MakeText(this, "Произошла ошибка :(", ToastLength.Short).Show();
				dialog.Dismiss();
				Finish();
				return;
			}

			Comments = new List<Comment>();
			foreach(var item in (JArray) data["result"]) {
				Comment e = new Comment {
					Id = Convert.ToInt32(item["id"]),
					Name = Convert.ToString(item["name"]),
					Text = Convert.ToString(item["coment"]),
					Time = Convert.ToString(item["datetime"])
				};
				Comments.Add(e);
			}

			BuildElements();
			dialog.Dismiss();
		}

		public void BuildElements() {
			Main.RemoveAllViews();
			Comments.ForEach(p => BuildElement(p));
		}

		public void BuildElement(Comment e) {
			

			LinearLayout items = new LinearLayout(this);
			items.LongClick += async delegate {
				bool deleted = await DeleteElement(e);
				if(deleted) {
					Toast.MakeText(this, "Комментарий удален", ToastLength.Short).Show();
					Comments.Remove(e);
					BuildElements();
				}
			};
			items.Orientation = Orientation.Vertical;
			items.SetGravity(GravityFlags.Left);
			Main.AddView(items);


			TextView name = new TextView(this);
			name.SetPadding(10.Dip(), 5.Dip(), 10.Dip(), 5.Dip());
			name.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
			name.Text = e.Name;
			items.AddView(name);

			TextView view = new TextView(this);
			view.SetPadding(10.Dip(), 5.Dip(), 10.Dip(), 5.Dip());
			view.Text = e.Text;
			items.AddView(view);

			TextView time = new TextView(this);
			time.SetPadding(10.Dip(), 5.Dip(), 10.Dip(), 5.Dip());
			time.Text = e.Time;
			time.Gravity = GravityFlags.Right;
			items.AddView(time);
		}

		public async Task<bool> DeleteElement(Comment e) {
			var data = new NameValueCollection();
			data.Add("id", e.Id.ToString());

			string responce = await Connector.PostAsync("comment.del", data) ?? "{}";
			JObject obj = JObject.Parse(responce);
			return Boolean.Parse(obj?["status"]?.ToString());
		}

		public List<Comment> Comments;
		public class Comment {
			public int Id { get; set; }
			public string Name { get; set; }
			public string Text { get; set; }
			public string Time { get; set; }
		}
	}
}