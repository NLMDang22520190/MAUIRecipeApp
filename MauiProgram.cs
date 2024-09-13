using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using UraniumUI;
using MAUIRecipeApp.View;
using MAUIRecipeApp.ViewModel;
using MAUIRecipeApp.View.Auth;
using MAUIRecipeApp.ViewModel.Auth;
using MAUIRecipeApp.View.UserView;
using MAUIRecipeApp.ViewModel.UserView;

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

            builder.Services.AddTransient<PasswordVerifyCodePageView>();
            builder.Services.AddTransient<PasswordVerifyCodePageViewModel>();

            builder.Services.AddTransient<NewPasswordPageView>();
            builder.Services.AddTransient<NewPasswordPageViewModel>();

            builder.Services.AddTransient<VerifyCodePageView>();
            builder.Services.AddTransient<VerifyCodePageViewModel>();
            #endregion

            #region UserView
            builder.Services.AddTransient<HomePageView>();
            builder.Services.AddTransient<HomePageViewModel>();
            #endregion

            return builder.Build();
        }
    }
}
