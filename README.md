# C# Function App for PGP Decryption

## Overview

This repository contains a Function App written in C# that leverages PGP (Pretty Good Privacy) to decrypt data in Azure's serverless environment. 

### What is PGP?

PGP (Pretty Good Privacy) is a data encryption and decryption program that provides cryptographic privacy and authentication. It's widely used for securing emails, files, and more. It employs a combination of symmetric and asymmetric cryptography to ensure the security and confidentiality of the data.


### How Does PGP Works? 

PGP work based on asymmetric encryption (uses a pair of keys):

- *Public Key*: This is used to encrypt data. It is shared publicly so that anyone can send you an encrypted message or file.
- *Private Key*: This is kept as a secret by the owner, it is used to decrypt the message or file of the encrypted file using the public key. 


### Use Case

User A wants to send an encrypted message or file to User B. They would use the public key that User B provide to encrypt the message or file. Once encrypted, only the person with the corresponding private key (User B) can decrypt it. 

#### User A Encryption: 
```csharp
pgp.EncryptStream(myBlob, encryptedStream, publicKeyStream, true, true);
```
Here's a breakdown of the parameters:

- myBlob: The input stream containing the encrypted data.
- encryptedStream: The path where the encrypted file should be saved.
- publicKeyStream: The path to the recipient's public key file which will be used for encryption.
- true: This indicates the encryption should use ASCII armor, making the output human-readable. If it's set to true, the encryption result is ASCII armored.
- true: This signifies whether to use integrity check. If it's set to true, an integrity check packet is added to the encrypted data. (Protect encrypted data from manipulation / tampering)

#### User B Decryption: 
```csharp
pgp.DecryptStream(myBlob, decryptedStream, privateKeyStream, password);
```

Here's a breakdown of the parameters:

- myBlob: The input stream containing the encrypted data.
- decryptedStream: The output stream where the decrypted data will be written.
- privateKeyStream: The stream containing the user's private key.
- password: The passphrase associated with the private key.


Note that we can also use *Passphrase Protection* to add an additional later of security to decrypt the file using the Private Key. 

## Code Explanation

#### Function Trigger:

```csharp
public static async Task Run(
    [BlobTrigger("landingzone/{name}.pgp", Connection = "AzureWebJobsStorage")] Stream myBlob,
    string name,
    ILogger log)
```

This function is triggered whenever a new blob is added to the *'landingzone'* container in Azure Blob Storage, the blob's file name should end up with *'.pgp'*. The blob's content is read as a Stream and passed to the function. The blob's name is captured using the {name} parameter, and logging capabilities are provided by the ILogger instance.

#### Azure Blob Client Setup:

```csharp
// NOTE: The privateKey and password variable is just a placeholder. You'd ideally get this from a secure source like Azure Key Vault.
string privateKey = "Placeholder";  // Replace with actual content or use Azure Key Vault.
string password = "Placeholder";  // You might use Azure Key Vault or another secure mechanism for this too.

string connectionString = System.Environment.GetEnvironmentVariable("AzureWebJobsStorage");
BlobContainerClient containerClient = new BlobContainerClient(connectionString, "landingzone");
BlobClient blobClient = containerClient.GetBlobClient($"{name}.csv.decrypted");
```

The PGP PrivateKey should be stored in an Azure KeyVault or another secure environment. Please ensure you create a Function App with Premium Plan to use this as there seems to be an issue with KeyVault reference and Linux Consumption Function App  <a href = "https://github.com/Azure/Azure-Functions/issues/2248"> here </a>.  *In this case, we are hardcoding it for demo purposes* 

The *'BlobContainerClient'* connects to Azure Blob Storage specifically to the 'landingzone' container. It sets up a *'BlobClient'* instance pointing to where the decrypted file should save. You can update *'containerClient'* to another Blob Storage container if you need to seperate both encryption and decryption files.

#### Decryption Process:

```csharp
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
```
Using the PGP library (PgpCore), the blob's content (which is encrypted) is decrypted using the private key and passphrase. The private key (which is a string) is converted into a byte array and a *'MemoryStream'* is created from that byte array. A *'MemoryStream'* represents a stream of data that is stored in memory as opposed to being stored on disk or being read from a network connection.

After the decryption, the current position of *'decryptedStream.Position'* will be at the end. Before you can read from this stream and upload it, you will need to resaet the position back to the start. 

Using the *'BlobClient'* we can upload it back to the same blob storage with all of the decrypted content, the stream's position needs to be at the start. If the stream's position wasn't reset, the *'UploadAsync'* method would think there's nothing left to read, and thus, it would upload an empty stream or miss the decrypted content.

## Usage
To use this Function App:

1. Set up your Azure Function App environment.
2. Deploy this Function App C# to your Azure account.
3. Configure the following with your own variables:

- AzureWebJobsStorage: Default connection string to the stroage account containing your blob containers.
- landingzone: This is your blob container name
- privateKey: replace with your actual content which should start something like this: '-----BEGIN PGP PRIVATE KEY BLOCK-----' (store this in Azure KeyVault)
- password: This is the passphrase for additional security in PGP keys

4. Upload an encrypted file / message (that is generated using the public key) to the blob container and it should automatically decrypt it. 

## Further Reading
For those new to PGP, it's recommended to familiarize oneself with the basic concepts of PGP encryption and decryption to get the most out of this Function App. Understanding key pairs (public and private keys), the difference between symmetric and asymmetric encryption, and how PGP combines these elements is beneficial.