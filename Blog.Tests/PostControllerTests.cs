using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Authorization;
using Blog.Controllers;
using Blog.Entities;
using Blog.Hubs;
using Blog.Interfaces;
using Blog.Interfaces.IRepositories;
using Blog.Models.PostViewModels;
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
        private Mock<UserManager<ApplicationUser>> _userManager;
        private Mock<IPostRepository> _postRepository;
        private Mock<IBlogRepository> _blogRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private List<ApplicationUser> _users;
        private IAuthorizationService _authService;
        private IMapper _mapper;
        private Mock<IHubContext<PostNotificationHub>> _hub;
        private Mock<IHubClients> _hubClients;


        [TestInitialize]
        public void SetupContext()
        {
            // User Manager
            _users = new List<ApplicationUser>
            {
                new() { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de", UserName = "user1@bv.com" },
                new() { Id = "c3be76af-87ab-49ee-99e6-1b00aa4bfaf2", UserName = "user2@bv.com" }
            };
            _userManager = MockHelpers.MockUserManager(_users);
            _userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("f42b68b2-3fe3-41d2-a58b-08d980f4d1de");
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.FromResult(new ApplicationUser { Id = "f42b68b2-3fe3-41d2-a58b-08d980f4d1de" }));

            // Repository
            _postRepository = new Mock<IPostRepository>();
            _blogRepository = new Mock<IBlogRepository>();
            _blogRepository.Setup(x => x.GetBlogsByOwnerId(It.IsAny<string>()))
                .ReturnsAsync(new List<Entities.Blog>
                {
                    new()
                    {
                        Id = new Guid("DA22885F-C57E-4590-EB91-08D99B01007A"),
                        Name = "Blog_1",
                        OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                    },
                    new()
                    {
                        Id = new Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                        Name = "Blog_2",
                        OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                    }
                });


            // Mapper
            var realProfile = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            _mapper = new Mapper(configuration);


            // Unit Of Work 
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(uow => uow.Blogs).Returns(_blogRepository.Object);
            _unitOfWork.Setup(uow => uow.Posts).Returns(_postRepository.Object);

            // AuthorizationService
            var authHandler = new UserIsPostOwnerAuthorizationHandler(_userManager.Object, _unitOfWork.Object);
            _authService = MockHelpers
                .BuildAuthorizationService(services =>
                {
                    services.AddScoped<IAuthorizationHandler>(_ => authHandler);
                    services.AddAuthorization();
                });

            // Hub 
            _hub = new Mock<IHubContext<PostNotificationHub>>();
            _hubClients = new Mock<IHubClients>();
            _hub.Setup(hub => hub.Clients).Returns(_hubClients.Object);
        }

        [TestMethod]
        public void Create_ViewModelIsValid_CreateNewPost()
        {
            // Arrange 
            var blogs = new Entities.Blog[]
            {
                new()
                {
                    Id = new Guid("DA22885F-C57E-4590-EB91-08D99B01007A"),
                    Name = "Blog_1"
                },
                new()
                {
                    Id = new Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_2"
                }
            };
            _blogRepository.Setup(r
                    => r.GetBlogsByOwnerId("f42b68b2-3fe3-41d2-a58b-08d980f4d1de"))
                .ReturnsAsync(blogs);
            _blogRepository.Setup(x => x.Get(new Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("DA22885F-C57E-4590-EB91-08D99B01007A"),
                    Name = "Blog_1",
                    Locked = false
                });
            var viewModel = new CreateEditPostViewModel
            {
                Post = new Post
                    { Title = "PostTitle_1", Body = "PostBody_1", BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A" },
                UsersBlogs = new List<Entities.Blog>(),
                Tags = new List<Tag>()
            };

            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            using (var result = controller.Create(viewModel))
            {
                controller.ControllerContext = MockHelpers.FakeControllerContext(true);

                // Assert
                Assert.IsNotNull(result, "View Result is null");
            }

            _postRepository.Verify(x => x.Add(It.IsAny<Post>()), Times.Exactly(1));
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(1));
        }

        [TestMethod]
        public void Create_ModelStateIsNotValid_DoNotCreateNewPost()
        {
            // Arrange 
            var blogs = new Entities.Blog[]
            {
                new()
                {
                    Id = new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A"),
                    Name = "Blog_1"
                },
                new()
                {
                    Id = new System.Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_2"
                }
            };
            _blogRepository.Setup(r
                    => r.GetBlogsByOwnerId("f42b68b2-3fe3-41d2-a58b-08d980f4d1de"))
                .ReturnsAsync(blogs);
            _blogRepository.Setup(x => x.Get(new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A"),
                    Name = "Blog_1",
                    Locked = false
                });

            var viewModel = new CreateEditPostViewModel
            {
                Post = new Post
                    { Title = "PostTitle_1", Body = "PostBody_1", BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A" },
                UsersBlogs = new List<Entities.Blog>(),
                Tags = new List<Tag>()
            };


            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            controller.ModelState.AddModelError("", "dummy error message");

            // Act 
            var result = controller.Create(viewModel);

            // Assert
            _postRepository.Verify(x => x.Add(It.IsAny<Post>()), Times.Exactly(0));
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(0));
        }

        [TestMethod]
        public void Create_BlogToWhichThePostBelongsIsLocked_DoNotCreateNewPost()
        {
            // Arrange 
            var blogs = new Entities.Blog[]
            {
                new()
                {
                    Id = new Guid("DA22885F-C57E-4590-EB91-08D99B01007A"),
                    Name = "Blog_1"
                },
                new()
                {
                    Id = new Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_2"
                }
            };
            _blogRepository.Setup(r
                    => r.GetBlogsByOwnerId("f42b68b2-3fe3-41d2-a58b-08d980f4d1de"))
                .ReturnsAsync(blogs);
            _blogRepository.Setup(x => x.Get(new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A"),
                    Name = "Blog_1",
                    Locked = true
                });

            var viewModel = new CreateEditPostViewModel
            {
                Post = new Post
                    { Title = "PostTitle_1", Body = "PostBody_1", BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A" },
                UsersBlogs = new List<Entities.Blog>(),
                Tags = new List<Tag>()
            };


            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            var result = controller.Create(viewModel);

            Assert.IsNotNull(result, "View Result is null");
            _postRepository.Verify(x => x.Add(It.IsAny<Post>()), Times.Exactly(0));
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(0));
        }

        [TestMethod]
        public void Edit_WhenCalledWithStringId_ReturnsView()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("6AD3D899-482E-4DD5-E75D-08D99AB50C36"),
                    Title = "PostTitle",
                    Body = "PostBody",
                    BlogId = "249DC88D-529F-4F16-A592-08D99AB4F35E"
                });

            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            )
            {
                ControllerContext = MockHelpers.FakeControllerContext(true)
            };

            // Act 
            var result = controller.Edit("6AD3D899-482E-4DD5-E75D-08D99AB50C36");


            Assert.IsNotNull(result, "View Result is null");
        }

        [TestMethod]
        public void Edit_WhenCalledWithValidViewModel_EditThePost()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_2",
                    Body = "PostBody_2",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });
            _blogRepository.Setup(x => x.Get(new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_1",
                    Locked = false
                });

            var viewModel = new CreateEditPostViewModel
            {
                Post = new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_1",
                    Body = "PostBody_1",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                },
                UsersBlogs = new List<Entities.Blog>(),
                Tags = new List<Tag>()
            };
            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            var result = controller.Edit(viewModel);

            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(1));
        }

        [TestMethod]
        public void Edit_WhenModelStateIsInvalid_DoNotEditThePost()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_2",
                    Body = "PostBody_2",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });
            _blogRepository.Setup(x => x.Get(new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new System.Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_1",
                    Locked = false
                });

            var viewModel = new CreateEditPostViewModel
            {
                Post = new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_1",
                    Body = "PostBody_1",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                },
                UsersBlogs = new List<Entities.Blog>(),
                Tags = new List<Tag>()
            };
            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            controller.ModelState.AddModelError("", "dummy error message");

            // Act 
            var result = controller.Edit(viewModel);

            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(0));
        }

        [TestMethod]
        public void Edit_WhenPostOwnerIsNotTheSameAsUserFromTheContext_DoNotEditThePost()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_2",
                    Body = "PostBody_2",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("213A857B-946F-4473-5F51-08D99B2B672B")
                });
            _blogRepository.Setup(x => x.Get(new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new System.Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_1",
                    Locked = false
                });

            var viewModel = new CreateEditPostViewModel
            {
                Post = new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_1",
                    Body = "PostBody_1",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("213A857B-946F-4473-5F51-08D99B2B672B")
                },
                UsersBlogs = new List<Entities.Blog>(),
                Tags = new List<Tag>()
            };
            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            var result = controller.Edit(viewModel);

            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(0));
        }

        [TestMethod]
        public void Edit_WhenPostOwnerIsTheSameAsUserFromTheContext_EditThePost()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_2",
                    Body = "PostBody_2",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });
            _blogRepository.Setup(x => x.Get(new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new System.Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_1",
                    Locked = false
                });

            var viewModel = new CreateEditPostViewModel
            {
                Post = new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_1",
                    Body = "PostBody_1",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                },
                UsersBlogs = new List<Entities.Blog>(),
                Tags = new List<Tag>()
            };
            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            var result = controller.Edit(viewModel);

            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(1));
        }

        [TestMethod]
        public void Delete_WhenCalledWithIdString_ReturnsView()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_2",
                    Body = "PostBody_2",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });
            _blogRepository.Setup(x => x.Get(new Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new System.Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_1",
                    Locked = false
                });

            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            var result = controller.Delete("E864E9DA-14A8-4FC4-226A-08D99B23A726");
            
            // Assert
            Assert.IsNotNull(result, "View Result is null");
        }

        [TestMethod]
        public void Delete_WhenPostOwnerIsTheSameAsLoggedInUserAndBlogIsNotLocked_DeletePost()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_2",
                    Body = "PostBody_2",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });
            _blogRepository.Setup(x => x.Get(new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_1",
                    Locked = false
                });

            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            var result = controller.Delete("E864E9DA-14A8-4FC4-226A-08D99B23A726");

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _postRepository.Verify(x => x.Remove(It.IsAny<Post>()), Times.Exactly(1));
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(1));
        }


        [TestMethod]
        public void Delete_WhenPostOwnerIsNotTheSameAsLoggedInUser_DoNotDeletePost()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_2",
                    Body = "PostBody_2",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f40000")
                });
            _blogRepository.Setup(x => x.Get(new Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_1",
                    Locked = false
                });

            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            var result = controller.Delete("E864E9DA-14A8-4FC4-226A-08D99B23A726");

            // Assert
            Assert.IsNotNull(result, "View Result is null");
            _postRepository.Verify(x => x.Remove(It.IsAny<Post>()), Times.Exactly(0));
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(0));
        }

        [TestMethod]
        public void Delete_WhenBlogToWhichThePostBelongIsLocked_DoNotDeletePost()
        {
            // Arrange 
            _postRepository.Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(new Post
                {
                    Id = new Guid("E864E9DA-14A8-4FC4-226A-08D99B23A726"),
                    Title = "PostTitle_2",
                    Body = "PostBody_2",
                    BlogId = "DA22885F-C57E-4590-EB91-08D99B01007A",
                    OwnerId = new Guid("f42b68b2-3fe3-41d2-a58b-08d980f4d1de")
                });
            _blogRepository.Setup(x => x.Get(new System.Guid("DA22885F-C57E-4590-EB91-08D99B01007A")))
                .Returns(new Entities.Blog
                {
                    Id = new Guid("8974F051-2DC7-4558-326F-08D99B185C8C"),
                    Name = "Blog_1",
                    Locked = true
                });

            var controller = new PostController
            (
                _unitOfWork.Object,
                _authService,
                _userManager.Object,
                _mapper,
                _hub.Object
            );
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            // Act 
            using (var result = controller.Delete("E864E9DA-14A8-4FC4-226A-08D99B23A726"))
            {
                Assert.IsNotNull(result, "View Result is null");
            }


            _postRepository.Verify(x => x.Remove(It.IsAny<Post>()), Times.Exactly(0));
            _unitOfWork.Verify(x => x.Complete(), Times.Exactly(0));
        }
    }
}