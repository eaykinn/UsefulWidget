using HandyControl.Controls;
using Newtonsoft.Json;
using NPSMLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

namespace MyWidget
{
    /// <summary>
    /// Interaction logic for albumCard.xaml
    /// </summary>
    public partial class albumCard : UserControl
    {
        public class Device
        {
            public string id { get; set; }
            public bool is_active { get; set; }
            public string name { get; set; }
            // Add other properties as needed
        }

        public class DevicesResponse
        {
            public List<Device> devices { get; set; }
        }

        public albumCard()
        {
            InitializeComponent();
            
        }

        public string PlayListUri { get; set; }
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
            string accessToken;
            string refreshToken;
            List<string> tokens;

            if (Properties.Settings.Default.accessTokenSet != "")
            {
                accessToken = Properties.Settings.Default.accessTokenSet;
                refreshToken = Properties.Settings.Default.refreshToken;

                using (HttpClient clientS = new HttpClient())
                {
                    clientS.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    HttpResponseMessage response = await clientS.GetAsync("https://api.spotify.com/v1/me");

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Token geçerli!");
                    }
                    else
                    {
                        tokens = await SpotifyGetAccessToken.GetUserPerm(0);
                        accessToken = tokens[0];
                        //refreshToken = tokens[1];
                        Properties.Settings.Default.accessTokenSet = accessToken;
                        //Properties.Settings.Default.refreshToken = accessToken;
                        Properties.Settings.Default.Save();
                    }
                }

            }
            else
            {
                tokens = await SpotifyGetAccessToken.GetUserPerm(1);
                accessToken = tokens[0];
                refreshToken = tokens[1];
                Properties.Settings.Default.accessTokenSet = accessToken;
                Properties.Settings.Default.refreshToken = accessToken;
                Properties.Settings.Default.Save();
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            string playUrl = "https://api.spotify.com/v1/me/player/play";
            string playBody = $"{{\"context_uri\": \"{PlayListUri}\"}}";

            HttpContent content = new StringContent(playBody, Encoding.UTF8, "application/json");
            
            // Cihazları al
            var devicesResponse = await client.GetAsync("https://api.spotify.com/v1/me/player/devices");
            var devicesJson = await devicesResponse.Content.ReadAsStringAsync();
            var devicesObj = JsonConvert.DeserializeObject<DevicesResponse>(devicesJson);
            string actDevId;

            if (devicesObj != null && devicesObj.devices.Count() > 0) {
                var activeDevice = devicesObj.devices.FirstOrDefault();
                if(activeDevice.is_active == false)
                {
                    actDevId = activeDevice.id;
                    string transferUrl = "https://api.spotify.com/v1/me/player";
                    string deviceId = actDevId; // Seçtiğin cihazın id'si
                    string transferBody = $"{{\"device_ids\": [\"{deviceId}\"], \"play\": true}}";
                    HttpContent transferContent = new StringContent(transferBody, Encoding.UTF8, "application/json");
                    await client.PutAsync(transferUrl, transferContent);

                }


            }
            else
            {   
                StartSpotify.Start();

                var dvcResp = await client.GetAsync("https://api.spotify.com/v1/me/player/devices");
                var dvc = await dvcResp.Content.ReadAsStringAsync();
                var dvcObj = JsonConvert.DeserializeObject<DevicesResponse>(dvc);
                var actDI = dvcObj.devices.FirstOrDefault();
                string transferUrl = "https://api.spotify.com/v1/me/player";
                string deviceId = actDI.id; // Seçtiğin cihazın id'si
                string transferBody = $"{{\"device_ids\": [\"{deviceId}\"], \"play\": true}}";
                HttpContent transferContent = new StringContent(transferBody, Encoding.UTF8, "application/json");
                await client.PutAsync(transferUrl, transferContent);
                Thread.Sleep(1500);

            }
         


            HttpResponseMessage playResponse = await client.PutAsync(playUrl, content);

            if (playResponse.IsSuccessStatusCode)
                Console.WriteLine("Şarkı çalmaya başladı!");
            else
                Console.WriteLine("Hata: " + await playResponse.Content.ReadAsStringAsync());

        }

       
    }
}
