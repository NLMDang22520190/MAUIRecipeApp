using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using UraniumUI;
using MAUIRecipeApp.View;
using MAUIRecipeApp.ViewModel;
using MAUIRecipeApp.View.Auth;
using MAUIRecipeApp.ViewModel.Auth;

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

            builder.Services.AddTransient<StartUpPageView>();
            builder.Services.AddTransient<StartUpPageViewModel>();

            #region Auth
            builder.Services.AddTransient<LoginPageView>();
            builder.Services.AddTransient<LoginPageViewModel>();

            builder.Services.AddTransient<SignUpPageView>();
            builder.Services.AddTransient<SignUpPageViewModel>();

            builder.Services.AddTransient<PasswordRecoveryPageView>();
            builder.Services.AddTransient<PasswordRecoveryPageViewModel>();


            #endregion

            return builder.Build();
        }
    }
}
