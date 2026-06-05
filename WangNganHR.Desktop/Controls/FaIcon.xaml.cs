using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace WangNganHR.Desktop.Controls;

public partial class FaIcon : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(FaIcon),
            new PropertyMetadata("circle", OnIconChanged));

    public static readonly DependencyProperty IconBrushProperty =
        DependencyProperty.Register(nameof(IconBrush), typeof(Brush), typeof(FaIcon),
            new PropertyMetadata(Brushes.Black));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Brush IconBrush
    {
        get => (Brush)GetValue(IconBrushProperty);
        set => SetValue(IconBrushProperty, value);
    }

    public FaIcon() => InitializeComponent();

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FaIcon icon && e.NewValue is string key)
            icon.PART_Icon.Kind = Map(key);
    }

    private static PackIconFontAwesomeKind Map(string key) => key switch
    {
        "clipboard-list" => PackIconFontAwesomeKind.ClipboardListSolid,
        "calendar" => PackIconFontAwesomeKind.CalendarDaysSolid,
        "bullhorn" => PackIconFontAwesomeKind.BullhornSolid,
        "rotate" => PackIconFontAwesomeKind.ArrowsRotateSolid,
        "plus" => PackIconFontAwesomeKind.PlusSolid,
        "lock" => PackIconFontAwesomeKind.LockSolid,
        "qrcode" => PackIconFontAwesomeKind.QrcodeSolid,
        "print" => PackIconFontAwesomeKind.PrintSolid,
        "save" => PackIconFontAwesomeKind.FloppyDiskSolid,
        "sign-in" => PackIconFontAwesomeKind.RightToBracketSolid,
        "search" => PackIconFontAwesomeKind.MagnifyingGlassSolid,
        "chevron-left" => PackIconFontAwesomeKind.ChevronLeftSolid,
        "chevron-right" => PackIconFontAwesomeKind.ChevronRightSolid,
        "circle-check" => PackIconFontAwesomeKind.CircleCheckSolid,
        "arrow-left" => PackIconFontAwesomeKind.ArrowLeftSolid,
        "arrow-right" => PackIconFontAwesomeKind.ArrowRightSolid,
        "user" => PackIconFontAwesomeKind.UserSolid,
        "phone" => PackIconFontAwesomeKind.PhoneSolid,
        "envelope" => PackIconFontAwesomeKind.EnvelopeSolid,
        "briefcase" => PackIconFontAwesomeKind.BriefcaseSolid,
        "clock" => PackIconFontAwesomeKind.ClockSolid,
        "eye" => PackIconFontAwesomeKind.EyeSolid,
        "calendar-check" => PackIconFontAwesomeKind.CalendarCheckSolid,
        "xmark" => PackIconFontAwesomeKind.XmarkSolid,
        "building" => PackIconFontAwesomeKind.BuildingSolid,
        "users" => PackIconFontAwesomeKind.UsersSolid,
        "route" => PackIconFontAwesomeKind.RouteSolid,
        "graduation-cap" => PackIconFontAwesomeKind.GraduationCapSolid,
        "location" => PackIconFontAwesomeKind.LocationDotSolid,
        "video" => PackIconFontAwesomeKind.VideoSolid,
        "gear" => PackIconFontAwesomeKind.GearSolid,
        _ => PackIconFontAwesomeKind.CircleSolid
    };
}
