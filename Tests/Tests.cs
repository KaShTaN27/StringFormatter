using Core;
using Core.Exception;
using Tests.Model;
using static NUnit.Framework.Assert;

namespace Tests;

public class Tests {
    
    private Motorbike _bike = new Motorbike("Yamaha TDM 900", 900);
    
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void Test_IfFormatCorrect() {
        const string expected = "I like riding on Yamaha TDM 900 with engine volume: 900";
        
        var result = StringFormatter.Shared.Format("I like riding on {Model} with engine volume: {engineVolume}", _bike);
        That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Test_WithEvenShieldingBraces() {
        const string expected = "I like riding on Yamaha TDM 900 with engine volume: {{engineVolume}}";

        var result = StringFormatter.Shared.Format("I like riding on {Model} with engine volume: {{{{engineVolume}}}}", _bike);
        That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void Test_WithOddShieldingBraces() {
        const string expected = "I like riding on Yamaha TDM 900 with engine volume: {{900}}";

        var result = StringFormatter.Shared.Format("I like riding on {Model} with engine volume: {{{{{engineVolume}}}}}", _bike);
        That(result, Is.EqualTo(expected));
    }
    
    [Test]
    public void Test_WithInvalidOpenBracesAmount() {
        try {
            StringFormatter.Shared.Format("{Model} has engine volume: {{engineVolume", _bike);
        }
        catch (AggregateException e) {
            That(e.InnerExceptions.Count, Is.EqualTo(1));
            That(e.InnerExceptions.Select(exception => exception.GetType()).Contains(typeof(InvalidSyntaxException)), Is.True);
        }
    }
    
    [Test]
    public void Test_WithInvalidClosingBracesAmount() {
        try {
            StringFormatter.Shared.Format("{Model} has engine volume: {engineVolume}}", _bike);
        }
        catch (AggregateException e) {
            That(e.InnerExceptions.Count, Is.EqualTo(1));
            That(e.InnerExceptions.Select(exception => exception.GetType()).Contains(typeof(InvalidSyntaxException)), Is.True);
        }
    }
    
    [Test]
    public void Test_IfFieldDoesNotExists() {
        try {
            StringFormatter.Shared.Format("{Model} has transmission: {transmission}", _bike);
        }
        catch (AggregateException e) {
            That(e.InnerExceptions.Count, Is.EqualTo(1));
            That(e.InnerExceptions.Select(exception => exception.GetType()).Contains(typeof(MemberNotFoundException)), Is.True);
        }
    }
    
    [Test]
    public void Test_IfFieldDoesNotExistsAndInvalidBracesSyntax() {
        try {
            StringFormatter.Shared.Format("{Model} has transmission: {transmission}}", _bike);
        }
        catch (AggregateException e) {
            That(e.InnerExceptions.Count, Is.EqualTo(2));
            var exceptionsTypes = e.InnerExceptions.Select(exception => exception.GetType()).ToList();
            That(exceptionsTypes.Contains(typeof(InvalidSyntaxException)), Is.True);
            That(exceptionsTypes.Contains(typeof(MemberNotFoundException)), Is.True);
        }
    }
}