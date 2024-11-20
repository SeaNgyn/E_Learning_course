using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly RepositoryContext _context;
        public QuestionService(RepositoryContext context)
        {
            _context = context;
        }
        public bool DeleteQuestionById(int questionId)
        {
            try
            {
                var question = _context.Questions.FirstOrDefault(x => x.Id == questionId);
                if (question != null)
                {
                    var answers = _context.Answers.Where(x => x.QuestionId == questionId).ToList();
                    _context.Answers.RemoveRange(answers);
                    _context.Questions.Remove(question);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<Question> GetQuestionsByLessionId(int lessionId)
        {
            return _context.Questions
                    .Where(q => q.LessonId == lessionId)
                    .ToList();
        }

    }
}
