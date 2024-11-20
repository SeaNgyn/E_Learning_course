using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string UserName { get; set; }
        public string avatar { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public double Progress { get; set; }
        public int Status { get; set; }
        public DateTime? ExpiredDate { get; set; }

    }
    public class EnrollmentService : IEnrollmentService
    {
        private readonly RepositoryContext _context;

        public async Task<List<Enrollment>> GetListCustomerEnrollment(string userId)
        {
            var enrollment = await _context.Enrollments.Include(x=>x.Course).Where(x=>x.UserId == userId).ToListAsync();
            return enrollment;
        }

        private readonly UserManager<User> _userManager;
        public EnrollmentService(RepositoryContext context, UserManager<User> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public int AmountEnroll(int courseId)
        {
          
            return _context.Enrollments.Where(er => er.CourseId == courseId).Count();
        }

        public async Task<List<Enrollment>> EnrollmentList()
        {
            return await _context.Enrollments.ToListAsync();
        }

        public async Task<List<EnrollmentDto>> EnrollmentListByCourseId(int courseId)
        {
            try
            {
                return await _context.Enrollments
                    .Where(er => er.CourseId == courseId)
                    .Select(er => new EnrollmentDto
                    {
                        EnrollmentId = er.Id,
                        CourseId = er.CourseId,
                        UserName = er.User.UserName,
                        avatar = er.User.Avatar,
                        EnrollmentDate = er.EnrollmentDate,
                        Progress = er.Progress,
                        Status = er.Status,
                        ExpiredDate = er.ExpiredDate != null ? er.ExpiredDate : DateTime.MinValue,
                    })
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PaginatedResult<Enrollment>> GetEnrollmentsAsync(string searchTerm, string sortField, bool isAscending, int page, int pageSize, string userId)
        {
            var query = _context.Enrollments.Include(x => x.Course).ThenInclude(x => x.Category).Include(x => x.User).Where(x => x.UserId == userId)
                .AsQueryable();
            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e => e.Course.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Sorting
            query = sortField switch
            {
                "name" => isAscending ? query.OrderBy(e => e.Course.Title) : query.OrderByDescending(e => e.Course.Title),
                "price" => isAscending ? query.OrderBy(e => e.Course.Price) : query.OrderByDescending(e => e.Course.Price),
                "date" => isAscending ? query.OrderBy(e => e.EnrollmentDate) : query.OrderByDescending(e => e.EnrollmentDate),
                _ => query
            };

            // Pagination
            var totalItems = await query.CountAsync();
            var enrollments = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResult<Enrollment>
            {
                Items = enrollments,
                TotalCount = totalItems,
                PageSize = pageSize,
                PageNumber = page
            };
        }
    }
}
