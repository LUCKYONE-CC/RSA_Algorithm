# RSA Algorithm

This project implements the RSA (Rivest-Shamir-Adleman) encryption algorithm in C#. It generates a pair of public and private keys, encrypts and decrypts a message using these keys.

**Please note that this implementation is provided for educational purposes only. It may not cover all aspects and security considerations of a real-world RSA implementation. Therefore, it should not be used in production systems or to handle sensitive information.**

## Getting Started

To run the project, follow these steps:

    1. Ensure you have the .NET Framework installed on your machine.
    2. Clone or download the project files.
    3. Open the project in your preferred C# development environment.
    4. Build and run the project.

## How It Works

The RSA algorithm works as follows:

    1. Two large prime numbers, `p` and `q`, are generated.
    2. The RSA modulus `n` and Euler's totient function `phi` are calculated using `p` and `q`.
    3. Public and private key components, `e` and `d`, are generated.
    4. The public key (`e`, `n`) and private key (`d`, `n`) are created as arrays.
    5. A plaintext message is chosen.
    6. The message is encrypted using the public key.
    7. The encrypted message is decrypted using the private key.
    8. The encrypted and decrypted messages are displayed.

## Key Generation

The `GeneratePrimeNumber` method generates a prime number of the specified bit length using a random number generator and the `IsPrime` method. The `IsPrime` method uses the Miller-Rabin primality test to determine if a number is probably prime.

## Encryption and Decryption

The `Encrypt` method takes a message string and a public key (`e`, `n`) and performs RSA encryption. It pads the message, breaks it into blocks, and encrypts each block using modular exponentiation.

The Decrypt method takes an encrypted message and a private key (`d`, `n`) and performs RSA decryption. It reverses the encryption process by decrypting each block and reconstructing the original message.

## Helper Methods

The `ModInverse` method calculates the modular inverse of a number `a` modulo `m` using the extended Euclidean algorithm.

The `GenerateCoprime` method generates a random coprime number to a given number `n` by iterating through possible values and checking their greatest common divisor.

The `BigIntegerExtensions` class provides extension methods for the `BigInteger` class. It includes the `NextPrime` method, which returns the next prime number greater than or equal to a given value, and the `IsPrime` method, which performs a probabilistic primality test.
Conclusion

The RSA algorithm is a widely used encryption algorithm that provides secure communication over insecure channels. This project demonstrates the implementation of RSA encryption and decryption in C#, allowing you to understand the fundamental concepts and mechanisms behind this cryptographic algorithm. Feel free to explore and modify the code to suit your needs.

## Disclaimer and Security Considerations

The RSA algorithm is a complex cryptographic system, and its security depends on various factors, including key generation, key management, and implementation details. This project aims to demonstrate the basic principles of RSA encryption but may not provide the same level of security as professional cryptographic libraries or frameworks.

It's important to consider the following points:
1. **Key Generation**: This project uses probabilistic tests to generate prime numbers. While these tests provide a high probability of generating prime numbers, they are not foolproof. For robust key generation, it is recommended to use well-established libraries or consult cryptographic experts.
2. **Security Auditing**: The implementation provided here has not undergone a comprehensive security audit. It may contain vulnerabilities or weaknesses that could be exploited by attackers. For real-world applications, it is crucial to rely on professionally audited and regularly updated cryptographic libraries.
3. **Side-Channel Attacks**:: The implementation does not address side-channel attacks, such as timing attacks or power analysis attacks. These attacks exploit information leaked during the execution of cryptographic algorithms. Mitigating side-channel attacks requires additional countermeasures at the hardware and software levels.
4. **Secure Key Storage**: The management and protection of private keys are critical to the security of RSA. In this project, the keys are stored in memory variables, which may not provide sufficient protection against unauthorized access. In practice, private keys should be stored securely using proper key management techniques.
5. **Cryptographic Strength**: The bit lengths used in this project (2048 bits) are considered secure for most applications. However, the security requirements may vary depending on the specific use case. It is important to consult cryptographic guidelines and standards to determine the appropriate key sizes for your application.
6. **Algorithm Updates**: Cryptographic algorithms are subject to ongoing research and advancements. New attacks or vulnerabilities may be discovered over time. Therefore, it is important to stay informed about the latest developments and updates in the field of cryptography and update the implementation accordingly.

Remember, cryptography is a complex field, and implementing cryptographic algorithms correctly requires deep expertise and careful consideration of various security aspects. If you need to use RSA encryption in a real-world application, it is strongly recommended to rely on established cryptographic libraries or consult with cryptography experts to ensure the security and integrity of your system.

## Conclusion
The RSA algorithm is a widely used encryption algorithm that provides secure communication over insecure channels. This project demonstrates the implementation of RSA encryption and decryption in C#, allowing you to understand the fundamental concepts and mechanisms behind this cryptographic algorithm. However, it is essential to be aware of the limitations and potential risks associated with this implementation.

Always prioritize the security of your systems and consider seeking professional advice when implementing cryptographic algorithms in production environments.

## References
https://medium.com/@ntnprdhmm/how-to-generate-big-prime-numbers-miller-rabin-49e6e6af32fb
<br>
https://studyflix.de/informatik/rsa-verschlusselung-1608
