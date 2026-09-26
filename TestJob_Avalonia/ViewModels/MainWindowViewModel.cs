using System.Threading.Tasks;

namespace TestJob_Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public override async Task Initialize()
    {
        await base.Initialize();
    }
}