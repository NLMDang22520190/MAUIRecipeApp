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
using MAUIRecipeApp.View.AdminView.EditPage;
using MAUIRecipeApp.View.UserView.Collection;
using MAUIRecipeApp.ViewModel.AdminViewModel;
using MAUIRecipeApp.ViewModel.AdminViewModel.EditPage;
using MAUIRecipeApp.ViewModel.UserView.Collection;
using Microsoft.Maui.LifecycleEvents;
using Mopups.Hosting;

namespace MAUIRecipeApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Load appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(FileSystem.Current.AppDataDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            builder.Configuration.AddConfiguration(configuration);

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
                .ConfigureMopups()
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
            builder.Services.AddSingleton<GeminiService>();
            builder.Services.AddMopupsDialogs();
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

            builder.Services.AddTransient<SubmitNewRecipeView>();
            builder.Services.AddTransient<SubmitNewRecipeViewModel>();

            builder.Services.AddTransient<EditIngredientsView>();
            builder.Services.AddTransient<EditIngredientsViewModel>();

            builder.Services.AddTransient<UserRecipesView>();
            builder.Services.AddTransient<UserRecipesViewModel>();

            builder.Services.AddTransient<TestScreensView>();
            builder.Services.AddTransient<TestScreensViewModel>();

            builder.Services.AddTransient<UserSavedRecipesView>();
            builder.Services.AddTransient<UserSavedRecipesViewModel>();

            builder.Services.AddTransient<EditFoodRecipeView>();
            builder.Services.AddTransient<EditFoodRecipeViewModel>();

            builder.Services.AddTransient<ChatPageView>();
            builder.Services.AddTransient<ChatPageViewModel>();

            builder.Services.AddTransient<UserSavedRecipePageView>();
            builder.Services.AddTransient<UserSavedRecipePageViewModel>();

            builder.Services.AddTransient<UserSavedCollectionPageView>();
            builder.Services.AddTransient<UserSavedCollectionPageViewModel>();

            builder.Services.AddTransient<CollectionDetailPageView>();
            builder.Services.AddTransient<CollectionDetailPageViewModel>();

            builder.Services.AddTransient<AllCollectionPageView>();
            builder.Services.AddTransient<AllCollectionPageViewModel>();

            builder.Services.AddTransient<UserInfoPageViewModel>();
            builder.Services.AddTransient<UserInfoPageView>();
            #endregion

            #region AdminView

            builder.Services.AddTransient<AdminHomePageView>();
            builder.Services.AddTransient<AdminHomePageViewModel>();

            builder.Services.AddTransient<EditUserPageView>();
            builder.Services.AddTransient<EditUserPageViewModel>();

            builder.Services.AddTransient<EditCurrentUserPageView>();
            builder.Services.AddTransient<EditCurrentUserPageViewModal>();

            builder.Services.AddTransient<EditFoodRecipePageView>();
            builder.Services.AddTransient<EditFoodRecipePageViewModel>();

            builder.Services.AddTransient<EditCurrentFoodRecipePageView>();
            builder.Services.AddTransient<EditCurrentFoodRecipePageViewModal>();

            builder.Services.AddTransient<EditFoodRecipeTypePageView>();
            builder.Services.AddTransient<EditFoodRecipeTypePageViewModel>();

            builder.Services.AddTransient<EditCurrentFoodTypePageView>();
            builder.Services.AddTransient<EditCurrentFoodTypePageViewModal>();

            builder.Services.AddTransient<EditIngredientsPageView>();
            builder.Services.AddTransient<EditIngredientsPageViewModel>();

            builder.Services.AddTransient<EditCurrentIngredientPageView>();
            builder.Services.AddTransient<EditCurrentIngredientPageViewModal>();

            #endregion

            return builder.Build();
        }
    }
}
