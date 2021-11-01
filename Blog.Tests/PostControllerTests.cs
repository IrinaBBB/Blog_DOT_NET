using System.Collections.Generic;
using AutoMapper;
using Blog.Authorization;
using Blog.Entities;
using Blog.Hubs;
using Blog.Interfaces;
using Blog.Interfaces.IRepositories;
using Blog.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Blog.Tests
{
    [TestClass]
    public class PostControllerTests
    {
        private Mock<IPostRepository> _repository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mockAutoMapper;
        private List<ApplicationUser> _users;
        private Mock<UserManager<ApplicationUser>> _userManager;
        private IAuthorizationService _authService;
        private IMapper _mapper;
        private Mock<IHubContext<PostNotificationHub>> _hub;


        [TestInitialize]
        public void SetupContext()
        {
            var realProfile = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            _mapper = new Mapper(configuration);

            _repository = new Mock<IPostRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockAutoMapper = new Mock<IMapper>();
            _users = new List<ApplicationUser>
            {
                new() { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de", UserName = "user1@bv.com" },
                new() { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de", UserName = "user2@bv.com" }
            };
            _userManager = MockHelpers.MockUserManager(_users);
            var authHandler = new UserIsPostOwnerAuthorizationHandler(_userManager.Object, _unitOfWork.Object);
            _authService = MockHelpers
                .BuildAuthorizationService(services =>
                {
                    services.AddScoped<IAuthorizationHandler>(sp => authHandler);
                    services.AddAuthorization();
                });
            _hub = new Mock<IHubContext<PostNotificationHub>>();
        }

        //[TestMethod]
        //public void SaveIsCalledWhenViewModelIsValid()
        //{
        //    // Arrange 
        //    _repository = new Mock<IPostRepository>();
        //    _repository.Setup(x => x.Add(It.IsAny<Post>()));
        //    _unitOfWork.Setup(uow => uow.Posts).Returns(_repository.Object);
        //    _userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
        //        .Returns("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");

        //    var controller = new PostController
        //    (
        //        _unitOfWork.Object,
        //        _authService,
        //        _userManager.Object,
        //        _mapper,
        //        _hub.Object
        //    );
        //    controller.ControllerContext = MockHelpers.FakeControllerContext(true);
        //    // Act 
        //    var viewModel = new CreateEditPostViewModel();
        //    var result = controller.Create(viewModel);
        //    controller.ControllerContext = MockHelpers.FakeControllerContext(true);

        //    // Assert
        //    Assert.IsNotNull(result, "View Result is null");
        //    _repository.Verify(x => x.Add(It.IsAny<Post>()), Times.Exactly(1));
        //}
    }
}