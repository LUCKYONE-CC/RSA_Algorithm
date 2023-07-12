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
            // Generiere zwei große Primzahlen p und q
            BigInteger p = GeneratePrimeNumber(2048);
            BigInteger q = GeneratePrimeNumber(2048);

            // Berechne den RSA-Modulus n und die Eulersche Phi-Funktion phi
            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);

            // Generiere eine öffentliche und private Schlüsselkomponente
            BigInteger e = GenerateCoprime(phi); // Öffentlicher Exponent
            BigInteger d = ModInverse(e, phi); // Privater Exponent

            // Erstelle die öffentlichen und privaten Schlüssel als Arrays
            BigInteger[] publicKey = new BigInteger[] { e, n };
            BigInteger[] privateKey = new BigInteger[] { d, n };

            // Klartextnachricht
            string message = "Hallo Welt";

            // Verschlüssele die Nachricht mit dem öffentlichen Schlüssel
            BigInteger[] encryptedMessage = Encrypt(message, publicKey);

            // Entschlüssele die Nachricht mit dem privaten Schlüssel
            string decryptedMessage = Decrypt(encryptedMessage, privateKey);

            // Gib die verschlüsselte Nachricht aus
            Console.WriteLine("Encrypted Message:");
            foreach (BigInteger block in encryptedMessage)
            {
                Console.WriteLine(block);
            }

            // Gib die entschlüsselte Nachricht aus
            Console.WriteLine("Decrypted Message: " + decryptedMessage);
        }

        // Verschlüsselungsfunktion
        static BigInteger[] Encrypt(string message, BigInteger[] publicKey)
        {
            BigInteger e = publicKey[0];
            BigInteger n = publicKey[1];

            int blockSize = (int)Math.Ceiling(BigInteger.Log(n, 256)); // Blockgröße berechnen
            int paddingSize = blockSize - 1; // Padding-Größe berechnen

            byte[] paddedMessage = Encoding.ASCII.GetBytes(message.PadRight(paddingSize, ' ')); // Nachricht padden
            int numBlocks = (int)Math.Ceiling((double)paddedMessage.Length / paddingSize); // Anzahl der Blöcke berechnen

            BigInteger[] encryptedMessage = new BigInteger[numBlocks];

            for (int i = 0; i < numBlocks; i++)
            {
                byte[] block = new byte[paddingSize];
                Array.Copy(paddedMessage, i * paddingSize, block, 0, Math.Min(paddingSize, paddedMessage.Length - i * paddingSize));
                BigInteger numericBlock = new BigInteger(block.Reverse().ToArray()); // Byte-Array umdrehen, um die Reihenfolge beizubehalten
                encryptedMessage[i] = BigInteger.ModPow(numericBlock, e, n); // Block verschlüsseln
            }

            return encryptedMessage;
        }

        // Entschlüsselungsfunktion
        static string Decrypt(BigInteger[] encryptedMessage, BigInteger[] privateKey)
        {
            BigInteger d = privateKey[0];
            BigInteger n = privateKey[1];

            int blockSize = (int)Math.Ceiling(BigInteger.Log(n, 256)); // Blockgröße berechnen
            int paddingSize = blockSize - 1; // Padding-Größe berechnen

            byte[] decryptedBytes = new byte[encryptedMessage.Length * paddingSize];

            for (int i = 0; i < encryptedMessage.Length; i++)
            {
                BigInteger numericBlock = BigInteger.ModPow(encryptedMessage[i], d, n); // Block entschlüsseln
                byte[] blockBytes = numericBlock.ToByteArray();
                Array.Reverse(blockBytes); // Byte-Array umdrehen, um die Reihenfolge wiederherzustellen
                Array.Copy(blockBytes, 0, decryptedBytes, i * paddingSize, Math.Min(paddingSize, blockBytes.Length));
            }

            string decryptedMessage = Encoding.ASCII.GetString(decryptedBytes).TrimEnd(); // Entschlüsselte Bytes in einen String umwandeln
            return decryptedMessage;
        }

        // Berechnet das multiplikative Inverse a^-1 mod m
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

        // Generiert eine zufällige Zahl, die zu n koprim ist
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

        // Überprüft, ob eine Zahl wahrscheinlich prim ist
        static bool IsPrime(BigInteger n, int k = 128)
        {
            // Test, ob n ungerade ist. Aber Vorsicht, 2 ist eine Primzahl!
            if (n == 2 || n == 3)
                return true;
            if (n <= 1 || n % 2 == 0)
                return false;

            // Finde r und s
            int s = 0;
            BigInteger r = n - 1;
            while (r % 2 == 0)
            {
                s++;
                r /= 2;
            }

            // Führe k Tests durch
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

        // Generiert eine Primzahl mit der angegebenen Bitlänge
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

        // Generiert eine zufällige BigInteger mit der angegebenen Bitlänge
        static BigInteger GenerateRandomBigInteger(int bitLength)
        {
            byte[] bytes = new byte[bitLength / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; // Stellt sicher, dass die Zahl positiv ist
            }
            return new BigInteger(bytes);
        }
    }

    // Erweiterungsmethoden für BigInteger
    static class BigIntegerExtensions
    {
        // Gibt die nächste Primzahl zurück, die größer oder gleich dem angegebenen Wert ist
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

        // Überprüft, ob eine Zahl wahrscheinlich prim ist
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

            for (int i = 0; i < 128; i++) // Führe 128 Tests durch
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

        // Generiert eine zufällige BigInteger im Bereich [min, max)
        private static BigInteger RandomBigInteger(BigInteger min, BigInteger max)
        {
            byte[] bytes = max.ToByteArray();
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; // Stellt sicher, dass die Zahl positiv ist
            }
            return new BigInteger(bytes) % (max - min) + min;
        }
    }
}
