using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using UraniumUI;
using MAUIRecipeApp.View;
using MAUIRecipeApp.ViewModel;
using MAUIRecipeApp.View.Auth;
using MAUIRecipeApp.ViewModel.Auth;
using MAUIRecipeApp.View.UserView;
using MAUIRecipeApp.ViewModel.UserView;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Đọc tệp appsettings.json
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("MAUIRecipeApp.appsettings.json");
            var config = new ConfigurationBuilder()
                            .AddJsonStream(stream)
                            .Build();

            // Đăng ký IConfiguration
            builder.Services.AddSingleton<IConfiguration>(config);
            // Đăng ký cấu hình vào DI container
            builder.Configuration.AddConfiguration(config);

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
                    fonts.AddFontAwesomeIconFonts();
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Đăng ký FirestoreService là Singleton
            builder.Services.AddSingleton(FirestoreService.Instance);

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

            builder.Services.AddTransient<FoodRecipePageView>();
            builder.Services.AddTransient<FoodRecipePageViewModel>();
            #endregion

            return builder.Build();
        }
    }
}
