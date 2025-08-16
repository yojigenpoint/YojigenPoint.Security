using FluentAssertions;
using System.Security.Cryptography;
using YojigenPoint.Security.Services;

namespace YojigenPoint.Security.Tests.Services
{
    public class AesEncryptionServiceTests
    {
        [Fact]
        public void Encrypt_Decrypt_ShouldReturnOriginalText()
        {
            // Arrange
            string originalText = "Yojigen Point";
            string key = "secret-key";

            // Act
            var aesService = new AesEncryptionService(key);
            string encryptedText = aesService.Encrypt(originalText);
            string decryptedText = aesService.Decrypt(encryptedText);

            // Assert
            decryptedText.Should().Be(originalText);
        }

        [Fact]
        public void Encrypt_ShouldProduceDifferentResultsForSameInput()
        {
            // Arrange
            string originalText = "test";
            string key = "secret-key";

            // Act
            var aesService = new AesEncryptionService(key);
            string encryptedText1 = aesService.Encrypt(originalText);
            string encryptedText2 = aesService.Encrypt(originalText);

            // Assert
            encryptedText1.Should().NotBe(encryptedText2);
        }

        [Fact]
        public void Decrypt_ShouldThrowException_ForIncorrectKey()
        {
            // Arrange
            string invalidCipherText = "Yojigen Point";
            string keyA = "secret_key1";
            string keyB = "secret_key2";

            // Act
            var aesServiceA = new AesEncryptionService(keyA);
            var aesServiceB = new AesEncryptionService(keyB);
            Action act = () => aesServiceB.Decrypt(aesServiceA.Encrypt(invalidCipherText));

            // Assert
            act.Should().Throw<CryptographicException>()
                .WithMessage("Decryption failed. The data is corrupted or the key is incorrect.");
        }

        [Fact]
        public void Decrypt_ShouldThrowCryptographicException_ForCorruptedData()
        {
            // Arrange
            string originalText = "This is a valid message";
            string key = "secret-key";
            var aesService = new AesEncryptionService(key);

            string validCipherText = aesService.Encrypt(originalText);

            byte[] cipherData = Convert.FromBase64String(validCipherText);

            int byteToTemper = cipherData.Length - 1;
            cipherData[byteToTemper] ^= 0xFF;

            var temperedCipherText = Convert.ToBase64String(cipherData);

            // Act
            Action act = () => aesService.Decrypt(temperedCipherText);

            // Assert
            act.Should().Throw<CryptographicException>();
        }


        [Theory]
        [InlineData("")] // Empty string
        [InlineData(" ")] // Whitespace string
        [InlineData(null)] // Null value
        public void Constructor_ShouldThrowException_ForNullOrEmptyMasterKey(string? masterKey)
        {
            // Act
            Action act = () => new AesEncryptionService(masterKey);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Master key cannot be null or empty. *")
                .WithParameterName(nameof(masterKey));
        }
    }
}
