using System;
using System.Threading.Tasks;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public abstract class VB6Visitor<T> : VB6BaseVisitor<T>
{
    public bool TryUnpack<T>(Vb6Value val, out T tout)
    {
        tout = default!;
        if (typeof(T) == typeof(int))
        {
            if (val.Type == Vb6Value.ValueType.Integer)
            {
                tout = (T)(object)(int)val.Value!;
                return true;
            }
            return false;
        }
        if (typeof(T) == typeof(bool))
        {
            if (val.Type == Vb6Value.ValueType.Boolean)
            {
                tout = (T)(object)(bool)val.Value!;
                return true;
            }
            return false;
        }
        if (typeof(T) == typeof(float))
        {
            if (val.Type == Vb6Value.ValueType.Integer)
            {
                tout = (T)(object)(float)(int)val.Value!;
                return true;
            }
            if (val.Type == Vb6Value.ValueType.Single)
            {
                tout = (T)(object)(float)val.Value!;
                return true;
            }
            return false;
        }
        if (typeof(T) == typeof(double))
        {
            if (val.Type == Vb6Value.ValueType.Integer)
            {
                tout = (T)(object)(double)(int)val.Value!;
                return true;
            }
            if (val.Type == Vb6Value.ValueType.Single)
            {
                tout = (T)(object)(float)val.Value!;
                return true;
            }
            if (val.Type == Vb6Value.ValueType.Double)
            {
                tout = (T)(object)(double)val.Value!;
                return true;
            }
            return false;
        }

        return false;
    }

    public bool TryUnpack<T>(Vb6Value left, Vb6Value right, out T tleft, out T tright)
    {
        tright = default!;
        return TryUnpack(left, out tleft) && TryUnpack(right, out tright);
    }

    public bool TryConvert(ref Vb6Value a, ref Vb6Value b)
    {
        if (a.Type == b.Type)
            return true;

        if (a.Type == Vb6Value.ValueType.Integer &&
            b.Type == Vb6Value.ValueType.String)
            return TryConvert(ref b, ref a);

        if (a.Type == Vb6Value.ValueType.String &&
            b.Type == Vb6Value.ValueType.Integer)
        {
            if (int.TryParse(a.Value?.ToString() ?? "", out var aInt))
            {
                a = new Vb6Value(aInt);
                return true;
            }
            else
                throw new Exception("Type mismatch");
        }
        throw new Exception("Type mismatch");
    }
}