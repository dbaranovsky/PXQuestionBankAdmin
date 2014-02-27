unit IPEqBrace;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqBrace = class(TIPEqComposite)
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
    constructor CreateDL; overload;
    constructor CreateDR; overload;
    constructor CreateBraceL; overload;
    constructor CreateBraceR; overload;
    constructor Create(ARow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;

end;

const
  Brace_INTERNALMARGIN = 20;

implementation


uses IPEqUtils,Math,Types,ststrs;

constructor TIPEqBrace.Create;
begin
  FType := 1;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBrace.CreateN;
begin
  FType := 2;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBrace.CreateDL;
begin
  FType := 3;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBrace.CreateDR;
begin
  FType := 4;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBrace.CreateBraceL;
begin
  FType := 5;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBrace.CreateBraceR;
begin
  FType := 6;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBrace.Create(ARow:TIPEqRow);
begin
  inherited Create;
  AddRow(ARow);
end;

procedure TIPEqBrace.BuildMathML(Buffer:TStrings; Level:Integer);
var
  FStr : String;
begin
    if FType=1 then
      FStr := 'abc'
    else if FType=2 then
      FStr := 'norm'
    else if FType=3 then
      FStr := 'norml'
    else if FType=4 then
      FStr := 'normr'
    else if FType=5 then
      FStr := 'Bracel'
    else if FType=6 then
      FStr := 'Bracer';

    Buffer.Add(CharStrS(' ',Level)+'<m'+FStr+'>');
    Child[0].BuildMathML(Buffer,Level+1);
    Buffer.Add(CharStrS(' ',Level)+'</m'+FStr+'>');
end;

function TIPEqBrace.GetName:String;
begin
  if FType=1 then
    Result := 'Brace'
  else if FType=2 then
    Result := 'NORM'
  else if FType=3 then
    Result := 'NORML'
  else if FType=4 then
    Result := 'NORMR'
  else if FType=5 then
    Result := 'BraceL'
  else if FType=6 then
    Result := 'BraceR';
end;

function  TIPEqBrace.Clone:TIPEqNode;
begin
  Result := TIPEqBrace.Create;
  TIPEqBrace(Result).CopyChildren(Self);
end;

function TIPEqBrace.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  Fm : TTextMetric;
begin
  Node := Row[0];
  Em := GetEMWidth(Font);

  Fm := GetTextMetrics;

  Ascent := Fm.tmAscent;
  Descent := Fm.tmDescent;
  Width := 0;
  if FType=1 then
    Width := 2*GetEmPart(Brace_INTERNALMARGIN,Em)+2
  else if FType=2 then
     Width := 2*GetEmPart(Brace_INTERNALMARGIN,Em)+8
  else if FType=3 then
    Width := 2*GetEmPart(Brace_INTERNALMARGIN,Em)+4
  else if FType=4 then
    Width := 2*GetEmPart(Brace_INTERNALMARGIN,Em)+4
  else if FType=5 then
    Width := 2*GetEmPart(Brace_INTERNALMARGIN,Em)+1
  else if FType=6 then
    Width := 2*GetEmPart(Brace_INTERNALMARGIN,Em)+1;

  if Assigned(Node) then
  begin
    Max(Ascent,Node.Ascent);
    Max(Descent,Node.Descent);
    inc(Width,Node.Width);
  end;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqBrace.Layout;
var
  Node : TIPEqNode;
  X,Y:Integer;
begin
  Node := Row[0];
  if Assigned(Node) then
  begin
    case FType of
      1: X := 1+GetEmPart(Brace_INTERNALMARGIN);
      2: X := 4+GetEmPart(Brace_INTERNALMARGIN);
      3: X := 4+GetEmPart(Brace_INTERNALMARGIN);
      4: X := GetEmPart(Brace_INTERNALMARGIN);
      5: X := 1+GetEmPart(Brace_INTERNALMARGIN);
      6: X := GetEmPart(Brace_INTERNALMARGIN);
    else
      X := 0;
    end;

    Y := Ascent-Node.Ascent;
    Node.SetLocation(X,Y);
  end;
  
end;

procedure TIPEqBrace.Draw(ACanvas:TCanvas);
var
  X1,Y1 : Integer;
  X2,Y2 : Integer;
begin
  X1 := 0;
  Y1 := 0;
  X2 := Width-1;
  Y2 := Height;
  if (FType<>4) And (FType<>6) then
  begin
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X1,Y2);
  end;
  if (FType<>3) And (FType<>5) then
  begin
    ACanvas.MoveTo(X2,Y1);
    ACanvas.LineTo(X2,Y2);
  end;


  if FType=2 then
  begin
    ACanvas.MoveTo(X1+2,Y1);
    ACanvas.LineTo(X1+2,Y2);
    ACanvas.MoveTo(X2-2,Y1);
    ACanvas.LineTo(X2-2,Y2);
  end;

  if FType=3 then
  begin
    ACanvas.MoveTo(X1+2,Y1);
    ACanvas.LineTo(X1+2,Y2);
  end;

  if FType=4 then
  begin
    ACanvas.MoveTo(X2-2,Y1);
    ACanvas.LineTo(X2-2,Y2);
  end;

end;


end.
