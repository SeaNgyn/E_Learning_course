using E_Learning_Course.Data.Entities;
using E_Learning_Course.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Learning_Course.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace E_Learning_Course.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly RepositoryContext context;

        public CategoryService(RepositoryContext repositoryContext)
        {
            context = repositoryContext;
        }

        public async Task<CategoryForCreation> AddCategoryAsync(CategoryForCreation category)
        {
            Category newCate = new Category
            {
                Name = category.Name,
                Description = category.Description,
                Status = 1,
            };

            context.Categories.Add(newCate);
            await context.SaveChangesAsync();
            return await Task.FromResult(category);
        }

        public async Task<bool> UpdateCategoryAsync(CategoryForCreation category)
        {
            var existingCategory = await context.Categories.FirstOrDefaultAsync(c=>c.Id == category.Id);

            if (existingCategory == null)
                return false;

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            context.Categories.Update(existingCategory);
            await context.SaveChangesAsync();

            return await Task.FromResult(true);
        }
    }
}
