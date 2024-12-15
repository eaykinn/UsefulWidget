using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;

namespace MyWidget
{
    internal class SpotifyGetAccessToken
    {
        public static string clientId { get; set; }
        public static string clientSecret { get; set; }


        static void GetClientIdandClientSecret()
        {
            string result;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "MyWidget.Resources.api_key.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            JObject expjson = JObject.Parse(result);


            clientId = expjson["clientId"].ToString();
            clientSecret = expjson["clientSecret"].ToString();
        }


        public static string StartHttpListener()
        {
            string redirectUri = "http://localhost:5533/callback/";
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri);
            listener.Start();


            /* HttpListenerContext context = null;*/
            Console.WriteLine("Waiting for redirect...");


            string scopes = "streaming user-read-playback-state user-modify-playback-state";

            string authorizationUrl = $"https://accounts.spotify.com/authorize" +
                                          $"?client_id={clientId}" +
                                          $"&response_type=code" +
                                          $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                                          $"&scope={Uri.EscapeDataString(scopes)}";

            Console.WriteLine("Go to this URL and authorize the application:");
            //MessageBox.Show(authorizationUrl);
            //Console.WriteLine(authorizationUrl);
            ProcessStartInfo linkx = new(authorizationUrl)
            {
                UseShellExecute = true
            };
            Process.Start(linkx);

            var context = listener.GetContext();


            string code = context.Request.QueryString["code"];

            var response = context.Response;
            string responseString = "Authorization successful. You can close this window.";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            listener.Stop();
            return code;
        }

        public static async Task<string> GetUserPerm()
        {
            GetClientIdandClientSecret();

            string code = StartHttpListener();



            string x = await GetAccessToken(code);
            return x;

        }


        public static async Task<string> GetAccessToken(string code)
        {

            string redirectUri = "http://localhost:5533/callback/";

            using (var client = new HttpClient())
            {
                var authHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri)
        });

                var response = await client.PostAsync("https://accounts.spotify.com/api/token", content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                    return json.access_token;
                }
                else
                {
                    Console.WriteLine("Error: " + responseContent);
                    return null;
                }
            }
        }

    }

}
