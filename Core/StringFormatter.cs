using System.Text;
using Core.Exception;

namespace Core;

public class StringFormatter : IStringFormatter {
    public static readonly StringFormatter Shared = new();
    private ExpressionsCachingService _expressionsCache = new();
    private List<System.Exception> _formattingExceptions = new();

    public string Format(string template, object target) {
        _formattingExceptions.Clear();
        var result = String.Empty;
        try {
            result = FormatString(template, target);
        }
        catch (InvalidSyntaxException e) {
            _formattingExceptions.Add(e);
        }
        if (_formattingExceptions.Count != 0)
            throw new AggregateException(_formattingExceptions);
        return result;
    }

    private string FormatString(string template, object target) {
        var formatted = new StringBuilder();
        var targetName = new StringBuilder();

        var isTargetValue = false;
        var isAppended = false;

        var bracesAmount = 0;
        var length = template.Length;
        for (var i = 0; i < length; i++) {
            switch (template[i]) {
                case '{':
                    bracesAmount++;
                    if (bracesAmount % 2 != 0) {
                        targetName.Clear();
                        isAppended = false;
                        isTargetValue = true;
                    }
                    else {
                        formatted.Append(template[i]);
                        isTargetValue = false;
                        isAppended = true;
                    }
                    break;
                case '}':
                    if (bracesAmount % 2 != 0) {
                        if (!isAppended) {
                            try {
                                var targetValue = GetTargetValueByName(target, targetName);
                                formatted.Append(targetValue);
                            }
                            catch (MemberNotFoundException e) {
                                _formattingExceptions.Add(e);
                            }
                            finally {
                                isAppended = !isAppended;
                                isTargetValue = !isTargetValue;
                            }
                        }
                    }
                    else {
                        formatted.Append(template[i]);
                    }
                    bracesAmount--;
                    break;
                default:
                    if (!isTargetValue) {
                        formatted.Append(template[i]);
                    }
                    targetName.Append(template[i]);
                    break;
            }
            if (bracesAmount < 0 || (bracesAmount != 0 && i == length - 1))
                throw new InvalidSyntaxException("Invalid braces amount");
        }
        return formatted.ToString();
    }

    private string GetTargetValueByName(object target, StringBuilder targetName) {
        var function = _expressionsCache.GetOrAdd(target, targetName);
        return function(target);
    }
}