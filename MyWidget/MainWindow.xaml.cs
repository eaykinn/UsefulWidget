using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using NPSMLib;
using Plugin.SimpleAudioPlayer;
using System.Drawing;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using HandyControl.Controls;
using System;
using System.ComponentModel;
using HandyControl.Tools.Extension;
using System.Media;
using System.Numerics;
using System.Linq;
using System.Net;
using System.Windows.Markup;
using Microsoft.Win32;
using Microsoft.VisualBasic;
using System.IO.Packaging;
using Microsoft.Windows.Themes;
using System.Windows.Media.Animation;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Runtime.Intrinsics.X86;
using System.Windows.Resources;
using System.Net.Http.Json;


namespace MyWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
        private readonly Image img = new();
        private readonly Image imageSource = new();
        private byte isMuted;
        private string lblSecOfTheSong;
        private readonly string path1 = Directory.GetCurrentDirectory();
        private byte restartProcessStarted = 0;
        private Brush themeColor;
        private readonly DispatcherTimer timer = new();
        bool colorPiclerOpened;
        JObject weatherCodes;
        private bool AutoStopMusicSet;
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
                hTxtBox.Text = ctHour.ToString();
                mTxtBox.Text = ctMin.ToString();
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
            }else
            {
                OpenCloseColorPicker(true);
                colorPiclerOpened = false;
            }      
        }

        private void OpenCloseColorPicker(bool isOpened)
        {
            if (isOpened == false) 
            {
                themeColor = this.Background;
                themeColorPicker.Visibility = Visibility.Visible;
                Grid.SetColumnSpan(themeColorPicker, 8);
                Grid.SetRowSpan(themeColorPicker, 15);
            }else
            {
                themeColorPicker.Visibility = Visibility.Hidden;
            }

           
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
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
            this.Background = x;
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
            sTxtBox.Text = ctSec.ToString();
            mTxtBox.Text = ctMin.ToString();
            hTxtBox.Text = ctHour.ToString();
        }

        private protected async Task GetCurrentMedia(bool onLoad, bool getTimeLine)
        {
            if (onLoad == false) { Thread.Sleep(50); }

            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;
            if (player.Count == 0) { return; }
            MediaPlaybackDataSource playnaclDataSource = currentSession.ActivateMediaPlaybackDataSource();

            await GetTimeLinePosition(playnaclDataSource, true);

            Stream streamInfo = playnaclDataSource.GetThumbnailStream();
            if (streamInfo != null)
            {
                songImage.Source = BitmapFrame.Create(streamInfo, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
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

            if (_lat == null || _lon == null) { return; }
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
            HttpClient client = new HttpClient();
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                string connectionStr = "https://geocoding-api.open-meteo.com/v1/search?name=" + nameOfTheCity + "& count=10&language=en&format=json";
                HttpResponseMessage response = await client.GetAsync(connectionStr);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                List<JToken> results;
                // Convert JSON string to JObject
                JObject jsonObject = JObject.Parse(responseBody);
                try
                {
                    results = (jsonObject["results"] ?? throw new InvalidOperationException()).ToList();
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
                img.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/" + lastPath, UriKind.Absolute));
                playStop.Content = img;
            }
            else
            {
                lastPath = "play.png";
                img.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/" + lastPath, UriKind.Absolute));
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

        private Task GetTimeLinePosition(MediaPlaybackDataSource playnaclDataSource, bool getTimeLine)
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
                string connectionStr = "https://api.open-meteo.com/v1/forecast?latitude=" + lat + "&longitude=" + lon + "&current=temperature_2m,rain,weather_code&daily=weather_code,temperature_2m_max,temperature_2m_min&forecast_days=4";
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

                string firstday = days[0].ToString();

                /*CurrentWeatherLabel.Content = currentTemp.ToString() + " °C";*/
                CurrentWeatherLabel.Content = $"{currentTemp:#}" + " °C";
                string weatherCode = dailyWheatherCode[0].ToString();
                JToken currentWeatherIconPath = weatherCodes[weatherCode];
                

                Image anlikDurumİmage = new Image();
                path = "pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/" + currentWeatherIconPath.ToString();
                anlikDurumİmage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                CurrentWeatherPic.Content = anlikDurumİmage;
                
               
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
                Image birinciGunImage = new Image();
                birinciGunImage.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/" + path, UriKind.Absolute));
                birinciGunDurum.Content = birinciGunImage;

                Image ikinciGunImage = new Image();
                ikinciGunDurum.Content = dailyWheatherCode[1].ToString();
                string ikinciGunWeatherIconCode = dailyWheatherCode[1].ToString();
                JToken ikinciGunWeatherIconPath = weatherCodes[ikinciGunWeatherIconCode];
                path = ikinciGunWeatherIconPath.ToString();
                ikinciGunImage.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/" + path, UriKind.Absolute));
                ikinciGunMin.Content = $"{dailyMin[1]:#}" + " °C";
                ikinciGunMax.Content = $"{dailyMax[1]:#}" + " °C";
                ikinciGunDurum.Content = ikinciGunImage;

                ucuncuGunDurum.Content = dailyWheatherCode[2].ToString();
                string ucuncuGunWeatherIconCode = dailyWheatherCode[2].ToString();
                JToken ucuncuGunWeatherIconPath = weatherCodes[ucuncuGunWeatherIconCode];
                Image ucuncuGunImage = new Image();
                path = ucuncuGunWeatherIconPath.ToString();
                ucuncuGunImage.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/" + path, UriKind.Absolute));
                ucuncuGunMin.Content = $"{dailyMin[2]:#}" + " °C";
                ucuncuGunMax.Content = $"{dailyMax[2]:#}" + " °C";
                ucuncuGunDurum.Content = ucuncuGunImage;

                dorduncuGunDurum.Content = dailyWheatherCode[3].ToString();
                string dorduncuGunWeatherIconCode = dailyWheatherCode[3].ToString();
                JToken dorduncuGunWeatherIconPath = weatherCodes[dorduncuGunWeatherIconCode];
                Image dorduncuGunImage = new Image();
                path = dorduncuGunWeatherIconPath.ToString();
                dorduncuGunImage.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/weather_icons/" + path, UriKind.Absolute));
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
            if (player.Count == 0) { return; }
            NowPlayingSession currentSession = player.CurrentSession;
            MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();
            x.SendMediaPlaybackCommand(MediaPlaybackCommands.Previous);
            GetCurrentMedia(false, true);
        }

        private void PlayStopMedia()
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;

            if (player.Count == 0) { return; }
            var x = currentSession.ActivateMediaPlaybackDataSource();
            var z = x.GetMediaPlaybackInfo();

            if (z.PlaybackState.ToString() == "Playing")
            {
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Stop);
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Pause);
               // string lastpath = GetImageDir("stop.png", false);
                img.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/stop.png", UriKind.Absolute));
                playStop.Content = img;
            }
            else
            {
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Play);

                //string lastpath = GetImageDir("play.png", false);
                img.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/play.png", UriKind.Absolute));
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

        private async void SearchBar_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
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
                string connectionStr = "https://geocoding-api.open-meteo.com/v1/search?name=" + nameOfTheCity + "& count=10&language=en&format=json";
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
                
                searchBarListBox.Name = "searchBarListBox";
                searchBarListBox.MouseDoubleClick += new MouseButtonEventHandler(srcBoxSelected);

                async void srcBoxSelected(object sender, MouseButtonEventArgs e)
                {
                    int selectedIndex = searchBarListBox.SelectedIndex;
                    if (selectedIndex < 0 ) { return;}
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
                        searchBarListBox.Items.Add(obj["name"].ToString() + " " + obj["latitude"].ToString() + "," + obj["longitude"].ToString());
                    }
                    else
                    {
                        searchBarListBox.Items.Add(obj["country_code"].ToString() + "," + obj["name"].ToString() + " " + obj["latitude"].ToString() + "," + obj["longitude"].ToString());
                    }
                    cordList.Add([obj["latitude"].ToString(), obj["longitude"].ToString()]);
                    cityNames.Add(obj["name"].ToString());
                }

                Grid.SetRow(searchBarListBox, 1);
                Grid.SetColumn(searchBarListBox, 1);
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
            System.Drawing.Color newColorDrawing = System.Drawing.Color.FromArgb(0Xff, (byte)newcolor.Color.R, (byte)newcolor.Color.G, (byte)newcolor.Color.B);
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
            NowPlayingSession currentSession = player.CurrentSession;
            if (player.Count == 0) { return; }
            MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();
            x.SendMediaPlaybackCommand(MediaPlaybackCommands.Next);
            
            GetCurrentMedia(false, true);
        }

        private void themeColorPicker_Canceled(object sender, EventArgs e)
        {
            themeColorPicker.Visibility = Visibility.Hidden;
            this.Background = themeColor;

            toprect.Fill = themeColor;
        }

        private void themeColorPicker_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e)
        {
            themeColorPicker.Visibility = Visibility.Hidden;
            var color = themeColorPicker.SelectedBrush;
            ChangeTheme(color);
        }

        private void themeColorPicker_SelectedColorChanged(object sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e)
        {
            var color = themeColorPicker.SelectedBrush;

            ChangeTheme(color);
        }

        private void timelineSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;
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
            Image img2 = new Image();
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

            img2.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/" + lastpath, UriKind.Absolute));
            img2.Stretch = Stretch.Uniform;
            muteButton.Content = img2;

            GetCurrentMedia(true, true);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo link = new("https://open-meteo.com/")
            {
                UseShellExecute = true
            };
            Process.Start(link);
           
        }


        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                NowPlayingSessionManager player = new NowPlayingSessionManager();
                NowPlayingSession currentSession = player.CurrentSession;
                if (player.Count == 0) { return; }
                MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Next);
                GetCurrentMedia(false, true);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetSettings();
            ChangeWindowSize(colorPiclerOpened);
            Image xxx = new();
            
            xxx.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/play.png", UriKind.Absolute));
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
            // string xxxx = Directory.GetCurrentDirectory();
            //var path2 = Directory.GetParent(path1);
            // if (path2 != null)
            //{
            //    DirectoryInfo? path3 = Directory.GetParent(path2.ToString());
            //     if (path3 != null)
            //     {
            //         DirectoryInfo? path4 = Directory.GetParent(path3.ToString());
            //          if (path4 != null)
            //          {
            Uri jsonUri = new Uri("pack://application:,,,/MyWidget;component/Resources/weather_icon_match.json", UriKind.RelativeOrAbsolute);
            StreamResourceInfo resourceInfo = Application.GetResourceStream(jsonUri);
            string jsonContent;
            using (StreamReader reader = new StreamReader(resourceInfo.Stream))
            {
                jsonContent = reader.ReadToEnd();
            }

            JObject json = JObject.Parse(jsonContent);
            weatherCodes = json;
     
      





            //}
            //}

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
                imageSource.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/max.png", UriKind.Absolute));
                imageSource.Stretch = Stretch.Fill;
                MaxMinButton.Content = imageSource;
                opened = false;
            }
            else
            { 
                grid1.Visibility = Visibility.Visible;
                weatherGrid.Visibility = Visibility.Visible;
                paletteButton.Visibility = Visibility.Visible;
                MscPlaterDivider.Visibility= Visibility.Visible;
                //MaxMinButton.SetValue(Grid.ColumnProperty, 5);
                this.Height = 780;
                Mscply.SetValue(Grid.RowProperty, 2);
                Mscply.SetValue(Grid.RowSpanProperty, 1);

                iconPath = "min.png";
                imageSource.Source = new BitmapImage(new Uri("pack://application:,,,/MyWidget;component/Resources/Icons/" + iconPath, UriKind.Absolute));
                imageSource.Stretch = Stretch.Fill;
                MaxMinButton.Content = imageSource;
                opened = true;

               
            }
           

        }
        
        public void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;
            if (player.Count == 0) { return; }
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
    }
}