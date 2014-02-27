unit TDXVar;


interface

uses
  Variants;


{ TDX variant creation utils }

function VarTDXCreate: Variant; overload;
function VarTDXCreate(const AReal: Double; const AFormatted:String): Variant; overload;
function VarTDXCreate(const AInt: Integer; const AFormatted:String): Variant; overload;

function VarTDX: TVarType;
function VarIsTDX(const AValue: Variant): Boolean;
function VarAsTDX(const AValue: Variant): Variant;


implementation

uses
  VarUtils, SysUtils, Math, SysConst, TypInfo, Classes,
  IPStrUtils;

type
  { TDX variant type handler }
  TTDXVariantType = class(TPublishableVariantType)
  protected
    function LeftPromotion(const V: TVarData; const Operator: TVarOp;
      out RequiredVarType: TVarType): Boolean; override;
    function RightPromotion(const V: TVarData; const Operator: TVarOp;
      out RequiredVarType: TVarType): Boolean; override;
    function GetInstance(const V: TVarData): TObject; override;
  public
    procedure Clear(var V: TVarData); override;
//    function IsClear(const V: TVarData): Boolean; override;
    procedure Copy(var Dest: TVarData; const Source: TVarData;
      const Indirect: Boolean); override;
    procedure Cast(var Dest: TVarData; const Source: TVarData); override;
    procedure CastTo(var Dest: TVarData; const Source: TVarData;
      const AVarType: TVarType); override;

    procedure BinaryOp(var Left: TVarData; const Right: TVarData;
      const Operator: TVarOp); override;
    procedure UnaryOp(var Right: TVarData; const Operator: TVarOp); override;
    function CompareOp(const Left: TVarData; const Right: TVarData;
      const Operator: Integer): Boolean; override;
  end;

var
  { TDX variant type handler instance }
  TDXVariantType: TTDXVariantType = nil;

type
  { TDX data that the TDX variant points to }
  TTDXData = class(TPersistent)
  private
    FNumber : Double;
    FFormatted : String;
    function GetAsString: String;
    procedure SetAsString(const Value: String);
  protected
  public

    // the many ways to create
    constructor Create(const AReal: Double; AFormatted:String); overload;
    constructor Create(const AInt : Integer; AFormatted:String); overload;
    constructor Create(const AData: TTDXData); overload;
    constructor Create(const AText:String); overload;

    // non-destructive operations
//    function Equal(const Right: TTDXData): Boolean; overload;
//    function Equal(const AText: string): Boolean; overload;

    // destructive operations

    // conversion
    property AsString: String read GetAsString write SetAsString;
  published
    property Number: Double read FNumber write FNumber;
    property Formatted:String read FFormatted write FFormatted;
  end;

  { Helper record that helps crack open TVarData }
  TTDXVarData = packed record
    VType: TVarType;
    Reserved1, Reserved2, Reserved3: Word;
    VTDX: TTDXData;
    Reserved4: LongInt;
  end;

{ TTDXData }


constructor TTDXData.Create(const AReal: Double; AFormatted:String);
begin
  inherited Create;
  FNumber := AReal;
  FFormatted := AFormatted;
end;

constructor TTDXData.Create(const AInt: Integer; AFormatted:String);
begin
  inherited Create;
  FNumber := AInt;
  FFormatted := AFormatted;
end;

constructor TTDXData.Create(const AData: TTDXData);
begin
  inherited Create;
  FNumber := AData.Number;
  FFormatted := AData.Formatted;
end;

constructor TTDXData.Create(const AText: String);
begin
  inherited Create;

  AsString := AText;
end;

function TTDXData.GetAsString: String;
begin
  Result := FFormatted;
end;

procedure TTDXData.SetAsString(const Value:String);
begin
  FNumber := IPStrToFloat(Value);
  FFormatted := Value;
end;


{ TTDXVariantType }


procedure TTDXVariantType.Cast(var Dest: TVarData; const Source: TVarData);
var
  LSource, LTemp: TVarData;
