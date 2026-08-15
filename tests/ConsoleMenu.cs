using ProcesadorImagenesParalelo.Models;

namespace ProcesadorImagenesParalelo.ConsoleUI;

/// <summary>
/// Contiene los menús y validaciones relacionados con la consola.
/// </summary>
public static class ConsoleMenu
{
    /// <summary>
    /// Muestra el encabezado principal de la aplicación.
    /// </summary>
    public static void ShowHeader()
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("   PROCESADOR PARALELO DE IMÁGENES");
        Console.WriteLine("==========================================");
        Console.WriteLine();
    }

    /// <summary>
    /// Solicita al usuario uno de los filtros disponibles.
    /// El ciclo continúa hasta recibir una opción válida.
    /// </summary>
    public static FilterType SelectFilter()
    {
        while (true)
        {
            Console.WriteLine("SELECCIONE UN FILTRO");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("1. Escala de grises");
            Console.WriteLine("2. Inversión de colores");
            Console.WriteLine("3. Brillo");
            Console.WriteLine("4. Contraste");
            Console.WriteLine("5. Desenfoque");
            Console.Write("Opción: ");

            string? input = Console.ReadLine();

            bool isNumber = int.TryParse(input, out int option);
            bool isValidFilter = Enum.IsDefined(
                typeof(FilterType),
                option
            );

            if (isNumber && isValidFilter)
            {
                return (FilterType)option;
            }

            ShowInvalidOptionMessage();
        }
    }

    /// <summary>
    /// Solicita el modo de procesamiento que utilizará la aplicación.
    /// </summary>
    public static ProcessingMode SelectProcessingMode()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("SELECCIONE EL MODO DE PROCESAMIENTO");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("1. Secuencial");
            Console.WriteLine("2. Paralelo por filas");
            Console.WriteLine("3. División recursiva");
            Console.WriteLine("4. Lote paralelo");
            Console.WriteLine("5. Comparar los modos");

            string? input = Console.ReadLine();

            bool isNumber = int.TryParse(input, out int option);
            bool isValidMode = Enum.IsDefined(
                typeof(ProcessingMode),
                option
            );

            if (isNumber && isValidMode)
            {
                return (ProcessingMode)option;
            }

            ShowInvalidOptionMessage();
        }
    }

    /// <summary>
    /// Muestra un mensaje cuando el usuario introduce
    /// una opción que no pertenece al menú.
    /// </summary>
    private static void ShowInvalidOptionMessage()
    {
        Console.WriteLine();
        Console.WriteLine("Opción no válida. Inténtelo nuevamente.");
        Console.WriteLine();
    }

    /// <summary>
    /// Solicita el valor adicional requerido por brillo o contraste.
    /// Para los demás filtros devuelve cero.
    /// </summary>
    public static int SelectFilterValue(FilterType selectedFilter)
    {
        return selectedFilter switch
        {
            FilterType.Brightness => ReadValueInRange(
                "Ajuste de brillo (-255 a 255): ",
                -255,
                255
            ),

            FilterType.Contrast => ReadValueInRange(
                "Ajuste de contraste (-100 a 100): ",
                -100,
                100
            ),

            FilterType.Blur => ReadValueInRange(
                "Radio de desenfoque (1 a 10): ",
                1,
                10
            ),

            _ => 0
        };
    }

    /// <summary>
    /// Solicita un número entero dentro del rango indicado.
    /// </summary>
    private static int ReadValueInRange(
        string message,
        int minimum,
        int maximum)
    {
        while (true)
        {
            Console.Write(message);

            string? input = Console.ReadLine();

            bool isValidNumber =
                int.TryParse(input, out int value);

            if (isValidNumber &&
                value >= minimum &&
                value <= maximum)
            {
                return value;
            }

            Console.WriteLine(
                $"Introduce un número entre {minimum} y {maximum}."
            );
        }
    }
}