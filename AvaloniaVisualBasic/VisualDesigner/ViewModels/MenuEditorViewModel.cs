using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using AvaloniaVisualBasic.IDE;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged.SourceGenerator;

namespace AvaloniaVisualBasic.VisualDesigner;

public partial class MenuEditorViewModel : ObservableObject, IDialog
{
    [Notify] private MenuViewModel? selectedMenu;

    public ObservableCollection<MenuViewModel> FlatMenu { get; } = new();

    public List<MenuViewModel> Deleted { get; } = new();

    public MenuEditorViewModel()
    {

    }

    public MenuEditorViewModel(FormEditViewModel form)
    {
        void AddRecur(ComponentInstance menu, int level)
        {
            var vm = new MenuViewModel(menu);
            vm.Indent = level;
            FlatMenu.Add(vm);
            if (menu.GetPropertyOrDefault(MenuComponentClass.SubItemsProperty) is { } subItems)
            {
                foreach (var subItem in subItems)
                    AddRecur(subItem, level + 1);
            }
        }
        foreach (var menu in form.TopLevelMenu)
            AddRecur(menu.Instance, 0);
    }

    public void Accept()
    {
        CloseRequested?.Invoke(true);
    }

    public void Cancel()
    {
        CloseRequested?.Invoke(false);
    }

    public void Next()
    {
        if (selectedMenu == null)
            return;

        var indexOf = FlatMenu.IndexOf(selectedMenu) + 1;
        if (indexOf >= FlatMenu.Count)
            indexOf = 0;

        SelectedMenu = FlatMenu[indexOf];
    }

    public void Insert()
    {
        var indexOf = selectedMenu == null ? Math.Max(0, FlatMenu.Count - 1) : FlatMenu.IndexOf(selectedMenu) + 1;
        var newMenu = new MenuViewModel();
        FlatMenu.Insert(indexOf, newMenu);
        newMenu.Indent = selectedMenu?.Indent ?? 0;
        SelectedMenu = newMenu;
    }

    public void Delete()
    {
        if (selectedMenu == null)
            return;

        var indexOf = FlatMenu.IndexOf(selectedMenu);
        FlatMenu.RemoveAt(indexOf);
        indexOf = Math.Min(indexOf, FlatMenu.Count - 1);
        Deleted.Add(selectedMenu);
        if (indexOf >= 0)
            SelectedMenu = FlatMenu[indexOf];
        else
            SelectedMenu = null;
    }

    public void MoveLeft()
    {
        if (selectedMenu == null)
            return;

        selectedMenu.Indent = Math.Max(0, selectedMenu.Indent - 1);
    }

    public void MoveRight()
    {
        if (selectedMenu == null)
            return;

        selectedMenu.Indent = selectedMenu.Indent + 1;
    }

    public void MoveUp()
    {
        if (selectedMenu == null)
            return;

        var indexOf = FlatMenu.IndexOf(selectedMenu);
        if (indexOf > 0)
        {
            FlatMenu.Move(indexOf, indexOf - 1);
        }
    }

    public void MoveDown()
    {
        if (selectedMenu == null)
            return;

        var indexOf = FlatMenu.IndexOf(selectedMenu);
        if (indexOf < FlatMenu.Count - 1)
        {
            FlatMenu.Move(indexOf, indexOf + 1);
        }
    }

    public string Title => "Menu Editor";
    public bool CanResize => false;
    public event Action<bool>? CloseRequested;
}

public partial class MenuViewModel : ObservableObject
{
    public ComponentInstance? Menu { get; }
    [Notify] private string caption;
    [Notify] private string name;
    [Notify] private bool isChecked;
    [Notify] private bool isEnable = true;
    [Notify] private bool isVisible = true;
    [Notify] private bool isWindowsList;
    [Notify] private int indent;

    public MenuViewModel()
    {
    }

    public MenuViewModel(ComponentInstance menu)
    {
        Menu = menu;
        caption = menu.GetPropertyOrDefault(VBProperties.CaptionProperty) ?? "";
        name = menu.GetPropertyOrDefault(VBProperties.NameProperty);
        isChecked = menu.GetPropertyOrDefault(VBProperties.CheckedProperty);
        isEnable = menu.GetPropertyOrDefault(VBProperties.EnabledProperty);
        isVisible = menu.GetPropertyOrDefault(VBProperties.VisibleProperty);
        isWindowsList = menu.GetPropertyOrDefault(VBProperties.WindowListProperty);
    }

    public void Apply(ComponentInstance menu)
    {
        menu.SetProperty(VBProperties.CaptionProperty, caption);
        menu.SetProperty(VBProperties.NameProperty, name);
        menu.SetProperty(VBProperties.EnabledProperty, isEnable);
        menu.SetProperty(VBProperties.VisibleProperty, isVisible);
        menu.SetProperty(VBProperties.CheckedProperty, isChecked);
        menu.SetProperty(VBProperties.WindowListProperty, isWindowsList);
    }
}