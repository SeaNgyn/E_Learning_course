
﻿using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
﻿using Azure.Storage.Blobs;
using E_Learning_Course.Data;
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
    public class CourseService : ICourseService
    {
        private readonly RepositoryContext _context;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IPaymentService _paymentService;
        public CourseService(RepositoryContext context, IFileService fileService, IHttpContextAccessor httpContextAccessor,UserManager<User> userManager, IPaymentService paymentService)
        {
            _context = context;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _paymentService = paymentService;
        }
        public async Task<List<CourseList>> GetProCourses()
        {
            var listCourse = await _context.Courses.Where(x=>x.Status == "Published")
                .Select(x => new CourseList
                {
                    Id = x.Id,
                    Title = x.Title,
                    Thumbnail = x.Thumbnail,
                    Price = x.Price,
                    Category = x.Category.Name,
                    Enrollments = _context.Enrollments.Where(y => y.CourseId == x.Id).Count(),
                    Lessons = _context.Lessons.Where(l => l.Chapter.CourseId == x.Id).Count(), // Count lessons
                    Durations = _context.Lessons
                              .Where(l => l.Chapter.CourseId == x.Id && l.Duration.HasValue)
                              .Sum(l => l.Duration.Value)
                }).Where(x => x.Price != 0).Take(8)
                .ToListAsync();

            return listCourse;
        }
        public async Task<List<CourseList>> GetCourseListHomePageFree()
        {
            var listCourse = await _context.Courses.Where(x => x.Status == "Published")
                .Select(x => new CourseList
                {
                    Id = x.Id,
                    Title = x.Title,
                    Thumbnail = x.Thumbnail,
                    Price = x.Price,
                    Category = x.Category.Name,
                    Enrollments = _context.Enrollments.Where(y => y.CourseId == x.Id).Count(),
                    Lessons = _context.Lessons.Where(l => l.Chapter.CourseId == x.Id).Count(), // Count lessons
                    Durations = _context.Lessons
                              .Where(l => l.Chapter.CourseId == x.Id && l.Duration.HasValue)
                              .Sum(l => l.Duration.Value)
                }).Where(x => x.Price == 0).Take(8)
                .ToListAsync();

            return listCourse;
        }
        public async Task<List<CourseList>> GetCourseTrend()
        {
            var listCourse = await _context.Courses
                .Where(x => x.Status == "Published")
                .Select(x => new CourseList
                {
                    Id = x.Id,
                    Title = x.Title,
                    Thumbnail = x.Thumbnail,
                    Price = x.Price,
                    Category = x.Category.Name,
                    Enrollments = _context.Enrollments.Count(y => y.CourseId == x.Id), // Count enrollments
                    Lessons = _context.Lessons.Count(l => l.Chapter.CourseId == x.Id), // Count lessons
                    Durations = _context.Lessons
                        .Where(l => l.Chapter.CourseId == x.Id && l.Duration.HasValue)
                        .Sum(l => l.Duration.Value)
                })
                .OrderByDescending(x => x.Enrollments) // Order by enrollments in descending order
                .Take(4) // Take the top 4
                .ToListAsync();

            return listCourse;
        }


        public async Task<bool> CreateCourseAsync(CourseForCreation course)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userLogin = await _userManager.FindByNameAsync(user.Identity.Name);
            var thumbnail = await _fileService.UploadAsync(course.Thumbnail);
            var previewVideo = await _fileService.UploadAsync(course.PreviewVideo);

            await _context.AddAsync(new Course()
            {
                Title = course.Title,
                CategoryId = course.CategoryId,
                Price = course.Price,
                Status = "Unpublished",
                Description = course.Description,
                Thumbnail = thumbnail.Blob.Uri.ToString(),
                PreviewVideo = previewVideo.Blob.Uri.ToString(),
                CreateBy = userLogin.Id.ToString(),
                CreatedAt = DateTime.UtcNow,
                Creator = userLogin,
                Updater = userLogin,
                LimitDay = course.LimitDay
            });

            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<string> ChangeCourseStatusAsync(int courseId, bool reviewSucess)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return "Course not found.";
            }

            // Change course status based on current status
            switch (course.Status)
            {
                case "Unpublished":
                    course.Status = "InReview";
                    await _context.SaveChangesAsync();
                    return "Course has been sent for review.";
                case "Editing":
                    course.Status = "InReview";
                    await _context.SaveChangesAsync();
                    return "Course has been sent for review.";

                case "InReview":
                    if (reviewSucess)
                    {
                        course.Status = "Published";
                        await _context.SaveChangesAsync();
                        return "Course has been published.";
                    }
                    else
                    {
                        course.Status = "Editing";
                        await _context.SaveChangesAsync();
                        return "Course has been published.";
                    }    

                case "Published":
                    course.Status = "Editing";
                    await _context.SaveChangesAsync();
                    return "Course has been editting.";
            }

            return "Status change failed.";
        }
        public async Task<CourseDetail> GetCourseDetailHomePage(int courseId)
        {
            var courseDetail = await _context.Courses
         .Where(x => x.Id == courseId)
         .Select(x => new CourseDetail
         {
             Id = x.Id,
             Title = x.Title,
             Thumbnail = x.Thumbnail,
             PreviewVideo = x.PreviewVideo,
             Description = x.Description,
             Price = x.Price,
             CategoryId = x.CategoryId,
             Lessons = _context.Lessons.Where(l => l.Chapter.CourseId == x.Id).Count(),
             Durations = _context.Lessons
                              .Where(l => l.Chapter.CourseId == x.Id && l.Duration.HasValue)
                              .Sum(l => l.Duration.Value),
             Enrollments = _context.Enrollments.Where(y => y.CourseId == x.Id).Count(),
             CategoryName = x.Category.Name,
             Status = x.Status
         })
         .FirstOrDefaultAsync();

            return courseDetail;
        }


        public async Task<CoursePayment> GetCoursePayment(int courseId, string ip)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userLogin = await _userManager.GetUserAsync(user); // Fetch user
            var userId = userLogin.Id; // Retrieve the Id of the logged-in user

            var enrollmentStatus = await _context.Enrollments
                .Where(x => x.CourseId == courseId && x.UserId == userId)
                .Select(x => x.Status)
                .FirstOrDefaultAsync();

            bool shouldReturnBack = enrollmentStatus == 2;

            var courseDetail = await _context.Courses
                .Where(x => x.Id == courseId)
                .Select(x => new CoursePayment
                {
                    Title = x.Title,
                    Thumbnail = x.Thumbnail,
                    Price = shouldReturnBack ? x.Price / 2 : x.Price,
                    Description = x.Description,
                    Url = _paymentService.CreatePaymentUrl(new CourseTransaction
                    {
                        CourseId = courseId,
                        CustomerId = userId,
                        Price = x.Price,
                        Ip = ip,
                        ReturnBack = shouldReturnBack
                    }),
                    Lessons = _context.Lessons.Where(l => l.Chapter.CourseId == x.Id).Count(),
                    Durations = _context.Lessons
                        .Where(l => l.Chapter.CourseId == x.Id && l.Duration.HasValue)
                        .Sum(l => l.Duration.Value)
                })
                .FirstOrDefaultAsync();

            return courseDetail;
        }
        public async Task<int> HasAccessToCourseAsync(int courseId)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userLogin = await _userManager.GetUserAsync(user); // Fetch user
            var userId = userLogin.Id;

            return await _context.Enrollments
            .Where(e => e.UserId == userId && e.CourseId == courseId).Select(x=>x.Status).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCourseAsync(CourseForUpdate course)
        {
            var courseToUpdate = await _context.Courses.FirstOrDefaultAsync(x => x.Id == course.Id);
            if (courseToUpdate == null)
            {
                return false;
            }

            var user = _httpContextAccessor.HttpContext.User;
            var userLogin = await _userManager.FindByNameAsync(user.Identity.Name);

            if (course.Thumbnail != null)
            {
                var thumbnail = await _fileService.UploadAsync(course.Thumbnail);
                courseToUpdate.Thumbnail = thumbnail.Blob.Uri.ToString();
            }

            if (course.PreviewVideo != null)
            {
                var previewVideo = await _fileService.UploadAsync(course.PreviewVideo);
                courseToUpdate.PreviewVideo = previewVideo.Blob.Uri.ToString();
            }

            courseToUpdate.Title = course.Title;
            courseToUpdate.CategoryId = course.CategoryId;
            courseToUpdate.Price = course.Price;
            courseToUpdate.Description = course.Description;
            courseToUpdate.UpdatedAt = DateTime.UtcNow; 
            courseToUpdate.Updater = userLogin;

            _context.Attach(courseToUpdate).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
