using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyWidget
{
    /// <summary>
    /// Interaction logic for TimePickerUC.xaml
    /// </summary>
    public partial class TimePickerUC : UserControl
    {
       private int hourint = 0;
       private int minint = 0;
       public int HOUR;
       public int MIN;
       public int canClose = 0;

       string hour = "00";
       string min ="00";
        public TimePickerUC()
        {
            InitializeComponent();

            for (int i = 0; i <= 23; i++)
            {
                hourList.Items.Add(i.ToString());
            }

            for (int i = 0; i <= 59; i++)
            {
                minList.Items.Add(i.ToString());
            }

            //hourList.Visibility = Visibility.Hidden;
            //minList.Visibility = Visibility.Hidden;
            //okbutton.Visibility = Visibility.Hidden;
            //cancelbutton.Visibility = Visibility.Hidden;
        }


        public void SetTime() {
            
            

            if(hourList.SelectedValue != null) {
                hourint = int.Parse(hourList.SelectedValue.ToString());
                if (hourList.SelectedValue.ToString().Length == 1)
                {
                    hour = "0" + hourList.SelectedValue.ToString();
                }
                else
                {
                    hour = hourList.SelectedValue.ToString();
                }
            }

            if (minList.SelectedValue != null) {
                minint = int.Parse(minList.SelectedValue.ToString());
                if (minList.SelectedValue.ToString().Length == 1)
                {
                    min = "0" + minList.SelectedValue.ToString();
                }
                else
                {
                    min = minList.SelectedValue.ToString();
                }
            }

            TimePickerReturn.Content = hour + ":" + min;
        }

        public void GettTime()
        {  
            HOUR = hourint;
            MIN = minint;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetTime();
            canClose = 1;
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            canClose = 1;
        }
    }
}
