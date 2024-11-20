using E_Learning_Course.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace E_Learning_Course.Data
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options) : base (options)
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<Finance> Finances { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        public DbSet<Certificate> Certificates { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating (modelBuilder);
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());
            // Course Table Configurations
            modelBuilder.Entity<Course> ()
                .HasKey (c => c.Id);

            modelBuilder.Entity<Course> ()
                .Property (c => c.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Course> ()
                .HasOne (c => c.Category)
                .WithMany ()
                .HasForeignKey (c => c.CategoryId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Course> ()
                .HasOne (c => c.Creator)
                .WithMany ()
                .HasForeignKey (c => c.CreateBy)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Course> ()
                .HasOne (c => c.Updater)
                .WithMany ()
                .HasForeignKey (c => c.UpdateBy)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Course> ()
                .Property (c => c.CreatedAt)
                .HasDefaultValueSql ("GETDATE()");

            // Category Table Configurations
            modelBuilder.Entity<Category> ()
                .HasKey (c => c.Id);

            modelBuilder.Entity<Category> ()
                .Property (c => c.Id)
                .ValueGeneratedOnAdd ();

            //Chapter
            modelBuilder.Entity<Chapter> ()
                .HasKey (c => c.Id);

            modelBuilder.Entity<Chapter> ()
                .Property (c => c.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Chapter> ()
               .HasOne (c => c.Course)
               .WithMany ()
               .HasForeignKey (c => c.CourseId)
               .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Chapter> ()
                .HasOne (c => c.Creator)
                .WithMany ()
                .HasForeignKey (c => c.CreateBy)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Chapter> ()
                .HasOne (c => c.Updater)
                .WithMany ()
                .HasForeignKey (c => c.UpdateBy)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Chapter> ()
                .Property (c => c.CreatedAt)
                .HasDefaultValueSql ("GETDATE()");

            //Lessons
            modelBuilder.Entity<Lesson> ()
                .HasKey (c => c.Id);

            modelBuilder.Entity<Lesson> ()
                .Property (c => c.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Lesson> ()
               .HasOne (c => c.Chapter)
               .WithMany ()
               .HasForeignKey (c => c.ChapterId)
               .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Lesson> ()
              .HasOne (c => c.Creator)
              .WithMany ()
              .HasForeignKey (c => c.CreateBy)
              .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Lesson> ()
                .HasOne (c => c.Updater)
                .WithMany ()
                .HasForeignKey (c => c.UpdateBy)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Chapter> ()
                .Property (c => c.CreatedAt)
                .HasDefaultValueSql ("GETDATE()");

            modelBuilder.Entity<Enrollment> ()
                .HasKey (e => e.Id);

            modelBuilder.Entity<Enrollment> ()
                .Property (e => e.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Enrollment> ()
                .HasOne (e => e.User)
                .WithMany ()
                .HasForeignKey (e => e.UserId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment> ()
                .HasOne (e => e.Course)
                .WithMany ()
                .HasForeignKey (e => e.CourseId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment> ()
                .Property (e => e.EnrollmentDate)
                .HasDefaultValueSql ("GETDATE()");

            modelBuilder.Entity<Payment> ()
        .HasKey (p => p.Id);

            modelBuilder.Entity<Payment> ()
                .Property (p => p.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Payment> ()
                .HasOne (p => p.Course)
                .WithMany ()
                .HasForeignKey (p => p.CourseId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment> ()
                .HasOne (p => p.User)
                .WithMany () // Same with user if needed
                .HasForeignKey (p => p.UserId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment> ()
                .Property (p => p.PaymentDate)
                .HasDefaultValueSql ("GETDATE()");

            modelBuilder.Entity<Question> ()
                .HasKey (c => c.Id);

            modelBuilder.Entity<Question> ()
                .Property (c => c.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Question> ()
               .HasOne (c => c.Lesson)
               .WithMany ()
               .HasForeignKey (c => c.LessonId)
               .OnDelete (DeleteBehavior.Restrict);

            //Answer
            modelBuilder.Entity<Answer> ()
                .HasKey (c => c.Id);

            modelBuilder.Entity<Answer> ()
                .Property (c => c.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Answer> ()
               .HasOne (c => c.Question)
               .WithMany ()
               .HasForeignKey (c => c.QuestionId)
               .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonProgress> ()
        .HasKey (lp => lp.Id);

            modelBuilder.Entity<LessonProgress> ()
                .Property (lp => lp.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<LessonProgress> ()
                .HasOne (lp => lp.Lesson)
                .WithMany ()
                .HasForeignKey (lp => lp.LessonId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonProgress> ()
                .HasOne (lp => lp.User)
                .WithMany ()
                .HasForeignKey (lp => lp.UserId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonProgress> ()
                .Property (lp => lp.CreatedAt)
                .HasDefaultValueSql ("GETDATE()");

            modelBuilder.Entity<Comment> ()
               .HasKey (c => c.Id); // Thiết lập khóa chính

            modelBuilder.Entity<Comment> ()
                .Property (c => c.Id)
                .ValueGeneratedOnAdd (); // ID tự động tăng

            modelBuilder.Entity<Comment> ()
                .HasOne (c => c.Lesson)
                .WithMany ()
                .HasForeignKey (c => c.LessonId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment> ()
                .HasOne (c => c.User)
                .WithMany () // Một người dùng có thể có nhiều bình luận
                .HasForeignKey (c => c.UserId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment> ()
                .HasOne (c => c.ParentComment)
                .WithMany (c => c.ChildComments) // Một bình luận có thể có nhiều bình luận con
                .HasForeignKey (c => c.ParentCommentId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment> ()
                .Property (c => c.CreatedAt)
                .HasDefaultValueSql ("GETDATE()"); // Mặc định thời gian tạo

            modelBuilder.Entity<Comment> ()
                .Property (c => c.IsDelete)
                .HasDefaultValue (false); // Mặc định IsDelete là false
            modelBuilder.Entity<Finance> ()
               .HasKey (f => f.Id);

            modelBuilder.Entity<Finance> ()
                .Property (f => f.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Finance> ()
                .Property (f => f.CreatedAt)
                .HasDefaultValueSql ("GETDATE()");

            modelBuilder.Entity<Finance> ()
                .Property (f => f.UpdatedAt)
                .HasDefaultValueSql ("GETDATE()");
            modelBuilder.Entity<Discount> ()
               .HasKey (d => d.Id); // Set the primary key

            modelBuilder.Entity<Discount> ()
                .Property (d => d.Id)
                .ValueGeneratedOnAdd (); // Auto-increment ID

            modelBuilder.Entity<Discount> ()
                .Property (d => d.Code)
                .IsRequired (); // Set the Code as required

            modelBuilder.Entity<Discount> ()
                .Property (d => d.DiscountPer)
                .IsRequired (); // Set DiscountPer as required

            modelBuilder.Entity<Discount> ()
                .Property (d => d.MaxUses)
                .IsRequired (); // Set MaxUses as required

            modelBuilder.Entity<Discount> ()
                .Property (d => d.CreatedAt)
                .HasDefaultValueSql ("GETDATE()"); // Default CreatedAt to current date

            modelBuilder.Entity<Discount> ()
                .Property (d => d.UpdatedAt)
                .HasDefaultValueSql ("GETDATE()"); // Default UpdatedAt to current date

            // If there are relationships to other entities, configure them here
            // For example, if you have a relationship with Course:
            modelBuilder.Entity<Discount> ()
                .HasOne<Course> () // Assuming a Discount can be associated with a Course
                .WithMany () // Specify navigation properties if available
                .HasForeignKey (d => d.CourseId) // Use CourseId as the foreign key
                .OnDelete (DeleteBehavior.Restrict); // Specify delete behavior

            modelBuilder.Entity<Discount> ()
    .HasKey (d => d.Id);

            modelBuilder.Entity<Discount> ()
                .Property (d => d.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Discount> ()
                .Property (d => d.Code)
                .IsRequired ();

            modelBuilder.Entity<Discount> ()
                .Property (d => d.DiscountPer)
                .IsRequired ();

            modelBuilder.Entity<Discount> ()
                .Property (d => d.MaxUses)
                .IsRequired ();

            modelBuilder.Entity<Discount> ()
                .Property (d => d.CreatedAt)
                .HasDefaultValueSql ("GETDATE()");

            modelBuilder.Entity<Discount> ()
                .Property (d => d.UpdatedAt)
                .HasDefaultValueSql ("GETDATE()");

            modelBuilder.Entity<Discount> ()
                .HasOne (d => d.Course) // Add navigation property in Discount for Course if needed
                .WithMany (c => c.Discounts) // Updated to allow multiple discounts per course
                .HasForeignKey (d => d.CourseId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate> ()
                .HasKey (c => c.Id);

            modelBuilder.Entity<Certificate> ()
                .Property (c => c.Id)
                .ValueGeneratedOnAdd ();

            modelBuilder.Entity<Certificate> ()
                .Property (c => c.IssueDate)
                .HasDefaultValueSql ("GETDATE()");

            modelBuilder.Entity<Certificate> ()
                .HasOne<Enrollment> ()
                .WithMany ()
                .HasForeignKey (c => c.EnrollmentId)
                .OnDelete (DeleteBehavior.Restrict);



        }
    }
}
