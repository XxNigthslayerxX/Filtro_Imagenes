using System.Diagnostics;
using ProcesadorImagenesParalelo.Models;
using ProcesadorImagenesParalelo.Parallelism;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace ProcesadorImagenesParalelo.Processing;

/// <summary>
/// Procesa imágenes dividiendo recursivamente sus filas
/// en regiones independientes.
/// </summary>
public static class RecursiveImageProcessor
{
    public static ProcessingResult ProcessBatch(
        string[] imagePaths,
        string outputDirectory,
        FilterType selectedFilter,
        int filterValue)
    {
        ProcessingResult result = new();

        foreach (string imagePath in imagePaths)
        {
            string fileName = Path.GetFileName(imagePath);

            string outputPath = Path.Combine(
                outputDirectory,
                fileName
            );

            Console.WriteLine(
                $"Procesando recursivamente: {fileName}"
            );

            try
            {
                using Image<Rgba32> image =
                    Image.Load<Rgba32>(imagePath);

                long imagePixels =
                    (long)image.Width * image.Height;

                Stopwatch stopwatch = Stopwatch.StartNew();

                ApplySelectedFilter(
                    image,
                    selectedFilter,
                    filterValue
                );

                stopwatch.Stop();

                image.Save(outputPath);

                result.ProcessedImages++;
                result.ProcessedPixels += imagePixels;
                result.ProcessingTime += stopwatch.Elapsed;

                Console.WriteLine(
                    $"Completada en " +
                    $"{stopwatch.Elapsed.TotalMilliseconds:F2} ms"
                );
            }
            catch (Exception exception)
            {
                result.FailedImages++;

                Console.WriteLine(
                    $"No se pudo procesar {fileName}: " +
                    exception.Message
                );
            }

            Console.WriteLine();
        }

        return result;
    }

    /// <summary>
    /// Selecciona entre los filtros independientes y el desenfoque.
    /// </summary>
    private static void ApplySelectedFilter(
        Image<Rgba32> image,
        FilterType selectedFilter,
        int filterValue)
    {
        if (selectedFilter == FilterType.Blur)
        {
            ApplyBlurRecursively(
                image,
                filterValue
            );

            return;
        }

        ApplyIndependentFilterRecursively(
            image,
            selectedFilter,
            filterValue
        );
    }

    /// <summary>
    /// Procesa filtros en los que cada píxel puede modificarse
    /// sin consultar sus vecinos.
    /// </summary>
    private static void ApplyIndependentFilterRecursively(
        Image<Rgba32> image,
        FilterType selectedFilter,
        int filterValue)
    {
        // El factor se calcula una sola vez.
        double contrastFactor =
            selectedFilter == FilterType.Contrast
                ? CalculateContrastFactor(filterValue)
                : 1.0;

        RecursiveRowPartitioner.Process(
            image.Height,
            (startRow, endRow) =>
            {
                for (int y = startRow; y < endRow; y++)
                {
                    Span<Rgba32> pixelRow =
                        image
                            .DangerousGetPixelRowMemory(y)
                            .Span;

                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        pixelRow[x] = TransformPixel(
                            pixelRow[x],
                            selectedFilter,
                            filterValue,
                            contrastFactor
                        );
                    }
                }
            }
        );
    }

    /// <summary>
    /// Aplica la transformación correspondiente a un píxel.
    /// </summary>
    private static Rgba32 TransformPixel(
        Rgba32 pixel,
        FilterType selectedFilter,
        int filterValue,
        double contrastFactor)
    {
        return selectedFilter switch
        {
            FilterType.Grayscale =>
                ConvertToGrayscale(pixel),

            FilterType.InvertColors =>
                InvertColors(pixel),

            FilterType.Brightness =>
                AdjustBrightness(pixel, filterValue),

            FilterType.Contrast =>
                AdjustContrast(pixel, contrastFactor),

            _ => throw new ArgumentOutOfRangeException(
                nameof(selectedFilter)
            )
        };
    }

    private static Rgba32 ConvertToGrayscale(Rgba32 pixel)
    {
        byte grayValue = (byte)(
            pixel.R * 0.299 +
            pixel.G * 0.587 +
            pixel.B * 0.114
        );

        return new Rgba32(
            grayValue,
            grayValue,
            grayValue,
            pixel.A
        );
    }

    private static Rgba32 InvertColors(Rgba32 pixel)
    {
        return new Rgba32(
            (byte)(255 - pixel.R),
            (byte)(255 - pixel.G),
            (byte)(255 - pixel.B),
            pixel.A
        );
    }

    private static Rgba32 AdjustBrightness(
        Rgba32 pixel,
        int adjustment)
    {
        return new Rgba32(
            ClampToByte(pixel.R + adjustment),
            ClampToByte(pixel.G + adjustment),
            ClampToByte(pixel.B + adjustment),
            pixel.A
        );
    }

    private static Rgba32 AdjustContrast(
        Rgba32 pixel,
        double factor)
    {
        return new Rgba32(
            ApplyContrast(pixel.R, factor),
            ApplyContrast(pixel.G, factor),
            ApplyContrast(pixel.B, factor),
            pixel.A
        );
    }

    private static double CalculateContrastFactor(int contrast)
    {
        return
            (259.0 * (contrast + 255.0)) /
            (255.0 * (259.0 - contrast));
    }

    private static byte ApplyContrast(
        byte colorChannel,
        double factor)
    {
        double adjustedValue =
            factor * (colorChannel - 128) + 128;

        int roundedValue =
            (int)Math.Round(adjustedValue);

        return (byte)Math.Clamp(
            roundedValue,
            0,
            255
        );
    }

    private static byte ClampToByte(int value)
    {
        return (byte)Math.Clamp(value, 0, 255);
    }

    /// <summary>
    /// Desenfoca regiones de la imagen utilizando una copia
    /// de solo lectura.
    /// </summary>
    private static void ApplyBlurRecursively(
        Image<Rgba32> image,
        int radius)
    {
        using Image<Rgba32> sourceImage = image.Clone();

        RecursiveRowPartitioner.Process(
            image.Height,
            (startRow, endRow) =>
            {
                for (int y = startRow; y < endRow; y++)
                {
                    Span<Rgba32> destinationRow =
                        image
                            .DangerousGetPixelRowMemory(y)
                            .Span;

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

                            Span<Rgba32> sourceRow =
                                sourceImage
                                    .DangerousGetPixelRowMemory(
                                        neighborY
                                    )
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
                }
            }
        );
    }
}