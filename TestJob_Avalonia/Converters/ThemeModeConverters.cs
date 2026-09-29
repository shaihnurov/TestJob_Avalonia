using System.Collections.Generic;
using Avalonia.Data.Converters;
using ShadUI;

namespace TestJob_Avalonia.Converters;

/// <summary>
/// Конвертеры для отображения текущей темы приложения в виде иконки Lucide
/// </summary>
public static class ThemeModeConverters
{
    /// <summary>
    /// Сопоставление режима темы с кодом иконки шрифта Lucide
    /// </summary>
    private static readonly Dictionary<ThemeMode, string> Icons = new()
    {
        { ThemeMode.System, "\uE2B2" },
        { ThemeMode.Light, "\uE2B1" },
        { ThemeMode.Dark, "\uE122" }
    };

    /// <summary>
    /// Преобразует <see cref="ThemeMode"/> в код иконки Lucide для кнопки переключения темы
    /// </summary>
    public static readonly IValueConverter ToLucideIcon =
        new FuncValueConverter<ThemeMode, string>(mode => Icons.TryGetValue(mode, out var icon) ? icon : Icons[ThemeMode.System]);
}