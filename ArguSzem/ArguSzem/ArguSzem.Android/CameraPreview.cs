using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Views;
using ArguSzem.ViewModels;
using Java.Lang;
using Java.Util.Zip;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Xamarin.Essentials;
using static Android.Hardware.Camera;

namespace ArguSzem.Droid
{
	public sealed class CameraPreview : ViewGroup, ISurfaceHolderCallback, IPreviewCallback
	{
		public SurfaceView surfaceView;
		readonly ISurfaceHolder holder;
		Camera.Size previewSize;
		IList<Camera.Size> supportedPreviewSizes;
		Camera camera;

		readonly IWindowManager windowManager;
		private readonly IPEndPoint EndP = SenderViewModel.EndP;
		private readonly Socket Sock = SenderViewModel.Sock;

		public bool IsPreviewing { get; set; }

		public Camera Preview
		{
			get { return camera; }
			set
			{
				camera = value;
				if (camera != null)
				{
					supportedPreviewSizes = Preview.GetParameters().SupportedPreviewSizes;
					RequestLayout();
				}
			}
		}

		public CameraPreview(Context context)
			: base(context)
		{
			surfaceView = new SurfaceView(context);
			AddView(surfaceView);
			windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

			IsPreviewing = false;
			holder = surfaceView.Holder;
			holder.AddCallback(this);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			int width = ResolveSize(SuggestedMinimumWidth, widthMeasureSpec);
			int height = ResolveSize(SuggestedMinimumHeight, heightMeasureSpec);
			SetMeasuredDimension(width, height);

			if (supportedPreviewSizes != null)
			{
				previewSize = GetOptimalPreviewSize(supportedPreviewSizes, width, height);
			}
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
			var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

			surfaceView.Measure(msw, msh);
			surfaceView.Layout(0, 0, r - l, b - t);
		}

		public void SurfaceCreated(ISurfaceHolder holder)
		{
			try
			{
				if (Preview != null)
				{
					Preview.SetPreviewDisplay(holder);
				}
			}
			catch (Java.Lang.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("ERROR: ", ex.Message);
			}
		}

		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
			if (Preview != null)
			{
				camera.SetPreviewCallback(null);
				Preview.StopPreview();
			}
		}

		public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
		{
			var parameters = Preview.GetParameters();
			parameters.SetPreviewSize(previewSize.Width, previewSize.Height);
			RequestLayout();
			switch (windowManager.DefaultDisplay.Rotation) {
			case SurfaceOrientation.Rotation0:
				camera.SetDisplayOrientation (90);
				break;
			case SurfaceOrientation.Rotation90:
				camera.SetDisplayOrientation (0);
				break;
			case SurfaceOrientation.Rotation270:
				camera.SetDisplayOrientation (180);
				break;
			}

			Preview.SetParameters(parameters);
			Preview.SetPreviewCallback(this);
			Preview.StartPreview();
			IsPreviewing = true;
		}

		void IPreviewCallback.OnPreviewFrame(byte[] b, Camera c)
		{
			Android.Graphics.YuvImage yuvImage = new Android.Graphics.YuvImage(b, Android.Graphics.ImageFormat.Nv21, previewSize.Width, previewSize.Height, null);
			byte[] msg = new byte[65000];
			MemoryStream ms = new MemoryStream();
			yuvImage.CompressToJpeg(new Android.Graphics.Rect(0, 0, previewSize.Width, previewSize.Height), 100, ms);

			Deflater compresser = new Deflater();
			compresser.SetInput(ms.ToArray());
			compresser.Finish();
			compresser.Deflate(msg); //int length = compresser.Deflate(msg);
			compresser.End();

			var current = Connectivity.NetworkAccess;
			if (current == Xamarin.Essentials.NetworkAccess.Internet)
			{
				try
				{
					Sock.SendTo(msg, EndP);
				}
				catch
				{
				}
			}
			Thread.Sleep(300);
		}

		Camera.Size GetOptimalPreviewSize(IList<Camera.Size> sizes, int w, int h)
		{
			const double AspectTolerance = 0.1;
			double targetRatio = (double)w / h;

			if (sizes == null)
			{
				return null;
			}

			Camera.Size optimalSize = null;
			double minDiff = double.MaxValue;

			int targetHeight = 288;
			foreach (Camera.Size size in sizes)
			{
				double ratio = (double)size.Width / size.Height;

				if (Java.Lang.Math.Abs(ratio - targetRatio) > AspectTolerance)
					continue;
				if (Java.Lang.Math.Abs(size.Height - targetHeight) < minDiff)
				{
					optimalSize = size;
					minDiff = Java.Lang.Math.Abs(size.Height - targetHeight);
				}
			}

			if (optimalSize == null)
			{
				minDiff = double.MaxValue;
				foreach (Camera.Size size in sizes)
				{
					if (Java.Lang.Math.Abs(size.Height - targetHeight) < minDiff)
					{
						optimalSize = size;
						minDiff = Java.Lang.Math.Abs(size.Height - targetHeight);
					}
				}
			}

			return optimalSize;
		}
	}
}