namespace TestJob_Avalonia.Models;

/// <summary>
/// Строка блока Информация о разработчике
/// </summary>
/// <param name="Icon">Код иконки из шрифта Lucide</param>
/// <param name="Label">Подпись поля (например Email)</param>
/// <param name="Value">Значение поля</param>
public sealed record DevInfoItem(string Icon, string Label, string Value);