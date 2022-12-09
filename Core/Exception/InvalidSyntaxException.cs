namespace Core.Exception; 

public class InvalidSyntaxException : System.Exception {
    public InvalidSyntaxException(string? message) : base(message) {
    }
}