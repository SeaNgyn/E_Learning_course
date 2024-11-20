using Microsoft.AspNetCore.Mvc;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using E_Learning_Course.Data;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.EntityFrameworkCore;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class LearningCourseModel : PageModel
    {
        public string? Mess { get; set; } = "";
        private readonly IHubContext<CommentHub> _hubContext;
        private readonly IProgressLessonService progressLessonService;

        private readonly IQuestionService questionService;
        [BindProperty]
        public string CurrentUserId { get; set; }
        private readonly ILessonService lessonService;

        public Course Course { get; set; }

        public Lesson CurrentLesson { get; set; }

        [BindProperty]
        public List<QuestionDTO>? Questions { get; set; }

        [BindProperty]
        public List<ChapterDTO> Chapters { get; set; } = new List<ChapterDTO>();
        public List<CommentList> Comments { get; set; } = new List<CommentList>();
        [BindProperty]
        public int CurrentLessonId { get; set; }
        [BindProperty]
        public int LessonId { get; set; }
        [BindProperty]
        public int CourseId { get; set; }
        private readonly ICommentService _commentService;

        [BindProperty]
        public double Score { get; set; }
        [BindProperty]
        public string CommentContent { get; set; }

        [BindProperty]
        public LessonProgress ProgressLesson { get; set; }
        public int CommentCount => Comments.Count;
        public List<LessonProgress> ListLearning { get; set; }

        [BindProperty]
        public bool IsDoingQuizz { get; set; } = false;
        private readonly UserManager<User> _userManager;
        private readonly ICourseService _courseService;
        private readonly IPaymentService _paymentService;
        private readonly RepositoryContext _context;

        public LearningCourseModel(IProgressLessonService progressLessonService, IQuestionService questionService, ILessonService lessonService, ICommentService commentService, UserManager<User> userManager, ICourseService courseService, IPaymentService paymentService, IHubContext<CommentHub> hubContext, RepositoryContext context)
        {
            this.progressLessonService = progressLessonService;
            this.questionService = questionService;
            this.lessonService = lessonService;
            _commentService = commentService;
            _userManager = userManager;
            _courseService = courseService;
            _paymentService = paymentService;
            _hubContext = hubContext;
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync(int courseId, int lessonId, string quizz = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (await _courseService.HasAccessToCourseAsync(courseId) == 0)
                {
                    await _paymentService.GrantCourseAccessToUser(user.Id, courseId);
                }
                CurrentUserId = user.Id;
            }
            LessonId = lessonId;
            CourseId = courseId;
            HttpContext.Session.SetInt32("CurrentLessonId", LessonId);
            Comments = await _commentService.GetCommentsByLessonIdAsync(lessonId);
            System.Diagnostics.Debug.WriteLine("courseId = " + courseId);
            System.Diagnostics.Debug.WriteLine("lessonId = " + lessonId);
            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                Course = context.Courses.Include(X => X.Category).FirstOrDefault(X => X.Id == courseId);

                if (lessonId == 0)
                {
                    var chapter = context.Chapters.Where(X => X.CourseId == courseId).ToList();
                    CurrentLesson = context.Lessons.FirstOrDefault(X => X.ChapterId == chapter[0].Id);
                }
                else
                {
                    CurrentLesson = context.Lessons.FirstOrDefault(X => X.Id == lessonId);
                }

                lessonId = CurrentLesson.Id;


                if (Course == null)
                {
                    return RedirectToAction("Error");
                }
                CurrentLessonId = CurrentLesson.Id;
                CourseId = Course.Id;
                Chapters = context.Chapters.Where(X => X.CourseId == courseId)
                    .Select(chap => new ChapterDTO
                    {
                        Id = chap.Id,
                        CourseId = chap.CourseId,
                        Name = chap.Name,
                        Status = chap.Status,
                        Duration = context.Lessons
                                    .Where(l => l.ChapterId == chap.Id).Sum(x => x.Duration),
                        Lessons = context.Lessons.Where(l => l.ChapterId == chap.Id)
                        .Select(lesson => new LessonDTO
                        {
                            Id = lesson.Id,
                            Name = lesson.Name,
                            Type = lesson.Type,
                            Status = lesson.Status,
                            Duration = lesson.Duration,
                            IsPass = progressLessonService.GetPLHaveEnroll(User.FindFirstValue(ClaimTypes.NameIdentifier), lesson.Id) == null ? false : progressLessonService.GetPLHaveEnroll(User.FindFirstValue(ClaimTypes.NameIdentifier), lesson.Id).Passing == 1
                        }).ToList(),

                    }).ToList();

                ProgressLesson = progressLessonService.GetPLHaveEnroll(User.FindFirstValue(ClaimTypes.NameIdentifier), lessonId);
                //check video trc đó đã pass hay chưa mới dc acctive video tiếp theo

                if (CurrentLesson.Type.Equals("Video"))
                {
                    if (ProgressLesson == null)
                    {
                        ProgressLesson = new LessonProgress();
                        ProgressLesson.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        ProgressLesson.LessonId = lessonId;
                        ProgressLesson.ProgressPercentage = 0;
                        ProgressLesson.Passing = 0;
                        ProgressLesson.CountDoing = 0;
                        progressLessonService.EnrollLesson(ProgressLesson);
                    }
                }
                if (CurrentLesson.Type.Equals("Quizz"))
                {
                    //if (ProgressLesson == null)
                    //{
                    //    ProgressLesson = new ProgressLesson();
                    //    ProgressLesson.UserId = "4b73c662-9170-445c-8f5b-10b054c12616";
                    //    ProgressLesson.LessonId = lessonId;
                    //    ProgressLesson.Progress = 0;
                    //    ProgressLesson.Passing = 0;
                    //    progressLessonService.EnrollLesson(ProgressLesson);
                    //}

                    //th: start
                    if (ProgressLesson == null && quizz.Equals("start")) //start
                    {
                        ProgressLesson = new LessonProgress();
                        ProgressLesson.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        ProgressLesson.LessonId = lessonId;
                        ProgressLesson.ProgressPercentage = 0;
                        ProgressLesson.Passing = 0;
                        progressLessonService.EnrollLesson(ProgressLesson);

                        IsDoingQuizz = true;
                        Questions = context.Questions
                        .Where(q => q.LessonId == lessonId)
                        .Select(q => new QuestionDTO
                        {
                            Id = q.Id,
                            LessonId = lessonId,
                            QuestionText = q.QuestionText,
                            Answers = context.Answers
                            .Where(a => a.QuestionId == q.Id)
                            .Select(a => new AnswerDTO
                            {
                                Id = a.Id,
                                QuestionId = q.Id,
                                AnswerText = a.AnswerText,
                                IsCorrect = a.IsCorrect
                            }).ToList()
                        }).ToList();
                    }

                    if (ProgressLesson != null && quizz.Equals("resume")) // resume
                    {
                        if (!validateTime(ProgressLesson.UpdatedAt ?? ProgressLesson.CreatedAt, ProgressLesson.CountDoing ?? 1))
                        {
                            TempData["messQuizz"] = "Bạn không đủ điều kiện để làm bài quizz này";
                        }
                        else
                        {
                            ProgressLesson.CountDoing = ProgressLesson.CountDoing++;
                            progressLessonService.UpdateProgress(ProgressLesson);

                            IsDoingQuizz = true;
                            Questions = context.Questions
                            .Where(q => q.LessonId == lessonId)
                            .Select(q => new QuestionDTO
                            {
                                Id = q.Id,
                                LessonId = lessonId,
                                QuestionText = q.QuestionText,
                                Answers = context.Answers
                                .Where(a => a.QuestionId == q.Id)
                                .Select(a => new AnswerDTO
                                {
                                    Id = a.Id,
                                    QuestionId = q.Id,
                                    AnswerText = a.AnswerText,
                                    IsCorrect = a.IsCorrect
                                }).ToList()
                            }).ToList();
                        }
                    }
                    //xem diem 
                }
            }
            return Page();
        }
        private string GenerateSasToken(string blobUrl)
        {
            string accountName = "";
            string accountKey = "";

            // Tạo đối tượng StorageSharedKeyCredential
            var sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);

            // Tạo BlobClient với thông tin xác thực SharedKey
            var blobClient = new BlobClient(new Uri(blobUrl), sharedKeyCredential);

            // Kiểm tra nếu blob tồn tại và có quyền tạo SAS token
            if (!blobClient.CanGenerateSasUri)
            {
                throw new InvalidOperationException("BlobClient is not authorized to generate SAS token.");
            }

            // Thiết lập thời gian hết hạn 1 phút
            var sasUri = blobClient.GenerateSasUri(
                BlobSasPermissions.Read,
                DateTimeOffset.UtcNow.AddMinutes(1)
            );

            return sasUri.ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                double mark = 0;
                double max = 10;

                // Lấy danh sách các câu hỏi trong bài học từ cơ sở dữ liệu
                var questionList = questionService.GetQuestionsByLessionId(CurrentLessonId);

                foreach (var question in questionList)
                {
                    if (Request.Form.TryGetValue($"question-{question.Id}", out var selectedAnswerIdValue))
                    {
                        System.Diagnostics.Debug.WriteLine("selectedAnswerIdValue = " + selectedAnswerIdValue);

                        // Split multiple answer IDs (e.g., "4,5" -> ["4", "5"])
                        var answerIds = selectedAnswerIdValue.ToString().Split(',');

                        int correctAnswersCount = context.Answers.Count(a => a.QuestionId == question.Id && a.IsCorrect);

                        foreach (var answerIdString in answerIds)
                        {
                            // Convert each AnswerId from string to integer
                            if (int.TryParse(answerIdString, out int selectedAnswerId))
                            {
                                // Retrieve the selected answer from the database
                                var selectedAnswer = context.Answers
                                    .FirstOrDefault(a => a.Id == selectedAnswerId && a.QuestionId == question.Id);

                                if (selectedAnswer != null && selectedAnswer.IsCorrect)
                                {
                                    // If the question has only one correct answer, award the full mark for the question
                                    if (correctAnswersCount == 1)
                                    {
                                        mark += max / questionList.Count;
                                        break; // If there's only one correct answer, stop checking further
                                    }
                                    else
                                    {
                                        // If the question has multiple correct answers, distribute the score among them
                                        mark += (max / questionList.Count) / correctAnswersCount;
                                    }
                                }
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Score = " + mark);
                Score = mark;

                Lesson less = context.Lessons.FirstOrDefault(x => x.Id == CurrentLessonId);

                LessonProgress pl = progressLessonService.GetPLHaveEnroll(User.FindFirstValue(ClaimTypes.NameIdentifier), CurrentLessonId);
                pl.ProgressPercentage = (float)Score;
                pl.UpdatedAt = DateTime.Now;
                pl.CountDoing = pl.CountDoing++;
                if (pl.Passing == 0)
                {
                    pl.Passing = (Score >= less.Passing) ? 1 : 0;
                }

                progressLessonService.UpdateProgress(pl);
                await _context.SaveChangesAsync();
                if (pl.Passing == 1)
                {
                    var enrollment = _context.Enrollments.Where(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.CourseId == CourseId).FirstOrDefault();
                    if (enrollment != null)
                    {
                        int count = _context.Lessons.Where(x => x.Chapter.Course.Id == CourseId).Count();

                        // Calculate the total number of lessons in the course (from all chapters)
                        int countLesson = _context.LessonProgresses
                .Where(lp => lp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                     && lp.Passing == 1
                     && lp.Lesson.Chapter.CourseId == CourseId)
                .Count();

                        // Calculate the number of passing lessons in the course


                        // Update the enrollment progress based on the ratio of passing lessons to total lessons
                        enrollment.Progress = (double)countLesson / count * 100;
                        _context.Enrollments.Update(enrollment);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToPage("", new { courseId = CourseId, lessonId = CurrentLessonId });
            }
        }

        public async Task<IActionResult> OnPostUpdateTimeLearning([FromBody] ProgressLessonDTO request)
        {
            int lessonId = request.LessonId;
            double progress = request.Progress;

            LessonProgress progressLesson = progressLessonService.GetPLHaveEnroll(User.FindFirstValue(ClaimTypes.NameIdentifier), lessonId);
            Lesson less = lessonService.GetLessonById(lessonId);
            bool isPassing = false;
            if (progressLesson != null)
            {
                isPassing = (progress >= less.Passing);
                progressLesson.ProgressPercentage = (float)progress;
                progressLesson.Passing = isPassing ? 1 : 0;
            }
            bool haveUpdate = progressLessonService.UpdateProgress(progressLesson);
            if (progressLesson.Passing == 1)
            {
                var enrollment = _context.Enrollments.Where(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.CourseId == CourseId).FirstOrDefault();
                if (enrollment != null)
                {
                    int count = _context.Lessons.Where(x => x.Chapter.Course.Id == CourseId).Count();

                    // Calculate the total number of lessons in the course (from all chapters)
                    int countLesson = _context.LessonProgresses
            .Where(lp => lp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                 && lp.Passing == 1
                 && lp.Lesson.Chapter.CourseId == CourseId)
            .Count();

                    // Calculate the number of passing lessons in the course


                    // Update the enrollment progress based on the ratio of passing lessons to total lessons
                    enrollment.Progress = (double)countLesson / count * 100;
                    _context.Enrollments.Update(enrollment);
                    await _context.SaveChangesAsync();
                }

                
            }
            System.Diagnostics.Debug.WriteLine("run here = " + haveUpdate);
            return new JsonResult(new { isPassing = isPassing });
        }

        public bool validateTime(DateTime lastDate, int count)
        {
            //quy dinh mỗi lần làm quizz sẽ tăng lên 5p
            TimeSpan requiredInterval = TimeSpan.FromMinutes(count * 1);

            DateTime currentDate = DateTime.Now;

            return (currentDate - lastDate) >= requiredInterval;
        }

        public async Task<IActionResult> OnGetStartQuizz(int lessonId)
        {
            //create progress
            ProgressLesson = new LessonProgress();
            ProgressLesson.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ProgressLesson.LessonId = lessonId;
            ProgressLesson.ProgressPercentage = 0;
            ProgressLesson.Passing = 0;
            progressLessonService.EnrollLesson(ProgressLesson);

            return RedirectToPage("", new { lessonId = lessonId });
        }
        public async Task<IActionResult> OnPostAddCommentAsync(int lessonId, int courseId)
        {
            if (string.IsNullOrWhiteSpace(CommentContent))
            {

                Mess = "Binh luan khong duoc de trong";
                return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });
            }

            //var cleanContent = ProcessedCommentContent (CommentContent);


            var commentForCreation = new CommentForCreation
            {
                LessonId = lessonId,
                ParentCommentId = null,
                Content = CommentContent
            };

            await _commentService.AddCommentAsync(commentForCreation);


            Comments = await _commentService.GetCommentsByLessonIdAsync(lessonId);

            await _hubContext.Clients.All.SendAsync("ReceiveComment", Comments);

            return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });

        }

        public async Task<IActionResult> OnPostAddReplyCommentAsync(CommentList newComment, int lessonId, int courseId)
        {

            if (string.IsNullOrWhiteSpace(CommentContent))
            {
                ModelState.AddModelError("CommentContent", "Bình luận không được để trống.");
                return Page();
            }

            //var cleanContent = ProcessedCommentContent (CommentContent);

            var commentForCreation = new CommentForCreation
            {
                LessonId = lessonId,
                ParentCommentId = newComment.ParentCommentId, // Bình luận phản hồi có ParentCommentId
                Content = CommentContent
            };

            await _commentService.AddCommentAsync(commentForCreation);
            Comments = await _commentService.GetCommentsByLessonIdAsync(lessonId);

            return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });
        }

        public async Task<IActionResult> OnPostEditCommentAsync(int Id, string EditedCommentContent, int courseId, int lessonId, string CurrentUserId)
        {
            var success = await _commentService.EditCommentAsync(Id, EditedCommentContent);
            if (!success)
            {
                ModelState.AddModelError("EditedCommentContent", "Không thể chỉnh sửa bình luận hoặc bình luận không tồn tại.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(EditedCommentContent))
            {
                ModelState.AddModelError("EditedCommentContent", "Nội dung bình luận không được để trống.");
                return Page();
            }


            return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync(int Id, int courseId, int lessonId)
        {
            var success = await _commentService.DeleteCommentAsync(Id);

            if (!success)
            {
                ModelState.AddModelError("DeleteComment", "Không thể xóa bình luận.");
            }

            // Sau khi xóa, chuyển hướng lại trang học
            return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });
        }
    }

    /*public class LearningCourseModel : PageModel
    {
        private readonly ICommentService _commentService;
        private readonly IChapterService _chapterService;
        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        private readonly UserManager<User> _userManager;
        private readonly IPaymentService _paymentService;
        private readonly RepositoryContext _context;
        private readonly IHubContext<CommentHub> _hubContext;

        public CourseDetail? CourseDetail;
        public double LessonLoad { get; set; }
        public List<ChapterDTO>? Chapters { get; set; }
        public LessonDTO CurrentLesson { get; set; }
        public int Score { get; set; }
        public List<CommentList> Comments { get; set; } = new List<CommentList>();
        public int CommentCount => Comments.Count;

        [BindProperty]
        public int LessonId { get; set; }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public int CourseId { get; set; }

        [BindProperty]
        public string CommentContent { get; set; }

        [BindProperty]
        public string EditedCommentContent { get; set; }

        [BindProperty]
        public string CurrentUserId { get; set; }
        public string Status { get; set; }
        public int CountLessonSuccess { get; set; }

        public string? Mess { get; set; } = "";

        //[BindProperty]
        //public Dictionary<int,int> SelectedAnswers { get; set; }

        // Kết hợp hai constructor thành một
        public LearningCourseModel(
            ICommentService commentService,
            IChapterService chapterService,
            ICourseService courseService,
            ILessonService lessonService,
            UserManager<User> userManager,
            IPaymentService paymentService,
            RepositoryContext context,
            IHubContext<CommentHub> hubContext)
        {
            _commentService = commentService;
            _chapterService = chapterService;
            _courseService = courseService;
            _lessonService = lessonService;
            _userManager = userManager;
            _paymentService = paymentService;
            _commentService = commentService;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> OnGetAsync(int courseId, int lessonId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (await _courseService.HasAccessToCourseAsync(courseId) == 0)
                {
                    await _paymentService.GrantCourseAccessToUser(user.Id, courseId);
                }
                CurrentUserId = user.Id;
            }
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            LessonLoad = _context.LessonProgresses.Where(x => x.LessonId == lessonId && x.UserId == userID).Select(x => x.ProgressPercentage).FirstOrDefault();
            CountLessonSuccess = _context.LessonProgresses
                .Where(lp => lp.UserId == userID
                     && lp.Status == "Đã hoàn thành"
                     && lp.Lesson.Chapter.CourseId == courseId)
                .Count();
            Status = _context.LessonProgresses.Where(x => x.LessonId == lessonId && x.UserId == userID).Select(x => x.Status).FirstOrDefault();
            LessonId = lessonId;
            CourseId = courseId;
            Comments = await _commentService.GetCommentsByLessonIdAsync(lessonId);
            CourseDetail = await _courseService.GetCourseDetailHomePage(courseId);
            Chapters = await _chapterService.GetChaptersByLearningCourse(courseId, userID);
            CurrentLesson = await _lessonService.GetCurrentLesson(lessonId);
            return Page();
        }
        public async Task<IActionResult> OnPostSubmitQuizAsync(Dictionary<int, int> selectedAnswers)
        {

            // Call the service to submit the quiz
            int isPassed = await _lessonService.SubmitQuizAsync(LessonId, selectedAnswers);
            return RedirectToPage(new { courseId = CourseId, lessonId = LessonId });
        }



        public async Task<IActionResult> OnPostAddCommentAsync(int lessonId, int courseId)
        {
            if (string.IsNullOrWhiteSpace (CommentContent) ) {

                Mess = "Binh luan khong duoc de trong";
                Comments = await _commentService.GetCommentsByLessonIdAsync (lessonId);
                CourseDetail = await _courseService.GetCourseDetailHomePage (courseId);
                Chapters = await _chapterService.GetChaptersByCourseIdAsync (courseId);
                CurrentLesson = await _lessonService.GetCurrentLesson (lessonId);
                return Page ();
            }

            //var cleanContent = ProcessedCommentContent (CommentContent);

            CourseDetail = await _courseService.GetCourseDetailHomePage(courseId);
            Chapters = await _chapterService.GetChaptersByCourseIdAsync(courseId);

            var commentForCreation = new CommentForCreation
            {
                LessonId = lessonId,
                ParentCommentId = null,
                Content = CommentContent
            };

            await _commentService.AddCommentAsync(commentForCreation);

  
            Comments = await _commentService.GetCommentsByLessonIdAsync(lessonId);

            await _hubContext.Clients.All.SendAsync ("ReceiveComment", Comments);

            return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });

        }

        public async Task<IActionResult> OnPostAddReplyCommentAsync(CommentList newComment, int lessonId, int courseId)
        {

            if ( string.IsNullOrWhiteSpace (CommentContent) ) {
                ModelState.AddModelError("CommentContent", "Bình luận không được để trống.");
                return Page ();
            }

            //var cleanContent = ProcessedCommentContent (CommentContent);

            var commentForCreation = new CommentForCreation
            {
                LessonId = lessonId,
                ParentCommentId = newComment.ParentCommentId, // Bình luận phản hồi có ParentCommentId
                Content = CommentContent
            };

            await _commentService.AddCommentAsync(commentForCreation);
            Comments = await _commentService.GetCommentsByLessonIdAsync(lessonId);

            return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });
        }

        public async Task<IActionResult> OnPostEditCommentAsync(int Id, string EditedCommentContent, int courseId, int lessonId, string CurrentUserId)
        {
            var success = await _commentService.EditCommentAsync(Id, EditedCommentContent);
            if (!success)
            {
                ModelState.AddModelError("EditedCommentContent", "Không thể chỉnh sửa bình luận hoặc bình luận không tồn tại.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(EditedCommentContent))
            {
                ModelState.AddModelError("EditedCommentContent", "Nội dung bình luận không được để trống.");
                return Page();
            }


            return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync(int Id, int courseId, int lessonId)
        {
            var success = await _commentService.DeleteCommentAsync(Id);

            if (!success)
            {
                ModelState.AddModelError("DeleteComment", "Không thể xóa bình luận.");
            }

            // Sau khi xóa, chuyển hướng lại trang học
            return RedirectToPage("/Homepage/LearningCourse", new { courseId, lessonId });
        }

    }*/
}
