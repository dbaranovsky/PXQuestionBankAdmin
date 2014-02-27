unit TDXExprFunction;

interface

uses TDXExpr,Contnrs,VarCmplx,Sysutils;

type

  TTDXExprFunctionType = (tdxefAbs,tdxefSin,tdxefCos,tdxefTan,tdxefArcSin,
    tdxefArcCos,tdxefArcTan,tdxefExp,tdxefLog,tdxefSqrt,tdxefLn,tdxefFloor,
    tdxefCeil,tdxefLogX,tdxefRtx,tdxefGCF,tdxefDiv,tdxefMod,tdxefMin,tdxefMax,
    tdxefPerm,tdxefComb,tdxefSimPAB,tdxefSimPBA,tdxefGCS,tdxefGCSR,tdxefLCM,
    tdxefTrunc,tdxefRound,tdxefFact,tdxefDivX,tdxefSec,tdxefCsc,tdxefCot,
    tdxefArcSec,tdxefArcCsc,tdxefArcCot,tdxefIfThen,
    tdxefFillArray,tdxefSubArray,tdxefFrac2Mnum,tdxefEqCase,tdxefNeqCase,
    tdxefSinh,tdxefCosh,tdxefTanh,tdxefCsch,tdxefSech,tdxefCoth,
    tdxefArcSinh,tdxefArcCosh,tdxefArcTanh,tdxefArcCsch,tdxefArcSech,tdxefArcCoth);

const
  TDXExprFunctionTypeNames: array[TTDXExprFunctionType] of string = (
    'abs','sin','cos','tan','arcsin',
    'arccos','arctan','exp','log','sqrt','ln','floor',
    'ceil','logx','rtx','gcf','div','mod','min','max',
    'perm','comb','simpab','simpba','gcs','gcsr','lcm','trunc','round',
    'fact','divx','sec','csc','cot','arcsec','arccsc','arccot','IfThen',
    'FillArray','SubArray','Frac2Mnum','EqCase','NEqCase','sinh','cosh','tanh',
    'csch','sech','coth','arcsinh','arccosh','arctanh','arccsch','arcsech','arccoth');

type

  TTDXExprFunction = class(TTDXExpr)
  private
    FType : TTDXExprFunctionType;
    FParams : TObjectList;
    FAltInverseForm : boolean;
    function GetParamCount:Integer;
    function GetParam(Index:Integer):TTDXExpr;
    function GetFrac2MnumForm:String;
    procedure Frac2MnumParse(Var Whole,Numer,Denom:Integer);
  protected
    function GetExprValue:TTDXExprValue; override;
    function GetToString:String; override;
    function GetExprCount:Integer; override;
    function GetExpr(Index:Integer):TTDXExpr; override;
    procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
  public
    Constructor Create(AType:TTDXExprFunctionType; AParams:array of TTDXExpr);
    Destructor Destroy; override;
    function GetEqText(ExprForm:boolean = false):String; override;
    function GetParamEqText(ExprForm:boolean = false):String;
    function IsEqual(Expr:TTDXExpr):boolean; override;
    function IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean; override;
    function IsTerm:boolean; override;
    function IsValidCommaNumber: boolean; override;
    function Clone:TTDXExpr; override;
    function SimilarExpr:TTDXExpr; override;
    procedure AddParam(AParam:TTDXExpr);
    function ReducedExpr(Fmt:String=''):TTDXExpr; override;

    property ParamCount:Integer read GetParamCount;
    property Param[Index:Integer]:TTDXExpr read GetParam;
    property AltInverseForm:boolean read FAltInverseForm write FAltInverseForm;
    property FuncType:TTDXExprFunctionType read FType;
  end;



implementation

uses Math,IPMath,Variants, IPFinancial;

Constructor TTDXExprFunction.Create(AType:TTDXExprFunctionType; AParams:array of TTDXExpr);
var
 I : Integer;
begin
  FType := AType;
  FParams := TObjectList.Create;
  for I := 0 to High(AParams) do
  begin
    if Assigned(AParams[I]) then
      FParams.Add(AParams[I]);
  end;
end;

