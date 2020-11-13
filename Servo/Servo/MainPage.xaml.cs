using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Threading;

namespace Servo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }

        async void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs args)
        {
            await Task.Delay(5);
            double degree;
            if (args.Reading.Acceleration.Y >= -1 && args.Reading.Acceleration.Y <= 1)
            {
                degree = Math.Asin(args.Reading.Acceleration.Y);
                degree = degree * (180 / 3.14);
                degree = degree + 90;

                int read = (int)Math.Round((decimal)degree);

                if (read < 10)
                {
                    read = 0;
                }

                if (read > 170)
                {
                    read = 180;
                }

                if (read >= 0 && read <= 180)
                {
                    myReuslt.Text = read.ToString() + "°";
                    MessagingCenter.Send<MainPage, int>(this, "tilt", read);
                }
            }
        }

        private void Lock_Toggled(object sender, ToggledEventArgs e)
        {
            if (Accelerometer.IsMonitoring)
                Accelerometer.Stop();
            else
                Accelerometer.Start(SensorSpeed.UI);
        }
    }
}

