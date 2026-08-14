using System.Diagnostics;
using ProcesadorImagenesParalelo.Metrics;
using ProcesadorImagenesParalelo.Models;

namespace ProcesadorImagenesParalelo.Processing;

/// <summary>
/// Ejecuta las estrategias disponibles utilizando exactamente
/// las mismas imágenes y el mismo filtro.
/// </summary>
public static class PerformanceComparisonRunner
{
    public static List<PerformanceMeasurement> Run(
        string[] imagePaths,
        ApplicationPaths paths,
        FilterType selectedFilter,
        int filterValue)
    {
        List<PerformanceMeasurement> measurements = new();

        Console.WriteLine();
        Console.WriteLine(
            "1/4 - EJECUTANDO MODO SECUENCIAL"
        );

        (ProcessingResult sequentialResult,
            TimeSpan sequentialTotalTime) =
            MeasureExecution(() =>
                SequentialImageProcessor.ProcessBatch(
                    imagePaths,
                    paths.SequentialOutputDirectory,
                    selectedFilter,
                    filterValue
                )
            );

        PerformanceMeasurement sequentialMeasurement =
            PerformanceCalculator.CreateMeasurement(
                "Secuencial",
                sequentialResult,
                sequentialTotalTime,
                sequentialTotalTime
            );

        // La versión secuencial es la referencia.
        sequentialMeasurement.Speedup = 1.0;
        sequentialMeasurement.EfficiencyPercentage = 100.0;

        measurements.Add(sequentialMeasurement);

        Console.WriteLine();
        Console.WriteLine(
            "2/4 - EJECUTANDO PARALELISMO POR FILAS"
        );

        (ProcessingResult parallelResult,
            TimeSpan parallelTotalTime) =
            MeasureExecution(() =>
                ParallelImageProcessor.ProcessBatch(
                    imagePaths,
                    paths.ParallelOutputDirectory,
                    selectedFilter,
                    filterValue
                )
            );

        measurements.Add(
            PerformanceCalculator.CreateMeasurement(
                "Paralelo por filas",
                parallelResult,
                parallelTotalTime,
                sequentialTotalTime
            )
        );

        Console.WriteLine();
        Console.WriteLine(
            "3/4 - EJECUTANDO DIVISIÓN RECURSIVA"
        );

        (ProcessingResult recursiveResult,
            TimeSpan recursiveTotalTime) =
            MeasureExecution(() =>
                RecursiveImageProcessor.ProcessBatch(
                    imagePaths,
                    paths.RecursiveOutputDirectory,
                    selectedFilter,
                    filterValue
                )
            );

        measurements.Add(
            PerformanceCalculator.CreateMeasurement(
                "División recursiva",
                recursiveResult,
                recursiveTotalTime,
                sequentialTotalTime
            )
        );

        Console.WriteLine();
        Console.WriteLine(
            "4/4 - EJECUTANDO LOTE PARALELO"
        );

        (ProcessingResult batchResult,
            TimeSpan batchTotalTime) =
            MeasureExecution(() =>
                ParallelBatchImageProcessor.ProcessBatch(
                    imagePaths,
                    paths.ParallelBatchOutputDirectory,
                    selectedFilter,
                    filterValue
                )
            );

        measurements.Add(
            PerformanceCalculator.CreateMeasurement(
                "Lote paralelo",
                batchResult,
                batchTotalTime,
                sequentialTotalTime
            )
        );

        return measurements;
    }

    /// <summary>
    /// Mide el tiempo total transcurrido desde que comienza
    /// la estrategia hasta que termina el lote completo.
    /// </summary>
    private static (
        ProcessingResult Result,
        TimeSpan TotalTime
    ) MeasureExecution(
        Func<ProcessingResult> processingOperation)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        ProcessingResult result =
            processingOperation();

        stopwatch.Stop();

        return (result, stopwatch.Elapsed);
    }
}