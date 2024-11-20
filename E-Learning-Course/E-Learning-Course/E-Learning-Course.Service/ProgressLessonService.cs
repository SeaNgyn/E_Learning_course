using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class ProgressLessonService : IProgressLessonService
    {
        private readonly RepositoryContext _context;
        public ProgressLessonService(RepositoryContext context)
        {
            _context = context;
        }
        public bool EnrollLesson(LessonProgress progressLesson)
        {
            _context.Add(progressLesson);
            _context.SaveChanges();
            return true;
        }

        public LessonProgress GetPLHaveEnroll(string userId, int lessonId)
        {
            return _context.LessonProgresses.FirstOrDefault(x => x.LessonId == lessonId && x.UserId == userId);
        }

        public bool UpdateProgress(LessonProgress progressLesson)
        {
            LessonProgress pr = _context.LessonProgresses.FirstOrDefault(x => x.Id == progressLesson.Id);
            if (pr != null)
            {
                pr.ProgressPercentage = progressLesson.ProgressPercentage;
                pr.Passing = progressLesson.Passing;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
