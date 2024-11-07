using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AvaloniaVisualBasic.Runtime.Components;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Runtime.ProjectElements;

public partial class FormDefinition
{
    public ProjectDefinition Owner { get; }

    public FormDefinition(ProjectDefinition owner, string name)
    {
        Owner = owner;
        Code = "Private Sub Form_Load()\n\nEnd Sub";
        components = new List<ComponentInstance>()
        {
            new ComponentInstance(FormComponentClass.Instance, name)
                .SetProperty(VBProperties.WidthProperty, 400)
                .SetProperty(VBProperties.HeightProperty, 300)
                .SetProperty(VBProperties.CaptionProperty, name)
        };
    }

    [Notify] private string? absolutePath;

    private List<ComponentInstance> components = new();

    public IReadOnlyList<ComponentInstance> Components => components;

    public string Code { get; private set; }

    public string Name => Components.Single(x => x.BaseClass == FormComponentClass.Instance)
        .GetPropertyOrDefault(VBProperties.NameProperty) ?? throw new Exception("Form without a name!");

    public void UpdateCode(string newCode) => Code = newCode;

    public void UpdateComponents(IReadOnlyList<ComponentInstance> components)
    {
        this.components.Clear();
        this.components.AddRange(components);
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Name)));
    }
    //
    // public event Action<FormDefinition>? OnChanged;
}