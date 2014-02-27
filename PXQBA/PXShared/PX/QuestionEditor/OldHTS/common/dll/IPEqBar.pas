unit IPEqBar;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes,Sysutils;

type TBarKind=(
  btBAR_OVERBAR,
  btBAR_UNDRBAR,
  btBAR_OVDBBAR,
  btBAR_UNDBBAR,
  btBAR_ARROWL,
  btBAR_ARROWR,
  btBAR_ARROWDB,
  btBAR_RAYL,
  btBAR_RAYR,
  btBAR_RAYDB,
  btBAR_OVERBRC,
  btBAR_UNDRBRC,
  btBAR_ARC,
  btBAR_SLASH,
  btBAR_HAT,
  btBAR_TILDE,
  btBAR_ACCENT,
  btBAR_UMLAUT,
  btBAR_PRIME,
  btBAR_PRIME2,
  btBAR_PRIME3,
  btBAR_REP,
  btBAR_HLINE,
  btBAR_DHLINE);

const STRBARKIND:Array [TBarKind] of string = (
  'overbar',
  'undrbar',
  'ovdbbar',
  'undbbar',
  'arrowl',
  'arrowr',
  'arrowdb',
  'rayl',
  'rayr',
  'raydb',
  'overbrc',
  'undrbrc',
  'arc',
  'slash',
  'hat',
  'tilde',
  'accent',
  'umlaut',
  'prime',
  'prime2',
  'prime3',
  'rep',
  'hline',
  'dhline');

type

TIPEqBAR = class(TIPEqComposite)
  private
    FType : TBarKind;//teger;
    FTildeDeltaHeight : integer;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor CreateUN; overload;
    constructor CreateOVDB; overload;
    constructor CreateUNDB; overload;
    constructor CreateArrowL; overload;
    constructor CreateArrowR; overload;
    constructor CreateArrowDB; overload;
    constructor CreateRayL; overload;
    constructor CreateRayR; overload;
    constructor CreateRayDB; overload;
    constructor CreateOverbrc; overload;
    constructor CreateUndrbrc; overload;
    constructor CreateArc; overload;
    constructor CreateSlash; overload;
    constructor CreateHat; overload;
    constructor CreateTilde; overload;
    constructor CreateAccent; overload;
    constructor CreateUmlaut; overload;
    constructor CreatePrime; overload;
    constructor CreatePrime2; overload;
    constructor CreatePrime3; overload;
    constructor CreateRep; overload;
    constructor CreateHLine; overload;
    constructor CreateDHLine; overload;
    constructor Create(ARow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;

    property BarType:TBarKind read FType write FType;
end;

const
  BAR_INTERNALMARGIN = 6;//10;  //C2767 DV 2/18/05 Changes to improve look of arrows/rays
  BAR_ArrowWIDTH = 25;//12;//20
  BAR_ARROWLINE = 2;
  BAR_ARROWMINWIDTH = 4;
  BAR_PrimeWIDTH = 20;
  BAR_PRIMEMINWIDTH = 4;
  BAR_TILDE_RATIO:double = 0.35;
  

{ BAR_OVERBAR = 1;
  BAR_UNDRBAR = 2;
  BAR_OVDBBAR = 3;
  BAR_UNDBBAR = 4;
  BAR_ARROWL  = 5;
  BAR_ARROWR  = 6;
  BAR_ARROWDB = 7;
  BAR_RAYL    = 8;
  BAR_RAYR    = 9;
  BAR_RAYDB   = 10;
  BAR_OVERBRC   = 11;
  BAR_UNDRBRC   = 12;
  BAR_ARC       = 13;
  BAR_SLASH     = 14;
  BAR_HAT       = 15;
  BAR_TILDE     = 16;
  BAR_ACCENT    = 17;
  BAR_UMLAUT    = 18;
  BAR_PRIME     = 19;
  BAR_PRIME2    = 20;
  BAR_PRIME3    = 21;
  BAR_REP       = 22;
  BAR_HLINE     = 23;
  BAR_DHLINE    = 24;
}
implementation


uses IPEqUtils,Math,Types,ststrL;

constructor TIPEqBAR.Create;
begin
  FType := btBAR_OVERBAR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateUN;
begin
  FType := btBAR_UNDRBAR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateOVDB;
begin
  FType := btBAR_OVDBBAR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateUNDB;
begin
  FType := btBAR_UNDBBAR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateArrowL;
begin
  FType := btBAR_ARROWL;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateArrowR;
begin
  FType := btBAR_ARROWR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateArrowDB;
begin
  FType := btBAR_ARROWDB;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateRayL;
begin
  FType := btBAR_RAYL;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateRayR;
begin
  FType := btBAR_RAYR;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateRayDB;
begin
  FType := btBAR_RAYDB;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateOverbrc;
begin
  FType := btBAR_OVERBRC;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateUndrbrc;
begin
  FType := btBAR_UNDRBRC;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateArc;
begin
  FType := btBAR_ARC;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateSlash;
begin
  FType := btBAR_Slash;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateHAT;
begin
  FType := btBAR_HAT;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateTilde;
begin
  FType := btBAR_TILDE;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateAccent;
begin
  FType := btBAR_ACCENT;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateUmlaut;
begin
  FType := btBAR_UMLAUT;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreatePrime;
begin
  FType := btBAR_PRIME;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreatePrime2;
begin
  FType := btBAR_PRIME2;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreatePrime3;
begin
  FType := btBAR_PRIME3;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateRep;
begin
  FType := btBAR_REP;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateHLine;
begin
  FType := btBAR_HLINE;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.CreateDHLine;
begin
  FType := btBAR_DHLINE;
  Create(TIPEqRow.Create);
end;

constructor TIPEqBAR.Create(ARow:TIPEqRow);
begin
  inherited Create;
  AddRow(ARow);
end;

procedure TIPEqBAR.BuildMathML(Buffer:TStrings; Level:Integer);
var
  FStr : String;
begin
  FStr := strBarKind[Ftype];

  Buffer.Add(CharStrL(' ',Level)+'<m'+FStr+'>');
  Child[0].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrL(' ',Level)+'</m'+FStr+'>');
