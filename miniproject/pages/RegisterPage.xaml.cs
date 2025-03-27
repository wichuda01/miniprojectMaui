using miniproject.Viewmodel;

namespace miniproject.pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
		BindingContext = new RegisterViewmodel();

	}
}