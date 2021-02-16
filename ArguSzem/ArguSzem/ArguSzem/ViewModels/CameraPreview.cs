using Xamarin.Forms;

namespace ArguSzem.ViewModels
{
	public class CameraPreview : View
	{
		public static readonly BindableProperty CameraProperty = BindableProperty.Create(
			propertyName: "Camera",
			returnType: typeof(CameraOptions),
			declaringType: typeof(CameraPreview),
			defaultValue: CameraOptions.Rear);

		public CameraOptions Camera
		{
			get => (CameraOptions)GetValue(CameraProperty);
			set => SetValue(CameraProperty, value);
		}
	}
}
