using System.IO.Compression;

namespace CoreConnect.Server.Services;

/// <summary>
/// Provides Brotli compression/decompression for remote control stream frames.
/// Uses quality level 1 (fastest) to minimize latency overhead on real-time streams.
/// </summary>
public static class StreamCompression
{
    /// <summary>
    /// Compresses a byte array using Brotli at the fastest quality level.
    /// Prepends a 1-byte header (0x01) to indicate the frame is compressed.
    /// </summary>
    public static byte[] CompressFrame(byte[] frame)
    {
        using var output = new MemoryStream();
        // Write compression marker byte.
        output.WriteByte(0x01);

        using (var brotli = new BrotliStream(output, CompressionLevel.Fastest, leaveOpen: true))
        {
            brotli.Write(frame, 0, frame.Length);
        }

        return output.ToArray();
    }

    /// <summary>
    /// Wraps an async enumerable of byte[] frames, compressing each one with Brotli.
    /// Skips tiny frames (< 64 bytes) to avoid overhead on control messages.
    /// </summary>
    public static async IAsyncEnumerable<byte[]> CompressStream(IAsyncEnumerable<byte[]> source)
    {
        await foreach (var chunk in source)
        {
            // Don't compress very small frames — the overhead isn't worth it.
            if (chunk.Length < 64)
            {
                // Prefix with 0x00 = uncompressed marker.
                var markedChunk = new byte[chunk.Length + 1];
                markedChunk[0] = 0x00;
                Buffer.BlockCopy(chunk, 0, markedChunk, 1, chunk.Length);
                yield return markedChunk;
            }
            else
            {
                yield return CompressFrame(chunk);
            }
        }
    }
}
