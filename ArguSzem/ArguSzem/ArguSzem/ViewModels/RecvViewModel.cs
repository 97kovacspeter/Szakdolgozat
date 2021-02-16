using ArguSzem.Models;
using ArguSzem.Services;
using Java.Util.Zip;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ArguSzem.ViewModels
{
    public class RecvViewModel : BaseViewModel
    {
        private bool _keep;
        private ImageSource _image;

        public static Socket Sock { get; private set; }
        public static IPEndPoint EndP { get; private set; }

        public bool Keep
        {
            get => _keep;
            set => SetProperty(ref _keep, value);
        }
        
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public RecvViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = "Megfigyelő mód";
            EndP = new IPEndPoint(IPAddress.Parse("52.236.157.144"), 8000);
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public DelegateCommand LogoutCommand => new DelegateCommand(async () => await ExecuteLogout());

        private async Task ExecuteLogout()
        {
            IsBusy = true;

            try
            {
                var result = await DataStoreService.LogoutAsync();

                if (result)
                {
                    await NavigationService.NavigateAsync("../../");
                }
            }
            catch (NetworkException e)
            {
                await PageDialogService.DisplayAlertAsync("Hiba", e.Message.ToString(), "OK");
            }
            catch
            {
                NetworkStop();
            }

            IsBusy = false;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Keep = true;
            Image = "wait.png";

            var current = Connectivity.NetworkAccess;
            if (current == Xamarin.Essentials.NetworkAccess.Internet)
            {
                KeepAlive();
                Receive();
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
        public async void KeepAlive()
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
                                    var data = Encoding.ASCII.GetBytes(TokenHandlerModel.Port + "|receiver");
                                    Sock.SendTo(data, EndP);
                                    Thread.Sleep(3000);
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

        public async void Receive()
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        while (Keep)
                        {

                            var buffer = new byte[65000];
                            var ep = (EndPoint)EndP;

                            var current = Connectivity.NetworkAccess;
                            if (current == Xamarin.Essentials.NetworkAccess.Internet)
                            {
                                try
                                {
                                    Sock.ReceiveFrom(buffer, ref ep);
                                    if (buffer[0] == 117 && buffer[1] == 114 && buffer[2] == 101 && buffer[3] == 115)
                                    {
                                        Image = "done.png";
                                    }
                                    else if (buffer[4] != 124 && buffer[5] != 115 && buffer[6] != 101)
                                    {
                                        byte[] bufferInflated = new byte[300000];

                                        Inflater decompresser = new Inflater();
                                        decompresser.SetInput(buffer, 0, 65000);
                                        int length = decompresser.Inflate(bufferInflated);
                                        decompresser.End();

                                        Array.Resize(ref bufferInflated, length);
                                        Image = ImageSource.FromStream(() => new MemoryStream(bufferInflated));
                                    }
                                    
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

