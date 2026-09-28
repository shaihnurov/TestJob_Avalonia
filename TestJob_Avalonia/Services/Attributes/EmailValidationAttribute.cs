using System.ComponentModel.DataAnnotations;

namespace TestJob_Avalonia.Services.Attributes;

/// <summary>
/// Атрибут валидации для проверки корректности email адреса
/// </summary>
public sealed class EmailValidationAttribute() : ValidationAttribute(DefaultErrorMessage)
{
    /// <summary>
    /// Сообщение об ошибке по умолчанию, если email некорректен
    /// </summary>
    private static readonly string DefaultErrorMessage = "Пожалуйста, введите действительный адрес электронной почты";

    /// <summary>
    /// Выполняет валидацию значения, проверяя, является ли оно корректным
    /// </summary>
    /// <param name="value">Значение для проверки</param>
    /// <param name="validationContext">Контекст валидации (не используется)</param>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email)
            return new ValidationResult(ErrorMessage);

        if (string.IsNullOrWhiteSpace(email))
            return new ValidationResult(ErrorMessage);

        var atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex == email.Length - 1)
            return new ValidationResult(ErrorMessage);

        var dotIndex = email.IndexOf('.', atIndex);
        if (dotIndex == -1 || dotIndex == atIndex + 1 || dotIndex == email.Length - 1)
            return new ValidationResult(ErrorMessage);

        for (var i = 0; i < email.Length - 1; i++)
        {
            if (email[i] == '.' && email[i + 1] == '.')
                return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
