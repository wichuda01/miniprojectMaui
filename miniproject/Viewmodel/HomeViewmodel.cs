// using System.Collections.ObjectModel;
// using CommunityToolkit.Mvvm.ComponentModel;
// using CommunityToolkit.Mvvm.Input;
// using miniproject.Model;

// namespace miniproject.Viewmodel;

// [QueryProperty(nameof(ReceivedStudent), "StudentData")]
// public partial class HomeViewmodel : ObservableObject
// {
//     [ObservableProperty]
//     private Students receivedStudent;

//     [ObservableProperty]
//     private Students currentStudent;

//     [ObservableProperty]
//     ObservableCollection<Students> student = new ObservableCollection<Students>();
//     partial void OnReceivedStudentChanged(Students value)
//     {
//         if (value != null)
//         {
//             CurrentStudent = value;
//             Student = new ObservableCollection<Students> { CurrentStudent };
//         }
//     }

//     [RelayCommand]
//     public async Task GotoPage(string page)
//     {
//         if (CurrentStudent != null)
//         {
//             var parameters = new Dictionary<string, object>
//             {
//                 { "StudentData", CurrentStudent }
//             };
//             await Shell.Current.GoToAsync(page, parameters);
//         }
//     }
// }
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using miniproject.Model;
using miniproject.pages;

namespace miniproject.Viewmodel;

[QueryProperty(nameof(ReceivedStudent), "StudentData")]
public partial class HomeViewmodel : ObservableObject
{
    [ObservableProperty]
    private Students receivedStudent;

    [ObservableProperty]
    private Students currentStudent;

    [ObservableProperty]
    private ObservableCollection<Students> student = new();

    [ObservableProperty]
    private ObservableCollection<Courses> course = new();

    [ObservableProperty]
    private string courseName = "";

    private Students _originalStudentData;

    partial void OnReceivedStudentChanged(Students value)
    {
        if (value != null)
        {
            _originalStudentData = CloneStudent(value);
            CurrentStudent = CloneStudent(value);
            Student = new ObservableCollection<Students> { CurrentStudent };
            LoadCourses();
        }
    }

    private Students CloneStudent(Students student)
    {
        return new Students
        {
            StudentId = student.StudentId,
            Email = student.Email,
            Password = student.Password,
            FirstName = student.FirstName,
            LastName = student.LastName,
            ImgUrl = student.ImgUrl,
            Faculty = student.Faculty,
            Major = student.Major,
            Year = student.Year,
            Gpa = student.Gpa,
            CoursesEnrolled = student.CoursesEnrolled.Select(c => new CoursesEnrolled
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                Year = c.Year,
                Term = c.Term,
                Credits = c.Credits,
                Grade = c.Grade,
                Instructor = c.Instructor
            }).ToList()
        };
    }

    private async void LoadCourses()
    {
        using var c_stream = await FileSystem.OpenAppPackageFileAsync("courses.json");
        using var c_reader = new StreamReader(c_stream);
        var c_contents = await c_reader.ReadToEndAsync();
        Course = new ObservableCollection<Courses>(Courses.FromJson(c_contents));
    }


    [RelayCommand]
    public void SearchCourse()
    {
        if (string.IsNullOrEmpty(courseName))
        {
            LoadCourses();
        }
        else
        {
            Course = new ObservableCollection<Courses>(Course.Where(c =>
                c.CourseName.Contains(courseName, StringComparison.OrdinalIgnoreCase)));
        }
    }

    [ObservableProperty]
    string router = nameof(RegisterPage);

    [RelayCommand]
    async Task GotoPageRegister(string page){
        if (CurrentStudent != null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "StudentData", _originalStudentData }
            };
            await Shell.Current.GoToAsync(page, parameters);
        }
    }

    [RelayCommand]
    public async Task GotoPage(string page)
    {
        if (CurrentStudent != null)
        {
            var parameters = new Dictionary<string, object> { { "StudentData", _originalStudentData } };
            await Shell.Current.GoToAsync(page, parameters);
        }
    }
}

