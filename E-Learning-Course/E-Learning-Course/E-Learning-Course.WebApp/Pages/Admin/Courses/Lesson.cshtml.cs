using Azure.Storage.Blobs;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

namespace E_Learning_Course.WebApp.Pages.Admin.Courses
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class LessonModel : PageModel
    {
        public string CurrentFilter { get; set; } = "";

        private readonly ILessonService _lessonService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;

        public LessonModel(ILessonService lessonService,IQuestionService questionService, IAnswerService answerService)
        {
            _lessonService = lessonService;
            _questionService = questionService;
            _answerService = answerService;
        }

        [BindProperty]

        public IFormFile? VideoUrl { get; set; }


        [BindProperty]
        public LessonDTO? Lesson { get; set; }


        public async Task<IActionResult> OnGetAsync(int lessonId = 0, int chapterId = 0)
        {
            if (TempData.ContainsKey("ModelStateErrors"))
            {
                var errorsJson = TempData["ModelStateErrors"].ToString();
                var errors = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(errorsJson);

                if (errors != null)
                {
                    foreach (var error in errors)
                    {
                        foreach (var errorMessage in error.Value)
                        {
                            ModelState.AddModelError(error.Key, errorMessage);
                        }
                    }
                }
            }
            Lesson = await _lessonService.GetLessonAsync(lessonId,chapterId);
            return Page();
        }

        public IActionResult OnGetDirect()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAddQuizLessonAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
               .ToDictionary(
                   kvp => kvp.Key,
                   kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
               );
                TempData["ModelStateErrors"] = JsonSerializer.Serialize(errors);

                return RedirectToPage(new { lessonId = Lesson.Id, chapterId = Lesson.ChapterId });
            }

            await _lessonService.AddOrUpdateQuizLessonAsync(Lesson);
            TempData["ToastMessage"] = "Thêm bài học thành công.";
            TempData["ToastType"] = "success";
            return RedirectToPage(new { lessonId = Lesson.Id });
        }
        public async Task<IActionResult> OnPostAddVideoLessonAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _lessonService.AddOrUpdateVideoLessonAsync(Lesson, VideoUrl);
            TempData["ToastMessage"] = "Thêm bài học thành công.";
            TempData["ToastType"] = "success";
            return RedirectToPage(new { lessonId = Lesson.Id });
        }
        public IActionResult OnGetDeleteAnswer(int answerId, int lessonId)
        {
            bool isDelete = _answerService.DeleteAnswerById(answerId);
            if (isDelete)
            {
                TempData["ToastMessage"] = "Xóa câu trả lời thành công";
                TempData["ToastType"] = "success";
            }
            else
            {
                TempData["ToastMessage"] = "Answer not found.";
                TempData["ToastType"] = "fail";
            }
            return RedirectToAction("", new { lessonId = lessonId });
        }

        //delete Quizz Question | delete Quizz Question
        public IActionResult OnGetDeleteQuestion(int questionId, int lessonId)
        {

            bool isDelete = _questionService.DeleteQuestionById(questionId);
            if (isDelete)
            {
                TempData["ToastMessage"] = "Xóa câu hỏi thành công.";
                TempData["ToastType"] = "success";
            }
            else
            {
                TempData["ToastMessage"] = "Question not found.";
                TempData["ToastType"] = "fail";
            }
            return RedirectToAction("", new { lessonId = lessonId });
        }
    }
}