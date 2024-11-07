using System.Collections.Generic;
using Avalonia.Input;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;

namespace AvaloniaVisualBasic.Runtime.Components;

public static class VBProperties
{
    public static Dictionary<string, List<PropertyClass>> PropertiesByName { get; } = new();

    public static PropertyClass<string> NameProperty = new PropertyClass<string>("(Name)", "Returns the name used in code to identify an object.", PropertyCategory.Misc);

    public static PropertyClass<double> LeftProperty = new PropertyClass<double>("Left", "Returns/sets the distance between the internal left edge of an object and the left edge of its container.", PropertyCategory.Position, 0);

    public static PropertyClass<double> TopProperty = new PropertyClass<double>("Top", "Returns/sets the distance between the internal top edge of an object and the top edge of its container.", PropertyCategory.Position, 0);

    public static PropertyClass<double> WidthProperty = new PropertyClass<double>("Width", "Returns/sets the width of an object.", PropertyCategory.Position, 0);

    public static PropertyClass<double> HeightProperty = new PropertyClass<double>("Height", "Returns/sets the height of an object.", PropertyCategory.Position, 0);

    public static PropertyClass<string> CaptionProperty = new PropertyClass<string>("Caption", "Returns/sets the text displayed in an object's title bar or below an object's icon.", PropertyCategory.Appearance, "");

    public static PropertyClass<object?> TagProperty = new PropertyClass<object?>("Tag", "Stores any extra data needed for your program", PropertyCategory.Misc);

    public static PropertyClass<bool> VisibleProperty = new PropertyClass<bool>("Visible", "Returns/sets a value that determines whether an object is visible or hidden.", PropertyCategory.Behavior, true);

    public static PropertyClass<bool> EnabledProperty = new PropertyClass<bool>("Enabled", "Returns/sets a value that determines whether an object can respond to user-generated events.", PropertyCategory.Behavior, true);

    public static PropertyClass<bool> CausesValidationProperty = new PropertyClass<bool>("CausesValidation", "Returns/sets whether validation occurs on the control which lost focus.", PropertyCategory.Behavior, true);

    public static PropertyClass<StandardCursorType> MousePointerProperty = new PropertyClass<StandardCursorType>("MousePointer", "Returns/sets the type of mouse pointer displayed when over part of an object.", PropertyCategory.Misc, StandardCursorType.Arrow);

    public static PropertyClass<bool> RightToLeftProperty = new PropertyClass<bool>("RightToLeft", "Determines text display direction and control visual appearance on a bidirectional system.", PropertyCategory.Behavior);

    public static PropertyClass<string?> ToolTipTextProperty = new PropertyClass<string?>("ToolTipText", "Returns/sets the text displayed when the mouse is paused over the control.", PropertyCategory.Misc);

    public static PropertyClass<int> WhatsThisHelpIdProperty = new PropertyClass<int>("WhatsThisHelpID", "Returns/sets an associated context number for an object.", PropertyCategory.Misc);

    public static PropertyClass<VBFont> FontProperty = new PropertyClass<VBFont>("Font", "Returns a Font object", PropertyCategory.Appearance, VBFont.Default);

    public static PropertyClass<VBTextAlignment> AlignmentProperty = new PropertyClass<VBTextAlignment>("Alignment", "Returns/sets the alignment of a CheckBox or OptionButton, or a control's text.", PropertyCategory.Misc);

    public static PropertyClass<VBAppearance> AppearanceProperty = new PropertyClass<VBAppearance>("Appearance", "Returns/sets whether or not an object is painted at run time with 3-D effects.", PropertyCategory.Appearance, VBAppearance._3D);

    public static PropertyClass<bool> AutoSizeProperty = new PropertyClass<bool>("AutoSize", "Determines whether a control is automatically resized to display its entire contents.", PropertyCategory.Position, false);

    public static PropertyClass<VBBorder> BorderStyleProperty = new PropertyClass<VBBorder>("BorderStyle", "Returns/sets the border style for an object.", PropertyCategory.Appearance, VBBorder.None);

    public static PropertyClass<VBColor> ForeColorProperty = new PropertyClass<VBColor>("ForeColor", "Returns/sets the the foreground color used to display text and graphics in an object..", PropertyCategory.Appearance, VBColor.FromSystemColor(VbSystemColor.Btntext));

    public static PropertyClass<bool> UseMnemonicProperty = new PropertyClass<bool>("UseMnemonic", "Returns/sets a value that specifies whether an & in a Label's Caption property defines an access key.", PropertyCategory.Misc, true);

    public static PropertyClass<bool> WordWrapProperty = new PropertyClass<bool>("WordWrap", "Returns/sets a value that determines whether a control expands to fit the text in its Caption.", PropertyCategory.Misc);

    public static readonly PropertyClass<VBAlign> AlignProperty = new PropertyClass<VBAlign>("Align",
        "Returns/sets a value that determines where an object is displayed on a form.", PropertyCategory.Position);

    public static readonly PropertyClass<bool> AutoRedrawProperty = new PropertyClass<bool>("AutoRedraw",
        "Returns/sets the output from a graphics method to a persistent bitmap.", PropertyCategory.Behavior);

    public static readonly PropertyClass<string> PictureProperty = new PropertyClass<string>("Picture",
        "Returns/sets a graphic to be displayed in a control", PropertyCategory.Appearance);

