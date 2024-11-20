using E_Learning_Course.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface ICategoryService
    {
        Task<CategoryForCreation> AddCategoryAsync(CategoryForCreation category);
        Task<bool> UpdateCategoryAsync(CategoryForCreation category);
    }
}
