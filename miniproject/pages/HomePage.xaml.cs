using miniproject.Viewmodel;

namespace miniproject.pages;

public partial class HomePage : ContentPage
{

	public HomePage()
	{
		InitializeComponent();
		BindingContext = new HomeViewmodel();
	}

}