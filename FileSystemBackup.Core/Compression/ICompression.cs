using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemBackup.Core.Compression
{
    /// <summary>
    /// Provides access to compression methods.
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// Compresses a batch of uncompressed data into a smaller data set.
        /// </summary>
        /// <param name="arrUncompressedData">A batch of uncompressed data that should be compressed.</param>
        /// <returns>The content of <paramref name="arrUncompressedData"/> in a compressed form.</returns>
        byte[] Compress(byte[] arrUncompressedData);

        /// <summary>
        /// Decompresses a batch of compressed data into the original data set.
        /// </summary>
        /// <param name="arrCompressedData">A batch of compressed data that should be decompressed.</param>
        /// <returns>The content of <paramref name="arrCompressedData"/> in an uncompressed form.</returns>
        byte[] Decompress(byte[] arrCompressedData);
    }
}
