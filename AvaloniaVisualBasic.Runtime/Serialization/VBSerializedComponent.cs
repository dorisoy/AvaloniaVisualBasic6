using System.Collections.Generic;

namespace AvaloniaVisualBasic.Runtime.Serialization;

public class VBSerializedComponent
{
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();
    public List<VBSerializedComponent> SubComponents { get; } = new List<VBSerializedComponent>();
}