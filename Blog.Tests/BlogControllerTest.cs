using AutoMapper;
using Blog.Controllers;
using Blog.Interfaces;
using Blog.Interfaces.IRepositories;
using Blog.Models.BlogViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Blog.Tests
{
    [TestClass]
    public class BlogControllerTest
    {
        private Mock<IBlogRepository> _repository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _autoMapper;
        private List<IdentityUser> _users;
        private Mock<UserManager<IdentityUser>> _userManager;
        private IAuthorizationService _authService;


        [TestInitialize]
        public void SetupContext()
        {
            _repository = new Mock<IBlogRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _autoMapper = new Mock<IMapper>();
            _users = new List<IdentityUser>
            {
                new("user1@bv.com") { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de" },
                new("user2@bv.com") { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de" }
            };

            _userManager = MockHelpers.MockUserManager<IdentityUser>(_users);
            _authService = MockHelpers
                .BuildAuthorizationService(services =>
                { });
        }

        [TestMethod]
        public void SaveIsNotCalledWhenViewModelIsEmpty()
        {
            // Arrange 
            _repository = new Mock<IBlogRepository>();
            _repository.Setup(x => x.Add(It.IsAny<Entities.Blog>()));
            _unitOfWork.Setup(uow => uow.Blogs).Returns(_repository.Object);


            var controller = new BlogController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _autoMapper.Object
            );

            // Act 
            var result = controller.Create(new CreateBlogViewModel());
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _repository.Verify(x => x.Add(It.IsAny<Entities.Blog>()), Times.Exactly(0));
        }

        [TestMethod]
        public void SaveIsCalledWhenViewModelIsValid()
        {
            // Arrange 
            _repository = new Mock<IBlogRepository>();
            _repository.Setup(x => x.Add(It.IsAny<Entities.Blog>()));
            _unitOfWork.Setup(uow => uow.Blogs).Returns(_repository.Object);
            _autoMapper.Setup(x => x.Map<Entities.Blog>(It.IsAny<CreateBlogViewModel>()))
                .Returns((CreateBlogViewModel source) => new Entities.Blog
                {
                    Name = "Test",
                    Description = "Test",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });


            var controller = new BlogController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _autoMapper.Object
            );

            // Act 
            var blogViewModel = new CreateBlogViewModel
            {
                Name = "Test",
                Description = "Test"
            };
            var result = controller.Create(blogViewModel);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _repository.Verify(x => x.Add(It.IsAny<Entities.Blog>()), Times.Exactly(1));
        }
    }
}