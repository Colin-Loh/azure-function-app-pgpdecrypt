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
            string privateKey = System.Environment.GetEnvironmentVariable("pgpkey");
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

                    log.LogInformation($"Decrypted blob: {name}");
                }
            }
        }
    }
}