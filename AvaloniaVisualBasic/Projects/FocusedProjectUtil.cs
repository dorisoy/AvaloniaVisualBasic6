using System;
using AvaloniaVisualBasic.Forms.ViewModels;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.VisualDesigner;
using PropertyChanged.SourceGenerator;
using R3;

namespace AvaloniaVisualBasic.Projects;

public partial class FocusedProjectUtil : IFocusedProjectUtil
{
    private readonly IMdiWindowManager mdiWindowManager;
    private readonly IProjectManager projectManager;
    [Notify] private ProjectDefinition? focusedProject;
    [Notify] private ProjectDefinition? focusedOrStartupProject;
    [Notify] private FormDefinition? focusedForm;
    [Notify] private string focusedComponentPosition = "0, 0";
    [Notify] private string focusedComponentSize = "0, 0";

    private System.IDisposable? activeWindowDisposable;

    public FocusedProjectUtil(IMdiWindowManager mdiWindowManager,
        IProjectManager projectManager)
    {
        this.mdiWindowManager = mdiWindowManager;
        this.projectManager = projectManager;
        mdiWindowManager.ObservePropertyChanged(x => x.ActiveWindow)
            .Subscribe(_ => Update());
        projectManager.ObservePropertyChanged(x => x.StartupProject)
            .Subscribe(_ => Update());
    }

    private void Update()
    {
        if (mdiWindowManager.ActiveWindow is FormEditViewModel formEdit)
        {
            FocusedForm = formEdit.FormDefinition;
            FocusedProject = FocusedForm?.Owner;
        }
        else if (mdiWindowManager.ActiveWindow is CodeEditorViewModel codeEdit)
        {
            FocusedForm = codeEdit.FormDefinition;
            FocusedProject = FocusedForm?.Owner;
        }
        else
        {
            FocusedProject = null;
            FocusedForm = null;
        }
        FocusedOrStartupProject = FocusedProject ?? projectManager.StartupProject;


        if (mdiWindowManager.ActiveWindow is FormEditViewModel formEdit2)
        {
            UpdateControlPositionBinding(formEdit2);
        }
        else
        {
            UpdateControlPositionBinding(null);
        }
    }

    private void UpdateControlPositionBinding(FormEditViewModel? formEditor)
    {
        FocusedComponentPosition = "0, 0";
        FocusedComponentSize = "0, 0";
        activeWindowDisposable?.Dispose();
        activeWindowDisposable = null;

        if (formEditor == null)
            return;

        var left = formEditor.ObservePropertyChanged(x => x.SelectedComponent)
            .Where(x => x != null)
            .SelectMany(x => x!.ObservePropertyChanged(y => y.Left));
        var top = formEditor.ObservePropertyChanged(x => x.SelectedComponent)
            .Where(x => x != null)
            .SelectMany(x => x!.ObservePropertyChanged(y => y.Top));
        var width = formEditor.ObservePropertyChanged(x => x.SelectedComponent)
            .Where(x => x != null)
            .SelectMany(x => x!.ObservePropertyChanged(y => y.Width));
        var height = formEditor.ObservePropertyChanged(x => x.SelectedComponent)
            .Where(x => x != null)
            .SelectMany(x => x!.ObservePropertyChanged(y => y.Height));

        activeWindowDisposable = new CompositeDisposable(left.CombineLatest(top, (a, b) => $"{a:0}, {b:0}")
            .Subscribe(position => FocusedComponentPosition = position), width.CombineLatest(height, (a, b) => $"{a:0}, {b:0}")
            .Subscribe(size => FocusedComponentSize = size));
    }
}