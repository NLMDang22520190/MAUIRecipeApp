using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.Service;
using MAUIRecipeApp.View.AdminView.EditPage;
using MAUIRecipeApp.View.UserView;
using MAUIRecipeApp.View.UserView.Collection;
using System.Diagnostics;

namespace MAUIRecipeApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("home/fooddetail", typeof(FoodRecipePageView));
            Routing.RegisterRoute("edituser/editcurrentuser", typeof(EditCurrentUserPageView));
            Routing.RegisterRoute("editfoodrecipe/editcurrentfoodrecipe", typeof(EditCurrentFoodRecipePageView));
            Routing.RegisterRoute("editfoodtype/editcurrentfoodtype", typeof(EditCurrentFoodTypePageView));
            Routing.RegisterRoute("editingredient/editcurrentingredient", typeof(EditCurrentIngredientPageView));
            Routing.RegisterRoute("collectiondetail", typeof(CollectionDetailPageView));
            Routing.RegisterRoute("collectiondetail/fooddetail", typeof(FoodRecipePageView));
            Routing.RegisterRoute("collectiondetailinall", typeof(CollectionDetailInAllPageView));
            Routing.RegisterRoute("collectiondetailinall/fooddetail", typeof(FoodRecipePageView));
            Routing.RegisterRoute("savedrecipe/fooddetail", typeof(FoodRecipePageView));
        }

        private async void MenuItem_OnClicked(object? sender, EventArgs e)
        {
            await AuthService.Instance.LogOut();
        }
    }
}