begin
  VarDataInit(LSource);
  try
    VarDataCopyNoInd(LSource, Source);
    if VarDataIsStr(LSource) then
      TTDXVarData(Dest).VTDX := TTDXData.Create(VarDataToStr(LSource))
    else
    begin
      VarDataInit(LTemp);
      try
        VarDataCastTo(LTemp, LSource, varDouble);
        TTDXVarData(Dest).VTDX := TTDXData.Create(LTemp.VDouble,FLoatToStr(LTemp.VDouble));
      finally
        VarDataClear(LTemp);
      end;
    end;
    Dest.VType := VarType;
  finally
    VarDataClear(LSource);
  end;
end;


procedure TTDXVariantType.CastTo(var Dest: TVarData; const Source: TVarData;
  const AVarType: TVarType);
var
  LTemp: TVarData;
begin
  if Source.VType = VarType then
    case AVarType of
      varOleStr:
        VarDataFromOleStr(Dest, TTDXVarData(Source).VTDX.AsString);
      varString:
        VarDataFromStr(Dest, TTDXVarData(Source).VTDX.AsString);
    else
      VarDataInit(LTemp);
      try
        LTemp.VType := varDouble;
        LTemp.VDouble := TTDXVarData(Source).VTDX.Number;
        VarDataCastTo(Dest,LTemp, AVarType);
      finally
        VarDataClear(LTemp);
      end;
    end
  else
    inherited;
end;

procedure TTDXVariantType.Clear(var V: TVarData);
begin
  V.VType := varEmpty;
  FreeAndNil(TTDXVarData(V).VTDX);
end;


procedure TTDXVariantType.Copy(var Dest: TVarData; const Source: TVarData;
  const Indirect: Boolean);
begin
  if Indirect and VarDataIsByRef(Source) then
    VarDataCopyNoInd(Dest, Source)
  else
    with TTDXVarData(Dest) do
    begin
      VType := VarType;
      VTDX  := TTDXData.Create(TTDXVarData(Source).VTDX);
    end;
end;

function TTDXVariantType.GetInstance(const V: TVarData): TObject;
begin
  Result := TTDXVarData(V).VTDX;
end;


function TTDXVariantType.LeftPromotion(const V: TVarData;
  const Operator: TVarOp; out RequiredVarType: TVarType): Boolean;
begin
  { TypeX Op TDX }
  if ((Operator = opAdd) or (Operator = opCompare)) and VarDataIsStr(V) then
    RequiredVarType := varString
  else
    RequiredVarType := VarType;
  Result := True;
end;

function TTDXVariantType.RightPromotion(const V: TVarData;
  const Operator: TVarOp; out RequiredVarType: TVarType): Boolean;
begin
  { TDX Op TypeX }

  if VarIsCustom(Variant(V)) and (V.VType <> VarType) then
  begin
    Result := false;
    Exit;
  end;

  RequiredVarType := VarType;
  Result := True;
end;



{ TDX variant creation utils }

procedure VarTDXCreateInto(var ADest: Variant; const ATDX: TTDXData);
begin
  VarClear(ADest);
  TTDXVarData(ADest).VType := VarTDX;
  TTDXVarData(ADest).VTDX := ATDX;
end;

function VarTDXCreate: Variant;
begin
  VarTDXCreateInto(Result, TTDXData.Create(0,''));
end;


function VarTDXCreate(const AReal: Double; const AFormatted:String): Variant;
begin
  VarTDXCreateInto(Result, TTDXData.Create(AReal,AFormatted));
end;

function VarTDXCreate(const AInt: Integer; const AFormatted:String): Variant;
begin
  VarTDXCreateInto(Result, TTDXData.Create(AInt,AFormatted));
end;



function VarTDX: TVarType;
begin
  Result := TDXVariantType.VarType;
end;

function VarIsTDX(const AValue: Variant): Boolean;
begin
  Result := (TVarData(AValue).VType and varTypeMask) = VarTDX;
end;

function VarAsTDX(const AValue: Variant): Variant;
begin
  if not VarIsTDX(AValue) then
    VarCast(Result, AValue, VarTDX)
  else
    Result := AValue;
end;

