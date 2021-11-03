using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using Blog.Authorization;
using Blog.Controllers;
using Blog.Entities;
using Blog.Interfaces;
using Blog.Interfaces.IRepositories;
using Blog.Models.CommentViewModels;
using Blog.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Blog.Tests
{
    [TestClass]
    public class CommentsControllerTest
    {
        private Mock<ICommentRepository> _repository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mockAutoMapper;
        private List<ApplicationUser> _users;
        private Mock<UserManager<ApplicationUser>> _userManager;
        private IAuthorizationService _authService;
        private IMapper _mapper;
        private Mock<ITokenService> _tokenService;
        private Mock<IConfiguration> _configuration;
        private Mock<SignInManager<ApplicationUser>> _signInManager;


        [TestInitialize]
        public void SetupContext()
        {
            var realProfile = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            _mapper = new Mapper(configuration);

            _repository = new Mock<ICommentRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockAutoMapper = new Mock<IMapper>();
            _users = new List<ApplicationUser>
            {
                new() { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de", UserName = "user1@bv.com" },
                new() { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de", UserName = "user2@bv.com" }
            };
            _userManager = MockHelpers.MockUserManager(_users);
            var authHandler = new UserIsCommentOwnerAuthorizationHandler(_userManager.Object, _unitOfWork.Object);
            _authService = MockHelpers
                .BuildAuthorizationService(services =>
                {
                    services.AddScoped<IAuthorizationHandler>(sp => authHandler);
                    services.AddAuthorization();
                });
            _configuration = new Mock<IConfiguration>();
            _tokenService = new Mock<ITokenService>();
            _signInManager = new Mock<SignInManager<ApplicationUser>>(
                _userManager.Object,
                /* IHttpContextAccessor contextAccessor */Mock.Of<IHttpContextAccessor>(),
                /* IUserClaimsPrincipalFactory<TUser> claimsFactory */
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                /* IOptions<IdentityOptions> optionsAccessor */null,
                /* ILogger<SignInManager<TUser>> logger */null,
                /* IAuthenticationSchemeProvider schemes */null,
                /* IUserConfirmation<TUser> confirmation */null);
        }

        [TestMethod]
        public void ReturnsViewResultWhenGetCommentsIsCalled()
        {
            // Arrange
            var controller = new CommentsController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mockAutoMapper.Object,
                _tokenService.Object,
                _signInManager.Object
            );

            // Act 
            var result = controller.GetComments("6AD3D899-482E-4DD5-E75D-08D99AB50C36");

            // Assert 
            Assert.IsNotNull(result, "View Result is null");
        }

        [TestMethod]
        public void SaveIsNotCalledWhenViewModelIsEmpty()
        {
            // Arrange 
            _repository = new Mock<ICommentRepository>();
            _repository.Setup(x => x.Add(It.IsAny<Comment>()));
            _unitOfWork.Setup(uow => uow.Comments).Returns(_repository.Object);


            var controller = new CommentsController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mockAutoMapper.Object,
                _tokenService.Object,
                _signInManager.Object
            );

            // Act 
            var result = controller.CreateComment(new CreateCommentDto());
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _repository.Verify(x => x.Add(It.IsAny<Comment>()), Times.Exactly(0));
        }

        [TestMethod]
        public void SaveIsCalledWhenViewModelIsValid()
        {
            // Arrange 
            _repository = new Mock<ICommentRepository>();
            _repository.Setup(x => x.Add(It.IsAny<Comment>()));
            _unitOfWork.Setup(uow => uow.Comments).Returns(_repository.Object);


            var controller = new CommentsController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _tokenService.Object,
                _signInManager.Object
            );

            // Act 
            var result = controller.CreateComment(new CreateCommentDto
            {
                Text = "Hello", PostId = new Guid("EF84D4C6-1E41-4195-36A9-08D99B2557C5"),
                OwnerId = "5a6649c3-3496-4733-bd9c-797b0c51077b"
            });
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _repository.Verify(x => x.Add(It.IsAny<Comment>()), Times.Exactly(1));
        }

        [TestMethod]
        public void SubscribeToBlog_WhenBlogOwnerIsEqualBlogOwnerFromViewModel_CompleteIsNotCalled()
        {
            var blogRepository = new Mock<IBlogRepository>();
            blogRepository.Setup(x => x.Add(It.IsAny<Entities.Blog>()));
            _unitOfWork.Setup(uow => uow.Blogs).Returns(blogRepository.Object);
            _userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.FromResult(new ApplicationUser { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de" }));
            blogRepository.Setup(x => x.Get(new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0"),
                    Name = "Test1",
                    Description = "Test1",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });

            var controller = new CommentsController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _tokenService.Object,
                _signInManager.Object
            )
            {
                ControllerContext = MockHelpers.FakeControllerContext(true)
            };

            // Act 
            var result = controller.SubscribeToBlog("555c3434-8ec7-48ff-b30c-5cb7047d87f0");
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(0));
        }

        [TestMethod]
        public void SubscribeToBlog_WhenBlogOwnerIsEqualBlogOwnerFromViewModel_CompleteIsCalled()
        {
            var blogRepository = new Mock<IBlogRepository>();
            blogRepository.Setup(x => x.Add(It.IsAny<Entities.Blog>()));
            _unitOfWork.Setup(uow => uow.Blogs).Returns(blogRepository.Object);
            _userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");

            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.FromResult(new ApplicationUser { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de" }));
            blogRepository.Setup(x => x.Get(new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0"),
                    Name = "Test1",
                    Description = "Test1",
                    OwnerId = new Guid("000b68b2-3fe3-41d2-a58b-08d980f4d000")
                });

            var controller = new CommentsController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _tokenService.Object,
                _signInManager.Object
            )
            {
                ControllerContext = MockHelpers.FakeControllerContext(true)
            };

            // Act 
            var result = controller.SubscribeToBlog("555c3434-8ec7-48ff-b30c-5cb7047d87f0");
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(1));
        }

        [TestMethod]
        public void UnsubscribeToBlogRemoveIsCalled()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.FromResult(new ApplicationUser { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de" }));
            var blogRepository = new Mock<IBlogRepository>();
            var blogUserRepo = new Mock<IBlogApplicationUserRepository>();
            blogRepository.Setup(x => x.Get(new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("555c3434-8ec7-48ff-b30c-5cb7047d87f0"),
                    Name = "Test1",
                    Description = "Test1",
                    OwnerId = new Guid("000b68b2-3fe3-41d2-a58b-08d980f4d000")
                });
            _unitOfWork.Setup(uow => uow.Blogs).Returns(blogRepository.Object);
            _unitOfWork.Setup(uow => uow.BlogApplicationUsers).Returns(blogUserRepo.Object);


            var controller = new CommentsController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _tokenService.Object,
                _signInManager.Object
            )
            {
                ControllerContext = MockHelpers.FakeControllerContext(true)
            };

            // Act 
            var result = controller.SubscribeToBlog("555c3434-8ec7-48ff-b30c-5cb7047d87f0");
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(1));
        }

        [TestMethod]
        public void RemoveIsNotCalledWhenCommentIsBeingDeletedIfUserIsNotAuthorized()
        {
            // Arrange 
            _repository = new Mock<ICommentRepository>();
            _repository.Setup(x => x.Add(It.IsAny<Comment>()));
            _unitOfWork.Setup(uow => uow.Comments).Returns(_repository.Object);


            var controller = new CommentsController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _tokenService.Object,
                _signInManager.Object
            )
            {
                ControllerContext = MockHelpers.FakeControllerContext(true)
            };

            // Act 
            var result = controller.Delete("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _repository.Verify(x => x.Remove(It.IsAny<Comment>()), Times.Exactly(0));
        }


    }
}