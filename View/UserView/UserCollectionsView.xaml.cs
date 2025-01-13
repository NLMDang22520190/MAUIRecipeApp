using MAUIRecipeApp.ViewModel.UserView;
namespace MAUIRecipeApp.View.UserView;

public partial class UserCollectionsView : ContentPage
{
	public UserCollectionsView(UserCollectionsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}