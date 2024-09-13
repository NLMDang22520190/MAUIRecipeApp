using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using UraniumUI;
using MAUIRecipeApp.View;
using MAUIRecipeApp.ViewModel;

namespace MAUIRecipeApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddMaterialSymbolsFonts();
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<StartUpPageView>();
            builder.Services.AddSingleton<StartUpPageViewModel>();

            return builder.Build();
        }
    }
}
