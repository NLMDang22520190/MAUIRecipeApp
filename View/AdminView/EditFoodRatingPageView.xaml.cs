using MAUIRecipeApp.ViewModel.AdminViewModel;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRatingPageView : ContentPage
{
	public EditFoodRatingPageView(EditFoodRatingPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}