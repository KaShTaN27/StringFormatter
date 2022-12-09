namespace Core; 

public interface IStringFormatter {
    public string Format(string template, object target);
}