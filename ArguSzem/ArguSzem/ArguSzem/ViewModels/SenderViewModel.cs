using ArguSzem.Models;
using ArguSzem.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ArguSzem.ViewModels
{
    public class SenderViewModel : BaseViewModel
    {
        private bool _keep;

        public bool Keep
        {
            get => _keep;
            set => SetProperty(ref _keep, value);
        }
        public static Socket Sock { get; private set; }
        public static IPEndPoint EndP { get; private set; }

        public SenderViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = "Küldő mód";
            EndP = new IPEndPoint(IPAddress.Parse("52.236.157.144"), 8000);
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public DelegateCommand LogoutCommand => new DelegateCommand(async () => await ExecuteLogout());

        private async Task ExecuteLogout()
        {
            IsBusy = true;
            var current = Connectivity.NetworkAccess;
            if (current == Xamarin.Essentials.NetworkAccess.Internet)
            {
                try
                {
                    var result = await DataStoreService.LogoutAsync();

                    if (result)
                    {
                        await NavigationService.NavigateAsync("../../");
                    }
                }
                catch
                {
                    NetworkStop();
                }
            }
            else
            {
                NetworkStop();
            }
            IsBusy = false;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Keep = true;
            var current = Connectivity.NetworkAccess;
            if (current == Xamarin.Essentials.NetworkAccess.Internet)
            {
                _ = KeepAlive();
            }
            else
            {
                NetworkStop();
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Keep = false;
        }

        public async void NetworkStop()
        {
            await NavigationService.NavigateAsync("../../");
            await PageDialogService.DisplayAlertAsync("Hiba", "Hálózati hiba", "OK");
        }

        public async Task KeepAlive()
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        while (Keep)
                        {
                            var current = Connectivity.NetworkAccess;
                            if (current == Xamarin.Essentials.NetworkAccess.Internet)
                            {
                                try
                                {
                                    var data = Encoding.ASCII.GetBytes(TokenHandlerModel.Port + "|sender");
                                    Sock.SendTo(data, EndP);
                                }
                                catch
                                {
                                    var e = new Exception();
                                    throw e;
                                }
                            }
                            else
                            {
                                var e = new Exception();
                                throw e;
                            }
                        }
                    }
                    catch
                    {
                        var e = new Exception();
                        throw e;
                    }
                });
            }
            catch
            {
                NetworkStop();
            }
        }
    }
}
