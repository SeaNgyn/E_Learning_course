using E_Learning_Course.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class AnswerService : IAnswerService
    {
        private readonly RepositoryContext _context;
        public AnswerService(RepositoryContext context)
        {
            _context = context;
        }

        public bool DeleteAnswerById(int answerId)
        {
            try
            {
                var answer = _context.Answers.FirstOrDefault(x => x.Id == answerId);
                if (answer != null)
                {
                    _context.Answers.Remove(answer);
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
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public bool DeleteAnswersByQuestionId(int questionId)
        {
            try
            {
                var question = _context.Questions.FirstOrDefault(x => x.Id == questionId);
                if (question != null)
                {
                    var answers = _context.Answers.Where(x => x.QuestionId == questionId).ToList();
                    _context.Answers.RemoveRange(answers);
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
    }
}
