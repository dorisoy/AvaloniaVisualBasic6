using System;
using Avalonia.Controls;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public readonly struct Vb6Value : IEquatable<Vb6Value>
{
    public bool Equals(Vb6Value other) => Type == other.Type && Equals(Value, other.Value);

    public int? TryCompareTo(Vb6Value other)
    {
        if (Type != other.Type)
            return null;

        if (Type == ValueType.Integer)
            return ((int)Value!).CompareTo((int)other.Value!);
        else if (Type == ValueType.Single)
            return ((float)Value!).CompareTo((float)other.Value!);
        else if (Type == ValueType.Double)
            return ((double)Value!).CompareTo((double)other.Value!);
        else if (Type == ValueType.String)
            return String.Compare(((string)Value!), (string)other.Value!, StringComparison.Ordinal);
        else if (Type == ValueType.Boolean)
            return ((bool)Value!).CompareTo((bool)other.Value!);

        return null;
    }

    public override bool Equals(object? obj) => obj is Vb6Value other && Equals(other);

    public override int GetHashCode() => HashCode.Combine((int)Type, Value);

    public static bool operator ==(Vb6Value left, Vb6Value right) => left.Equals(right);

    public static bool operator !=(Vb6Value left, Vb6Value right) => !left.Equals(right);

    public readonly ValueType Type;
    public readonly object? Value;

    private Vb6Value(ValueType type, object? value)
    {
        Type = type;
        Value = value;
    }

    public Vb6Value(int value) : this(ValueType.Integer, value) {}
    public Vb6Value(float value) : this(ValueType.Single, value) {}
    public Vb6Value(double value) : this(ValueType.Double, value) {}
    public Vb6Value(bool value) : this(ValueType.Boolean, value) {}
    public Vb6Value(string? value) : this(value == null ? ValueType.Null : ValueType.String, value) {}

    public Vb6Value(bool? b) : this(b.HasValue ? ValueType.Boolean : ValueType.Null, b) { }

    public Vb6Value(ICSharpProxy proxy) : this(ValueType.CSharpProxyObject, proxy) { }

    public Vb6Value(Control control) : this(ValueType.Control, control) { }

    public Vb6Value(VBColor color) : this(ValueType.Color, color) { }

    public enum ValueType
    {
        Color,
        Date,
        Double,
        Single,
        File,
        Integer,
        String,
        Boolean,
        Control,
        Nothing,
        EmptyVariant,
        CSharpProxyObject,
        Null
    }

    public static implicit operator Vb6Value(int value) =>
        new Vb6Value(ValueType.Integer, value);

    public static implicit operator Vb6Value(string value) =>
        new Vb6Value(ValueType.String, value);

    public static implicit operator Vb6Value(bool value) =>
        new Vb6Value(ValueType.Boolean, value);

    public static implicit operator Vb6Value(bool? value) =>
        new Vb6Value(value);

    public static implicit operator Vb6Value(double value) =>
        new Vb6Value(ValueType.Double, value);

    public override string ToString()
    {
        return $"<{Type}>({Value})";
    }

    public static readonly Vb6Value Null = new Vb6Value(ValueType.Null, null);
    public static readonly Vb6Value Nothing = new Vb6Value(ValueType.Nothing, null);
    public static readonly Vb6Value Variant = new Vb6Value(ValueType.EmptyVariant, null);
    public bool IsNull => Type == ValueType.Null;
}