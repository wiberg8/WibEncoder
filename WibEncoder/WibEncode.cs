using System.Text;

namespace WibEncoder;

public static class WibEncode
{
    public static async Task<MemoryStream> EncodeAsync(string text, byte[][] files)
    {
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        MemoryStream ms = new();
        await ms.WriteAsync(BitConverter.GetBytes(textBytes.Length));
        await ms.WriteAsync(textBytes);
        foreach (byte[] file in files)
        {
            await ms.WriteAsync(BitConverter.GetBytes(file.Length));
            await ms.WriteAsync(file);
        }
        return ms;
    }

    public static MemoryStream Encode(string text, byte[][] files)
    {
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        MemoryStream ms = new();
        ms.Write(BitConverter.GetBytes(textBytes.Length));
        ms.Write(textBytes);
        foreach (byte[] file in files)
        {
            ms.Write(BitConverter.GetBytes(file.Length));
            ms.Write(file);
        }
        return ms;
    }

    public static async Task<DecodeResult> DecodeAsync(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        byte[] firstBytes = await ReadExactlyAsync(stream, 4);
        int textByteCount = BitConverter.ToInt32(firstBytes);
        byte[] textBytes = await ReadExactlyAsync(stream, textByteCount);
        List<MemoryStream> files = new();
        while (stream.Position != stream.Length)
        {
            byte[] nextFileLengthBytes = await ReadExactlyAsync(stream, 4);
            int nextFileLength = BitConverter.ToInt32(nextFileLengthBytes);
            byte[] fileBytes = await ReadExactlyAsync(stream, nextFileLength);
            files.Add(new MemoryStream(fileBytes));
        }
        return new(Encoding.UTF8.GetString(textBytes), files);
    }

    public static DecodeResult Decode(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        byte[] firstBytes = ReadExactly(stream, 4);
        int textByteCount = BitConverter.ToInt32(firstBytes);
        byte[] textBytes = ReadExactly(stream, textByteCount);
        List<MemoryStream> files = new();
        while (stream.Position != stream.Length)
        {
            byte[] nextFileLengthBytes = ReadExactly(stream, 4);
            int nextFileLength = BitConverter.ToInt32(nextFileLengthBytes);
            byte[] fileBytes = ReadExactly(stream, nextFileLength);
            files.Add(new MemoryStream(fileBytes));
        }
        return new(Encoding.UTF8.GetString(textBytes), files);
    }

    private static async Task<byte[]> ReadExactlyAsync(Stream stream, int count)
    {
        byte[] buffer = new byte[count];
        int offset = 0;
        while (offset < count)
        {
            int read = await stream.ReadAsync(buffer, offset, count - offset);
            if (read == 0)
                throw new System.IO.EndOfStreamException();
            offset += read;
        }
        return buffer;
    }

    private static byte[] ReadExactly(Stream stream, int count)
    {
        byte[] buffer = new byte[count];
        int offset = 0;
        while (offset < count)
        {
            int read = stream.Read(buffer, offset, count - offset);
            if (read == 0)
                throw new System.IO.EndOfStreamException();
            offset += read;
        }
        return buffer;
    }
}