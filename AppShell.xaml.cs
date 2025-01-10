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
        }
    }
}
