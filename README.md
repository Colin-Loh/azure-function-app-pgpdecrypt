# C# Function App for PGP Decryption

## Overview

This repository contains a Function App written in C# that leverages PGP (Pretty Good Privacy) to decrypt data in Azure's serverless environment. 

### What is PGP?

PGP (Pretty Good Privacy) is a data encryption and decryption program that provides cryptographic privacy and authentication. It's widely used for securing emails, files, and more. It employs a combination of symmetric and asymmetric cryptography to ensure the security and confidentiality of the data.

## Code Explanation

The primary function:

```csharp
pgp.DecryptStream(myBlob, decryptedStream, privateKeyStream, password);
```

Here's a breakdown of the parameters:

- myBlob: The input stream containing the encrypted data.
- decryptedStream: The output stream where the decrypted data will be written.
- privateKeyStream: The stream containing the user's private key.
- password: The passphrase associated with the private key.

## Usage
To use this Function App:

1. Set up your Azure Function App environment.
2. Deploy this Function App to your Azure account.
3. Send an HTTP request with the encrypted data and necessary decryption details.

## Further Reading
For those new to PGP, it's recommended to familiarize oneself with the basic concepts of PGP encryption and decryption to get the most out of this Function App. Understanding key pairs (public and private keys), the difference between symmetric and asymmetric encryption, and how PGP combines these elements is beneficial.