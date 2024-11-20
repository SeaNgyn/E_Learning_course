using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface ICourseService
    {
        Task<List<CourseList>> GetProCourses();
        Task<bool> CreateCourseAsync(CourseForCreation course);
        Task<List<CourseList>> GetCourseListHomePageFree();
        Task<CourseDetail> GetCourseDetailHomePage(int courseId);
        Task<CoursePayment> GetCoursePayment(int courseId, string ip);
        Task<int> HasAccessToCourseAsync(int courseId);
        Task<bool> UpdateCourseAsync(CourseForUpdate course);
        Task<string> ChangeCourseStatusAsync(int courseId, bool reviewSucess);
        Task<List<CourseList>> GetCourseTrend();
    }
}
