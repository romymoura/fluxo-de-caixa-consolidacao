namespace FluxoDeCaixa.Consolidacao.Common.Utils;

public static class StringExtensions
{
    public static Stream ToStream(this string content)
    {
        var memoryStream = new MemoryStream();
        using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            writer.Write(content);
            writer.Flush();
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}
