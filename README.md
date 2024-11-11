# Avalonia Visual Basic 6

A recreation of the classic Visual Basic 6 IDE and language in C# with Avalonia.

This is purely toy and for fun project. All the rights to the Visual Basic name, icons, graphics, belong to Microsoft Corporation.

### [>> Open a web version in your browser! <<](https://bandysc.github.io/AvaloniaVisualBasic6/)

## Features

- Visual Designer
- Save/Load project in VB6 compatible format
- Run project
- VB6 language (limited)

![Avalonia Visual Basic](https://raw.githubusercontent.com/BAndysc/AvaloniaVisualBasic6/refs/heads/master/examples/img_vb6.gif)


## How to make `Make Publish` work

You need to publish standalone runtime first:
```
dotnet publish AvaloniaVisualBasic.Standalone -o standalone/ -f net8.0
```
And then copy the whole folder to the IDE folder.


## Thanks to
 - [Avalonia](https://github.com/AvaloniaUI/Avalonia)
 - [Dock by Wiesław Šoltés](https://github.com/wieslawsoltes/Dock)