using CoreConnect.Desktop.Shared;
using CoreConnect.Desktop.Shared.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CoreConnect.Desktop.UI.Services;

public class SessionIndicator : ISessionIndicator
{
    private readonly IUiDispatcher _dispatcher;

    public SessionIndicator(IUiDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    public void Show()
    {
        _dispatcher.Post(() =>
        {
            var indicatorWindow = new SessionIndicatorWindow()
            {
                DataContext = StaticServiceProvider.Instance?.GetRequiredService<ISessionIndicatorWindowViewModel>()
            };
            _dispatcher.ShowMainWindow(indicatorWindow);
        });
    }
}
