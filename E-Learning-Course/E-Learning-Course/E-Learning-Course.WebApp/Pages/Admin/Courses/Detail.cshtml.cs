using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Learning_Course.WebApp.Pages.Admin.Courses
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class DetailModel : PageModel
    {
        private readonly IChapterService _chapterService;
        private readonly ICourseService _courseService;
        private readonly RepositoryContext _context;
        public CourseDetail CourseDetail { get; set; } 
        public bool HasAccess { get;set; }
        private readonly IEnrollmentService _enrollmentService;
        public int courseId { get; set; }
        public int AmountEnrollment { get; set; }

        public string CurrentFilter { get; set; } = "";

        [BindProperty]
        public List<ChapterDTO> Chapters { get; set; } = new List<ChapterDTO>();

        [BindProperty]
        public List<ChapterDTO> NewChapters { get; set; } = new List<ChapterDTO>();

        public List<LessonDTO> Lessons { get; set; } = new List<LessonDTO>();

        private readonly ILogger<DetailModel> _logger;

      
        public DetailModel(ILogger<DetailModel> logger, IChapterService chapterService,ICourseService courseService, IEnrollmentService enrollmentService,RepositoryContext context)
        {
            _logger = logger;
            _chapterService = chapterService;
            _courseService = courseService;
            _context = context;
            _enrollmentService = enrollmentService;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            HasAccess = _context.Courses.Where(x=>x.CreateBy == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Id == id).Any();
            CourseDetail = await _courseService.GetCourseDetailHomePage(id);
            if (CourseDetail == null)
            {
                return RedirectToAction("Error");
            }
            courseId = id;
            Chapters = await _chapterService.GetChaptersByCourseIdAsync(id);
            AmountEnrollment = _enrollmentService.AmountEnroll(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {

            if (ModelState.IsValid)
            {
                // Update existing chapters
                await _chapterService.UpdateChaptersAsync(Chapters);

                // Add new chapters
                await _chapterService.AddChaptersAsync(NewChapters,id);

                TempData["ToastMessage"] = "Thêm chương mới thành công";
                TempData["ToastType"] = "success";

                return RedirectToPage(new { id });
            }
            else
            {
                    var errors = ModelState
                    .ToDictionary(
                     kvp => kvp.Key,
                     kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                     );
                    TempData["ModelStateErrors"] = JsonSerializer.Serialize(errors);
                    TempData["ToastMessage"] = string.Join("<br>", errors);
                    TempData["ToastType"] = "fail";
                    return RedirectToPage("", new { id });
            }
        }

        public async Task<IActionResult> OnPostChangeStatus(int courseId, bool reviewSuccess)
        {
            string message = await _courseService.ChangeCourseStatusAsync(courseId, reviewSuccess);
            TempData["Message"] = message;

            return RedirectToPage();
        }

        public IActionResult OnGetDeleteChapter(int chapterId, int courseId)
        {
            bool isDelete = _chapterService.DeleteChapterById(chapterId);
            if (isDelete)
            {
                TempData["ToastMessage"] = "Đã xóa thành công chương.";
                TempData["ToastType"] = "success";
            }
            else
            {
                TempData["ToastMessage"] = "Xóa chương thất bại.";
                TempData["ToastType"] = "fail";
            }
            return RedirectToAction("", new { id = courseId});
        }
    }
}
