unit TDXExprSimEq;

interface

uses TDXExpr,Contnrs,VarCmplx,Sysutils,AttrUtils;

type
  TTDXAnswerRule = (arAcceptAnyForm,arAcceptSimilarForm,
  arExact,arText,arReducedNumber,arNumericOnly,arNumericFrac,
  arNumericMult,arNoEvalFrac,arNoReduceFrac,arNoDecimalEquiv);

const
   TDXAnswerRulesLongName : array[TTDXAnswerRule] of string = (
      'Accept any form',
      'Accept similar form',
      'Accept only specified answer',
      'Text comparison',
      'Fully-Reduced Number, Fraction or Mixed Number',
      'Any Numeric',
      'Numeric Fraction no operations',
      'Numeric with multiplication only',
      'Single non-reduced fraction',
      'Any non-reduced fraction',
      'Do not accept decimal equivalents');

   TDXAnswerRulesShortName : array[TTDXAnswerRule] of string = (
      'anyform','similarform',
      'exact','text','reducednum','numeric','numericfrac','numericmult','noevalfrac','noreducefrac','nodecimalequiv');

type

  TTDXExprSimEq = class(TTDXExpr)
  private
    FExpr1 : String;
    FExpr2 : String;
    FExprType : String;
    FAnswerRule : TTDXAnswerRule;
  protected
    function GetExprValue:TTDXExprValue; override;
    function GetToString:String; override;
    function GetAnswerRule:TTDXAnswerRule;
  public
    Constructor Create(Expr1,Expr2,ExprType:String);
    Destructor Destroy; override;
    function Clone:TTDXExpr; override;
    function IsEqual(Expr:TTDXExpr):Boolean; override;
    function GetEqText(ExprForm:boolean = false):string; override;

  end;



implementation

uses Math,IPMath,STStrL,Clipbrd;



Constructor TTDXExprSimEq.Create(Expr1,Expr2,ExprType:String);
begin
  FExpr1 := Expr1;
  FExpr2 := Expr2;
  FExprType := ExprType;
  FAnswerRule := GetAnswerRule;
end;

function TTDXExprSimEq.GetAnswerRule:TTDXAnswerRule;
var
  Str : String;
  I : Integer;
  AnswerRule : TTDXAnswerRule;
begin
  Str := FExprType;
  I := GetChoiceAttributeValue(str,TDXAnswerRulesShortName,-1);
  if I < 0 then
  begin
    Str := '"'+ Str + '" is not a valid answer rule.  Valid rules are: '#13#10;
    for AnswerRule := Low(TTDXAnswerRule) to High(TTDXAnswerRule) do
      Str := Str + #13#10'  '+ PadL('"' + TDXAnswerRulesShortName[AnswerRule] + '"',22)+#9'- ' + TDXAnswerRulesLongName[AnswerRule];
    Clipboard.AsText := Str;
    raise Exception.Create(Str);
  end;
  Result := TTDXAnswerRule(I);
end;

function TTDXExprSimEq.Clone:TTDXExpr;
begin
  Result := TTDXExprSimEq.Create(FExpr1,FExpr2,FExprType);
end;

Destructor TTDXExprSimEq.Destroy;
begin
  inherited Destroy;
end;

function TTDXExprSimEq.GetExprValue:TTDXExprValue;
begin
  Result := false;
end;

function TTDXExprSimEq.GetToString:String;
begin
  Result := 'SYMEQ['  + '"' + FExpr1 +'","'+ FExpr2+'","'+TDXAnswerRulesShortName[FAnswerRule]+'"]';
end;

function TTDXExprSimEq.IsEqual(Expr:TTDXExpr): Boolean;
begin
  Result := False; //Compiler messages cleanup 3/2/05
end;

function TTDXExprSimEq.GetEqText(ExprForm: boolean): string;
begin
  Result := ''; //Compiler messages cleanup 3/2/05
end;

end.

