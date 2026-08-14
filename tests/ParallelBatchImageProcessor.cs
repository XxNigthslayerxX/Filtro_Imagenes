using System.Collections.Concurrent;
using System.Diagnostics;
using ProcesadorImagenesParalelo.Filters;
using ProcesadorImagenesParalelo.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ProcesadorImagenesParalelo.Processing;

/// <summary>
/// Procesa varias imágenes simultáneamente.
///
/// Cada imagen utiliza internamente el filtro secuencial.
/// El paralelismo se aplica al nivel del lote.
/// </summary>
public static class ParallelBatchImageProcessor
{
    // Evita que varios hilos escriban mensajes incompletos
    // o mezclados en la consola.
    private static readonly object ConsoleLock = new();

    public static ProcessingResult ProcessBatch(
        string[] imagePaths,
        string outputDirectory,
        FilterType selectedFilter,
        int filterValue)
    {
        int processedImages = 0;
        int failedImages = 0;
        long processedPixels = 0;
        int completedImages = 0;

        // Almacena los errores de forma segura desde varios hilos.
        ConcurrentBag<string> errors = new();

        // Este cronómetro mide el tiempo real del lote completo.
        Stopwatch batchStopwatch = Stopwatch.StartNew();

        ParallelOptions options = new()
        {
            // Limita la cantidad de imágenes procesadas
            // simultáneamente según el hardware disponible.
            MaxDegreeOfParallelism =
                Environment.ProcessorCount
        };

        Parallel.ForEach(
            imagePaths,
            options,
            imagePath =>
            {
                string fileName =
                    Path.GetFileName(imagePath);

                string outputPath = Path.Combine(
                    outputDirectory,
                    fileName
                );

                try
                {
                    using Image<Rgba32> image =
                        Image.Load<Rgba32>(imagePath);

                    long imagePixels =
                        (long)image.Width * image.Height;

                    ApplySelectedFilter(
                        image,
                        selectedFilter,
                        filterValue
                    );

                    image.Save(outputPath);

                    // Interlocked actualiza valores compartidos
                    // mediante operaciones atómicas.
                    Interlocked.Increment(
                        ref processedImages
                    );

                    Interlocked.Add(
                        ref processedPixels,
                        imagePixels
                    );
                }
                catch (Exception exception)
                {
                    Interlocked.Increment(
                        ref failedImages
                    );

                    errors.Add(
                        $"{fileName}: {exception.Message}"
                    );
                }
                finally
                {
                    int currentProgress =
                        Interlocked.Increment(
                            ref completedImages
                        );

                    // lock evita que dos hilos escriban
                    // simultáneamente en la consola.
                    lock (ConsoleLock)
                    {
                        Console.WriteLine(
                            $"Progreso: {currentProgress}/" +
                            $"{imagePaths.Length} - {fileName}"
                        );
                    }
                }
            }
        );

        batchStopwatch.Stop();

        if (!errors.IsEmpty)
        {
            Console.WriteLine();
            Console.WriteLine("ERRORES DEL LOTE");
            Console.WriteLine(
                "------------------------------------------"
            );

            foreach (string error in errors)
            {
                Console.WriteLine(error);
            }

            Console.WriteLine();
        }

        return new ProcessingResult
        {
            ProcessedImages = processedImages,
            FailedImages = failedImages,
            ProcessedPixels = processedPixels,
            ProcessingTime = batchStopwatch.Elapsed
        };
    }

    /// <summary>
    /// Ejecuta la versión secuencial del filtro sobre una imagen.
    /// El paralelismo ya existe entre las imágenes del lote.
    /// </summary>
    private static void ApplySelectedFilter(
        Image<Rgba32> image,
        FilterType selectedFilter,
        int filterValue)
    {
        switch (selectedFilter)
        {
            case FilterType.Grayscale:
                GrayscaleFilter.ApplySequential(image);
                break;

            case FilterType.InvertColors:
                InvertColorsFilter.ApplySequential(image);
                break;

            case FilterType.Brightness:
                BrightnessFilter.ApplySequential(
                    image,
                    filterValue
                );
                break;

            case FilterType.Contrast:
                ContrastFilter.ApplySequential(
                    image,
                    filterValue
                );
                break;

            case FilterType.Blur:
                BlurFilter.ApplySequential(
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