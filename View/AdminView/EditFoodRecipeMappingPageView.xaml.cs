using System.Diagnostics;
using MAUIRecipeApp.ViewModel.AdminViewModel;
using UraniumUI.Material.Controls;
using UraniumUI.Pages;

namespace MAUIRecipeApp.View.AdminView;

public partial class EditFoodRecipeMappingPageView : UraniumContentPage
{
	public EditFoodRecipeMappingPageView(EditFoodRecipeMappingPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    private void OnPageSizeChanged(object sender, EventArgs e)
    {
        // Chỉ cần gọi InvalidateMeasure để reload lại kích thước của DataGrid
        Debug.WriteLine("Change");
        this.OnAppearing();
    }
}