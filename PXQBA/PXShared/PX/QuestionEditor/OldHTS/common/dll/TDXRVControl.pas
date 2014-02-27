{------------------------------------------------------------------------------
  Label Item - item class for RichView.
  Non-text item that looks like a text (but cannot be wrapped and edited)
  Does not support Unicode.

  v1.5:
  impr: hypertext support
  v1.4:
  fix: correct copying by AppendFrom
  v1.3:
  fix: printing
  v1.2:
  impr: correct working with DeleteUnusedStyles
  impr: ApplyText and ApplyStyleConversion affect this item
    (if ProtectTextStyleNo=False)
  impr: can be saved in text, RTF, HTML (in RTF and HTML, it is saved as a
    plain text, i.e. it has style of the preceding text)
-------------------------------------------------------------------------------}

unit TDXRVControl;

interface
uses Messages,SysUtils, Classes, Windows, Graphics, RVFuncs,
     RVScroll, CRVData, RVStyle, RVItem, RVFMisc, DLines, CRVFData, RichView,
     RVClasses, RVERVData, RVEdit,IPEqNode,Controls;

const
  rvsComponentStyle = -301;

  WM_RVGETDESCENT = WM_USER + 333;
  WM_RVGETCUSTOMPRINT = WM_USER + 334;
  WM_RVCUSTOMPRINT = WM_USER + 335;

type

  PTDXRVPrintInfo = ^TTDXRVPrintInfo;
  TTDXRVPrintInfo = record
     Canvas: TCanvas;
     X,Y,X2: Integer;
     Preview, Correction: Boolean;
     Sad: TRVScreenAndDevice;
     RichView: TRVScroller;
     Dli: TRVDrawLineInfo;
     Part: Integer;
     ColorMode: TRVColorMode;
     RVData: TPersistent
  end;

  TWMRVPrintInfo = packed record
    Msg: Cardinal;
    Unused: Integer;
    PrintInfo : PTDXRVPrintInfo;
    Result: Longint;
  end;


  TTDXRVControlItemInfo = class(TRVControlItemInfo)
    private
      FTextStyleNo: Integer;
      procedure SetTextStyleNo(value:Integer);
      function GetPrintToBitmap:boolean;
    protected
      function GetDescent: Integer; override;
      function GetRVFExtraPropertyCount: Integer; override;
      procedure SaveRVFExtraProperties(Stream: TStream); override;
    public
      RVStyle: TRVStyle;
      ProtectTextStyleNo: Boolean;
      constructor CreateEx(RVData: TPersistent; TextStyleNo: Integer); overload;
      constructor CreateEx(RVData: TPersistent; AControl: TControl; AVAlign: TRVVAlign; TextStyleNo:Integer); overload;
      destructor Destroy; override;
      function GetBoolValue(Prop: TRVItemBoolProperty): Boolean; override;
      function GetBoolValueEx(Prop: TRVItemBoolPropertyEx; RVStyle: TRVStyle): Boolean; override;
      procedure SetExtraCustomProperty(const PropName, Value:String); override;
      procedure Paint(X,Y: Integer; Canvas: TCanvas; State: TRVItemDrawStates;
                      Style: TRVStyle; Dli: TRVDrawLineInfo); override;

      procedure Print(Canvas: TCanvas; X,Y,X2: Integer;
        Preview, Correction: Boolean; const sad: TRVScreenAndDevice;
        RichView: TRVScroller; Dli: TRVDrawLineInfo;
        Part: Integer; ColorMode: TRVColorMode; RVData: TPersistent); override;
      procedure AfterLoading(FileFormat: TRVLoadFormat); override;
      procedure SaveRVF(Stream: TStream; RVData: TPersistent; ItemNo, ParaNo: Integer;
                        const Name: String; Part: TRVMultiDrawItemPart;
                        ForceSameAsPrev: Boolean); override;
      function ReadRVFLine(const s: String; RVData: TPersistent;
                           ReadType, LineNo, LineCount: Integer;
                           var Name: String;
                           var ReadMode: TRVFReadMode;
                           var ReadState: TRVFReadState): Boolean; override;
      procedure Assign(Source: TCustomRVItemInfo); override;
      procedure MarkStylesInUse(UsedTextStyles, UsedParaStyles, UsedListStyles: TRVIntegerList); override;
      procedure UpdateStyles(TextStylesShift, ParaStylesShift, ListStylesShift: TRVIntegerList); override;
      procedure ApplyStyleConversion(RVData: TPersistent; ItemNo:Integer; UserData: Integer); override;
      procedure UpdateMe; virtual;
      procedure Execute(RVData:TPersistent);override;
      procedure Inserted(RVData: TObject; ItemNo: Integer); override;
      procedure Inserting(RVData: TObject; var Text: String; Safe: Boolean); override;
  published
      property TextStyleNo:Integer read FTextStyleNo write SetTextStyleNo;
  end;

implementation

uses RVUndo;

type
  THackControl = class(TControl)
  end;

{==============================================================================}

