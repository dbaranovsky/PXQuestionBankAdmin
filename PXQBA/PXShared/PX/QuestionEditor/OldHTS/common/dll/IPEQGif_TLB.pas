unit IPEQGif_TLB;

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
// File generated on 11.08.2010 12:15:25 from Type Library described below.

// ************************************************************************  //
// Type Lib: D:\_Projects\_Levkov\_HTS\Phase 2\WorthDLLSource\IPEQGif.tlb (1)
// LIBID: {64280ACC-056E-4E12-8888-E6287DE1E59C}
// LCID: 0
// Helpfile: 
// HelpString: IPEQGif Library
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
  IPEQGifMajorVersion = 1;
  IPEQGifMinorVersion = 0;

  LIBID_IPEQGif: TGUID = '{64280ACC-056E-4E12-8888-E6287DE1E59C}';

  IID_IIPEqGifServer: TGUID = '{2A78F87C-4F28-45E8-9DE3-DF8F5141A1F3}';
  CLASS_IPEqGifServer: TGUID = '{E5051695-3E15-40B3-AF65-6EBFE9B7AE9C}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IIPEqGifServer = interface;
  IIPEqGifServerDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  IPEqGifServer = IIPEqGifServer;


// *********************************************************************//
// Interface: IIPEqGifServer
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {2A78F87C-4F28-45E8-9DE3-DF8F5141A1F3}
// *********************************************************************//
  IIPEqGifServer = interface(IDispatch)
    ['{2A78F87C-4F28-45E8-9DE3-DF8F5141A1F3}']
    function Get_EqText: WideString; safecall;
    procedure Set_EqText(const Value: WideString); safecall;
    function Get_ExprText: WideString; safecall;
    procedure Set_ExprText(const Value: WideString); safecall;
    function Get_Top: Integer; safecall;
    procedure Set_Top(Value: Integer); safecall;
    function Get_Bottom: Integer; safecall;
    procedure Set_Bottom(Value: Integer); safecall;
    function Get_Fontsize: Integer; safecall;
    procedure Set_Fontsize(Value: Integer); safecall;
    function Get_InterpretFunctions: WordBool; safecall;
    procedure Set_InterpretFunctions(Value: WordBool); safecall;
    function Get_AlignByEq: WordBool; safecall;
    procedure Set_AlignByEq(Value: WordBool); safecall;
    function Get_Path: WideString; safecall;
    procedure Set_Path(const Value: WideString); safecall;
    procedure Save; safecall;
    function Get_Filename: WideString; safecall;
    procedure Set_Filename(const Value: WideString); safecall;
    function Get_DoNotReduce: WordBool; safecall;
    procedure Set_DoNotReduce(Value: WordBool); safecall;
    function Get_ErrorDisplayText: WideString; safecall;
    procedure Set_ErrorDisplayText(const Value: WideString); safecall;
    function Get_Error: WideString; safecall;
    function Get_IsError: WordBool; safecall;
    function Get_DoBorder: WordBool; safecall;
    procedure Set_DoBorder(Value: WordBool); safecall;
    function GetReduced(const ExprText: WideString): WideString; safecall;
    function GetImageInfo: WideString; safecall;
    property EqText: WideString read Get_EqText write Set_EqText;
    property ExprText: WideString read Get_ExprText write Set_ExprText;
    property Top: Integer read Get_Top write Set_Top;
    property Bottom: Integer read Get_Bottom write Set_Bottom;
    property Fontsize: Integer read Get_Fontsize write Set_Fontsize;
    property InterpretFunctions: WordBool read Get_InterpretFunctions write Set_InterpretFunctions;
    property AlignByEq: WordBool read Get_AlignByEq write Set_AlignByEq;
    property Path: WideString read Get_Path write Set_Path;
    property Filename: WideString read Get_Filename write Set_Filename;
    property DoNotReduce: WordBool read Get_DoNotReduce write Set_DoNotReduce;
    property ErrorDisplayText: WideString read Get_ErrorDisplayText write Set_ErrorDisplayText;
    property Error: WideString read Get_Error;
    property IsError: WordBool read Get_IsError;
    property DoBorder: WordBool read Get_DoBorder write Set_DoBorder;
  end;

// *********************************************************************//
// DispIntf:  IIPEqGifServerDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {2A78F87C-4F28-45E8-9DE3-DF8F5141A1F3}
// *********************************************************************//
  IIPEqGifServerDisp = dispinterface
    ['{2A78F87C-4F28-45E8-9DE3-DF8F5141A1F3}']
    property EqText: WideString dispid 1;
    property ExprText: WideString dispid 2;
    property Top: Integer dispid 3;
    property Bottom: Integer dispid 4;
    property Fontsize: Integer dispid 5;
    property InterpretFunctions: WordBool dispid 6;
    property AlignByEq: WordBool dispid 7;
    property Path: WideString dispid 8;
    procedure Save; dispid 9;
    property Filename: WideString dispid 11;
    property DoNotReduce: WordBool dispid 10;
    property ErrorDisplayText: WideString dispid 13;
    property Error: WideString readonly dispid 14;
    property IsError: WordBool readonly dispid 15;
    property DoBorder: WordBool dispid 12;
    function GetReduced(const ExprText: WideString): WideString; dispid 16;
    function GetImageInfo: WideString; dispid 17;
  end;

// *********************************************************************//
// The Class CoIPEqGifServer provides a Create and CreateRemote method to          
// create instances of the default interface IIPEqGifServer exposed by              
// the CoClass IPEqGifServer. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoIPEqGifServer = class
    class function Create: IIPEqGifServer;
    class function CreateRemote(const MachineName: string): IIPEqGifServer;
  end;

implementation

uses ComObj;

class function CoIPEqGifServer.Create: IIPEqGifServer;
begin
  Result := CreateComObject(CLASS_IPEqGifServer) as IIPEqGifServer;
end;

class function CoIPEqGifServer.CreateRemote(const MachineName: string): IIPEqGifServer;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_IPEqGifServer) as IIPEqGifServer;
end;

end.
