using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using AvaloniaVisualBasic.Runtime.BuiltinTypes;
using AvaloniaVisualBasic.Runtime.Components;
using AvaloniaVisualBasic.Runtime.Interpreter;

namespace AvaloniaVisualBasic.Runtime.BuiltinControls;

public class VBShape : Control
{
    public static readonly StyledProperty<ShapeTypes> ShapeTypeProperty = AvaloniaProperty.Register<VBShape, ShapeTypes>("ShapeType");

    public static readonly StyledProperty<double> BorderWidthProperty = AvaloniaProperty.Register<VBShape, double>("BorderWidth");

    public static readonly StyledProperty<VBColor> FillColorProperty = AvaloniaProperty.Register<VBShape, VBColor>("FillColor");

    public static readonly StyledProperty<VBColor> BorderColorProperty = AvaloniaProperty.Register<VBShape, VBColor>("BorderColor");

    public static readonly StyledProperty<BorderStyles> BorderStyleProperty = AvaloniaProperty.Register<VBShape, BorderStyles>("BorderStyle");

    public static readonly StyledProperty<FillStyles> FillStyleProperty = AvaloniaProperty.Register<VBShape, FillStyles>("FillStyle");

    public static readonly StyledProperty<VBColor> BackColorProperty = AvaloniaProperty.Register<VBShape, VBColor>("BackColor");

    public static readonly StyledProperty<BackStyles> BackStyleProperty = AvaloniaProperty.Register<VBShape, BackStyles>("BackStyle");

    public ShapeTypes ShapeType
    {
        get => GetValue(ShapeTypeProperty);
        set => SetValue(ShapeTypeProperty, value);
    }

