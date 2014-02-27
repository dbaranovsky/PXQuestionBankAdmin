unit IPEqCIS;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqCIS = class(TIPEqComposite)
  private
    FType : Integer;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor CreateN; overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;

end;

const
  MARGIN_IN = 20;
  MARGIN_DOWN = 10;
  MARGIN_ANGLE = 10;
  MIN_DOWN = 3;
  MIN_ANGLE = 5;

implementation


uses IPEqUtils,Math,Types,ststrL;

constructor TIPEqCIS.Create;
begin
  FType := 1;
  inherited Create;
  AddRow(TIPEqRow.Create);
  AddRow(TIPEqRow.Create);
end;

constructor TIPEqCIS.CreateN;
begin
  FType := 2;
  inherited Create;
  AddRow(TIPEqRow.Create);
  AddRow(TIPEqRow.Create);
end;

procedure TIPEqCIS.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
  FStr : String;
begin
    if FType=1 then
      FStr := 'cis'
    else if FType=2 then
      FStr := 'syndiv';

    Buffer.Add(CharStrL(' ',Level)+'<m'+FStr+'>');
     for I := 0 to ChildCount-1 do
       Child[I].BuildMathML(Buffer,Level+1);
    Buffer.Add(CharStrL(' ',Level)+'</m'+FStr+'>');
end;

function TIPEqCIS.GetName:String;
begin
    if FType=1 then
      RESULT := 'CIS'
    else if FType=2 then
      RESULT := 'SYNDIV';
end;

function  TIPEqCIS.Clone:TIPEqNode;
begin
  Result := TIPEqCIS.Create;
  TIPEqCIS(Result).FType := FType;
  TIPEqCIS(Result).CopyChildren(Self);
end;

function TIPEqCIS.CalcMetrics:TIPEqMetrics;
var
  ARow,BRow : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  AAscent,ADescent,AWidth : Integer;
  BAscent,BDescent,BWidth : Integer;
  EmIn, EmDraw, EmDown : Integer;

begin
  Em := getEMWidth(Font);
  EmDown := Max(MIN_DOWN,GetEmPart(MARGIN_DOWN,Em));

  ARow := nil; // Compiler warnings cleanup 1/3/05
  BRow := nil;
  EmDraw := 0;

  if FType=1 then
  begin
    EmDraw := Max(MIN_ANGLE,GetEmPart(MARGIN_ANGLE,Em));
    ARow := Row[1];
    BRow := Row[0];
  end
  else if FType=2 then
  begin
    EmDraw := 1;
    ARow := Row[0];
    BRow := Row[1];
  end;

  if Assigned(ARow) then
  begin
    AAscent := ARow.Ascent;
    ADescent := ARow.Descent;
    AWidth := ARow.Width;
  end
  else
  begin
    AAscent := 0;
    ADescent := 0;
    AWidth := 0;
  end;

  if Assigned(BRow) then
  begin
    BAscent := BRow.Ascent;
    BDescent := BRow.Descent;
    BWidth := BRow.Width;
  end
  else
  begin
    BAscent := 0;
    BDescent := 0;
    BWidth := 0;
  end;


  EmIn := GetEmPart(MARGIN_IN,Em);

  Ascent := Max(BAscent,AAscent);
  Descent := Max(BDescent, ADescent)+EmDown;
  Width := 2*EmIn+AWidth+BWidth+EmDraw;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqCIS.Layout;
var
  ARow,BRow : TIPEqNode;
begin
  ARow := nil; 
  BRow := nil;

  if FType=1 then
  begin
    ARow := Row[1];
    BRow := Row[0];
  end
  else if FType=2 then
  begin
    ARow := Row[0];
    BRow := Row[1];
  end;

  if Assigned(ARow) then
  begin
    ARow.SetLocation(Width-ARow.Width,Ascent-ARow.Ascent);
  end;

  if Assigned(BRow) then
  begin
    BRow.SetLocation(0,Ascent-BRow.Ascent);
  end;

end;

procedure TIPEqCIS.Draw(ACanvas:TCanvas);
var
  BRow : TIPEqNode;
  EmIn, EmDraw : Integer;
  X1, Y1, X2, Y2, X3, Y3 : Integer;
begin
  EmIn := GetEmPart(MARGIN_IN);

  BRow := nil;
  EmDraw := 0;

  if FType=1 then
  begin
    EmDraw := Max(MIN_ANGLE,GetEMPart(MARGIN_ANGLE));
    BRow := Row[0];
  end
  else if FType=2 then
  begin
    EmDraw := 0;
    BRow := Row[1];
  end;


  X1 := BRow.Width+EmIn+EmDraw;
  Y1 := 0;
  X2 := BRow.Width+EmIn;
  Y2 := Height-1;
  X3 := Width;
  Y3 := Height-1;

  ACanvas.MoveTo(X1,Y1);
  ACanvas.LineTo(X2,Y2);
  ACanvas.LineTo(X3,Y3);
end;

end.



