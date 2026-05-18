namespace FlowProtocol2.UnitTests
{
    using FlowProtocol2.Commands;
    using FlowProtocol2.Core;
    using Xunit;
    
    public class ErrorElementTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var errorCode = 404;
            var errorMessage = "Not Found";
            var readcontext = new ReadContext("", 0, 0, "");

            // Act
            var errorElement = new ErrorElement(readcontext, errorCode.ToString(), errorMessage);

            // Assert
            Assert.Equal(errorCode.ToString(), errorElement.ErrorCode);
            Assert.Equal(errorMessage, errorElement.ErrorText);
            Assert.Equal(readcontext, errorElement.ReadContext);
        }

        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var readcontext = new ReadContext("", 0, 0, "");
            var errorCode = 500;
            var errorMessage = "Internal Server Error";
            var errorElement = new ErrorElement(readcontext, errorCode.ToString(), errorMessage);
            var expectedString = $"Error {errorCode}: Internal Server Error";

            // Act
            var result = errorElement.ToString();

            // Assert
            Assert.Equal(expectedString, result);
        }

        [Fact]
        public void Equals_ShouldReturnTrueForSameErrorCodeAndMessage()
        {
            // Arrange
            var errorElement1 = new ErrorElement(new ReadContext("", 0, 0, ""), "Bad Request", string.Empty);
            var errorElement2 = new ErrorElement(new ReadContext("", 0, 0, ""), "Bad Request", string.Empty);

            // Act
            var result = errorElement1.Equals(errorElement2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldHaveSameHashCodeForIdenticalErrorElements()
        {
            // Arrange
            var errorElement1 = new ErrorElement(new ReadContext("", 0, 0, ""), "Bad Request", string.Empty);
            var errorElement2 = new ErrorElement(new ReadContext("", 0, 0, ""), "Bad Request", string.Empty);

            // Act
            var result = errorElement1.GetHashCode() == errorElement2.GetHashCode();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_ShouldReturnFalseForDifferentErrorCodeOrMessage()
        {
            // Arrange
            var errorElement1 = new ErrorElement(new ReadContext("", 0, 0, ""), "Bad Request", string.Empty);
            var errorElement2 = new ErrorElement(new ReadContext("", 0, 0, ""), "Unauthorized", string.Empty);

            // Act
            var result = errorElement1.Equals(errorElement2);

            // Assert
            Assert.False(result);
        }
    }
}