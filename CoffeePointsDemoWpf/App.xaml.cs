using ActivityScheduler.Data.Managers;
using CoffeePointsDemo;
using CoffeePointsDemo.Service;
using Microsoft.Extensions.DependencyInjection;
using RIFDC;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CoffeePointsDemoWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private Serilog.ILogger _logger;
        public App()
        {
            ServiceCollection services = new ServiceCollection();

            ConfigureServices(services);
            
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {

            services.AddSingleton<CoffeePointsDemoFrm>();

            var _app = new ApplicatonObject();

            _app.Connect();

            var keeper = new ItemKeeper<CoffeePoint>(_app.MainDataRoom);

            services.AddSingleton(typeof(ApplicatonObject), (x) => _app);

            string logFilePath = System.IO.Path.Combine(_app.LogsDirectory, Functions.GetNextFreeFileName(_app.LogsDirectory, "CoffeePointsDemoWpfLogs", "txt"));

            Serilog.ILogger _logger = new LoggerConfiguration()
                  .WriteTo.File(logFilePath)
                  .CreateLogger();

            services.AddSingleton(typeof(Serilog.ILogger), (x) => _logger);

            services.AddScoped<CoffeePointsDemoViewModel>();
            services.AddScoped<CoffeePointsManager>();
            services.AddScoped<CoffeePointsManager>();
            services.AddSingleton(typeof(ItemKeeper<CoffeePoint>), (x) => keeper);
            services.AddScoped<DataFillManager>();
            

        }

        public void OnStartup(object sender, StartupEventArgs e)
        {
            _logger = _serviceProvider.GetService<Serilog.ILogger>();

            _logger.Information("Starting Rifdc wpf demo app");

            var window = _serviceProvider.GetService<CoffeePointsDemoFrm>();

            var app = _serviceProvider.GetService<ApplicatonObject>();

            var appConnectResult = app.Connect();

            if (!appConnectResult.Success)
            {
                _logger.Information("Failed to connect, app exits");
            }

            var dataFillManager = _serviceProvider.GetService<DataFillManager>();

            var repo = _serviceProvider.GetService<ItemKeeper<CoffeePoint>>();

            repo.readItems();

            window.Show();
        }


    }

}
