using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;

namespace ProcesadorImagenesParalelo.Filters;

/// <summary>
/// Contiene la implementación secuencial del filtro de desenfoque.
/// </summary>
public static class BlurFilter
{
    /// <summary>
    /// Aplica un desenfoque de caja utilizando los píxeles
    /// vecinos de cada posición.
    /// </summary>
    /// <param name="image">Imagen que será modificada.</param>
    /// <param name="radius">
    /// Cantidad de píxeles vecinos utilizados en cada dirección.
    /// </param>
    public static void ApplySequential(
        Image<Rgba32> image,
        int radius)
    {
        // Necesitamos una copia de la imagen original.
        // La copia será utilizada únicamente para lectura.
        using Image<Rgba32> sourceImage = image.Clone();

        // Accedemos simultáneamente a la imagen original
        // y a la imagen que recibirá el resultado.
        sourceImage.ProcessPixelRows(
            image,
            (sourceAccessor, destinationAccessor) =>
            {
                for (int y = 0; y < sourceAccessor.Height; y++)
                {
                    Span<Rgba32> destinationRow =
                        destinationAccessor.GetRowSpan(y);

                    for (int x = 0; x < sourceAccessor.Width; x++)
                    {
                        int redSum = 0;
                        int greenSum = 0;
                        int blueSum = 0;
                        int neighborCount = 0;

                        // Recorremos las filas vecinas.
                        for (
                            int offsetY = -radius;
                            offsetY <= radius;
                            offsetY++)
                        {
                            int neighborY = y + offsetY;

                            // Ignoramos las posiciones que están
                            // fuera de los límites de la imagen.
                            if (neighborY < 0 ||
                                neighborY >= sourceAccessor.Height)
                            {
                                continue;
                            }

                            Span<Rgba32> sourceRow =
                                sourceAccessor.GetRowSpan(neighborY);

                            // Recorremos las columnas vecinas.
                            for (
                                int offsetX = -radius;
                                offsetX <= radius;
                                offsetX++)
                            {
                                int neighborX = x + offsetX;

                                if (neighborX < 0 ||
                                    neighborX >= sourceAccessor.Width)
                                {
                                    continue;
                                }

                                Rgba32 neighborPixel =
                                    sourceRow[neighborX];

                                redSum += neighborPixel.R;
                                greenSum += neighborPixel.G;
                                blueSum += neighborPixel.B;

                                neighborCount++;
                            }
                        }

                        // Conservamos la transparencia del píxel original.
                        Rgba32 originalPixel =
                            sourceAccessor.GetRowSpan(y)[x];

                        // El nuevo color es el promedio de los vecinos.
                        destinationRow[x] = new Rgba32(
                            (byte)(redSum / neighborCount),
                            (byte)(greenSum / neighborCount),
                            (byte)(blueSum / neighborCount),
                            originalPixel.A
                        );
                    }
                }
            }
        );
    }

    /// <summary>
    /// Desenfoca la imagen procesando varias filas simultáneamente.
    /// </summary>
    public static void ApplyParallel(
        Image<Rgba32> image,
        int radius)
    {
        // Todas las tareas leen desde esta copia inmutable.
        using Image<Rgba32> sourceImage = image.Clone();

        Parallel.For(0, image.Height, y =>
        {
            Span<Rgba32> destinationRow =
                image.DangerousGetPixelRowMemory(y).Span;

            for (int x = 0; x < image.Width; x++)
            {
                int redSum = 0;
                int greenSum = 0;
                int blueSum = 0;
                int neighborCount = 0;

                for (
                    int offsetY = -radius;
                    offsetY <= radius;
                    offsetY++)
                {
                    int neighborY = y + offsetY;

                    if (neighborY < 0 ||
                        neighborY >= sourceImage.Height)
                    {
                        continue;
                    }

                    // La fila de origen solo se utiliza para lectura.
                    Span<Rgba32> sourceRow =
                        sourceImage
                            .DangerousGetPixelRowMemory(neighborY)
                            .Span;

                    for (
                        int offsetX = -radius;
                        offsetX <= radius;
                        offsetX++)
                    {
                        int neighborX = x + offsetX;

                        if (neighborX < 0 ||
                            neighborX >= sourceImage.Width)
                        {
                            continue;
                        }

                        Rgba32 neighborPixel =
                            sourceRow[neighborX];

                        redSum += neighborPixel.R;
                        greenSum += neighborPixel.G;
                        blueSum += neighborPixel.B;

                        neighborCount++;
                    }
                }

                Rgba32 originalPixel =
                    sourceImage[x, y];

                destinationRow[x] = new Rgba32(
                    (byte)(redSum / neighborCount),
                    (byte)(greenSum / neighborCount),
                    (byte)(blueSum / neighborCount),
                    originalPixel.A
                );
            }
        });
    }
}