unit IPEqIntegral;

interface

uses IPEqNode,IPEqComposite,IPEqStack,Graphics,Windows,Classes;

type

TIPEqIntegral = class(TIPEqStack)
  private
    FIntSize : Integer;
    FSymbolVOffset : Integer;
    FSymbolWidth : Integer;
    FSymbolHeight : Integer;
    FIsSymbol : Boolean;
    FIsContour: Boolean;
  private
    function GetHGap: Integer;
    function GetWGap: Integer;
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
    constructor CreateContour;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    procedure ReduceEmptyRows;
    function GetText:String; override;
    property IsSymbol:boolean read FIsSymbol write FIsSymbol;
    property IsContour:Boolean read FIsContour write FIsContour;

end;

const
  INTEGRAL_OVERHANG = 20; //Amount of overhang from line to largest row
  INTEGRALWidth = 50;    //thickness of line -- in pixels for now
  INTEGRAL_HMARGIN = 10;  //Space between left and right sides
  INTEGRAL_VMARGIN = 0;  //Space between top row and line and bottom row and line
  INTEGRAL_CONTOURRAD = 30; //Radius of Countour Circle in Percent of Width of Integral Sign

implementation

uses Math, IPEqUtils,Types,StStrL;

constructor TIPEqIntegral.Create;
begin
  inherited Create;
end;

constructor TIPEqIntegral.CreateN();
begin
  inherited Create;
  AddRow(TIPEqRow.Create);
  AddRow(TIPEqRow.Create);
end;

constructor TIPEqIntegral.CreateSymbol();
begin
  inherited Create;
  FIsSymbol := true;
end;

constructor TIPEqIntegral.CreateContour();
begin
  inherited Create;
  FIsSymbol := true;
  FIsContour := true;
end;


procedure TIPEqIntegral.ReduceEmptyRows;
begin
  if ChildCount = 2 then
  begin
    if (Row[0].ChildCount = 0) and (Row[1].ChildCount = 0) then
      Clear;
  end;
end;

function  TIPEqIntegral.Clone:TIPEqNode;
begin
  Result := TIPEqIntegral.Create;
  (Result as TIPEqIntegral).IsSymbol := FIsSymbol;
  (Result as TIPEqIntegral).IsContour := FIsContour;
  TIPEqIntegral(Result).CopyChildren(Self);
end;

procedure TIPEqIntegral.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
   Buffer.Add(CharStrL(' ',Level)+'<mfrac>');
   for I := 0 to ChildCount-1 do
     Child[i].BuildMathML(Buffer,Level+1);
   Buffer.Add(CharStrL(' ',Level)+'</mfrac>');
end;

function TIPEqIntegral.GetName:String;
begin
  Result := 'INTEGRAL';
end;

function TIPEqIntegral.CalcMetrics:TIPEqMetrics;
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
  TextMetric := GetTextMetrics(AFont);
  Siz := GetTextExtent(#$F3,AFont);
  AFont.Free;

  FSymbolVOffset := TextMetric.tmInternalLeading;
  FSymbolWidth := Siz.cx;
  FSymbolHeight := Siz.cy;
  FIntSize := TextMetric.tmAscent*2;
  Inc(Height,FIntSize);

  if Assigned(EqBottom) then
  begin
    EqBottom.ReduceFontSize;
    Inc(Height,EqBottom.Height div 2 + HGap);
    Width := Max(Width,EqBottom.Width+FSymbolWidth div 2+WGap);
  end;

  if Assigned(EqTop) then
  begin
    EqTop.ReduceFontSize;
    Inc(Height,EqTop.Height div 2 + HGap);
    Width := Max(Width,EqTop.Width+FSymbolWidth+2*WGap);
  end;

  Width := Max(Width,siz.cx);
  Inc(Width,2*WGap);

  Ascent := Height div 2 + Offset;
  Descent := Height-Ascent;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqIntegral.Layout;
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
    X := FSymbolWidth+3*WGap; //(Width -EqTop.Width) div 2;
    Y := 0;
    EqTop.SetLocation(x,y);
  end;

  if Assigned(EqBottom) then
  begin
    X := FSymbolWidth div 2 + 2*WGap; //(Width -EqBottom.Width) div 2;
    Y := Height-EqBottom.Height;
    EqBottom.SetLocation(X,Y);
  end;
end;

procedure TIPEqIntegral.Draw(ACanvas:TCanvas);
var
  EqTop,EqBottom : TIPEqNode;
  TopHeight : Integer;
  BottomHeight : Integer;
  X, Y : Integer;
  TextMetric : TTextMetric;
  FRecall : TFOntRecall;
  Xc,Yc,Rc : Integer;
begin
  TextMetric := GetTextMetrics;

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

  if Assigned(EqBottom) then
  begin
    BottomHeight := EqBottom.Height;
  end
  else
  begin
    BottomHeight := 0;
  end;

  if Assigned(EqTop) then
  begin
    TopHeight := EqTop.Height;
  end
  else
  begin
    TopHeight := 0;
  end;

  X := WGap;
  Y := TopHeight div 2 + HGap;

  FRecall := TFontRecall.Create(ACanvas.Font);
  try
    ACanvas.Font := Font;
    ACanvas.Font.Name := 'Symbol';
    ACanvas.TextOut(x,y,#$F3);

    Y := Height-BottomHeight div 2-HGap-FSymbolHeight;
    ACanvas.TextOut(X,Y,#$F5);

    if FIsContour then
    begin
      Xc := X + FSymbolWidth div 2;
      Yc := Y + FSymbolVOffset div 2;
      Rc := (FSymbolWidth * INTEGRAL_CONTOURRAD) div 100;
      ACanvas.Ellipse(Xc-Rc,Yc-Rc,Xc+Rc+1,Yc+Rc+1);
    end;

  finally
    FRecall.Free;
  end;

end;

function TIPEqIntegral.GetText:String;
begin
  if FIsSymbol then
  begin
    if FIsContour then
      Result := '&contourintegral;'
    else
      Result := '&integral;'
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

function TIPEqIntegral.GetHGap: Integer;
var
  Em : Integer;
begin
  Em := getEMWidth(Font);
  Result := GetEmPart(INTEGRAL_VMARGIN,Em);
end;

function TIPEqIntegral.GetWGap: Integer;
var
  Em : Integer;
begin
  Em := getEMWidth(Font);
  Result := GetEmPart(INTEGRAL_HMARGIN,Em);
end;

end.

