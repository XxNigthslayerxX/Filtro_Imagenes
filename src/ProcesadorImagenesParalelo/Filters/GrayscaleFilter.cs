using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ProcesadorImagenesParalelo.Filters;

/// <summary>
/// Contiene las implementaciones del filtro de escala de grises.
/// </summary>
public static class GrayscaleFilter
{
    /// <summary>
    /// Convierte secuencialmente todos los píxeles de una imagen
    /// a escala de grises.
    /// </summary>
    public static void ApplySequential(Image<Rgba32> image)
    {
        // ProcessPixelRows permite acceder directamente a las
        // filas de píxeles almacenadas en la imagen.
        image.ProcessPixelRows(pixelAccessor =>
        {
            // Recorremos todas las filas de arriba hacia abajo.
            for (int y = 0; y < pixelAccessor.Height; y++)
            {
                // Obtenemos los píxeles pertenecientes a la fila actual.
                Span<Rgba32> pixelRow =
                    pixelAccessor.GetRowSpan(y);

                // Recorremos la fila de izquierda a derecha.
                for (int x = 0; x < pixelRow.Length; x++)
                {
                    Rgba32 originalPixel = pixelRow[x];

                    // Calculamos la luminancia utilizando una media
                    // ponderada de rojo, verde y azul.
                    byte grayValue = (byte)(
                        originalPixel.R * 0.299 +
                        originalPixel.G * 0.587 +
                        originalPixel.B * 0.114
                    );

                    // Los tres canales reciben el mismo valor.
                    // Conservamos el canal Alpha de transparencia.
                    pixelRow[x] = new Rgba32(
                        grayValue,
                        grayValue,
                        grayValue,
                        originalPixel.A
                    );
                }
            }
        });
    }
}