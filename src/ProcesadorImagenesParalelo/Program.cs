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

        ProcessingMode selectedMode =
            ConsoleMenu.SelectProcessingMode();

        Console.WriteLine();
        Console.WriteLine("CONFIGURACIÓN SELECCIONADA");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine($"Imágenes: {imagePaths.Length}");
        Console.WriteLine($"Filtro: {selectedFilter}");
        Console.WriteLine($"Modo: {selectedMode}");
        Console.WriteLine();
        Console.WriteLine(
            "El procesamiento se implementará en el próximo paso."
        );

        Console.WriteLine();
        Console.WriteLine("Presiona cualquier tecla para finalizar...");
        Console.ReadKey();
    }
}