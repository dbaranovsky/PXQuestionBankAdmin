unit IPEqCheck;


interface

uses IPEqNode,IPEqComposite,IPEqStack,Graphics,Windows,Classes;

type

TIPEqCheck = class(TIPEqStack)
  private
    FLeftWidth : Integer;
    FRightWidth : Integer;
    FTopOffset : Integer;
    function GetCheckRowCount:Integer;
    function GetLeftRow(Index:Integer):TIPEqRow;
    function GetRightRow(Index:Integer):TIPEqRow;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create;
    constructor CreateN(NRows:Integer);
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function GetText:String; override;
    property CheckRowCount:Integer read GetCheckRowCount;
    property LeftRow[Index:Integer]:TIPEqRow read GetLeftRow;
    property RightRow[Index:Integer]:TIPEqRow read GetRightRow;
end;

const
  CHECK_INTERNALMARGIN = 20;

implementation

uses Math, IPEqUtils,Types,StStrL,IPEqOp;

constructor TIPEqCheck.Create;
begin
  inherited Create;
  AddRow(TIPEqRow.Create);
end;

constructor TIPEqCheck.CreateN(NRows:Integer);
var
 I : Integer;
begin
  Create;
  for I := 0 to NRows-1 do
  begin
    AddRow(TIPEqRow.Create);
    AddRow(TIPEqRow.Create);
  end;
end;

function  TIPEqCheck.Clone:TIPEqNode;
begin
  Result := TIPEqCheck.Create;
  TIPEqCheck(Result).CopyChildren(Self);
end;

procedure TIPEqCheck.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
   Buffer.Add(CharStrL(' ',Level)+'<mCheck>');
   for I := 0 to ChildCount-1 do
     Child[I].BuildMathML(Buffer,Level+1);
   Buffer.Add(CharStrL(' ',Level)+'</mCheck>');
end;

function TIPEqCheck.GetName:String;
begin
  Result := 'CHECK';
end;

function TIPEqCheck.CalcMetrics:TIPEqMetrics;
var
  RowAscent,RowDescent : Integer;
  Width,Height,Em : Integer;
  Ascent,Descent : Integer;
  Gap : Integer;
  I : Integer;
  WLeft,WRight : Integer;
  W : Integer;
begin
  FleftWidth := 0;
  FrightWidth := 0;

  Em := GetFontHeight(GetTextMetrics);
  Gap := GetEmPart(CHECK_INTERNALMARGIN,Em);

  Height := Gap;

  for I := 0 to CheckRowCount-1 do
  begin
    FleftWidth := Max(FLeftWidth,LeftRow[I].Width);
    FRightWidth := Max(FRightWidth,RightRow[I].Width);
    RowAscent := Max(LeftRow[I].Ascent,RightRow[I].Ascent);
    RowDescent := Max(LeftRow[I].Descent,RightRow[I].Descent);
    Inc(Height,RowAscent+RowDescent+Gap);
  end;

  if ChildCount > 0 then
    Height := Height + Row[0].Height;

  Ascent := Height div 2;
  Descent := Height-Ascent;

  //Now look at top piece and find alignment point.
  WLeft := 0;
  WRight := 0;
  FTopOffset := 0;

  if ChildCount > 0 then
  begin
    if Row[0].ChildCount > 0 then
    begin
      for I := 0 to Row[0].ChildCount-1 do
      begin
        W := Row[0].Child[I].Width;
        if (WRight = 0) and (Row[0].Child[I] is TIPEqOp) and (TIPEqOp(Row[0].Child[I]).Op = eqoEqual) then
        begin
          Inc(WLeft,W div 2);
          Inc(WRight,W-(w div 2));
        end
        else if WRight = 0 then
          Inc(WLeft,W)
        else
          Inc(WRight,W);
      end;
    end
    else
    begin
      WLeft := Row[0].Width;
    end;

    FLeftWidth := Max(FLeftWidth,WLeft-Gap);
    FRightWidth := Max(FRightWidth,WRight-Gap);
    FTopOffset := FLeftWidth-WLeft+Gap;
  end;

    Width := FleftWidth+FrightWidth+4*Gap;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqCheck.Layout;
var
  Gap : Integer;
  X,Y : Integer;
  Ascent,Descent : Integer;
  I : integer;
begin

  Gap := GetEmPart(CHECK_INTERNALMARGIN);

  Y := 0;

  if ChildCount > 0 then
  begin
    Row[0].SetLocation(Gap+FTopOffset,Y);
    Inc(Y,Row[0].Height+Gap);
  end;

  Inc(Y,Gap);

  for I := 0 to CheckRowCount-1 do
  begin
    Ascent := Max(LeftRow[I].Ascent,RightRow[I].Ascent);
    Descent := Max(LeftRow[I].Descent,RightRow[I].Descent);
    X := FLeftWidth-LeftRow[I].Width+Gap;
    LeftRow[I].SetLocation(X,Y+Ascent-LeftRow[I].Ascent);
    X := Width-Gap-RightRow[I].Width;
    RightRow[I].SetLocation(X,Y+Ascent-RightRow[I].Ascent);
    Inc(Y,Ascent+Descent+Gap);
  end;

end;

procedure TIPEqCheck.Draw(ACanvas:TCanvas);
var
  Gap : Integer;
  Y : Integer;
  X : Integer;
begin

  Gap := GetEmPart(CHECK_INTERNALMARGIN);
  Y := 0;
  if ChildCount > 0 then
    Inc(y,Row[0].Height+Gap);

  ACanvas.MoveTo(Gap,Y);
  ACanvas.LineTo(Width-Gap,Y);

  X := FLeftWidth+2*Gap;
  ACanvas.MoveTo(X,Y);
  ACanvas.LineTo(X,Height);

end;

function TIPEqCheck.GetCheckRowCount:Integer;
begin
  Result := (ChildCount-1) div 2;
end;

function TIPEqCheck.GetLeftRow(Index:Integer):TIPEqRow;
begin
  Result := Row[1+Index];
end;

function TIPEqCheck.GetRightRow(Index:Integer):TIPEqRow;
begin
  Result := Row[1+CheckRowCount+Index];
end;

function TIPEqCheck.GetText:String;
var
  I :Integer;
begin
  Result := '@'+GetName+'{';
  if ChildCount > 0 then
    Result := Result + Row[0].Text;
  Result := Result + ';{';

  for I := 0 to CheckRowCount-1 do
  begin
    if I > 0 then
      Result := Result + ';';
    Result := Result + LeftRow[I].Text;
  end;
  Result := Result + '};{';

  for I := 0 to CheckRowCount-1 do
  begin
    if I > 0 then
      Result := Result + ';';
    Result := Result + RightRow[I].Text;
  end;
  Result := Result + '}}';

end;

end.
