using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia.Media;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;

namespace AvaloniaVisualBasic.Runtime.Serialization;

public class VbFrmFormatSerializer
{
    private readonly StringWriter builder = new StringWriter();
    private readonly Stack<string> elementStack = new Stack<string>();
    private readonly int indentSize;
    private int indentLevel = 0;

    public VbFrmFormatSerializer(int indentSize = 3, bool includeVersionHeader = true)
    {
        builder.NewLine = "\r\n";
        this.indentSize = indentSize;
        if (includeVersionHeader)
        {
            builder.WriteLine("VERSION 5.00");
        }
    }

    public void WriteCode(string code)
    {
        builder.Write(code);
    }

    public void Begin(string type, string name)
    {
        WriteIndent();
        builder.WriteLine($"Begin {type} {name}");
        elementStack.Push(name);
        indentLevel++;
    }

    public void End()
    {
        if (elementStack.Count > 0)
        {
            indentLevel--;
            WriteIndent();
            builder.WriteLine("End");
            elementStack.Pop();
        }
        else
        {
            throw new InvalidOperationException("No open elements to close.");
        }
    }

    private void WriteSimpleType(string property, Type type, object value)
    {
        WriteIndent();
        if (type == typeof(VBColor))
        {
            builder.WriteLine($"{property} =   {(VBColor)value}");
        }
        else
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    builder.WriteLine($"{property} =   \"{value}\"");
                    break;
                case TypeCode.Int32:
                    if (type.IsEnum)
                        builder.WriteLine($"{property} =   {(int)value}");
                    else
                        builder.WriteLine($"{property} =   {value}");
                    break;
                case TypeCode.Double:
                case TypeCode.Single:
                    builder.WriteLine($"{property} =   {value}");
                    break;
                case TypeCode.Boolean:
                    var boolVal = (bool)value ? -1 : 0;
                    builder.WriteLine($"{property} =   {boolVal}");  // VB-style true/false
                    break;
                default:
                    if (type.IsEnum)
                    {
                        builder.WriteLine($"{property} =   {(int)value}");
                    }
                    else
                    {
                        throw new Exception("Property type not supported: " + type);
                    }
                    break;
            }
        }
    }

    public void WriteProperty(string property, Type type, object? value)
    {
        if (value == null)
            throw new NotImplementedException("I don't know if VB has a concept of null, please confirm");

        if (type == typeof(VBFont))
        {
            WriteIndent();
            builder.WriteLine($"BeginProperty {property}");
            indentLevel++;
            var font = (VBFont)value;
            WriteSimpleType("Name", typeof(string), $"\"{font.FontFamily.Name}\"");
            WriteSimpleType("Size", typeof(float), font.Size);
            WriteSimpleType("Charset", typeof(int), 2);
            WriteSimpleType("Weight", typeof(int), (int)font.Weight);
            WriteSimpleType("Underline", typeof(bool), false);
            WriteSimpleType("Italic", typeof(bool), font.Style == FontStyle.Italic);
            WriteSimpleType("Strikethrough", typeof(bool), false);
            indentLevel--;
            WriteIndent();
            builder.WriteLine($"EndProperty");
        }
        else
        {
            WriteSimpleType(property, type, value);
        }
    }

    public string GetOutput()
    {
        if (elementStack.Count > 0)
        {
            throw new InvalidOperationException("Not all elements have been closed.");
        }

        return builder.ToString();
    }

    private void WriteIndent()
    {
        builder.Write(new string(' ', indentLevel * indentSize));
    }
}