using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using FluentAssertions;
using Xunit;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Application.Requests
{
    public class GetAvisosRequestTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaultValues_WhenNoParameters()
        {
            // Act
            var request = new GetAvisosRequest();

            // Assert
            request.Page.Should().Be(1);
            request.PageSize.Should().Be(10);
        }

        [Fact]
        public void Constructor_ShouldSetValues_WhenValidParameters()
        {
            // Act
            var request = new GetAvisosRequest(2, 20);

            // Assert
            request.Page.Should().Be(2);
            request.PageSize.Should().Be(20);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(-1, 10)]
        [InlineData(-5, 10)]
        public void Constructor_ShouldNormalizePage_WhenPageIsInvalid(int invalidPage, int pageSize)
        {
            // Act
            var request = new GetAvisosRequest(invalidPage, pageSize);

            // Assert
            request.Page.Should().Be(1);
            request.PageSize.Should().Be(pageSize);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        [InlineData(1, 101)]
        [InlineData(1, 200)]
        public void Constructor_ShouldNormalizePageSize_WhenPageSizeIsInvalid(int page, int invalidPageSize)
        {
            // Act
            var request = new GetAvisosRequest(page, invalidPageSize);

            // Assert
            request.Page.Should().Be(page);
            request.PageSize.Should().Be(10);
        }

        [Fact]
        public void Constructor_ShouldAcceptMaxPageSize()
        {
            // Act
            var request = new GetAvisosRequest(1, 100);

            // Assert
            request.Page.Should().Be(1);
            request.PageSize.Should().Be(100);
        }
    }
}

