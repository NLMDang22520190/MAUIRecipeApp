using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class HomePageView : ContentPage
{
	public HomePageView(HomePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

	private async void OnSubmitNewRecipeClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("///submitnewrecipe");
	}
}