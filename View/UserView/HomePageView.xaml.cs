using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class HomePageView : ContentPage
{
	public HomePageView(HomePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}