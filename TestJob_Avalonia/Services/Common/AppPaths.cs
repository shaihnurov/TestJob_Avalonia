using System;
using System.IO;

namespace TestJob_Avalonia.Services.Common;

/// <summary>
/// Предоставляет общие для всего приложения константы для часто используемых путей файловой системы, таких как папки для журналов, данных,
/// шаблонов и изображений
/// </summary>
public static class AppPaths
{
    /// <summary>
    /// Представляет полный путь к папке данных приложения
    /// </summary>
    public static readonly string BaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestJob_Avalonia");

    /// <summary>
    /// Представляет полный путь к папке logs приложения
    /// </summary>
    public static readonly string LogFolder = Path.Combine(BaseFolder, "logs");
}