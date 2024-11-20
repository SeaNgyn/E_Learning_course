using E_Learning_Course.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IDiscountService
    {
        Task AddDiscount(DiscountForCourse discount);
        Task UpdateDiscount(DiscountForCourse discount, ClaimsPrincipal currentUser);
    }
}