procedure TTDXVariantType.BinaryOp(var Left: TVarData;
  const Right: TVarData; const Operator: TVarOp);
begin
  if Right.VType = VarType then
    case Left.VType of
      varString:
        case Operator of
          opAdd:
            Variant(Left) := Variant(Left) + TTDXVarData(Right).VTDX.AsString;
        else
          RaiseInvalidOp;
        end;
    else
      if Left.VType = VarType then
        case Operator of
          opAdd:
            Variant(Left) := TTDXVarData(Left).VTDX.FNumber + TTDXVarData(Right).VTDX.FNumber;
          opSubtract:
            Variant(Left) := TTDXVarData(Left).VTDX.FNumber - TTDXVarData(Right).VTDX.FNumber;
          opMultiply:
            Variant(Left) := TTDXVarData(Left).VTDX.FNumber * TTDXVarData(Right).VTDX.FNumber;
          opDivide:
            Variant(Left) := TTDXVarData(Left).VTDX.FNumber / TTDXVarData(Right).VTDX.FNumber;
          opIntDivide:
            Variant(Left) := Trunc(TTDXVarData(Left).VTDX.FNumber / TTDXVarData(Right).VTDX.FNumber);
          opModulus:
            Variant(Left) := Trunc(TTDXVarData(Left).VTDX.FNumber) mod Trunc(TTDXVarData(Right).VTDX.FNumber);
        else
          RaiseInvalidOp;
        end
      else
        RaiseInvalidOp;
    end
  else
    RaiseInvalidOp;
end;

procedure TTDXVariantType.UnaryOp(var Right: TVarData; const Operator: TVarOp);
begin
  if Right.VType = VarType then
    case Operator of
      opNegate:
        Variant(Right) := -TTDXVarData(Right).VTDX.FNumber;
    else
      RaiseInvalidOp;
    end
  else
    RaiseInvalidOp;
end;



function TTDXVariantType.CompareOp(const Left, Right: TVarData;
  const Operator: Integer): Boolean;
begin
  Result := false;
  if Right.VType = VarType then
    case Left.VType of
      varString:
        case Operator of
          opCmpEq: Result := Variant(Left) = TTDXVarData(Right).VTDX.AsString;
          opCmpNe: Result := Variant(Left) <> TTDXVarData(Right).VTDX.AsString;
          opCmpLt: Result := Variant(Left) < TTDXVarData(Right).VTDX.AsString;
          opCmpLe: Result := Variant(Left) <= TTDXVarData(Right).VTDX.AsString;
          opCmpGt: Result := Variant(Left) > TTDXVarData(Right).VTDX.AsString;
          opCmpGe: Result := Variant(Left) >= TTDXVarData(Right).VTDX.AsString;
        else
          RaiseInvalidOp;
        end;
    else
      if Left.VType = VarType then
        case Operator of
          opCmpEq: Result := CompareValue(TTDXVarData(Left).VTDX.FNumber,TTDXVarData(Right).VTDX.FNumber) = 0;
          opCmpNe: Result := CompareValue(TTDXVarData(Left).VTDX.FNumber,TTDXVarData(Right).VTDX.FNumber) <> 0;
          opCmpLt: Result := CompareValue(TTDXVarData(Left).VTDX.FNumber,TTDXVarData(Right).VTDX.FNumber) < 0;
          opCmpLe: Result := CompareValue(TTDXVarData(Left).VTDX.FNumber,TTDXVarData(Right).VTDX.FNumber) <= 0;
          opCmpGt: Result := CompareValue(TTDXVarData(Left).VTDX.FNumber,TTDXVarData(Right).VTDX.FNumber) > 0;
          opCmpGe: Result := CompareValue(TTDXVarData(Left).VTDX.FNumber,TTDXVarData(Right).VTDX.FNumber) >= 0;
        else
          RaiseInvalidOp;
        end
      else
        RaiseInvalidOp;
    end
  else
    RaiseInvalidOp;
end;

initialization
  TDXVariantType := TTDXVariantType.Create;
finalization
  FreeAndNil(TDXVariantType);
end.