end;

function TIPEqBAR.GetName:String;
begin
  Result := UpperCase(strBarKind[fType]);
end;

function  TIPEqBAR.Clone:TIPEqNode;
begin
  Result := TIPEqBAR.Create;
  TIPEqBAR(Result).CopyChildren(Self);
end;

function TIPEqBAR.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  MinWidth : Integer;
  ArrowMinWidth : Integer;
begin
  Node := Row[0];
  Em := getEMWidth(Font);

  Ascent := 0;
  Descent := 0;
  Width := 0;
  MinWidth := 0;

  ArrowMinWidth := Max(GetEmPart(BAR_ArrowWIDTH,Em),BAR_ArrowMinWidth)*3+3;

  case fType of
   btBAR_OVERBAR,btBAR_REP:
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+1;
   btBAR_UNDRBAR:
    Descent := GetEmPart(BAR_INTERNALMARGIN,Em)+1;
   btBAR_OVDBBAR:
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+4;
   btBAR_UNDBBAR:
    Descent := GetEmPart(BAR_INTERNALMARGIN,Em)+4;
   btBAR_ARROWL,btBAR_ARROWR,btBAR_ARROWDB:
   begin
    Descent := GetEmPart(BAR_INTERNALMARGIN,Em)+Max(GetEmPart(BAR_ArrowWIDTH,Em),BAR_ArrowMinWidth)+2;
    MinWidth := ArrowMinWidth;
    if ftype = btBAR_ARROWDB then
      Inc(MinWidth,3);
   end;
   btBAR_RAYL,btBAR_RAYR,btBAR_RAYDB:
   begin
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+Max(GetEmPart(BAR_ArrowWIDTH,Em),BAR_ArrowMinWidth);
    MinWidth := ArrowMinWidth;
    if ftype = btBAR_RAYDB then
      Inc(MinWidth,3);
   end;
   btBAR_OVERBRC:
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+10;
   btBAR_UNDRBRC:
    Descent := GetEmPart(BAR_INTERNALMARGIN,Em)+10;
   btBAR_ARC:
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+Max(GetEmPart(BAR_PrimeWIDTH,Em),BAR_PRIMEMINWIDTH);
   btBAR_HAT:
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+Max(GetEmPart(BAR_PrimeWIDTH,Em),BAR_PRIMEMINWIDTH);
   btBAR_TILDE:
   begin
    Ascent := GetTextSize(Font,'~').cy;
    FTildeDeltaHeight := Round(Ascent*BAR_TILDE_RATIO);
    Ascent := Ascent - 2*FTildeDeltaHeight;
   end;
   btBAR_ACCENT:
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+Max(GetEmPart(BAR_PrimeWIDTH,Em),BAR_PRIMEMINWIDTH);
   btBAR_UMLAUT:
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+2;
   btBAR_PRIME:
    begin
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+Max(GetEmPart(BAR_PrimeWIDTH,Em),BAR_PRIMEMINWIDTH);
    Width :=  Max(GetEmPart(BAR_PrimeWIDTH,Em),BAR_PRIMEMINWIDTH) div 2;
    end;
   btBAR_PRIME2:
    begin
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+Max(GetEmPart(BAR_PrimeWIDTH,Em),BAR_PRIMEMINWIDTH);
    Width :=  Max(GetEmPart(BAR_PrimeWIDTH,Em),BAR_PRIMEMINWIDTH);
    end;
   btBAR_PRIME3:
    begin
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+GetEmPart(BAR_PrimeWIDTH,Em);
    Width :=  3*(Max(GetEmPart(BAR_PrimeWIDTH,Em),BAR_PRIMEMINWIDTH) div 2);
    end;
   btBAR_HLINE:
    begin;
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+1;
    Descent := GetEmPart(BAR_INTERNALMARGIN,Em)+1;
    end;
   btBAR_DHLINE:
    begin
    Ascent := GetEmPart(BAR_INTERNALMARGIN,Em)+4;
    Descent := GetEmPart(BAR_INTERNALMARGIN,Em)+4;
    end;
  end;

  if assigned(Node) then
  begin
    inc(Ascent,Node.Ascent);
    inc(Descent,Node.Descent);
    inc(Width,Node.Width);
    Width := Max(Width,MinWidth);
  end;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqBAR.Layout;
