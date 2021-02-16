using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ArguSzem.Views;
using Prism.Unity;
using Prism;
using Prism.Ioc;
using ArguSzem.ViewModels;
using FFImageLoading;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ArguSzem
{
    public partial class App : PrismApplication
    {
        public static string AzureBackendUrl = "https://arguszem.azurewebsites.net";

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            ImageService.Instance.Initialize();
            await NavigationService.NavigateAsync("NavigationPage/LoginView");
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<AboutView, AboutViewModel>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<RecvView, RecvViewModel>();
            containerRegistry.RegisterForNavigation<RegisterView, RegisterViewModel>();
            containerRegistry.RegisterForNavigation<SelectView, SelectViewModel>();
            containerRegistry.RegisterForNavigation<SenderView, SenderViewModel>();
        }
    }
}
