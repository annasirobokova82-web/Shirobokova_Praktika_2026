using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShirobokovaPartnerLib.Data;
using ShirobokovaPartnerLib.Services;

namespace ShirobokovaPartnerApp
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = new MainWindow(
                _serviceProvider.GetRequiredService<PartnerService>(),
                _serviceProvider
            );
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "Host=localhost;Port=5432;Database=shirobokova;Username=app;Password=123456789";

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<PartnerService>();
            services.AddSingleton<MainWindow>();
        }
    }
}