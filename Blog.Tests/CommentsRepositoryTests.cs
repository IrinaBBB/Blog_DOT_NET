using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Data.Repositories;
using Blog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blog.Tests
{
    [TestClass]
    public class CommentsRepositoryTests
    {
        [TestMethod]
        public void CommentGetsSavedIntoDatabase()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("CommentDatabaseForTesting")
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Comments.Add(new Comment
            {
                Text = "Test",
            });

            context.SaveChanges();

            var commentsRepository = new CommentRepository(context);

            // Act
            var comments = commentsRepository.GetAll();

            // Assert
            Assert.AreEqual(1, comments.Count());
        }

        [TestMethod]
        public async Task CommentsGetReturnedByPostId()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("CommentsDatabaseForTesting")
                .Options;

            await using var context = new ApplicationDbContext(options);
            context.Comments.AddRange(
                new Comment
                {
                    Text = "Test",
                    OwnerId = new System.Guid("61946b17-7ed0-4154-c5df-08d9811874dc"),
                    PostId = new System.Guid("5a6649c3-3496-4733-bd9c-797b0c51077b")
                },
                new Comment
                {
                    Text = "Test",
                    OwnerId = new System.Guid("61946b17-7ed0-4154-c5df-08d9811874dc"),
                    PostId = new System.Guid("5a6649c3-3496-4733-bd9c-797b0c51077b")
                },
                new Comment
                {
                    Text = "Test3",
                    OwnerId = new System.Guid("61946b17-7ed0-4154-c5df-08d9811874dc"),
                    PostId = new System.Guid("5a6649c3-3496-4733-bd9c-797b0c51077b")
                },
                new Comment
                {
                    Text = "Test4",
                    OwnerId = new System.Guid("61946b17-7ed0-4154-c5df-08d9811874dc"),
                    PostId = new System.Guid("0a6649c3-3496-4733-bd9c-797b0c510770")
                },
                new Comment
                {
                    Text = "Test5",
                    OwnerId = new System.Guid("61946b17-7ed0-4154-c5df-08d9811874dc"),
                    PostId = new System.Guid("0a6649c3-3496-4733-bd9c-797b0c510770")
                }
            );


            await context.SaveChangesAsync();
            var commentRepository = new CommentRepository(context);

            // Act
            var comments = await commentRepository.GetCommentsByPostId("5a6649c3-3496-4733-bd9c-797b0c51077b");

            // Assert
            Assert.AreEqual(3, comments.Count());
        }
    }
}