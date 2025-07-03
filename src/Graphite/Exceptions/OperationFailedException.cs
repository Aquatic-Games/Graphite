namespace Graphite.Exceptions;

public class OperationFailedException(string text) : Exception(text);