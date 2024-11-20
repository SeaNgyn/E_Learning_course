using E_Learning_Course.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IChapterService
    {
        Task<List<ChapterDTO>> GetChaptersByCourseIdAsync(int courseId);
        Task UpdateChaptersAsync(List<ChapterDTO> chapters);
        Task AddChaptersAsync(List<ChapterDTO> newChapters, int courseId);
        Task<List<ChapterDTO>> GetChaptersByLearningCourse(int courseId, string userId);
        bool DeleteChapterById(int chapterId);
    }
}