{ TTDXRVControlItemInfo }
constructor TTDXRVControlItemInfo.CreateEx(RVData: TPersistent;
  TextStyleNo: Integer);
begin
   inherited Create(RVData);
   StyleNo := rvsComponentStyle;
   Self.FTextStyleNo := TextStyleNo;
   RVStyle := TCustomRVData(RVData).GetRVStyle;
   UpdateMe;
end;

constructor TTDXRVControlItemInfo.CreateEx(RVData: TPersistent; AControl: TControl; AVAlign: TRVVAlign; TextStyleNo:Integer);
begin
  inherited CreateEX(RVData,AControl,AValign);
   StyleNo := rvsComponentStyle;
   Self.FTextStyleNo := TextStyleNo;
   RVStyle := TCustomRVData(RVData).GetRVStyle;
   UpdateMe;
end;



destructor TTDXRVControlItemInfo.Destroy;
begin
  inherited Destroy;
end;


procedure TTDXRVControlItemInfo.SetTextStyleNo(value:Integer);
begin
  if value <> FTextStyleNo then
  begin
    FTextStyleNo := value;
    UpdateMe;
  end;
end;

{------------------------------------------------------------------------------}
procedure TTDXRVControlItemInfo.AfterLoading(FileFormat: TRVLoadFormat);
begin
  inherited;
  UpdateMe;
end;
{------------------------------------------------------------------------------}

procedure TTDXRVControlItemInfo.UpdateMe;
begin
   with RVStyle.TextStyles[FTextStyleNo] do
   begin
     if Assigned(Control) then
     begin
       THackControl(Control).Font.Name := FontName;
       THackControl(Control).Font.Size := Size;
       THackCOntrol(Control).Font.Style := Style;
       THackControl(Control).Font.Color := Color;
     end;
   end;
end;

{------------------------------------------------------------------------------}
procedure TTDXRVControlItemInfo.Assign(Source: TCustomRVItemInfo);
begin
  if Source is TTDXRVControlItemInfo then
  begin
    StyleNo := TTDXRVControlItemInfo(Source).StyleNo;
  end;
  inherited;
end;
{------------------------------------------------------------------------------}
procedure TTDXRVControlItemInfo.Paint(X, Y: Integer; Canvas: TCanvas;
  State: TRVItemDrawStates; Style: TRVStyle; Dli: TRVDrawLineInfo);
begin
  inherited Paint(X,Y,Canvas,State,Style,Dli);
end;
{------------------------------------------------------------------------------}

function TTDXRVControlItemInfo.GetPrintToBitmap:boolean;
begin
  if Control.Perform(WM_RVGETCUSTOMPRINT,0,0) <> 0 then
    Result := false
  else
    Result := true;
end;

procedure TTDXRVControlItemInfo.Print(Canvas: TCanvas; X,Y,X2: Integer;
        Preview, Correction: Boolean; const Sad: TRVScreenAndDevice;
        RichView: TRVScroller; Dli: TRVDrawLineInfo;
        Part: Integer; ColorMode: TRVColorMode; RVData: TPersistent);
var
  PInfo : PTDXRVPrintInfo;
begin
  GetMem(PInfo,SizeOf(TTDXRVPrintInfo));
  try
    PInfo^.Canvas := Canvas;
    PInfo^.X := X;
    PInfo^.Y := Y;
    PInfo^.X2 := X2;
    PInfo^.Preview := Preview;
    PInfo^.Correction := Correction;
    PInfo^.Sad := Sad;
    PInfo^.RichView := RichView;
    PInfo^.Dli := Dli;
    PInfo^.Part := Part;
    PInfo^.ColorMode := COlorMode;
    PInfo^.RVData := RVData;
    Control.Perform(WM_RVCUSTOMPRINT,0,Integer(PInfo));
  finally
    FreeMem(PInfo,SizeOf(TTDXRVPrintInfo));
  end;
end;


{------------------------------------------------------------------------------}
function TTDXRVControlItemInfo.GetBoolValueEx(Prop: TRVItemBoolPropertyEx;
  RVStyle: TRVStyle): Boolean;
begin
  case Prop of
    rvbpJump, rvbpAllowsFocus,rvbpXORFocus:
      Result := RVStyle.TextStyles[FTextStyleNo].Jump;
    rvbpHotColdJump:
      Result := RVStyle.TextStyles[FTextStyleNo].Jump and
                RVStyle.StyleHoverSensitive(StyleNo);
    rvbpPrintToBMP:
      Result := GetPrintToBitmap;
   else
     Result := inherited GetBoolValueEx(Prop, RVStyle);
  end;
end;
{------------------------------------------------------------------------------}
function TTDXRVControlItemInfo.GetBoolValue(Prop: TRVItemBoolProperty): Boolean;
begin
  case Prop of
    rvbpAlwaysInText:
      Result := True;
    rvbpDrawingChangesFont:
      Result := True;
    else
      Result := inherited GetBoolValue(Prop);
  end;
