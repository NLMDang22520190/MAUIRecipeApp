using MAUIRecipeApp.Service;
using MAUIRecipeApp.ViewModel.UserView;
using Microsoft.Extensions.Configuration;

namespace MAUIRecipeApp.View.UserView;

public partial class HomePageView : ContentPage
{
    private readonly GeminiService _geminiService;
    public HomePageView(GeminiService geminiService)
	{
		InitializeComponent();
        _geminiService = geminiService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // T?o m?i ViewModel và gán l?i BindingContext
        BindingContext = new HomePageViewModel(_geminiService);
    }
}