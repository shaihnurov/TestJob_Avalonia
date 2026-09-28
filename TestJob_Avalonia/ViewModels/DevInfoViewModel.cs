using System.Collections.Generic;
using TestJob_Avalonia.Models;

namespace TestJob_Avalonia.ViewModels;

public partial class DevInfoViewModel : ViewModelBase
{
    public IReadOnlyList<DevInfoItem> Items { get; } =
    [
        new("\ue46b", "ФИО", "Шайхнуров Ильнар Айратович"),
        new("\ue113", "Email", "me@shaihnurov.ru"),
        new("\ue156", "Telegram", "t.me/shaihnurov"),
        new("\ue17a", "Мой стек", "Avalonia, WPF, ASP.NET Core Web API")
    ];
}