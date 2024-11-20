using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class CommentService : ICommentService
    {
        private readonly RepositoryContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public CommentService(RepositoryContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }


        public async Task<List<CommentList>> GetAllCommentsAsync()
{
    return await _context.Comments
        .Where(c => !c.IsDelete)
        .Join(_context.Users,
            comment => comment.UserId,
            user => user.Id,
            (comment, user) => new CommentList
            {
                Id = comment.Id,
                LessonId = comment.LessonId,
                UserId = comment.UserId,
                UserName = user.UserName,
                ParentCommentId = comment.ParentCommentId ?? 0,
                Content = comment.Content ?? string.Empty,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                IsDelete = comment.IsDelete
            })
        .ToListAsync();
}


        public async Task<List<CommentList>> GetCommentsByLessonIdAsync(int lessonId)
        { 
             return await _context.Comments
             .Where (c => c.LessonId == lessonId && !c.IsDelete)
             .Join (_context.Users, 
            comment => comment.UserId, 
            user => user.Id, 
            (comment, user) => new CommentList 
            {
                Id = comment.Id,
                LessonId = comment.LessonId,
                UserId = comment.UserId,
                UserName = user.UserName,
                ParentCommentId = comment.ParentCommentId ?? 0,
                Content = comment.Content ?? string.Empty,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                IsDelete = comment.IsDelete
            })
        .ToListAsync ();    
        }


        public async Task<bool> AddCommentAsync(CommentForCreation comment)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userLogin = await _userManager.GetUserAsync (user); // Lấy người dùng hiện tại
            var lesson = await _context.Lessons.FindAsync (comment.LessonId);
            if ( lesson == null ) {
                return false; // Lesson doesn't exist, return false or handle appropriately
            }

            var newComment = new Comment
            {
                LessonId = comment.LessonId,
                UserId = userLogin.Id,
                ParentCommentId = comment.ParentCommentId,
                Content = comment.Content,
                CreatedAt = DateTime.Now,
                IsDelete = false
            };

            await _context.Comments.AddAsync (newComment);
            await _context.SaveChangesAsync ();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync (id);
            if ( comment != null ) {
                // Đánh dấu bình luận cha là đã xóa
                comment.IsDelete = true;

                // Tìm tất cả các bình luận con và đánh dấu là đã xóa
                var childComments = _context.Comments.Where (c => c.ParentCommentId == id);
                foreach ( var childComment in childComments ) {
                    childComment.IsDelete = true;
                }

                await _context.SaveChangesAsync ();
                return true;
            }
            return false;
        }

        public async Task<bool> EditCommentAsync(int Id, string newContent)
        {

            var comment = await _context.Comments.FindAsync(Id);
            if ( comment == null || comment.IsDelete ) {
                return false; 
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue (ClaimTypes.NameIdentifier);
            if ( userId != comment.UserId ) return false;


            comment.Content = newContent;
            comment.UpdatedAt = DateTime.Now;

            _context.Comments.Update (comment);
            await _context.SaveChangesAsync ();

            return true; // Trả về true sau khi chỉnh sửa thành công
        }

        public async Task<List<CommentList>> SearchCommentsAsync(
    string content,
    string userId,
    bool? isDeleted = null,
    DateTime? fromDate = null,
    DateTime? toDate = null)
        {
            var query = _context.Comments.AsQueryable();

            if (!string.IsNullOrEmpty(content))
            {
                query = query.Where(c => c.Content.Contains(content));
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(c => c.UserId == userId);
            }

            // Check isDeleted condition; by default, fetch only comments that are not deleted
            if (isDeleted.HasValue)
            {
                query = query.Where(c => c.IsDelete != isDeleted.Value);
            }
            else
            {
                query = query.Where(c => !c.IsDelete);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= toDate.Value);
            }

            return await query
                .Join(
                    _context.Users,
                    comment => comment.UserId,
                    user => user.Id,
                    (comment, user) => new { comment, user })
                .Join(
                    _context.Lessons,
                    commentUser => commentUser.comment.LessonId,
                    lesson => lesson.Id,
                    (commentUser, lesson) => new { commentUser, lesson })
                .Select(result => new CommentList
                {
                    Id = result.commentUser.comment.Id,
                    LessonId = result.commentUser.comment.LessonId,
                    CourseId = result.lesson.Chapter.CourseId, // Retrieve CourseId from Lesson
                    UserId = result.commentUser.comment.UserId,
                    UserName = result.commentUser.user.UserName,
                    ParentCommentId = result.commentUser.comment.ParentCommentId ?? 0,
                    Content = result.commentUser.comment.Content ?? string.Empty,
                    CreatedAt = result.commentUser.comment.CreatedAt,
                    UpdatedAt = result.commentUser.comment.UpdatedAt,
                    IsDelete = result.commentUser.comment.IsDelete,
                    Avatar = result.commentUser.user.Avatar,
                })
                .ToListAsync();
        }

    }
}
