using E_Learning_Course.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IQuestionService
    {
        bool DeleteQuestionById(int questionId);
        List<Question> GetQuestionsByLessionId(int lessionId);
    }
}
