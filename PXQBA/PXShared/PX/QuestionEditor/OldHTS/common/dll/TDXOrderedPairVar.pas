unit TDXOrderedPairVar;

interface

uses
  Variants;


{ TDX variant creation utils }

function VarTDXOrderedPairCreate: Variant; overload;
function VarTDXOrderedPairCreate(Aitem1,Aitem2:Variant): Variant; overload;

function VarTDXOrderedPair: TVarType;
function VarIsTDXOrderedPair(const AValue: Variant): Boolean;
function VarAsTDXOrderedPair(const AValue: Variant): Variant;

function VarTDXOrderedPairItem1(const AValue:Variant):Variant;
function VarTDXOrderedPairItem2(const AValue:Variant):Variant;


implementation

uses
  VarUtils, SysUtils, Math, SysConst, TypInfo, Classes;

type
  { TDX variant type handler }
  TTDXOrderedPairVariantType = class(TPublishableVariantType)
  protected
    function LeftPromotion(const V: TVarData; const Operator: TVarOp;
      out RequiredVarType: TVarType): Boolean; override;
    function RightPromotion(const V: TVarData; const Operator: TVarOp;
      out RequiredVarType: TVarType): Boolean; override;
    function GetInstance(const V: TVarData): TObject; override;
  public
    procedure Clear(var V: TVarData); override;
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
  TDXOrderedPairVariantType: TTDXOrderedPairVariantType = nil;

type
  { TDX data that the TDX variant points to }
  TTDXOrderedPairData = class(TPersistent)
  private
    FItem1 : Variant;
    FItem2 : Variant;
    function GetAsString: String;
    procedure SetAsString(const Value: String);
  protected
  public

    // the many ways to create
    constructor Create(AItem1,AItem2:Variant); overload;
    constructor Create(const AData: TTDXOrderedPairData); overload;
    constructor Create(const AText:String); overload;

    // non-destructive operations
//    function Equal(const Right: TTDXData): Boolean; overload;
//    function Equal(const AText: string): Boolean; overload;

    // destructive operations

    // conversion
    property AsString: String read GetAsString write SetAsString;
  published
    property Item1: Variant read FItem1 write FItem1;
    property Item2: Variant read FItem2 write FItem2;
  end;

  { Helper record that helps crack open TVarData }
  TTDXOrderedPairVarData = packed record
    VType: TVarType;
    Reserved1, Reserved2, Reserved3: Word;
    VTDX: TTDXOrderedPairData;
    Reserved4: LongInt;
  end;

{ TTDXData }


constructor TTDXOrderedPairData.Create(AItem1,AItem2 : Variant);
begin
  inherited Create;
  FItem1 := AItem1;
  FItem2 := AItem2;
end;

constructor TTDXOrderedPairData.Create(const AData: TTDXOrderedPairData);
begin
  inherited Create;
  FItem1 := AData.Item1;
  FItem2 := AData.Item2;
end;

constructor TTDXOrderedPairData.Create(const AText: String);
begin
  inherited Create;
  AsString := AText;
end;

function TTDXOrderedPairData.GetAsString: String;
begin
  Result := '('+FItem1+','+Fitem2+')';
end;

procedure TTDXOrderedPairData.SetAsString(const Value:String);
var
  CommaPos : Integer;
  S : String;
begin
  CommaPos := Pos(',',Value);
  if CommaPos > 0 then
  begin
    S := Copy(Value,1,CommaPos-1);
    FItem1 := s;
    S := Copy(Value,CommaPos+1,Length(Value)-CommaPos);
    FItem2 := S;
    Exit;
  end;
  raise Exception.CreateFmt('Can not convert %s to an ordered pair',[Value]);
end;


{ TTDXVariantType }


procedure TTDXOrderedPairVariantType.Cast(var Dest: TVarData; const Source: TVarData);
var
  LSource: TVarData;
begin
  VarDataInit(LSource);
  try
    VarDataCopyNoInd(LSource, Source);
    if VarDataIsStr(LSource) then
    begin
      TTDXOrderedPairVarData(Dest).VTDX := TTDXOrderedPairData.Create(VarDataToStr(LSource));
      Dest.VType := VarType;
    end
    else
      RaiseCastError;
  finally
    VarDataClear(LSource);
  end;
end;


procedure TTDXOrderedPairVariantType.CastTo(var Dest: TVarData; const Source: TVarData;
  const AVarType: TVarType);
begin
  if Source.VType = VarType then
    case AVarType of
      varOleStr:
        VarDataFromOleStr(Dest, TTDXOrderedPairVarData(Source).VTDX.AsString);
      varString:
        VarDataFromStr(Dest, TTDXOrderedPairVarData(Source).VTDX.AsString);
    else
      RaiseCastError;
    end
  else
    inherited;
end;

procedure TTDXOrderedPairVariantType.Clear(var V: TVarData);
begin
  V.VType := varEmpty;
  FreeAndNil(TTDXOrderedPairVarData(V).VTDX);
end;


procedure TTDXOrderedPairVariantType.Copy(var Dest: TVarData; const Source: TVarData;
  const Indirect: Boolean);
