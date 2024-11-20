using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Learning_Course.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonProgressController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public LessonProgressController(RepositoryContext context)
        {
            _context = context;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveProgress([FromBody] LearnProgressViewModel progress)
        {
            var lessonProgress = await _context.LessonProgresses
                .Where(lp => lp.LessonId == progress.LessonId && lp.UserId == progress.UserId)
                .FirstOrDefaultAsync();
            var lesson = await _context.Lessons.Where(x=>x.Id == progress.LessonId).FirstOrDefaultAsync();

            if (lessonProgress == null)
            {
                lessonProgress = new LessonProgress
                {
                    UserId = progress.UserId,
                    LessonId = progress.LessonId,
                    ProgressPercentage = (float)progress.TimeSpent,
                    CreatedAt = DateTime.Now,
                    TimeSpent = 0,
                    Status = "Chưa hoàn thành"
                };
                _context.LessonProgresses.Add(lessonProgress);
            }
            else
            {
                lessonProgress.ProgressPercentage = (float)progress.TimeSpent;
                lessonProgress.UpdatedAt = DateTime.Now;
            }
            

            await _context.SaveChangesAsync();
            if(progress.TimeSpent > lesson.Passing)
            {
                lessonProgress.Status = "Đã hoàn thành";
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
