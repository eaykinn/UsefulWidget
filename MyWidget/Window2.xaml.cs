using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using static System.Net.Mime.MediaTypeNames;

namespace MyWidget
{
    /// <summary>
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2(SolidColorBrush color, string lyrics, string seachQuery, string artistName)
        {
            InitializeComponent();

            Color color2 = Colors.Black;

            Point startPoint = new Point();
            startPoint.X = 0.5;
            startPoint.Y = 0;

            Point endPoint = new Point();
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

            Lyrics.Text = lyrics;

            SearchTypeLabel.Text = "Lyrics of " + seachQuery + " by " + artistName;

            //Lyrics.Text = lyrics;
        }

        private void toprect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
