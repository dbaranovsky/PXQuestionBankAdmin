unit TDXExprMath;

interface

uses TDXExpr,Contnrs,Sysutils{,MathClient};

type

  TTDXExprMath = class(TTDXExpr)
    private
      FExpr : TTDXExpr;
      FEvaluate : boolean;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
    public
      Constructor Create(AExpr:TTDXExpr; AEval:boolean);
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function Clone:TTDXExpr; override;
      property Expr:TTDXExpr read FExpr;
  end;

implementation

uses TDXExprVariable;

{****************************************************************************}
{**** TTDXExprMath *****************************************************}
{****************************************************************************}

Constructor TTDXExprMath.Create(AExpr:TTDXExpr; AEval:boolean);
begin
  FExpr := AExpr;
  FEvaluate := AEval;
end;

Destructor TTDXExprMath.Destroy;
begin
  inherited Destroy;
  FExpr.Free;
end;


function TTDXExprMath.Clone:TTDXExpr;
begin
  Result := TTDXExprMath.Create(FExpr.Clone,FEvaluate);
end;

function TTDXExprMath.GetExprValue:TTDXExprValue;
var
 Str : String;
begin
  Str := FExpr.ExprValue;
  REsult := Str;
end;

function TTDXExprMath.GetEqText(ExprForm:boolean = false):String;
begin
  if ExprForm then
  begin
    Result := FExpr.GetEqText(ExprForm);
    if FEvaluate then
      REsult := 'meval('+Result+')'
    else
      REsult := 'math('+Result+')';
  end
  else
    Result := GetExprValue;
end;


function TTDXExprMath.GetToString:String;
begin
  Result := GetExprValue;
end;

function TTDXExprMath.IsEqual(Expr:TTDXExpr):boolean;
begin
  Result := false;
  if (Expr is TTDXExprMath) then
  begin
    if (TTDXExprMath(Expr).FEvaluate = FEvaluate) and
       (FExpr.IsEqual(Expr)) then
       Result := true;
  end;
end;

end.
