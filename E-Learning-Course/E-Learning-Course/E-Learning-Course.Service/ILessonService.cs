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
    public interface ILessonService
    {
        Task<int> SubmitQuizAsync(int lessonId, Dictionary<int, int> selectedAnswers);
        Task<LessonDTO> GetLessonAsync(int lessonId, int chapterId);
        Task<LessonDTO> GetCurrentLesson(int id);
        Task<int> AddOrUpdateVideoLessonAsync(LessonDTO lessonDto, IFormFile videoFile);
        Task<int> AddOrUpdateQuizLessonAsync(LessonDTO lessonDto);
        Lesson GetLessonById(int id);
    }
}
