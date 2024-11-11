using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using AvaloniaVisualBasic.Events;
using AvaloniaVisualBasic.Forms.ViewModels;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Projects;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.ProjectElements;
using AvaloniaVisualBasic.Runtime.Serialization;
using AvaloniaVisualBasic.Utils;
using Classic.Avalonia.Theme;
using PropertyChanged.SourceGenerator;
using R3;

namespace AvaloniaVisualBasic.VisualDesigner;

public partial class FormEditViewModel : BaseEditorWindowViewModel
{
    private FormDefinition? formDefinition;
    private readonly IEventBus eventBus;
    private readonly IProjectService projectService;
    private readonly IEditorService editorService;
    private readonly IWindowManager windowManager;
    public ToolBoxToolViewModel ToolsBoxToolViewModel { get; }
    public override string Title => $"{formDefinition?.Owner.Name} - {formDefinition?.Name} (Form)";

    public override IImage Icon { get; } =
        new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaVisualBasic/Icons/form.gif")));

    [Notify] [AlsoNotify(nameof(SelectedVisualComponent))]
    private ComponentInstanceViewModel? selectedComponent;

    public ComponentInstanceViewModel? SelectedVisualComponent
    {
        get => selectedComponent;
        set
        {
            if (value == null)
                return;

            SelectedComponent = value;
        }
    }

    public ObservableCollection<ComponentInstanceViewModel> AllComponents { get; } = new();

    public ObservableCollection<ComponentInstanceViewModel> Components { get; } = new();

    public ComponentInstanceViewModel Form { get; private set; }

    public ObservableCollection<ComponentInstanceViewModel> TopLevelMenu { get; } = new();

    public IEventBus EventBus => eventBus;

    public FormDefinition? FormDefinition => formDefinition;

    public FormEditViewModel(ToolBoxToolViewModel toolsBoxToolViewModel,
        IEventBus eventBus,
        IProjectService projectService,
        IEditorService editorService,
        IWindowManager windowManager) : this()
    {
        this.eventBus = eventBus;
        this.projectService = projectService;
        this.editorService = editorService;
        this.windowManager = windowManager;
        ToolsBoxToolViewModel = toolsBoxToolViewModel;
        AutoDispose(this.eventBus.Subscribe<ApplyAllUnsavedChangesEvent>(e =>
        {
            var positionInList = Components.Select((comp, index) => (comp, index)).ToDictionary(x => x.comp, x => x.index);
            var orderedComponents = new List<ComponentInstance>();
            foreach (var component in AllComponents.OrderBy(x => positionInList.GetValueOrDefault(x, 0)))
            {
                orderedComponents.Add(component.Instance);
            }
            formDefinition.UpdateComponents(orderedComponents);
        }));
        AutoDispose(this.eventBus.Subscribe<FormUnloadedEvent>(e =>
        {
            if (e.Form == formDefinition)
                RequestClose();
        }));
    }

    public FormEditViewModel Initialize(FormDefinition formElement)
    {
        AutoDispose(formElement.ObservePropertyChanged(x => x.Name)
            .Subscribe(name => OnPropertyChanged(new PropertyChangedEventArgs(nameof(Title)))));
        AutoDispose(formElement.Owner.ObservePropertyChanged(x => x.Name)
            .Subscribe(name => OnPropertyChanged(new PropertyChangedEventArgs(nameof(Title)))));
        formDefinition = formElement;
        AllComponents.Clear();
        Components.Clear();
        foreach (var comp in formElement.Components)
        {
            var vm = new ComponentInstanceViewModel(this, comp);
            if (comp.BaseClass != FormComponentClass.Instance)
                Components.Add(vm);
            else
                Form = vm;
            AllComponents.Add(vm);
        }

        SelectedComponent = Form;

        return this;
    }

    public FormEditViewModel()
    {
        Form = new ComponentInstanceViewModel(this, new ComponentInstance(FormComponentClass.Instance, "Form1")
            .SetProperty(VBProperties.WidthProperty, 400)
            .SetProperty(VBProperties.HeightProperty, 300)
            .SetProperty(VBProperties.CaptionProperty, "Form1"));
        AllComponents.Add(Form);
    }

