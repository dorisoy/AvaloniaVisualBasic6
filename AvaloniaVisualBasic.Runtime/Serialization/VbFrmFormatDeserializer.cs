using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;

namespace AvaloniaVisualBasic.Runtime.Serialization;

public class VbFrmFormatDeserializer
{
    private readonly Stack<VBSerializedComponent> componentStack = new Stack<VBSerializedComponent>();
    private readonly StringBuilder _codeBuilder = new StringBuilder();
    private bool parsingCode = false;

    public string Code => _codeBuilder.ToString();

    public (VBSerializedComponent, string) Deserialize(string input)
    {
        Dictionary<string, object>? nestedField = null;

        using (var reader = new StringReader(input))
        {
            string? line;
            VBSerializedComponent? rootComponent = null;

            while ((line = reader.ReadLine()) != null)
            {
                if (parsingCode)
                {
                    _codeBuilder.AppendLine(line);
                    continue;
                }
                line = line.Trim();
                if (line.StartsWith("VERSION"))
                {
                    // Skip version line
                    continue;
                }
                else if (line.StartsWith("BeginProperty"))
                {
                    nestedField = new();
                    var property = line.Split(' ', 2)[1];
                    componentStack.Peek().Properties[property] = nestedField;
                }
                else if (line.StartsWith("EndProperty"))
                {
                    nestedField = null;
                }
                else if (line.StartsWith("Begin"))
                {
                    var component = ParseBegin(line);
                    if (componentStack.Count == 0)
                    {
                        rootComponent = component;
                    }
                    else
                    {
                        componentStack.Peek().SubComponents.Add(component);
                    }
                    componentStack.Push(component);
                }
                else if (line.StartsWith("End"))
                {
                    componentStack.Pop();
                    if (componentStack.Count == 0)
                    {
                        parsingCode = true;
                    }
                }
                else
                {
                    ParseProperty(line, nestedField, componentStack.Peek());
                }
            }

            return (rootComponent ?? throw new InvalidOperationException("No root component found in input."), Code);
        }
    }

    private VBSerializedComponent ParseBegin(string line)
    {
        var tokens = line.Split(' ', 3);
        if (tokens.Length < 3)
            throw new FormatException("Invalid Begin line format.");

        return new VBSerializedComponent
        {
            Type = tokens[1],
            Name = tokens[2]
        };
    }

    private void ParseProperty(string line, Dictionary<string, object>? nestedObject, VBSerializedComponent serializedComponent)
    {
        var parts = line.Split(new[] { '=' }, 2);
        if (parts.Length != 2)
            throw new FormatException("Invalid property line format.");

        var propertyName = parts[0].Trim();
        var valueText = parts[1].Trim();

        object value = ParseValue(valueText);
        if (nestedObject != null)
            nestedObject[propertyName] = value;
        else
            serializedComponent.Properties[propertyName] = value;
    }

    private object ParseValue(string valueText)
    {
        if (VBColor.TryParse(valueText, out var vbColor))
        {
            return vbColor;
        }
        else if (valueText.StartsWith("\"") && valueText.EndsWith("\""))
        {
            return valueText.Substring(1, valueText.Length - 2);
        }
        else if (int.TryParse(valueText, out var intValue))
        {
            return intValue;
        }
        else if (double.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleValue))
        {
            return doubleValue;
        }
        else
        {
            throw new FormatException("Unsupported property value format: " + valueText);
        }
    }
}