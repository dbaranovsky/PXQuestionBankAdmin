unit IPEqSqrt;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqSqrt = class(TIPEqComposite)
  private
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor Create(ARow:TIPEqRow); overload;
    constructor CreateN; overload;
    constructor CreateN(ARadicand,ARoot:TIPEqRow); overload;
    function GetNodeAt(X,Y:Integer):TIPEqNode; override;
    function GetLastRow:TIPEqRow; override;
    function GetFirstRow:TIPEqRow; override;
    procedure GotoNextRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent); override;
    procedure GotoPrevRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent); override;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;

end;

const
  SQRT_INTERNALRIGHTMARGIN = 25;
  SQRT_EXTERNALRIGHTMARGIN = 10;
  SQRT_INTERNALTOPMARGIN = 25;
  SQRT_EXTERNALLEFTROOTMARGIN = 20;

implementation


uses IPEqUtils,Math,Types,ststrL;

constructor TIPEqSqrt.Create;
begin
  Create(TIPEqRow.Create);
end;

constructor TIPEqSqrt.Create(ARow:TIPEqRow);
begin
  inherited Create;
  AddRow(ARow);
end;

constructor TIPEqSqrt.CreateN;
begin
  CreateN(TIPEqRow.Create,TIPEqRow.Create);
end;

constructor TIPEqSqrt.CreateN(ARadicand,ARoot:TIPEqRow);
begin
  inherited Create;
  AddRow(ARadicand);
  AddRow(ARoot);
end;

procedure TIPEqSqrt.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
   if ChildCount = 1 then
   begin
     Buffer.Add(CharStrL(' ',Level)+'<msqrt>');
     Child[0].BuildMathML(Buffer,Level+1);
     Buffer.Add(CharStrL(' ',Level)+'</msqrt>');
   end
   else
   begin
     Buffer.Add(CharStrL(' ',Level)+'<mroot>');
     for I := 0 to ChildCount-1 do
       Child[I].BuildMathML(Buffer,Level+1);
     Buffer.Add(CharStrL(' ',Level)+'</mroot>');
   end;
end;

function TIPEqSqrt.GetName:String;
begin
  Result := 'RT';
end;

function  TIPEqSqrt.Clone:TIPEqNode;
begin
  Result := TIPEqSqrt.Create;
  TIPEqSqrt(Result).CopyChildren(Self);
end;

function TIPEqSqrt.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  H : Integer;
begin
  Node := Row[0];
  Em := getEMWidth(Font);
  Descent := 0;
  Ascent := 1+ GetEmPart(SQRT_INTERNALTOPMARGIN,Em);
  //Width include an em for stuff on left and an em for internal and external margin on right
  Width := Em + GetEmPart(SQRT_INTERNALRIGHTMARGIN,Em) + GetEmPart(SQRT_EXTERNALRIGHTMARGIN,Em);

  if assigned(Node) then
  begin
    Inc(Ascent,Node.Ascent);
    Inc(Descent,Node.Descent);
    Inc(Width,Node.Width);
  end;

  //Handle option root
  if ChildCount > 1 then
  begin
    Node := Row[1];
    Node.ReduceFontSize;
    Inc(Width,Node.Width-(Em div 2)+GetEMPart(SQRT_EXTERNALLEFTROOTMARGIN,Em));
    H := Node.Height+Em-Descent;
    Ascent := Max(Ascent,H);
  end;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqSqrt.Layout;
var
  Node : TIPEqNode;
  X,Y:Integer;
begin
  if ChildCOunt > 1 then
  begin
    Node := Row[1];
    X := GetEMPArt(SQRT_EXTERNALLEFTROOTMARGIN);
    Y := Height-Em-Node.Height;
    Node.SetLocation(X,Y);
  end;

  Node := Row[0];
  if assigned(Node) then
  begin
    X := Width-Node.Width-GetEmPart(SQRT_INTERNALRIGHTMARGIN+SQRT_EXTERNALRIGHTMARGIN);
    Y := Height-Node.Height;
    Node.SetLocation(X,Y);
  end;
  
end;

procedure TIPEqSqrt.Draw(ACanvas:TCanvas);
var
  H : Integer;
  E : Integer;
  Pts : Array[0..4] of TPoint;
  XOff,YOff : Integer;
begin
  XOff := Width-Row[0].Width-GetEMPart(SQRT_INTERNALRIGHTMARGIN+SQRT_EXTERNALRIGHTMARGIN+100);
  YOff := Height-Row[0].Height-GetEMPart(SQRT_INTERNALTOPMARGIN)-1;
  E := GetEmPart(100);
  H  := Height;
  Pts[0].X := XOff+E div 8;
  Pts[0].Y := H-E+ 3*E div 8 - E div 8;
  Pts[1].X := XOff+(E div 4);
  Pts[1].Y := H-E+(E div 8);
  Pts[2].X := XOff+(E div 2);
  Pts[2].Y := H;
  Pts[3].X := XOff+(E - E div 4);
  Pts[3].Y := YOff;
  Pts[4].X := Width-GetEmPart(SQRT_EXTERNALRIGHTMARGIN);
  Pts[4].Y := YOff;
  ACanvas.Polyline(Pts);

  //ACanvas.Rectangle(Bounds(0,0,Width+1,Height+1));

end;

{Add code to handle clicking to right of row inside sqrt}
function TIPEqSqrt.GetNodeAt(X,Y:Integer):TIPEqNode;
begin
  //Before calling inherited check to see if cursor is
  //in leftmost area, then just return self
  if ChildCount = 1 then
  begin
    if Contains(x-XLoc,y-Yloc) and ((x-XLoc) < getEmPart(50)) then
    begin
      Result := Self;
      Exit;
    end;
  end
  else
  begin
    if Contains(x-XLoc,y-Yloc) and ((x-XLoc) < Row[1].XLoc) then
    begin
      Result := Self;
      Exit;
    end;
  end;

  Result := inherited GetNodeAt(X,Y);

end;

function TIPEqSqrt.GetLastRow:TIPEqRow;
begin
  Result := Row[0];
end;

function TIPEqSqrt.GetFirstRow:TIPEqRow;
begin
  Result := TIPEqRow(LastChild);
end;

procedure TIPEqSqrt.GotoNextRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent);
begin
  //Reverse rows for NRoot
  if (ChildCount > 1) and (Node = Row[1]) then
  begin
    CaretEvent.Row := Row[0];
    CaretEvent.Position := 0;
    Exit;
  end
  else if (ChildCount > 1) and (Node = Row[0]) then
    Node := Row[1];
  Inherited GotoNExtRow(Node,CaretEvent);
end;

procedure TIPEqSqrt.GotoPrevRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent);
begin
  //Reverse rows for NRoot
  if (ChildCount > 1) and (Node = Row[0]) then
  begin
    CaretEvent.Row := Row[1];
    CaretEvent.Position := 0;
    Exit;
  end
  else if (ChildCount > 1) and (Node = Row[1]) then
    Node := Row[0];
  Inherited GotoPrevRow(Node,CaretEvent);
end;


end.



