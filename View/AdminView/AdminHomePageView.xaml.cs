using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class AdminHomePageView : ContentPage
{
	public AdminHomePageView(AdminHomePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}