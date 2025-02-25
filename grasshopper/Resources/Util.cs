using System.Drawing;

namespace Handzone.Resources;

static class Util
{
    public static Bitmap GetIcon(string name)
    {
        var icon = $"Handzone.Resources.Icons.{name}.png";
        var assembly = typeof(HandzoneInfo).Assembly;
        using var stream = assembly.GetManifestResourceStream(icon);
        return new Bitmap(stream);
    }
}