procedure TTDXExprFunction.AddParam(AParam:TTDXExpr);
begin
  if Assigned(AParam) then
    FParams.Add(AParam);
end;

function TTDXExprFunction.Clone:TTDXExpr;
var
 I : integer;
begin
  Result := TTDXExprFunction.Create(FType,[]);
 (Result as TTDXExprFunction).AltInverseForm := FAltInverseForm;
  for I := 0 to ParamCount-1 do
    TTDXExprFunction(Result).FParams.Add(Param[I].Clone);
end;

function TTDXExprFunction.SimilarExpr:TTDXExpr;
var
 I : integer;
 DivExpr : TTDXExprBinary;
 Whole,Numer,Denom:Integer;
begin
  if FType = tdxefDivX then
  begin
     //So we don't have to repeat the login in the binaryExpression.SimilarExpr
     //Let's create a divide not and call it's SimilarExpr.
     DivExpr := TTDXExprBinary.Create(tdxebDivide,Param[0].Clone,Param[1].Clone);
     try
       Result := DivExpr.SimilarExpr;
     finally
       DivExpr.Free;
     end;
  end
  else if FType = tdxefSqrt then
  begin
     Result :=  TTDXExprBinary.Create(tdxebPower,Param[0].SimilarExpr,
                 TTDXExprBinary.Create(tdxebPower,TTDXExprLiteral.Create(2),
                 TTDXExprBinary.Create(tdxebMultiply,TTDXExprLiteral.Create(1),TTDXExprLiteral.Create(-1)))
                 );
  end
  else if FType = tdxefRTX then
  begin
     Result :=  TTDXExprBinary.Create(tdxebPower,Param[1].Clone,
                 TTDXExprBinary.Create(tdxebPower,Param[0].Clone,
                 TTDXExprBinary.Create(tdxebMultiply,TTDXExprLiteral.Create(1),TTDXExprLiteral.Create(-1)))
                 );
  end
  else if FType = tdxefFrac2Mnum then
  begin
    Frac2MnumParse(Whole,Numer,Denom);
    if Whole <> 0 then
    begin
      if Numer <> 0 then
        Result := TTDXExprMixedNum.Create(TTDXExprLiteral.Create(Whole),
            TTDXExprLiteral.Create(Numer),TTDXExprLiteral.Create(Denom))
      else
        Result := TTDXExprLiteral.Create(Whole);
    end
    else
    begin
      if Numer <> 0 then
        Result := TTDXExprBinary.Create(tdxebDivide,TTDXExprLiteral.Create(Numer),
          TTDXExprLiteral.Create(Denom))
      else
        Result := TTDXExprLiteral.Create(0);
    end;
  end
  else
  begin
    Result := TTDXExprFunction.Create(FType,[]);
   (Result as TTDXExprFunction).AltInverseForm := FAltInverseForm;
    for I := 0 to ParamCount-1 do
      TTDXExprFunction(Result).FParams.Add(Param[I].SimilarExpr);
  end;
end;


Destructor TTDXExprFunction.Destroy;
begin
  inherited Destroy;
  FParams.Free;
end;

function TTDXExprFunction.GetParamCount:Integer;
begin
  Result := FParams.Count;
end;

function TTDXExprFunction.GetParam(Index:Integer):TTDXExpr;
begin
  Result := FParams[Index] as TTDXExpr;
end;


function TTDXExprFunction.GetExprCount:Integer;
begin
  Result := GetParamCount;
end;

function TTDXExprFunction.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := GetParam(Index);
end;

procedure TTDXExprFunction.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  if ExprCount < Index then
  begin
    FParams.Delete(Index);
    FParams.Insert(Index,Value);
  end
  else if ExprCount = Index then
    FParams.Add(Value)
  else
    raise Exception.CreateFmt('Can not add Expression at position %d',[Index]);
end;


