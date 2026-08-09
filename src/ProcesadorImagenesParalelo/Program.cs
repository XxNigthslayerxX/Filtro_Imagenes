using ProcesadorImagenesParalelo.ConsoleUI;
using ProcesadorImagenesParalelo.Models;
using ProcesadorImagenesParalelo.Processing;

namespace ProcesadorImagenesParalelo;

internal class Program
{
    static void Main(string[] args)
    {
        ConsoleMenu.ShowHeader();

        // La carpeta actual será la base para Input y Output.
        string baseDirectory = Directory.GetCurrentDirectory();

        // Preparamos todas las rutas utilizadas por la aplicación.
        ApplicationPaths paths = new(baseDirectory);
        paths.CreateDirectories();

        // Buscamos todas las imágenes colocadas en Input.
        string[] imagePaths = ImageFileScanner.FindImages(
            paths.InputDirectory
        );

        // Si Input está vacío, el programa informa dónde
        // deben colocarse las imágenes y finaliza.
        if (imagePaths.Length == 0)
        {
            Console.WriteLine("No se encontraron imágenes.");
            Console.WriteLine();
            Console.WriteLine("Coloca las imágenes en esta carpeta:");
            Console.WriteLine(paths.InputDirectory);
            Console.WriteLine();
            Console.WriteLine("Presiona cualquier tecla para finalizar...");

            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Imágenes encontradas: {imagePaths.Length}");
        Console.WriteLine();

        // Mostramos solamente el nombre, no la ruta completa.
        foreach (string imagePath in imagePaths)
        {
            Console.WriteLine($"- {Path.GetFileName(imagePath)}");
        }

        Console.WriteLine();

        FilterType selectedFilter = ConsoleMenu.SelectFilter();

        int filterValue = ConsoleMenu.SelectFilterValue(selectedFilter);

        ProcessingMode selectedMode =
            ConsoleMenu.SelectProcessingMode();

        Console.WriteLine();
        Console.WriteLine("CONFIGURACIÓN SELECCIONADA");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine($"Imágenes: {imagePaths.Length}");
        Console.WriteLine($"Filtro: {selectedFilter}");
        if (selectedFilter is FilterType.Brightness or FilterType.Contrast)
        {
            Console.WriteLine($"Valor del ajuste: {filterValue}");
        }
        Console.WriteLine($"Modo: {selectedMode}");
        Console.WriteLine();

        // En este incremento solamente está disponible
        // el filtro de escala de grises secuencial.
        // El desenfoque será implementado en el próximo incremento.
        if (selectedFilter == FilterType.Blur)
        {
            Console.WriteLine(
                "El filtro de desenfoque todavía no está implementado."
            );

            Console.ReadKey();
            return;
        }

        if (selectedMode != ProcessingMode.Sequential)
        {
            Console.WriteLine(
                "Ese modo de procesamiento todavía no está implementado."
            );

            Console.ReadKey();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("INICIANDO PROCESAMIENTO SECUENCIAL");
        Console.WriteLine("------------------------------------------");

        ProcessingResult result =
            SequentialImageProcessor.ProcessBatch(
                imagePaths,
                paths.SequentialOutputDirectory,
                selectedFilter,
                filterValue
            );

        // Evitamos una división entre cero.
        double pixelsPerSecond =
            result.ProcessingTime.TotalSeconds > 0
                ? result.ProcessedPixels /
                  result.ProcessingTime.TotalSeconds
                : 0;

        Console.WriteLine("RESULTADOS DEL LOTE");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine(
            $"Imágenes procesadas: {result.ProcessedImages}"
        );
        Console.WriteLine(
            $"Imágenes fallidas: {result.FailedImages}"
        );
        Console.WriteLine(
            $"Píxeles procesados: {result.ProcessedPixels:N0}"
        );
        Console.WriteLine(
            $"Tiempo del filtro: " +
            $"{result.ProcessingTime.TotalMilliseconds:F2} ms"
        );
        Console.WriteLine(
            $"Píxeles por segundo: {pixelsPerSecond:N0}"
        );
        Console.WriteLine();
        Console.WriteLine("Resultados guardados en:");
        Console.WriteLine(paths.SequentialOutputDirectory);
        Console.WriteLine();
        Console.WriteLine("Presiona cualquier tecla para finalizar...");

        Console.ReadKey();
    }
}