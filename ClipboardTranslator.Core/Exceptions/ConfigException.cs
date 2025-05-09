namespace ClipboardTranslator.Core.Exceptions;

public class ConfigException : Exception
{
    public ConfigException() { }

    public ConfigException(string message) : base(message) { }

    public ConfigException(string message, Exception innerException)
        : base(message, innerException) { }
}
