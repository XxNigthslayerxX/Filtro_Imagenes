using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ProcesadorImagenesParalelo.Filters;

/// <summary>
/// Contiene el filtro secuencial de inversión de colores.
/// </summary>
public static class InvertColorsFilter
{
    public static void ApplySequential(Image<Rgba32> image)
    {
        image.ProcessPixelRows(pixelAccessor =>
        {
            for (int y = 0; y < pixelAccessor.Height; y++)
            {
                Span<Rgba32> pixelRow =
                    pixelAccessor.GetRowSpan(y);

                for (int x = 0; x < pixelRow.Length; x++)
                {
                    Rgba32 pixel = pixelRow[x];

                    // Para invertir un canal, restamos su valor a 255.
                    // El canal Alpha se conserva sin modificaciones.
                    pixelRow[x] = new Rgba32(
                        (byte)(255 - pixel.R),
                        (byte)(255 - pixel.G),
                        (byte)(255 - pixel.B),
                        pixel.A
                    );
                }
            }
        });
    }
}