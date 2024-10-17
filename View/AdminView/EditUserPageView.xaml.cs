using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditUserPageView : ContentPage
{
	public EditUserPageView(EditUserPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}