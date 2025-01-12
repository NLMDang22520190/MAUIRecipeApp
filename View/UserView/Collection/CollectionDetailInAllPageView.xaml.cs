using MAUIRecipeApp.ViewModel.UserView.Collection;

namespace MAUIRecipeApp.View.UserView.Collection;

public partial class CollectionDetailInAllPageView : ContentPage
{
	public CollectionDetailInAllPageView(CollectionDetailInAllPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CollectionDetailInAllPageViewModel vm)
        {
            vm.OnAppearing();
        }
    }
}