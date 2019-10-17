
using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;	
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
	[Activity(Label = "Список фраз", Theme = "@style/AppTheme",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		ScreenOrientation = ScreenOrientation.SensorPortrait)]
	public class PhraseServiceActivity : AppCompatActivity {
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

			/*Snackbar.Make(Main, "Чтобы удалить фразу, долго жмите на неё", Snackbar.LengthIndefinite)
					.SetAction("Ok", delegate { })
					.Show();*/
			Load();
		}

		public async void Load() {
			LoadingDialog builder = new LoadingDialog(this).SetMessage("Загрузка...");
			Android.App.AlertDialog dialog = builder.Create();
			dialog.Show();

			string response = await Connector.GetAsync("phrase.set") ?? "{}";
			var data = JObject.Parse(response);
			if(data?["status"] == null) {
				Toast.MakeText(this, "Произошла ошибка :(", ToastLength.Short).Show();
				dialog.Dismiss();
				Finish();
				return;
			}

			Phrases = new List<Phrase>();
			foreach(var item in (JArray) data["result"]) {
				Phrase ph = new Phrase {
					Id = Convert.ToInt32(item["id"]),
					Text = Convert.ToString(item["text"])
				};
				Phrases.Add(ph);
			}

			BuildElements();
			dialog.Dismiss();
		}

		public void BuildElements() {
			Main.RemoveAllViews();
			Phrases.ForEach(e => BuildElement(e));
		}

		public void BuildElement(Phrase e) {
			TextView view = new TextView(this);
			view.SetPadding(10.Dip(), 8.Dip(), 10.Dip(), 8.Dip());
			view.Text = e.Text;
			view.LongClick += async delegate {
				bool deleted = await DeleteElement(e);
				if(deleted) {
					Toast.MakeText(this, "Фраза удалена", ToastLength.Short).Show();
					Phrases.Remove(e);
					BuildElements();
				}
			};
			Main.AddView(view);
		}

		public async Task<bool> DeleteElement(Phrase p) {
			var data = new NameValueCollection();
			data.Add("id", p.Id.ToString());

			string responce = await Connector.PostAsync("phrase.del", data) ?? "{}";
			JObject obj = JObject.Parse(responce);
			return Boolean.Parse(obj?["status"]?.ToString());
		}



		public List<Phrase> Phrases;
		public class Phrase {
			public int Id { get; set; }
			public string Text { get; set; }
		}
	}
}