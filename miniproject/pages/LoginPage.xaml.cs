using miniproject.Viewmodel;

namespace miniproject.pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = new LoginViewmodel();
	}
}