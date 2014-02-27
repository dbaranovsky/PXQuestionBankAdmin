unit IPEqStack;

interface

uses IPEqNode,IPEqComposite;

type

TIPEqStack = class(TIPEqComposite)
  private
  protected
  public
    function GetRowAbove(ARow:Integer):TIPEqRow; virtual;
    function GetRowBelow(ARow:Integer):TIPEqRow; virtual;
end;

implementation

function TIPEqStack.GetRowAbove(ARow:Integer):TIPEqRow;
var
  Y,YAbove:Integer;
  I : Integer;
  AboveRow:TIPEqRow;
  Node : TIPEqRow;
begin
  Y := Row[ARow].Yloc;
  YAbove := 0;
  AboveRow := nil;
  for i := 0 to ChildCount-1 do
  begin
    if I <> ARow then
    begin
     Node := Row[I];
     if (Node.YLoc < Y) and (Node.YLoc >= YAbove) then
       if (AboveRow = nil) or ((AboveRow <> nil) and (Node.YLoc > AboveRow.YLoc)) then
         AboveRow := Node;
    end;
  end;
  Result := AboveRow;
end;

function TIPEqStack.GetRowBelow(ARow:Integer):TIPEqRow;
var
  Y,YBelow:Integer;
  I : Integer;
  BelowRow:TIPEqRow;
  Node : TIPEqRow;
begin
  Y := Row[ARow].Yloc;
  YBelow := Height;
  BelowRow := nil;
  for I := 0 to ChildCount-1 do
  begin
    if I <> ARow then
    begin
     Node := Row[I];
     if (Node.YLoc > Y) and (Node.YLoc <= YBelow) then
       if (BelowRow = nil) or ((BelowRow <> nil) and (Node.YLoc < BelowRow.YLoc)) then
         BelowRow := Node;
    end;
  end;
  Result := BelowRow;
end;


end.
