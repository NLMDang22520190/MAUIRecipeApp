using MAUIRecipeApp.ViewModel.AdminViewModel;
using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class EditFoodRecipeView : ContentPage
{
	public EditFoodRecipeView(EditFoodRecipeViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	private void DifficultySlider_ValueChanged(object sender, ValueChangedEventArgs e)
	{
		if (sender is Slider slider)
		{
			slider.Value = Math.Round(e.NewValue);
		}
	}
}