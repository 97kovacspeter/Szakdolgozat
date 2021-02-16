using ArguSzem.Models;
using ArguSzem.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ArguSzem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private bool _unauthorized;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool UnAuth
        {
            get => _unauthorized;
            set => SetProperty(ref _unauthorized, value);
        }

        public DelegateCommand LoginCommand => new DelegateCommand(async () => await ExecuteLogin());
        public DelegateCommand RegisterCommand => new DelegateCommand(async () => await NavigationService.NavigateAsync("RegisterView"));
        public DelegateCommand AboutCommand => new DelegateCommand(async () => await NavigationService.NavigateAsync("AboutView"));

        public LoginViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = "Bejelentkezés";
            UnAuth = false;
            Email = TokenHandlerModel.Username;
        }

        private async Task ExecuteLogin()
        {
            IsBusy = true;

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                try
                {
                    var current = Connectivity.NetworkAccess;
                    if (current == Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        var result = await DataStoreService.LoginAsync(Email, Password);

                        TokenHandlerModel.Username = Email;

                        if (result)
                        {
                            await NavigationService.NavigateAsync("SelectView");
                            UnAuth = false;
                        }
                        else
                        {
                            UnAuth = true;
                        }
                    }
                    else
                    {
                        await PageDialogService.DisplayAlertAsync("Hiba", "Hálózati hiba", "OK");
                    }
                }
                catch (NetworkException)
                {
                    UnAuth = true;
                }
                catch
                {
                    await PageDialogService.DisplayAlertAsync("Hiba", "Hálózati hiba", "OK");
                }
            }
            else
            {
                UnAuth = true;
            }

            IsBusy = false;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            UnAuth = false;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            Password = "";
        }
    }
}
