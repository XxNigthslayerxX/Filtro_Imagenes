namespace ProcesadorImagenesParalelo.Processing;

/// <summary>
/// Busca archivos de imágenes válidos dentro de una carpeta.
/// </summary>
public static class ImageFileScanner
{
    // Extensiones admitidas inicialmente por la aplicación.
    private static readonly string[] SupportedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".bmp"
    };

    /// <summary>
    /// Devuelve las rutas de todas las imágenes encontradas.
    /// </summary>
    public static string[] FindImages(string inputDirectory)
    {
        return Directory
            .EnumerateFiles(
                inputDirectory,
                "*.*",
                SearchOption.TopDirectoryOnly
            )
            .Where(IsSupportedImage)
            .OrderBy(filePath => filePath)
            .ToArray();
    }

    /// <summary>
    /// Comprueba si la extensión del archivo está admitida.
    /// </summary>
    private static bool IsSupportedImage(string filePath)
    {
        string extension = Path.GetExtension(filePath);

        return SupportedExtensions.Contains(
            extension,
            StringComparer.OrdinalIgnoreCase
        );
    }
}