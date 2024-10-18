using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRecipePageView : UraniumContentPage
{
	public EditFoodRecipePageView(EditFoodRecipePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = (Slider)sender;
        slider.Value = Math.Round(slider.Value);
    }

}