var
  Node : TIPEqNode;
  X,Y:Integer;
begin
    Node := Row[0];
    X :=(Width-Node.Width) div 2;
    Y := Ascent-Node.Ascent;
    Node.SetLocation(X,Y);
end;

procedure TIPEqBAR.Draw(ACanvas:TCanvas);
var
  X1,Y1 : Integer;
  X2,Y2 : Integer;
  X3,Y3 : Integer;
  left:integer;
  pts : Array of TPoint;
  ArrowW, ArrowH : Integer; //, arrowL : Integer;
  PrimeW, PrimeH : Integer;
  OldColor : TColor;
  BR:TBrushRecall;
  D : Integer;
  ProportionalPenWidth,PPW:integer;
  W,H,R : Integer;
  procedure SetPen; begin ACanvas.Pen.Width:=ProportionalPenWidth end;
  Procedure ResetPen; begin ACanvas.Pen.Width:=1 end;
begin

  ArrowW := Max(GetEmPart(BAR_ArrowWIDTH),BAR_ArrowMinWidth) div 2;//2  DV:Arrows/rays change C2767 - 2/18/05;
  ArrowH := Max(GetEmPart(BAR_ArrowWIDTH),BAR_ArrowMinWidth)*3 div 2;
  PrimeW := Max(GetEmPart(BAR_PrimeWIDTH),BAR_PRIMEMINWIDTH) div 2;
  PrimeH := Max(GetEmPart(BAR_PrimeWIDTH),BAR_PRIMEMINWIDTH)*3 div 2;
  //arrowL := GetEmPart(BAR_ArrowWIDTH) div 5;  //not used for right arrows

  ProportionalPenWidth:=GetEMPART(BAR_ARROWLINE);
  PPW:=ProportionalPenWidth;

  if Assigned(Row[0]) and (Row[0].childCount>0) and Assigned(Row[0].Child[0]) then
    Left := Row[0].Child[0].LeftOffset
  else
    Left := 0;

  case fType of
  btBAR_OVERBAR,btBAR_REP:
  begin
    X1 := Left-1;
    Y1 := 0;
    X2 := Width;
    Y2 := 0;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_UNDRBAR:
  begin
    X1 := 0;
    Y1 := Height-1;
    X2 := Width;
    Y2 := Height-1;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_OVDBBAR:
  begin
    X1 := 0;
    Y1 := 0;
    X2 := Width;
    Y2 := 0;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
    Y1 := 3;
    Y2 := 3;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_UNDBBAR:
  begin
    X1 := 0;
    Y1 := Height-1;
    X2 := Width;
    Y2 := Height-1;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
    Y1 := Height-4;
    Y2 := Height-4;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_ARROWL,btBAR_ARROWR,btBAR_ARROWDB:
  begin
    X1 := 1;
    Y1 := HEIGHT-ArrowW-1;
    X2 := Width-1;
    Y2 := HEIGHT-ArrowW-1;

    SetPen;
    case fType of //better tips of arrows
    btBAR_ARROWL:
    begin
      ACanvas.MoveTo(X1+PPW,Y1);
      ACanvas.LineTo(X2,Y2);
    end;
    btBAR_ARROWR:
    begin
      ACanvas.MoveTo(X1,Y1);
      ACanvas.LineTo(X2-PPW,Y2);
    end;
    btBAR_ARROWDB:
    begin
      ACanvas.MoveTo(X1+PPW,Y1);
      ACanvas.LineTo(X2-PPW,Y2);
    end;
    end;
    ResetPen;

    if FType<>btBAR_ARROWR then
    begin
      SetLength(pts,3);
      Pts[0].X := X1;
      Pts[0].Y := Y1;
      Pts[1].X := X1+ArrowH;
      Pts[1].Y := Y1-ArrowW;
      //Pts[2].X := X1+arrowL;
      //Pts[2].Y := Y1;
      Pts[2].X := X1+ArrowH;
      Pts[2].Y := Y1+ArrowW;
      //Pts[4].X := X1;
      //Pts[4].Y := Y1;
      //oldBrColor := ACanvas.Brush.Color; //This type of handling is not always working
      //ACanvas.Brush.Color := clBlack;    //Brush colos does not get reset for next draw
      //ACanvas.Polygon(pts);              //Use of TBrushRecall(below) solves it
      //ACanvas.Brush.Color := oldBrColor;
      br:=TBrushRecall.Create(aCanvas.Brush);
      ACanvas.Brush.Color := ACanvas.Font.Color;
      ACanvas.Polygon(pts);
      br.Free;
    end;
    if FType<>btBAR_ARROWL then
    begin
      SetLength(pts,3);
      Pts[0].X := X2;
      Pts[0].Y := Y2;
      Pts[1].X := X2-ArrowH;
      Pts[1].Y := Y2-ArrowW;
      //Pts[2].X := X2-arrowL;
      //Pts[2].Y := Y2;
      Pts[2].X := X2-ArrowH;
      Pts[2].Y := Y2+ArrowW;
      //Pts[4].X := X2;
      //Pts[4].Y := Y2;
      br:=TBrushRecall.Create(aCanvas.Brush);
      ACanvas.Brush.Color := ACanvas.Font.Color;
      ACanvas.Polygon(pts);
      br.Free;
    end;
  end;
  btBAR_RAYL,btBAR_RAYR,btBAR_RAYDB:
  begin
    X1 := 1;
    Y1 := ArrowW;
    X2 := Width-1;
    Y2 := ArrowW;
    SetPen;
    case fType of //better tips of arrows
    btBAR_RAYL:
    begin
      ACanvas.MoveTo(X1+PPW,Y1);
      ACanvas.LineTo(X2,Y2);
    end;
    btBAR_RAYR:
    begin
      ACanvas.MoveTo(X1,Y1);
      ACanvas.LineTo(X2-PPW,Y2);
    end;
    btBAR_RAYDB:
    begin
      ACanvas.MoveTo(X1+PPW,Y1);
      ACanvas.LineTo(X2-PPW,Y2);
    end;
    end;
    ResetPen;
    if FType<>btBAR_RAYR then
    begin
      SetLength(pts,3);
      Pts[0].X := X1;
      Pts[0].Y := Y1;
      Pts[1].X := X1+ArrowH;
      Pts[1].Y := Y1-ArrowW;
      //Pts[2].X := X1+arrowL;
      //Pts[2].Y := Y1;
      Pts[2].X := X1+ArrowH;
      Pts[2].Y := Y1+ArrowW;
      //Pts[4].X := X1;
      //Pts[4].Y := Y1;
      br:=TBrushRecall.Create(aCanvas.Brush);
      ACanvas.Brush.Color := ACanvas.Font.Color;
      ACanvas.Polygon(pts);
      br.Free;
    end;
    if FType<>btBAR_RAYL then
    begin
      SetLength(pts,3);
      Pts[0].X := X2;
      Pts[0].Y := Y2;
      Pts[1].X := X2-ArrowH;
      Pts[1].Y := Y2-ArrowW;
      //Pts[2].X := X2-arrowL;
      //Pts[2].Y := Y2;
      Pts[2].X := X2-ArrowH;
      Pts[2].Y := Y2+ArrowW;
      //Pts[4].X := X2;
      //Pts[4].Y := Y2;
      br:=TBrushRecall.Create(aCanvas.Brush);
      ACanvas.Brush.Color := ACanvas.Font.Color;
      ACanvas.Polygon(pts);
      br.Free;
    end;
  end;
  btBAR_SLASH:
  begin
    X1 := 0;
    Y1 := HEIGHT-1;
    X2 := Width-1;
    Y2 := 0;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_UMLAUT:
  begin
    X1 := Width div 2-2;
    Y1 := 1;
    X2 := Width div 2+2;
    Y2 := 1;
    OldColor := ACanvas.Brush.Color;
    ACanvas.Brush.Color := ACanvas.Font.Color;
    ACanvas.FillRect(Rect(X1-1,Y1-1,X1+1,Y1+1));
    ACanvas.FillRect(Rect(X2-1,Y2-1,X2+1,Y2+1));
    ACanvas.Brush.Color := OldColor;
  end;
  btBAR_HAT:
  begin
    Left := Round(0.7*Left);
    W := Width - Left;
    D := W div 2;
    X1 := (W div 2) - D + 1;
    Y1 := PrimeH-2;
    X2 := (W div 2) + D;
    Y2 := PrimeH-1;
    X3 := W div 2;
    Y3 := 0;
    ACanvas.Polyline([Point(X1+Left,Y1),Point(X3+Left,Y3),Point(X2+Left,Y2)]);
  end;
  btBAR_ARC:
  begin
    X1 := 0;
    Y1 := PrimeH;
    X2 := Width-1;
    Y2 := PrimeH;

    W := Width div 2;
    H := PrimeH;
    R := (W*W+H*H) div (2*H);

    ACanvas.Arc(W-R,0,W+R,2*R,X2,Y2,X1,Y1);
  end;
  btBAR_TILDE:
  begin
    W := Width-Left;
    X1 := W div 2- ACanvas.TextWidth('~') div 2 + Left;
    Y1 := -FTildeDeltaHeight;

    //OldColor := ACanvas.Brush.Color;
    Br := nil;
    try
      Br := TBrushRecall.Create(ACanvas.Brush);
      ACanvas.Brush.Style := bsClear;
      ACanvas.TextOut(X1,Y1,'~');
    finally
      Br.Free;
    end;
    //ACanvas.Brush.Color := OldColor;
  end;
  btBAR_OVERBRC:
  begin
    SetLength(Pts,15);
    Pts[0].X := 0;
    Pts[0].Y := 10;
    Pts[1].X := 0;
    Pts[1].Y := 7;
    Pts[2].X := 1;
    Pts[2].Y := 6;
    Pts[3].X := 1;
    Pts[3].Y := 5;
    Pts[4].X := 2;
    Pts[4].Y := 4;
    Pts[5].X := Width div 2-2;
    Pts[5].Y := 4;
    Pts[6].X := Width div 2-1;
    Pts[6].Y := 3;
    Pts[7].X := Width div 2;
    Pts[7].Y := 0;
    Pts[8].X := Width div 2+1;
    Pts[8].Y := 3;
    Pts[9].X := Width div 2 + 2;
    Pts[9].Y := 4;
    Pts[10].X := Width-3;
    Pts[10].Y := 4;
    Pts[11].X := Width-2;
    Pts[11].Y := 5;
    Pts[12].X := Width-2;
    Pts[12].Y := 6;
    Pts[13].X := Width-1;
    Pts[13].Y := 7;
    Pts[14].X := Width-1;
    Pts[14].Y := 10;

    ACanvas.Polyline(pts);
  end;
  btBAR_UNDRBRC:
  begin
    SetLength(Pts,15);
    Pts[0].X := 0;
    Pts[0].Y := Height-10;
    Pts[1].X := 0;
    Pts[1].Y := Height-8;
    Pts[2].X := 1;
    Pts[2].Y := Height-7;
    Pts[3].X := 1;
    Pts[3].Y := Height-6;
    Pts[4].X := 2;
    Pts[4].Y := Height-5;
    Pts[5].X := Width div 2-2;
    Pts[5].Y := Height-5;
    Pts[6].X := Width div 2-1;
    Pts[6].Y := Height-4;
    Pts[7].X := Width div 2;
    Pts[7].Y := Height-1;
    Pts[8].X := Width div 2+1;
    Pts[8].Y := Height-4;
    Pts[9].X := Width div 2 + 2;
    Pts[9].Y := Height-5;
    Pts[10].X := Width-3;
    Pts[10].Y := Height-5;
    Pts[11].X := Width-2;
    Pts[11].Y := Height-6;
    Pts[12].X := Width-2;
    Pts[12].Y := Height-7;
    Pts[13].X := Width-1;
    Pts[13].Y := Height-8;
    Pts[14].X := Width-1;
    Pts[14].Y := Height-11;

    ACanvas.Polyline(pts);
  end;
  btBAR_ACCENT:
  begin
    X1 := (Width+PrimeW) div 2;
    Y1 := 0;
    X2 := (Width-PrimeW) div 2;
    Y2 := PrimeH;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_PRIME:
  begin
    X1 := Width-1;
    Y1 := 0;
    X2 := Width-PrimeW-1;
    Y2 := PrimeH;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_PRIME2:
  begin
    X1 := Width-1;
    Y1 := 0;
    X2 := Width-PrimeW-1;
    Y2 := PrimeH;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
    X1 := Width-PrimeW-1;
    Y1 := 0;
    X2 := Width-2*PrimeW-1;
    Y2 := PrimeH;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_PRIME3:
  begin
    X1 := Width-1;
    Y1 := 0;
    X2 := Width-PrimeW-1;
    Y2 := PrimeH;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
    X1 := Width-PrimeW-1;
    Y1 := 0;
    X2 := Width-2*PrimeW-1;
    Y2 := PrimeH;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
    X1 := Width-2*PrimeW-1;
    Y1 := 0;
    X2 := Width-3*PrimeW-1;
    Y2 := PrimeH;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_HLINE:
  begin
    X1 := 0;
    Y1 := Height-1;
    X2 := Width-1;
    Y2 := Height-1;
    ACanvas.MoveTo(X1,0);
    ACanvas.LineTo(X2,0);
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end;
  btBAR_DHLINE:
  begin
    X1 := 0;
    Y1 := 0;
    X2 := Width-1;
    Y2 := 0;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
    Y1 := 3;
    Y2 := 3;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);

    X1 := 0;
    Y1 := Height-1;
    X2 := Width-1;
    Y2 := Height-1;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
    Y1 := Height-4;
    Y2 := Height-4;
    ACanvas.MoveTo(X1,Y1);
    ACanvas.LineTo(X2,Y2);
  end
  end;

end;


end.



