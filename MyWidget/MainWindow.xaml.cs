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
using HandyControl.Tools.Extension;
using System.Media;
using System.Numerics;
using System.Linq;
using System.Net;
using System.Windows.Markup;



namespace MyWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        int currVol;
        byte isMuted;
        byte restartProcessStarted = 0;
        int cthour = 0;
        int ctmin = 0;
        int ctsec = 0;
        string lblsecOfTheSong;
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer ctimer = new DispatcherTimer();
        Image img = new Image();
        static string lat;
        static string lon;
        string path1 = Directory.GetCurrentDirectory();
        List<List<string>> coordList = new List<List<string>>();
        List<string> cityNames = new List<string>();
        Brush x;
        public MainWindow()
        {
            InitializeComponent();
            searchBarTxt.Text = "basaksehir";
            GetFiveDaysWeatherForecast("basaksehir");
          
        }

 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            GetCurrentMedia(true,true);
            
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += TimerTickAsync;
            timer.Start();

            
            ctimer.Interval = TimeSpan.FromSeconds(1);
            ctimer.Tick += ctimer_Tick;

            for (int i=0; i <= 23; i++)
            {
                hourCmbx.Items.Add(i.ToString());
            }

            for (int i = 0; i <= 59; i++)
            {
                minCmbx.Items.Add(i.ToString());
            }
            hourCmbx.SelectedValue = "1";
            minCmbx.SelectedValue = "0";
        }
        void TimerTickAsync(object sender, EventArgs e)
        {
            timeLbl.Content = DateTime.Now.ToString("HH:mm:ss");
            dateLbl.Content = DateTime.Now.ToString("yyy-MM-dd");
            int currentVolume = SystemAudio.WindowsSystemAudio.GetVolume();

            prewslid.Value = currentVolume;
            currentSoundLevel.Content = currentVolume;
            Image img2 = new Image();
            string lastpath;
            if (currentVolume > 50)
            {
                lastpath = GetImageDir("volume-high-solid.png");
            }
            else if (currentVolume <= 50 && currentVolume > 0)
            {
                lastpath = GetImageDir("volume-low-solid.png");
            }
            else
            {
                lastpath = GetImageDir("volume-xmark-solid.png");
            }

            img2.Source = new BitmapImage(new Uri(lastpath));
            muteButton.Content = img2;


            GetCurrentMedia(true,true);
            
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

        private void muteButton_Click(object sender, RoutedEventArgs e)
        {   if(isMuted == 0) {
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //timepicker.GettTime();
            cthour = Convert.ToInt16(hourCmbx.SelectedValue);
            ctmin = Convert.ToInt16(minCmbx.SelectedValue);
            if (cthour != 0 || ctmin != 0)
            {
                Process.Start("shutdown", "/s /t " + ((cthour * 3600) + ctmin * 60).ToString());
                restartProcessStarted = 1;
                hTxtBox.Text = cthour.ToString();
                mTxtBox.Text = ctmin.ToString();
                sTxtBox.Text = "00";
                CountDowntimer(1);
            }
        }

       /* private void timepicker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            timepicker.Height = 207;
            this.timepicker.SetValue(Grid.RowSpanProperty, 7);
        }*/

        private void resButton_Click(object sender, RoutedEventArgs e)
        {
           
            cthour = Convert.ToInt16(hourCmbx.SelectedValue);
            ctmin = Convert.ToInt16(minCmbx.SelectedValue);
            if (cthour != 0 || ctmin != 0)
            {
                Process.Start("shutdown", "/r /t " + ((cthour * 3600) + ctmin * 60).ToString());
                restartProcessStarted = 1;
                hTxtBox.Text = cthour.ToString();
                mTxtBox.Text = ctmin.ToString();
                sTxtBox.Text = "00";
                CountDowntimer(1);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("shutdown", "/a");
            hTxtBox.Text = "00";
            mTxtBox.Text = "00";
            sTxtBox.Text = "00";
            ctsec = 0;
            ctmin = 0;
            cthour = 0;
            restartProcessStarted = 0;
            CountDowntimer(0);
        }
        
        private void CountDowntimer(int start)
        {
            if (start == 1)
            {
                ctimer.Start();               
            }
            else
            {
                ctimer.Stop();         
            }

        }
        void ctimer_Tick(object sender, EventArgs e)
        {

            if (ctsec > 0)
            {
                ctsec = ctsec - 1; 
            }
            else
            {
                ctsec = 59;

                if (ctmin > 0)
                {
                    ctmin = ctmin - 1;
                }
                else
                {
                    if (cthour > 0)
                    {
                        cthour = cthour - 1;
                        ctmin = 59;
                    }
                }
            }
            sTxtBox.Text = ctsec.ToString();
            mTxtBox.Text = ctmin.ToString();
            hTxtBox.Text = cthour.ToString();
            
        }
        async Task GetTimeLinePosition(MediaPlaybackDataSource playnaclDataSource,bool getTimeLine)
        {
           
           MediaTimelineProperties timeline = playnaclDataSource.GetMediaTimelineProperties();
                  
           DateTime lastUpdateofTimeLineDate = timeline.PositionSetFileTime;
           DateTime currentTime = DateTime.Now;
            long x = lastUpdateofTimeLineDate.Ticks;
            long y = currentTime.Ticks;
            long diff = y - x;
            //double difference = TimeSpan.FromTicks(diff).TotalSeconds;

            MediaPlaybackInfo z = playnaclDataSource.GetMediaPlaybackInfo();

           
            if (z.PlaybackState.ToString() != "Playing") {
               
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
                lblsecOfTheSong = minOfTheSong.ToString() + ":" + "0" + secOfTheSong.ToString();
            }
            else
            {
                lblsecOfTheSong = minOfTheSong.ToString() + ":" + secOfTheSong.ToString();
            }

            currentTimeLbl.Content = lblsecOfTheSong;

            int maxminOfTheSong = Convert.ToInt16(Math.Floor(timeline.EndTime.TotalSeconds / 60));
            int maxsecOfTheSong = Convert.ToInt16(timeline.EndTime.TotalSeconds % 60);

            if (maxsecOfTheSong.ToString().Length == 1)
            {
                lblsecOfTheSong = "0" + maxsecOfTheSong.ToString();
            }
            else
            {
                lblsecOfTheSong = maxsecOfTheSong.ToString();
            }

            maxTimeLbl.Content = maxminOfTheSong.ToString() + ":" + lblsecOfTheSong;

        }
        private async Task GetCurrentMedia(bool onLoad, bool getTimeLine)
        {
            if (onLoad == false) {Thread.Sleep(50);}
            
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;
            if(player.Count == 0) { return; }
            MediaPlaybackDataSource playnaclDataSource = currentSession.ActivateMediaPlaybackDataSource();

            await GetTimeLinePosition(playnaclDataSource,true);

           



            Stream streamInfo = playnaclDataSource.GetThumbnailStream(); 
            if (streamInfo != null) { 
                songImage.Source = BitmapFrame.Create(streamInfo, BitmapCreateOptions.None, BitmapCacheOption.OnLoad); 
            }
            
            MediaObjectInfo mediaInfo = playnaclDataSource.GetMediaObjectInfo();
            lbl1.Content = mediaInfo.Title;
            lbl2.Content = mediaInfo.Artist;
            lbl3.Content = mediaInfo.AlbumTitle;
            
            await GetPic(playnaclDataSource);


        }

        async Task GetPic(MediaPlaybackDataSource playnaclDataSource)
        {
            MediaPlaybackInfo z = playnaclDataSource.GetMediaPlaybackInfo();

            if (z.PlaybackState.ToString() == "Playing")
            {
                string lastpath = GetImageDir("stop.png");

                img.Source = new BitmapImage(new Uri(lastpath));
                playStop.Content = img;
            }
            else
            {
                string lastpath = GetImageDir("play.png");
                img.Source = new BitmapImage(new Uri(lastpath));
                playStop.Content = img;
            }
        }

        private string GetImageDir(string imgName)
        {
            DirectoryInfo path2 = Directory.GetParent(path1);
            DirectoryInfo path3 = Directory.GetParent(path2.ToString());
            DirectoryInfo path4 = Directory.GetParent(path3.ToString());
            string lastpath = path4.ToString() + "\\" + imgName;
            return lastpath;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            
            PlayStopMedia();
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
                string lastpath = GetImageDir("stop.png");
                img.Source = new BitmapImage(new Uri(lastpath));
                playStop.Content = img;
            }
            else
            {
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Play);
                string lastpath = GetImageDir("play.png");
                img.Source = new BitmapImage(new Uri(lastpath));
                playStop.Content = img;
            }
               
        }

        private void oncekiSarki_Click(object sender, RoutedEventArgs e)
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            if (player.Count == 0) { return; }
            NowPlayingSession currentSession = player.CurrentSession;
            MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();
            x.SendMediaPlaybackCommand(MediaPlaybackCommands.Previous);
            GetCurrentMedia(false,true);
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

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
       
        private async Task GetFiveDaysWeatherForecast(string cityName)
        {
            await GetLatitudeAndLongitude(cityName);
            await GetWheather(lat, lon);

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

        async Task GetLatitudeAndLongitude(string nameOfTheCity)
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
                
                

                foreach (JToken obj in results)
                {
                    string fcstr = obj["feature_code"].ToString();

                    if (fcstr.StartsWith("P") == true)
                    {
                        lat = obj["latitude"].ToString();
                        lon = obj["longitude"].ToString();
                        CityLabel.Content = obj["name"].ToString();
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

        private async Task GetWheather(string lat, string lon)
        {
            HttpClient client = new HttpClient();
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                lat = lat.Replace(",",".");
                lon = lon.Replace(",", ".");
                string connectionStr = "https://api.open-meteo.com/v1/forecast?latitude=" + lat + "&longitude=" + lon + "&current=temperature_2m,rain,weather_code&daily=weather_code,temperature_2m_max,temperature_2m_min&forecast_days=3";
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


                CurrentWeatherLabel.Content = currentTemp.ToString();

                birinciGunTarih.Content = days[0].ToString();
                ikinciGunTarih.Content = days[1].ToString();
                ucuncuGunTarih.Content = days[2].ToString();

                birinciGunMin.Content = dailyMin[0].ToString();
                birinciGunMax.Content = dailyMax[0].ToString();
                birinciGunDurum.Content = dailyWheatherCode[0].ToString();
                    
                ikinciGunMin.Content = dailyMin[1].ToString();
                ikinciGunMax.Content = dailyMax[1].ToString();
                ikinciGunDurum.Content = dailyWheatherCode[1].ToString();

                ucuncuGunMin.Content = dailyMin[2].ToString();
                ucuncuGunMax.Content = dailyMax[2].ToString();
                ucuncuGunDurum.Content = dailyWheatherCode[2].ToString();

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return;
            }

        }

        private void timelineSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            NowPlayingSessionManager player = new NowPlayingSessionManager();
            NowPlayingSession currentSession = player.CurrentSession;
            MediaPlaybackDataSource x = currentSession.ActivateMediaPlaybackDataSource();

            x.SendPlaybackPositionChangeRequest(TimeSpan.FromSeconds(timelineSlider.Value));
        }

        private async void SearchBar_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
        {
           
            await GetFiveDaysWeatherForecast(searchBarTxt.Text);
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

        private void searchBarTxt_KeyDown(object sender, KeyEventArgs e)
        {
            searchCoordinates(searchBarTxt.Text);           
        }

        async void searchCoordinates(string nameOfTheCity)
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

                coordList.Clear();
                cityNames.Clear();
                ListBox searchBarListBox = new ListBox();
                searchBarListBox.Name = "searchBarListBox";
                searchBarListBox.MouseDoubleClick += new MouseButtonEventHandler(srcBoxSelected);

                async void srcBoxSelected(object sender, MouseButtonEventArgs e)
                {
                    int selectedIndex = searchBarListBox.SelectedIndex;
                    string x = coordList[selectedIndex][0];
                    string y = coordList[selectedIndex][1];
                    CityLabel.Content = cityNames[selectedIndex];
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
                    searchBarListBox.Items.Add(obj["country_code"].ToString() + "," + obj["name"].ToString() + " " + obj["latitude"].ToString() + "," + obj["longitude"].ToString());
                    coordList.Add([obj["latitude"].ToString(), obj["longitude"].ToString()]);
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

        private void changeTheme(SolidColorBrush color)
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
            gradientStop2.Color = color.Color;
            gradientStop2.Offset = 1;

            IEnumerable<GradientStop> coll = [gradientStop, gradientStop2];

            var z = new GradientStopCollection(coll);

            var x = new LinearGradientBrush(z, startPoint, endPoint);
            this.Background = x;
            toprect.Fill = x;

        }

       
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            x = this.Background;

            themeColorPicker.Visibility = Visibility.Visible;
            Grid.SetRow(themeColorPicker, 1);
            Grid.SetColumn(themeColorPicker, 3);
            Grid.SetColumnSpan(themeColorPicker, 8);
            Grid.SetRowSpan(themeColorPicker, 10);

        }

        private void themeColorPicker_SelectedColorChanged(object sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e)
        {
           
            var color = themeColorPicker.SelectedBrush;
            changeTheme(color);
        }

        private void themeColorPicker_Canceled(object sender, EventArgs e)
        {
            themeColorPicker.Visibility = Visibility.Hidden;
            this.Background = x;
            toprect.Fill = x;
        }

        private void themeColorPicker_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e)
        {
            themeColorPicker.Visibility = Visibility.Hidden;
            var color = themeColorPicker.SelectedBrush;
            changeTheme(color);
        }
    }
}