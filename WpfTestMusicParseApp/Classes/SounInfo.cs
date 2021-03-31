using System;
using System.Collections.Generic;
using System.Text;

namespace WpfTestMusicParseApp.Classes {
	class SounInfo {
		private string _artistName; 
		private string _songName;

		public string ArtistName {
			get { return _artistName; }
			set { _artistName = value; }
		}

		public string SongName {
			get { return _songName; }
			set { _songName = value; }
		}

	}
}
