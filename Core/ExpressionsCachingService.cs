using System.Linq.Expressions;
using System.Text;
using Core.Exception;

namespace Core; 

public class ExpressionsCachingService {

    private Dictionary<string, Func<object, string>> _expressions = new();

    public Func<object, string> GetOrAdd(object target, StringBuilder targetName) {
        var expressionKey = target.GetType() + "." + targetName;
        return _expressions.ContainsKey(expressionKey)
            ? _expressions[expressionKey]
            : Add(expressionKey, target, targetName);
    }

    private Func<object,string> Add(string expressionKey, object target,  StringBuilder targetName) {
        var parameterExpression = Expression.Parameter(typeof(object));
            var memberExpression = FindMember(target, targetName, parameterExpression);
            var methodExpression = Expression.Call(memberExpression, "ToString", null, null);
            var expression = Expression.Lambda<Func<object, string>>(methodExpression, parameterExpression)
                .Compile();
            _expressions.Add(expressionKey, expression);
            return expression;
    }

    private static MemberExpression FindMember(object target, StringBuilder targetName, ParameterExpression parameterExpression) {
        try {
            return Expression.PropertyOrField(Expression.TypeAs(parameterExpression, target.GetType()), targetName.ToString());
        }
        catch (System.Exception e) {
            throw new MemberNotFoundException("Member {" + targetName + "} does not exists");
        }
    }
}