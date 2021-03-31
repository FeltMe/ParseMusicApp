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

		public MainWindow() {
			InitializeComponent();
			SoundsList.ItemsSource = observable;
		}

		private async void Button_Click(object sender, RoutedEventArgs e) {
			GetSoundNodes();
		}

		private async void GetSoundNodes() {
			using (HttpClient client = new HttpClient()) {
				await GetContectAsync(client);
			}
		}

		private async Task GetContectAsync(HttpClient client) {
			try {
				HttpResponseMessage response = await client.GetAsync("http://groovesharks.org/artist/Lady-Gaga");
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();

				var doc = new HtmlDocument();
				doc.LoadHtml(responseBody);

				var sound_names = doc.DocumentNode.SelectNodes("//a[contains(@class, 'list-group-item removehref')]");

				var header = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'col-md-9')]").ChildNodes;
				var name_artist = header[1].InnerText;

				foreach (var item in sound_names) {
					observable.Add(new SounInfo() { ArtistName = name_artist, SongName = item.ChildNodes[1].InnerText.Trim() });
				}

			} finally {
				Console.WriteLine("Done");
			}
		}

	}
}
