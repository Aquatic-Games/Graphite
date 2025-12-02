namespace Graphite.Exceptions;

/// <summary>
/// An exception thrown when a required feature(s) is not supported.
/// </summary>
public class UnsupportedFeatureException(string message) : Exception(message);