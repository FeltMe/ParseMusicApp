using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using WpfTestMusicParseApp.Classes;

namespace WpfTestMusicParseApp {

	public partial class MainWindow : Window {

		#region fields
		/// <summary>
		/// Collection that realize observable pattern and hold sound data
		/// </summary>
		private ObservableCollection<SounInfo> observable = new ObservableCollection<SounInfo>();
		/// <summary>
		/// Url address to parse info
		/// </summary>
		private string link;
		#endregion

		#region ctors

		/// <summary>
		/// Initialize component and set item source of ListView to our collection
		/// </summary>
		public MainWindow() {
			InitializeComponent();
			SoundsList.ItemsSource = observable;
		}
		#endregion

		#region WindowEvents
		/// <summary>
		/// Handles events for adding and removing elements
		/// </summary>
		/// <param name="sender">Instance of <see cref="Button"/> that called the event</param>
		/// <param name="e">Arguments passed by sender for subscribers</param>
		private void Button_Click(object sender, RoutedEventArgs e) {
			observable.Clear();
			if (IsValidLink(LinkBox.Text)) {
				link = LinkBox.Text;
				StartClientAndTakeDatas();
			} else
				MessageBox.Show("Enter valid link");

		}
		#endregion

		#region BuisnesFuckingLogic

		//todo Move this logic to new sln and connect there is to just take datas



		/// <summary>
		/// Start new HTTPClient wich realize <see cref="IDisposable"/> to connect to website
		/// </summary>
		private async void StartClientAndTakeDatas() {
			using (HttpClient client = new HttpClient()) {
				await GetContectAsync(client);
			}
		}


		/// <summary>
		/// Async parse all HTML to string, than add it to <see cref="HtmlDocument"/>
		/// Than try to find Artist name and his sounds by using HtmlAgilityPack method SelectNode\SelectSingleNode/>
		/// And add item's as new <see cref="SounInfo"/> observable/>
		/// </summary>
		/// <param name="client">Web client, which we are working with</param>
		/// <exception cref="Exception">Thrown if parameter was null or other exceptions</exception>
		/// <returns> <see cref="Task"/> which do all dirty work</returns>
		private async Task GetContectAsync(HttpClient client) {
			try {
				HttpResponseMessage response = await client.GetAsync(link);
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();

				var doc = new HtmlDocument();
				doc.LoadHtml(responseBody);

				var sound_names = doc.DocumentNode.SelectNodes("//a[contains(@class, 'list-group-item removehref')]");

				var header = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'col-md-9')]").ChildNodes;
				var name_artist = header[1].InnerText;

				foreach (var item in sound_names) {
					observable?.Add(new SounInfo() { ArtistName = name_artist, SongName = item.ChildNodes[1].InnerText.Trim() });
				}

			} catch {
				MessageBox.Show("No artist found, try enter valid link or artist name");
			}
		}

		/// <summary>
		/// Check for valid link
		/// </summary>
		/// <param name="myLink">Link to check</param>
		/// <returns>True if link valid, false if link is invalid</returns>
		private bool IsValidLink(string myLink) {
			Uri uriResult;
			return myLink.StartsWith("http://groovesharks.org/artist")
			//Hardcode 
			&& Uri.TryCreate(myLink, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
		}
		#endregion
	}
}
