using Prism.Navigation;
using Prism.Commands;
using System.Threading.Tasks;
using Prism.Services;
using ArguSzem.Models;
using ArguSzem.Services;

namespace ArguSzem.ViewModels
{
    public class SelectViewModel : BaseViewModel
    {
        public SelectViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = "Mód";
        }

        public DelegateCommand RecvCommand => new DelegateCommand(async () => await NavigationService.NavigateAsync("RecvView"));
        public DelegateCommand SenderCommand => new DelegateCommand(async () => await NavigationService.NavigateAsync("SenderView"));
        public DelegateCommand LogoutCommand => new DelegateCommand(async () => await ExecuteLogout());

        private async Task ExecuteLogout()
        {
            IsBusy = true;

            try
            {
                var result = await DataStoreService.LogoutAsync();

                if (result)
                {
                    await NavigationService.NavigateAsync("../");
                }
            }
            catch (NetworkException e)
            {
                await PageDialogService.DisplayAlertAsync("Hiba", e.Message.ToString(), "OK");
            }
            catch
            {
                await PageDialogService.DisplayAlertAsync("Hiba", "Hálózati hiba", "OK");
            }

            IsBusy = false;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            IsBusy = true;

            try
            {
                var port = await DataStoreService.GetPortAsync();
                TokenHandlerModel.Port = port;
            }
            catch
            {
                await NavigationService.NavigateAsync("../");
                await PageDialogService.DisplayAlertAsync("Hiba", "Hálózati hiba", "OK");
            }

            IsBusy = false;
        }

    }
}
