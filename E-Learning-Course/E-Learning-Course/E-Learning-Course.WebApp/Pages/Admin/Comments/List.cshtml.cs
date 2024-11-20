using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using X.PagedList;
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Learning_Course.WebApp.Pages.Admin.Comments
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class ListModel : PageModel
    {
        private readonly ICommentService _commentService;
        private readonly RepositoryContext _context;

        public ListModel(ICommentService commentService, RepositoryContext context)
        {
            _commentService = commentService;
            _context = context;
        }


        [BindProperty]
        public int LessonId { get; set; }

        [BindProperty]
        public string CommentContent { get; set; }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public int CourseId { get; set; }

        [BindProperty]
        public string EditedCommentContent { get; set; }

        public List<CommentList> Comments { get; set; }

        public IPagedList<CommentList> PagedComment { get; set; }

        public string CurrentFilter { get; set; } = "";
        public string CurrentSort { get; set; }
        public int? PageSize { get; set; }
        public int? PageNo { get; set; }

        public int? TotalPage { get; set; }

        public async Task OnGetAsync(int lessonId, int? pageNo, int? pageSize, DateTime? fromDate, DateTime? toDate, string content = null, string userId = null, bool? isDeleted = null)
        {
            
            LessonId = lessonId;
            pageNo ??= 1;
            pageSize ??= 5;

            var commentQuery = await _commentService.SearchCommentsAsync(
                content,
                userId,
                isDeleted,
                fromDate,
                toDate
            );

            PagedComment = commentQuery.ToPagedList((int)pageNo, (int)pageSize);

            int totalItems = commentQuery.Count();
            int morePage = totalItems % pageSize != 0 ? 1 : 0;
            TotalPage = ( totalItems / pageSize ) + morePage;

            PageNo = pageNo;
            PageSize = pageSize;

        }

        public async Task<IActionResult> OnPostAddReplyCommentAsync(CommentList newComment , int lessonId)
        {

            var lessonExists = await _context.Lessons.AnyAsync (l => l.Id == lessonId);
            if ( string.IsNullOrWhiteSpace (CommentContent) ) {
                ModelState.AddModelError ("CommentContent", "Bình luận không được để trống.");
                return Page ();
            }

            //var cleanContent = ProcessedCommentContent (CommentContent);

            var commentForCreation = new CommentForCreation
            {
                LessonId = lessonId,
                ParentCommentId = newComment.ParentCommentId,
                Content = CommentContent
            };

            await _commentService.AddCommentAsync (commentForCreation);
            Comments = await _commentService.GetCommentsByLessonIdAsync(lessonId);

            return RedirectToPage ("/Admin/Comments/List");
        }

        public async Task<IActionResult> OnPostEditCommentAsync(int Id, string EditedCommentContent, int lessonId)
        {
            var success = await _commentService.EditCommentAsync (Id, EditedCommentContent);
            if ( !success ) {
                ModelState.AddModelError ("EditedCommentContent", "Không thể chỉnh sửa bình luận hoặc bình luận không tồn tại.");
                return Page ();
            }

            if ( string.IsNullOrWhiteSpace (EditedCommentContent) ) {
                ModelState.AddModelError ("EditedCommentContent", "Nội dung bình luận không được để trống.");
                return Page ();
            }

            return RedirectToPage ("/Admin/Comments/List");

        }

        public async Task<IActionResult> OnPostDeleteCommentAsync(int Id)
        {
            var success = await _commentService.DeleteCommentAsync (Id);

            if ( !success ) {
                ModelState.AddModelError ("DeleteComment", "Không thể xóa bình luận.");
            }

            // Sau khi xóa, chuyển hướng lại trang học
            return RedirectToPage ("/Admin/Comments/List");
        }
    }
}
