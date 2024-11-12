using System;
using Antlr4.Runtime;

// ReSharper disable UnusedMember.Global

namespace AvaloniaVisualBasic.Runtime.Interpreter;

public class VBRunTimeException : Exception
{
    public ParserRuleContext Context { get; }
    private readonly VBStandardError stdError;

    public VBRunTimeException(ParserRuleContext context, VBStandardError stdError, string? extraMessage = null) : base(
        $"Run-time error '{stdError.ErrNo}':\n\n{stdError.Description}" + (extraMessage == null ? "" : "\n\n" + extraMessage))
    {
        Context = context;
        this.stdError = stdError;
    }

    public VBRunTimeException(ParserRuleContext context, string custom) : base($"Run-time error:\n\n{custom}")
    {
        Context = context;
    }
}

public class VBCompileErrorException : Exception
{
    public int? Line { get; init; }

    public VBCompileErrorException(string custom) : base($"Compile error:\n\n{custom}")
    {

    }
}

public class VBSubOrFunctionNotDefinedException : VBCompileErrorException
{
    public VBSubOrFunctionNotDefinedException(string ident) : base("Sub or Function not defined (" + ident + ")") {}
}
public class VBVariableNotDefinedException : VBCompileErrorException
{
    public VBVariableNotDefinedException(string ident) : base("Variable not defined (" + ident + ")") {}
}

public class VBMethodOrDataMemberNotFoundException : VBCompileErrorException
{
    public VBMethodOrDataMemberNotFoundException(string ident, ValueType type) : base($"Method or data member not found ({ident} in {type})") {}
}

public struct VBStandardError
{
    public VBStandardError(int errNo, string description)
    {
        ErrNo = errNo;
        Description = description;
    }

    public int ErrNo { get; }
    public string Description { get; }

