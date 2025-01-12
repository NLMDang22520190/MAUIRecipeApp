using MAUIRecipeApp.ViewModel.UserView;

namespace MAUIRecipeApp.View.UserView;

public partial class CreateCollectionView : ContentPage
{
	public CreateCollectionView(CreateCollectionViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}