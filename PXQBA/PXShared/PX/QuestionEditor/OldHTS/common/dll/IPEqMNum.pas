unit IPEqMNum;

interface

uses IPEqNode,IPEqComposite,IPEqStack,Graphics,Windows,Classes,IPEqDivide;

type


TIPEqMnum = class(TIPEqStack)
  private
    FVOffset : Integer;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
    procedure PaintChildren(ACanvas:TCanvas); override;
  public
    constructor Create; overload;
    constructor Create(NumRow,TopRow,BottomRow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    procedure Paint(ACanvas:TCanvas); override;
end;


implementation

uses Math, IPEqUtils,Types,StStrL;

constructor TIPEqMNum.Create;
begin
  Create(TIPEqRow.Create,TIPEqRow.Create,TIPEqRow.Create);
end;


constructor TIPEqMNum.Create(NumRow,TopRow,BottomRow:TIPEqRow);
begin
  inherited Create;
  AddRow(NumRow);
  AddRow(TopRow);
  AddRow(BottomRow);
end;

function  TIPEqMNum.Clone:TIPEqNode;
begin
  Result := TIPEqMNum.Create;
  TIPEqMNum(Result).CopyChildren(Self);
end;

procedure TIPEqMNum.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
   Buffer.Add(CharStrL(' ',Level)+'<mnum>');
   for I := 0 to ChildCount-1 do
     Child[I].BuildMathML(Buffer,Level+1);
   Buffer.Add(CharStrL(' ',Level)+'</mmum>');
end;

function TIPEqMNum.GetName:String;
begin
  Result := 'MNUM';
end;

function TIPEqMNum.CalcMetrics:TIPEqMetrics;
var

  EqNum,EqTop,EqBottom : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  TextMetric : TTextMetric;
  MinGap : Integer;
  DenomDepth : Integer;
  NumBaseLine : Integer;
begin

  EqNum := Row[0];
  EqTop := Row[1];
  EqBottom := Row[2];

  //getmetrics of top before bottom so their order is preserve when
  //emdeding objects.
  EqNum.Metrics;
  EqTop.Metrics;
  EqBottom.Metrics;

  if not Document.Editable and
  (
  (Assigned(EqNum) and not EqNum.RowsFilled) or
  (Assigned(EqTop) and not EqTop.RowsFilled) or
  (Assigned(EqBottom) and not EqBottom.RowsFilled)
  ) then
  begin
    Result := TIPEqMetrics.Create(0,0,0,0);
    Exit;
  end;

  //Use the Height here.  We need it to do size calculations
  TextMetric := GetTextMetrics;
  Em := GetFontHeight(TextMetric);

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

  //**MNUM - Add width for number part
  Inc(Width,EqNum.Width);

  //Adjust ascent and descent to shift up so divisor bar is inline with middle of
  //adjacent characters.
  FVOffset := Round(TextMetric.tmAscent*0.5);
  Inc(Ascent,FVOffset);
 dec(Descent,FVOffset);

 //**MNUM - Adjust ascent and descent just in case
 Ascent := Max(Ascent,EqNum.Ascent);
 Descent := Max(Descent,EqNum.Descent);

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqMNum.Layout;
var
  EqNum,EqTop,EqBottom : TIPEqNode;
  W,X,Y:Integer;
begin

  EqNum := Row[0];
  EqTop := Row[1];
  EqBottom := Row[2];

  if not Document.Editable and (
   (Assigned(EqNum) and not EqNum.RowsFilled) or
   (Assigned(EqTop) and not EqTop.RowsFilled) or
   (Assigned(EqBottom) and not EqBottom.RowsFilled)) then
  begin
    Exit;
  end;

  if Assigned(EqNum) then
  begin
    X := 0;
    Y := Height-Descent-EqNum.Ascent;
    EqNum.SetLocation(X,Y);
  end;

  if Assigned(EqTop) then
  begin
    if Assigned(EqNum) then
      W := EqNum.Width
    else
      W := 0;
    X := W+((Width-W)-EqTop.Width) div 2;
    Y := 0;
    EqTop.SetLocation(X,Y);
  end;

  if Assigned(EqBottom) then
  begin
    if Assigned(EqNum) then
      W := EqNum.Width
    else
      W := 0;
    X := W+((Width-W)-EqBottom.Width) div 2;
    Y := Height-EqBottom.Height;
    EqBottom.SetLocation(X,Y);
  end;
end;

procedure TIPEqMNum.Paint(ACanvas:TCanvas);
var
  EqNum,EqTop,EqBottom : TIPEqNode;
begin
  EqNum := Row[0];
  EqTop := Row[1];
  EqBottom := Row[2];
  if not Document.Editable and (
  (Assigned(EqNum) and not EqNum.RowsFilled) or
  (Assigned(EqTop) and not EqTop.RowsFilled) or
  (Assigned(EqBottom) and not EqBottom.RowsFilled)) then
    Exit;
  inherited Paint(ACanvas);
end;

procedure TIPEqMnum.PaintChildren(ACanvas:TCanvas);
var
  OldBrush : TBrushRecall;
begin
  if Document.Authoring then
  begin
    OldBrush := TBrushRecall.Create(Acanvas.Brush);
    try
      ACanvas.Brush.Color := RGB(225,225,225);
      ACanvas.FillRect(Types.Bounds(0,0,Width+1,Height+1));
    finally
      OldBrush.Free;
    end;
  end;
  inherited PaintChildren(ACanvas);
end;

procedure TIPEqMNum.Draw(ACanvas:TCanvas);
var
  I,X,Y,W,NumW : Integer;
begin
  Y := Ascent-FVOffset;
  if Assigned(Row[0]) then
    NumW := Row[0].Width
  else
    NumW := 0;
  X := GetEmPart(SP_FRACTIONBARMARGIN)+NumW;
  W := (Width-NumW)-2*GetEmPart(SP_FRACTIONBARMARGIN);
  for i := 0 to DIV_WIDTH-1 do
  begin
    ACanvas.MoveTo(X,Y);
    ACanvas.LineTo(X+W,Y);
    Inc(Y);
  end;
end;


end.

