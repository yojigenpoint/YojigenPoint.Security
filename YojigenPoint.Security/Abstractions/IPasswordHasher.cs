namespace YojigenPoint.Security.Abstractions
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a plain-text password.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>The resulting password hash.</returns>
        string Hash(string password);

        /// <summary>
        /// Verifies that a plain-text password matches a given hash.
        /// </summary>
        /// <param name="password">The plain-text password to check.</param>
        /// <param name="hash">The hash to check against.</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        bool Verify(string password, string hash);
    }
}
