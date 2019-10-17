using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Views;
using WSReview.Misc;
using Android.Graphics;
using Android.Content;

namespace WSReview.Activities{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity {
		private long LongClickTime = 200;
		private long LastLongClickTime = -1;
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);

			Screen.Init(this);
			Connector.Init();

			LinearLayout main = new LinearLayout(this);
			main.SetGravity(GravityFlags.Top | GravityFlags.Center);
			main.Orientation = Orientation.Vertical;
			SetContentView(main);



			ImageView image = new ImageView(this);
			image.SetPadding(10.Dip(), 10.Dip(), 10.Dip(), 20.Dip());
			image.SetImageBitmap(BitmapFactory.DecodeStream(Assets.Open("logo.png")));
			main.AddView(image);


			TextView label = new TextView(this);
			label.Text = "Имя пользователя: ";
			label.SetPadding(4.Dip(), 0, 0, 0);
			label.Gravity = GravityFlags.Left | GravityFlags.Bottom;
			main.AddView(label);



			Options.ClientName = GetPreferences(FileCreationMode.Private).GetString("name", Options.ClientName);



			EditText name = new EditText(this);
			name.SetSingleLine();
			name.Text = Options.ClientName;
			name.TextChanged += delegate {
				if(name.Text.Length < 1) return;
				Options.ClientName = name.Text;
				GetPreferences(FileCreationMode.Private).Edit().PutString("name", Options.ClientName).Apply();
			};
			main.AddView(name);

			LinearLayout buttons = new LinearLayout(this);
			buttons.SetGravity(GravityFlags.Center);
			buttons.Orientation = Orientation.Horizontal;
			main.AddView(buttons);


			var p = new ViewGroup.LayoutParams(Screen.Width / 2, Screen.Width / 2);

			Button comment = new Button(this);
			comment.LayoutParameters = p;
			comment.Text = "Отправить комментарий";
			comment.Click += delegate {
				Intent intent = new Intent(this, typeof(CommentActivity));
				StartActivity(intent);
			};
			comment.LongClick += delegate {
				long currentTime = Java.Lang.JavaSystem.CurrentTimeMillis();
				if(currentTime - LastLongClickTime < LongClickTime) {
					Intent intent = new Intent(this, typeof(CommentServiceActivity));
					StartActivity(intent);
				}
				LastLongClickTime = currentTime;
			};
			buttons.AddView(comment);

			Button phrase = new Button(this);
			phrase.LayoutParameters = p;
			phrase.Text = "Добавить фразу";
			phrase.Click += delegate {
				Intent intent = new Intent(this, typeof(PhraseActivity));
				StartActivity(intent);
			};
			phrase.LongClick += delegate {
				long currentTime = Java.Lang.JavaSystem.CurrentTimeMillis();
				if(currentTime - LastLongClickTime < LongClickTime) {
					Intent intent = new Intent(this, typeof(PhraseServiceActivity));
					StartActivity(intent);
				}
				LastLongClickTime = currentTime;
			};
			buttons.AddView(phrase);
		}
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}