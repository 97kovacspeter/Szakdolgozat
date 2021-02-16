using ArguSzem.Models;
using ArguSzem.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ArguSzem.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private string _confirmPassword;
        private bool _regFail;

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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool RegFail
        {
            get => _regFail;
            set => SetProperty(ref _regFail, value);
        }

        public DelegateCommand RegisterCommand => new DelegateCommand(async () => await ExecuteRegister());

        private async Task ExecuteRegister()
        {
            IsBusy = true;

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                try
                {
                    var current = Connectivity.NetworkAccess;
                    if (current == Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        var isRegisterSuccess = await DataStoreService.RegisterAsync(Email, Password, ConfirmPassword);

                        TokenHandlerModel.Username = Email;

                        var isLoginSuccess = await DataStoreService.LoginAsync(Email, Password);

                        if (isRegisterSuccess && isLoginSuccess)
                        {
                            RegFail = false;
                            await NavigationService.NavigateAsync("SelectView");
                        }
                        else
                        {
                            RegFail = true;
                        }
                    }
                    else
                    {
                        await PageDialogService.DisplayAlertAsync("Hiba", "Hálózati hiba", "OK");
                    }
                }
                catch(NetworkException)
                {
                    RegFail = true;
                }
                catch
                {
                    await PageDialogService.DisplayAlertAsync("Hiba", "Hálózati hiba", "OK");
                }
            }
            else
            {
                RegFail = true;
            }

            IsBusy = false;
        }

        public RegisterViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = "Regisztráció";
            RegFail = false;
            Email = TokenHandlerModel.Username;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            RegFail = false;
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            Password = "";
            ConfirmPassword = "";
        }
    }
}