using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class CourseDetailModel : PageModel
    {
        private readonly ICourseService _courseService;
        private readonly IChapterService _chapterService;
        public CourseDetailModel(ICourseService courseService,IChapterService chapterService)
        {
            _courseService = courseService;
            _chapterService = chapterService;
        }
        public CourseDetail? CourseDetail { get; set; }
        public int LessonId { get; set; }
        public int HasAccess { get; set; }
        public List<ChapterDTO>? Chapters { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            CourseDetail = await _courseService.GetCourseDetailHomePage(id);
            Chapters = await _chapterService.GetChaptersByCourseIdAsync(id);
            if (!User.Identity.IsAuthenticated)
            {
                HasAccess = 0;
            }    
            else if (CourseDetail != null)
            {
                if (CourseDetail.Price == 0)
                {
                    HasAccess = 1;
                }
                else
                {
                    HasAccess = await _courseService.HasAccessToCourseAsync(id);
                }
            }
            if (HasAccess == 1)
            {
                LessonId = (int)(HttpContext.Session.GetInt32("CurrentLessonId") ??
           (Chapters?.FirstOrDefault()?.Lessons?.FirstOrDefault()?.Id ?? 0));
            }
            return Page();
        }

    }
}