function TTDXExprFunction.GetExprValue:TTDXExprValue;

  function ExpandedArgValues:Variant;
  var
    I, J , K : Integer;
    N,M : Integer;
    ExpParams : Variant;
    V : Variant;
  begin
    N := ParamCount;
    K := 0;
    ExpParams :=  VarArrayCreate([0,N-1],varVariant);
    for I := 0 to ParamCount-1 do
    begin
      V := Param[I].ExprValue;
      if VarIsArray(V) then
      begin
        m := VarArrayHighBound(V,1)+1;
        VarArrayRedim(ExpParams,VarArrayHighBound(ExpParams,1)+M-1);
        for J := 0 to M-1 do
        begin
          ExpParams[K] := V[J];
          Inc(K);
        end;
      end
      else
      begin
        ExpParams[K] := V;
        Inc(K);
      end;
    end;
    Result := ExpParams;
  end;

  function AnyValuesComplex(Values:Variant):Boolean;
  var
   I : Integer;
  begin
    Result := False;
    if VarIsArray(Values) then
    begin
      for I := 0 to VarArrayHighBound(Values,1) do
      begin
        if VarIsComplex(Values[I]) then
        begin
          Result := True;
          Exit;
        end;
      end;
    end
    else
    begin
      if VarIsComplex(Values) then
        Result := True;
    end;
  end;

  function IIfThen(V1,V2,V3:Variant):Variant;
  begin
    if V1 then
      Result := V2
    else
      Result := V3;
  end;

  function VarComplexLog(V:Variant):Variant;
  begin
    Result := VarComplexLn(V)/Ln(10);
  end;

  function VarComplexLogX(Base,V:Variant): Variant;
  begin
    Result := VarComplexLn(V)/VarComplexLn(Base);
  end;

  { Compare Magnitudes in the case of complex vars }
  function VarComplexMin: Variant;
  var
    Values : Variant;
    Complex : Boolean;
    I : Integer;
  begin
    Values := ExpandedArgValues;
    Complex := AnyValuesComplex(Values);
    Result := Values[0];
    for I := 1 to VarArrayHighBound(Values,1) do
    begin
      if Complex then
      begin
        if VarComplexAbs(Values[I]) < VarComplexAbs(Result) then
          Result := Values[I];
      end
      else
      begin
        if Values[I] < Result then
          Result := Values[I];
      end;
    end;
  end;

  { Compare Magnitudes in the case of complex vars }
  function VarComplexMax: Variant;
  var
    Values : Variant;
    Complex : Boolean;
    I : Integer;
  begin
    Values := ExpandedArgValues;
    Complex := AnyValuesComplex(Values);
    Result := Values[0];
    for I := 1 to VarArrayHighBound(Values,1) do
    begin
      if Complex then
      begin
        if VarComplexAbs(Values[I]) > VarComplexAbs(Result) then
          Result := Values[I];
      end
      else
      begin
        if Values[I] > Result then
          Result := Values[I];
      end;
    end;
  end;

  function VarComplexNRoot(N, V1: Variant): Variant;
  begin
    Result := VarComplexPower(V1,1/N);
  end;

  function CalcFuncValue(V1,V2,V3:TTDXExprValue):Variant;
  begin
    case FType of
      tdxefAbs:     Result := VarComplexAbs(V1);
      tdxefSin:     Result := VarComplexSin(V1);
      tdxefCos:     Result := VarComplexCos(V1);
      tdxefTan:     Result := VarComplexTan(V1);
      tdxefArcSin : Result := VarComplexArcSin(V1);
      tdxefArcCos : Result := VarComplexArcCos(V1);
      tdxefArcTan : Result := VarComplexArcTan(V1);
      tdxefExp:     Result := VarComplexExp(V1);
      tdxefLog:     Result := VarComplexLog(V1);
      tdxefSqrt:    Result := VarComplexSqrt(V1);
      tdxefLn:      Result := VarComplexLn(V1);
      tdxefFloor:   Result := Floor(V1);
      tdxefCeil:    Result := Ceil(V1);
      tdxefRtx:     Result := VarComplexNRoot(V1,V2);
      tdxefLogX:    Result := VarComplexLogX(V1,V2);
      tdxefGCF:     Result := GCF(V1,V2);
      tdxefDiv:     Result := V1 div V2;
      tdxefMod:     Result := V1 mod V2;
      tdxefMin:     Result := VarComplexMin;
      tdxefMax:     Result := VarComplexMax;
      tdxefPerm:    Result := Perm(V1,V2);
      tdxefComb:    Result := Comb(V1,V2);
      tdxefSimpAB:  Result := SimpAB(V1,V2);
      tdxefSimpBA:  Result := SimpAB(V2,V1);
      tdxefGCS:     Result := GreatestCommonSquare(V1);
      tdxefGCSR:    Result := GreatestCommonSquareRem(V1);
      tdxefLCM:     Result := LCM(V1,V2);
      tdxefTrunc:   Result := IPTruncEx(V1,V2);
      tdxefRound:   Result := IPRoundEx(V1,V2);
      tdxefFact: Result := Factorial(V1);
      tdxefDivx :  Result := V1/V2;
      tdxefSec : Result := VarComplexSec(V1);
      tdxefCsc : Result := VarComplexCsc(V1);
      tdxefCot : Result := VarComplexCot(V1);
      tdxefArcSec : Result := VarComplexArcSec(V1);
      tdxefArcCsc : Result := VarcomplexArcCsc(V1);
      tdxefArcCot : Result := VarComplexArcCot(V1);
      tdxefIfThen : Result := IIfThen(V1,V2,V3);
      tdxefFillArray : Result := FillArray(V1,V2,V3);
      tdxefSubArray: Result := SubArray(V1,V2,V3);
      tdxefFrac2Mnum : Result := V1/V2;
      tdxefEqCase : Result := V1 = V2;
      tdxefNEqCase: Result := V1 <> V2;
      //Added to suport symblic formula stuff.
      tdxefSinh : Result := VarComplexSinH(V1);
      tdxefCosh : Result := VarComplexCosH(V1);
      tdxefTanh : Result := VarComplexTanH(V1);

      tdxefCsch : Result := VarComplexCscH(V1);
      tdxefSech : Result := VarComplexSecH(V1);
      tdxefCoth : Result := VarComplexCotH(V1);

      tdxefArcSinh : Result := VarComplexArcSinH(V1);
      tdxefArcCosH : Result := VarComplexArcCosH(V1);
      tdxefArcTanh : Result := VarComplexArcTanH(V1);

      tdxefArcCsch : Result := VarComplexArcCscH(V1);
      tdxefArcSech : Result := VarComplexArcSecH(V1);
      tdxefArcCoth : Result := VarComplexArcCotH(V1);
    else
      raise Exception.Create('function not yet implemented');
    end;
    Result := VarComplexSimplify(Result);
  end;

