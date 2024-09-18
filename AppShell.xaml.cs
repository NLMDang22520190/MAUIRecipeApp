using MAUIRecipeApp.View.UserView;

namespace MAUIRecipeApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("home/fooddetail", typeof(FoodRecipePageView));
        }
    }
}
