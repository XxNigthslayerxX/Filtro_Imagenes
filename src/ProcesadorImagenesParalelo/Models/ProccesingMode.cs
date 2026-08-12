namespace ProcesadorImagenesParalelo.Models;

/// <summary>
/// Representa la estrategia utilizada para procesar las imágenes.
/// </summary>
public enum ProcessingMode
{
    Sequential = 1,
    Parallel = 2,
    Recursive = 3,
    ParallelBatch = 4,
    Comparison = 5
}