begin
  if Indirect and VarDataIsByRef(Source) then
    VarDataCopyNoInd(Dest, Source)
  else
    with TTDXOrderedPairVarData(Dest) do
    begin
      VType := VarType;
      VTDX  := TTDXOrderedPairData.Create(TTDXOrderedPairVarData(Source).VTDX);
    end;
end;

function TTDXOrderedPairVariantType.GetInstance(const V: TVarData): TObject;
begin
  Result := TTDXOrderedPairVarData(V).VTDX;
end;


function TTDXOrderedPairVariantType.LeftPromotion(const V: TVarData;
  const Operator: TVarOp; out RequiredVarType: TVarType): Boolean;
begin
  { TypeX Op TDX }
  if (Operator = opAdd) and VarDataIsStr(V) then
    RequiredVarType := varString
  else
//    RequiredVarType := VarDouble;
    RequiredVarType := VarType;
  Result := True;
end;

function TTDXOrderedPairVariantType.RightPromotion(const V: TVarData;
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

procedure VarTDXOrderedPairCreateInto(var ADest: Variant; const ATDX: TTDXOrderedPairData);
begin
  VarClear(ADest);
  TTDXOrderedPairVarData(ADest).VType := VarTDXOrderedPair;
  TTDXOrderedPairVarData(ADest).VTDX := ATDX;
end;

function VarTDXOrderedPairCreate: Variant;
begin
  VarTDXOrderedPairCreateInto(Result, TTDXOrderedPairData.Create(0,0));
end;


function VarTDXOrderedPairCreate(AItem1,AItem2:Variant): Variant;
begin
  VarTDXOrderedPairCreateInto(Result, TTDXOrderedPairData.Create(AItem1,AItem2));
end;


function VarTDXOrderedPair: TVarType;
begin
  Result := TDXOrderedPairVariantType.VarType;
end;

function VarIsTDXOrderedPair(const AValue: Variant): Boolean;
begin
  Result := (TTDXOrderedPairVarData(AValue).VType and varTypeMask) = VarTDXOrderedPair;
end;

function VarAsTDXOrderedPair(const AValue: Variant): Variant;
begin
  if not VarIsTDXOrderedPair(AValue) then
    VarCast(Result, AValue, VarTDXOrderedPair)
  else
    Result := AValue;
end;


function VarTDXOrderedPairItem1(const AValue:Variant):Variant;
var
  LTemp: Variant;
begin
  VarCast(LTemp, AValue, VarTDXOrderedPair);
  Result := TTDXOrderedPairVarData(LTemp).VTDX.Item1;
end;

function VarTDXOrderedPairItem2(const AValue:Variant):Variant;
var
  LTemp: Variant;
begin
  VarCast(LTemp, AValue, VarTDXOrderedPair);
  Result := TTDXOrderedPairVarData(LTemp).VTDX.Item2;
end;

procedure TTDXOrderedPairVariantType.BinaryOp(var Left: TVarData;
  const Right: TVarData; const Operator: TVarOp);
begin
  if Right.VType = VarType then
    case Left.VType of
      varString:
        case Operator of
          opAdd:
            Variant(Left) := Variant(Left) + TTDXOrderedPairVarData(Right).VTDX.AsString;
        else
          RaiseInvalidOp;
        end;
    else
      RaiseInvalidOp;
    end
  else
    RaiseInvalidOp;
end;

procedure TTDXOrderedPairVariantType.UnaryOp(var Right: TVarData; const Operator: TVarOp);
begin
  RaiseInvalidOp;
end;



function TTDXOrderedPairVariantType.CompareOp(const Left, Right: TVarData;
  const Operator: Integer): Boolean;
begin
  Result := false;
  if Right.VType = VarType then
    case Left.VType of
      varString:
        case Operator of
          opCmpEq: Result := Variant(Left) = TTDXOrderedPairVarData(Right).VTDX.AsString;
          opCmpNe: Result := Variant(Left) <> TTDXOrderedPairVarData(Right).VTDX.AsString;
          opCmpLt: Result := Variant(Left) < TTDXOrderedPairVarData(Right).VTDX.AsString;
          opCmpLe: Result := Variant(Left) <= TTDXOrderedPairVarData(Right).VTDX.AsString;
          opCmpGt: Result := Variant(Left) > TTDXOrderedPairVarData(Right).VTDX.AsString;
          opCmpGe: Result := Variant(Left) >= TTDXOrderedPairVarData(Right).VTDX.AsString;
        else
          RaiseInvalidOp;
        end;
    else
      if Left.VType = VarType then
        case Operator of
          opCmpEq: Result := (TTDXOrderedPairVarData(Left).VTDX.Item1 = TTDXOrderedPairVarData(Right).VTDX.Item1) and
                             (TTDXOrderedPairVarData(Left).VTDX.Item2 = TTDXOrderedPairVarData(Right).VTDX.Item2);
          opCmpNe: Result := (TTDXOrderedPairVarData(Left).VTDX.Item1 <> TTDXOrderedPairVarData(Right).VTDX.Item1) or
                             (TTDXOrderedPairVarData(Left).VTDX.Item2 <> TTDXOrderedPairVarData(Right).VTDX.Item2);
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
  TDXOrderedPairVariantType := TTDXOrderedPairVariantType.Create;
finalization
  FreeAndNil(TDXOrderedPairVariantType);
end.
