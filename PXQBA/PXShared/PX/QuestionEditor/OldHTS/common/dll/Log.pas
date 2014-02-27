unit Log;

interface

uses SysUtils,TDXExpr;

procedure LogStr(Str:string);
procedure LogExpr(Expr:TTDXExpr);

implementation

const LOGNAME:string = 'c:\dlllog.txt';

procedure LogStr(Str:string);
var
 F:Text;
begin
  AssignFile(F,LOGNAME);
  if FileExists(LOGNAME) then
    Append(F)
  else
    Rewrite(F);

  Writeln(F,DateTimeToStr(Now()),'   ',Str);

  Close(F);
end;

procedure LogExpr(Expr:TTDXExpr);
var I:integer;
begin
  LogStr(Expr.ClassName+': '+IntToStr(Expr.ExprCount));
  for I := 0 to Expr.ExprCount - 1 do
    LogExpr(Expr.Expr[1]);
end;

end.
