namespace Core.Exception; 

public class MemberNotFoundException : System.Exception {
    public MemberNotFoundException(string? message) : base(message) {
    }
}