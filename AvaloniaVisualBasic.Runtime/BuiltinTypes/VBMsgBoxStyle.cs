using System;

namespace AvaloniaVisualBasic.Runtime.BuiltinTypes;

[Flags]
public enum VBMsgBoxStyle
{
    vbOKOnly = 0,
    vbOKCancel = 1,
    vbAbortRetryIgnore = 2,
    vbYesNoCancel = 3,
    vbYesNo = 4,
    vbRetryCancel = 5,
    vbCritical = 16,
    vbQuestion = 32,
    vbExclamation = 48,
    vbInformation = 64,
    vbDefaultButton1 = 0,
    vbDefaultButton2 = 256,
    vbDefaultButton3 = 512,
    vbDefaultButton4 = 768,
    vbApplicationModal = 0,
    vbSystemModal = 4096,
    vbMsgBoxHelpButton = 16384,
    vbMsgBoxSetForeground = 65536,
    vbMsgBoxRight = 524288,
    vbMsgBoxRtlReading = 1048576,

    ButtonsBits = 0b0111,
    IconBits = 0b1110000
}