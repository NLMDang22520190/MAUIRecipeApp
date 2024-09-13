using MAUIRecipeApp.ViewModel;

namespace MAUIRecipeApp.View;

public partial class StartUpPageView : ContentPage
{
	public StartUpPageView(StartUpPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}