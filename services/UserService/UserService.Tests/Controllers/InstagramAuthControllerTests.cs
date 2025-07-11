using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserService.API.Controllers;
using UserService.Application.DTOs;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using Xunit;

namespace UserService.Tests.Controllers;

public class InstagramAuthControllerTests
{
    [Fact]
    public async Task Callback_Should_SaveTokenAndReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var code = "fake_code";
        var state = userId.ToString();
        var tokenResponse = new InstagramTokenResponseDto { AccessToken = "access_token", UserId = "insta_user_id" };

        var mockAuthService = new Mock<IInstagramAuthService>();
        mockAuthService.Setup(s => s.ExchangeCodeForTokenAsync(code, It.IsAny<string>())).ReturnsAsync(tokenResponse);

        var mockRepo = new Mock<ISocialAccountRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<SocialAccount>())).Returns(Task.CompletedTask).Verifiable();

        var controller = new InstagramAuthController(mockAuthService.Object, mockRepo.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.Request.Scheme = "https";
        controller.ControllerContext.HttpContext.Request.Host = new Microsoft.AspNetCore.Http.HostString("localhost");

        var dto = new InstagramAuthCallbackDto { Code = code, State = state };

        // Act
        var result = await controller.Callback(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedToken = Assert.IsType<InstagramTokenResponseDto>(okResult.Value);
        Assert.Equal("access_token", returnedToken.AccessToken);
        mockRepo.Verify(r => r.AddAsync(It.Is<SocialAccount>(a => a.UserId == userId && a.AccessToken == "access_token")), Times.Once);
    }
}
