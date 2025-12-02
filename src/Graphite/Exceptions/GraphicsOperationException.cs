namespace Graphite.Exceptions;

/// <summary>
/// Represents an exception that may occur when performing graphics operations.
/// </summary>
/// <param name="message">The message.</param>
public class GraphicsOperationException(string message) : Exception(message);