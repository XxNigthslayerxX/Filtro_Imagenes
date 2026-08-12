namespace ProcesadorImagenesParalelo.Models;

/// <summary>
/// Almacena las métricas obtenidas por una estrategia
/// durante la comparación de rendimiento.
/// </summary>
public class PerformanceMeasurement
{
    public string ModeName { get; set; } = string.Empty;

    public TimeSpan TotalTime { get; set; }

    public int ProcessedImages { get; set; }

    public int FailedImages { get; set; }

    public long ProcessedPixels { get; set; }

    public double PixelsPerSecond { get; set; }

    public double Speedup { get; set; }

    public double EfficiencyPercentage { get; set; }
}