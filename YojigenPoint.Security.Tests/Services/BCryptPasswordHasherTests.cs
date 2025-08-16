using FluentAssertions;
using YojigenPoint.Security.Services;

namespace YojigenPoint.Security.Tests.Services
{
    public class BCryptPasswordHasherTests
    {
        [Fact]
        public void Hash_ShouldReturnValidHash_ForValidPassword()
        {
            // Arrange
            var hasher = new BCryptPasswordHasher();
            string password = "TestPassword123";

            // Act
            string hash = hasher.Hash(password);

            // Assert
            BCrypt.Net.BCrypt.Verify(password, hash).Should().BeTrue();
        }

        [Theory]
        [InlineData("")] // Empty password
        [InlineData(" ")] // Whitespace password
        [InlineData(null)] // Null password
        public void ShouldThrowException_ForNullOrEmptyPassword(string? psw)
        {
            // Arrange
            var hasher = new BCryptPasswordHasher();

            // Act
            Action act = () => hasher.Hash(psw);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Password cannot be null or empty.*")
                .WithParameterName("password");
        }

        [Fact]
        public void Verify_ShouldReturnTrue_ForCorrectPassword()
        {
            // Arrange
            var hasher = new BCryptPasswordHasher();
            string password = "TestPassword123";
            string hash = hasher.Hash(password);

            // Act
            bool result = hasher.Verify(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_ShouldReturnFalse_ForIncorrectPassword()
        {
            // Arrange
            var hasher = new BCryptPasswordHasher();
            string password = "wrongPassword";
            string hash = hasher.Hash("password123");

            // Act
            bool result = hasher.Verify(password, hash);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Verify_ShouldReturnFalse_ForInvalidHash()
        {
            // Arrange
            var hasher = new BCryptPasswordHasher();
            string password = "password123";
            string invalidHash = "not_a_valid_hash";

            // Act
            bool result = hasher.Verify(password, invalidHash);

            // Assert
            result.Should().BeFalse();
        }
    }
}
