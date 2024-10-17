using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditUserInfoPageView : ContentPage
{
	public EditUserInfoPageView(EditUserInfoPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}