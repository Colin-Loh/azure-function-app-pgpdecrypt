using PgpCore;
using System;
using System.IO;
using Kurukuru;
using System.Text;

namespace PGPEncryptConsoleApp
{
    class Program
    {
        static void Main()
        {
            using (PGP pgp = new PGP())
            {
                Console.WriteLine($"Welcome to PGPEncryptConsoleApp!");

                string basePath = "PlaceholderToFilesPathLocationToEncrypt";  // Directly use your base path
                string publicKeyFilePath = Path.Combine(basePath, "public.asc");
                string publicKeyBase64FilePath = Path.Combine(basePath, "public_base64.asc");
                string privateKeyFilePath = Path.Combine(basePath, "private.asc");
                string privateKeyBase64FilePath = Path.Combine(basePath, "private_base64.asc");
                string contentFilePath = Path.Combine(basePath, "PlaceholderToFilesToEncrypt.csv");  // Your CSV file path
                string encryptedFilePath = Path.Combine(basePath, "PlaceholderToEncryptedFile.pgp");
                string decryptedFilePath = Path.Combine(basePath, "PlaceholderToDecryptedFile.csv");
                string username = null;
                int strength = 4096;
                int certainty = 8;

                // Check if CSV exists
                if (!File.Exists(contentFilePath))
                {
                    Console.WriteLine($"CSV file not found at: {contentFilePath}");
                    return;
                }

                Console.WriteLine($"Found CSV file: {contentFilePath}");

                // Create a password
                Console.WriteLine($"Enter a passphrase or press enter to not use a password");
                string password = ReadLine.ReadPassword();
                Console.WriteLine($"Your passphrase is: {password}");

                // Generate keys
                Spinner.Start("Generating keys...", () =>
                {
                    pgp.GenerateKey(publicKeyFilePath, privateKeyFilePath, username, password, strength, certainty);
                });

                string publicKey = File.ReadAllText(publicKeyFilePath);
                File.WriteAllText(publicKeyBase64FilePath, Convert.ToBase64String(Encoding.UTF8.GetBytes(publicKey)));
                Console.WriteLine($"Created public key: {publicKeyFilePath}");
                Console.WriteLine($"Converted public key to base64: {publicKeyBase64FilePath}");

                Console.WriteLine($"Created private key: {privateKeyFilePath}");
                string privateKey = File.ReadAllText(privateKeyFilePath);
                File.WriteAllText(privateKeyBase64FilePath, Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKey)));
                Console.WriteLine($"Created private key: {privateKeyFilePath}");
                Console.WriteLine($"Converted private key to base64: {privateKeyBase64FilePath}");

                // Encrypt file
                pgp.EncryptFile(contentFilePath, encryptedFilePath, publicKeyFilePath, true, true);
                Console.WriteLine($"Encrypted CSV file: {encryptedFilePath}");

                // Decrypt file (optional if you want to test decryption)
                pgp.DecryptFile(encryptedFilePath, decryptedFilePath, privateKeyFilePath, password);
                Console.WriteLine($"Decrypted CSV file: {decryptedFilePath}");

                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
            }
        }
    }
}
