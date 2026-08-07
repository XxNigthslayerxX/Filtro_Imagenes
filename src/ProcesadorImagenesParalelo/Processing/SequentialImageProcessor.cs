using System.Diagnostics;
using ProcesadorImagenesParalelo.Filters;
using ProcesadorImagenesParalelo.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ProcesadorImagenesParalelo.Processing;

/// <summary>
/// Procesa un lote de imágenes utilizando una ejecución secuencial.
/// Cada imagen comienza después de terminar la anterior.
/// </summary>
public static class SequentialImageProcessor
{
    /// <summary>
    /// Procesa todas las imágenes recibidas y las guarda
    /// en el directorio de salida.
    /// </summary>
    public static ProcessingResult ProcessBatch(
        string[] imagePaths,
        string outputDirectory,
        FilterType selectedFilter)
    {
        ProcessingResult result = new();

        // Acumulará el tiempo del filtro de todas las imágenes.
        TimeSpan accumulatedProcessingTime = TimeSpan.Zero;

        foreach (string imagePath in imagePaths)
        {
            string fileName = Path.GetFileName(imagePath);
            string outputPath = Path.Combine(
                outputDirectory,
                fileName
            );

            Console.WriteLine($"Procesando: {fileName}");

            try
            {
                // using garantiza que la memoria de la imagen sea
                // liberada cuando termine esta iteración.
                using Image<Rgba32> image =
                    Image.Load<Rgba32>(imagePath);

                long imagePixels =
                    (long)image.Width * image.Height;

                Stopwatch stopwatch = Stopwatch.StartNew();

                ApplySelectedFilter(image, selectedFilter);

                stopwatch.Stop();

                // El guardado no se incluye en el tiempo del filtro.
                image.Save(outputPath);

                result.ProcessedImages++;
                result.ProcessedPixels += imagePixels;
                accumulatedProcessingTime += stopwatch.Elapsed;

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

        result.ProcessingTime = accumulatedProcessingTime;

        return result;
    }

    /// <summary>
    /// Ejecuta el filtro seleccionado sobre la imagen.
    /// </summary>
    private static void ApplySelectedFilter(
        Image<Rgba32> image,
        FilterType selectedFilter)
    {
        switch (selectedFilter)
        {
            case FilterType.Grayscale:
                GrayscaleFilter.ApplySequential(image);
                break;

            default:
                throw new NotImplementedException(
                    $"El filtro {selectedFilter} todavía no está implementado."
                );
        }
    }
}