var
  Param1 : TTDXExprValue;
  Param2 : TTDXExprValue;
  Param3 : TTDXExprValue;
  IEnd : Integer;
  I : Integer;

begin

  Param1 := Param[0].ExprValue;
  if ParamCount > 1 then
    Param2 := Param[1].ExprValue
  else
    Param2 := 0;
  if ParamCount > 2 then
    Param3 := Param[2].ExprValue
  else
    Param3 := 0;

  if VarIsArray(Param1) and not (FType in [tdxefSubArray,tdxefMin,tdxefMax]) then
  begin
    IEnd := VarArrayHighBound(Param1,1);
    Result := VarArrayCreate([0,IEnd],varVariant);
    for I := 0 to IEnd do
      REsult[I] := CalcFuncValue(Param1[I],Param2,Param3);
  end
  else
    Result := CalcFuncValue(Param1,Param2,Param3);
end;


function TTDXExprFunction.GetToString:String;
var
 I : Integer;
begin
  Result := TDXExprFunctionTypeNames[FType]+'[';
  for I := 0 to ParamCount-1 do
  begin
    if I > 0 then
      Result := Result + ',';
    Result := Result + Param[I].ToString;
  end;
  Result := Result + ']';
end;

function TTDXExprFunction.GetParamEqText(ExprForm:boolean = false):String;
var
  I : Integer;
