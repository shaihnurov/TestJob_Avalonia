using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TestJob_Avalonia.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    /// <summary>
    /// Вызывается при навигации на эту VM — здесь можно подгрузить данные
    /// </summary>
    public virtual Task Initialize() => Task.CompletedTask;
}