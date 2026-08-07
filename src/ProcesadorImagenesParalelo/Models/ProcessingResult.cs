namespace ProcesadorImagenesParalelo.Models;

/// <summary>
/// Almacena los resultados y las métricas obtenidas después
/// de procesar un lote de imágenes.
/// </summary>
public class ProcessingResult
{
    // Cantidad de imágenes procesadas correctamente.
    public int ProcessedImages { get; set; }

    // Cantidad de imágenes que no pudieron procesarse.
    public int FailedImages { get; set; }

    // Cantidad total de píxeles modificados.
    public long ProcessedPixels { get; set; }

    // Tiempo dedicado exclusivamente a aplicar los filtros.
    public TimeSpan ProcessingTime { get; set; }
}