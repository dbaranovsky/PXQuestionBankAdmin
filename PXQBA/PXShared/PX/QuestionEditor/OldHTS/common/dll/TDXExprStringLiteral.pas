unit TDXExprStringLiteral;

interface

uses TDXExpr,Contnrs,Sysutils;

type

  TTDXExprStringLiteral = class(TTDXExpr)
    private
      FExprText:String;
      FIsMath : boolean;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
    public
      Constructor Create(Value:TTDXExprValue); overload;
      Constructor Create(Value:TTDXExprValue; AIsMath:boolean); overload;
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function Clone:TTDXExpr; override;
      property IsMath:boolean read FIsMath write FIsMath;
  end;

implementation

uses TDXExprVariable;

{****************************************************************************}
{**** TTDXExprStringLiteral *****************************************************}
{****************************************************************************}

Constructor TTDXExprStringLiteral.Create(Value:TTDXExprValue);
begin
  Create(Value,false);
end;

Constructor TTDXExprStringLiteral.Create(Value:TTDXExprValue; AIsMath:boolean);
begin
  FExprText := Value;
  FIsMath := AIsMath;
end;

function TTDXExprStringLiteral.Clone:TTDXExpr;
begin
  Result := TTDXExprStringLiteral.Create(FExprText);
end;

function TTDXExprStringLiteral.GetExprValue:TTDXExprValue;
begin
  Result := FExprText;
end;

function TTDXExprStringLiteral.GetEqText(ExprForm:boolean = false):String;
begin
  if ExprForm then
    Result := AnsiQuotedStr(FExprText,'"')
  else
    Result := GetExprValue;
end;

Destructor TTDXExprStringLiteral.Destroy;
begin
  inherited Destroy;
end;

function TTDXExprStringLiteral.GetToString:String;
begin
  Result := GetExprValue;
end;

function TTDXExprStringLiteral.IsEqual(Expr:TTDXExpr):boolean;
begin
  Result := false;
  if (Expr is TTDXExprStringLiteral) then
    Result := SameText(TTDXExprStringLiteral(Expr).ExprValue,ExprValue);
end;

end.
