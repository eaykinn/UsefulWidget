using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Threading;
using HandyControl.Controls;
using HandyControl.Tools;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using NPSMLib;

namespace MyWidget
{
    public partial class MainWindow : System.Windows.Window
    {
        private static string? _lat;
        private static string? _lon;
        private readonly List<string> cityNames = [];
        private readonly List<List<string>> cordList = [];
        private int ctHour = 0;
        private readonly DispatcherTimer ctTimer = new();
        private int ctMin = 0;
        private int ctSec = 0;
        private int currVol;
        private System.Drawing.Color defCol;
        private readonly System.Windows.Controls.Image img = new();
        private readonly System.Windows.Controls.Image imageSource = new();
        private byte isMuted;
        private string lblSecOfTheSong;
        private readonly string path1 = Directory.GetCurrentDirectory();
        private byte restartProcessStarted = 0;
        private Brush themeColor;
        private readonly DispatcherTimer timer = new();
        bool colorPiclerOpened;
        JObject weatherCodes;
        JObject weatherExpCodes;
        private bool AutoStopMusicSet;
        Window1 window1;

        private const int HOTKEY_ID = 9029;
        private const int HOTKEY2_ID = 9061; // Benzersiz bir ID
        private const uint VK_F9 = 0x78; // F9 tuşu
        private const uint VK_F12 = 0x7A; // F12 tuşu

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.ToString());
                Console.ReadLine();
            }

            Loaded += MainWindow_Loaded2;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded2(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            RegisterHotKey(helper.Handle, HOTKEY_ID, 0, VK_F9);
            RegisterHotKey(helper.Handle, HOTKEY2_ID, 0, VK_F12); // Ctrl+Alt+F9
            ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
            UnregisterHotKey(helper.Handle, HOTKEY2_ID);
        }

        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != 0x0312) // Sadece WM_HOTKEY
                return;

            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;
            if (player.Count == 0)
            {
                return;
            }
            MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();

            if (msg.message == 0x0312) // WM_HOTKEY
            {
                int id = msg.wParam.ToInt32();
                if (id == HOTKEY_ID)
                {
                    handled = true;
                    x.SendMediaPlaybackCommand(MediaPlaybackCommands.Previous);
                }
                else if (id == HOTKEY2_ID)
                {
                    handled = true;
                    x.SendMediaPlaybackCommand(MediaPlaybackCommands.Next);
                }
            }
            GetCurrentMedia(false, true);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //timepicker.GettTime();
            ctHour = Convert.ToInt16(hourCmbx.SelectedValue);
            ctMin = Convert.ToInt16(minCmbx.SelectedValue);
            if (ctHour != 0 || ctMin != 0)
            {
                Process.Start("shutdown", "/s /t " + ((ctHour * 3600) + ctMin * 60).ToString());
                restartProcessStarted = 1;
                if (ctHour.ToString().Length == 1)
                {
                    hTxtBox.Text = "0" + ctHour.ToString();
                }
                else
                {
                    hTxtBox.Text = ctHour.ToString();
                }

                if (ctMin.ToString().Length == 1)
                {
                    mTxtBox.Text = "0" + ctMin.ToString();
                }
                else
                {
                    mTxtBox.Text = ctMin.ToString();
                }

                if (ctHour.ToString().Length == 1)
                {
                    hTxtBox.Text = "0" + ctHour.ToString();
                }
                else
                {
                    hTxtBox.Text = ctHour.ToString();
                }

                sTxtBox.Text = "00";
                CountDowntimer(1);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (colorPiclerOpened == false)
            {
                OpenCloseColorPicker(false);
                colorPiclerOpened = true;
            }
            else
            {
                OpenCloseColorPicker(true);
                colorPiclerOpened = false;
            }
        }

        private void OpenCloseColorPicker(bool isOpened)
        {
            if (isOpened == false)
            {
                themeColor = border.Background;
                themeColorPicker.Visibility = Visibility.Visible;
                Grid.SetColumnSpan(themeColorPicker, 8);
                Grid.SetRowSpan(themeColorPicker, 15);
            }
            else
            {
                themeColorPicker.Visibility = Visibility.Hidden;
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PlayStopMedia();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("shutdown", "/a");
            hTxtBox.Text = "00";
            mTxtBox.Text = "00";
            sTxtBox.Text = "00";
            ctSec = 0;
            ctMin = 0;
            ctHour = 0;
            restartProcessStarted = 0;
            CountDowntimer(0);
        }

        private void ChangeTheme(SolidColorBrush color)
        {
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
            border.Background = x;
            toprect.Fill = x;

            //searchBarTxt.Background = color;
            // searchBarTxt.BorderBrush = Brushes.White;

            SetColorSettings(color);
        }

        private void CountDowntimer(int start)
        {
            if (start == 1)
            {
                ctTimer.Start();
            }
            else
            {
                ctTimer.Stop();
            }
        }

        private void CtTimerTick(object sender, EventArgs e)
        {
            if (ctSec > 0)
            {
                ctSec = ctSec - 1;
            }
            else
            {
                ctSec = 59;

                if (ctMin > 0)
                {
                    ctMin = ctMin - 1;
                }
                else
                {
                    if (ctHour > 0)
                    {
                        ctHour = ctHour - 1;
                        ctMin = 59;
                    }
                }
            }

            if (ctHour.ToString().Length == 1)
            {
                hTxtBox.Text = "0" + ctHour.ToString();
            }
            else
            {
                hTxtBox.Text = ctHour.ToString();
            }

            if (ctSec.ToString().Length == 1)
            {
                sTxtBox.Text = "0" + ctSec.ToString();
            }
            else
            {
                sTxtBox.Text = ctSec.ToString();
            }

            if (ctMin.ToString().Length == 1)
            {
                mTxtBox.Text = "0" + ctMin.ToString();
            }
            else
            {
                mTxtBox.Text = ctMin.ToString();
            }
        }

        private protected async Task GetCurrentMedia(bool onLoad, bool getTimeLine)
        {
            if (onLoad == false)
            {
                Thread.Sleep(50);
            }

            NowPlayingSessionManager player = new NowPlayingSessionManager();

            NowPlayingSession[] sessions = player.GetSessions();
            var sessionInfos = sessions
                .Where(x =>
                    x.SourceAppId == "Spotify.exe"
                    || x.SourceAppId.Contains("spotify")
                    || x.SourceAppId.Contains("Spotify")
                )
                .Select(x => x.GetSessionInfo())
                .ToList();
            if (sessionInfos.Count == 0)
            {
                return;
            }
            else
            {
                player.SetCurrentSession(sessionInfos[0]);
            }
            NowPlayingSession currentSession = player.CurrentSession;
            MediaPlaybackDataSource playnaclDataSource =
                currentSession.ActivateMediaPlaybackDataSource();

            await GetTimeLinePosition(playnaclDataSource, true);

            Stream streamInfo = playnaclDataSource.GetThumbnailStream();
            if (streamInfo != null)
            {
                songImage.Source = BitmapFrame.Create(
                    streamInfo,
                    BitmapCreateOptions.None,
                    BitmapCacheOption.OnLoad
                );
            }

            MediaObjectInfo mediaInfo = playnaclDataSource.GetMediaObjectInfo();
            lbl1.Content = mediaInfo.Title;
            lbl2.Content = mediaInfo.Artist;
            lbl3.Content = mediaInfo.AlbumTitle;

            await GetPic(playnaclDataSource);
        }

        private async Task GetFiveDaysWeatherForecast(string cityName)
        {
            await GetLatitudeAndLongitude(cityName);

            SetCitySettings(cityName);

            if (_lat == null || _lon == null)
            {
                return;
            }
            await GetWheather(_lat, _lon);

            Grid weatherGrid = (Grid)FindName("weatherGrid");
            var ChildrenOfGrid = weatherGrid.Children.OfType<FrameworkElement>().ToList();
            foreach (FrameworkElement child in ChildrenOfGrid)
            {
                if (child.Name == "searchBarListBox")
                {
                    weatherGrid.Children.Remove(child);
                }
            }
        }

        /*    private string GetImageDir(string imgName,bool isIcon)
            {
    
                string lastPath;
                var path2 = Directory.GetParent(path1);
                if (path2 != null)
                {
                    DirectoryInfo? path3 = Directory.GetParent(path2.ToString());
                    if (path3 != null)
                    {
                        DirectoryInfo? path4 = Directory.GetParent(path3.ToString());
                        if (path4 != null)
                        {
                            if(isIcon == false)
                            {
                                lastPath = path4.ToString() + "\\Resources\\Icons\\" + imgName;
                            }
                            else
                            {
                                lastPath = path4.ToString() + "\\Resources\\Icons\\weather_icons\\" + imgName;
                            }
                           
                        }
                        else
                        {
                            return "noImage";
                        }
                    }
                    else
                    {
                        return "noImage";
                    }
                }
                else
                {
                    return "noImage";
                }
    
                return lastPath;
            }
        */
        private async Task GetLatitudeAndLongitude(string nameOfTheCity)
        {
            _lat = null;
            _lon = null;
            HttpClient client = new HttpClient();
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string connectionStr =
                    "https://geocoding-api.open-meteo.com/v1/search?name="
                    + nameOfTheCity
                    + "& count=10&language=en&format=json";
                HttpResponseMessage response = await client.GetAsync(connectionStr);
                var httpResponse = response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                List<JToken> results;
                // Convert JSON string to JObject
                JObject jsonObject = JObject.Parse(responseBody);
                if (jsonObject.First.Path == "generationtime_ms")
                {
                    return;
                }
                try
                {
                    results = (
                        jsonObject["results"] ?? throw new InvalidOperationException()
                    ).ToList();
                }
                catch (System.ArgumentNullException e)
                {
                    Console.WriteLine("Message :{0} ", e.Message);
                    CityLabel.Content = "cityNotFound";
                    return;
                }

                foreach (JToken obj in results)
                {
                    string fcstr = obj["feature_code"]!.ToString();

                    if (fcstr.StartsWith("P") == true)
                    {
                        _lat = obj["latitude"]?.ToString();
                        _lon = obj["longitude"]?.ToString();
                        CityLabel.Content = obj["name"]?.ToString();

                        break;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        private async Task GetPic(MediaPlaybackDataSource playnaclDataSource)
        {
            MediaPlaybackInfo z = playnaclDataSource.GetMediaPlaybackInfo();
            string lastPath;
            if (z.PlaybackState.ToString() == "Playing")
            {
                lastPath = "stop.png";
                img.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/" + lastPath,
                        UriKind.Absolute
                    )
                );
                playStop.Content = img;
            }
            else
            {
                lastPath = "play.png";
                img.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/" + lastPath,
                        UriKind.Absolute
                    )
                );
                playStop.Content = img;
            }
        }

        private void GetSettings()
        {
            defCol = Properties.Settings.Default.defaultColor;
            System.Windows.Media.Color mediaColor = new System.Windows.Media.Color();
            mediaColor.A = defCol.A;
            mediaColor.R = defCol.R;
            mediaColor.G = defCol.G;
            mediaColor.B = defCol.B;
            SolidColorBrush newCol = new(mediaColor);
            ChangeTheme(newCol);
            colorPiclerOpened = Properties.Settings.Default.colorPiclerOpenedSet;
            searchBarTxt.Text = Properties.Settings.Default.defaultCityName;
            AutoStopMusicSet = Properties.Settings.Default.musicLock;
            AutoStopMusic.IsChecked = AutoStopMusicSet;
            AutoStopMusicChange(AutoStopMusicSet);
        }

        private Task GetTimeLinePosition(
            MediaPlaybackDataSource playnaclDataSource,
            bool getTimeLine
        )
        {
            MediaTimelineProperties timeline = playnaclDataSource.GetMediaTimelineProperties();

            DateTime lastUpdateofTimeLineDate = timeline.PositionSetFileTime;
            DateTime currentTime = DateTime.Now;
            long x = lastUpdateofTimeLineDate.Ticks;
            long y = currentTime.Ticks;
            long diff = y - x;
            //double difference = TimeSpan.FromTicks(diff).TotalSeconds;

            MediaPlaybackInfo z = playnaclDataSource.GetMediaPlaybackInfo();

            if (z.PlaybackState.ToString() != "Playing")
            {
                diff = 0;
            }

            long totalSec = timeline.Position.Ticks + diff;
            double currentSongPositionSec = TimeSpan.FromTicks(totalSec).TotalSeconds;

            if (timelineSlider.IsMouseOver == false && getTimeLine == true)
            {
                timelineSlider.Maximum = Convert.ToDouble(timeline.EndTime.TotalSeconds);
                timelineSlider.Value = Convert.ToDouble(currentSongPositionSec);
            }

            int minOfTheSong = Convert.ToInt16(Math.Floor(currentSongPositionSec / 60.0));
            int secOfTheSong = Convert.ToInt16(Math.Floor(currentSongPositionSec % 60.0));

            /*if (secOfTheSong == 0 || secOfTheSong == 60) {
                secOfTheSong = 0;
            }*/

            if (secOfTheSong.ToString().Length == 1)
            {
                lblSecOfTheSong = minOfTheSong.ToString() + ":" + "0" + secOfTheSong.ToString();
            }
            else
            {
                lblSecOfTheSong = minOfTheSong.ToString() + ":" + secOfTheSong.ToString();
            }

            currentTimeLbl.Content = lblSecOfTheSong;

            int maxminOfTheSong = Convert.ToInt16(Math.Floor(timeline.EndTime.TotalSeconds / 60));
            int maxsecOfTheSong = Convert.ToInt16(timeline.EndTime.TotalSeconds % 60);

            if (maxsecOfTheSong.ToString().Length == 1)
            {
                lblSecOfTheSong = "0" + maxsecOfTheSong.ToString();
            }
            else
            {
                lblSecOfTheSong = maxsecOfTheSong.ToString();
            }

            maxTimeLbl.Content = maxminOfTheSong.ToString() + ":" + lblSecOfTheSong;
            return Task.CompletedTask;
        }

        private async Task GetWheather(string lat, string lon)
        {
            HttpClient client = new HttpClient();
            string path;
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                lat = lat.Replace(",", ".");
                lon = lon.Replace(",", ".");
                string connectionStr =
                    "https://api.open-meteo.com/v1/forecast?latitude="
                    + lat
                    + "&longitude="
                    + lon
                    + "&current=temperature_2m,rain,weather_code&daily=weather_code,temperature_2m_max,temperature_2m_min&forecast_days=4";
                HttpResponseMessage response = await client.GetAsync(connectionStr);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Convert JSON string to JObject
                JObject jsonObject = JObject.Parse(responseBody);
                List<JToken> days = jsonObject["daily"]["time"].ToList();
                List<JToken> dailyMax = jsonObject["daily"]["temperature_2m_max"].ToList();
                List<JToken> dailyMin = jsonObject["daily"]["temperature_2m_min"].ToList();
                List<JToken> dailyWheatherCode = jsonObject["daily"]["weather_code"].ToList();
                JToken currentTemp = jsonObject["current"]["temperature_2m"];
                JToken currentWC = jsonObject["current"]["weather_code"];

                string firstday = days[0].ToString();

                /*CurrentWeatherLabel.Content = currentTemp.ToString() + " °C";*/
                CurrentWeatherLabel.Content = $"{currentTemp:#}" + " °C";
                string weatherCode = currentWC.ToString();
                JToken currentWeatherIconPath = weatherCodes[weatherCode];

                System.Windows.Controls.Image anlikDurumİmage = new();
                path =
                    "pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/"
                    + currentWeatherIconPath.ToString();
                anlikDurumİmage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                CurrentWeatherPic.Content = anlikDurumİmage;

                string currentGunWeatherExp = currentWC.ToString();
                currentweatherCodeExpl.Content = weatherExpCodes[currentGunWeatherExp];

                birinciGunTarih.Content = days[0].ToString();
                ikinciGunTarih.Content = days[1].ToString();
                ucuncuGunTarih.Content = days[2].ToString();
                dorduncuGunTarih.Content = days[3].ToString();

                birinciGunMin.Content = $"{dailyMin[0]:#}" + " °C";
                birinciGunMax.Content = $"{dailyMax[0]:#}" + " °C";
                birinciGunDurum.Content = dailyWheatherCode[0].ToString();

                string birinciGunWeatherIconCode = dailyWheatherCode[0].ToString();
                JToken birinciGunWeatherIconPath = weatherCodes[birinciGunWeatherIconCode];
                path = birinciGunWeatherIconPath.ToString();
                System.Windows.Controls.Image birinciGunImage = new();
                birinciGunImage.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/"
                            + path,
                        UriKind.Absolute
                    )
                );
                birinciGunDurum.Content = birinciGunImage;

                System.Windows.Controls.Image ikinciGunImage = new();
                ikinciGunDurum.Content = dailyWheatherCode[1].ToString();
                string ikinciGunWeatherIconCode = dailyWheatherCode[1].ToString();
                JToken ikinciGunWeatherIconPath = weatherCodes[ikinciGunWeatherIconCode];
                path = ikinciGunWeatherIconPath.ToString();
                ikinciGunImage.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/"
                            + path,
                        UriKind.Absolute
                    )
                );
                ikinciGunMin.Content = $"{dailyMin[1]:#}" + " °C";
                ikinciGunMax.Content = $"{dailyMax[1]:#}" + " °C";
                ikinciGunDurum.Content = ikinciGunImage;

                ucuncuGunDurum.Content = dailyWheatherCode[2].ToString();
                string ucuncuGunWeatherIconCode = dailyWheatherCode[2].ToString();
                JToken ucuncuGunWeatherIconPath = weatherCodes[ucuncuGunWeatherIconCode];
                System.Windows.Controls.Image ucuncuGunImage = new();
                path = ucuncuGunWeatherIconPath.ToString();
                ucuncuGunImage.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/"
                            + path,
                        UriKind.Absolute
                    )
                );
                ucuncuGunMin.Content = $"{dailyMin[2]:#}" + " °C";
                ucuncuGunMax.Content = $"{dailyMax[2]:#}" + " °C";
                ucuncuGunDurum.Content = ucuncuGunImage;

                dorduncuGunDurum.Content = dailyWheatherCode[3].ToString();
                string dorduncuGunWeatherIconCode = dailyWheatherCode[3].ToString();
                JToken dorduncuGunWeatherIconPath = weatherCodes[dorduncuGunWeatherIconCode];
                System.Windows.Controls.Image dorduncuGunImage = new();
                path = dorduncuGunWeatherIconPath.ToString();
                dorduncuGunImage.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/"
                            + path,
                        UriKind.Absolute
                    )
                );
                dorduncuGunMin.Content = $"{dailyMin[3]:#}" + " °C";
                dorduncuGunMax.Content = $"{dailyMax[3]:#}" + " °C";
                dorduncuGunDurum.Content = dorduncuGunImage;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return;
            }
        }

        private void muteButton_Click(object sender, RoutedEventArgs e)
        {
            if (isMuted == 0)
            {
                SystemAudio.WindowsSystemAudio.SetVolume(0);
                prewslid.Value = 0;
                isMuted = 1;
            }
            else
            {
                SystemAudio.WindowsSystemAudio.SetVolume(currVol);
                prewslid.Value = currVol;
                isMuted = 0;
            }
        }

        private void oncekiSarki_Click(object sender, RoutedEventArgs e)
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();

            NowPlayingSession[] sessions = player.GetSessions();
            var sessionInfos = sessions
                .Where(x =>
                    x.SourceAppId == "Spotify.exe"
                    || x.SourceAppId.Contains("spotify")
                    || x.SourceAppId.Contains("Spotify")
                )
                .Select(x => x.GetSessionInfo())
                .ToList();
            if (sessionInfos.Count == 0)
            {
                return;
            }
            else
            {
                player.SetCurrentSession(sessionInfos[0]);
            }
            NowPlayingSession currentSession = player.CurrentSession;
            MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();
            x.SendMediaPlaybackCommand(MediaPlaybackCommands.Previous);
            GetCurrentMedia(false, true);
        }

        private void PlayStopMedia()
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();

            NowPlayingSession[] sessions = player.GetSessions();
            var sessionInfos = sessions
                .Where(x =>
                    x.SourceAppId == "Spotify.exe"
                    || x.SourceAppId.Contains("spotify")
                    || x.SourceAppId.Contains("Spotify")
                )
                .Select(x => x.GetSessionInfo())
                .ToList();
            if (sessionInfos.Count == 0)
            {
                return;
            }
            else
            {
                player.SetCurrentSession(sessionInfos[0]);
            }
            NowPlayingSession currentSession = player.CurrentSession;

            var x = currentSession.ActivateMediaPlaybackDataSource();
            var z = x.GetMediaPlaybackInfo();

            if (z.PlaybackState.ToString() == "Playing")
            {
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Stop);
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Pause);
                // string lastpath = GetImageDir("stop.png", false);
                img.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/stop.png",
                        UriKind.Absolute
                    )
                );
                playStop.Content = img;
            }
            else
            {
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Play);

                //string lastpath = GetImageDir("play.png", false);
                img.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/play.png",
                        UriKind.Absolute
                    )
                );
                playStop.Content = img;
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void resButton_Click(object sender, RoutedEventArgs e)
        {
            ctHour = Convert.ToInt16(hourCmbx.SelectedValue);
            ctMin = Convert.ToInt16(minCmbx.SelectedValue);
            if (ctHour != 0 || ctMin != 0)
            {
                Process.Start("shutdown", "/r /t " + ((ctHour * 3600) + ctMin * 60).ToString());
                restartProcessStarted = 1;
                hTxtBox.Text = ctHour.ToString();
                mTxtBox.Text = ctMin.ToString();
                sTxtBox.Text = "00";
                CountDowntimer(1);
            }
        }

        private async void SearchBar_SearchStarted(
            object sender,
            HandyControl.Data.FunctionEventArgs<string> e
        )
        {
            await GetFiveDaysWeatherForecast(searchBarTxt.Text);
        }

        private void searchBarTxt_KeyDown(object sender, KeyEventArgs e)
        {
            searchCoordinates(searchBarTxt.Text);
        }

        private async void searchCoordinates(string nameOfTheCity)
        {
            HttpClient client = new HttpClient();
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string connectionStr =
                    "https://geocoding-api.open-meteo.com/v1/search?name="
                    + nameOfTheCity
                    + "& count=10&language=en&format=json";
                HttpResponseMessage response = await client.GetAsync(connectionStr);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                List<JToken> results;
                // Convert JSON string to JObject
                JObject jsonObject = JObject.Parse(responseBody);
                try
                {
                    results = jsonObject["results"].ToList();
                }
                catch (System.ArgumentNullException e)
                {
                    Console.WriteLine("Message :{0} ", e.Message);
                    CityLabel.Content = "cityNotFound";
                    return;
                }

                cordList.Clear();
                cityNames.Clear();
                ListBox searchBarListBox = new ListBox();
                searchBarListBox.Margin = new Thickness(65, 0, 0, 0);
                searchBarListBox.Name = "searchBarListBox";
                searchBarListBox.MouseDoubleClick += new MouseButtonEventHandler(srcBoxSelected);

                async void srcBoxSelected(object sender, MouseButtonEventArgs e)
                {
                    int selectedIndex = searchBarListBox.SelectedIndex;
                    if (selectedIndex < 0)
                    {
                        return;
                    }
                    string x = cordList[selectedIndex][0];
                    string y = cordList[selectedIndex][1];
                    CityLabel.Content = cityNames[selectedIndex];
                    SetCitySettings(cityNames[selectedIndex]);
                    GetWheather(x, y);

                    Grid weatherGrid = (Grid)FindName("weatherGrid");
                    var ChildrenOfGrid = weatherGrid.Children.OfType<FrameworkElement>().ToList();
                    foreach (FrameworkElement child in ChildrenOfGrid)
                    {
                        if (child.Name == "searchBarListBox")
                        {
                            weatherGrid.Children.Remove(child);
                        }
                    }
                }

                foreach (JToken obj in results)
                {
                    var ccExist = obj["country_code"];

                    if (ccExist == null)
                    {
                        searchBarListBox.Items.Add(
                            obj["name"].ToString()
                                + " "
                                + obj["latitude"].ToString()
                                + ","
                                + obj["longitude"].ToString()
                        );
                    }
                    else
                    {
                        searchBarListBox.Items.Add(
                            obj["country_code"].ToString()
                                + ","
                                + obj["name"].ToString()
                                + " "
                                + obj["latitude"].ToString()
                                + ","
                                + obj["longitude"].ToString()
                        );
                    }
                    cordList.Add([obj["latitude"].ToString(), obj["longitude"].ToString()]);
                    cityNames.Add(obj["name"].ToString());
                }

                Grid.SetRow(searchBarListBox, 1);
                Grid.SetColumn(searchBarListBox, 0);
                Grid.SetColumnSpan(searchBarListBox, 3);
                Grid.SetRowSpan(searchBarListBox, 4);

                weatherGrid.Children.Add(searchBarListBox);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        private void SetCitySettings(String newCity)
        {
            Properties.Settings.Default.defaultCityName = newCity;
            Properties.Settings.Default.Save();
        }

        private void SetColorSettings(SolidColorBrush newcolor)
        {
            System.Drawing.Color newColorDrawing = System.Drawing.Color.FromArgb(
                0Xff,
                (byte)newcolor.Color.R,
                (byte)newcolor.Color.G,
                (byte)newcolor.Color.B
            );
            Properties.Settings.Default.defaultColor = newColorDrawing;
            Properties.Settings.Default.Save();
        }

        private void SetMinMax(bool isMaxed)
        {
            Properties.Settings.Default.colorPiclerOpenedSet = isMaxed;
            Properties.Settings.Default.Save();
        }

        private void Slider_Initialized(object sender, EventArgs e)
        {
            int currentVolume = SystemAudio.WindowsSystemAudio.GetVolume();
            prewslid.Value = currentVolume;
            currVol = currentVolume;

            if (currentVolume == 0)
            {
                isMuted = 1;
            }
            else
            {
                isMuted = 0;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int changedVolume = Convert.ToInt16(prewslid.Value);
            if (changedVolume != 0)
            {
                currVol = changedVolume;
            }
            SystemAudio.WindowsSystemAudio.SetVolume(changedVolume);
        }

        private void sonrakiSarki_Click(object sender, RoutedEventArgs e)
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession[] sessions = player.GetSessions();
            var sessionInfos = sessions
                .Where(x =>
                    x.SourceAppId == "Spotify.exe"
                    || x.SourceAppId.Contains("spotify")
                    || x.SourceAppId.Contains("Spotify")
                )
                .Select(x => x.GetSessionInfo())
                .ToList();
            if (sessionInfos.Count == 0)
            {
                return;
            }
            else
            {
                player.SetCurrentSession(sessionInfos[0]);
            }

            NowPlayingSession currentSession = player.CurrentSession;
            /*if (currentSession == null)
            {
                return;
            }
            else
            {
                if (currentSession.SourceAppId != "Spotify.exe")
                {
                    return;
                }
            }*/

            MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();
            x.SendMediaPlaybackCommand(MediaPlaybackCommands.Next);

            GetCurrentMedia(false, true);
        }

        private void themeColorPicker_Canceled(object sender, EventArgs e)
        {
            themeColorPicker.Visibility = Visibility.Hidden;
            border.Background = themeColor;

            toprect.Fill = themeColor;
        }

        private void themeColorPicker_Confirmed(
            object sender,
            HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e
        )
        {
            themeColorPicker.Visibility = Visibility.Hidden;
            var color = themeColorPicker.SelectedBrush;
            ChangeTheme(color);
        }

        private void themeColorPicker_SelectedColorChanged(
            object sender,
            HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e
        )
        {
            var color = themeColorPicker.SelectedBrush;
            ChangeTheme(color);
        }

        private void timelineSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;
            if (currentSession == null)
            {
                return;
            }
            else
            {
                if (
                    currentSession.SourceAppId != "Spotify.exe"
                    && currentSession.SourceAppId.Contains("spotify") == false
                    && currentSession.SourceAppId.Contains("Spotify") == false
                )
                {
                    return;
                }
            }
            MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();

            x.SendPlaybackPositionChangeRequest(TimeSpan.FromSeconds(timelineSlider.Value));
        }

        private void TimerTickAsync(object sender, EventArgs e)
        {
            timeLbl.Content = DateTime.Now.ToString("HH:mm:ss");
            dateLbl.Content = DateTime.Now.ToString("yyy-MM-dd") + " / " + DateTime.Now.DayOfWeek;

            int currentVolume = SystemAudio.WindowsSystemAudio.GetVolume();

            prewslid.Value = currentVolume;
            currentSoundLevel.Content = currentVolume;
            System.Windows.Controls.Image img2 = new();
            string lastpath;
            if (currentVolume > 50)
            {
                lastpath = "volume-high-solid.png";
            }
            else if (currentVolume <= 50 && currentVolume > 0)
            {
                lastpath = "volume-low-solid.png";
            }
            else
            {
                lastpath = "volume-xmark-solid.png";
            }

            img2.Source = new BitmapImage(
                new Uri(
                    "pack://application:,,,/MyWidget;component/Resources/Icons/" + lastpath,
                    UriKind.Absolute
                )
            );
            img2.Stretch = Stretch.Uniform;
            muteButton.Content = img2;

            GetCurrentMedia(true, true);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo link = new("https://open-meteo.com/") { UseShellExecute = true };
            Process.Start(link);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            ConfigHelper.Instance.SetLang("en");

            GetSettings();
            ChangeWindowSize(colorPiclerOpened);
            System.Windows.Controls.Image xxx = new();

            xxx.Source = new BitmapImage(
                new Uri(
                    "pack://application:,,,/MyWidget;component/Resources/Icons/play.png",
                    UriKind.Absolute
                )
            );
            ImageSource imageSrc = xxx.Source;
            TrayIcon.Icon = imageSrc;

            GetCurrentMedia(true, true);

            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += TimerTickAsync;
            timer.Start();

            ctTimer.Interval = TimeSpan.FromSeconds(1);
            ctTimer.Tick += CtTimerTick;

            for (int i = 0; i <= 23; i++)
            {
                hourCmbx.Items.Add(i.ToString());
            }

            for (int i = 0; i <= 59; i++)
            {
                minCmbx.Items.Add(i.ToString());
            }
            hourCmbx.SelectedValue = "1";
            minCmbx.SelectedValue = "0";

            GetFiveDaysWeatherForecast(searchBarTxt.Text);
            LoadJson();
        }

        private void LoadJson()
        {
            Uri iconPathjsonUri = new Uri(
                "pack://application:,,,/MyWidget;component/Resources/weather_icon_match.json",
                UriKind.RelativeOrAbsolute
            );
            StreamResourceInfo resourceInfo = Application.GetResourceStream(iconPathjsonUri);
            string jsonContent;
            using (StreamReader reader = new StreamReader(resourceInfo.Stream))
            {
                jsonContent = reader.ReadToEnd();
            }

            JObject json = JObject.Parse(jsonContent);
            weatherCodes = json;

            Uri iconExpjsonUri = new Uri(
                "pack://application:,,,/MyWidget;component/Resources/weather_code_exp.json",
                UriKind.RelativeOrAbsolute
            );
            StreamResourceInfo expresourceInfo = Application.GetResourceStream(iconExpjsonUri);
            string expjsonContent;
            using (StreamReader reader = new StreamReader(expresourceInfo.Stream))
            {
                expjsonContent = reader.ReadToEnd();
            }

            JObject expjson = JObject.Parse(expjsonContent);
            weatherExpCodes = expjson;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (colorPiclerOpened == true)
            {
                ChangeWindowSize(false);
                colorPiclerOpened = false;
            }
            else
            {
                ChangeWindowSize(true);
                colorPiclerOpened = true;
            }
        }

        private void ChangeWindowSize(bool opened)
        {
            SetMinMax(opened);
            string iconPath;
            if (opened == true)
            {
                grid1.Visibility = Visibility.Hidden;
                weatherGrid.Visibility = Visibility.Hidden;
                paletteButton.Visibility = Visibility.Hidden;
                MscPlaterDivider.Visibility = Visibility.Hidden;
                this.Height = 260;
                Mscply.SetValue(Grid.RowProperty, 1);
                Mscply.SetValue(Grid.RowSpanProperty, 4);
                //MaxMinButton.SetValue(Grid.ColumnProperty, 6);
                // iconPath = GetImageDir("max.png", false);
                imageSource.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/max.png",
                        UriKind.Absolute
                    )
                );
                imageSource.Stretch = Stretch.Fill;
                MaxMinButton.Content = imageSource;
                opened = false;
            }
            else
            {
                grid1.Visibility = Visibility.Visible;
                weatherGrid.Visibility = Visibility.Visible;
                paletteButton.Visibility = Visibility.Visible;
                MscPlaterDivider.Visibility = Visibility.Visible;
                //MaxMinButton.SetValue(Grid.ColumnProperty, 5);
                this.Height = 780;
                Mscply.SetValue(Grid.RowProperty, 2);
                Mscply.SetValue(Grid.RowSpanProperty, 1);

                iconPath = "min.png";
                imageSource.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/MyWidget;component/Resources/Icons/" + iconPath,
                        UriKind.Absolute
                    )
                );
                imageSource.Stretch = Stretch.Fill;
                MaxMinButton.Content = imageSource;
                opened = true;
            }
        }

        public void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;
            if (player.Count == 0)
            {
                return;
            }
            var x = currentSession.ActivateMediaPlaybackDataSource();
            var z = x.GetMediaPlaybackInfo();

            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                if (z.PlaybackState.ToString() == "Playing")
                {
                    x.SendMediaPlaybackCommand(MediaPlaybackCommands.Stop);
                    x.SendMediaPlaybackCommand(MediaPlaybackCommands.Pause);
                }
            }
        }

        private void AutoStopMusic_Click(object sender, RoutedEventArgs e)
        {
            if (AutoStopMusic.IsChecked == true)
            {
                AutoStopMusicChange(true);
            }
            else
            {
                AutoStopMusicChange(false);
            }
        }

        private void AutoStopMusicChange(bool x)
        {
            if (x == true)
            {
                Properties.Settings.Default.musicLock = true;

                SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            }
            else
            {
                Properties.Settings.Default.musicLock = false;
                SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            }
            Properties.Settings.Default.Save();
        }

        private void lbl1_MouseClick(object sender, MouseButtonEventArgs e)
        {
            string seachQuery = lbl1.Content.ToString();
            var x = Properties.Settings.Default.defaultColor;
            Color mediaColor = new Color();
            mediaColor.A = x.A;
            mediaColor.R = x.R;
            mediaColor.G = x.G;
            mediaColor.B = x.B;

            SolidColorBrush newCol = new(mediaColor);

            // window1 = new Window1(seachQuery, newCol);
            //window1.Show();
            //window1.ShowDialog();

            if (window1 == null) // Eğer diyalog penceresi zaten açık değilse
            {
                window1 = new Window1(seachQuery, newCol, true);
                window1.Closed += (s, args) => window1 = null; // Pencere kapandığında referansı sıfırla
                window1.Show(); // Pencereyi modal olmadan aç
            }
            else
            {
                window1.Activate(); // Pencere zaten açıksa öne getir
            }
        }

        private void lbl1_MouseClick2(object sender, MouseButtonEventArgs e)
        {
            string seachQuery = lbl2.Content.ToString();
            var x = Properties.Settings.Default.defaultColor;
            Color mediaColor = new Color();
            mediaColor.A = x.A;
            mediaColor.R = x.R;
            mediaColor.G = x.G;
            mediaColor.B = x.B;

            SolidColorBrush newCol = new(mediaColor);

            // window1 = new Window1(seachQuery, newCol);
            //window1.Show();
            //window1.ShowDialog();

            if (window1 == null) // Eğer diyalog penceresi zaten açık değilse
            {
                window1 = new Window1(seachQuery, newCol, false);
                window1.Closed += (s, args) => window1 = null; // Pencere kapandığında referansı sıfırla
                window1.Show(); // Pencereyi modal olmadan aç
            }
            else
            {
                window1.Activate(); // Pencere zaten açıksa öne getir
            }
        }

        private void lbl1_MouseEnter(object sender, MouseEventArgs e)
        {
            lbl1.Foreground = Brushes.LightGray;
        }

        private void lbl1_MouseLeave(object sender, MouseEventArgs e)
        {
            lbl1.Foreground = Brushes.White;
        }

        private void lbl2_MouseEnter(object sender, MouseEventArgs e)
        {
            lbl2.Foreground = Brushes.LightGray;
        }

        private void lbl2_MouseLeave(object sender, MouseEventArgs e)
        {
            lbl2.Foreground = Brushes.White;
        }
    }
}
