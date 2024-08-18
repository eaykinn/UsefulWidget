using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
using System.Windows.Shapes;
using HandyControl;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Security.Policy;

namespace MyWidget
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<FullTrack> tracks;
        string sLink;
        string clientId;
        string clientSecret;
        string accessToken;
        SpotifyClient spotify;
        public Window1(string searchQuery)
        {
            this.searchQuery = searchQuery;
         
            InitializeComponent();
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
            
        }

        private async Task<BitmapImage> GetPicofTrack(string songId)
        {
            var xxxxxxxx = await spotify.Tracks.Get(songId);
            var uuuu = xxxxxxxx.Album.Images[0].Url;
            return await LoadImageFromUrl(uuuu);
       
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
            clientId = "57d565246d7d41d2b8dadd774621c2b1";
            clientSecret = "514cc776fc66433a971fa335b0bb624a";
            accessToken = await GetAccessToken(clientId, clientSecret);
            spotify = new SpotifyClient(accessToken);

            SearchRequest searchRequest = new (SearchRequest.Types.Track, searchQuery);
            searchRequest.Limit = 5;
           
            var searchResponse = await spotify.Search.Item(searchRequest);
            var xxxx = searchResponse.Tracks;
            tracks = xxxx.Items;
            UpdateLabels();
 
        }

        static async Task<string> GetAccessToken(string clientId, string clientSecret)
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
    }
}
