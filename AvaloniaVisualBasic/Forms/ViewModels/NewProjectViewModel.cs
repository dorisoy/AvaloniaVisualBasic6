using System;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaEdit.Utils;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Projects;
using AvaloniaVisualBasic.Utils;
using Classic.CommonControls.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public partial class NewProjectViewModel : ObservableObject, IDialog
{
    public string Title => "New Project";
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;

    [Notify] private ProjectTemplateViewModel? selectedTemplate;

    public ObservableCollection<ProjectTemplateViewModel> Templates { get; } = new();

    public DelegateCommand Ok { get; }
    public DelegateCommand Cancel { get; }

    public NewProjectViewModel(IWindowManager windowManager)
    {
        Ok = new DelegateCommand(() =>
        {
            if (SelectedTemplate?.Supported == false)
            {
                windowManager.MessageBox("Your Avalonia Visual Basic version doesn't support " + SelectedTemplate.Name, "Avalonia Visual Basic", MessageBoxButtons.Ok, MessageBoxIcon.Information);
                return;
            }
            CloseRequested?.Invoke(true);
        }, () => SelectedTemplate != null);
        Cancel = new DelegateCommand(() => CloseRequested?.Invoke(false), () => true);
        Templates.AddRange(IProjectTemplate.Templates.Select(template => new ProjectTemplateViewModel(template)));
        SelectedTemplate = Templates.FirstOrDefault();
    }

    private void OnSelectedTemplateChanged()
    {
        Ok.RaiseCanExecutedChanged();
    }
}