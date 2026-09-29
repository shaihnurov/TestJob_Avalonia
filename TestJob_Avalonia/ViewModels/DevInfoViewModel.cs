using System.Collections.Generic;
using TestJob_Avalonia.Models;

namespace TestJob_Avalonia.ViewModels;

/// <summary>
/// ViewModel раздела Информация о разработчике (View_2)
/// </summary>
public sealed class DevInfoViewModel : ViewModelBase
{
    /// <summary>
    /// Коллекция, которая содержит информацию о разработчике приложения по ТЗ
    /// </summary>
    public IReadOnlyList<DevInfoItem> Items { get; } =
    [
        new("\ue46b", "ФИО", "Шайхнуров Ильнар Айратович"),
        new("\ue113", "Email", "me@shaihnurov.ru"),
        new("\ue156", "Telegram", "t.me/shaihnurov"),
        new("\ue17a", "Мой стек", "Avalonia, WPF, ASP.NET Core Web API")
    ];
}