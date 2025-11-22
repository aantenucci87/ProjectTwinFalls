using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TwinFalls.Mobile
{
    public class Program : MauiApplication
    {
        public Program() : base(CreateMauiApp()) { }

        static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>();

            // configuration, DI registrations would go here
            return builder.Build();
        }

        protected override void OnLaunched(MauiLaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
