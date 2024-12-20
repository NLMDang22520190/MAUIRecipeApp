using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditUserInfoPageView : UraniumContentPage
{
	public EditUserInfoPageView(EditUserInfoPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}