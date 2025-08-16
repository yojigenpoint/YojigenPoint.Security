# YojigenPoint.Security

[![MIT License](https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge)](https://choosealicense.com/licenses/mit/)
[![Nuget](https://img.shields.io/nuget/v/YojigenPoint.Security?style=for-the-badge&color=purple)](https://www.nuget.org/packages/YojigenPoint.Security/)

**YojigenPoint.Security** is a modern, professional security library for .NET, providing robust and easy-to-use services for common cryptographic operations. It is designed with a focus on security best practices, performance, and testability.

This library abstracts away complex cryptographic implementations behind simple interfaces, allowing developers to secure their applications without needing to be security experts.

## Features

-   **Password Hashing:** A service-based implementation of the industry-standard **BCrypt** algorithm for securely hashing and verifying passwords.
-   **Symmetric Encryption:** A robust implementation of **AES-256** encryption. It correctly uses a **PBKDF2-derived key**, a **random salt**, and a **random IV** for every encryption operation, following modern security best practices.
-   **Dependency Injection Ready:** All services are exposed via interfaces (`IPasswordHasher`, `IEncryptionService`) for easy integration into modern .NET applications using DI containers.

## Installation

This library is distributed as a NuGet package. You can add it to your project using the .NET CLI:

```bash
dotnet add package YojigenPoint.Security
```


## Usage
### 1. Registering the Services
In your Program.cs or startup configuration, register the services with your DI container.
```
using YojigenPoint.Security.Abstractions;
using YojigenPoint.Security.Services;

// ...

// The Password Hasher is stateless, so it can be a Singleton.
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

// The Encryption Service requires a master key, which should be loaded from a secure configuration source.
var encryptionKey = builder.Configuration["EncryptionMasterKey"];
builder.Services.AddSingleton<IEncryptionService>(new AesEncryptionService(encryptionKey));
```

### 2. Using the Services in Your Application
Inject the interfaces into your classes via the constructor.
```
public class MyUserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEncryptionService _encryptionService;

    public MyUserService(IPasswordHasher passwordHasher, IEncryptionService encryptionService)
    {
        _passwordHasher = passwordHasher;
        _encryptionService = encryptionService;
    }

    public void RegisterUser(string password, string personalData)
    {
        // Hash the password for storage
        string passwordHash = _passwordHasher.Hash(password);

        // Encrypt sensitive data
        string encryptedData = _encryptionService.Encrypt(personalData);

        // ... save to database ...
    }
}
```

## License
This project is licensed under the **MIT License**. See the [LICENSE.md](https://github.com/yojigenpoint/YojigenPoint.Security/blob/master/README.md) file for details.

