using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IAnswerService
    {
        bool DeleteAnswerById(int answerId);
        bool DeleteAnswersByQuestionId(int questionId);
    }
}
