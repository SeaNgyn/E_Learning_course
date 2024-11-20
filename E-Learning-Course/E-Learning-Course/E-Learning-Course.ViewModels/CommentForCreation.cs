using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class CommentForCreation
    {
        public string Content { get; set; }

        public int LessonId { get; set; }

        public int? ParentCommentId { get; set; } 

    }
}
