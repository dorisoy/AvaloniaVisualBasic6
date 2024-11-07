using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvaloniaVisualBasic.IDE;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.Forms.ViewModels;

public partial class AddInManagerViewModel : ObservableObject, IDialog
{
    public string Title => "Add-In Manager";
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;

    public ObservableCollection<AddInViewModel> AddIns { get; }
    [Notify] private AddInViewModel? selectedAddIn;

    public AddInManagerViewModel()
    {
        AddIns = new()
        {
            new AddInViewModel("Avalonia Visual Basic IDE", "This is the main AVB6 module which can't be disabled.", true, true, false),
        };
        SelectedAddIn = AddIns[0];
    }

    public void Close()
    {
        CloseRequested?.Invoke(false);
    }
}

public partial class AddInViewModel : ObservableObject
{
    public string Name { get; }
    public string Description { get; }
    [Notify] [AlsoNotify(nameof(LoadBehavior))] private bool startup;
    [Notify] [AlsoNotify(nameof(LoadBehavior))] private bool loaded;
    [Notify] [AlsoNotify(nameof(LoadBehavior))] private bool commandline;

    public string LoadBehavior
    {
        get
        {
            if (Startup && Loaded && commandline)
                return "ALL";

            if (!Startup && !loaded && !commandline)
                return "";

            List<string> behaviors = new();
            if (startup)
                behaviors.Add("Startup");
            if (loaded)
                behaviors.Add("Loaded");
            else if (behaviors.Count > 0)
                behaviors.Add("Unloaded");
            if (commandline)
                behaviors.Add("Command Line");
            return string.Join(" / ", behaviors);
        }
    }

    public AddInViewModel(string name, string description, bool startup, bool loaded, bool commandline)
    {
        Name = name;
        Description = description;
        this.startup = startup;
        this.loaded = loaded;
        this.commandline = commandline;
    }
}