using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using static MyWidget.SpotifyGetAccessToken;
using static SpotifyAPI.Web.PlaylistRemoveItemsRequest;

namespace MyWidget
{
    public class GeniusApiAccesToket
    {
        public class GeniusApiResponse
        {
            public string response { get; set; }
        }

        private readonly string clientAccessToken =
            "qDID9TaTLVHgjBfR0LyCu1YjZ6Xfv9TY4bgROy0rCcV_FQHd-juRd7SH3ut6L8UU";

        public async Task<string> SearchSongAsync(string songQuery, string artistName)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                clientAccessToken
            );

            string url = $"https://api.genius.com/search?q={Uri.EscapeDataString(songQuery)}";

            var response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            var data = JObject.Parse(json);

            var hits = data["response"]["hits"];

            int index = -1;
            int maxAnnInd = -1;
            int annCount = 0;
            foreach (var item in hits)
            {
                index += 1;
                int ac = (int)item["result"]["annotation_count"];
                if (item["result"]["artist_names"].ToString().Contains(artistName) && annCount < ac)
                {
                    annCount = ac;
                    maxAnnInd = index;
                }
            }

            if (maxAnnInd == -1)
            {
                return "Şarkı Sözü Bulunamadı";
            }
            string a = hits[maxAnnInd]["result"]["url"].ToString();

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(a);

            // 4. Lyrics div'ini parse et
            // Genius sayfasında lyrics div sınıfı değişebilir, farklı versiyonlar olabilir
            var lyricSpans = doc.DocumentNode.SelectNodes(
                "//span[contains(@class,'ReferentFragment-desktop__Highlight')]"
            );

            var sb = new StringBuilder();
            if (lyricSpans == null)
            {
                var lyricSpans2 = doc.DocumentNode.SelectNodes(
                    "//div[contains(@class,'Lyrics__Container')]"
                );

                if (lyricSpans2 == null)
                    return "Lyrics bulunamadı.";
                else
                {
                    var span = lyricSpans2[0];

                    foreach (var item in span.ChildNodes)
                    {
                        string xx = item.InnerText.Trim();
                        if (xx != "")
                        {
                            string yy = WebUtility.HtmlDecode(xx);
                            if (yy.Contains("<br>"))
                            {
                                yy = yy.Replace("<br>", "\n");
                            }
                            sb.AppendLine(yy);
                            if (!yy.Contains("\n"))
                            {
                                sb.AppendLine("\n");
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var span in lyricSpans)
                {
                    string xx = span.InnerHtml.Trim();
                    if (xx != "")
                    {
                        string yy = WebUtility.HtmlDecode(xx);
                        if (yy.Contains("<br>"))
                        {
                            yy = yy.Replace("<br>", "\n");
                        }
                        sb.AppendLine(yy);

                        if (!yy.Contains("\n"))
                        {
                            sb.AppendLine("\n");
                        }
                    }
                }
            }

            string x = sb.ToString();

            return x;
        }
    }
}
