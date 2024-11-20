using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class AnswerDTO
    {
        public int? Id { get; set; }

        public int? QuestionId { get; set; }

        public string AnswerText { get; set; }

        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}