    public void SpawnControl(Rect rect)
    {
        if (ToolsBoxToolViewModel.SelectedComponent?.BaseClass is not { } baseClass)
            return;

        var newName = baseClass.Name + Components.Count;
        var newComponent = new ComponentInstanceViewModel(this, new ComponentInstance(baseClass, newName)
            .SetProperty(VBProperties.CaptionProperty, newName)
            .SetProperty(VBProperties.TabIndexProperty, AllComponents.Select(x => x.Instance.GetPropertyOrDefault(VBProperties.TabIndexProperty)).DefaultIfEmpty().Max() + 1)
        )
        {
            Width = rect.Width,
            Height = rect.Height,
            Left = rect.Left,
            Top = rect.Top
        };
        Components.Add(newComponent);
        AllComponents.Add(newComponent);

        ToolsBoxToolViewModel.SelectedComponent = ToolsBoxToolViewModel.Components[0];
        SelectedComponent = newComponent;
    }

    public void BringToFront()
    {
        if (selectedComponent != null)
            BringToFront(selectedComponent);
    }

    public void BringToFront(ComponentInstanceViewModel instance)
    {
        var indexOf = Components.IndexOf(instance);
        if (indexOf != -1)
        {
            Components.Move(indexOf, Components.Count - 1);
            SelectedComponent = null; // required, otherwise GUI lose selected item
            SelectedComponent = instance;
        }
    }

    public void SendToBack()
    {
        if (selectedComponent != null)
            SendToBack(selectedComponent);
    }

    public void SendToBack(ComponentInstanceViewModel instance)
    {
        var indexOf = Components.IndexOf(instance);
        if (indexOf != -1)
        {
            Components.Move(indexOf, 0);
            SelectedComponent = null; // required, otherwise GUI lose selected item
            SelectedComponent = instance;
        }
    }

    public void CenterHorizontally()
    {
        if (selectedComponent == null)
            return;

        selectedComponent.Left = Form.Width / 2 - selectedComponent.Width / 2;
    }

    public void CenterVertically()
    {
        if (selectedComponent == null)
            return;

        selectedComponent.Top = Form.Height / 2 - selectedComponent.Height / 2;
    }

    public void DeleteSelected()
    {
        if (selectedComponent == null)
            return;

        var component = selectedComponent;

        Components.Remove(component);
        AllComponents.Remove(component);
    }

    public void SaveForm() => projectService.SaveForm(formDefinition, false).ListenErrors();

    public void SaveFormAs() => projectService.SaveForm(formDefinition, true).ListenErrors();

    public void ViewCode() => editorService.EditCode(formDefinition);

    public void ViewObject() => editorService.EditForm(formDefinition);

    public async Task EditMenu()
    {
        var vm = new MenuEditorViewModel(this);
        if (!await windowManager.ShowDialog(vm))
            return;

        foreach (var deleted in vm.Deleted)
        {
            if (deleted.Menu != null)
                AllComponents.Remove(AllComponents.First(c => c.Instance == deleted.Menu));
        }

        List<ComponentInstanceViewModel> topLevel = new();
        Stack<(int level, ComponentInstance component)> stack = new();
        foreach (var menuViewModel in vm.FlatMenu)
        {
            if (menuViewModel.Menu != null)
            {
                if (menuViewModel.Menu.TryGetProperty(MenuComponentClass.SubItemsProperty, out var subItems) &&
                    subItems != null)
                    subItems.Clear();
            }
        }
        foreach (var menuViewModel in vm.FlatMenu)
        {
            var menu = menuViewModel.Menu;
            if (menu == null)
                menu = new ComponentInstance(MenuComponentClass.Instance, menuViewModel.Name);
            menuViewModel.Apply(menu);
            if (menuViewModel.Menu == null)
                AllComponents.Add(new ComponentInstanceViewModel(this, menu));

            while (stack.Count > 0 && stack.Peek().level >= menuViewModel.Indent)
                stack.Pop();

            var parent = stack.Count > 0 ? stack.Peek().component : null;
            if (parent == null)
                topLevel.Add(AllComponents.First(c => c.Instance == menu));
            else
            {
                var elements = parent.GetPropertyOrDefault(MenuComponentClass.SubItemsProperty) ?? new();
                elements.Add(menu);
                parent.SetProperty(MenuComponentClass.SubItemsProperty, elements);
            }

            stack.Push((menuViewModel.Indent, menu));
        }

        while (TopLevelMenu.Count > 0)
            TopLevelMenu.RemoveAt(TopLevelMenu.Count - 1);
        foreach (var x in topLevel)
            TopLevelMenu.Add(x);
    }

    public void RequestCode(string subName)
    {
        editorService.EditCode(formDefinition);
        // this is a hack, the following line can be executed only after the window is created, it should be solved in a better way.
        DispatcherTimer.RunOnce(() =>
        {
            eventBus.Publish(new CreateOrNavigateToSubEvent(formDefinition, subName));
        }, TimeSpan.FromMilliseconds(16));
    }
}