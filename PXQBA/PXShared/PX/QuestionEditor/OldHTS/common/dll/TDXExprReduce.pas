unit TDXExprReduce;

interface

uses TDXExpr,Contnrs,VarCmplx,Sysutils,IPStrUtils;

type
  TTDXExprReduce = class(TTDXExpr)
  private
    FExpr : TTDXExpr;
    FValueFormat : String;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
      function GetExprCount:Integer; override;
      function GetExpr(Index:Integer):TTDXExpr; override;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
    public
      Constructor Create(Expr:TTDXExpr; Fmt:String = '');
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function IsNumber:boolean; override;
      function IsConstant: boolean; override;
      function IsInteger:boolean; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean; override;
      function IsNumericFraction:boolean; override;
      function IsSimplifiedFraction:boolean; override;
      function IsFactFraction:boolean; override;
      function IsNumericMult:boolean; override;
      function IsMixedNumber:boolean; override;
      function FindDivide:TTDXExprBinary; override;
      function FindOrderedPair:TTDXExprOrderedPair; override;
      function IsValidCommaNumber: boolean; override;
      function Clone:TTDXExpr; override;
      function SimilarExpr:TTDXExpr; override;
      property Expr:TTDXExpr read FExpr;
      property ValueFormat:String read FValueFormat write FValueFormat; 
    end;

function GetReducedEqText(Expr:TTDXExpr; ValueFormat:String = ''):String;


implementation

uses IPMath, Math;

function GetReducedEqText(Expr:TTDXExpr; ValueFormat:String = ''):String;

  function FormatNum(V:TTDXExprValue):String;
  var
    D : Double;
    Fmt : String;
  begin
    //Type cast to double becuase TDXVar variant Is currently not handling
    //integers very well.  We'll need to fix but not a priority.
    if Trim(ValueFormat) = '' then
    begin
      D := Round(V);
      if (Abs(D-V) < 1e-6) and (Trim(ValueFormat) = '') then
        Fmt := '#'
      else
      begin
        //**MAD** 8/5/04 instead of using default formatting just use Value I guess.

        Result := V;
        Exit;
      end;
    end
    else
      Fmt := Trim(ValueFormat);

    Result := IPFormatFloat(V,Fmt);
  end;

  function IsNumber(Str:String):boolean;
  begin
    try
      IPStrToFloat(Str);
      Result := true;
    except
      Result := false;
    end;
  end;

var
    TExpr : TTDXExpr;
    GCFVal : Double;
    V1,V2 : Variant;
begin

  if Expr Is TTDXExprUnary then
  begin
    TExpr := TTDXExprUnary(Expr).Expr;
    Result := '-';
  end
  else
  begin
    TExpr := Expr;
    Result := '';
  end;

  if TExpr Is TTDXExprLiteral then
  begin
    Result := Result + FormatNum(TExpr.ExprValue);
  end
  else if (TExpr Is TTDXExprBinary) and (TTDXExprBinary(TExpr).BinaryType = tdxebDivide) then
  begin
    with TTDXExprBinary(TExpr) do
    begin
      if Expr1.IsNumber and Expr2.IsNumber then
      begin
        V1 := Expr1.ExprValue;
        V2 := Expr2.ExprValue;
        GCFVal := GCF(V1,V2);
        if (V1*V2 < 0) then
          Result := Result + '-';
        V1 := Abs(V1);
        V2 := Abs(V2);
        if GCFVal = 0 then
          Result := Result + FormatNum(V1/V2)
        else if CompareValue(V2/GCFVal,1,1e-6) = 0 then
          Result := Result + FormatNum(Abs(V1)/GCFVal)
        else
          Result := Result + '@DIV{'+FOrmatNum(V1/GCFVal)+';'+FormatNum(V2/GCFVal)+'}';
      end;
    end
  end
  else
    Result := '';

end;




Constructor TTDXExprReduce.Create(Expr:TTDXExpr; Fmt:String='');
begin
  FExpr := Expr;
  FValueFormat := Fmt;
end;

function TTDXExprReduce.Clone:TTDXExpr;
begin
  Result := TTDXExprReduce.Create(FExpr.Clone);
end;

function TTDXExprReduce.SimilarExpr:TTDXExpr;
begin
  Result := FExpr.SimilarExpr;
end;

function TTDXExprReduce.GetExprValue:TTDXExprValue;
var
  V : TTDXExprValue;
begin
  V := FExpr.ExprValue;
  Result := VarComplexSimplify(V);
end;

function TTDXExprReduce.GetEqText(ExprForm:boolean = false):String;
var
  Expr : TTDXExpr;
begin
  Expr := FExpr.ReducedExpr(FValueFormat);
  try
    if Assigned(Expr) then
      Result := Expr.GetEqText(ExprForm)
    else
      Result := '';
  finally
    Expr.Free;
  end;
end;


Destructor TTDXExprReduce.Destroy;
begin
  inherited Destroy;
  FExpr.Free;
end;

function TTDXExprReduce.GetToString:String;
begin
  Result := 'Reduce['+FExpr.GetEqText+']';
end;

function TTDXExprReduce.IsNumber:boolean;
begin
  Result := FExpr.IsNumber;
end;

function TTDXExprReduce.IsConstant:boolean;
begin
  Result := FExpr.IsConstant;
end;

function TTDXExprReduce.IsInteger:boolean;
begin
  Result := FExpr.IsInteger;
end;

function TTDXExprReduce.IsEqual(Expr:TTDXExpr):boolean;
begin
  if (Expr Is TTDXExprReduce) then
    Result := TTDXExprReduce(Expr).FExpr.IsEqual(FExpr)
  else
    Result := false;
end;

function TTDXExprReduce.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;
begin
  if (Expr Is TTDXExprReduce) then
    Result := TTDXExprReduce(Expr).FExpr.IsSimilar(FExpr,ReduceMult)
  else
    Result := false;
end;


function TTDXExprReduce.IsValidCommaNumber: boolean;
begin
  Result := FExpr.IsValidCommaNumber;
end;

function TTDXExprReduce.IsNumericFraction:boolean;
begin
  Result := FExpr.IsNumericFraction;
end;

function TTDXExprReduce.IsSimplifiedFraction:boolean;
begin
  Result := FExpr.IsSimplifiedFraction;
end;

function TTDXExprReduce.IsFactFraction:boolean;
begin
  Result := FExpr.IsFactFraction;
end;


function TTDXExprReduce.IsNumericMult:boolean;
begin
  Result := FExpr.IsNumericMult;
end;

function TTDXExprReduce.IsMixedNumber:boolean;
begin
  Result := FExpr.IsMixedNumber;
end;


function TTDXExprReduce.FindDivide:TTDXExprBinary;
begin
  Result := FExpr.FindDivide;
end;

function TTDXExprReduce.FindOrderedPair:TTDXExprOrderedPair;
begin
  Result := FExpr.FindOrderedPair;
end;

function TTDXExprReduce.GetExprCount:Integer;
begin
  Result := 1;
end;

function TTDXExprReduce.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := nil; //Compiler messages cleanup 3/2/05
  if Index = 0 then
    Result := FExpr
  else
    ExprOutOfBounds;
end;

procedure TTDXExprReduce.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  if Index = 0 then
  begin
    FExpr.Free;
    FExpr := Value;
  end
  else
    ExprOutOfBounds;
end;


end.
