unit IPEqDivide;

interface

uses IPEqNode,IPEqComposite,IPEqStack,Graphics,Windows,Classes;

type

TIPEqDivideType = (divNormal,divSmall);

TIPEqDivide = class(TIPEqStack)
  private
    FVOffset : Integer;
    FType : TIPEqDivideType;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor CreateM; overload;
    constructor Create(TopRow,BottomRow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function IsIntDiv:Boolean;
    procedure Paint(ACanvas:TCanvas); override;

end;

const
  DIV_OVERHANG = 20; //Amount of overhang from line to largest row
  DIV_WIDTH = 1;    //thickness of line -- in pixels for now
  DIV_HMARGIN =15;  //Space between left and right sides
  DIV_VMARGIN = 10;  //Space between top row and line and bottom row and line

implementation

uses Math, IPEqUtils,Types,StStrL;

constructor TIPEqDivide.Create;
begin
  FType := divNormal;
  Create(TIPEqRow.Create,TIPEqRow.Create);
end;

constructor TIPEqDivide.CreateM;
begin
  FType := divSmall;
  Create(TIPEqRow.Create,TIPEqRow.Create);
end;

constructor TIPEqDivide.Create(TopRow,BottomRow:TIPEqRow);
begin
  inherited Create;
  AddRow(TopRow);
  AddRow(BottomRow);
end;

function  TIPEqDivide.Clone:TIPEqNode;
begin
  Result := TIPEqDivide.Create;
  TIPEqDivide(Result).CopyChildren(Self);
end;

procedure TIPEqDivide.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
   Buffer.Add(CharStrL(' ',Level)+'<mfrac>');
   for I := 0 to ChildCount-1 do
     Child[I].BuildMathML(Buffer,Level+1);
   Buffer.Add(CharStrL(' ',Level)+'</mfrac>');
end;

function TIPEqDivide.GetName:String;
begin
  case FType of
    divNormal: Result := 'DIV';
    divSmall: Result := 'DIVSM';
  end;
end;

function TIPEqDivide.IsIntDiv:Boolean;
begin
  Result :=  Row[0].IsInteger and Row[1].IsInteger;
end;


function TIPEqDivide.CalcMetrics:TIPEqMetrics;
var
  EqTop,EqBottom : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  TextMetric : TTextMetric;
  MinGap : Integer;
  DenomDepth : Integer;
  NumBaseLine : Integer;
begin

  EqTop := Row[0];
  EqBottom := Row[1];

  //getmetrics of top before bottom so their order is preserve when
  //emdeding objects.
  EqTop.Metrics;
  EqBottom.Metrics;

  //if not Document.Editable and ((Assigned(EqTop) and not EqTop.RowsFilled) or (Assigned(EqBottom) and not EqBottom.RowsFilled)) then
  //begin
  //  Result := TIPEqMetrics.Create(0,0,0,0);
  //  exit;
  //end;


  if (FType = divSmall) then
  begin
    ReduceFontSize;
  end;

  //Use the Height here.  We need it to do size calculations
  TextMetric := GetTextMetrics;
  Em := GetFontHeight(TextMetric)-1;

  //This is the minimum gap allowed between the top of the denominator
  //and the divisor line.
  MinGap := GetEmPart(SP_MINIMUMGAP,Em);

  //This is the perferred distance between the divisor bar and the bottom of
  //the denominator
  DenomDepth := GetEmPart(SP_DENOMINATORDEPTH,Em);

  //Adjust denomBot based on height of bottom and MinGap
  if Assigned(EqBottom) then
  begin
    DenomDepth := Max(DenomDepth,EqBottom.Ascent+MinGap);
    Inc(DenomDepth,EqBottom.Descent);
  end;

  //Determine perfered distance from divisor line to numerator baseline
  NumBaseLine := GetEmPart(SP_NUMERATORHEIGHT,Em);

  //Adjust based on descent of numerator.
  if Assigned(EqTop) then
  begin
    NumBaseLine := Max(NumBaseLine,EqTop.Descent+MinGap);
  end;

  Descent := DenomDepth;
  Ascent := NumBaseLine;

  if Assigned(EqTop) then
    Inc(Ascent,EqTop.Ascent);

  Width := 0;
  if Assigned(EqTop) then
    Width := Max(Width,EqTop.Width);
  if Assigned(EqBottom) then
    Width := Max(Width,EqBottom.Width);

  //Adjust width for divisor bar overhang and margin
  Inc(Width,2*GetEmPart(SP_FRACTIONBAROVERHANG,Em)+2*GetEmPart(SP_FRACTIONBARMARGIN,Em));

  //Adjust ascent and descent to shift up so divisor bar is inline with middle of
  //adjacent characters.
  FVOffset := Round(TextMetric.tmAscent*0.5)-2;  //-2
  Inc(Ascent,FVOffset);
  Dec(Descent,FVOffset);

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqDivide.Layout;
var
  EqTop,EqBottom : TIPEqNode;
  X,Y:Integer;
begin

  EqTop := Row[0];
  EqBottom := Row[1];

  //if not Document.Editable and ((Assigned(EqTop) and not EqTop.RowsFilled) or (Assigned(EqBottom) and not EqBottom.RowsFilled)) then
  //begin
  //  exit;
  //end;

  if Assigned(EqTop) then
  begin
    X := (Width-EqTop.Width) div 2;
    Y := 0;
    EqTop.SetLocation(X,Y);
  end;

  if Assigned(EqBottom) then
  begin
    X := (Width-EqBottom.Width) div 2;
    Y := Height-EqBottom.Height;
    EqBottom.SetLocation(X,Y);
  end;
end;

procedure TIPEqDivide.Paint(ACanvas:TCanvas);
var
  EqTop,EqBottom : TIPEqNode;
begin

  EqTop := Row[0];
  EqBottom := Row[1];
  //if not Document.Editable and ((Assigned(EqTop) and not EqTop.RowsFilled) or (Assigned(EqBottom) and not EqBottom.RowsFilled)) then
  //  Exit;
  inherited Paint(ACanvas);
end;


procedure TIPEqDivide.Draw(ACanvas:TCanvas);
var
  I,X,Y,W : Integer;
begin
  X := GetEmPart(SP_FRACTIONBARMARGIN);
  Y := Ascent-FVOffset;
  W := Width-2*GetEmPart(SP_FRACTIONBARMARGIN);
  for I := 0 to DIV_WIDTH-1 do
  begin
    ACanvas.MoveTo(X,Y);
    ACanvas.LineTo(X+W,Y);
    Inc(Y);
  end;

  //ACanvas.Rectangle(Types.Bounds(0,0,Width+1,Height+1));
end;


end.

