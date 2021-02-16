using Prism.Navigation;
using Prism.Services;

namespace ArguSzem.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = "Az alkalmazásról";
        }
    }
}