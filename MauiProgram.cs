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
using InputKit.Handlers;
using MAUIRecipeApp.Service;
using MAUIRecipeApp.View.AdminView;
using MAUIRecipeApp.ViewModel.AdminViewModel;
using Microsoft.Maui.LifecycleEvents;

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
                .UseMauiCommunityToolkitMediaElement()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddMaterialSymbolsFonts();
                    fonts.AddFontAwesomeIconFonts();
                }).ConfigureMauiHandlers(handlers =>
                {
                    // Add following line:
                    handlers.AddInputKitHandlers(); // 👈
                });



#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Đăng ký FirestoreService là Singleton
            builder.Services.AddSingleton(FirestoreService.Instance);
            builder.Services.AddMemoryCache();

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

            #region AdminView
            builder.Services.AddTransient<BackDropMainPageView>();
            builder.Services.AddTransient<BackDropMainPageViewModel>();

            builder.Services.AddTransient<AdminHomePageView>();
            builder.Services.AddTransient<AdminHomePageViewModel>();

            builder.Services.AddTransient<EditUserPageView>();
            builder.Services.AddTransient<EditUserPageViewModel>();

            builder.Services.AddTransient<EditUserInfoPageView>();
            builder.Services.AddTransient<EditUserInfoPageViewModel>();

            builder.Services.AddTransient<EditUserSavedRecipePageView>();
            builder.Services.AddTransient<EditUserSavedRecipePageViewModel>();

            builder.Services.AddTransient<EditFoodRecipePageView>();
            builder.Services.AddTransient<EditFoodRecipePageViewModel>();

            builder.Services.AddTransient<EditFoodRecipeTypePageView>();
            builder.Services.AddTransient<EditFoodRecipeTypePageViewModel>();

            builder.Services.AddTransient<EditFoodRecipeMappingPageView>();
            builder.Services.AddTransient<EditFoodRecipeMappingPageViewModel>();

            builder.Services.AddTransient<EditIngredientsPageView>();
            builder.Services.AddTransient<EditIngredientsPageViewModel>();

            builder.Services.AddTransient<EditRecipeIngredientsPageView>();
            builder.Services.AddTransient<EditRecipeIngredientsPageViewModel>();

            builder.Services.AddTransient<EditFoodRatingPageView>();
            builder.Services.AddTransient<EditFoodRatingPageViewModel>();

            builder.Services.AddTransient<EditTempTablePageView>();
            builder.Services.AddTransient<EditTempTablePageViewModel>();
            #endregion

            return builder.Build();
        }
    }
}
