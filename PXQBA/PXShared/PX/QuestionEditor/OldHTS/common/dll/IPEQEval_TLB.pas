unit IPEQEval_TLB;

// ************************************************************************ //
// WARNING                                                                    
// -------                                                                    
// The types declared in this file were generated from data read from a       
// Type Library. If this type library is explicitly or indirectly (via        
// another type library referring to this type library) re-imported, or the   
// 'Refresh' command of the Type Library Editor activated while editing the   
// Type Library, the contents of this file will be regenerated and all        
// manual modifications will be lost.                                         
// ************************************************************************ //

// PASTLWTR : 1.2
// File generated on 11.08.2010 12:26:58 from Type Library described below.

// ************************************************************************  //
// Type Lib: D:\_Projects\_Levkov\_HTS\Phase 2\WorthDLLSource\IPEQEval.tlb (1)
// LIBID: {831DB78A-42B2-4365-A346-B27915EB39FD}
// LCID: 0
// Helpfile: 
// HelpString: IPEQEval Library
// DepndLst: 
//   (1) v2.0 stdole, (D:\WINDOWS\system32\stdole2.tlb)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
{$WARN SYMBOL_PLATFORM OFF}
{$WRITEABLECONST ON}
{$VARPROPSETTER ON}
interface

uses Windows, ActiveX, Classes, Graphics, StdVCL, Variants;
  

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  IPEQEvalMajorVersion = 1;
  IPEQEvalMinorVersion = 0;

  LIBID_IPEQEval: TGUID = '{831DB78A-42B2-4365-A346-B27915EB39FD}';

  IID_IIpEqEvaluator: TGUID = '{79B198EB-F821-4A7B-9B4D-D80DF5C3E8B3}';
  CLASS_IpEqEvaluator: TGUID = '{2BA5B309-9716-4048-AF11-895816CC9E14}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum IPCommaOptions
type
  IPCommaOptions = TOleEnum;
const
  coAllowed = $00000000;
  coNotAllowed = $00000001;
  coRequired = $00000002;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IIpEqEvaluator = interface;
  IIpEqEvaluatorDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  IpEqEvaluator = IIpEqEvaluator;


// *********************************************************************//
// Interface: IIpEqEvaluator
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {79B198EB-F821-4A7B-9B4D-D80DF5C3E8B3}
// *********************************************************************//
  IIpEqEvaluator = interface(IDispatch)
    ['{79B198EB-F821-4A7B-9B4D-D80DF5C3E8B3}']
    function Get_ExprText1: WideString; safecall;
    procedure Set_ExprText1(const Value: WideString); safecall;
    function Get_ExprText2: WideString; safecall;
    procedure Set_ExprText2(const Value: WideString); safecall;
    function Get_AnswerRuleID: Integer; safecall;
    procedure Set_AnswerRuleID(Value: Integer); safecall;
    function Get_CommaOptions: Integer; safecall;
    procedure Set_CommaOptions(Value: Integer); safecall;
    function Get_AllowOrderedPairs: WordBool; safecall;
    procedure Set_AllowOrderedPairs(Value: WordBool); safecall;
    function Get_AnwerRuleCount: Integer; safecall;
    function Get_AnswerRuleText(index: Integer): WideString; safecall;
    function Evaluate: WideString; safecall;
    function Get_ListOptions: Integer; safecall;
    procedure Set_ListOptions(Value: Integer); safecall;
    function Get_TextOptions: Integer; safecall;
    procedure Set_TextOptions(Value: Integer); safecall;
    property ExprText1: WideString read Get_ExprText1 write Set_ExprText1;
    property ExprText2: WideString read Get_ExprText2 write Set_ExprText2;
    property AnswerRuleID: Integer read Get_AnswerRuleID write Set_AnswerRuleID;
    property CommaOptions: Integer read Get_CommaOptions write Set_CommaOptions;
    property AllowOrderedPairs: WordBool read Get_AllowOrderedPairs write Set_AllowOrderedPairs;
    property AnwerRuleCount: Integer read Get_AnwerRuleCount;
    property AnswerRuleText[index: Integer]: WideString read Get_AnswerRuleText;
    property ListOptions: Integer read Get_ListOptions write Set_ListOptions;
    property TextOptions: Integer read Get_TextOptions write Set_TextOptions;
  end;

// *********************************************************************//
// DispIntf:  IIpEqEvaluatorDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {79B198EB-F821-4A7B-9B4D-D80DF5C3E8B3}
// *********************************************************************//
  IIpEqEvaluatorDisp = dispinterface
    ['{79B198EB-F821-4A7B-9B4D-D80DF5C3E8B3}']
    property ExprText1: WideString dispid 1;
    property ExprText2: WideString dispid 2;
    property AnswerRuleID: Integer dispid 3;
    property CommaOptions: Integer dispid 4;
    property AllowOrderedPairs: WordBool dispid 5;
    property AnwerRuleCount: Integer readonly dispid 6;
    property AnswerRuleText[index: Integer]: WideString readonly dispid 8;
    function Evaluate: WideString; dispid 9;
    property ListOptions: Integer dispid 7;
    property TextOptions: Integer dispid 10;
  end;

// *********************************************************************//
// The Class CoIpEqEvaluator provides a Create and CreateRemote method to          
// create instances of the default interface IIpEqEvaluator exposed by              
// the CoClass IpEqEvaluator. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoIpEqEvaluator = class
    class function Create: IIpEqEvaluator;
    class function CreateRemote(const MachineName: string): IIpEqEvaluator;
  end;

implementation

uses ComObj;

class function CoIpEqEvaluator.Create: IIpEqEvaluator;
begin
  Result := CreateComObject(CLASS_IpEqEvaluator) as IIpEqEvaluator;
end;

class function CoIpEqEvaluator.CreateRemote(const MachineName: string): IIpEqEvaluator;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_IpEqEvaluator) as IIpEqEvaluator;
end;

end.
