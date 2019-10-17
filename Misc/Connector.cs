using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Net;

namespace WSReview.Misc {
	static class Connector {
		private static string Address = @"https://lesson.kristeva.ru/wsr/";
		private static WebClient Client;

		public static void Init() {
			Client = new WebClient();
			Client.Encoding = Encoding.UTF8;
		}

		public static async Task<string> GetAsync(string method) {
			try {
				return await Client.DownloadStringTaskAsync(Address + method);
			} catch {
				return null;
			}
		}

		public static async Task<string> PostAsync(string method, NameValueCollection data) {
			try {
				byte[] bytes = await Client.UploadValuesTaskAsync(Address + method, data);
				return BytesToString(bytes);
			} catch {
				return null;
			}
		}

		public static string BytesToString(byte[] bytes) {
			using(var stream = new MemoryStream(bytes)) {
				using(var streamReader = new StreamReader(stream)) {
					return streamReader.ReadToEnd();
				}
			}
		}
	}
}