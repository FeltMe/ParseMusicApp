using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using WpfTestMusicParseApp.Classes;

namespace WpfTestMusicParseApp
{

	public partial class MainWindow : Window {

		#region fields
		/// <summary>
		/// Collection that realize observable pattern and hold sound data
		/// </summary>
		private readonly ObservableCollection<SounInfo> observableCollectionSounds = new();
		/// <summary>
		/// Url address to parse info
		/// </summary>
		private string Link { get; set; }
		/// <summary>
		/// https://docs.microsoft.com/ru-ru/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
		/// </summary>
		private readonly HttpClient _httpClient;
		#endregion

		#region ctors

		/// <summary>
		/// Initialize component and set item source of ListView to our collection
		/// </summary>
		public MainWindow() {
			InitializeComponent();
			SoundsList.ItemsSource = observableCollectionSounds;
			_httpClient = new HttpClient();
		}
		#endregion

		#region WindowEvents
		/// <summary>
		/// Handles events for adding and removing elements
		/// </summary>
		/// <param name="sender">Instance of <see cref="Button"/> that called the event</param>
		/// <param name="e">Arguments passed by sender for subscribers</param>
		private void Button_Click(object sender, RoutedEventArgs e) {
			observableCollectionSounds.Clear();
			if (IsValidLink(LinkBox.Text)) {
				Link = LinkBox.Text;
				StartClientAndTakeDatas();
			} else
				MessageBox.Show("Enter valid link");
		}
		#endregion

		#region BuisnesFuckingLogic
		/// <summary>
		/// Start new HTTPClient wich realize <see cref="IDisposable"/> to connect to website
		/// </summary>
		private async void StartClientAndTakeDatas() {
			await GetContectAsync(_httpClient);
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
				HttpResponseMessage response = await client.GetAsync(Link);
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();

				var htmlDocument = new HtmlDocument();
				htmlDocument.LoadHtml(responseBody);

				var soundNames = htmlDocument.DocumentNode.SelectNodes("//a[contains(@class, 'list-group-item removehref')]");

				var header = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'col-md-9')]");

				if (soundNames != null && header != null) {
					var artistName = header.ChildNodes[1].InnerText;
					foreach (var item in soundNames) {
						observableCollectionSounds?.Add(new SounInfo() { ArtistName = artistName, SongName = item.ChildNodes[1].InnerText.Trim() });
					}
				}
			} catch (Exception) {
				Exception ex;
				MessageBox.Show("smth went wrong, call your sysadmin xD" + nameof(ex));
			}
		}

		/// <summary>
		/// Check for valid link
		/// </summary>
		/// <param name="link">Link to check</param>
		/// <returns>True if link valid, false if link is invalid</returns>
		private bool IsValidLink(string link) {
			return link.StartsWith("http://groovesharks.org/artist")
			//Hardcode 
			&& Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
		}
		#endregion
	}
}
