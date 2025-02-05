using FlowProtocol2.Commands;
using FlowProtocol2.Core;
using NUnit.Framework;

namespace FlowProtocol2.UnitTests
{
    [TestFixture]
    public class ErrorElementTests
    {
        [Test]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var errorCode = 404;
            var errorMessage = "Not Found";

            var readcontext = new  ReadContext("", 0, 0, "");

            // Act
            var errorElement = new ErrorElement(readcontext, errorMessage, string.Empty);

            // Assert
            Assert.That(errorCode, Is.EqualTo(errorElement.ErrorCode));
            Assert.That(errorMessage, Is.EqualTo(errorElement.ErrorText));
        }

        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var readcontext = new  ReadContext("", 0, 0, "");
            var errorCode = 500;
            var errorMessage = "Internal Server Error";
            var errorElement = new ErrorElement(readcontext, errorMessage, string.Empty);
            var expectedString = $"Error {errorCode}: Internal Server Error";

            // Act
            var result = errorElement.ToString();

            // Assert
            Assert.That(expectedString, Is.EqualTo(result));
        }

        [Test]
        public void Equals_ShouldReturnTrueForSameErrorCodeAndMessage()
        {
            // Arrange
            var readcontext = new  ReadContext("", 0, 0, "");
            var errorCode = 500;

            var errorElement1 = new ErrorElement(readcontext, errorCode.ToString(), "Bad Request");
            var errorElement2 = new ErrorElement(readcontext, errorCode.ToString(), "Bad Request");

            // Act
            var result = errorElement1.Equals(errorElement2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_ShouldReturnFalseForDifferentErrorCodeOrMessage()
        {            
            // Arrange
            var readcontext = new  ReadContext("", 0, 0, "");
            var errorCode400 = 400;
            var errorCode401 = 401;
            var errorElement1 = new ErrorElement(readcontext, errorCode400.ToString(), "Bad Request");
            var errorElement2 = new ErrorElement(readcontext, errorCode401.ToString(), "Unauthorized");

            // Act
            var result = errorElement1.Equals(errorElement2);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}