using System.Diagnostics;
using ProcesadorImagenesParalelo.Filters;
using ProcesadorImagenesParalelo.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ProcesadorImagenesParalelo.Processing;

/// <summary>
/// Procesa imágenes utilizando paralelismo interno por filas.
/// En este incremento, las imágenes todavía se procesan una por una.
/// </summary>
public static class ParallelImageProcessor
{
    public static ProcessingResult ProcessBatch(
        string[] imagePaths,
        string outputDirectory,
        FilterType selectedFilter,
        int filterValue)
    {
        ProcessingResult result = new();

        TimeSpan accumulatedProcessingTime = TimeSpan.Zero;

        foreach (string imagePath in imagePaths)
        {
            string fileName = Path.GetFileName(imagePath);

            string outputPath = Path.Combine(
                outputDirectory,
                fileName
            );

            Console.WriteLine(
                $"Procesando en paralelo: {fileName}"
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

    private static void ApplySelectedFilter(
        Image<Rgba32> image,
        FilterType selectedFilter,
        int filterValue)
    {
        switch (selectedFilter)
        {
            case FilterType.Grayscale:
                GrayscaleFilter.ApplyParallel(image);
                break;

            case FilterType.InvertColors:
                InvertColorsFilter.ApplyParallel(image);
                break;

            case FilterType.Brightness:
                BrightnessFilter.ApplyParallel(
                    image,
                    filterValue
                );
                break;

            case FilterType.Contrast:
                ContrastFilter.ApplyParallel(
                    image,
                    filterValue
                );
                break;

            case FilterType.Blur:
                BlurFilter.ApplyParallel(
                    image,
                    filterValue
                );
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(selectedFilter)
                );
        }
    }
}
