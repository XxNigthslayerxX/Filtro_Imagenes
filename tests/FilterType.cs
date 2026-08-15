namespace ProcesadorImagenesParalelo.Models;

/// <summary>
/// Representa los filtros que pueden aplicarse a las imágenes.
/// Los valores numéricos coincidirán con las opciones del menú.
/// </summary>
public enum FilterType
{
    Grayscale = 1,
    InvertColors = 2,
    Brightness = 3,
    Contrast = 4,
    Blur = 5
}