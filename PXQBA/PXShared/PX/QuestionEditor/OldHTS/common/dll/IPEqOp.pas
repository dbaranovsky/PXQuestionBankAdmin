unit IPEqOp;

interface

uses IPEqNode,IPEqChar,Classes,Graphics;

type

  TIPEqOpType = (eqoEmpty,eqoSpace,eqoPlus,eqoMinus,eqoMult,eqoDivide,
                 eqoEqual,eqoPower,eqoLT,eqoGT,eqoLE,eqoGE,eqoNe,eqoDivide2,
                 eqoTimes,eqoStar,eqoApprox,eqoCong,eqoPlusMinus,eqoComp,eqoApproxEqual);

const
  IPEqOpValuesW : array[TIPEqOpType] of WideChar =
      (#$0000,' ','+',#$2212,#$2219,'/'{#$2215},
       '=','^','<','>',#$2264,#$2265,#$2260,#$00F7,
       #$00D7,#$002A,#$2248,#$2245,#$00b1,#$25E6,#$2245);

  IPEqOpValues : array[TIPEqOpType] of Char =
      (#0,' ','+',#$2d,#$D7,#$2f,
       '=','^','<','>',#$a3,#$b3,#$b9,#184,
       #180,#$2a,#187,#64,#177,#00,#$40);

  IPEqOpNames : array[TIPEqOpType] of String = (
    '&emptyop;',' ','+','-','*','/',
    '=','^','&lt;','&gt;','&le;','&ge;','&ne;','&div;',
    '&times;','&star;','&approx;','&cong;','&plusminus;','&compfn;','&approxequal;');

  IPEqMMNames : array[TIPEqOpType] of string = (
    '','','RawPlus','RawMinus','RawStar','RawSlash',
    'Equal','RawWedge','RawLess','RawGreater','LessEqual','GreaterEqual','NotEqual','Divide',
    'Cross','Star','TildeTilde','TildeEqual','PlusMinus','SmallCircle','ApproxEqual');

  IPEqOpForceSymbol : set of TIPEqOpType =
    [eqoCong,eqoApproxEqual];

  IPEqOpForceUnicode : set of TIPEqOpType =
    [eqoComp];


type
TIPEqOp = class(TIPEqNode)
  private
    FOp : TIPEqOpType;
    FSpaceSize : Integer;
    FVOffset : Integer;
    procedure SetOp(Value:TIPEqOpType);
    function UseSymbolFont:Boolean;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Draw(ACanvas:TCanvas); override;
    procedure Layout; override;
  public
    constructor Create(AOp:TIPEqOpType);
    procedure DeleteCharacter(CaretEvent:TIPEqCaretEvent); override;
    function  Clone:TIPEqNode; override;
    function IsEmpty:boolean; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function GetText:String; override;

    property Op:TIPEqOpType read FOp write SetOp;
    property SpaceSize:Integer read FSpaceSize write FSpaceSize;
end;
Function GetEqOpType(Name:String; var OpType:TIPEqOpType):boolean;
Function GetEqOpTypeMM(Name:String; var OpType:TIPEqOpType):boolean;


implementation

uses IPEqUtils,Math,Windows,sysutils;

Function GetEqOpType(Name:String; var OpType:TIPEqOpType):boolean;
var
 I : TIPEqOpType;
begin
  for I := Low(IPEqOpNames) to High(IPEqOpNames) do
  begin
    if SameText(IPEqOpNames[I],Name) then
    begin
      OpType := I;
      Result := true;
      Exit;
    end;
  end;
  Result := false;
end;

Function GetEqOpTypeMM(name:String; var OpType:TIPEqOpType):boolean;
var
 I : TIPEqOpType;
begin
  for I := Low(IPEqOpNames) to High(IPEqOpNames) do
  begin
    if SameText(IPEqOpNames[I],Name) then
    begin
      OpType := I;
      Result := true;
      Exit;
    end;
  end;
  Result := false;
end;

constructor TIPEqOp.Create(AOp:TIPEqOpType);
begin
  inherited Create;
  FOp := AOp;
  FSpaceSize := 25;
end;

function  TIPEqOp.Clone:TIPEqNode;
begin
  Result := TIPEqOp.Create(FOp);
end;

procedure TIPEqOp.BuildMathML(Buffer:TStrings; Level:Integer);
begin
  Buffer.Add('<mo>'+IPEqOpNames[FOp]+'</mo>');
end;

function TIPEqOp.GetText:String;
begin
  Result := IPEqOpNames[FOp];
end;

function TIPEqOp.UseSymbolFont:Boolean;
begin
  Result := (Document.UseSymbolFont or (FOp in IPEqOpForceSymbol)) and not
            (FOp in IPEqOpForceUnicode);
end;

function TIPEqOp.CalcMetrics:TIPEqMetrics;
var
  TextMetric : TTextMetric;
  W : Integer;
  Em : Integer;
begin

  if assigned(Parent) and UseSymbolFont then
  begin
    InitFont;
    Font.Name := 'Symbol';
    Font.Size := Font.Size+1;
  end;

  Em := GetEMWidth(Font);
  TextMetric := GetTextMetrics;
  if UseSymbolFont then
    W := GetTextExtent(IPEqOpValues[FOp]).Cx+2*GetEMPart(FSpaceSize,Em)
  else
    W := GetTextSizeW(Font,IPEqOpValuesW[FOp]).Cx+2*GetEMPart(FSpaceSize,Em);
  FVOffset := TextMetric.tmInternalLeading;
  Result := TIPEqMetrics.Create(TextMetric.tmAscent-FVOffset,0,W,Em);
end;

procedure TIPEqOp.Draw(ACanvas:TCanvas);
begin
  if UseSymbolFont then
    ACanvas.TextOut(GetEmPart(FSpaceSize),-FVOffset,IPEqOpValues[FOp])
  else
  begin
    TextOutW(ACanvas.Handle,GetEmPart(FSpaceSize),-FVOffset,@IPEqOpValuesW[FOp],1);
  end;
end;

procedure TIPEqOp.Layout;
begin
end;

procedure TIPEqOp.SetOp(Value:TIPEqOpType);
begin
  if Value <> FOp then
  begin
    FOp := Value;
    Invalidate;
  end;
end;


procedure TIPEqOp.DeleteCharacter(CaretEvent:TIPEqCaretEvent);
var
  Pos1 : Integer;
begin
  Pos1 := Min(CaretEvent.Position,CaretEvent.PositionStart);

  if Pos1 = 0 then
  begin
    FOp := eqoEmpty;
    Invalidate;
    CaretEvent.CharacterDeleted := true;
  end;
end;

function TIPEqOp.IsEmpty:boolean;
begin
  Result := FOp = eqoEmpty;
end;

end.
