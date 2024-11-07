using Avalonia.Media.Imaging;
using AvaloniaVisualBasic.Projects;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public class ProjectTemplateViewModel : ObservableObject
{
    private readonly IProjectTemplate template;

    public string Name => template.Name;

    public Bitmap Icon => template.Icon;

    public bool Supported => template.Supported;

    public IProjectTemplate? Template => template;

    public ProjectTemplateViewModel(IProjectTemplate template)
    {
        this.template = template;
    }
}