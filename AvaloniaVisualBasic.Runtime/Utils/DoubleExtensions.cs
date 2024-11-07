namespace AvaloniaVisualBasic.Runtime.Utils;

public static class DoubleExtensions
{
    public static double OrZero(this double d)
    {
        return double.IsNaN(d) ? 0 : d;
    }
}