unit IPEqVariable;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqVariable = class(TIPEqComposite)
  private
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor Create(ARow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;

end;

const
  VAR_INTERNALMARGIN = 20;
  VAR_BOX_COLOR: TColor = $0000CCFF; // orange

implementation


uses IPEqText, IPEqTextParser, IPEqUtils,Math,Types,ststrL;

constructor TIPEqVariable.Create;
begin
  Create(TIPEqRow.Create);
end;

constructor TIPEqVariable.Create(ARow:TIPEqRow);
begin
  inherited Create;
  AddRow(ARow);
end;

procedure TIPEqVariable.BuildMathML(Buffer:TStrings; Level:Integer);
var
  FStr : String;
begin
    FStr := 'var';
    Buffer.Add(CharStrL(' ',Level)+'<m'+FStr+'>');
    Child[0].BuildMathML(Buffer,Level+1);
    Buffer.Add(CharStrL(' ',Level)+'</m'+FStr+'>');
end;

function TIPEqVariable.GetName:String;
begin
  Result := 'VAR';
end;

function TIPEqVariable.Clone:TIPEqNode;
begin
  Result := TIPEqVariable.Create;
  TIPEqVariable(Result).CopyChildren(Self);
end;

function TIPEqVariable.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  TextMetric : TTextMetric;
  Tmp: Integer;
begin
  Node := Row[0];
  Em := getEMWidth(Font);

  TextMetric := GetTextMetrics;

  Ascent := TextMetric.tmAscent;
  Descent := TextMetric.tmDescent;

  Tmp := GetEmPart(VAR_INTERNALMARGIN,Em)+1;
  Width := 2*Tmp;
  Inc(Ascent, Tmp);
  Inc(Descent, Tmp);

  if Assigned(Node) then
  begin
    Ascent := Max(Ascent,Node.Ascent+Tmp);
    Descent := Max(Descent,Node.Descent+Tmp);
    Inc(Width,Node.Width);
  end;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqVariable.Layout;
var
  Node : TIPEqNode;
  X,Y:Integer;
begin
  Node := Row[0];
  if Assigned(Node) then
  begin
    X := 1+GetEmPart(VAR_INTERNALMARGIN);
    Y := Ascent-Node.Ascent;
    Node.SetLocation(X,Y);
  end;
  
end;

procedure TIPEqVariable.Draw(ACanvas:TCanvas);
var
  X1,Y1 : Integer;
  X2,Y2 : Integer;
  Color: TColor;
begin
  X1 := 0;
  Y1 := 0;
  X2 := Width-1;
  Y2 := Height-1;

  Color:= ACanvas.Pen.Color;
  ACanvas.Pen.Color:= VAR_BOX_COLOR;
  ACanvas.MoveTo(X1,Y1);
  ACanvas.LineTo(X1,Y2);
  ACanvas.LineTo(X2,Y2);
  ACanvas.LineTo(X2,Y1);
  ACanvas.LineTo(X1,Y1);
  ACanvas.Pen.Color:= Color;

end;

end.



