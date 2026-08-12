using ProcesadorImagenesParalelo.Models;

namespace ProcesadorImagenesParalelo.ConsoleUI;

/// <summary>
/// Presenta en consola los resultados de la comparación.
/// </summary>
public static class PerformanceReportPrinter
{
    public static void Print(
        List<PerformanceMeasurement> measurements)
    {
        Console.WriteLine();
        Console.WriteLine(
            "RESULTADOS DE LA COMPARACIÓN"
        );
        Console.WriteLine(
            "============================================================"
        );

        Console.WriteLine(
            $"Procesadores lógicos disponibles: " +
            $"{Environment.ProcessorCount}"
        );

        Console.WriteLine();

        foreach (
            PerformanceMeasurement measurement
            in measurements)
        {
            Console.WriteLine(
                $"Modo: {measurement.ModeName}"
            );

            Console.WriteLine(
                $"Tiempo total: " +
                $"{measurement.TotalTime.TotalMilliseconds:F2} ms"
            );

            Console.WriteLine(
                $"Imágenes procesadas: " +
                $"{measurement.ProcessedImages}"
            );

            Console.WriteLine(
                $"Imágenes fallidas: " +
                $"{measurement.FailedImages}"
            );

            Console.WriteLine(
                $"Píxeles por segundo: " +
                $"{measurement.PixelsPerSecond:N0}"
            );

            Console.WriteLine(
                $"Speedup: {measurement.Speedup:F2}x"
            );

            Console.WriteLine(
                $"Eficiencia: " +
                $"{measurement.EfficiencyPercentage:F2}%"
            );

            Console.WriteLine(
                "------------------------------------------------------------"
            );
        }

        PerformanceMeasurement fastest =
            measurements.MinBy(
                measurement => measurement.TotalTime
            )!;

        Console.WriteLine(
            $"Estrategia más rápida: {fastest.ModeName}"
        );
    }
}