using System.ComponentModel;
using Shared;

namespace Base.Extensions.Math;

[SpeakUpTool]
public class Math
{
    [Description("Adds two numbers together.")]
    public static async Task<double> GetSum(double a, double b, CancellationToken cancellationToken)
    {
        var result = a + b;
        return result;
    }

    [Description("Subtracts the first number from the second number.")]
    public static async Task<double> Subtract(double a, double b, CancellationToken cancellationToken)
    {
        var result = a - b;
        return result;
    }

    [Description("Calculates the product of a number and the cosine of another number.")]
    public static async Task<double> Cos(double a, double b, CancellationToken cancellationToken)
    {
        return a * System.Math.Cos(b);
    }

    [Description("Calculates the product of a number and the sine of another number.")]
    public static async Task<double> Sin(double a, double b, CancellationToken cancellationToken)
    {
        return a * System.Math.Sin(b);
    }

    [Description("Calculates the product of a number and the tangent of another number.")]
    public static async Task<double> Tan(double a, double b, CancellationToken cancellationToken)
    {
        return a * System.Math.Tan(b);
    }

    [Description("Calculates the product of two numbers.")]
    public static async Task<double> Multiply(double a, double b, CancellationToken cancellationToken)
    {
        return a * b;
    }

    [Description("Calculates the quotient of two numbers.")]
    public static async Task<double> Divide(double a, double b, CancellationToken cancellationToken)
    {
        return a / b;
    }

    [Description("Calculates the result of raising a number to the power of another number.")]
    public static async Task<double> Exponential(double a, double b, CancellationToken cancellationToken)
    {
        return System.Math.Pow(a, b);
    }

    [Description("Calculates the result of the logarithm of a number with a specified base.")]
    public static async Task<double> Logarithmic(double a, double b, CancellationToken cancellationToken)
    {
        return System.Math.Log(a, b);
    }

    [Description("Calculates the result of rounding a number to a specified number of decimal places.")]
    public static async Task<double> Round(double a, int b, CancellationToken cancellationToken)
    {
        return System.Math.Round(a, b);
    }
}