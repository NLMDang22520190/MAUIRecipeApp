using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.Service;
using MAUIRecipeApp.View.AdminView.EditPage;
using MAUIRecipeApp.View.UserView;

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
        }

        private async void MenuItem_OnClicked(object? sender, EventArgs e)
        {
            await AuthService.Instance.LogOut();
        }
    }
}
