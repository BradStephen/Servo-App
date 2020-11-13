using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using System.Linq;
using Xamarin.Forms;
using System.Threading;
using AlertDialog = Android.App.AlertDialog;
using System.Threading.Tasks;

namespace Servo.Droid
{
    [Activity(Label = "Servo", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        BluetoothConnection myConnection = new BluetoothConnection();

        int data;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MessagingCenter.Subscribe<MainPage, int>(this, "tilt", (sender, e) =>  // this is going from MainPage to HERE (Board)
            {
                data = e;
                Output_To_Bluetooth();
            });

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            BluetoothSocket _socket = null;

            {
                myConnection = new BluetoothConnection();
                myConnection.getAdapter();
                myConnection.thisAdapter.StartDiscovery();

                try
                {
                    myConnection.getDevice();
                    myConnection.thisDevice.SetPairingConfirmation(false);
                    myConnection.thisDevice.SetPairingConfirmation(true);
                    myConnection.thisDevice.CreateBond();
                }
                catch (Exception)
                { }
                myConnection.thisAdapter.CancelDiscovery();

                try
                { _socket = myConnection.thisDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")); }
                catch (Exception)
                { }

                myConnection.thisSocket = _socket;

                try
                {
                    myConnection.thisSocket.Connect();
                }
                catch (Exception)
                { }
            };

            LoadApplication(new App());
        }

        public void Output_To_Bluetooth()
        {
            try
            {
                myConnection.thisSocket.OutputStream.WriteByte(Convert.ToByte(data));
            }
            catch (Exception)
            {}
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public class BluetoothConnection
        {
            public void getAdapter() { this.thisAdapter = BluetoothAdapter.DefaultAdapter; }
            public void getDevice() { this.thisDevice = (from bd in this.thisAdapter.BondedDevices where bd.Name == "HC-06" select bd).FirstOrDefault(); }
            public BluetoothAdapter thisAdapter { get; set; }
            public BluetoothDevice thisDevice { get; set; }
            public BluetoothSocket thisSocket { get; set; }
        }
    }
}