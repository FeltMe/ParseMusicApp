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
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary
	public partial class MainWindow : Window {

		private ObservableCollection<SounInfo> observable = new ObservableCollection<SounInfo>();
		private static bool LinkCheck = false;
		private string link = "http://groovesharks.org/artist/Beyonc%C3%A9";

		public MainWindow() {
			InitializeComponent();
			SoundsList.ItemsSource = observable;
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			observable.Clear();
			if (IsValidLink(LinkBox.Text)) {
				link = LinkBox.Text;
				GetSoundNodes();
			} else
				MessageBox.Show("Enter valid link");

		}

		private async void GetSoundNodes() {
			using (HttpClient client = new HttpClient()) {
				await GetContectAsync(client);
			}
		}

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

			} catch (NullReferenceException ex) {
				MessageBox.Show("No artist found, try enter valid link or artist name" + nameof(ex));
			}
		}

		private bool IsValidLink(string myLink) {
			return myLink.StartsWith("http://") && myLink.Contains(".org");
		}

	}
}