    public static readonly VBStandardError ReturnWithoutGoSub = new(3, "Return without GoSub");
    public static readonly VBStandardError InvalidProcedureCall = new(5, "Invalid procedure call");
    public static readonly VBStandardError Overflow = new(6, "Overflow");
    public static readonly VBStandardError OutOfMemory = new(7, "Out of memory");
    public static readonly VBStandardError SubscriptOutOfRange = new(9, "Subscript out of range");
    public static readonly VBStandardError ThisArrayIsFixedOrTemporarilyLocked = new(10, "This array is fixed or temporarily locked");
    public static readonly VBStandardError DivisionByZero = new(11, "Division by zero");
    public static readonly VBStandardError TypeMismatch = new(13, "Type mismatch");
    public static readonly VBStandardError OutOfStringSpace = new(14, "Out of string space");
    public static readonly VBStandardError ExpressionTooComplex = new(16, "Expression too complex");
    public static readonly VBStandardError CantPerformRequestedOperation = new(17, "Can't perform requested operation");
    public static readonly VBStandardError UserInterruptOccurred = new(18, "User interrupt occurred");
    public static readonly VBStandardError ResumeWithoutError = new(20, "Resume without error");
    public static readonly VBStandardError OutOfStackSpace = new(28, "Out of stack space");
    public static readonly VBStandardError SubFunctionOrPropertyNotDefined = new(35, "Sub, Function, or Property not defined");
    public static readonly VBStandardError TooManyDLLApplicationClients = new(47, "Too many DLL application clients");
    public static readonly VBStandardError ErrorInLoadingDLL = new(48, "Error in loading DLL");
    public static readonly VBStandardError BadDLLCallingConvention = new(49, "Bad DLL calling convention");
    public static readonly VBStandardError InternalError = new(51, "Internal error");
    public static readonly VBStandardError BadFileNameOrNumber = new(52, "Bad file name or number");
    public static readonly VBStandardError FileNotFound = new(53, "File not found");
    public static readonly VBStandardError BadFileMode = new(54, "Bad file mode");
    public static readonly VBStandardError FileAlreadyOpen = new(55, "File already open");
    public static readonly VBStandardError DeviceIOError = new(57, "Device I/O error");
    public static readonly VBStandardError FileAlreadyExists = new(58, "File already exists");
    public static readonly VBStandardError BadRecordLength = new(59, "Bad record length");
    public static readonly VBStandardError DiskFull = new(61, "Disk full");
    public static readonly VBStandardError InputPastEndOfFile = new(62, "Input past end of file");
    public static readonly VBStandardError BadRecordNumber = new(63, "Bad record number");
    public static readonly VBStandardError TooManyFiles = new(67, "Too many files");
    public static readonly VBStandardError DeviceUnavailable = new(68, "Device unavailable");
    public static readonly VBStandardError PermissionDenied = new(70, "Permission denied");
    public static readonly VBStandardError DiskNotReady = new(71, "Disk not ready");
    public static readonly VBStandardError CantRenameWithDifferentDrive = new(74, "Can't rename with different drive");
    public static readonly VBStandardError PathFileAccessError = new(75, "Path/File access error");
    public static readonly VBStandardError PathNotFound = new(76, "Path not found");
    public static readonly VBStandardError ObjectVariableOrWithBlockVariableNotSet = new(91, "Object variable or With block variable not set");
    public static readonly VBStandardError ForLoopNotInitialized = new(92, "For loop not initialized");
    public static readonly VBStandardError InvalidPatternString = new(93, "Invalid pattern string");
    public static readonly VBStandardError InvalidUseOfNull = new(94, "Invalid use of Null");
    public static readonly VBStandardError CantCallFriendProcedureOnAnObjectThatIsNotAnInstanceOfTheDefiningClass = new(97, "Can't call Friend procedure on an object that is not an instance of the defining class");
    public static readonly VBStandardError APropertyOrMethodCallCannotIncludeAReferenceToAPrivateObjectEitherAsAnArgumentOrAsAReturnValue = new(98, "A property or method call cannot include a reference to a private object, either as an argument or as a return value");
    public static readonly VBStandardError SystemDLLCouldNotBeLoaded = new(298, "System DLL could not be loaded");
    public static readonly VBStandardError CantUseCharacterDeviceNamesInSpecifiedFileNames = new(320, "Can't use character device names in specified file names");
    public static readonly VBStandardError InvalidFileFormat = new(321, "Invalid file format");
    public static readonly VBStandardError CantCreateNecessaryTemporaryFile = new(322, "Cant create necessary temporary file");
    public static readonly VBStandardError InvalidFormatInResourceFile = new(325, "Invalid format in resource file");
    public static readonly VBStandardError DataValueNamedNotFound = new(327, "Data value named not found");
    public static readonly VBStandardError IllegalParameterCantWriteArrays = new(328, "Illegal parameter; can't write arrays");
    public static readonly VBStandardError CouldNotAccessSystemRegistry = new(335, "Could not access system registry");
    public static readonly VBStandardError ComponentNotCorrectlyRegistered = new(336, "Component not correctly registered");
    public static readonly VBStandardError ComponentNotFound = new(337, "Component not found");
    public static readonly VBStandardError ComponentDidNotRunCorrectly = new(338, "Component did not run correctly");
    public static readonly VBStandardError ObjectAlreadyLoaded = new(360, "Object already loaded");
    public static readonly VBStandardError CantLoadOrUnloadThisObject = new(361, "Can't load or unload this object");
    public static readonly VBStandardError ControlSpecifiedNotFound = new(363, "Control specified not found");
    public static readonly VBStandardError ObjectWasUnloaded = new(364, "Object was unloaded");
    public static readonly VBStandardError UnableToUnloadWithinThisContext = new(365, "Unable to unload within this context");
    public static readonly VBStandardError TheSpecifiedFileIsOutOfDateThisProgramRequiresALaterVersion = new(368, "The specified file is out of date. This program requires a later version");
    public static readonly VBStandardError TheSpecifiedObjectCantBeUsedAsAnOwnerFormForShow = new(371, "The specified object can't be used as an owner form for Show");
    public static readonly VBStandardError InvalidPropertyValue = new(380, "Invalid property value");
    public static readonly VBStandardError InvalidPropertyArrayIndex = new(381, "Invalid property-array index");
    public static readonly VBStandardError PropertySetCantBeExecutedAtRunTime = new(382, "Property Set can't be executed at run time");
    public static readonly VBStandardError PropertySetCantBeUsedWithAReadOnlyProperty = new(383, "Property Set can't be used with a read-only property");
    public static readonly VBStandardError NeedPropertyArrayIndex = new(385, "Need property-array index");
    public static readonly VBStandardError PropertySetNotPermitted = new(387, "Property Set not permitted");
    public static readonly VBStandardError PropertyGetCantBeExecutedAtRunTime = new(393, "Property Get can't be executed at run time");
    public static readonly VBStandardError PropertyGetCantBeExecutedOnWriteOnlyProperty = new(394, "Property Get can't be executed on write-only property");
    public static readonly VBStandardError FormAlreadyDisplayedCantShowModally = new(400, "Form already displayed; can't show modally");
    public static readonly VBStandardError CodeMustCloseTopmostModalFormFirst = new(402, "Code must close topmost modal form first");
    public static readonly VBStandardError PermissionToUseObjectDenied = new(419, "Permission to use object denied");
    public static readonly VBStandardError PropertyNotFound = new(422, "Property not found");
    public static readonly VBStandardError PropertyOrMethodNotFound = new(423, "Property or method not found");
    public static readonly VBStandardError ObjectRequired = new(424, "Object required");
    public static readonly VBStandardError InvalidObjectUse = new(425, "Invalid object use");
    public static readonly VBStandardError ComponentCantCreateObjectOrReturnReferenceToThisObject = new(429, "Component can't create object or return reference to this object");
    public static readonly VBStandardError ClassDoesntSupportAutomation = new(430, "Class doesn't support Automation");
    public static readonly VBStandardError FileNameOrClassNameNotFoundDuringAutomationOperation = new(432, "File name or class name not found during Automation operation");
    public static readonly VBStandardError ObjectDoesntSupportThisPropertyOrMethod = new(438, "Object doesn't support this property or method");
    public static readonly VBStandardError AutomationError = new(440, "Automation error");
    public static readonly VBStandardError ConnectionToTypeLibraryOrObjectLibraryForRemoteProcessHasBeenLost = new(442, "Connection to type library or object library for remote process has been lost");
    public static readonly VBStandardError AutomationObjectDoesntHaveADefaultValue = new(443, "Automation object doesn't have a default value");
    public static readonly VBStandardError ObjectDoesntSupportThisAction = new(445, "Object doesn't support this action");
    public static readonly VBStandardError ObjectDoesntSupportNamedArguments = new(446, "Object doesn't support named arguments");
    public static readonly VBStandardError ObjectDoesntSupportCurrentLocaleSetting = new(447, "Object doesn't support current locale setting");
    public static readonly VBStandardError NamedArgumentNotFound = new(448, "Named argument not found");
    public static readonly VBStandardError ArgumentNotOptionalOrInvalidPropertyAssignment = new(449, "Argument not optional or invalid property assignment");
    public static readonly VBStandardError WrongNumberOfArgumentsOrInvalidPropertyAssignment = new(450, "Wrong number of arguments or invalid property assignment");
    public static readonly VBStandardError ObjectNotACollection = new(451, "Object not a collection");
    public static readonly VBStandardError InvalidOrdinal = new(452, "Invalid ordinal");
    public static readonly VBStandardError SpecifiedNotFound = new(453, "Specified not found");
    public static readonly VBStandardError CodeResourceNotFound = new(454, "Code resource not found");
    public static readonly VBStandardError CodeResourceLockError = new(455, "Code resource lock error");
    public static readonly VBStandardError ThisKeyIsAlreadyAssociatedWithAnElementOfThisCollection = new(457, "This key is already associated with an element of this collection");
    public static readonly VBStandardError VariableUsesATypeNotSupportedInVisualBasic = new(458, "Variable uses a type not supported in Visual Basic");
    public static readonly VBStandardError ThisComponentDoesntSupportTheSetOfEvents = new(459, "This component doesn't support the set of events");
    public static readonly VBStandardError InvalidClipboardFormat = new(460, "Invalid Clipboard format");
    public static readonly VBStandardError MethodOrDataMemberNotFound = new(461, "Method or data member not found");
    public static readonly VBStandardError TheRemoteServerMachineDoesNotExistOrIsUnavailable = new(462, "The remote server machine does not exist or is unavailable");
    public static readonly VBStandardError ClassNotRegisteredOnLocalMachine = new(463, "Class not registered on local machine");
    public static readonly VBStandardError CantCreateAutoRedrawImage = new(480, "Can't create AutoRedraw image");
    public static readonly VBStandardError InvalidPicture = new(481, "Invalid picture");
    public static readonly VBStandardError PrinterError = new(482, "Printer error");
    public static readonly VBStandardError PrinterDriverDoesNotSupportSpecifiedProperty = new(483, "Printer driver does not support specified property");
    public static readonly VBStandardError ProblemGettingPrinterInformationFromTheSystemMakeSureThePrinterIsSetUpCorrectly = new(484, "Problem getting printer information from the system. Make sure the printer is set up correctly");
    public static readonly VBStandardError InvalidPictureType = new(485, "Invalid picture type");
    public static readonly VBStandardError CantPrintFormImageToThisTypeOfPrinter = new(486, "Can't print form image to this type of printer");
    public static readonly VBStandardError CantEmptyClipboard = new(520, "Can't empty Clipboard");
    public static readonly VBStandardError CantOpenClipboard = new(521, "Can't open Clipboard");
    public static readonly VBStandardError CantSaveFileToTEMPDirectory = new(735, "Can't save file to TEMP directory");
    public static readonly VBStandardError SearchTextNotFound = new(744, "Search text not found");
    public static readonly VBStandardError ReplacementsTooLong = new(746, "Replacements too long");
    public static readonly VBStandardError OutOfMemory2 = new(31001, "Out of memory");
    public static readonly VBStandardError NoObject = new(31004, "No object");
    public static readonly VBStandardError ClassIsNotSet = new(31018, "Class is not set");
    public static readonly VBStandardError UnableToActivateObject = new(31027, "Unable to activate object");
    public static readonly VBStandardError UnableToCreateEmbeddedObject = new(31032, "Unable to create embedded object");
    public static readonly VBStandardError ErrorSavingToFile = new(31036, "Error saving to file");
    public static readonly VBStandardError ErrorLoadingFromFile = new(31037, "Error loading from file");
}