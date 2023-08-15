using System.IO;
using Microsoft.Azure.WebJobs;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using PgpCore;
using System.Text;
using System.Threading.Tasks;


namespace PGPEncryptFunctionApp
{
    public static class BlobTrigger1
    {
        [FunctionName("DecryptBlob")]
        public static async Task Run(
            [BlobTrigger("landingzone/{name}.pgp", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name,
            ILogger log)
        {
            
            if (myBlob.Length == 0)
            {
                log.LogInformation($"Blob {name} is empty");
                return;
            }

            log.LogInformation($"C# Blob trigger function processed blob\n Name: {name} \n Size: {myBlob.Length} Bytes");

            // NOTE: The privateKey variable is just a placeholder. You'd ideally get this from a secure source like Azure Key Vault.
            string privateKey = "-----BEGIN PGP PRIVATE KEY BLOCK-----\nVersion: BCPG C# v1.9.0.0\n\n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\n=Kz+x\n-----END PGP PRIVATE KEY BLOCK-----\n"; 
            // string privateKey = System.Environment.GetEnvironmentVariable("pgpkey");
            string password = System.Environment.GetEnvironmentVariable("pgppassphrase");

            string connectionString = System.Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            BlobContainerClient containerClient = new BlobContainerClient(connectionString, "landingzone");
            BlobClient blobClient = containerClient.GetBlobClient($"{name}.csv");

            using (PGP pgp = new PGP())
            {
                // Convert the privateKeyContent to a MemoryStream for decryption
                using (MemoryStream privateKeyStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(privateKey)))
                using (MemoryStream decryptedStream = new MemoryStream())
                {
                    pgp.DecryptStream(myBlob, decryptedStream, privateKeyStream, password);
                    decryptedStream.Position = 0;  // Reset the position to the start of the stream for uploading

                    // Use BlobClient to upload the decrypted content
                    await blobClient.UploadAsync(decryptedStream, overwrite: true);

                    log.LogInformation($"Decrypted blob saved to: {name}.decrypted");
                }
            }
        }
    }
}