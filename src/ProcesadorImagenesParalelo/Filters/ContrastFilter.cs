using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ProcesadorImagenesParalelo.Filters;

/// <summary>
/// Contiene el filtro secuencial para ajustar el contraste.
/// </summary>
public static class ContrastFilter
{
    /// <param name="contrast">
    /// Valor comprendido entre -100 y 100.
    /// </param>
    public static void ApplySequential(
        Image<Rgba32> image,
        int contrast)
    {
        // Convertimos el porcentaje en un factor de contraste.
        double factor =
            (259.0 * (contrast + 255.0)) /
            (255.0 * (259.0 - contrast));

        image.ProcessPixelRows(pixelAccessor =>
        {
            for (int y = 0; y < pixelAccessor.Height; y++)
            {
                Span<Rgba32> pixelRow =
                    pixelAccessor.GetRowSpan(y);

                for (int x = 0; x < pixelRow.Length; x++)
                {
                    Rgba32 pixel = pixelRow[x];

                    pixelRow[x] = new Rgba32(
                        ApplyContrast(pixel.R, factor),
                        ApplyContrast(pixel.G, factor),
                        ApplyContrast(pixel.B, factor),
                        pixel.A
                    );
                }
            }
        });
    }

    /// <summary>
    /// Aleja o acerca el canal al punto medio 128.
    /// </summary>
    private static byte ApplyContrast(
        byte colorChannel,
        double factor)
    {
        double adjustedValue =
            factor * (colorChannel - 128) + 128;

        int roundedValue = (int)Math.Round(adjustedValue);

        return (byte)Math.Clamp(roundedValue, 0, 255);
    }
}