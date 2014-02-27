unit IPEqComposite;

interface

uses IPEqNode;

type

TIPEqComposite = class (TIPEqList)
  private
    function GetRow(Index:Integer):TIPEqRow;
  protected
    procedure AddNodes(Nodes:TIPEqNodeList); override;
    function  GetName:String; virtual; abstract;
  public
    function GetLastRow:TIPEqRow; override;
    function GetFirstRow:TIPEqRow; override;
    function AddRow(Row:TIPEqRow):TIPEqRow;
    function DeleteRow(ARow:TIPEqRow; CaretEvent:TIPEqCaretEvent):TIPEqNode;
    procedure InitCaret(var FocusedRow: TIPEqRow; var CaretPos:Integer); override;
    procedure DeleteCharacter(CaretEvent:TIPEqCaretEvent); override;
    function GetNodeAt(X,Y:Integer):TIPEqNode; override;
    procedure GotoNextRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent); override;
    procedure GotoPrevRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent); override;
    function GetText:String; override;

    property Row[Index:Integer]:TIPEqRow read GetRow;
end;

implementation

function TIPEqComposite.GetRow(Index:Integer):TIPEqRow;
begin
  Result := TIPEqRow(Child[Index]);
end;

function TIPEqComposite.AddRow(row:TIPEqRow):TIPEqRow;
begin
  Result := TIPEqRow(AddChild(row));
end;

function TIPEqComposite.GetLastRow:TIPEqRow;
var
  Node : TIPEqNode;
begin
  Node := LastChild;
  if Assigned(Node) then
    Result := LastChild.GetLastRow
  else
    Result := nil;
end;

function TIPEqComposite.GetFirstRow:TIPEqRow;
begin
  if ChildCount > 0 then
    Result := Child[0].GetFirstRow
  else
    Result := nil;
end;

function TIPEqComposite.DeleteRow(Arow:TIPEqRow; CaretEvent:TIPEqCaretEvent):TIPEqNode;
var
  Pos : Integer;
  Index : Integer;
begin

  if not assigned(Parent) then
  begin
    Result := nil;
    Exit;
  end;

  if Parent.InheritsFrom(TIPEqRow) then
  begin
    Index := TIPEqRow(Parent).GetChildIndex(self);
    Pos := TIPEqRow(Parent).FocusedPosition[Index];
    RemoveChild(Arow).Free;

    if ChildCount = 0 then
    begin
      CaretEvent.Row := TIPEqRow(Parent);
      CaretEvent.Position := Pos;
      //Pass back self so caller can free me.
      Result := Parent.RemoveChild(self);
      Exit;
    end;
    Parent.AddChildrenAfter(Row[0].GetNodeList,Index);

  end;

  Result := nil;

end;

{By default set the first row of a composited to the focus}
procedure TIPEqComposite.InitCaret(var FocusedRow: TIPEqRow; var CaretPos:Integer);
var
  R : TIPEqRow;
  I : integer;
begin
  if ChildCount > 0 then
  begin
    R := nil;
    //Select first mpty row
    for I := 0 to ChildCount-1 do
    begin
      if Row[I].ChildCount = 0 then
      begin
        R := Row[I];
        break;
      end;
    end;

    if not assigned(R) then
      R := Row[0];
    FocusedRow := R;
    if R.ChildCount > 0 then
      CaretPos := R.GetLastCaretPosition
    else
      CaretPos := 0;
  end
  else
    inherited InitCaret(FocusedRow,CaretPos);
end;

procedure TIPEqComposite.DeleteCharacter(CaretEvent:TIPEqCaretEvent);
begin
  with CaretEvent do
  begin
    ExtendSelection := true;
  end;
end;


function TIPEqComposite.GetNodeAt(x,y:Integer):TIPEqNode;

  {return the x distance only if y is within top and bottom of
   row}
  function GetDist(Node:TIPEqNode):Integer;
  begin
    if (Node.YLoc <= Y) and ((Node.YLoc+Node.Height) >= Y) then
    begin
      if X < Node.XLoc then
        Result := Node.XLoc - X
      else if X > (Node.XLoc+Node.Width) then
        Result := X - (Node.XLoc+Node.Width)
      else
        Result := 0;
    end
    else
      Result := -1;

  end;

var
  ClosestRow : TIPEqRow;
  ClosestDist : Integer;
  I : Integer;
  D : Integer;
  R : TIPEqRow;
begin
  Result := inherited GetNodeAt(X,Y);
  {If generic handling returned self then we must find the
   Closest row withing ourself.}
  if Result = self then
  begin
    //First make x and y relative to self
    X := X - XLoc;
    Y := Y - YLoc;
    ClosestDist := 9999999;
    ClosestRow := nil;
    for I := 0 to ChildCount-1 do
    begin
      R := Row[I];
      D := getDist(R);
      if (D >= 0) and (D < ClosestDist) then
      begin
        ClosestDist := D;
        ClosestRow := R;
      end;
    end;

    if Assigned(ClosestRow) then
      Result := ClosestRow;

  end;
end;

procedure TIPEqComposite.AddNodes(Nodes:TIPEqNodeList);
begin
  Row[0].AddChildren(Nodes);
end;

procedure TIPEqComposite.GotoNextRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent);
var
  Row : TIPEqRow;
  Index : Integer;
begin
  if CaretEvent.ExtendSelection then
  begin
    if Parent is TIPEqRow then
    begin
      Row := TIPEqRow(Parent);
      Index := Row.GetChildIndex(Self);
      CaretEvent.Row := Row;
      CaretEvent.PositionStart := Row.FocusedPosition[Index];
      CaretEvent.Position := Row.FocusedPosition[Index+1];
    end;
  end
  else
    inherited GotoNextRow(Node,CaretEvent);
end;

procedure TIPEqComposite.GotoPrevRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent);
var
  Row : TIPEqRow;
  Index : Integer;
begin
  if CaretEvent.ExtendSelection then
  begin
    if Parent is TIPEqRow then
    begin
      Row := TIPEqRow(Parent);
      Index := Row.GetChildIndex(Self);
      CaretEvent.Row := Row;
      CaretEvent.Position := Row.FocusedPosition[Index];
      CaretEvent.PositionStart := Row.FocusedPosition[Index+1];
    end;
  end
  else
    inherited GotoPrevRow(Node,CaretEvent);
end;

function TIPEqComposite.GetText:String;
var
 I : Integer;
begin
  Result := '@'+GetName + '{';
  for I := 0 to ChildCount-1 do
  begin
    if I > 0 then
      Result := Result + ';';
    Result := Result + Child[I].Text;
  end;
  Result := Result + '}';
end;


end.
