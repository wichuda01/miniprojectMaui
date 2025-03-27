using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using miniproject.Model;
using Microsoft.Maui.Storage;
using miniproject.pages;

namespace miniproject.Viewmodel;

public partial class LoginViewmodel : ObservableObject
{
    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private Students currentStudent; // เปลี่ยนจาก ObservableCollection เป็น single object

    [RelayCommand]
    public async Task Login()
    {

        using var stream = await FileSystem.OpenAppPackageFileAsync("students.json");
        using var reader = new StreamReader(stream);
        var contents = await reader.ReadToEndAsync();
        List<Students> studentList = Students.FromJson(contents);

        CurrentStudent = studentList.Find(u => u.Email == Email && u.Password == Password);

        if (CurrentStudent != null)
        {
            await GotoPage(nameof(HomePage));
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Invalid Email or Password", "OK");
        }
    }


    [RelayCommand]
    async Task GotoPage(string page)
    {
        if (CurrentStudent == null) return;

        var parameters = new Dictionary<string, object>
        {
            { "StudentData", CurrentStudent }
        };

        await Shell.Current.GoToAsync(page, parameters);
    }
}