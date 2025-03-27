using miniproject.Viewmodel;

namespace miniproject.pages;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		BindingContext = new ProfileViewmodel();
	}
}