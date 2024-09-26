using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class FoodRecipePageView : ContentPage
{
	public FoodRecipePageView(FoodRecipePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is FoodRecipePageViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Dừng video bằng cách đặt Source thành trang trắng
        MyWebView.Source = "about:blank";
    }
}