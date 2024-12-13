using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using HandyControl.Controls;
using SpotifyAPI.Web;

namespace MyWidget
{
    /// <summary>
    /// Interaction logic for songCard.xaml
    /// </summary>
    public partial class songCard : UserControl
    {
        public songCard()
        {
            InitializeComponent();
            
        }

        public string TrackUri { get; set; }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            GoToLink();
        }

        private void songName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void GoToLink()
        {
            ProcessStartInfo linkx = new(linkText.Text)
            {
                UseShellExecute = true
            };
            Process.Start(linkx);
        }

        private async void linkButton_Click(object sender, RoutedEventArgs e)
        {

            //Window1.GetAccessToken()

            //Window1.GetAccessToken(acce);

            HttpClient client = new HttpClient();
            string accessToken =  "BQDYX0PxXZCRLSajIYlk7tmdaJG77DJcqXwv4qu_xlwDvmqEis4RlO5S6ZILUk3QbMeX7sq2qXstaSizyTxaDLZFrZu-dL2VdxGl2IJlMw71cPlj8JPYlzXgyZiUhc00o46PZNOxdeAmB7Klph2Jze35H1tJxtFXOVtDMimjvIjVXeC2_IxRhMJpAyiQOk_49J3WnpPWTAdxYd0";
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            string playUrl = "https://api.spotify.com/v1/me/player/play";
            string playBody = $"{{\"uris\": [\"{TrackUri}\"]}}";

            HttpContent content = new StringContent(playBody, Encoding.UTF8, "application/json");
            
            HttpResponseMessage playResponse = await client.PutAsync(playUrl, content);

            if (playResponse.IsSuccessStatusCode)
                Console.WriteLine("Şarkı çalmaya başladı!");
            else
                Console.WriteLine("Hata: " + await playResponse.Content.ReadAsStringAsync());


            //GoToLink();
        }
    }
}
