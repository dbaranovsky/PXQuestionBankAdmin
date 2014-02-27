unit TDXExprVariable;

interface

uses TDXExpr,Contnrs,Sysutils,Classes;

type

  TTDXExprVariable = class(TTDXExpr)
  private
    FVarName:String;
    FIsMath : boolean;
  protected
    function GetExprValue:TTDXExprValue; override;
    function GetToString:String; override;
    function GetEqRule:TTDXExprEqRule; override;
    function GetExprText:TTDXExprValue; override;
  public
    Constructor Create(AVarName:String);
    Destructor Destroy; override;
    function GetEqText(ExprForm:boolean = false):String; override;
    function IsEqual(Expr:TTDXExpr):boolean; override;
    function Clone:TTDXExpr; override;
    property VarName: String read FVarName write FVarName;
    property IsMath:boolean read FIsMath write FIsMath;
  end;

implementation

uses StStrL;

Constructor TTDXExprVariable.Create(AVarName:String);
begin
  FVarName := FilterL(AVarName,'[]');
end;

function TTDXExprVariable.Clone:TTDXExpr;
begin
  Result := TTDXExprVariable.Create(FVarName);
end;

Destructor TTDXExprVariable.Destroy;
begin
  inherited Destroy;
end;

function TTDXExprVariable.GetExprValue:TTDXExprValue;
begin
    Result := '~'+FVarName;
end;

function TTDXExprVariable.GetExprText:TTDXExprValue;
begin
    Result := '~'+FVarName;
end;


function TTDXExprVariable.GetEqText(ExprForm:boolean = false):String;
begin
    Result := '~'+FVarName
end;

function TTDXExprVariable.GetEqRule:TTDXExprEqRule;
begin
  Result := inherited GetEqRule;
end;


function TTDXExprVariable.GetToString:String;
begin
  Result := '~'+FVarName;
end;

function TTDXExprVariable.IsEqual(Expr:TTDXExpr):boolean;
begin
  Result := false;
  if Expr is TTDXExprVariable then
    Result := SameText(TTDXExprVariable(Expr).FVarName,FVarName);
end;


end.