    public double BorderWidth
    {
        get => GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    public VBColor FillColor
    {
        get => GetValue(FillColorProperty);
        set => SetValue(FillColorProperty, value);
    }

    public VBColor BorderColor
    {
        get => GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public BorderStyles BorderStyle
    {
        get => GetValue(BorderStyleProperty);
        set => SetValue(BorderStyleProperty, value);
    }

    public FillStyles FillStyle
    {
        get => GetValue(FillStyleProperty);
        set => SetValue(FillStyleProperty, value);
    }

    public VBColor BackColor
    {
        get => GetValue(BackColorProperty);
        set => SetValue(BackColorProperty, value);
    }

    public BackStyles BackStyle
    {
        get => GetValue(BackStyleProperty);
        set => SetValue(BackStyleProperty, value);
    }

    static VBShape()
    {
        AffectsRender<VBShape>(ShapeTypeProperty,
            BorderWidthProperty,
            FillColorProperty,
            BorderColorProperty,
            BorderStyleProperty,
            FillStyleProperty,
            BackColorProperty,
            BackStyleProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        IBrush? fillBrush = FillStyle == FillStyles.Solid ? FillColor.ToBrush() : null;
        IPen? borderPen = BorderStyle == BorderStyles.Solid ? new Pen(BorderColor.ToBrush(), BorderWidth) : null;
        var width = Bounds.Width - (BorderStyle == BorderStyles.Transparent ? 0 : BorderWidth);
        var height = Bounds.Height - (BorderStyle == BorderStyles.Transparent ? 0 : BorderWidth);
        var x = BorderStyle == BorderStyles.Solid ? BorderWidth / 2 : 0;
        var y = BorderStyle == BorderStyles.Solid ? BorderWidth / 2 : 0;
        var size = Math.Min(width, height);

        IReadOnlyList<Control>? visualBrush = null;
        if (FillStyle == FillStyles.HorizontalLine)
        {
            visualBrush =
            [
                new Line()
                {
                    StrokeThickness = 1,
                    Stroke = FillColor.ToBrush(),
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(8, 0)
                }
            ];
        }
        else if (FillStyle == FillStyles.VerticalLine)
        {
            visualBrush =
            [
                new Line()
                {
                    StrokeThickness = 1,
                    Stroke = FillColor.ToBrush(),
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 8)
                }
            ];
        }
        else if (FillStyle == FillStyles.UpwardDiagonal)
        {
            visualBrush =
            [
                new Line()
                {
                    StrokeThickness = 1,
                    Stroke = FillColor.ToBrush(),
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(8, 8)
                }
            ];
        }
        else if (FillStyle == FillStyles.DownwardDiagonal)
        {
            visualBrush =
            [
                new Line()
                {
                    StrokeThickness = 1,
                    Stroke = FillColor.ToBrush(),
                    StartPoint = new Point(8, 0),
                    EndPoint = new Point(0, 8)
                }
            ];
        }
        else if (FillStyle == FillStyles.Cross)
        {
            visualBrush =
            [
                new Line()
                {
                    StrokeThickness = 1,
                    Stroke = FillColor.ToBrush(),
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(8, 0)
                },
                new Line()
                {
                    StrokeThickness = 1,
                    Stroke = FillColor.ToBrush(),
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 8)
                }
            ];
        }
        else if (FillStyle == FillStyles.DiagonalCross)
        {
            visualBrush =
            [
                new Line()
                {
                    StrokeThickness = 1,
                    Stroke = FillColor.ToBrush(),
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(8, 8)
                },
                new Line()
                {
                    StrokeThickness = 1,
                    Stroke = FillColor.ToBrush(),
                    StartPoint = new Point(8, 0),
                    EndPoint = new Point(0, 8)
                }
            ];
        }

        if (visualBrush != null)
        {
            var panel = new Panel();
            panel.Children.AddRange(visualBrush);
            fillBrush = new VisualBrush(panel)
            {
                SourceRect = new RelativeRect(0, 0, 8, 8, RelativeUnit.Absolute),
                DestinationRect = new RelativeRect(0, 0, 8, 8, RelativeUnit.Absolute),
                TileMode = TileMode.Tile,
                Stretch = Stretch.None,
            };
        }

        var rect = new Rect(x, y, width, height);

        if (ShapeType is ShapeTypes.Square or ShapeTypes.Circle or ShapeTypes.RoundedSquare)
        {
            rect = new Rect(Bounds.Width / 2 - size / 2, Bounds.Height / 2 - size / 2, size, size);
        }

        switch (ShapeType)
        {
            case ShapeTypes.Rectangle:
            case ShapeTypes.Square:
                if (BackStyle == BackStyles.Opaque)
                    context.FillRectangle(BackColor.ToBrush(), rect);
                context.DrawRectangle(fillBrush, borderPen, rect);
                break;
            case ShapeTypes.Oval:
            case ShapeTypes.Circle:
                if (BackStyle == BackStyles.Opaque)
                    context.DrawEllipse(BackColor.ToBrush(), null, rect);
                context.DrawEllipse(fillBrush, borderPen, rect);
                break;
            case ShapeTypes.RoundedRectangle:
                if (BackStyle == BackStyles.Opaque)
                    context.DrawRectangle(BackColor.ToBrush(), null, rect, width * 0.01, height * 0.01);
                context.DrawRectangle(fillBrush, borderPen, rect, width * 0.01, height * 0.01);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public IReadOnlyList<PropertyClass> AccessibleProperties { get; } =
    [
        VBProperties.LeftProperty,
        VBProperties.TopProperty,
        VBProperties.WidthProperty,
        VBProperties.HeightProperty
    ];

    public Vb6Value? GetPropertyValue(PropertyClass property)
    {
        if (property == VBProperties.LeftProperty)
            return (int)Canvas.GetLeft(this);
        if (property == VBProperties.TopProperty)
            return (int)Canvas.GetTop(this);
        if (property == VBProperties.WidthProperty)
            return (int)this.Width;
        if (property == VBProperties.HeightProperty)
            return (int)this.Height;
        return null;
    }

    public void SetPropertyValue(PropertyClass property, Vb6Value value)
    {
        if (property == VBProperties.LeftProperty)
            Canvas.SetLeft(this, value.Value as int? ?? value.Value as float? ?? value.Value as double? ?? 0);
        if (property == VBProperties.TopProperty)
            Canvas.SetTop(this, value.Value as int? ?? value.Value as float? ?? value.Value as double? ?? 0);
        if (property == VBProperties.WidthProperty)
            this.Width = value.Value as int? ?? value.Value as float? ?? value.Value as double? ?? 0;
        if (property == VBProperties.HeightProperty)
            this.Height = value.Value as int? ?? value.Value as float? ?? value.Value as double? ?? 0;
    }
}