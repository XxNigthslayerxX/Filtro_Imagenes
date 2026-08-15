namespace ProcesadorImagenesParalelo.Parallelism;

/// <summary>
/// Divide recursivamente un conjunto de filas en regiones menores.
/// </summary>
public static class RecursiveRowPartitioner
{
    // Una región con esta cantidad de filas o menos
    // será procesada directamente.
    private const int MinimumRowsPerRegion = 64;

    /// <summary>
    /// Inicia la división recursiva de las filas.
    /// </summary>
    public static void Process(
        int totalRows,
        Action<int, int> processRegion)
    {
        // Calculamos una profundidad apropiada según
        // la cantidad de procesadores lógicos disponibles.
        int maximumDepth =
            Environment.ProcessorCount > 1
                ? (int)Math.Ceiling(
                    Math.Log2(Environment.ProcessorCount)
                )
                : 0;

        ProcessRecursively(
            startRow: 0,
            endRow: totalRows,
            currentDepth: 0,
            maximumDepth,
            processRegion
        );
    }

    /// <summary>
    /// Divide una región en dos hasta alcanzar el tamaño
    /// mínimo o la profundidad máxima.
    /// </summary>
    private static void ProcessRecursively(
        int startRow,
        int endRow,
        int currentDepth,
        int maximumDepth,
        Action<int, int> processRegion)
    {
        int rowCount = endRow - startRow;

        // Caso base de la recursividad.
        if (rowCount <= MinimumRowsPerRegion ||
            currentDepth >= maximumDepth)
        {
            processRegion(startRow, endRow);
            return;
        }

        int middleRow = startRow + rowCount / 2;

        // La mitad superior se asigna a una tarea.
        Task upperHalfTask = Task.Run(() =>
        {
            ProcessRecursively(
                startRow,
                middleRow,
                currentDepth + 1,
                maximumDepth,
                processRegion
            );
        });

        // El hilo actual procesa recursivamente la mitad inferior.
        ProcessRecursively(
            middleRow,
            endRow,
            currentDepth + 1,
            maximumDepth,
            processRegion
        );

        // Esperamos que ambas mitades finalicen.
        upperHalfTask.GetAwaiter().GetResult();
    }
}