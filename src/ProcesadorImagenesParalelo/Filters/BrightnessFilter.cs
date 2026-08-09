using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ProcesadorImagenesParalelo.Filters;

/// <summary>
/// Contiene el filtro secuencial para ajustar el brillo.
/// </summary>
public static class BrightnessFilter
{
    /// <param name="adjustment">
    /// Valor comprendido entre -255 y 255.
    /// Los valores positivos aumentan el brillo y los negativos lo reducen.
    /// </param>
    public static void ApplySequential(
        Image<Rgba32> image,
        int adjustment)
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

                    pixelRow[x] = new Rgba32(
                        ClampToByte(pixel.R + adjustment),
                        ClampToByte(pixel.G + adjustment),
                        ClampToByte(pixel.B + adjustment),
                        pixel.A
                    );
                }
            }
        });
    }

    /// <summary>
    /// Mantiene el resultado dentro del rango válido de un byte.
    /// </summary>
    private static byte ClampToByte(int value)
    {
        return (byte)Math.Clamp(value, 0, 255);
    }
}