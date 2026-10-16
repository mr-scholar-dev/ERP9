using System.ComponentModel.DataAnnotations;

namespace ERP.Domain.Validations;

/// <summary>
/// Valida que un nombre de archivo termine en una de las extensiones permitidas.
/// Los campos vacíos quedan a cargo de [Required], cuando corresponde.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;

    public AllowedExtensionsAttribute(params string[] extensions)
    {
        _extensions = extensions ?? Array.Empty<string>();
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string fileName || string.IsNullOrWhiteSpace(fileName))
            return ValidationResult.Success;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (_extensions.Any(allowed => string.Equals(allowed, extension, StringComparison.OrdinalIgnoreCase)))
            return ValidationResult.Success;

        var message = ErrorMessage ??
            $"La imagen debe tener una de estas extensiones: {string.Join(", ", _extensions)}.";

        return new ValidationResult(message, new[] { validationContext.MemberName! });
    }
}
