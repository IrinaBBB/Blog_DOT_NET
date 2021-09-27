using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blog.Tests
{
    [TestClass]
    public class BlogRepositoryTests
    {
        [TestMethod]
        public void BlogGetsSavedIntoDatabase()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("BlogDatabaseForTesting")
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Blogs.Add(new Entities.Blog
            {
                Name = "Test",
                Description = "Test"
            });

            context.SaveChanges();

            var blogRepository = new BlogRepository(context);

            // Act
            var blogs = blogRepository.GetAll();

            // Assert
            Assert.AreEqual(1, blogs.Count());
        }

        [TestMethod]
        public async Task BlogsGetReturnedByOwnerId()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("BlogDatabaseForTesting")
                .Options;

            await using var context = new ApplicationDbContext(options);
            context.Blogs.AddRange(
                new Entities.Blog
                {
                    Name = "Test",
                    Description = "Test",
                    OwnerId = new System.Guid("61946b17-7ed0-4154-c5df-08d9811874dc")
                },
                new Entities.Blog
                {
                    Name = "Test1",
                    Description = "Test2",
                    OwnerId = new System.Guid("61946b17-7ed0-4154-c5df-08d9811874dc")
                },
                new Entities.Blog
                {
                    Name = "Test3",
                    Description = "Test3",
                    OwnerId = new System.Guid("61946b17-7ed0-4154-c5df-08d9811874dc")
                },
                new Entities.Blog
                {
                    Name = "Test4",
                    Description = "Test4",
                    OwnerId = new System.Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                },
                new Entities.Blog
                {
                    Name = "Test5",
                    Description = "Test5",
                    OwnerId = new System.Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });


            await context.SaveChangesAsync();
            var blogRepository = new BlogRepository(context);

            // Act
            var blogs = await blogRepository.GetBlogsByOwnerId("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");

            // Assert
            Assert.AreEqual(2, blogs.Count());
        }
    }
}