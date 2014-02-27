unit IPEqLDiv;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqLDiv = class(TIPEqComposite)
  private
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
  MARGIN_IN = 10;
  MARGIN_EX = 15;
  MARGIN_TOP = 10;
  MARGIN_ARC = 20;

implementation


uses IPEqUtils,Math,Types,ststrL;

constructor TIPEqLDiv.Create;
begin
  inherited Create;
  AddRow(TIPEqRow.Create);
  AddRow(TIPEqRow.Create);
end;

constructor TIPEqLDiv.CreateN;
begin
  inherited Create;
  AddRow(TIPEqRow.Create);
  AddRow(TIPEqRow.Create);
  AddRow(TIPEqRow.Create);
end;

procedure TIPEqLDiv.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
   if ChildCount = 2 then
   begin
     Buffer.Add(CharStrL(' ',Level)+'<mLDiv>');
     for I := 0 to ChildCount-1 do
       Child[I].BuildMathML(Buffer,Level+1);
     Buffer.Add(CharStrL(' ',Level)+'</mLDiv>');
   end
   else
   begin
     Buffer.Add(CharStrL(' ',Level)+'<mLDivQ>');
     for I := 0 to ChildCount-1 do
       Child[I].BuildMathML(Buffer,Level+1);
     Buffer.Add(CharStrL(' ',Level)+'</mLDivQ>');
   end;
end;

function TIPEqLDiv.GetName:String;
begin
  if ChildCount = 2 then
    Result := 'LDiv'
  else
    Result := 'LDivQ';
end;

function  TIPEqLDiv.Clone:TIPEqNode;
begin
  Result := TIPEqLDiv.Create;
  TIPEqLDiv(Result).CopyChildren(Self);
end;

function TIPEqLDiv.CalcMetrics:TIPEqMetrics;
var
  ARow,BRow,CRow : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  AAscent,ADescent,AWidth : Integer;
  BAscent,BDescent,BWidth : Integer;
  CAscent,CDescent : Integer;
  EmEx, EmIn, EmArc, EmTop : Integer;
begin
  ARow := Row[0];
  BRow := Row[1];

  if ChildCount = 3 then
    CRow := Row[2]
  else
    CRow := nil;

  Em := getEMWidth(Font);

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

  if Assigned(CRow) then
  begin
    CAscent := CRow.Ascent;
    CDescent := CRow.Descent;
  end
  else
  begin
    CAscent := 0;
    CDescent := 0;
  end;

  EmEx := GetEmPart(MARGIN_EX,Em);
  EmIn := GetEmPart(MARGIN_IN,Em);
  EmArc := GetEmPart(MARGIN_ARC,Em);
  EmTop := GetEmPart(MARGIN_TOP,Em);

  Ascent := Max(BAscent,AAscent+CAscent+CDescent+2*EmTop+1);
  Descent := Max(BDescent, ADescent);
  Width := 2*EmEx+3*EmIn+EmArc+BWidth+Max(AWidth,BWidth);

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqLDiv.Layout;
var
  ARow,BRow,CRow : TIPEqNode;
  AAscent,AWidth : Integer;
  BAscent : Integer;
  CWidth : Integer;
  EmEx, EmIn : Integer;
begin
  ARow := Row[0];
  BRow := Row[1];

  if ChildCount = 3 then
    CRow := Row[2]
  else
    CRow := nil;

  if Assigned(ARow) then
  begin
    AAscent := ARow.Ascent;
    AWidth := ARow.Width;
  end
  else
  begin
    AAscent := 0;
    AWidth := 0;
  end;

  if Assigned(BRow) then
  begin
    BAscent := BRow.Ascent;
  end
  else
  begin
    BAscent := 0;
  end;

  if Assigned(CRow) then
  begin
    CWidth := CRow.Width;
  end
  else
  begin
    CWidth := 0;
  end;

  EmEx := GetEmPart(MARGIN_EX);
  EmIn := GetEmPart(MARGIN_IN);

  if Assigned(ARow) then
    ARow.SetLocation(Width-EmEx-EmIn-Max(AWidth,CWidth),Ascent-AAscent);

  if Assigned(BRow) then
    BRow.SetLocation(EmEx,Ascent-BAscent);

  if Assigned(CRow) then
    CRow.SetLocation(Width-EmEx-EmIn-Max(AWidth,CWidth),0);

end;

procedure TIPEqLDiv.Draw(ACanvas:TCanvas);
var
  ARow,BRow,CRow : TIPEqNode;
  AAscent,ADescent : Integer;
  BWidth : Integer;
  CAscent,CDescent : Integer;
  EmEx, EmIn, EmArc, EmTop : Integer;
  X1, Y1, X2, Y2 : Integer;
  W,H,R : Integer;
  Xe1,Ye1,Xe2,Ye2,Xe3,Ye3,Xe4,Ye4 : Integer;
begin
  ARow := Row[0];
  BRow := Row[1];

  if ChildCount = 3 then
    CRow := Row[2]
  else
    CRow := nil;

  if Assigned(ARow) then
  begin
    AAscent := ARow.Ascent;
    ADescent := ARow.Descent;
  end
  else
  begin
    AAscent := 0;
    ADescent := 0;
  end;

  if Assigned(BRow) then
  begin
    BWidth := BRow.Width;
  end
  else
  begin
    BWidth := 0;
  end;

  if Assigned(CRow) then
  begin
    CAscent := CRow.Ascent;
    CDescent := CRow.Descent;
  end
  else
  begin
    CAscent := 0;
    CDescent := 0;
  end;

  EmEx := GetEmPart(MARGIN_EX);
  EmIn := GetEmPart(MARGIN_IN);
  EmArc := GetEmPart(MARGIN_ARC);
  EmTop := GetEmPart(MARGIN_TOP);

  X1 := BWidth+EmEx+EmIn;
  Y1 := CAscent+CDescent+EmTop;
  X2 := Width-EmEx;
  Y2 := Y1;

  ACanvas.MoveTo(X1,Y1);
  ACanvas.LineTo(X2,Y2);

  W := EmArc;
  H := AAscent+ADescent+EmIn+1;
  R := (W*W+(H*H div 4)) div (2*W);

  Xe4 := X1;
  Ye4 := Y1;
  Xe3 := X1;
  Ye3 := Y1+h;
  Xe1 := X1+w-2*r;
  Ye1 := Y1+ h div 2 -r;
  Xe2 := Xe1+2*r;
  Ye2 := Ye1+2*r;
  ACanvas.Arc(Xe1,Ye1,Xe2,Ye2,Xe3,Ye3,Xe4,Ye4);
end;

end.



