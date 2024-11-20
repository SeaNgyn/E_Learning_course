using E_Learning_Course.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Learning_Course.ViewModels;


namespace E_Learning_Course.Service
{
    public interface ICommentService
    {
        Task<List<CommentList>> GetAllCommentsAsync();
        Task<List<CommentList>> GetCommentsByLessonIdAsync(int lessonId);
        Task<bool> AddCommentAsync(CommentForCreation comment); 
        Task<bool> DeleteCommentAsync(int Id);
        Task<bool> EditCommentAsync(int Id, string newConten);
        Task<List<CommentList>> SearchCommentsAsync(string content = null, string userId = null, bool? isDeleted = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
