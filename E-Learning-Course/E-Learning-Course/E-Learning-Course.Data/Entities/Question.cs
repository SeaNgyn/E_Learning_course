using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Data.Entities
{
    public class Question
    {
        public int Id { get; set; }

        public int LessonId { get; set; }

        public string QuestionText { get; set; }

        public Lesson Lesson { get; set; }
    }
}
