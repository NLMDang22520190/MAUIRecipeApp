using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class ChatPageView : ContentPage
{
	public ChatPageView(ChatPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}