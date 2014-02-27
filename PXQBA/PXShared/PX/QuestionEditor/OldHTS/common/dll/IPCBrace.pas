unit IPCBrace;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqABS = class(TIPEqComposite)
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
    constructor CreateABSL; overload;
    constructor CreateABSR; overload;
    constructor Create(ARow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; level:Integer); override;

end;

const
  ABS_INTERNALMARGIN = 20;

implementation


uses IPEqUtils,Math,Types,ststrs;

constructor TIPEqABS.Create;
begin
  FType := 1;
  Create(TIPEqRow.Create);
end;

constructor TIPEqABS.CreateN;
begin
  FType := 2;
  Create(TIPEqRow.Create);
end;

constructor TIPEqABS.CreateDL;
begin
  FType := 3;
  Create(TIPEqRow.Create);
end;

constructor TIPEqABS.CreateDR;
begin
  FType := 4;
  Create(TIPEqRow.Create);
end;

constructor TIPEqABS.CreateABSL;
begin
  FType := 5;
  Create(TIPEqRow.Create);
end;

constructor TIPEqABS.CreateABSR;
begin
  FType := 6;
  Create(TIPEqRow.Create);
end;

constructor TIPEqABS.Create(ARow:TIPEqRow);
begin
  inherited Create;
  addRow(ARow);
end;

procedure TIPEqABS.BuildMathML(Buffer:TStrings; level:Integer);
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
      FStr := 'absl'
    else if FType=6 then
      FStr := 'absr';

    Buffer.Add(CharStrS(' ',level)+'<m'+FStr+'>');
    Child[0].BuildMathML(Buffer,level+1);
    Buffer.Add(CharStrS(' ',level)+'</m'+FStr+'>');
end;

function TIPEqABS.GetName:String;
begin
  if FType=1 then
    Result := 'ABS'
  else if FType=2 then
    Result := 'NORM'
  else if FType=3 then
    Result := 'NORML'
  else if FType=4 then
    Result := 'NORMR'
  else if FType=5 then
    Result := 'ABSL'
  else if FType=6 then
    Result := 'ABSR';
end;

function  TIPEqABS.Clone:TIPEqNode;
begin
  Result := TIPEqABS.Create;
  TIPEqABS(Result).CopyChildren(Self);
end;

function TIPEqABS.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  Fm : TTextMetric;
begin
  Node := Row[0];
  Em := getEMWidth(Font);

  Fm := GetTextMetrics;

  Ascent := fm.tmAscent;
  Descent := fm.tmDescent;
  Width := 0;
  if FType=1 then
    Width := 2*GetEmPart(ABS_INTERNALMARGIN,Em)+2
  else if FType=2 then
     Width := 2*GetEmPart(ABS_INTERNALMARGIN,Em)+8
  else if FType=3 then
    Width := 2*GetEmPart(ABS_INTERNALMARGIN,Em)+4
  else if FType=4 then
    Width := 2*GetEmPart(ABS_INTERNALMARGIN,Em)+4
  else if FType=5 then
    Width := 2*GetEmPart(ABS_INTERNALMARGIN,Em)+1
  else if FType=6 then
    Width := 2*GetEmPart(ABS_INTERNALMARGIN,Em)+1;

  if assigned(Node) then
  begin
    Max(Ascent,Node.Ascent);
    Max(Descent,Node.Descent);
    inc(Width,Node.Width);
  end;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqABS.Layout;
var
  Node : TIPEqNode;
  X,Y:Integer;
begin
  Node := Row[0];
  X := 0;
  if assigned(Node) then
  begin
    if FType=1 then
      X := 1+GetEmPart(ABS_INTERNALMARGIN)
    else if FType=2 then
      X := 4+GetEmPart(ABS_INTERNALMARGIN)
    else if FType=3 then
      X := 4+GetEmPart(ABS_INTERNALMARGIN)
    else if FType=4 then
      X := GetEmPart(ABS_INTERNALMARGIN)
    else if FType=5 then
      X := 1+GetEmPart(ABS_INTERNALMARGIN)
    else if FType=6 then
      X := GetEmPart(ABS_INTERNALMARGIN);

    Y := Ascent-Node.Ascent;
    node.SetLocation(X,Y);
  end;
  
end;

procedure TIPEqABS.Draw(ACanvas:TCanvas);
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