begin
  if ParamCount = 1 then
  begin
    if Param[0] Is TTDXExprParen then
      Result := Param[0].GetEqText(ExprForm)
    //DV 2/19/08 sin cos x should have parens sin(cos x)
    else if Param[0].IsTerm and not (Param[0] Is TTDXExprFunction) then
      Result := ' '+Param[0].GetEqText(ExprForm)
    else
      Result := '('+Param[0].GetEqText(ExprForm)+')';
  end
  else if ParamCount > 1 then
  begin
    Result := '(';
    for I := 0 to ParamCOunt-1 do
    begin
      if I > 0 then
        Result := Result + ',';
      Result := Result + Param[I].GetEqText(ExprForm);
    end;
    Result := Result +  ')';
  end
  else
    Result := '';
end;

function TTDXExprFunction.ReducedExpr(Fmt:String=''):TTDXExpr;
var
 I : integer;
 Ex1 : TTDXExpr;
 V1 : Extended;

begin
  Result := nil;

  case FType of
    tdxefSqrt : begin
      Ex1 := Param[0].ReducedExpr(Fmt);
      try
        if Ex1.IsNumber then
        begin
          V1 := Ex1.ExprValue;
          if V1 > 0 then
          begin
            V1 := Sqrt(V1);
            if IsZero(Abs(Round(V1)-V1),GetEpsilon(V1)) then
              Result := MakeLiteral(V1,Fmt);
          end;

        end;
      finally
        Ex1.Free;
      end;
    end;
  end;

  if Result = nil then
  begin
    Result := TTDXExprFunction.Create(FType,[]);
    (Result as TTDXExprFunction).AltInverseForm := FAltInverseForm;
    for I := 0 to ParamCount-1 do
      TTDXExprFunction(Result).FParams.Add(Param[I].ReducedExpr(Fmt));
  end;

end;

function TTDXExprFunction.GetEqText(ExprForm:boolean = false):String;

  function AddParen(Node:TTDXExpr):String;
  begin
    if Node.IsTerm and not (Node Is TTDXExprFunction) then
      Result := Node.GetEqText(ExprForm)
    else
      Result := '(' + Node.GetEqText(ExprForm) + ')';
  end;

var
 FStr : String;
begin
  case FType of
    tdxefSqrt : begin
      Result := '@RT{'+Param[0].GetEqText(ExprForm)+'}'
    end;
    tdxefFact : begin
      if (Param[0] Is TTDXExprParen) or (Param[0].IsTerm) then
        Result := Param[0].GetEqText(ExprForm)+'!'
      else
        Result := '('+Param[0].GetEqText(ExprForm)+')!';
    end;
    tdxefAbs : begin
      Result := '@ABS{'+Param[0].GetEqText(ExprForm)+'}'
    end;
    tdxefRTX : begin
      Result := '@RT{'+Param[1].GetEqText(ExprForm)+';'+Param[0].GetEqText(ExprForm)+'}';
    end;
    tdxefExp : begin
      Result := 'e@SUP{'+Param[0].GetEqText(ExprForm)+'}';
    end;
    tdxefLogX : begin
       Result := 'log@SUB{'+Param[0].GetEqText(ExprForm)+'}'+AddParen(Param[1]);
    end;
    tdxefDivX : begin
       Result := AddParen(Param[0]) + '/' + AddParen(Param[1]);
    end;
    tdxefFrac2Mnum : begin
       Result := GetFrac2MnumForm;
    end;
  else
    if ParamCount = 1 then
    begin

      FStr := TDXExprFunctionTypeNames[FType];

      if FAltInverseForm then
      begin
        case FType of
          tdxefArcSin : begin
            FStr := 'sin@SUP{-1}'
          end;
          tdxefArcCos: begin
            FStr := 'cos@SUP{-1}'
          end;
          tdxefArcTan : begin
            FStr := 'tan@SUP{-1}'
          end;
          tdxefArcCot : begin
            FStr := 'cot@SUP{-1}'
          end;
          tdxefArcCsc : begin
            FStr := 'csc@SUP{-1}'
          end;
          tdxefArcSec : begin
            FStr := 'sec@SUP{-1}'
          end;
          tdxefArcSinh : begin
            FStr := 'sinh@SUP{-1}'
          end;
          tdxefArcCosh: begin
            FStr := 'cosh@SUP{-1}'
          end;
          tdxefArcTanh : begin
            FStr := 'tanh@SUP{-1}'
          end;
          tdxefArcCoth : begin
            FStr := 'coth@SUP{-1}'
          end;
          tdxefArcCsch : begin
            FStr := 'csch@SUP{-1}'
          end;
          tdxefArcSech : begin
            fstr := 'sech@SUP{-1}'
        end;
      end;

      Result := FStr + GetParamEqText(ExprForm);

    end
    else
    begin
      Result := TDXExprFunctionTypeNames[FType]+ GetParamEqText(ExprForm);
    end;
  end;
