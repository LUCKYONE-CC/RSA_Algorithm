using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
namespace RSA_Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            // Primes p and q
            BigInteger p = GeneratePrimeNumber(2048);
            BigInteger q = GeneratePrimeNumber(2048);

            // Calc RSA-Modulus n and phi
            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);

            BigInteger e = GenerateCoprime(phi); // Pub exp
            BigInteger d = ModInverse(e, phi); // Priv exp

            BigInteger[] publicKey = new BigInteger[] { e, n };
            BigInteger[] privateKey = new BigInteger[] { d, n };

            string message = "Hallo Welt";

            BigInteger[] encryptedMessage = Encrypt(message, publicKey);

            string decryptedMessage = Decrypt(encryptedMessage, privateKey);

            Console.WriteLine("Encrypted Message:");
            foreach (BigInteger block in encryptedMessage)
            {
                Console.WriteLine(block);
            }

            Console.WriteLine("Decrypted Message: " + decryptedMessage);
        }

        static BigInteger[] Encrypt(string message, BigInteger[] publicKey)
        {
            BigInteger e = publicKey[0];
            BigInteger n = publicKey[1];

            int blockSize = (int)Math.Ceiling(BigInteger.Log(n, 256));
            int paddingSize = blockSize - 1;

            byte[] paddedMessage = Encoding.ASCII.GetBytes(message.PadRight(paddingSize, ' ')); 
            int numBlocks = (int)Math.Ceiling((double)paddedMessage.Length / paddingSize);

            BigInteger[] encryptedMessage = new BigInteger[numBlocks];

            for (int i = 0; i < numBlocks; i++)
            {
                byte[] block = new byte[paddingSize];
                Array.Copy(paddedMessage, i * paddingSize, block, 0, Math.Min(paddingSize, paddedMessage.Length - i * paddingSize));
                BigInteger numericBlock = new BigInteger(block.Reverse().ToArray()); 
                encryptedMessage[i] = BigInteger.ModPow(numericBlock, e, n); 
            }

            return encryptedMessage;
        }

        static string Decrypt(BigInteger[] encryptedMessage, BigInteger[] privateKey)
        {
            BigInteger d = privateKey[0];
            BigInteger n = privateKey[1];

            int blockSize = (int)Math.Ceiling(BigInteger.Log(n, 256)); 
            int paddingSize = blockSize - 1;

            byte[] decryptedBytes = new byte[encryptedMessage.Length * paddingSize];

            for (int i = 0; i < encryptedMessage.Length; i++)
            {
                BigInteger numericBlock = BigInteger.ModPow(encryptedMessage[i], d, n); 
                byte[] blockBytes = numericBlock.ToByteArray();
                Array.Reverse(blockBytes); 
                Array.Copy(blockBytes, 0, decryptedBytes, i * paddingSize, Math.Min(paddingSize, blockBytes.Length));
            }

            string decryptedMessage = Encoding.ASCII.GetString(decryptedBytes).TrimEnd();
            return decryptedMessage;
        }

        // Inverse a^-1 mod m
        static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger y = 0, x = 1;

            if (m == 1)
                return 0;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;

                m = a % m;
                a = t;
                t = y;

                y = x - q * y;
                x = t;
            }

            if (x < 0)
                x += m0;

            return x;
        }

        // generate num that is koprim to n
        static BigInteger GenerateCoprime(BigInteger n)
        {
            BigInteger coprime = 0;
            for (BigInteger i = 2; i < n; i++)
            {
                if (BigInteger.GreatestCommonDivisor(i, n) == 1)
                {
                    coprime = i;
                    break;
                }
            }
            return coprime;
        }

        static bool IsPrime(BigInteger n, int k = 128)
        {
            if (n == 2 || n == 3)
                return true;
            if (n <= 1 || n % 2 == 0)
                return false;

            int s = 0;
            BigInteger r = n - 1;
            while (r % 2 == 0)
            {
                s++;
                r /= 2;
            }

            for (int i = 0; i < k; i++)
            {
                RandomNumberGenerator rng = RandomNumberGenerator.Create();
                byte[] bytes = new byte[n.ToByteArray().LongLength];
                BigInteger a;
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a < 2 || a >= n - 2);

                BigInteger x = BigInteger.ModPow(a, r, n);

                if (x != 1 && x != n - 1)
                {
                    int j = 1;
                    while (j < s && x != n - 1)
                    {
                        x = BigInteger.ModPow(x, 2, n);
                        if (x == 1)
                            return false;
                        j++;
                    }
                    if (x != n - 1)
                        return false;
                }
            }
            return true;
        }

        static BigInteger GeneratePrimeNumber(int bitLength)
        {
            BigInteger p;
            do
            {
                p = GenerateRandomBigInteger(bitLength);
                p = BigIntegerExtensions.NextPrime(p);
            } while (!IsPrime(p));

            return p;
        }

        static BigInteger GenerateRandomBigInteger(int bitLength)
        {
            byte[] bytes = new byte[bitLength / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; 
            }
            return new BigInteger(bytes);
        }
    }

    static class BigIntegerExtensions
    {
        public static BigInteger NextPrime(BigInteger start)
        {
            BigInteger prime = start;
            while (true)
            {
                if (IsPrime(prime))
                    return prime;
                prime++;
            }
        }

        private static bool IsPrime(BigInteger n)
        {
            if (n == 2 || n == 3)
                return true;
            if (n <= 1 || n % 2 == 0)
                return false;

            BigInteger d = n - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            for (int i = 0; i < 128; i++)
            {
                BigInteger a = RandomBigInteger(2, n - 2);
                BigInteger x = BigInteger.ModPow(a, d, n);

                if (x == 1 || x == n - 1)
                    continue;

                bool isProbablyPrime = false;
                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                    {
                        isProbablyPrime = true;
                        break;
                    }
                }

                if (!isProbablyPrime)
                    return false;
            }

            return true;
        }

        private static BigInteger RandomBigInteger(BigInteger min, BigInteger max)
        {
            byte[] bytes = max.ToByteArray();
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F;
            }
            return new BigInteger(bytes) % (max - min) + min;
        }
    }
}
