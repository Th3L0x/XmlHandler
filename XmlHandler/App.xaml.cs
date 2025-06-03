using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Windows;
using XmlHandler.Services;
using XmlHandler.Services.Interfaces;
using XmlHandler.View;
using XmlHandler.ViewModel;

namespace XmlHandler;

public partial class App : Application
{
    private readonly static IHost? _appHost;

    static App()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _appHost = Host.CreateDefaultBuilder()
           .UseSerilog()
           .ConfigureServices((context, services) =>
           {
               services.AddSingleton<IXmlHandlerService, XmlHandlerService>();
               services.AddTransient<INotificationService, NotificationService>();
               services.AddSingleton<MainViewModel>();
               services.AddSingleton<MainPage>();
           })
           .Build();
    }

    public App()
    {
       
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        if (_appHost == null) return;
        await _appHost.StartAsync();
        var mainPage = _appHost.Services.GetRequiredService<MainPage>();
        mainPage.Closed -= MainPage_Closed;
        mainPage.Closed += MainPage_Closed;
        mainPage.Show();
        base.OnStartup(e);
    }

    private void MainPage_Closed(object? sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_appHost != null)
        {
            await _appHost.StopAsync();
            _appHost.Dispose();
        }
        base.OnExit(e);
    }
}
