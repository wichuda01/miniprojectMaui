using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using miniproject.Model;

namespace miniproject.Viewmodel;

[QueryProperty(nameof(ReceivedStudent), "StudentData")]

public partial class RegisterViewmodel : ObservableObject
{
    [ObservableProperty]
    private string courseName = "";

    [ObservableProperty]
    private ObservableCollection<Courses> course = new();

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

    private async Task OnReceivedStudentChanged(Students value)
    {
        if (value == null) return;

        using var c_stream = await FileSystem.OpenAppPackageFileAsync("courses.json");

        using var c_reader = new StreamReader(c_stream);
        var c_contents = await c_reader.ReadToEndAsync();
        List<Courses> courseList = Courses.FromJson(c_contents);
        Course = new ObservableCollection<Courses>(courseList);

        _originalStudentData = new Students
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

        Term("3");
    }

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
    public void AddCourse(string cid)
    {
        System.Diagnostics.Debug.WriteLine($"cid: {cid}");

        if (CurrentStudent != null && !string.IsNullOrEmpty(cid))
        {
            var courseToAdd = Course.FirstOrDefault(c => c.CourseId == cid);
            if (courseToAdd != null)
            {
                var newCourseEnrolled = new CoursesEnrolled
                {
                    CourseId = courseToAdd.CourseId,
                    CourseName = courseToAdd.CourseName,
                    Year = courseToAdd.Year,
                    Term = courseToAdd.Term,
                    Credits = courseToAdd.Credits,
                    Grade = "",
                    Instructor = courseToAdd.Instructor
                };

                if (CurrentStudent.CoursesEnrolled == null)
                {
                    CurrentStudent.CoursesEnrolled = new List<CoursesEnrolled>();
                }

                if (!CurrentStudent.CoursesEnrolled.Any(c => c.CourseId == newCourseEnrolled.CourseId))
                {
                    CurrentStudent.CoursesEnrolled.Add(newCourseEnrolled);
                    _originalStudentData.CoursesEnrolled.Add(newCourseEnrolled);
                    Student = new ObservableCollection<Students> { CurrentStudent };
                }
            }
        }
    }
    [RelayCommand]
    public void DropCourse(string cid)
    {
        if (CurrentStudent != null && !string.IsNullOrEmpty(cid))
        {
            var courseToRemove = CurrentStudent.CoursesEnrolled.FirstOrDefault(c => c.CourseId == cid);
            if (courseToRemove != null)
            {
                CurrentStudent.CoursesEnrolled.Remove(courseToRemove);
                _originalStudentData.CoursesEnrolled.Remove(courseToRemove);
                Student = new ObservableCollection<Students> { CurrentStudent };
            }
        }
    }
    [RelayCommand]
    public void SearchCourse()
    {
        if (string.IsNullOrEmpty(courseName))
        {
            using var c_stream = FileSystem.OpenAppPackageFileAsync("courses.json").Result;
            using var c_reader = new StreamReader(c_stream);
            var c_contents = c_reader.ReadToEndAsync().Result;
            List<Courses> courseList = Courses.FromJson(c_contents);
            Course = new ObservableCollection<Courses>(courseList);
        }
        else
        {
            using var c_stream = FileSystem.OpenAppPackageFileAsync("courses.json").Result;
            using var c_reader = new StreamReader(c_stream);
            var c_contents = c_reader.ReadToEndAsync().Result;
            List<Courses> courseList = Courses.FromJson(c_contents);
            var filteredCourses = courseList.Where(c => c.CourseName.Contains(courseName, StringComparison.OrdinalIgnoreCase)).ToList();
            Course = new ObservableCollection<Courses>(filteredCourses);
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
