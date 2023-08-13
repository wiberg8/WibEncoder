namespace WibEncoder;

public class DecodeResult
{
    public string Text { get; }
    public List<MemoryStream> Files { get; }

    public DecodeResult(string text, List<MemoryStream> files)
    {
        Text = text;
        Files = files;
    }
}