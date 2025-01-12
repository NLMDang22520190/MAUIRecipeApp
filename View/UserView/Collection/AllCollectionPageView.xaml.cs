using MAUIRecipeApp.ViewModel.UserView.Collection;

namespace MAUIRecipeApp.View.UserView.Collection;

public partial class AllCollectionPageView : ContentPage
{
	public AllCollectionPageView(AllCollectionPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AllCollectionPageViewModel vm)
        {
            vm.OnAppearing();
        }
    }
}