using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shop.Api.Controllers;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.Category;

namespace ShopTest
{
    public class CategoryControllerTests
    {
        [Fact]
        public async Task GetCategoryById_ReturnsOk_WhenCategoryExists()
        {
            var mockService = new Mock<ICategoryService>();
            var mockMediator = new Mock<IMediator>();

            var testCategory = new CategoryReadDTO
            {
                Id = 1,
                Name = "Books"
            };

            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<GetCategoryByIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(testCategory);

            var controller = new CategoryController(
                mockService.Object,
                mediator: mockMediator.Object);

            var result = await controller.GetCategoryById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategory = Assert.IsType<CategoryReadDTO>(okResult.Value);

            Assert.Equal("Books", returnedCategory.Name);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            var mockService = new Mock<ICategoryService>();
            var mockMediator = new Mock<IMediator>();

            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<GetCategoryByIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryReadDTO?)null);

            var controller = new CategoryController(
                mockService.Object,
                mediator: mockMediator.Object);

            var result = await controller.GetCategoryById(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
