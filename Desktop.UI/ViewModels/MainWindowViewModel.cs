using Microsoft.Extensions.Logging;
using CoreConnect.Desktop.Shared.Services;

namespace CoreConnect.Desktop.UI.ViewModels;

public interface IMainWindowViewModel : IBrandedViewModelBase
{
}

public class MainWindowViewModel : BrandedViewModelBase, IMainWindowViewModel
{
    public MainWindowViewModel(
        IBrandingProvider brandingProvider,
        IUiDispatcher dispatcher,
        ILogger<BrandedViewModelBase> logger)
        : base(brandingProvider, dispatcher, logger)
    {
    }
}
