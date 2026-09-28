using System.Windows.Input;

namespace TestJob_Avalonia.Models;

/// <summary>
/// Пункт навигационного меню главного окна
/// </summary>
/// <param name="Icon">Код иконки из шрифта Lucide</param>
/// <param name="Title">Текст кнопки</param>
/// <param name="Command">Команда перехода на соответствующий раздел</param>
public sealed record NavigationItem(string Icon, string Title, ICommand Command);