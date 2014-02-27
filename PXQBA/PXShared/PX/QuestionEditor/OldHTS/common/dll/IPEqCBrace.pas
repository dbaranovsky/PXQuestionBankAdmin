unit IPEqCBrace;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqCBrace = class(TIPEqComposite)
  private
    FType : Integer;
    FParenWidth : Integer;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor CreateCBraceL; overload;
    constructor CreateCBraceR; overload;
    constructor CreateParen; overload;
    constructor CreateParenL; overload;
    constructor CreateParenR; overload;
    constructor CreateBrace; overload;
    constructor CreateBraceL; overload;
    constructor CreateBraceR; overload;
    constructor CreateVector; overload;
    constructor CreateVectorL; overload;
    constructor CreateVectorR; overload;
    constructor CreateGrint; overload;
    constructor CreateDBraceL; overload;
    constructor CreateDBraceR; overload;
    constructor CreateHBrace; overload;
    constructor CreateHBraceT; overload;
    constructor CreateHBraceB; overload;
    constructor Create(ARow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;

    property IPType:Integer read FType write FType;
end;

const
  CBRACE_GAP = 20;
  CBRACE_THICKFONT = 13;

  CBRACE_CBRACE  = 1;
  CBRACE_CBRACEL = 2;
  CBRACE_CBRACER = 3;
  CBRACE_PAREN   = 4;
  CBRACE_PARENL  = 5;
  CBRACE_PARENR  = 6;
  CBRACE_BRACE   = 7;
  CBRACE_BRACEL  = 8;
  CBRACE_BRACER  = 9;
  CBRACE_VECTOR   = 10;
  CBRACE_VECTORL  = 11;
  CBRACE_VECTORR  = 12;
  CBRACE_GRINT    = 13;
  CBRACE_DBRACEL  = 14;
  CBRACE_DBRACER  = 15;
  CBRACE_HBRACE   = 16;
  CBRACE_HBRACET  = 17;
  CBRACE_HBRACEB  = 18;

implementation


uses IPEqUtils,Math,Types,ststrs;

constructor TIPEqCBrace.Create;
begin
  FType := CBRACE_CBRACE;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateCBraceL;
begin
  FType := CBRACE_CBRACEL;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateCBraceR;
begin
  FType := CBRACE_CBRACER;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateParen;
begin
  FType := CBRACE_PAREN;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateParenL;
begin
  FType := CBRACE_PARENL;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateParenR;
begin
  FType := CBRACE_PARENR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateBrace;
begin
  FType := CBRACE_BRACE;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateBraceL;
begin
  FType := CBRACE_BRACEL;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateBraceR;
begin
  FType := CBRACE_BRACER;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateVector;
begin
  FType := CBRACE_VECTOR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateVectorL;
begin
  FType := CBRACE_VECTORL;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateVectorR;
begin
  FType := CBRACE_VECTORR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateGrint;
begin
  FType := CBRACE_GRINT;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateDBraceL;
begin
  FType := CBRACE_DBRACEL;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateDBraceR;
begin
  FType := CBRACE_DBRACER;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateHBrace;
begin
  FType := CBRACE_HBRACE;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateHBraceT;
begin
  FType := CBRACE_HBRACET;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.CreateHBraceB;
begin
  FType := CBRACE_HBRACEB;
  Create(TIPEqRow.Create);
end;

constructor TIPEqCBrace.Create(ARow:TIPEqRow);
begin
  inherited Create;
  AddRow(ARow);
end;

procedure TIPEqCBrace.BuildMathML(Buffer:TStrings; Level:Integer);
var
  FStr : String;
begin
    if FType=CBRACE_CBRACE then
      FStr := 'cbrace'
    else if FType=CBRACE_CBRACEL then
      FStr := 'cbracel'
    else if FType=CBRACE_CBRACER then
      FStr := 'cbracer'
    else if FType=CBRACE_PAREN then
      FStr := 'paren'
    else if FType=CBRACE_PARENL then
      FStr := 'parenl'
    else if FType=CBRACE_PARENR then
      FStr := 'parenr'
    else if FType=CBRACE_BRACE then
      FStr := 'brace'
    else if FType=CBRACE_BRACEL then
      FStr := 'bracel'
    else if FType=CBRACE_BRACER then
      FStr := 'bracer'
    else if FType=CBRACE_VECTOR then
      FStr := 'vector'
    else if FType=CBRACE_VECTORL then
      FStr := 'vectorl'
    else if FType=CBRACE_VECTORR then
      FStr := 'vectorr'
    else if FType=CBRACE_GRINT then
      FStr := 'grint'
    else if FType=CBRACE_DBRACEL then
      FStr := 'dbracel'
    else if FType=CBRACE_DBRACER then
      FStr := 'dbracer'
    else if FType=CBRACE_HBRACE then
      FStr := 'hbrace'
    else if FType=CBRACE_HBRACET then
      FStr := 'hbracet'
    else if FType=CBRACE_HBRACEB then
      FStr := 'hbraceb';

    Buffer.Add(CharStrS(' ',Level)+'<m'+FStr+'>');
    Child[0].BuildMathML(Buffer,Level+1);
    Buffer.Add(CharStrS(' ',Level)+'</m'+FStr+'>');
end;

function TIPEqCBrace.GetName:String;
begin
  if FType=CBRACE_CBRACE then
    Result := 'CBRACE'
  else if FType=CBRACE_CBRACEL then
    Result := 'CBRACEL'
  else if FType=CBRACE_CBRACER then
    Result := 'CBRACER'
  else if FType=CBRACE_PAREN then
    Result := 'PAREN'
  else if FType=CBRACE_PARENL then
    Result := 'PARENL'
  else if FType=CBRACE_PARENR then
    Result := 'PARENR'
  else if FType=CBRACE_BRACE then
    Result := 'BRACE'
  else if FType=CBRACE_BRACEL then
    Result := 'BRACEL'
  else if FType=CBRACE_BRACER then
    Result := 'BRACER'
  else if FType=CBRACE_VECTOR then
    Result := 'VECTOR'
  else if FType=CBRACE_VECTORL then
    Result := 'VECTORL'
  else if FType=CBRACE_VECTORR then
    Result := 'VECTORR'
  else if FType=CBRACE_GRINT then
    Result := 'GRINT'
  else if FType=CBRACE_DBRACEL then
    Result := 'DBRACEL'
  else if FType=CBRACE_DBRACER then
    Result := 'DBRACER'
  else if FType=CBRACE_HBRACE then
    Result := 'HBRACE'
  else if FType=CBRACE_HBRACET then
    Result := 'HBRACET'
  else if FType=CBRACE_HBRACEB then
    Result := 'HBRACEB'
end;

function  TIPEqCBrace.Clone:TIPEqNode;
begin
  Result := TIPEqCBrace.Create;
  TIPEqCBrace(Result).CopyChildren(Self);
end;

function TIPEqCBrace.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  Fm : TTextMetric;
  MinAscent,MinDescent : Integer;
begin
  Node := Row[0];
  Em := GetEMWidth(Font);
  FParenWidth := Em div 3;
  FParenWidth := Max(FParenWidth,4);

  if (FType = CBRACE_GRINT) or (FType = CBRACE_DBRACEL) or (FType = CBRACE_DBRACER) then
    FParenWidth := Max(FParenWidth,6);

  Ascent := 0;
  Descent := 0;
  Width := 0;

  Fm := GetTextMetrics;

  MinAscent := Fm.tmAscent-Fm.tmInternalLeading;
  MinDescent := Fm.tmDescent;

  if FType=CBRACE_CBRACE then
    Width := 2*FParenWidth+2*GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_CBRACEL then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_CBRACER then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_PAREN then
    Width := 2*FParenWidth+2*GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_PARENL then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_PARENR then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_BRACE then
    Width := 2*FParenWidth+2*GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_BRACEL then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_BRACER then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_VECTOR then
    Width := 2*FParenWidth+2*GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_VECTORL then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_VECTORR then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_GRINT then
    Width := 2*FParenWidth+2*GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_DBRACEL then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_DBRACER then
    Width := FParenWidth+GetEmPart(CBRACE_GAP,Em)
  else if FType=CBRACE_HBRACE then
  begin
    Ascent := FParenWidth+GetEmPart(CBRACE_GAP,Em);
    Descent := FParenWidth+GetEmPart(CBRACE_GAP,Em);
    Width := 0;
  end
  else if FType=CBRACE_HBRACET then
  begin
    Ascent := FParenWidth+GetEmPart(CBRACE_GAP,Em);
    Descent := 0;
    Width := 0;
  end
  else if FType=CBRACE_HBRACEB then
  begin
    Ascent := 0;
    Descent := FParenWidth+GetEmPart(CBRACE_GAP,Em);
    Width := 0;
  end;


  if assigned(Node) then
  begin
    inc(Ascent,Max(MinAscent,Node.Ascent));
    inc(Descent,Max(MinDescent,Node.Descent));
    inc(Width,Node.Width);
  end;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqCBrace.Layout;
var
  Node : TIPEqNode;
  X,Y:Integer;
begin
  Node := Row[0];

  X := 0;
  if assigned(Node) then
  begin
  if FType=CBRACE_CBRACE then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_CBRACEL then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_CBRACER then
    X := 0
  else if FType=CBRACE_PAREN then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_PARENL then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_PARENR then
    X := 0
  else if FType=CBRACE_BRACE then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_BRACEL then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_BRACER then
    X := 0
  else if FType=CBRACE_VECTOR then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_VECTORL then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_VECTORR then
    X := 0
  else if FType=CBRACE_GRINT then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_DBRACEL then
    X := FParenWidth+GetEmPart(CBRACE_GAP)
  else if FType=CBRACE_DBRACER then
    X := 0
  else if FType=CBRACE_HBRACE then
  begin
    X := 0;
  end
  else if FType=CBRACE_HBRACET then
  begin
    X := 0;
  end
  else if FType=CBRACE_HBRACEB then
  begin
    X := 0;
  end;

  Y := Ascent-Node.Ascent;
  Node.SetLocation(X,Y);
  end;

end;

procedure TIPEqCBrace.Draw(ACanvas:TCanvas);
var
  H,W : Integer;
  XOff : Integer;
  P2 : Integer;
  Pts : array of TPoint;
begin
  H := Height-1;
  XOff := 0;
  P2 := Max(2,FParenWidth div 2);

  if (FType=CBRACE_CBRACE) OR (FType=CBRACE_CBRACEL) then
  begin
    SetLength(Pts,7);
    Pts[0].X := XOff+FParenWidth;
    Pts[0].Y := 0;
    Pts[1].X := XOff+FParenWidth-P2;
    Pts[1].Y := P2;
    Pts[2].X := Pts[1].X;
    Pts[2].Y := H div 2 -P2;
    Pts[3].X := XOff;
    Pts[3].Y := H div 2;
    Pts[4].X := Pts[1].X;
    Pts[4].Y := Pts[3].Y+P2;
    Pts[5].X := Pts[1].X;
    Pts[5].Y := H - P2;
    Pts[6].X := Pts[0].X;
    Pts[6].Y := H;
    ACanvas.PolyLine(Pts);
  end;

  if (FType=CBRACE_CBRACE) OR (FType=CBRACE_CBRACER) then
  begin
    SetLength(Pts,7);
    W := Width-FParenWidth-1;
    Pts[0].X := W+XOff;
    Pts[0].Y := 0;
    Pts[1].X := W+XOff+P2;
    Pts[1].Y := P2;
    Pts[2].X := W+XOff+P2;
    Pts[2].Y := H div 2 - P2;
    Pts[3].X := W+FParenWidth;
    Pts[3].Y := H div 2;
    Pts[4].X := W+XOff+P2;
    Pts[4].Y := H div 2+P2;
    Pts[5].X := W+XOff+P2;
    Pts[5].Y := H - P2;
    Pts[6].X := W+XOff;
    Pts[6].Y := H;
    ACanvas.PolyLine(Pts);
  end;

  if (FType=CBRACE_PAREN) OR (FType=CBRACE_PARENL) then
  begin
    ACanvas.Arc(XOff,0,2*(XOff+FParenWidth)+1,2*FParenWidth+1,XOff+FParenWidth,0,XOff,FParenWidth);
    ACanvas.Arc(XOff,H-2*FParenWidth,2*(XOff+FParenWidth)+1,H+1,XOff,H-FParenWidth,XOff+FParenWidth,H);
    ACanvas.MoveTo(XOff,FParenWidth-1);
    ACanvas.LineTo(XOff,H-FParenWidth);
    if Font.Size > CBRACE_THICKFONT then
    begin
      ACanvas.MoveTo(XOff+1,FParenWidth-1);
      ACanvas.LineTo(XOff+1,H-FParenWidth);
      ACanvas.Arc(XOff+1,0,2*(XOff+FParenWidth),2*FParenWidth+1,XOff+FParenWidth,0,XOff,FParenWidth);
      ACanvas.Arc(XOff+1,H-2*FParenWidth,2*(XOff+FParenWidth),H+1,XOff,H-FParenWidth,XOff+FParenWidth,H);
    end;
  end;

  if (FType=CBRACE_PAREN) OR (FType=CBRACE_PARENR) then
  begin
    W := Width-FParenWidth-1;
    ACanvas.Arc(W+XOff-FParenWidth,0,W+XOff+FParenWidth+1,2*FParenWidth+1,W+XOff+FParenWidth,FParenWidth,W+XOff,0);
    ACanvas.Arc(W+XOff-FParenWidth,H-2*FParenWidth,W+XOff+FParenWidth+1,H+1,W+XOff,h,W+XOff+FParenWidth,H-FParenWidth);
    ACanvas.MoveTo(W+XOff+FParenWidth,FParenWidth);
    ACanvas.LineTo(W+XOff+FParenWidth,H-FParenWidth+1);
    if Font.Size > CBRACE_THICKFONT then
    begin
      ACanvas.MoveTo(W+XOff+FParenWidth-1,FParenWidth);
      ACanvas.LineTo(W+XOff+FParenWidth-1,H-FParenWidth+1);
      ACanvas.Arc(W+XOff-FParenWidth+1,0,W+XOff+FParenWidth,2*FParenWidth+1,W+XOff+FParenWidth-1,FParenWidth,W+XOff,0);
      ACanvas.Arc(W+XOff-FParenWidth+1,H-2*FParenWidth,W+XOff+FParenWidth,H+1,W+XOff,h,W+XOff+FParenWidth-1,H-FParenWidth);
    end;
  end;

  if (FType=CBRACE_BRACE) OR (FType=CBRACE_BRACEL) then
  begin
    SetLength(Pts,4);
    Pts[0].X := XOff+FParenWidth;
    Pts[0].Y := 0;
    Pts[1].X := XOff;
    Pts[1].Y := 0;
    Pts[2].X := XOff;
    Pts[2].Y := H;
    Pts[3].X := XOff+FParenWidth+1;
    Pts[3].Y := H;
    ACanvas.PolyLine(Pts);
  end;

  if (FType=CBRACE_BRACE) OR (FType=CBRACE_BRACER) then
  begin
    SetLength(Pts,4);
    W := Width-FParenWidth-1;
    Pts[0].X := W+XOff;
    Pts[0].Y := 0;
    Pts[1].X := W+XOff+FParenWidth;
    Pts[1].Y := 0;
    Pts[2].X := W+XOff+FParenWidth;
    Pts[2].Y := H;
    Pts[3].X := W+XOff-1;
    Pts[3].Y := H;
    ACanvas.PolyLine(Pts);
  end;

  if (FType=CBRACE_VECTOR) OR (FType=CBRACE_VECTORL) then
  begin
    SetLength(Pts,3);
    Pts[0].X := XOff+FParenWidth;
    Pts[0].Y := 0;
    Pts[1].X := XOff;
    Pts[1].Y := H div 2;
    Pts[2].X := XOff+FParenWidth;
    Pts[2].Y := H;
    ACanvas.PolyLine(Pts);
  end;

  if (FType=CBRACE_VECTOR) OR (FType=CBRACE_VECTORR) then
  begin
    SetLength(Pts,3);
    W := Width-FParenWidth-1;
    Pts[0].X := W+XOff;
    Pts[0].Y := 0;
    Pts[1].X := W+XOff+FParenWidth;
    Pts[1].Y := H div 2;
    Pts[2].X := W+XOff;
    Pts[2].Y := H;
    ACanvas.PolyLine(Pts);
  end;

  if (FType=CBRACE_GRINT) OR (FType=CBRACE_DBRACEL) then
  begin
    SetLength(Pts,4);
    Pts[0].X := XOff+FParenWidth;
    Pts[0].Y := 0;
    Pts[1].X := XOff;
    Pts[1].Y := 0;
    Pts[2].X := XOff;
    Pts[2].Y := H;
    Pts[3].X := XOff+FParenWidth+1;
    Pts[3].Y := H;
    ACanvas.PolyLine(Pts);
    ACanvas.MoveTo(XOff+FParenWidth div 2,0);
    ACanvas.LineTo(XOff+FParenWidth div 2,H);
  end;

  if (FType=CBRACE_GRINT) OR (FType=CBRACE_DBRACER) then
  begin
    SetLength(Pts,4);
    W := Width-FParenWidth-1;
    Pts[0].X := W+XOff;
    Pts[0].Y := 0;
    Pts[1].X := W+XOff+FParenWidth;
    Pts[1].Y := 0;
    Pts[2].X := W+XOff+FParenWidth;
    Pts[2].Y := H;
    Pts[3].X := W+XOff-1;
    Pts[3].Y := H;
    ACanvas.PolyLine(Pts);
    ACanvas.MoveTo(W+XOff+FParenWidth-(FParenWidth div 2),0);
    ACanvas.LineTo(W+XOff+FParenWidth-(FParenWidth div 2),H);
  end;

  if (FType=CBRACE_HBRACE) OR (FType=CBRACE_HBRACET) then
  begin
    SetLength(Pts,4);
    Pts[0].X := 0;
    Pts[0].Y := FParenWidth;
    Pts[1].X := 0;
    Pts[1].Y := 0;
    Pts[2].X := Width-1;
    Pts[2].Y := 0;
    Pts[3].X := Width-1;
    Pts[3].Y := FParenWidth+1;
    ACanvas.PolyLine(Pts);
  end;

  if (FType=CBRACE_HBRACE) OR (FType=CBRACE_HBRACEB) then
  begin
    SetLength(Pts,4);
    Pts[0].X := 0;
    Pts[0].Y := H-FParenWidth;
    Pts[1].X := 0;
    Pts[1].Y := H;
    Pts[2].X := Width-1;
    Pts[2].Y := H;
    Pts[3].X := Width-1;
    Pts[3].Y := H-FParenWidth-1;
    ACanvas.PolyLine(Pts);
  end;

end;


end.



