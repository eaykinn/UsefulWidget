using HandyControl.Controls;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyWidget
{
    /// <summary>
    /// Interaction logic for PlayList.xaml
    /// </summary>
    public partial class PlayList : System.Windows.Window
    {
        public PlayList(SolidColorBrush color)
        {
            InitializeComponent();

            Color color2 = Colors.Black;

            Point startPoint = new Point();
            startPoint.X = 0.5;
            startPoint.Y = 0;

            Point endPoint = new Point();
            endPoint.X = 0.5;
            endPoint.Y = 1;

            GradientStop gradientStop = new GradientStop();
            gradientStop.Color = color2;

            GradientStop gradientStop2 = new GradientStop();
            System.Windows.Media.Color xasd = new System.Windows.Media.Color();
            xasd.A = color.Color.A;
            xasd.B = color.Color.B;
            xasd.R = color.Color.R;
            xasd.G = color.Color.G;

            gradientStop2.Color = xasd;
            gradientStop2.Offset = 1;

            IEnumerable<GradientStop> coll = [gradientStop, gradientStop2];

            var z = new GradientStopCollection(coll);

            var x = new LinearGradientBrush(z, startPoint, endPoint);
            this.Background = x;


            playListBox = this.FindName("playListBox") as ListBox;
            Task.Run(() => GetPlayLists());
        }





        async Task GetPlayLists()
        {
            string accessToken;
            string refreshToken;
            List<string> tokens = new List<string>();
            if (Properties.Settings.Default.accessTokenSet != "")
            {
                accessToken = Properties.Settings.Default.accessTokenSet;
                refreshToken = Properties.Settings.Default.refreshToken;
                using (HttpClient clientS = new HttpClient())
                {
                    clientS.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    HttpResponseMessage response = await clientS.GetAsync(
                        "https://api.spotify.com/v1/me"
                    );

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Token geçerli!");
                    }
                    else
                    {
                        tokens = await SpotifyGetAccessToken.GetUserPerm(0);
                        accessToken = tokens[0];
                        //refreshToken = tokens[1];
                        Properties.Settings.Default.accessTokenSet = tokens[0];
                        //Properties.Settings.Default.refreshToken = tokens[1];
                        Properties.Settings.Default.Save();
                    }
                }
            }
            else
            {
                tokens = await SpotifyGetAccessToken.GetUserPerm(1);
                accessToken = tokens[0];
                refreshToken = tokens[1];
                Properties.Settings.Default.accessTokenSet = tokens[0];
                Properties.Settings.Default.refreshToken = tokens[1];
                Properties.Settings.Default.Save();
            }

            var spotify = new SpotifyClient(accessToken);

            var playlists = await spotify.Playlists.CurrentUsers();

            foreach (var item in playlists.Items)
            {
                await UpdateList(item);
            }
            return;
        }

        async Task UpdateList(FullPlaylist playlist)
        {
            // Make sure this runs on the UI thread
            _ = playListBox.Dispatcher.Invoke(async () =>
            {   
                albumCard card = new albumCard();
                card.albumName.Text = playlist.Name;
                card.ownerName.Text = playlist.Owner.DisplayName;
                card.trackCount.Text = playlist.Tracks.Total.ToString() + " Songs";
                var img = await LoadImageFromUrl(playlist.Images[0].Url);
              
                System.Windows.Controls.Image image = new();
                card.songImage.Source = img;

                card.PlayListUri = playlist.Uri;

                var sLink = playlist.ExternalUrls.Values.First().ToString();
                Uri songUrl = new(sLink);
                card.songHyperLink.NavigateUri = songUrl;
                card.linkText.Text = sLink;


                playListBox.Items.Add(card);
            });
        }

        private async Task<BitmapImage> LoadImageFromUrl(string url)
        {
            BitmapImage bitmap = new BitmapImage();
            

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                }
                catch (Exception ex)
                {
                    HandyControl.Controls.MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            return bitmap;
            // İndirilen resmi Image kontrolüne ata
        }

        private void kapat(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void toprect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


    }
}
