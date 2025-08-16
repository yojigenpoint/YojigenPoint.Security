namespace YojigenPoint.Security.Abstractions
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts a plaintext string.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <returns>A Base64 encoded string representing the encrypted data (salt + IV + ciphertext).</returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Decrypts a cipher text string.
        /// </summary>
        /// <param name="cipherText">The Base64 encoded string to decrypt.</param>
        /// <returns>The original plaintext string.</returns>
        string Decrypt(string cipherText);
    }
}
