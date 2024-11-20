using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class ChapterService : IChapterService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly RepositoryContext _context;
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;
        public ChapterService(RepositoryContext context,IHttpContextAccessor httpContextAccessor,UserManager<User> userManager,IAnswerService answerService,IQuestionService questionService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _answerService = answerService;
            _questionService = questionService;
        }
        public async Task<List<ChapterDTO>> GetChaptersByCourseIdAsync(int courseId)
        {
            var chapters = await _context.Chapters
                .Where(x => x.CourseId == courseId)
                .ToListAsync(); // Fetch chapters first

            var chapterDTOs = new List<ChapterDTO>();

            foreach (var chapter in chapters)
            {
                var duration = await _context.Lessons
                    .Where(l => l.ChapterId == chapter.Id)
                    .SumAsync(l => l.Duration); 

                var lessons = await _context.Lessons
                    .Where(l => l.ChapterId == chapter.Id)
                    .Select(lesson => new LessonDTO
                    {
                        Id = lesson.Id,
                        Name = lesson.Name,
                        Type = lesson.Type,
                        Status = lesson.Status,
                        Duration = (double)lesson.Duration
                    })
                    .ToListAsync(); // Fetch lessons asynchronously

                chapterDTOs.Add(new ChapterDTO
                {
                    Id = chapter.Id,
                    CourseId = chapter.CourseId,
                    Name = chapter.Name,
                    Status = chapter.Status,
                    Duration = duration,
                    Lessons = lessons
                });
            }

            return chapterDTOs;
        }
        public async Task<List<ChapterDTO>> GetChaptersByLearningCourse(int courseId,string userId)
        {
            var chapters = await _context.Chapters
                .Where(x => x.CourseId == courseId)
                .ToListAsync(); // Fetch chapters first

            var chapterDTOs = new List<ChapterDTO>();

            foreach (var chapter in chapters)
            {
                var duration = await _context.Lessons
                    .Where(l => l.ChapterId == chapter.Id)
                    .SumAsync(l => l.Duration);

                var lessons = await _context.Lessons
                    .Where(l => l.ChapterId == chapter.Id)
                    .Select(lesson => new LessonDTO
                    {
                        Id = lesson.Id,
                        Name = lesson.Name,
                        Type = lesson.Type,
                        Status = lesson.Status,
                        Duration = (double)lesson.Duration,
                        StatusProgress = _context.LessonProgresses.Where(le => le.LessonId == lesson.Id && le.UserId == userId).Select(x => x.Status).FirstOrDefault() ?? "Chưa hoàn thành"
                    })
                    .ToListAsync(); // Fetch lessons asynchronously

                chapterDTOs.Add(new ChapterDTO
                {
                    Id = chapter.Id,
                    CourseId = chapter.CourseId,
                    Name = chapter.Name,
                    Status = chapter.Status,
                    Duration = duration,
                    Lessons = lessons
                });
            }

            return chapterDTOs;
        }

        public async Task UpdateChaptersAsync(List<ChapterDTO> chapters)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userLogin = await _userManager.GetUserAsync(user); // Fetch user
            var userId = userLogin.Id; // Retrieve the Id of the logged-in user
            foreach (var chapter in chapters)
            {
                var oldChapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == chapter.Id);
                if (oldChapter != null)
                {
                    oldChapter.Name = chapter.Name;
                    oldChapter.UpdatedAt = DateTime.Now;
                    oldChapter.UpdateBy = userId;
                }
                _context.Chapters.Update(oldChapter);
            }

            await _context.SaveChangesAsync();
        }
        public async Task AddChaptersAsync(List<ChapterDTO> newChapters,int courseId)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userLogin = await _userManager.GetUserAsync(user); // Fetch user
            var userId = userLogin.Id;
            foreach (var newChapter in newChapters)
            {
                if (!string.IsNullOrEmpty(newChapter.Name))
                {
                    var chapter = new Chapter
                    {
                        CourseId = courseId,
                        Name = newChapter.Name,
                        Status = "Active",
                        CreateBy = userId,
                        CreatedAt = DateTime.Now
                    };
                    _context.Chapters.Add(chapter);
                }
            }
            await _context.SaveChangesAsync();
        }
        public bool DeleteChapterById(int chapterId)
        {
            try
            {
                var chapter = _context.Chapters.FirstOrDefault(x => x.Id == chapterId);
                if (chapter != null)
                {
                    var lessons = _context.Lessons.Where(x => x.ChapterId == chapterId).ToList();
                    if (lessons.Count > 0)
                    {
                        foreach (var lesson in lessons)
                        {
                            if (lesson.Type.Equals("Video"))
                            {
                                _context.Lessons.Remove(lesson);
                            }
                            else
                            {
                                var quessions = _context.Questions.Where(x => x.LessonId == lesson.Id).ToList();
                                foreach (var quession in quessions)
                                {
                                    _answerService.DeleteAnswersByQuestionId(quession.Id);
                                    _questionService.DeleteQuestionById(quession.Id);
                                }
                            }
                        }
                    }
                    _context.Chapters.Remove(chapter);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