end;
end;


function TTDXExprFunction.IsEqual(Expr:TTDXExpr):boolean;
var
 I : Integer;
begin
  Result := false;
  if (Expr Is TTDXExprFunction) and (TTDXExprFunction(Expr).FType = FType)  then
  begin
    if TTDXExprFunction(Expr).ParamCount = ParamCount then
    begin
      for I := 0 to ParamCount - 1 do
      begin
        if not TTDXExprFunction(Expr).Param[I].IsEqual(Param[I]) then
          Exit;
      end;
      Result := true;
    end;
  end
  else if (FType = tdxefDivX) and (Expr Is TTDXExprBinary) and (TTDXExprBinary(Expr).BinaryType = tdxebDivide) then
    Result := Param[0].IsEqual(TTDXExprBinary(Expr).Expr1) and
              Param[1].IsEqual(TTDXExprBinary(Expr).Expr2);
end;

function TTDXExprFunction.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;
var
 I : Integer;
begin
  Result := false;
  if (Expr Is TTDXExprFunction) and (TTDXExprFunction(Expr).FType = FType)  then
  begin
    if TTDXExprFunction(Expr).ParamCount = ParamCount then
    begin
      for I := 0 to ParamCount - 1 do
      begin
        if not TTDXExprFunction(Expr).Param[I].IsSimilar(Param[I],ReduceMult) then
          Exit;
      end;
      Result := true;
    end;
  end;
end;


function TTDXExprFunction.IsTerm:boolean;
begin
  Result := true;
end;

function TTDXExprFunction.IsValidCommaNumber: boolean;
var
  I : integer;
begin
  Result := true;
  for I := 0 to ParamCount - 1 do
    Result := Result and Param[I].IsValidCommaNumber;
end;

function TTDXExprFunction.GetFrac2MnumForm:String;
var
  Whole,Numer,Denom : Integer;
begin

  Frac2MNumParse(Whole,Numer,Denom);

  if Whole <> 0 then
  begin
    if Numer <> 0 then
      Result := Format('@MNUM{%d;%d;%d}',[Whole,Numer,Denom])
    else
      Result := IntToStr(Whole);
  end
  else
  begin
    if Numer <> 0 then
      Result := Format('@DIV{%d;%d}',[Numer,Denom])
    else
      Result := '0';
  end;

end;

procedure TTDXExprFunction.Frac2MnumParse(Var Whole,Numer,Denom:Integer);
var
  V1,V2 : Variant;
  Simplify : Boolean;
  Nx,Dx : Extended;
begin
  V1 := Param[0].ExprValue;
  V2 := Param[1].ExprValue;
  if not SameValue(Trunc(V1),V1) then
    raise Exception.Create('First parameter of Frac2Mnum must be an integer');
  if not SameValue(Trunc(V2),V2) then
    raise Exception.Create('Second parameter of Frac2Mnum must be an integer');

  Simplify := False;
  if ParamCount > 2 then
  begin
    try
      Simplify := Param[2].ExprValue;
    except
      raise Exception.Create('Third parameter of Frac2Mnum must evaluate to true or false');
    end;
  end;


  Numer := Round(V1);
  Denom := Round(V2);

  Whole := Numer div Denom;
  Numer := Numer - Whole*Denom;

  if Simplify then
  begin
    Nx := Numer;
    Dx := Denom;
    ReduceFraction(Nx,Dx);
    Numer := Round(Nx);
    Denom := Round(Dx);
  end;

end;


end.