    public static PropertyClass<int> IntervalProperty = new PropertyClass<int>("Interval", "Returns/sets the number of milliseconds between calls to a Timer control's Timer event.", PropertyCategory.Misc, defaultValue: 1000);

    public static readonly PropertyClass<string> TextProperty = new PropertyClass<string>("Text", "Returns/sets the text contained in the control.", PropertyCategory.Misc, "");


    public static PropertyClass<VBColor> BackColorProperty = new PropertyClass<VBColor>("BackColor", "Returns/sets the background color used to display text and graphics in an object.", PropertyCategory.Appearance, VBColor.FromSystemColor(VbSystemColor.Btnface));

    public static PropertyClass<BackStyles> BackStyleProperty = new PropertyClass<BackStyles>("BackStyle", "Indicates whether a Label or the background of a Shape is transparent or opaque.", PropertyCategory.Appearance);

    public static PropertyClass<VBColor> BorderColorProperty = new PropertyClass<VBColor>("BorderColor", "Returns/sets the color of an object's border.", PropertyCategory.Appearance, VBColor.Black);

    public static PropertyClass<BorderStyles> ShapeBorderStyleProperty = new PropertyClass<BorderStyles>("BorderStyle", "Returns/sets the border style for an object.", PropertyCategory.Appearance, BorderStyles.Solid);

    public static PropertyClass<VBColor> FillColorProperty = new PropertyClass<VBColor>("FillColor", "Returns/sets the color used to fill in shapes, circles, and boxes.", PropertyCategory.Appearance, VBColor.Black);

    public static PropertyClass<FillStyles> FillStyleProperty = new PropertyClass<FillStyles>("FillStyle", "Returns/sets the fill style of a shape.", PropertyCategory.Appearance, FillStyles.Transparent);

    public static PropertyClass<double> BorderWidthProperty = new PropertyClass<double>("BorderWidth", "Returns or sets the width of a control's border.", PropertyCategory.Appearance, 1);

    public static PropertyClass<ShapeTypes> ShapeProperty = new PropertyClass<ShapeTypes>("Shape", "Returns/sets a value indicating the appearance of a control.", PropertyCategory.Appearance, ShapeTypes.Rectangle);

    public static PropertyClass<List<string>?> ListProperty = new PropertyClass<List<string>?>("List", "Returns/sets the items contained in a control's list portion.", PropertyCategory.List);

    public static PropertyClass<int> ListIndexProperty = new PropertyClass<int>("ListIndex", "Returns/sets selected element index.", PropertyCategory.List, -1);

    public static PropertyClass<int> LargeChangeProperty = new PropertyClass<int>("LargeChange", "Returns/sets amount of change to Value property in a scroll bar when user clicks the scroll bar area.", PropertyCategory.Behavior, 1);

    public static PropertyClass<int> SmallChangeProperty = new PropertyClass<int>("SmallChange", "Returns/sets amount of change to Value property in a scroll bar when user clicks a scroll arrow.", PropertyCategory.Behavior, 1);

    public static PropertyClass<int> MinProperty = new PropertyClass<int>("Min", "Returns/sets a scroll bar position's minimum Value property setting.", PropertyCategory.Behavior, 0);

    public static PropertyClass<int> MaxProperty = new PropertyClass<int>("Max", "Returns/sets a scroll bar position's maximum Value property setting.", PropertyCategory.Behavior, 32767);

    public static PropertyClass<int> ValueProperty = new PropertyClass<int>("Value", "Returns/sets the value of an object.", 0);

    public static PropertyClass<bool> CheckedProperty = new PropertyClass<bool>("Checked", "Returns/seta a value that determines whether a check mark is displayed next to a menu item.", PropertyCategory.Appearance);

    public static PropertyClass<bool> WindowListProperty = new PropertyClass<bool>("WindowList", "Returns/sets a value that determines whether a Menu object maintains a list of the current MDI child windows.", PropertyCategory.Misc);

    public static PropertyClass<VBCheckValue> CheckValueProperty = new PropertyClass<VBCheckValue>("Value", "Returns/sets the value of an object.", PropertyCategory.Misc);

    public static PropertyClass<bool> LockedProperty = new PropertyClass<bool>("Locked", "Determines whether a control can be edited.", PropertyCategory.Behavior, false);

    public static PropertyClass<VBStartupPosition> StartUpPositionProperty = new PropertyClass<VBStartupPosition>("StartUpPosition", "Returns or sets a value specyfing the position of a Form when it first appears.", PropertyCategory.Position, VBStartupPosition.StartUpWindowsDefault);

    public static PropertyClass<bool> ShowInTaskbarProperty = new PropertyClass<bool>("ShowInTaskbar", "Determines whether a Form or MDIForm object appears in the Windows 95 taskbar.", PropertyCategory.Misc, true);

    public static PropertyClass<int> TabIndexProperty = new PropertyClass<int>("TabIndex", "Returns/sets the order of an object within its parent form.", PropertyCategory.Behavior);

    public static PropertyClass<bool> TabStopProperty = new PropertyClass<bool>("TabStop", "Returns/sets a value indicating whether a user can use the TAB key to give the focus to an object.", PropertyCategory.Behavior, true);
}