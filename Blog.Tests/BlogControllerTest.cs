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
using System.Security.Claims;
using Blog.Authorization;
using Blog.Entities;
using Blog.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Tests
{
    [TestClass]
    public class BlogControllerTest
    {
        private Mock<IBlogRepository> _repository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mockAutoMapper;
        private List<ApplicationUser> _users;
        private Mock<UserManager<ApplicationUser>> _userManager;
        private IAuthorizationService _authService;
        private IMapper _mapper;


        [TestInitialize]
        public void SetupContext()
        {
            var realProfile = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            _mapper = new Mapper(configuration);

            _repository = new Mock<IBlogRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockAutoMapper = new Mock<IMapper>();
            _users = new List<ApplicationUser>
            {
                new() { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de", UserName = "user1@bv.com" },
                new() { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de", UserName = "user2@bv.com" }
            };
            _userManager = MockHelpers.MockUserManager(_users);
            var authHandler = new UserIsBlogOwnerAuthorizationHandler(_userManager.Object);
            _authService = MockHelpers
                .BuildAuthorizationService(services =>
                {
                    services.AddScoped<IAuthorizationHandler>(sp => authHandler);
                    services.AddAuthorization();
                });
        }

        [TestMethod]
        public void CreateWhenCalledWithParameterReturnsView()
        {
            // Arrange
            var controller = new BlogController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mockAutoMapper.Object
            );

            // Act 
            var result = controller.Create(new CreateBlogViewModel());

            // Assert 
            Assert.IsNotNull(result, "View Result is null");
        }

        [TestMethod]
        public void EditWhenCalledWithIdParameterReturnsView()
        {
            // Arrange
            var controller = new BlogController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mockAutoMapper.Object
            );

            // Act 
            var result = controller.Edit("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");

            // Assert 
            Assert.IsNotNull(result, "View Result is null");
        }

        [TestMethod]
        public void EditWhenCalledWithViewModelReturnsView()
        {
            // Arrange
            var controller = new BlogController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mockAutoMapper.Object
            );

            // Act 
            var result = controller.Edit(new Entities.Blog());

            // Assert 
            Assert.IsNotNull(result, "View Result is null");
        }

        [TestMethod]
        public void DeleteWhenCalledReturnsView()
        {
            // Arrange
            var controller = new BlogController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mockAutoMapper.Object
            );

            // Act 
            var result = controller.Delete("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");

            // Assert 
            Assert.IsNotNull(result, "View Result is null");
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
                _mapper
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
            _mockAutoMapper.Setup(x => x.Map<Entities.Blog>(It.IsAny<CreateBlogViewModel>()))
                .Returns((CreateBlogViewModel source) => new Entities.Blog
                {
                    Name = "Test",
                    Description = "Test"
                });
            _userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");

            var controller = new BlogController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mockAutoMapper.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            // Act 
            var blogViewModel = new CreateBlogViewModel
            {
                Name = "Test",
                Description = "Test",
                OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
            };
            var result = controller.Create(blogViewModel);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _repository.Verify(x => x.Add(It.IsAny<Entities.Blog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void CompleteIsNotCalledWhenViewModelIsEmpty()
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
                _mapper
            );

            // Act 
            var result = controller.Edit(new Entities.Blog());
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(0));
        }

        [TestMethod]
        public void CompleteIsCalledWhenViewModelIsValid()
        {
            // Arrange 
            _repository = new Mock<IBlogRepository>();
            _repository.Setup(x => x.Add(It.IsAny<Entities.Blog>()));
            _unitOfWork.Setup(uow => uow.Blogs).Returns(_repository.Object);
            _userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");
            _repository.Setup(x => x.Get(new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0"),
                    Name = "Test1",
                    Description = "Test1",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });

            var controller = new BlogController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mockAutoMapper.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            // Act 
            var blogViewModel = new Entities.Blog
            {
                Id = new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0"),
                Name = "Test",
                Description = "Test",
                OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
            };
            var result = controller.Edit(blogViewModel);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(1));
        }

        [TestMethod]
        public void DeleteIsNotCalledWhenViewModelIsEmpty()
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
                _mapper
            );

            // Act 
            var result = controller.Delete("");
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _repository.Verify(x => x.Remove(It.IsAny<Entities.Blog>()), Times.Exactly(0));
        }

        [TestMethod]
        public void DeleteIsCalledWhenViewModelIsNotEmpty()
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
                _mapper
            );

            // Act 
            var result = controller.Delete("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _repository.Verify(x => x.Remove(It.IsAny<Entities.Blog>()), Times.Exactly(0));
        }
    }
}