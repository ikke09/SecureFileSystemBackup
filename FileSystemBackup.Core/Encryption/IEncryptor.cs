using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemBackup.Core.Encryption
{
    /// <summary>
    /// Provides access to encryption methods.
    /// </summary>
    public interface IEncryptor
    {
        /// <summary>
        /// Encrypts a given batch of data.
        /// </summary>
        /// <param name="arrUnencryptedData">A batch of unencrypted data as byte[].</param>
        /// <returns>The content of <paramref name="arrUnencryptedData"/>, but encrypted.</returns>
        byte[] Encrypt(byte[] arrUnencryptedData);

        /// <summary>
        /// Decrypts a given batch of data.
        /// </summary>
        /// <param name="arrEncryptedData">A batch of encrypted data as byte[].</param>
        /// <returns>The content of <paramref name="arrEncryptedData"/>, but unencrypted.</returns>
        byte[] Decrypt(byte[] arrEncryptedData);
    }
}
