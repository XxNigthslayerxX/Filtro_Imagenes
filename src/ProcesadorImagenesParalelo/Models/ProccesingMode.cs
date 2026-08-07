namespace ProcesadorImagenesParalelo.Models;

/// <summary>
/// Representa la estrategia utilizada para procesar las imágenes.
/// </summary>
public enum ProcessingMode
{
    Sequential = 1,
    Parallel = 2,
    Comparison = 3
}