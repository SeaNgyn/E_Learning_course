using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class LessonService : ILessonService
    {
        private readonly RepositoryContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFileService _fileService;
        private readonly UserManager<User> _userManager;
        public LessonService(RepositoryContext context, IFileService fileService, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _fileService = fileService;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }
        public async Task<int> AddOrUpdateVideoLessonAsync(LessonDTO lessonDto, IFormFile videoFile)
        {
            var user = _contextAccessor.HttpContext.User;
            var userLogin = await _userManager.FindByNameAsync(user.Identity.Name);
            var lessonEntity = await _context.Lessons.FirstOrDefaultAsync(x => x.Id == lessonDto.Id) ?? new Lesson();
            var video = await _fileService.UploadAsync(videoFile);
                if (videoFile != null)
                {
                    lessonEntity.VideoUrl = video.Blob.Uri.ToString();
                    lessonEntity.Duration = (float)lessonDto.Duration;
                }

                if (lessonDto.Id == 0)
                {
                    lessonEntity.Name = lessonDto.Name;
                    lessonEntity.Content = lessonDto.Content;
                    lessonEntity.ChapterId = lessonDto.ChapterId;
                    lessonEntity.CreatedAt = DateTime.Now;
                    lessonEntity.Status = "Active";
                    lessonEntity.Type = "Video";
                    lessonEntity.CreateBy = userLogin.Id; 
                    _context.Lessons.Add(lessonEntity);
                }
                else
                {
                    lessonEntity.Name = lessonDto.Name;
                    lessonEntity.Content = lessonDto.Content;
                    lessonEntity.UpdatedAt = DateTime.Now;
                    lessonEntity.UpdateBy = userLogin.Id;
                    _context.Lessons.Update(lessonEntity);
                }

                await _context.SaveChangesAsync();
                return lessonEntity.Id;
        }

        public async Task<LessonDTO> GetCurrentLesson(int id)
        {
            var lesson = await _context.Lessons
                .Where(x => x.Id == id)
                .Select(x => new LessonDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    Status = x.Status,
                    Duration = (double)x.Duration,
                    VideoUrl = x.VideoUrl,
                    Questions = x.Type == "Quizz"
                        ? _context.Questions.Where(a=>a.LessonId == x.Id).Select(q => new QuestionDTO
                        {
                            Id = q.Id,
                            QuestionText = q.QuestionText,
                            Answers = _context.Answers.Where(b=>b.QuestionId == q.Id).Select(o => new AnswerDTO
                            {
                                Id = o.Id,
                                AnswerText = o.AnswerText
                            }).ToList()
                        }).ToList()
                        : null
                })
                .FirstOrDefaultAsync();

            return lesson;
        }
        public async Task<int> AddOrUpdateQuizLessonAsync(LessonDTO lessonDto)
        {
            var user = _contextAccessor.HttpContext.User;
            var userLogin = await _userManager.FindByNameAsync(user.Identity.Name);
            Lesson lessonEntity = lessonDto.Id == 0 ? new Lesson() : await _context.Lessons.FirstOrDefaultAsync(x => x.Id == lessonDto.Id);

            if (lessonDto.Id == 0) // New Lesson
            {
                lessonEntity = new Lesson
                {
                    Name = lessonDto.Name,
                    Content = lessonDto.Content,
                    ChapterId = lessonDto.ChapterId,
                    CreatedAt = DateTime.Now,
                    Status = "Active",
                    Type = "Quizz",
                    CreateBy = userLogin.Id,
                    Duration = (float?)lessonDto.Duration,
                    Passing = (float?)lessonDto.Passing
                };
                _context.Lessons.Add(lessonEntity);
                await _context.SaveChangesAsync(); // Save lesson first to get the Id
            }
            else // Update Lesson
            {
                lessonEntity.Name = lessonDto.Name;
                lessonEntity.Content = lessonDto.Content;
                lessonEntity.UpdatedAt = DateTime.Now;
                lessonEntity.Duration = (float?)lessonDto.Duration;
                lessonEntity.Passing = (float?)lessonDto.Passing;
                lessonEntity.UpdateBy = userLogin.Id;
                _context.Lessons.Update(lessonEntity);
                await _context.SaveChangesAsync(); // Save updates to lesson
            }

            // Manage questions and answers
            foreach (var questionDto in lessonDto.Questions)
            {
                Question questionEntity = questionDto.Id == 0 ? new Question { LessonId = lessonEntity.Id } : await _context.Questions.FirstOrDefaultAsync(q => q.Id == questionDto.Id);

                if (questionDto.Id == 0)
                {
                    questionEntity.QuestionText = questionDto.QuestionText;
                    _context.Questions.Add(questionEntity);
                    await _context.SaveChangesAsync(); // Save each question
                }
                else
                {
                    questionEntity.QuestionText = questionDto.QuestionText;
                    _context.Questions.Update(questionEntity);
                }

                foreach (var answerDto in questionDto.Answers)
                {
                    if (answerDto.Id == 0)
                    {
                        _context.Answers.Add(new Answer { AnswerText = answerDto.AnswerText, IsCorrect = answerDto.IsCorrect, QuestionId = questionEntity.Id });
                    }
                    else
                    {
                        var answerEntity = await _context.Answers.FirstOrDefaultAsync(a => a.Id == answerDto.Id);
                        answerEntity.AnswerText = answerDto.AnswerText;
                        answerEntity.IsCorrect = answerDto.IsCorrect;
                        _context.Answers.Update(answerEntity);
                    }
                }
            }

            await _context.SaveChangesAsync(); // Final save for all changes
            return lessonEntity.Id;
        }
        public async Task<LessonDTO> GetLessonAsync(int lessonId, int chapterId)
        {
            if (lessonId == 0)
            {
                return new LessonDTO { ChapterId = chapterId };
            }

            var lesson = await _context.Lessons
                .Where(l => l.Id == lessonId)
                .Select(l => new LessonDTO
                {
                    Id = l.Id,
                    Name = l.Name,
                    Content = l.Content,
                    Type = l.Type,
                    ChapterId = l.ChapterId,
                    VideoUrl = l.Type == "Video" ? l.VideoUrl : null, // Set VideoUrl only if Type is Video
                    Duration = l.Type == "Video" ? (float)l.Duration : 0, // Set Duration only if Type is Video
                    Passing = l.Type == "Video" ? l.Passing : 0,
                    Questions = l.Type == "Quizz" ?_context.Questions.Where(q=>q.LessonId==l.Id).Select(q => new QuestionDTO
                    {
                        Id = q.Id,
                        QuestionText = q.QuestionText,
                        Answers = _context.Answers.Where(a=>a.QuestionId == q.Id).Select(a => new AnswerDTO
                        {
                            Id = a.Id,
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect,
                            QuestionId = a.QuestionId
                        }).ToList()
                    }).ToList() : null // Set Questions only if Type is Question
                }).FirstOrDefaultAsync();

            return lesson;
        }
        public async Task<int> SubmitQuizAsync(int lessonId, Dictionary<int, int> selectedAnswers)
        {
            var user = _contextAccessor.HttpContext.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            // Fetch correct answers for the lesson's questions
            var questions = await _context.Questions
                .Where(q => q.LessonId == lessonId)
                .ToListAsync();

            int correctAnswersCount = 0;
            foreach (var question in questions)
            {
                if (selectedAnswers.TryGetValue(question.Id, out int selectedAnswerId))
                {
                    var selectedAnswer = _context.Answers.FirstOrDefault(a => a.Id == selectedAnswerId);
                    if (selectedAnswer != null && selectedAnswer.IsCorrect)
                    {
                        correctAnswersCount++;
                    }
                }
            }

            var lesson = _context.Lessons.Where(x=>x.Id==lessonId).Select(x=>x.Passing).FirstOrDefault();
            var lessonProgress = _context.LessonProgresses.Where(x => x.LessonId == lessonId && x.UserId == userId).FirstOrDefault();
            // Update user's lesson progress
            if (lessonProgress == null)
            {
                lessonProgress = new LessonProgress
                {
                    LessonId = lessonId,
                    UserId = userId,
                    ProgressPercentage = correctAnswersCount,
                    TimeSpent = 0, // Add logic to track time if needed
                    Status = correctAnswersCount >= lesson ? "Đã hoàn thành" : "Chưa hoàn thành",
                    CreatedAt = DateTime.UtcNow
                };
                _context.LessonProgresses.Add(lessonProgress);
                await _context.SaveChangesAsync();
            }
            else
            {
                lessonProgress.ProgressPercentage = correctAnswersCount;
                lessonProgress.Status = correctAnswersCount >= lesson ? "Đã hoàn thành" : "Chưa hoàn thành";
                _context.LessonProgresses.Update(lessonProgress);
                await _context.SaveChangesAsync();
            }    


            return correctAnswersCount;
        }
        public Lesson GetLessonById(int id)
        {
            return _context.Lessons.FirstOrDefault(x => x.Id == id);
        }
    }
}
