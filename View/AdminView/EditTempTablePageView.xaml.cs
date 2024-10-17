using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditTempTablePageView : ContentPage
{
	public EditTempTablePageView(EditTempTablePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}