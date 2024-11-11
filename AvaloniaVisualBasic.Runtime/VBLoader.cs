using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Runtime;

public class VBLoader
{
    public static Control SpawnComponents(FormDefinition element,
        ModuleExecutionContext executionContext,
        ExecutionEnvironment environment)
    {
        var canvas = new Canvas()
        {
            ClipToBounds = true
        };
        var menu = new Menu();
        DockPanel.SetDock(menu, Avalonia.Controls.Dock.Top);
        // foreach (var topLevelMenu in TopLevelMenu)
        // {
        //     menu.Items.Add(topLevelMenu.Instance.BaseClass.Instantiate(topLevelMenu));
        // }

        foreach (var component in element.Components)
        {
            if (component.BaseClass is FormComponentClass)
                continue;

            var instance = component.BaseClass.Instantiate(component);
            Canvas.SetLeft(instance, component.GetPropertyOrDefault(VBProperties.LeftProperty));
            Canvas.SetTop(instance, component.GetPropertyOrDefault(VBProperties.TopProperty));
            instance.Width = component.GetPropertyOrDefault(VBProperties.WidthProperty);
            instance.Height = component.GetPropertyOrDefault(VBProperties.HeightProperty);
            instance.IsVisible = component.GetPropertyOrDefault(VBProperties.VisibleProperty);
            canvas.Children.Add(instance);
            if (component.GetPropertyOrDefault(VBProperties.NameProperty) is { } name)
                executionContext.AllocVariable(environment, name, new Vb6Value(instance));
        }

        return new DockPanel()
        {
            Children =
            {
                menu,
                canvas
            }
        };
    }

    public static Task RunForm(FormDefinition element, CancellationToken token, out VBFormRuntime window)
    {
        var form = element.Components.FirstOrDefault(x => x.BaseClass == FormComponentClass.Instance);
        if (form == null)
            throw new Exception("No form found");

        window = ((FormComponentClass)form.BaseClass).InstantiateWindow(form);
        if (form.GetPropertyOrDefault(VBProperties.NameProperty) is { } formName)
        {
            window.Context.ExecutionContext.AllocVariable(window.Context.RootEnv, formName, new Vb6Value(window));
            window.Context.ExecutionContext.AllocVariable(window.Context.RootEnv, "Me", new Vb6Value(window));
        }

        window.Content = SpawnComponents(element, window.Context.ExecutionContext, window.Context.RootEnv);
        window.Context.SetCode(code: element.Code);
        window.Show();
#if DEBUG
        window.AttachDevTools();
#endif

        var tcs = new TaskCompletionSource();

        token.Register((state, _) =>
        {
            (state as Window)!.Close();
        }, window);

        window.Tag = tcs;
        window.Closed += OnWindowClosed;

        return tcs.Task;
    }

    private static void OnWindowClosed(object? sender, EventArgs e)
    {
        var window = (sender as Window)!;
        var completionSource = window.Tag as TaskCompletionSource;
        completionSource?.SetResult();
        window.Closed -= OnWindowClosed;
    }
}