using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;

namespace ArguSzem.Droid
{
    [Activity(Label = "ArguSzem", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: false);

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                LoadApplication(new App());
            }
            else
            {
                RequestPermissions(new string[] { Manifest.Permission.Camera }, 1);
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if(grantResults[0]==Permission.Granted)
            {
                LoadApplication(new App());
            }
            else
            {
                Toast.MakeText(this, "This app needs the camera permissions to function properly.", ToastLength.Long).Show();
                FinishAffinity();
            }
        }
    }
}
