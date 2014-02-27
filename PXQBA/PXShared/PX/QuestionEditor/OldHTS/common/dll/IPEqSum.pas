unit IPEqSum;

interface

uses IPEqNode,IPEqComposite,IPEqStack,Graphics,Windows,Classes;

type

TIPEqSum = class(TIPEqStack)
  private
    function GetHGap: Integer;
    function GetWGap: Integer;
  private
    FIntSize : Integer;
    FSymbolVOffset : Integer;
    FSymbolWidth : Integer;
    FSymbolHeight : Integer;
    FIsSymbol : Boolean;
    property WGap: Integer read GetWGap;
    property HGap: Integer read GetHGap;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor CreateN; overload;
    constructor CreateSymbol;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    procedure ReduceEmptyRows;
    function GetText:String; override;
    property IsSymbol:boolean read FIsSymbol write FIsSymbol;

end;

const
  SUM_OVERHANG = 20; //Amount of overhang from line to largest row
  SUM_WIDTH = 50;    //thickness of line -- in pixels for now
  SUM_HMARGIN = 10;  //Space between left and right sides
  SUM_VMARGIN = 0;  //Space between top row and line and bottom row and line

implementation

uses Math, IPEqUtils,Types,StStrL;

constructor TIPEqSum.Create;
begin
  inherited Create;
end;

constructor TIPEqSum.CreateN();
begin
  inherited Create;
  AddRow(TIPEqRow.Create);
  AddRow(TIPEqRow.Create);
end;

constructor TIPEqSum.CreateSymbol();
begin
  inherited Create;
  FIsSymbol := true;
end;

procedure TIPEqSum.ReduceEmptyRows;
begin
  if ChildCount = 2 then
  begin
    if (Row[0].ChildCount = 0) and (Row[1].ChildCount = 0) then
      Clear;
  end;
end;

function  TIPEqSum.Clone:TIPEqNode;
begin
  Result := TIPEqSum.Create;
  (Result as TIPEqSum).IsSymbol := FIsSymbol;
  TIPEqSum(Result).CopyChildren(Self);
end;

procedure TIPEqSum.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
   Buffer.Add(CharStrL(' ',Level)+'<msum>');
   for I := 0 to ChildCount-1 do
     Child[i].BuildMathML(Buffer,Level+1);
   Buffer.Add(CharStrL(' ',Level)+'</msum>');
end;

function TIPEqSum.GetName:String;
begin
  Result := 'SUM';
end;

function TIPEqSum.CalcMetrics:TIPEqMetrics;
var
  EqTop,EqBottom : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Height : Integer;
  Em : Integer;
  TextMetric : TTextMetric;
  AFont : TFOnt;
  Siz : TSize;
  Offset : Integer;
begin
  if ChildCount=2 then
  begin
    EqBottom := Row[0];
    EqTop := Row[1];
  end
  else
  begin
    EqBottom := nil;
    EqTop := nil;
  end;

  Em := getEMWidth(Font);

  TextMetric := GetTextMetrics;
  Offset := TextMetric.tmAscent div 4;

  Height := 0;
  Width  := 0;

  AFont := TFont.Create;
  AFont.Assign(Font);
  AFont.Name := 'Symbol';
  AFont.Size := Round(FOnt.Size*1.5);
  TextMetric := GetTextMetrics(AFont);
  Siz := GetTextExtent(#$E5,AFont);
  AFont.Free;

  FSYmbolVOffset := TextMetric.tmInternalLeading;
  FSymbolWidth := Siz.Cx;
  FSymbolHeight := Siz.Cy;
  FIntSize := TextMetric.tmAscent+TextMetric.tmDescent-TextMetric.tmInternalLeading;
  Inc(Height,FIntSize);

  if Assigned(EqBottom) then
  begin
    EqBottom.ReduceFontSize;
    Inc(Height,EqBottom.Height div 2+HGap);
    Width := Max(Width,EqBottom.Width+FSymbolWidth);
  end;

  if Assigned(EqTop) then
  begin
    EqTop.ReduceFontSize;
    Inc(Height,EqTop.Height div 2+HGap);
    Width := Max(Width,EqTop.Width+FSymbolWidth);
  end;
  
  Width := Max(Width,Siz.Cx);
  Inc(Width,2*WGap);

  Ascent := Height div 2 + Offset;
  Descent := Height-Ascent;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqSum.Layout;
var
  EqTop,EqBottom : TIPEqNode;
  X, Y : Integer;
begin
  if ChildCount=2 then
  begin
    EqBottom := Row[0];
    EqTop := Row[1];
  end
  else
  begin
    EqBottom := nil;
    EqTop := nil;
  end;

  if Assigned(EqTop) then
  begin
    X := FSymbolWidth + WGap;
    Y := 0;
    EqTop.SetLocation(X,Y);
  end;

  if Assigned(EqBottom) then
  begin
    X := FSymbolWidth + WGap;
    Y := Height-EqBottom.Height;
    EqBottom.SetLocation(X,Y);
  end;
end;

procedure TIPEqSum.Draw(ACanvas:TCanvas);
var
  EqTop : TIPEqNode;
  TopHeight : Integer;
  X, Y : Integer;
  TextMetric : TTextMetric;
  Frecall : TFontRecall;
begin

  if ChildCount=2 then
  begin
    EqTop := Row[1];
  end
  else
  begin
    EqTop := nil;
  end;


  if Assigned(EqTop) then
  begin
    TopHeight := EqTop.Height;
  end
  else
  begin
    TopHeight := 0;
  end;

  Frecall := TFontRecall.Create(ACanvas.Font);
  try
    ACanvas.Font := Font;
    ACanvas.FOnt.Name := 'Symbol';
    ACanvas.Font.Size := Round(Font.Size*1.5);
    TextMetric := GetTextMetrics(ACanvas.Font);
    X := WGap;
    Y := TopHeight div 2+HGap-TextMetric.tmInternalLeading;
    ACanvas.TextOut(X,Y,#$E5);
  finally
    Frecall.Free;
  end;


end;

function TIPEqSum.GetText:String;
begin
  if FIsSymbol then
  begin
    Result := '&sum;'
  end
  else if ChildCount=2 then
  begin
    Result := inherited GetText;
  end
  else
  begin
    Result := '@'+GetName + '{;}';
  end;
end;

function TIPEqSum.GetHGap: Integer;
var
  Em : Integer;
begin
  Em := getEMWidth(Font);
  Result := GetEmPart(Sum_VMARGIN,Em);
end;

function TIPEqSum.GetWGap: Integer;
var
  Em : Integer;
begin
  Em := getEMWidth(Font);
  Result := GetEmPart(Sum_HMARGIN,Em);
end;

end.

