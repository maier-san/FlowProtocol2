using System;
using System.IO;
using System.IO.Compression;

namespace FlowProtocol2.Core
{
    public static class UrlCompressor
    {
        // Prefix to mark compressed values
        public const string Marker = "z:";

        public static string CompressToUrl(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(text);
            using (var ms = new MemoryStream())
            {
                using (var ds = new DeflateStream(ms, CompressionLevel.Optimal, true))
                {
                    ds.Write(inputBytes, 0, inputBytes.Length);
                }
                var compressed = ms.ToArray();
                string b64 = Convert.ToBase64String(compressed);
                // make URL-safe
                b64 = b64.TrimEnd('=');
                b64 = b64.Replace('+', '-').Replace('/', '_');
                return Marker + b64;
            }
        }

        public static string DecompressFromUrl(string compressedText)
        {
            if (string.IsNullOrEmpty(compressedText)) return string.Empty;
            if (!compressedText.StartsWith(Marker)) return compressedText;
            string b64 = compressedText.Substring(Marker.Length);
            b64 = b64.Replace('-', '+').Replace('_', '/');
            // pad
            switch (b64.Length % 4)
            {
                case 2: b64 += "=="; break;
                case 3: b64 += "="; break;
            }
            byte[] compressed = Convert.FromBase64String(b64);
            using (var inMs = new MemoryStream(compressed))
            using (var ds = new DeflateStream(inMs, CompressionMode.Decompress))
            using (var outMs = new MemoryStream())
            {
                ds.CopyTo(outMs);
                return System.Text.Encoding.UTF8.GetString(outMs.ToArray());
            }
        }
    }
}
