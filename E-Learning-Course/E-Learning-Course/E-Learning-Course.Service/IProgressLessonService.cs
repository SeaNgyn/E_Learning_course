using E_Learning_Course.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IProgressLessonService
    {
        bool EnrollLesson(LessonProgress progressLesson);
        LessonProgress GetPLHaveEnroll(string userId, int lessonId);
        bool UpdateProgress(LessonProgress progressLesson);
    }
}
