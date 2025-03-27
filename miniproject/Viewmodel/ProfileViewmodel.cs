using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using miniproject.Model;

namespace miniproject.Viewmodel;

[QueryProperty(nameof(ReceivedStudent), "StudentData")]
public partial class ProfileViewmodel : ObservableObject
{
    private Students _originalStudentData;

    [ObservableProperty]
    private ObservableCollection<Students> student = new();

    [ObservableProperty]
    private Students currentStudent;

    public Students ReceivedStudent
    {
        get => _receivedStudent;
        set
        {
            if (_receivedStudent != value)
            {
                _receivedStudent = value;
                OnReceivedStudentChanged(value);
            }
        }
    }
    private Students _receivedStudent;

    private void OnReceivedStudentChanged(Students value)
    {
        if (value == null) return;

        _originalStudentData = new Students
        {
            // คัดลอก properties ต่างๆ จาก value
            StudentId = value.StudentId,
            Email = value.Email,
            Password = value.Password,
            FirstName = value.FirstName,
            LastName = value.LastName,
            ImgUrl = value.ImgUrl,
            Faculty = value.Faculty,
            Major = value.Major,
            Year = value.Year,
            Gpa = value.Gpa,
            CoursesEnrolled = new List<CoursesEnrolled>(
            value.CoursesEnrolled.Select(c => new CoursesEnrolled
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                Year = c.Year,
                Term = c.Term,
                Credits = c.Credits,
                Grade = c.Grade,
                Instructor = c.Instructor
            }).ToList()
            )
        };

        CurrentStudent = new Students
        {
            StudentId = value.StudentId,
            Email = value.Email,
            Password = value.Password,
            FirstName = value.FirstName,
            LastName = value.LastName,
            ImgUrl = value.ImgUrl,
            Faculty = value.Faculty,
            Major = value.Major,
            Year = value.Year,
            Gpa = value.Gpa,
            CoursesEnrolled = new List<CoursesEnrolled>(
            value.CoursesEnrolled.Select(c => new CoursesEnrolled
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                Year = c.Year,
                Term = c.Term,
                Credits = c.Credits,
                Grade = c.Grade,
                Instructor = c.Instructor
            }).ToList()
            )
        };

        Term("1");
    }

    [RelayCommand]
    public async Task Term(string term)
    {

        int termInt = Convert.ToInt32(term);
        if (_originalStudentData != null)
        {
            var filteredCourses = _originalStudentData.CoursesEnrolled.Where(c => c.Term == termInt).ToList();
            CurrentStudent.CoursesEnrolled = filteredCourses;
            Student = new ObservableCollection<Students> { CurrentStudent };
        }
    }
    [RelayCommand]
    public async Task GotoPage(string page)
    {
        if (CurrentStudent != null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "StudentData", _originalStudentData }
            };
            await Shell.Current.GoToAsync(page, parameters);
        }
    }
}