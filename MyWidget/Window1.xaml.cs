﻿using System;
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
using System.IO;
using System.Windows.Resources;
using Swan.Logging;
using Swan;

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
        public Window1(string searchQuery, SolidColorBrush color)
        {
            this.searchQuery = searchQuery;
            InitializeComponent();
            var color2 = Colors.Black;

            var startPoint = new System.Windows.Point();
            startPoint.X = 0.5;
            startPoint.Y = 0;

            var endPoint = new System.Windows.Point();
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
            Uri keyUri = new Uri("pack://application:,,,/MyWidget;component/Resources/api_key.json", UriKind.RelativeOrAbsolute);

            StreamResourceInfo expresourceInfo = null;
            try
            {
                expresourceInfo = Application.GetResourceStream(keyUri);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Error);
            }
      
            string jsonStr;

            using (StreamReader reader = new StreamReader(expresourceInfo!.Stream))
                {
                    jsonStr = reader.ReadToEnd();
                }

            JObject expjson = JObject.Parse(jsonStr);
            JObject key = expjson;
            clientId = (string)key["clientId"];
            clientSecret = (string)key["clientSecret"];
            accessToken = await GetAccessToken(clientId, clientSecret);
            spotify = new SpotifyClient(accessToken);

          

            SearchRequest searchRequest = new (SearchRequest.Types.Track, searchQuery);
            searchRequest.Limit = 10;
           
            var searchResponse = await spotify.Search.Item(searchRequest);
            var fullTracks = searchResponse.Tracks;
            tracks = fullTracks.Items;
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
