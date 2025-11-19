using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using FluentAssertions;
using Xunit;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Application.Responses
{
    public class PagedResultTests
    {
        [Fact]
        public void TotalPages_ShouldCalculateCorrectly_WhenTotalCountIsDivisibleByPageSize()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                TotalCount = 20,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.TotalPages.Should().Be(2);
        }

        [Fact]
        public void TotalPages_ShouldRoundUp_WhenTotalCountIsNotDivisibleByPageSize()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                TotalCount = 25,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.TotalPages.Should().Be(3);
        }

        [Fact]
        public void TotalPages_ShouldReturnOne_WhenTotalCountIsLessThanPageSize()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                TotalCount = 5,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.TotalPages.Should().Be(1);
        }

        [Fact]
        public void TotalPages_ShouldReturnZero_WhenTotalCountIsZero()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                TotalCount = 0,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.TotalPages.Should().Be(0);
        }

        [Fact]
        public void HasPreviousPage_ShouldReturnFalse_WhenPageIsOne()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 1
            };

            // Act & Assert
            pagedResult.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public void HasPreviousPage_ShouldReturnTrue_WhenPageIsGreaterThanOne()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 2
            };

            // Act & Assert
            pagedResult.HasPreviousPage.Should().BeTrue();
        }

        [Fact]
        public void HasPreviousPage_ShouldReturnTrue_WhenPageIsThree()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 3
            };

            // Act & Assert
            pagedResult.HasPreviousPage.Should().BeTrue();
        }

        [Fact]
        public void HasNextPage_ShouldReturnFalse_WhenPageIsLastPage()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 2,
                TotalCount = 20,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void HasNextPage_ShouldReturnTrue_WhenPageIsNotLastPage()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 1,
                TotalCount = 20,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void HasNextPage_ShouldReturnFalse_WhenPageIsGreaterThanTotalPages()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 5,
                TotalCount = 20,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void HasNextPage_ShouldReturnFalse_WhenTotalCountIsZero()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 1,
                TotalCount = 0,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void HasNextPage_ShouldReturnTrue_WhenOnFirstPageWithMultiplePages()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 1,
                TotalCount = 25,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.HasNextPage.Should().BeTrue();
            pagedResult.TotalPages.Should().Be(3);
        }

        [Fact]
        public void HasNextPage_ShouldReturnFalse_WhenOnLastPage()
        {
            // Arrange
            var pagedResult = new PagedResult<string>
            {
                Page = 3,
                TotalCount = 25,
                PageSize = 10
            };

            // Act & Assert
            pagedResult.HasNextPage.Should().BeFalse();
            pagedResult.TotalPages.Should().Be(3);
        }
    }
}

