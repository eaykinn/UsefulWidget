using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;


namespace MyWidget
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<FullTrack> tracks;
        string sLink;
        public string clientId;
        public string clientSecret;
        public static string? accessToken;
        SpotifyClient spotify;
        public Window1(string searchQuery, SolidColorBrush color)
        {
            this.searchQuery = searchQuery;
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
            toprect.Fill = x;

            GetMusicList();
        }
        private string searchQuery {  get; set; }   
        
        private async void UpdateLabels()
        {

            foreach (FullTrack track in tracks)
            {
                
                songCard songCard = new songCard();
                songCard.songName.Text = track.Name;
                songCard.artistName.Text = track.Artists[0].Name;
                songCard.albumName.Text = track.Album.Name;
                songCard.TrackUri = track.Uri;
                sLink = track.ExternalUrls.Values.First().ToString();
                Uri songUrl = new(sLink);
                songCard.songHyperLink.NavigateUri = songUrl;          
                songCard.linkText.Text = sLink;
                ListBoxItem songItem = new ListBoxItem();
                songItem.Content = songCard;
                songItem.Background = null;
                songCard.songImage.Source = await GetPicofTrack(track.Id);
              
                songList.Items.Add(songItem);
            }

            /*Uri iconExpjsonUri = new Uri("pack://application:,,,/MyWidget;component/Resources/index.html", UriKind.RelativeOrAbsolute);
            StreamResourceInfo expresourceInfo = Application.GetResourceStream(iconExpjsonUri);
            string expjsonContent;
            using (StreamReader reader = new StreamReader(expresourceInfo.Stream))
            {
                expjsonContent = reader.ReadToEnd();
            }
            await webView2.EnsureCoreWebView2Async();
            webView2.NavigateToString(expjsonContent);*/

            string localHtmlPath = Path.GetFullPath("C:\\Users\\PC_3741\\source\\repos\\eaykinn\\UsefulWidget\\MyWidget\\Resources\\index.html");
            string localHtmlUri = new Uri(localHtmlPath).AbsoluteUri;

            await webView2.EnsureCoreWebView2Async();
            // Yerel HTML dosyasını yükle
            webView2.CoreWebView2.Navigate(localHtmlUri);

        }

        private async Task<BitmapImage> GetPicofTrack(string songId)
        {
            var fullTrack = await spotify.Tracks.Get(songId);
            var imageUrl = fullTrack.Album.Images[0].Url;
            return await LoadImageFromUrl(imageUrl);
       
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo link = new(sLink)
            {
                UseShellExecute = true
            };
            Process.Start(link);

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
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            return bitmap;
            // İndirilen resmi Image kontrolüne ata
           
        }

        async Task GetMusicList()
        {
            Uri iconExpjsonUri = new Uri("pack://application:,,,/MyWidget;component/Resources/api_key.json", UriKind.RelativeOrAbsolute);
            StreamResourceInfo expresourceInfo = Application.GetResourceStream(iconExpjsonUri);
            string expjsonContent;
            using (StreamReader reader = new StreamReader(expresourceInfo.Stream))
            {
                expjsonContent = reader.ReadToEnd();
            }

            JObject expjson = JObject.Parse(expjsonContent);


            clientId = expjson["clientId"].ToString();
            clientSecret = expjson["clientSecret"].ToString();

            accessToken = await GetAccessToken(clientId, clientSecret);
            spotify = new SpotifyClient(accessToken);

            SearchRequest searchRequest = new (SearchRequest.Types.Track, searchQuery);
            searchRequest.Limit = 10;
           
            var searchResponse = await spotify.Search.Item(searchRequest);
            var fullTracks = searchResponse.Tracks;
            tracks = fullTracks.Items;
            UpdateLabels();
 
        }

        public static async Task<string> GetAccessToken(string clientId, string clientSecret)
        {

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),

            });


                var response = await client.PostAsync("https://accounts.spotify.com/api/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    JObject x = JObject.Parse(tokenResponse);
                    string accessToken = x["access_token"].ToString();
                    return accessToken;
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve access token: {response.ReasonPhrase}");
                    return null;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void toprect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