end;
{------------------------------------------------------------------------------}
procedure TTDXRVControlItemInfo.SaveRVF(Stream: TStream; RVData: TPersistent;
  ItemNo, ParaNo: Integer; const Name: String; Part: TRVMultiDrawItemPart;
  ForceSameAsPrev: Boolean);
begin
   inherited SaveRVF(Stream,RVData,ItemNo,ParaNo,Name,Part,FOrceSameAsPrev);
   // if you want to modify saving/loading, modify
   // 1) second parameter in header - number of additional lines
   // 2) lines after header
   // Do not change other parameters in header
end;
{------------------------------------------------------------------------------}
function TTDXRVControlItemInfo.ReadRVFLine(const S: String; RVData: TPersistent;
                           ReadType, LineNo, LineCount: Integer;
                           var Name: String;
                           var ReadMode: TRVFReadMode;
                           var ReadState: TRVFReadState): Boolean;
begin
  Result := inherited ReadRVFLine(S,RVData,ReadType,LineNo,LineCount,Name,ReadMode,ReadState);
  if Result and Assigned(Control) then
  begin
    //This item was cloned fix name.
    if Copy(Control.HelpKeyword,1,1) = '@' then
    begin
      Name := Copy(Control.HelpKeyword,2,Length(Control.HelpKeyword)-1);
    end;
  end;
  if LineNo = (LineCount-1) then
  begin
    RVStyle := TCustomRVData(RVData).GetRVStyle;
    UpdateMe;
  end;
end;

procedure TTDXRVControlItemInfo.MarkStylesInUse(UsedTextStyles, UsedParaStyles,
  UsedListStyles: TRVIntegerList);
begin
  inherited MarkStylesInUse(UsedTextStyles, UsedParaStyles, UsedListStyles);
  UsedTextStyles[FTextStyleNo] := 1;
end;

procedure TTDXRVControlItemInfo.UpdateStyles(TextStylesShift, ParaStylesShift,
  ListStylesShift: TRVIntegerList);
begin
  inherited UpdateStyles(TextStylesShift, ParaStylesShift, ListStylesShift);
  Dec(FTextStyleNo,TextStylesShift[FTextStyleNo]-1);
  UpdateMe;
end;

procedure TTDXRVControlItemInfo.ApplyStyleConversion(RVData: TPersistent; ItemNo:Integer;
  UserData: Integer);
var
  Rve: TCustomRichViewEdit;
  NewNo : Integer;
begin
  Rve := TCustomRichViewEdit(TRVEditRVData(RVData).RichView);
  if not Assigned(rve.FCurStyleConversion) then
    Exit;

  Rve.FCurStyleConversion(Rve, FTextStyleNo, UserData, True, NewNo);

  TRVEditRVData(RVData).Do_ModifyItemIntProperty(
    TRVEditRVData(RVData).GetItemNo(Self), Self,'textstyleno',NewNo,
    true,true, TRVUndoModifyItemIntProperty);
end;

procedure TTDXRVControlItemInfo.Inserting(RVData: TObject; var Text: String; Safe: Boolean);
begin
  inherited Inserting(RVData,Text,Safe);
  if RVData <> nil then
    UpdateMe;
end;


procedure TTDXRVControlItemInfo.Inserted(RVData: TObject; ItemNo: Integer);
begin
  if RVData<>nil then
  begin
    RVStyle := TCustomRVData(RVData).GetRVStyle;
    UpdateMe;
  end;
end;

procedure TTDXRVControlItemInfo.Execute(RVData: TPersistent);
begin
  if RVData is TCustomRVFormattedData then begin
    if GetBoolValueEx(rvbpJump, TCustomRVData(RVData).GetRVStyle) then
      TCustomRVFormattedData(RVData).DoJump(JumpID+
          TCustomRVFormattedData(RVData).FirstJumpNo)
  end;
end;

function TTDXRVCOntrolItemInfo.GetDescent: Integer;
var
  Desc : Integer;
begin
  Result := inherited GetDescent;

  if Assigned(Control) then
  begin
    Desc := 0;
    if Control.Perform(WM_RVGETDESCENT,0,Longint(@Desc)) <> 0 then
       Result := Result + Desc;
  end;

end;


function TTDXRVControlItemInfo.GetRVFExtraPropertyCount: Integer;
begin
  Result := inherited GetRVFExtraPropertyCount;
  Inc(Result);
end;
{------------------------------------------------------------------------------}
procedure TTDXRVControlItemInfo.SaveRVFExtraProperties(Stream: TStream);
begin
  inherited SaveRVFExtraProperties(Stream);
  RVWriteLn(Stream,'textstyleno='+IntToStr(FTextStyleNo));
end;

procedure TTDXRVControlItemInfo.SetExtraCustomProperty(const PropName,Value:String);
begin
  inherited SetExtraCustomProperty(PropName,Value);
  try
    if PropName = 'textstyleno' then
      FTextStyleNo := StrToInt(Value);
  except
  end;
end;

initialization

  RegisterRichViewItemClass(rvsComponentStyle, TTDXRVControlItemInfo);

end.
