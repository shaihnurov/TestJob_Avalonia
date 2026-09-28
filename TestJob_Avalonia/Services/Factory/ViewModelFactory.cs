using System;
using Microsoft.Extensions.DependencyInjection;

namespace TestJob_Avalonia.Services.Factory;

/// <summary>
/// Реализация фабрики ViewModel, использующая DI-контейнер для создания экземпляров
/// </summary>
/// <typeparam name="T">Тип создаваемого ViewModel</typeparam>
/// <remarks>
/// Инициализирует новый экземпляр <see cref="ViewModelFactory{T}"/>
/// </remarks>
/// <param name="serviceProvider">Провайдер служб, используемый для создания объектов</param>
public class ViewModelFactory<T>(IServiceProvider serviceProvider) : IViewModelFactory<T> where T : class
{
    /// <inheritdoc />
    public T Create(params object[] parameters) => ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
}