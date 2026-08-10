namespace ProcesadorImagenesParalelo.Models;

/// <summary>
/// Centraliza las rutas utilizadas por la aplicación.
/// </summary>
public class ApplicationPaths
{
    public string InputDirectory { get; }
    public string SequentialOutputDirectory { get; }
    public string ParallelOutputDirectory { get; }
    public string RecursiveOutputDirectory { get; }

    public ApplicationPaths(string baseDirectory)
    {
        InputDirectory = Path.Combine(
            baseDirectory,
            "Input"
        );

        SequentialOutputDirectory = Path.Combine(
            baseDirectory,
            "Output",
            "Sequential"
        );

        ParallelOutputDirectory = Path.Combine(
            baseDirectory,
            "Output",
            "Parallel"
        );
        RecursiveOutputDirectory = Path.Combine(
            baseDirectory,
            "Output",
            "Recursive"
);
    }

    /// <summary>
    /// Crea las carpetas necesarias si todavía no existen.
    /// </summary>
    public void CreateDirectories()
    {
        Directory.CreateDirectory(InputDirectory);
        Directory.CreateDirectory(SequentialOutputDirectory);
        Directory.CreateDirectory(ParallelOutputDirectory);
        Directory.CreateDirectory(RecursiveOutputDirectory);
    }
}