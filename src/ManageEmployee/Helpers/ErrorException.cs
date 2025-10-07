using System.Globalization;

namespace ManageEmployee.Helpers;

// Custom exception class for throwing application specific exceptions (e.g. for validation) 
// that can be caught and handled within the application
public class ErrorException : Exception
{
    public ErrorException() : base() {}

    public ErrorException( string message) : base(message) { }

    public ErrorException(string message, params object[] args) 
        : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
    public int HttpStatusCode { get; set; }
    public bool Success { get; set; }
    public IEnumerable<object> Messages { get; set; }
}