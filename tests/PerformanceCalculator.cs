using ProcesadorImagenesParalelo.Models;

namespace ProcesadorImagenesParalelo.Metrics;

/// <summary>
/// Calcula las métricas de rendimiento utilizadas para
/// comparar las diferentes estrategias.
/// </summary>
public static class PerformanceCalculator
{
    public static PerformanceMeasurement CreateMeasurement(
        string modeName,
        ProcessingResult result,
        TimeSpan totalTime,
        TimeSpan sequentialTime)
    {
        double pixelsPerSecond =
            totalTime.TotalSeconds > 0
                ? result.ProcessedPixels /
                  totalTime.TotalSeconds
                : 0;

        double speedup =
            sequentialTime.TotalMilliseconds > 0 &&
            totalTime.TotalMilliseconds > 0
                ? sequentialTime.TotalMilliseconds /
                  totalTime.TotalMilliseconds
                : 0;

        double efficiency =
            Environment.ProcessorCount > 0
                ? speedup /
                  Environment.ProcessorCount * 100
                : 0;

        return new PerformanceMeasurement
        {
            ModeName = modeName,
            TotalTime = totalTime,
            ProcessedImages = result.ProcessedImages,
            FailedImages = result.FailedImages,
            ProcessedPixels = result.ProcessedPixels,
            PixelsPerSecond = pixelsPerSecond,
            Speedup = speedup,
            EfficiencyPercentage = efficiency
        };
    }
}