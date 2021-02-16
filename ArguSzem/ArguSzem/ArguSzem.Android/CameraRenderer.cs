using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Widget;
using ArguSzem.Droid;
using Java.Lang;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ArguSzem.ViewModels.CameraPreview), typeof(CameraRenderer))]
namespace ArguSzem.Droid
{
    public class CameraRenderer : ViewRenderer<ViewModels.CameraPreview, ArguSzem.Droid.CameraPreview>
    {
        CameraPreview cameraPreview;

        public CameraRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ArguSzem.ViewModels.CameraPreview> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    cameraPreview = new CameraPreview(Context);
                    SetNativeControl(cameraPreview);
                }
                try
                {
                    Control.Preview = Camera.Open((int)e.NewElement.Camera);
                }
                catch
                {
                    JavaSystem.Exit(0);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.Preview.Release();
            }
            base.Dispose(disposing);
        }
    }
}