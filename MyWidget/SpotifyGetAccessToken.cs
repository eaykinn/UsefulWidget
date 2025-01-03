using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            HttpListenerContext context;

            try
            {
                context = listener.GetContext();
            }
            catch (Exception)
            {

                throw;

            }

           
            string code = context.Request.QueryString["code"];
         

            if (code == null)
            {
                return "No auth";
            }
            var response = context.Response;
          
            string responseString = "Authorization successful. You can close this window.";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            var z = context.Request.QueryString;
            listener.Stop();
            
            return code;
        }

        public static async Task<List<string>> GetUserPerm(int getAuth)
        {
            string code;
            List<string> y = new();
            List<string> accessToken = new();
            List<string> x = new();

            GetClientIdandClientSecret();

            if (getAuth == 0) 
            {   
                if(Properties.Settings.Default.refreshToken == "") 
                {
                    code = StartHttpListener();
                    y = await GetAccessToken(code);
                    x.Add(y[0]);
                }
                else
                {
                    code = await RefreshToken(clientId, clientSecret, Properties.Settings.Default.refreshToken);
                    if(code == "")
                    {
                        await GetUserPerm(1);
                    }
                    else
                    {
                        x.Add(code);
                    }
                    
                }
                
            }
            else
            {
                code = StartHttpListener();
                y = await GetAccessToken(code);
                x.Add(y[0]);

            }

            return x;

        }

        public async static Task<string> RefreshToken(string clientId, string clientSecret, string refreshToken)
        {   
            HttpClient httpClient = new HttpClient();   
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");

            var authHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            request.Content = new FormUrlEncodedContent(new[]
            {
                 new KeyValuePair<string, string>("grant_type", "refresh_token"),
                 new KeyValuePair<string, string>("refresh_token", refreshToken),
             });

            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenResponse>(responseBody);
                return tokenResponse?.access_token;
            }
            else
            {
                return "";
            }
        }

        public class SpotifyTokenResponse
        {
            public string access_token { get; set; }
            public string TokenType { get; set; }
            public int ExpiresIn { get; set; }
        }

        public class SpToken
        {   
            public string access_token { get; set; }
            public string refresh_token { get; set; }

        }

        public static async Task<List<string>> GetAccessToken(string code)
        {

            List<string> tokens = new();
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
                    SpToken json = JsonSerializer.Deserialize<SpToken>(responseContent);
                    tokens.Add(json.access_token);
                    tokens.Add(json.refresh_token);

                    Properties.Settings.Default.accessTokenSet = tokens[0];
                    Properties.Settings.Default.refreshToken = tokens[1];
                    Properties.Settings.Default.Save();
                    // tokens[0] = (String)json.access_token;
                    //tokens[1] = (String)json.refresh_token;
                    return tokens;
                }
                else
                {
                    Console.WriteLine("Error: " + responseContent);
                    return tokens;
                }
            }
        }

    }

}
