unit IPEqChar;

interface

uses IPEqNode,Graphics,Windows,Classes;

type

TIPEqChar = class(TIPEqNode)
  private
    FChar : Char;
    FSpaceSize : Integer;
    FVOffset : Integer;
    procedure SetCharacter(Value:Char);
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Draw(ACanvas:TCanvas); override;
    procedure Layout; override;
  public
    constructor Create(C:Char);
    procedure DeleteCharacter(CaretEvent:TIPEqCaretEvent); override;
    function  Clone:TIPEqNode; override;
    function isEmpty:boolean; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function GetText:String; override;

    property Character:Char read FChar write SetCharacter;
    property SpaceSize:Integer read FSpaceSize write FSpaceSize;
end;

  TIPEqSymbol = class (TIPEqChar)
    private
    protected
      function CalcMetrics:TIPEqMetrics; override;
      procedure Draw(ACanvas:TCanvas); override;
    public
    function  Clone:TIPEqNode; override;
  end;

implementation

uses IPEqUtils,Math;

constructor TIPEqChar.Create(C:Char);
begin
  inherited Create;
  FChar := C;
  FSpaceSize := 0;
end;

function  TIPEqChar.Clone:TIPEqNode;
begin
  Result := TIPEqChar.Create(FChar);
end;

procedure TIPEqChar.BuildMathML(Buffer:TStrings; Level:Integer);
begin
  if FChar = ' ' then
    Buffer.Add('<mi>&emsp;</mi>')
  else
    Buffer.Add('<mo>'+FChar+'</mo>');
end;

function TIPEqChar.GetText:String;
begin
  if FChar in ['{','}',';','@','&','^','"','''','^','_','\'] then
    Result := '\'+FChar
  else
    Result := FChar;
end;

function TIPEqChar.CalcMetrics:TIPEqMetrics;
var
  Tm : TTextMetric;
  W : Integer;
  Em : Integer;
begin
  Em := GetEMWidth(Font);
  Tm := GetTextMetrics;
  W := GetTextExtent(FChar).cx+2*GetEMPart(FSpaceSize,Em);
  if FChar <> '|' then
    FVOffset := Tm.tmInternalLeading
  else
    FVOffset := 0;
  Result := TIPEqMetrics.Create(Tm.tmAscent-FVOffset,Tm.tmDescent,W,Em);
end;

procedure TIPEqChar.Draw(ACanvas:TCanvas);
begin
  ACanvas.TextOut(GetEmPart(FSpaceSize),-FVOffset,FChar);
end;

procedure TIPEqChar.Layout;
begin
end;

procedure TIPEqChar.SetCharacter(Value:Char);
begin
  if Value <> FChar then
  begin
    FChar := Value;
    Invalidate;
  end;
end;


procedure TIPEqChar.DeleteCharacter(CaretEvent:TIPEqCaretEvent);
var
  Pos1 : Integer;
begin
  Pos1 := Min(CaretEvent.Position,CaretEvent.PositionStart);

  if Pos1 = 0 then
  begin
    FChar := #0;
    Invalidate;
    CaretEvent.CharacterDeleted := true;
  end;
end;

function TIPEqChar.isEmpty:boolean;
begin
  Result := FChar = #0;
end;


function  TIPEqSymbol.Clone:TIPEqNode;
begin
  Result := TIPEqSymbol.Create(FChar);
end;

function TIPEqSymbol.CalcMetrics:TIPEqMetrics;
begin
  if Assigned(Parent) then
  begin
    InitFont;
    Font.Name := 'Symbol';
    Font.Size := Font.Size+1;
  end;
  Result := inherited CalcMetrics;
end;

procedure TIPEqSymbol.Draw(ACanvas:TCanvas);
begin
  if Character = 'g' then
    ACanvas.TextOut(GetEmPart(FSpaceSize),(-Descent div 2)-FVOffset,Character)
  else
    inherited Draw(ACanvas);
end;


end.
