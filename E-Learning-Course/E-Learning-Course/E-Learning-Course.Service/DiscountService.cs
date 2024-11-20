using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class DiscountService : IDiscountService
    {

        private readonly RepositoryContext _repositoryContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DiscountService(RepositoryContext repositoryContext,IHttpContextAccessor httpContextAccessor)
        {
            _repositoryContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task AddDiscount(DiscountForCourse discount)
        {
            string uid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Discount newDiscount = new Discount()
            {
                Code = discount.Code,
                DiscountPer = discount.DiscountPer,
                MaxUses = discount.MaxUses,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                CourseId = discount.CourseName,
                CreateBy = uid,
            };
            _repositoryContext.Add(newDiscount);
            _repositoryContext.SaveChanges();
        }

        public async Task UpdateDiscount(DiscountForCourse discount, ClaimsPrincipal currentUser)
        {
            string uid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Discount updateDiscount = _repositoryContext.Discounts.Find(discount.Id);
            if (updateDiscount == null)
            {
                throw new KeyNotFoundException("Discount not found.");
            }


            updateDiscount.Code = discount.Code;
            updateDiscount.DiscountPer = discount.DiscountPer;
            updateDiscount.MaxUses = discount.MaxUses;
            updateDiscount.StartDate = discount.StartDate;
            updateDiscount.EndDate = discount.EndDate;
            updateDiscount.CourseId = discount.CourseName;
            updateDiscount.UpdateBy = uid;

            _repositoryContext.Update(updateDiscount);
            _repositoryContext.SaveChanges();
        }
    }
}
