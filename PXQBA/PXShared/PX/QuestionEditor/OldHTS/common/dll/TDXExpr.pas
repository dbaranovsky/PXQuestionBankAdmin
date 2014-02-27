unit TDXExpr;

interface

uses sysutils,contnrs,Variants;

type

  TTDXExprValue = Variant;

  TTDXExprBinaryType = (tdxebAdd,tdxebSubtract,tdxebMultiply,tdxebDivide,
     tdxebPower,tdxebMod,tdxebNRoot,tdxebOr,tdxebAnd);

  TTDXExprRelationalType = (tdxerGT,tdxerGE,tdxerLT,tdxerLE,tdxerEQ,tdxerNE,
      tdxerContains,tdxerOnly);

  TTDXExprUnaryType = (tdxeuMinus,tdxeuNot);

  TTDXExprEqRule = (tdxeqValue,tdxeqText,tdxeqAnyForm);

const
  TDXExprBinaryTypeNames : array[TTDXExprBinaryType] of string = (
     'Add','Subtract','Multiply','Divide','Power','Mod','Nroot',
     'Or','And');

  TDXExprRelationalTypeNames: array[TTDXExprRelationalType] of string = (
     'GT','GE','LT','LE','EQ','NE','CONTAINS','ONLY');

  TDXExprUnaryTypeNames: array[TTDXExprUnaryType] of string = (
     'Minus','Not');

type

  TTDXExprBinary = class;
  TTDXExprOrderedPair = class;

  TTDXExpr = class
    private
    protected
      function GetExprValue:TTDXExprValue; virtual; abstract;
      function GetToString:String; virtual;
      function AddParen(const S:String):String;
      function GetExprCount:Integer; virtual;
      function GetExpr(Index:Integer):TTDXExpr; virtual;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); virtual;
      procedure ExprOutofBounds;
      function  GetEqRule:TTDXExprEqRule; virtual;
      function GetExprText:TTDXExprValue; virtual;
    public
      function IsNumber:boolean; virtual;
      function IsConstant: boolean; virtual;
      function IsFactFraction:boolean; virtual;
      function IsNumericFraction:boolean; virtual;
      function IsSimplifiedFraction:boolean; virtual;
      function IsNumericMult:boolean; virtual;
      function IsInteger:boolean; virtual;
      function IsValidCommaNumber: boolean; virtual;
      function IsEqual(Expr:TTDXExpr):boolean; virtual; abstract;
      function IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean; virtual;
      function IsTerm:boolean; virtual;
      function IsMixedNumber:boolean; virtual;
      function FindDivide:TTDXExprBinary; virtual;
      function FindOrderedPair:TTDXExprOrderedPair; virtual;
      function Clone:TTDXExpr; virtual; abstract;
      function SimilarExpr:TTDXExpr; virtual;
      function ReducedExpr(Fmt:String=''):TTDXExpr; virtual;
      function GetEqText(ExprForm:boolean = false):String; virtual; abstract;
      property ToString:String read GetToString;
      property ExprValue:TTDXExprValue read GetExprValue;
      property ExprCount:Integer read GetExprCount;
      property Expr[Index:Integer]:TTDXExpr read GetExpr write SetExpr;
      property EqRule:TTDXExprEqRule read GetEqRule;
      property ExprText:TTDXExprValue read GetExprText;
  end;

  TTDXSymVarCreateEvent = procedure(Sender:TTDXExpr; VarName:String)
     of object;

  TTDXSymVarGetValueEvent = function(Sender:TTDXExpr; VarName:String):TTDXExprValue
     of object;

  TTDXObjectGetValueEvent = function(Sender:TTDXExpr; ObjName:String; PropName:String; Arg:Variant):TTDXExprValue
     of object;

  TTDXObjectCheckSyntaxEvent = procedure(Sender:TTDXExpr; ObjName:String; PropName:String; NArgs:Integer)
     of object;


  TTDXExprLiteral = class(TTDXExpr)
    private
      FExprValue:TTDXExprValue;
      FEqText : String;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
    public
      Constructor Create(Value:TTDXExprValue);
      Constructor CreateEq(Value:TTDXExprValue; AEqText:String);
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function IsConstant: boolean; override;
      function IsNumber:boolean; override;
      function IsNumericMult:boolean; override;
      function IsInteger:boolean; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function IsTerm:boolean; override;
      function IsValidCommaNumber: boolean; override;
      function Clone:TTDXExpr; override;
      function SimilarExpr:TTDXExpr; override;
  end;

  TTDXExprList = class(TTDXExpr)
   private
     FExprList : TObjectList;
   protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
      function GetExprText:TTDXExprValue; override;
      function GetExprCount:Integer; override;
      function GetExpr(Index:Integer):TTDXExpr; override;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
   public
     constructor Create;
     destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
     function AddExpr(Value:TTDXExpr):Integer;
     function IsEqual(Expr:TTDXExpr):boolean; override;
     function IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean; override;
     function Clone:TTDXExpr; override;
     function SimilarExpr:TTDXExpr; override;
   end;

  TTDXExprBinary = class(TTDXExpr)
    private
      FExpr1 : TTDXExpr;
      FExpr2 : TTDXExpr;
      FBinaryType : TTDXExprBinaryType;
      FOpName : String;
      FAltTrigPowerForm:boolean;
      procedure SetExpr2(Value:TTDXExpr);
      function GetArrayValue(V1,V2:TTDXExprValue):TTDXExprValue;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
      function GetExprCount:Integer; override;
      function GetExpr(Index:Integer):TTDXExpr; override;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
    public
      Constructor Create(AType:TTDXExprBinaryType; Expr1,Expr2:TTDXExpr);
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean; override;
      function SimilarExpr:TTDXExpr; override;
      function ReducedExpr(Fmt:String=''):TTDXExpr; override;
      function IsConstant:boolean; override;
      function IsTerm:boolean; override;
      function IsNumericFraction:boolean; override;
      function IsSimplifiedFraction:boolean; override;
      function IsFactFraction:boolean; override;
      function IsNumericMult:boolean; override;
      function FindDivide:TTDXExprBinary; override;
      function FindOrderedPair:TTDXExprOrderedPair; override;
      function IsValidCommaNumber: boolean; override;
      function Clone:TTDXExpr; override;
      property BinaryType:TTDXExprBinaryType read FBinaryType write FBinaryType;
      property Expr2:TTDXExpr read FExpr2 write SetExpr2;
      property Expr1:TTDXExpr read FExpr1;
      property OpName:String read FOpName write FOpName;
      property AltTrigPowerForm:boolean read FAltTrigPowerForm write FAltTrigPowerForm;
    end;

  TTDXExprRelational = class(TTDXExpr)
    private
      FExpr1 : TTDXExpr;
      FExpr2 : TTDXExpr;
      FRelationalType : TTDXExprRelationalType;
      function Contains(V1,V2: TTDXExprValue; EqRule:TTDXExprEqRule; Exact:boolean = false):TTDXExprValue;
      function Only(V1,V2:TTDXExprValue; EqRule:TTDXExprEqRule):TTDXExprValue;
      function Equals(V1,V2:TTDXExprValue; EqRule:TTDXExprEqRule):TTDXExprValue;
   protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
      function GetExprCount:Integer; override;
      function GetExpr(Index:Integer):TTDXExpr; override;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
    public
      Constructor Create(AType:TTDXExprRelationalType; Expr1,Expr2:TTDXExpr);
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean; override;
      function SimilarExpr:TTDXExpr; override;
      function Clone:TTDXExpr; override;
      property RelationalType: TTDXExprRelationalType read FRelationalType;
      property Expr1 : TTDXExpr read FExpr1;
      property Expr2 : TTDXExpr read FExpr2;
  end;

  TTDXExprUnary = class(TTDXExpr)
    private
      FExpr : TTDXExpr;
      FUnaryType : TTDXExprUnaryType;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
      function GetExprCount:Integer; override;
      function GetExpr(Index:Integer):TTDXExpr; override;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
    public
      Constructor Create(AType:TTDXExprUnaryType; Expr:TTDXExpr);
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function ReducedExpr(Fmt:String=''):TTDXExpr; override;
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
      property UnaryType:TTDXExprUnaryType read FUnaryType;
    end;

  TTDXExprParen = class(TTDXExpr)
    private
      FExpr : TTDXExpr;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
      function GetExprCount:Integer; override;
      function GetExpr(Index:Integer):TTDXExpr; override;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
    public
      Constructor Create(Expr:TTDXExpr);
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function IsConstant:boolean; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean; override;
      function IsValidCommaNumber: boolean; override;
      function Clone:TTDXExpr; override;
      function SimilarExpr:TTDXExpr; override;
      property Expr:TTDXExpr read FExpr;
    end;

  TTDXExprMixedNum = class(TTDXExpr)
    private
      FExprNum : TTDXExpr;
      FExprTop : TTDXExpr;
      FExprBottom : TTDXExpr;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
      function GetExprCount:Integer; override;
      function GetExpr(Index:Integer):TTDXExpr; override;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
    public
      Constructor Create(ANum,ATop,ABottom:TTDXExpr);
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function Clone:TTDXExpr; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function IsTerm:boolean; override;
      function IsValidCommaNumber: boolean; override;
      function IsNumericMult:boolean; override;
      function IsNumber:boolean; override;
      function IsConstant: boolean; override;
      function IsMixedNumber:boolean; override;
      property ExprNum:TTDXExpr read FExprNum;
      property ExprTop:TTDXExpr read FExprTop;
      property ExprBottom:TTDXExpr read FExprBottom;
    end;


  TTDXExprOrderedPair = class(TTDXExpr)
    private
      FExpr1 : TTDXExpr;
      FExpr2 : TTDXExpr;
    protected
      function GetExprValue:TTDXExprValue; override;
      function GetToString:String; override;
      function GetExprCount:Integer; override;
      function GetExpr(Index:Integer):TTDXExpr; override;
      procedure SetExpr(Index:Integer; Value:TTDXExpr); override;
    public
      Constructor Create(AExpr1,AExpr2:TTDXExpr);
      Destructor Destroy; override;
      function GetEqText(ExprForm:boolean = false):String; override;
      function IsEqual(Expr:TTDXExpr):boolean; override;
      function IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean; override;
      function IsTerm:boolean; override;
      function FindDivide:TTDXExprBinary; override;
      function FindOrderedPair:TTDXExprOrderedPair; override;
      function IsValidCommaNumber: boolean; override;
      function IsNumber:boolean; override;
      function IsNumericFraction:boolean; override;
      function IsSimplifiedFraction:boolean; override;
      function IsFactFraction:boolean; override;
      function IsNumericMult:boolean; override;
      function Clone:TTDXExpr; override;
      function SimilarExpr:TTDXExpr; override;
       property Expr1:TTDXExpr read FExpr1;
      property Expr2:TTDXExpr read FExpr2;
  end;

      function GetExprEqText(Expr:TTDXExpr; ExprForm:boolean = false):String;
      function IsSingleValueVarArray(Var V:variant):Boolean;
      function MakeLiteral(V:Double; Fmt:String):TTDXExpr;

implementation

  uses Math,VarCmplx,TDXExprSymbolicVar, TDXOrderedPairVar,Classes,
       IPStrUtils,TDXExprFunction,IPMath,ststrL, TDXVar, StrUtils;


function GetExprEqText(Expr:TTDXExpr; ExprForm:boolean = false):String;
begin
  begin
    Result := Expr.GetEqText(ExprForm);
  end;
end;

function IsSingleValueVarArray(Var V:variant):Boolean;
begin
  Result:= (VarArrayDimCount(V)=1) AND //one-dimensional array
           (VarArrayLowBound(V,1)=VarArrayHighBound(V,1)) //with single Value
end;

function IsUM(Expr:TTDXExpr):Boolean;
begin
  Result := (Expr Is TTDXExprUnary) and (TTDXExprUnary(Expr).FUnaryType = tdxeuMinus);
end;

function InvOp(AOp:TTDXExprBinaryType):TTDXExprBinaryType;
begin
  if AOP = tdxebAdd then
    Result := tdxebSubtract
  else if AOP = TDXEbSubtract then
    Result := tdxebAdd
  else if AOP = tdxebMultiply then
    Result := tdxebDivide
  else if Aop = tdxebDivide then
    Result := tdxebMultiply
  else
    Raise Exception.Create('Invalid inverse operator');
end;

function IsNumEqual(V1,V2:double):Boolean;
begin
  Result := IsFloatEqual(V1,V2);
end;

function IsZero(V:Double):Boolean;
begin
  Result := IsFloatEqual(V,0);
end;

function VarArrayUpperCase(V:Variant):Variant;
var
 I : Integer;
 Low,High : Integer;
begin
  if VarArrayDimCount(V) = 1 then
  begin
    Low := VarArrayLowBound(V,1);
    High := VarArrayHighBound(V,1);
    Result := VarArrayCreate([Low,High],varVariant);
    for I := Low to high do
    begin
      Result[I] := VarArrayUpperCase(V[I]);
    end;
  end
  else
  begin
    //**MAD** Force uppercase regardless of variant type.  If we don't them symbolics don't get converted and bad
    //things happen. 5/31/05
    try
       Result := UpperCase(V);
    except
       Result := V;
    end;
  end;
end;

function VarArrayIsStr(const V:Variant): Boolean;
var
  I : Integer;
  Low,High : Integer;
begin
  if VarArrayDimCount(V) = 1 then
  begin
    Result := False;
    Low := VarArrayLowBound(V,1);
    High := VarArrayHighBound(V,1);
    for I := Low to High do
    begin
      if not VarArrayIsStr(V[I]) then
        Exit;
    end;
    Result := True;
  end
  else
    Result := VarIsStr(V);
end;

function VarArrayEncodeForCompare(V:Variant):Variant;
var
 I : Integer;
 Low,High : Integer;
begin
  if VarArrayDimCount(V) = 1 then
  begin
    Low := VarArrayLowBound(V,1);
    High := VarArrayHighBound(V,1);
    Result := VarArrayCreate([Low,High],varVariant);
    for I := Low to high do
    begin
      Result[I] := VarArrayEncodeForCompare(V[I]);
    end;
  end
  else if VarIsStr(V) then
    Result := IPEncodeTextForCompare(V)
  else
    Result := V;
end;

var
  ONEExpr : TTDXExpr;
  MINUSONEExpr : TTDXExpr;

type
  TExprSimilarNode = class
    FExpr : TTDXExpr;
    FFactor : TTDXExpr;
    FPower :TTDXExpr;
    Constructor Create(AFactor,APower,AExpr:TTDXExpr);
  end;


Constructor TExprSimilarNode.Create(AFactor,APower,AExpr:TTDXExpr);
begin
  FExpr := AExpr;
  FFactor := AFactor;
  FPower := APower;
end;

function TTDXExpr.GetToString:String;
begin
  Result := ClassName;
end;

function TTDXExpr.IsNumber:boolean;
begin
  Result := false;
end;


function TTDXExpr.IsConstant:boolean;
begin
  Result := false;
end;

function TTDXExpr.IsMixedNumber:boolean;
begin
  Result := false;
end;


function TTDXExpr.IsInteger:boolean;
begin
  Result := false;
end;


function TTDXExpr.IsTerm:boolean;
begin
  Result := false;
end;

function TTDXExpr.AddParen(const S:String):String;
begin
  Result := '(' + S + ')';
end;

function TTDXExpr.IsNumericFraction:boolean;
begin
  Result := false;
end;

function TTDXExpr.IsSimplifiedFraction:boolean;
begin
  Result := false;
end;


function TTDXExpr.IsFactFraction:boolean;
begin
  Result := false;
end;


function TTDXExpr.IsNumericMult:boolean;
begin
  Result := false;
end;

function TTDXExpr.FindDivide:TTDXExprBinary;
begin
  Result := nil;
end;

function TTDXExpr.FindOrderedPair:TTDXExprOrderedPair;
begin
  Result := nil;
end;

function TTDXExpr.IsValidCommaNumber: boolean;
begin
  Result := true;
end;

function TTDXExpr.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;
begin
  Result := IsEqual(Expr);
end;

function TTDXExpr.SimilarExpr:TTDXExpr;
begin
  Result := Clone;
end;

function TTDXExpr.ReducedExpr(Fmt:String=''):TTDXExpr;
begin
  Result := Clone;
end;

procedure TTDXExpr.ExprOutofBounds;
begin
  raise Exception.Create('Expression Index out of bounds');
end;

function TTDXExpr.GetExprCount:Integer;
begin
  Result := 0;
end;

function TTDXExpr.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := nil; //Compiler messages cleanup 3/2/05
  ExprOutOfBounds;
end;

procedure TTDXExpr.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  ExprOutOfBounds;
end;

function TTDXExpr.GetEqRule:TTDXExprEqRule;
begin
  Result := tdxeqValue; 
end;

function TTDXExpr.GetExprText:TTDXExprValue;
begin
  Result := GetEqText(false);
end;

{****************************************************************************}
{**** TTDXExprList *******************************************************}
{****************************************************************************}

constructor TTDXExprList.Create;
begin
  FExprList := TObjectList.Create;
end;

destructor TTDXExprList.Destroy;
begin
  inherited Destroy;
  FExprList.Free;
end;

function TTDXExprList.Clone:TTDXExpr;
var
  I : Integer;
begin
  Result := TTDXExprList.Create;
  for I := 0 to ExprCount-1 do
    TTDXExprList(Result).AddExpr(Expr[I].Clone);
end;

function TTDXExprList.SimilarExpr:TTDXExpr;
var
  I : Integer;
begin
  Result := TTDXExprList.Create;
  for I := 0 to ExprCount-1 do
    TTDXExprList(Result).AddExpr(Expr[I].SimilarExpr);
end;


function TTDXExprList.GetExprCount:Integer;
begin
  Result := FExprList.Count;
end;

function TTDXExprList.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := TTDXExpr(FExprList[Index]);
end;

procedure TTDXExprList.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  if ExprCount < Index then
  begin
    FExprList.Delete(Index);
    FExprList.Insert(Index,Value);
  end
  else if ExprCount = Index then
    AddExpr(Value)
  else
    raise Exception.CreateFmt('Can not add Expression at position %d',[Index]);
end;

function TTDXExprList.AddExpr(Value:TTDXExpr):Integer;
begin
  FExprList.Add(Value);
  Result := ExprCount-1;
end;

function TTDXExprList.GetExprText:TTDXExprValue;
var
  I : integer;
begin
  Result := VarArrayCreate([0,ExprCount-1],varVariant);
  for I := 0 to ExprCount-1 do
  begin
    Result[I] := Expr[I].ExprText;
  end;
end;

function TTDXExprList.GetExprValue:TTDXExprValue;
var
  I : integer;
begin
  Result := VarArrayCreate([0,ExprCount-1],varVariant);
  for I := 0 to ExprCount-1 do
  begin
    Result[I] := Expr[I].ExprValue;
  end;
end;

function TTDXExprList.GetEqText(ExprForm:boolean = false):String;
var
 I : integer;
begin
  Result := '';
  for I := 0 to ExprCount-1 do
  begin
    if I > 0 then
      Result := Result + ',';
    Result := Result + Expr[I].GetEqText(ExprForm);
  end;
end;

function TTDXExprList.IsEqual(Expr:TTDXExpr):boolean;
var
  I : integer;
begin
  Result := false;
  if Expr Is TTDXExprList then
  begin
    if TTDXExprList(Expr).ExprCount = ExprCount then
    begin
      for I := 0 to ExprCount-1 do
      begin
        if not Self.Expr[I].IsEqual(TTDXExprList(Expr).Expr[I]) then
          Exit;
      end;
      Result := true;
    end;
  end;
end;

function TTDXExprList.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;
var
  I,J : integer;
  List:TList;
  FoundMatch : boolean;
begin
  Result := false;
  if (Expr Is TTDXExprList) and (TTDXExprList(Expr).ExprCount = ExprCount) then
  begin
    List := TList.Create;
    try
     for I := 0 to ExprCount-1 do
       List.Add(TTDXExprList(Expr).Expr[I]);
     for I := 0 to ExprCount-1 do
     begin
       FoundMatch := false;
       for J := 0 to List.Count-1 do
       begin
          if Self.Expr[I].IsSimilar(TTDXExprList(List[J]).Expr[I],ReduceMult) then
          begin
            FoundMatch := true;
            List.Delete(J);
            Break;
          end;
       end;

       if not FoundMatch then
         Exit;
     end;

     Result := true;

    finally
      List.Free;
    end;
  end;
end;



function TTDXExprList.GetToString:String;
var
 I : integer;
begin
  Result := '';
  for I := 0 to ExprCount-1 do
  begin
    if I > 0 then
      Result := Result + ',';
    Result := Result + Expr[I].ToString;
  end;
  Result := 'List['+Result+']';
end;

{****************************************************************************}
{**** TTDXExprLiteral *******************************************************}
{****************************************************************************}

Constructor TTDXExprLiteral.Create(Value:TTDXExprValue);
begin
  FExprValue := Value;
end;

Constructor TTDXExprLiteral.CreateEq(Value:TTDXExprValue; AEqText:String);
begin
  FExprValue := Value;
  FEqText := AEqText;
end;


function TTDXExprLiteral.Clone:TTDXExpr;
begin
  Result := TTDXExprLiteral.CreateEq(FExprValue,FEqText);
end;

function TTDXExprLiteral.GetExprValue:TTDXExprValue;
begin
  Result := FExprValue;
end;

function TTDXExprLiteral.GetEqText(ExprForm:boolean = false):String;
begin

  if FEqText <> '' then
  begin
    Result := FEqText;
    Exit;
  end;

  Result := FExprValue;
  //If this literal contains commas, it must be a comma formatted number
  if Pos(',',Result) > 0 then
    Result := AnsiReplaceStr(Result,',','.');
      //Result := '@CNUM{'+FilterL(Result,',')+'}';
end;

Destructor TTDXExprLiteral.Destroy;
begin
  inherited Destroy;
//  FExprValue.Free;
end;

function TTDXExprLiteral.GetToString:String;
begin
  Result := FExprValue;
end;

function TTDXExprLiteral.IsNumber:boolean;
begin
  Result :=  not (SameText(ExprText,'&pi;') or SameText(ExprText,'e'))
end;

function TTDXExprLiteral.IsConstant:boolean;
begin
  Result := True;
end;

function TTDXExprLiteral.IsNumericMult:boolean;
begin
  Result := true;
end;


function TTDXExprLiteral.IsInteger:boolean;
begin
  try
    Result := IsZero(Frac(ExprValue));
  except
    Result := false;
  end;
end;

function TTDXExprLiteral.IsValidCommaNumber: boolean;
var
  S :String;
  V : Double;
begin
  S := ExprValue;
  V := ExprValue;
  Result := (Abs(V) < 1000) or
    (Pos(',',S) > 0);
end;

function TTDXExprLiteral.IsEqual(Expr:TTDXExpr):boolean;
begin
  Result := false;

  if (Expr Is TTDXExprLiteral) then
//    if  (Expr As TTDXExprLiteral).IsNumber and VarType(ExprValue)
    //if  VarTypeIsNumeric(VarType((Expr As TTDXExprLiteral).FExprValue)) and VarTypeIsNumeric(VarType(ExprValue)) then
    if IsVariantEqual(Expr.ExprValue, ExprValue) then
      Result := true;
end;

function TTDXExprLiteral.IsTerm:boolean;
begin
  Result := (ExprValue >= 0);
end;

function TTDXExprLiteral.SimilarExpr:TTDXExpr;
begin
  if ExprValue < 0 then
    Result := TTDXExprBinary.Create(tdxebMultiply,
       TTDXExprLiteral.Create(Abs(ExprValue)),
       TTDXExprLiteral.Create(-1))
  else
    Result := Clone;
end;


{****************************************************************************}
{**** TTDXExprBinary *******************************************************}
{****************************************************************************}

Constructor TTDXExprBinary.Create(AType:TTDXExprBinaryType; Expr1,Expr2:TTDXExpr);
begin
  FExpr1 := Expr1;
  FExpr2 := Expr2;
  FBinaryType := AType;
  FOpName := '';
end;

function TTDXExprBinary.Clone:TTDXExpr;
begin
  Result := TTDXExprBinary.Create(FBinaryType,FExpr1.Clone,FExpr2.Clone);
  TTDXExprBinary(Result).OpName := FOpName;
end;

function MakeLiteral(V:Double; Fmt:String):TTDXExpr;
begin
  if Trim(Fmt) <> '' then
  begin
    Result := TTDXExprLiteral.Create(VarTDXCreate(V,IPFormatFloat(V,Fmt)));
  end
  else
  begin
    Result := TTDXExprLiteral.Create(V);
  end;
end;

function TTDXExprBinary.ReducedExpr(Fmt:String=''):TTDXExpr;
var
  Ex1,Ex2,Res,Res1,Res2:TTDXExpr;
  V1,V2 : Extended;
  V1T,V2T : Extended;
  GCFVal : Integer;
  Neg : Boolean;
  SndOp : TTDXExprBinaryType;
begin
  Ex1 := nil;
  Ex2 := nil;
  Res := nil;
  SndOp := tdxebAdd;
  Result := nil;
  try
    Ex1 := FExpr1.ReducedExpr(Fmt);
    Ex2 := FExpr2.ReducedExpr(Fmt);

    if Ex2.IsNumber and (Ex1 Is TTDXExprBinary) and Ex1.Expr[1].IsNumber
    then
    begin
      case TTDXExprBinary(Ex1).FBinaryType of
        tdxebAdd :
        begin
          if FBinaryType in [tdxebAdd,tdxebSubtract] then
          begin
            Res := TTDXExprBinary.Create(tdxebAdd,Ex1.Expr[0].Clone,
                     TTDXExprBinary.Create(FBinaryType,Ex1.Expr[1].Clone,Ex2.Clone));
            Result := Res.ReducedExpr(Fmt);
          end;
        end;
        tdxebSubtract :
        begin
          if FBinaryType in [tdxebAdd,tdxebSubtract] then
          begin
            Res := TTDXExprBinary.Create(tdxebSubtract,Ex1.Expr[0].Clone,
                     TTDXExprBinary.Create(InvOp(FBinaryType),Ex1.Expr[1].Clone,Ex2.Clone));
            Result := Res.ReducedExpr(Fmt);
          end;
        end;
        tdxebMultiply :
        begin
          if FBinaryType in [tdxebMultiply,tdxebDivide] then
          begin
            Res := TTDXExprBinary.Create(tdxebMultiply,Ex1.Expr[0].Clone,
                     TTDXExprBinary.Create(FBinaryType,Ex1.Expr[1].Clone,Ex2.Clone));
            Result := Res.ReducedExpr(Fmt);
          end;
        end;
        tdxebDivide :
        if SndOp in [tdxebMultiply,tdxebDivide] then
        begin
          if FBinaryType = tdxebMultiply then //here replace (x/2)*3 -> x * 3/2. change places 2 , 3
          begin
            Res    := TTDXExprBinary.Create(tdxebMultiply,Ex1.Expr[0].Clone,
                        TTDXExprBinary.Create(tdxebDivide,Ex2.Clone,Ex1.Expr[1].Clone));
            Result := Res.ReducedExpr(Fmt);
          end
          else if FBinaryType = tdxebDivide then //here replace (x/2)/3 -> x /(2*3). no change in places
          begin
            Res := TTDXExprBinary.Create(tdxebMultiply,Ex1.Expr[0].Clone,
                        TTDXExprBinary.Create(tdxebDivide,Ex1.Expr[1].Clone,Ex2.Clone));
            Result := Res.ReducedExpr(Fmt);
          end;
        end;
      end;
    end
    else if Ex1.IsNumber and (Ex2 Is TTDXExprBinary) and Ex2.Expr[0].IsNumber
    then
    begin
      SndOp := TTDXExprBinary(Ex2).FBinaryType;
      case FBinaryType of
        tdxebAdd :
        begin
          if SndOp in [tdxebAdd,tdxebSubtract] then
          begin
            Res    := TTDXExprBinary.Create(SndOp,
                        TTDXExprBinary.Create(tdxebAdd,Ex1.Clone,Ex2.Expr[0].Clone),
                        Ex2.Expr[1].Clone);
            Result := Res.ReducedExpr(Fmt);
          end;
        end;
        tdxebSubtract :
        begin
          if SndOp in [tdxebAdd,tdxebSubtract] then
          begin
            Res    := TTDXExprBinary.Create(InvOp(SndOp),
                        TTDXExprBinary.Create(tdxebSubtract,Ex1.Clone,Ex2.Expr[0].Clone),
                        Ex2.Expr[1].Clone);
            Result := res.ReducedExpr(Fmt);
          end;
        end;
        tdxebMultiply :
        begin
          if SndOp in [tdxebMultiply,tdxebDivide] then
          begin
            Res    := TTDXExprBinary.Create(SndOp,
                        TTDXExprBinary.Create(FBinaryType,Ex1.Clone,Ex2.Expr[0].Clone),
                        Ex2.Expr[1].Clone);
            Result := res.ReducedExpr(Fmt);
          end;
        end;
        tdxebDivide :
        if SndOp in [tdxebMultiply,tdxebDivide] then
        begin
           Res := nil;
           Res1 := nil;
           Res2 := nil;
           try
             Res1 := TTDXExprBinary.Create(tdxebDivide,Ex1.Clone,Ex2.Expr[0].Clone);
             Res := Res1.ReducedExpr(Fmt);
             if Res Is TTDXExprBinary then
             begin
               if SndOp = tdxebMultiply then
               begin
                 Res2   := TTDXExprBinary.Create(tdxebMultiply,Res.Expr[1].Clone,
                                                   Ex2.Expr[1].Clone);
                 Result := TTDXExprBinary.Create(tdxebDivide,
                             res.Expr[0].Clone,
                             Res2.ReducedExpr(Fmt));
               end
               else if SndOp = tdxebDivide then
               begin
                 Res2   := TTDXExprBinary.Create(tdxebDivide,
                             TTDXExprBinary.Create(tdxebMultiply,Res.Expr[0].Clone,
                                                   Ex2.Expr[1].Clone),
                                                   Res.Expr[1].Clone);

                 Result := Res2.ReducedExpr(Fmt);
               end
             end
             else
             begin
               Res2   := TTDXExprBinary.Create(InvOP(SndOp),res.Clone,Ex2.Expr[1].Clone);
               Result := Res2.ReducedExpr(Fmt);
             end;
           finally
             Res1.Free;
             Res2.Free;
           end;
        end;

      end;
    end;

    if Result = nil then
    case FBinaryType of
      tdxebDivide : begin
        if Ex1.IsNumber and Ex2.IsNumber then
        begin
          V1 := Ex1.ExprValue;
          V2 := Ex2.ExprValue;
          GCFVal := GCF(V1,V2);
          Neg := (V1*V2 < 0);
          V1 := Abs(V1);
          V2 := Abs(V2);
          if GCFVal = 0 then
          begin
            Result := MakeLiteral(V1/V2,Fmt);
          end
          else if CompareValue(V2/GCFVal,1,1e-6) = 0 then
          begin
            Result := MakeLiteral(V1/GCFVal,Fmt);
          end
          else
          begin
              V1T := V1/GCFVal;
              V2T := V2/GCFVal;
              V1T := RoundTo(V1T,-1);
              V2T := RoundTo(V2T,-1);
              Result := TTDXExprBinary.Create(tdxebDivide,MakeLiteral(V1T,Fmt),
              MakeLiteral(V2T,Fmt));
          end;

          if Neg then
            Result := TTDXExprUnary.Create(tdxeuMinus,Result);
        end
        else if Ex1.IsNumber and IsZero(Ex1.ExprValue) then
          Result := MakeLiteral(0,Fmt)
        else if Ex2.IsNumber and IsNumEqual(Ex2.ExprValue,1) then
          Result := Ex1.Clone
        else if Ex2.IsNumber and IsNumEqual(Ex2.ExprValue,-1) then
          Result := TTDXExprUnary.Create(tdxeuMinus,Ex1.Clone)
        else if IsUM(Ex1) and not IsUM(Ex2) then
        begin
          Res := TTDXExprBinary.Create(tdxebDivide,Ex1.Expr[0].Clone,Ex2.Clone);
          Result := TTDXExprUnary.Create(tdxeuMinus,Res.Clone);
        end
        else if not IsUM(Ex1) and IsUM(Ex2) then
        begin
          Res := TTDXExprBinary.Create(tdxebDivide,Ex1.Clone,Ex2.Expr[0].Clone);
          Result := TTDXExprUnary.Create(tdxeuMinus,Res.Clone);
        end
        else if IsUM(Ex1) and IsUM(Ex2) then
        begin
          Result := TTDXExprBinary.Create(tdxebDivide,Ex1.Expr[0].Clone,Ex2.Expr[0].Clone);
        end;
      end;
      tdxebAdd : begin
         if Ex1.IsNumber and Ex2.IsNumber then
         begin
           V1 := Ex1.ExprValue;
           V2 := Ex2.ExprValue;
           Result := MakeLiteral(Abs(V1+V2),Fmt);
           if (V1+V2) < 0 then
             Result := TTDXExprUnary.Create(tdxeuMinus,Result);
         end
         else if Ex1.IsNumber and IsZero(Ex1.ExprValue) then
           Result := Ex2.Clone
         else if Ex2.IsNumber and IsZero(Ex2.ExprValue) then
           Result := Ex1.Clone
         else if IsUM(Ex2) then
         begin
           Result := TTDXExprBinary.Create(tdxebSubtract,Ex1.Clone,Ex2.Expr[0].Clone);
         end;
      end;
      tdxebSubtract : begin
         if Ex1.IsNumber and Ex2.IsNumber then
         begin
           V1 := Ex1.ExprValue;
           V2 := Ex2.ExprValue;
           Result := MakeLiteral(Abs(V1-V2),Fmt);
           if (V1-V2) < 0 then
             Result := TTDXExprUnary.Create(tdxeuMinus,Result);
         end
         else if Ex1.IsNumber and IsZero(Ex1.ExprValue) then
         begin
           if IsUM(Ex2) then
             Result := Ex2.Expr[0].Clone
           else
             Result := TTDXExprUnary.Create(tdxeuMinus,Ex2.Clone);
         end
         else if Ex2.IsNumber and IsZero(Ex2.ExprValue) then
           Result := Ex1.Clone
         else if IsUM(Ex2) then
         begin
           Result := TTDXExprBinary.Create(tdxebAdd,Ex1.Clone,Ex2.Expr[0].Clone);
         end;
      end;
      tdxebMultiply : begin
         if Ex1.IsNumber and Ex2.IsNumber then
         begin
           V1 := Ex1.ExprValue;
           V2 := Ex2.ExprValue;
           Result := MakeLiteral(Abs(V1*V2),Fmt);
           if (V1*V2) < 0 then
             Result := TTDXExprUnary.Create(tdxeuMinus,Result);
         end
         else if Ex1.IsNumber and IsZero(Ex1.ExprValue) then
           Result := MakeLiteral(0,Fmt)
         else if Ex2.IsNumber and IsZero(Ex2.ExprValue) then
           Result := MakeLiteral(0,Fmt)
         else if Ex1.IsNumber and IsNumEqual(Ex1.ExprValue,1) then
           Result := Ex2.Clone
         else if Ex1.IsNumber and IsNumEqual(Ex1.ExprValue,-1) then
           Result := TTDXExprUnary.Create(tdxeuMinus,Ex2.Clone)
         else if Ex2.IsNumber and IsNumEqual(Ex2.ExprValue,1) then
           Result := Ex1.Clone
         else if Ex2.IsNumber and IsNumEqual(Ex2.ExprValue,-1) then
           Result := TTDXExprUnary.Create(tdxeuMinus,Ex1.Clone)
        else if IsUM(Ex1) and not IsUM(Ex2) then
         begin
           Res := TTDXExprBinary.Create(tdxebMultiply,Ex1.Expr[0].Clone,Ex2.Clone);
           Result := TTDXExprUnary.Create(tdxeuMinus,Res.Clone);
         end
         else if not IsUM(Ex1) and IsUM(Ex2) then
         begin
           Res := TTDXExprBinary.Create(tdxebMultiply,Ex1.Clone,Ex2.Expr[0].Clone);
           Result := TTDXExprUnary.Create(tdxeuMinus,Res.Clone);
         end
         else if IsUM(Ex1) and IsUM(Ex2) then
         begin
           Result := TTDXExprBinary.Create(tdxebMultiply,Ex1.Expr[0].Clone,Ex2.Expr[0].Clone);
         end;

      end;
      tdxebPower : begin
         if Ex1.IsNumber and Ex2.IsNumber then
         begin
           V1 := Expr1.ExprValue;
           V2 := Expr2.ExprValue;
           Result := MakeLiteral(Abs(Power(V1,V2)),Fmt);
           if Power(V1,V2) < 0 then
             Result := TTDXExprUnary.Create(tdxeuMinus,Result);
         end
         else if Ex2.IsNumber and IsNumEqual(Ex2.ExprValue,1) then
         begin
           Result := Ex1.Clone;
         end
         else if Ex2.IsNumber and IsZero(Ex2.ExprValue) then
         begin
           Result := MakeLiteral(1,Fmt);
         end;
      end;
    end;

    if Result = nil then
    begin
      Result := TTDXExprBinary.Create(FBinaryType,Expr1.ReducedExpr(Fmt),Expr2.ReducedExpr(Fmt));
    end;

  finally
    Ex1.Free;
    Ex2.Free;
    res.Free;
  end;

end;

function TTDXExprBinary.SimilarExpr:TTDXExpr;
var
  Ex1,Ex2:TTDXExpr;
begin
  case FBinaryType of
    tdxebDivide : begin
       Ex1 := FExpr1.SimilarExpr;
       Ex2 := FExpr2.SimilarExpr;
       try
         //See if item on right of divide Is already an exponent
         //If it Is then check the right side of the exponent for a literal
         //If it Is then multiply the literal by -1;
         if (Ex2 Is TTDXExprBinary) and (TTDXExprBinary(Ex2).BinaryType = tdxebPower) and
            (TTDXExprBinary(Ex2).Expr2 Is TTDXExprLiteral) then
         begin
           TTDXExprLiteral(TTDXExprBinary(Ex2).Expr2).FExprValue :=
             -TTDXExprLiteral(TTDXExprBinary(Ex2).Expr2).FExprValue;
           //Don't do multiply if numerator Is 1
           if (Ex1 Is TTDXExprLiteral) and (Ex1.ExprValue = 1) then
             Result := Ex2.SimilarExpr
           else
             Result := TTDXExprBinary.Create(tdxebMultiply,Ex1.SimilarExpr,Ex2.SimilarExpr);
         end
         else
         begin
           // This will return 1/-2 in a Similar way to -1/2
           if (FExpr2 Is TTDXExprUnary) then
           begin
             Result := TTDXExprBinary.Create(tdxebMultiply,TTDXExprLiteral.Create(-1),
               TTDXExprBinary.Create(tdxebPower,TTDXExprUnary(FExpr2).Expr.SimilarExpr,
                 TTDXExprBinary.Create(tdxebMultiply,TTDXExprLiteral.Create(-1),
                   TTDXExprLiteral.Create(1))));
           end
           else
           begin
             Result :=  TTDXExprBinary.Create(tdxebPower,Ex2.SimilarExpr,
                TTDXExprBinary.Create(tdxebMultiply,TTDXExprLiteral.Create(-1),
                   TTDXExprLiteral.Create(1)));
           end;

           //As long as Ex1 Is not a 1 then do multiply
           if not ((Ex1 Is TTDXExprLiteral) and (Ex1.ExprValue = 1)) then
             Result := TTDXExprBinary.Create(tdxebMultiply,Ex1.SimilarExpr,Result);
         end;
       finally
         Ex1.Free;
         Ex2.Free;
       end;
      end;
    tdxebSubtract :  begin
       Ex1 := FExpr2.SimilarExpr;
       try
         if (Ex1 Is TTDXExprLiteral) then
         begin
           TTDXExprLiteral(Ex1).FExprValue := - TTDXExprLiteral(Ex1).FExprValue;
           Result := TTDXExprBinary.Create(tdxebAdd,FExpr1.SimilarExpr,Ex1.SimilarExpr);
         end
         else
           Result := TTDXExprBinary.Create(tdxebAdd,FExpr1.SimilarExpr,
              TTDXExprBinary.Create(tdxebMultiply,TTDXExprLiteral.Create(-1),Ex1.SimilarExpr));
       finally
         Ex1.Free;
       end;
    end;
    tdxebMultiply : begin
         Ex1 := FExpr1.SimilarExpr;
         try
           if (Ex1 Is TTDXExprLiteral) and (Ex1.ExprValue = 1) then
           begin
             Result := FExpr2.SimilarExpr;
             Exit;
           end;
         finally
           Ex1.Free;
         end;

         Ex1 := FExpr2.SimilarExpr;
         try
           if (Ex1 Is TTDXExprLiteral) and (Ex1.ExprValue = 1) then
           begin
             Result := FExpr1.SimilarExpr;
             Exit;
           end;
         finally
           Ex1.Free;
         end;

         Result := TTDXExprBinary.Create(FBinaryType,FExpr1.SimilarExpr,FExpr2.SimilarExpr);

       end;
  else
    Result := TTDXExprBinary.Create(FBinaryType,FExpr1.SimilarExpr,FExpr2.SimilarExpr);
  end;
end;

function TTDXExprBinary.GetArrayValue(V1,V2:TTDXExprValue):TTDXExprValue;
var
  Hi1,Hi2 : Integer;
  T1,T2 : TTDXExprValue;
  HiR : Integer;
  I : INteger;
begin

  if VarIsArray(V1) then
    Hi1 := VarArrayHighBound(V1,1)
  else
    Hi1 := -1;

  if VarIsArray(V2) then
    Hi2 := VarArrayHighBound(V2,1)
  else
    Hi2 := -1;

  HiR := Max(Hi1,Hi2);
  Result := VarArrayCreate([0,HiR],varVariant);

  for I := 0 to HiR do
  begin
    if Hi1 < 0 then
      T1 := V1
    else if I <= Hi1 then
      T1 := V1[I]
    else
      T1 := 0;

    if Hi2 < 0 then
      T2 := V2
    else if I <= Hi2 then
      T2 := V2[I]
    else
      T2 := 0;
    case FBinaryType of
      tdxebAdd:  Result[I] := T1+T2;
      tdxebSubtract:  Result[I] := T1-T2;
      tdxebMultiply:  Result[I] := T1*T2;
      tdxebDivide:    Result[I] := T1/T2;
      tdxebPower:     Result[I] := VarComplexPower(T1,T2);
      tdxebMod:       Result[I] := T1 MOD T2;
      tdxebNRoot:     begin
         Result[I] := VarComplexPower(T1,1/T2);
        end;
      tdxebAnd:       Result[I] := T1 and T2;
      tdxebOr:        Result[I] := T1 or T2;
    else
      raise Exception.Create('invalid floating point operation');
    end;

    Result[I] := VarComplexSimplify(Result[I]);

  end;

end;

function TTDXExprBinary.GetExprValue:TTDXExprValue;
var
  V1,V2 : TTDXExprValue;
begin

  V1 := FExpr1.ExprValue;
  V2 := FExpr2.ExprValue;

  //Check for variant arrays
  if VarIsArray(V1) or VarisArray(V2) then
    Result := GetArrayValue(V1,V2)
  else
  begin
    case FBinaryType of
      tdxebAdd:       Result := V1+V2;
      tdxebSubtract:  Result := V1-V2;
      tdxebMultiply:  Result := V1*V2;
      tdxebDivide:    Result := V1/V2;
      tdxebPower:     Result := VarComplexPower(V1,V2);
      tdxebMod:       Result := V1 MOD V2;
      tdxebNRoot:     begin
          Result := VarComplexPower(V1,1/V2);
        end;
      tdxebAnd:       Result := V1 and V2;
      tdxebOr:        Result := V1 or V2;
    else
      raise Exception.Create('invalid floating point operation');
    end;
    Result := VarComplexSimplify(Result);
  end;
end;

procedure TTDXExprBinary.SetExpr2(Value:TTDXExpr);
begin
  FExpr2.Free;
  FExpr2 := Value;
end;



function TTDXExprBinary.GetEqText(ExprForm:boolean = false):String;
var
  T1,T2 : String;

  function ReduceParen(Expr:TTDXExpr):String;
  begin
    if Expr Is TTDXExprParen then
      Result := (Expr as TTDXExprParen).Expr.GetEqText(ExprForm)
    else
      Result := Expr.GetEqText(ExprForm);
  end;

  //This routine will remove the parens from a divide.
  function RemoveDivParen(Expr:TTDXExpr):String;
  begin
    if (Expr Is TTDXExprParen) and (TTDXExprParen(Expr).Expr Is TTDXExprBinary)
       and (TTDXExprBinary(TTDXExprParen(Expr).Expr).BinaryType = tdxebDivide) then
      Result := TTDXExprParen(Expr).Expr.GetEqText(ExprForm)
    else
      Result := Expr.GetEqText(ExprForm);
  end;

  function FormatPlus:String;
  begin
    if (Length(T2) > 0) and (T2[1] = '-') then
      T2 := AddParen(T2);
    Result := T1 + '+' + T2;
  end;

  function FormatMinus:String;
  var
    LParen,RParen: boolean;
  begin
    LParen := false;
    RParen := false;

// Subtract should only control parens on right side.

    if (FExpr2 Is TTDXExprBinary) and (TTDXExprBinary(FExpr2).BinaryType in [tdxebAdd,tdxebSubtract]) then
      RParen := true;

    if LParen then
      T1 := AddParen(T1);
    if RParen or ((Length(T2) > 0) and (T2[1] = '-')) then
      T2 := AddParen(T2);
    Result := T1 + '-' + T2;
  end;

  function FormatAnd:String;
  var
    LParen,RParen: boolean;
  begin
    LParen := false;
    RParen := false;

    if (FExpr1 Is TTDXExprBinary) and (TTDXExprBinary(FExpr1).BinaryType in [tdxebOr]) then
      LParen := true;

    if (FExpr2 Is TTDXExprBinary) and (TTDXExprBinary(FExpr2).BinaryType in [tdxebOr]) then
      RParen := true;

    if LParen then
      T1 := AddParen(T1);
    if RParen then
      T2 := AddParen(T2);
    Result := T1 + ' and ' + T2;
  end;



  function FormatMod:String;
  var
    LParen,RParen: boolean;
  begin
    LParen := false;
    RParen := false;

    if (FExpr1 Is TTDXExprBinary) and (TTDXExprBinary(FExpr1).BinaryType in [tdxebAdd,tdxebSubtract]) then
      LParen := true;

    if (FExpr2 Is TTDXExprBinary) and (TTDXExprBinary(FExpr2).BinaryType in [tdxebAdd,tdxebSubtract]) then
      RParen := true;

    if LParen then
      T1 := AddParen(T1);
    if RParen then
      T2 := AddParen(T2);
    Result := T1 + ' mod ' + T2;
  end;


  function FormatPower:String;
  var
    TT : String;
  begin
    TT := '';
    if FAltTrigPowerForm and (FExpr1 Is TTDXExprFunction) then
    begin
      T1 := TDXExprFunctionTypeNames[(FExpr1 as TTDXExprFunction).FuncType];
      TT := (FExpr1 as TTDXExprFunction).GetParamEqText(ExprForm);
    end
    else if (not FExpr1.IsTerm) or (FExpr1 Is TTDXExprFunction) then
      T1 := AddParen(T1)
    else if (FExpr1 Is TTDXExprBinary) and (TTDXExprBinary(FExpr1).BinaryType = tdxebDivide)   then
      T1 := AddParen(T1);

    if not (FExpr2.IsTerm  or (FExpr2 Is TTDXExprUnary)) then
      T2 := AddParen(T2);
    Result := T1 + '@SUP{' + T2 + '}' + TT;
  end;

  function FormatDiv:String;
  begin
    if (Expr1 Is TTDXExprUnary) then
    begin
      if (Expr2 Is TTDXExprUnary) then
        Result := '@DIV{'+ReduceParen(TTDXExprUnary(Expr1).Expr)+';'+
                  ReduceParen(TTDXExprUnary(Expr2).Expr)+'}'
      else
        Result := '-@DIV{'+ReduceParen(TTDXExprUnary(Expr1).Expr)+';'+
                  ReduceParen(FExpr2)+'}'
    end
    else if (Expr2 Is TTDXExprUnary) then
        Result := '-@DIV{'+ReduceParen(FExpr1)+';'+
                  ReduceParen(TTDXExprUnary(Expr2).Expr)+'}'
    else
      Result := '@DIV{'+ReduceParen(FExpr1)+';'+REduceParen(FExpr2)+'}';
  end;

  function FormatMult:String;
  type
    TMultType = (mtUnary,mtLiteral,mtSymbolic,mtDivide,mtPower, mtMultiply, mtFunction, mtOther, mtSpecialSymbol);
  var
    LParen,RParen,ShowDot : boolean;
    LeftType,RightType : TMultType;
  const
    ShowDots : array[TMultType,TMultType] of boolean = (
       {left/right   mtUnary, mtLiteral, mtSymbolic, mtDivide, mtPower, mtMultiply, mtFunction, mtOther, mtSpecialSymbol
       {mtUnary}    (  true,   true,     false,      false,    true,    false,     false,       true,     false),
       {mtLiteral}  (  true,   true,     false,      true,     true,    true,      false,       true,     false ),
       {mtSymbolic} (  true,   true,     false,      false,    true,    false,     false,       true,     false ),
       {mtDivide}   (  true,   true,     false,      true,     true,    true,      false,       true,     false ),
       {mtPower}    (  true,   true,     true,       true,     true,    true,      false,       true,     false ),
       {mtMutliply} (  true,   true,     false,      true,     true,    true,      false,       true,     false ),
       {mtFunction} (  true,   true,     true,       true,     true,    true,      false,       true,     false ),
       {mtOther}    (  true,   true,     true,       true,     true,    true,      true,        true,     false ),
   {mtSpecialSymbol}( false,  false,    false,      false,    false,   false,     false,       false,     false ));

    function GetType(Expr:TTDXExpr):TMultType;
    begin
      if not Assigned(Expr)then
      begin
        Result := mtOther;
        Exit;
      end;

      if Expr Is TTDXExprUnary then
        Result := mtUnary
      else if Expr Is TTDXExprLiteral then
      begin
        if Expr.IsNumber then
          Result := mtLiteral
        else
          Result := mtSymbolic    //pi e
      end
      else if Expr Is TTDXExprSymbolicVar then
      begin
        if TTDXExprSymbolicVar(Expr).IsSpecialSymbol then
           Result := mtSpecialSymbol
        else
           Result := mtSymbolic
      end
      else if (Expr Is TTDXExprBinary) and (TTDXExprBinary(Expr).BinaryType = tdxebDivide) then
        Result := mtDivide
      else if (Expr Is TTDXExprBinary) and (TTDXExprBinary(Expr).BinaryType = tdxebPower) then
        Result := mtPower
      else if (Expr Is TTDXExprBinary) and (TTDXExprBinary(Expr).BinaryType = tdxebMultiply) then
        Result := mtMultiply
      else if (Expr Is TTDXExprFunction) then
        Result := mtFunction
      else
        Result := mtOther;
    end;

    function RightMostNode(Expr:TTDXExpr):TTDXExpr;
    begin
      Result := Expr;
      while Assigned(Expr) and (Expr Is TTDXExprBinary) do
      begin
        Expr := TTDXExprBinary(Expr).Expr[1];
        if Assigned(Expr) then
          Result := Expr;
      end;
    end;

    function LeftMostNode(Expr:TTDXExpr):TTDXExpr;
    begin
      Result := Expr;
      while Assigned(Expr) and (Expr Is TTDXExprBinary) do
      begin
        Expr := TTDXExprBinary(Expr).Expr[0];
        if Assigned(Expr) then
          Result := Expr;
      end;
    end;


  begin
    LParen := false;
    RParen := false;

    if (FExpr1 Is TTDXExprBinary) and (TTDXExprBinary(FExpr1).BinaryType in [tdxebAdd,tdxebSubtract]) then
      LParen := true;

    if (FExpr2 Is TTDXExprBinary) and (TTDXExprBinary(FExpr2).BinaryType in [tdxebAdd,tdxebSubtract]) then
      RParen := true;

    if LParen and (FExpr2 Is TTDXExprLiteral) then
      RParen := true;

    LeftType := GetType(FExpr1);
    RightType := GetType(FExpr2);

    //**MAD** 11/18/04 - If element on the left Is a binary structure that winds up having
    //a function as the rightmost element, then we need to enclose the whole thing in parens if the
    //Expression to the right has a paren.
    if (GetType(RightMostNode(FExpr1)) = mtFunction) and (Copy(T2,1,1) = '(') then
      LParen := true;

    //6/16/03
    //This Is a piece of coded added to fix the 3*w^2*z^3 scenario.  If the item on the right
    //is a power or Multiply type.  We should actually consider the type of element on the
    //left of this node.
    if (RightType in [mtPower,mtMultiply]) and (FExpr2 Is TTDXExprBinary) then
         RightType := GetType(TTDXExprBinary(FExpr2).Expr1);


    if (LParen and RParen) or
       (LParen and  ( FExpr2.IsTerm or (RightType = mtMultiply))) or
       (RParen and ((FExpr1.IsTerm and not (LeftType = mtFunction)) or (LeftType = mtMultiply))) or
       (StrStCopyL(T2,0,1) = '(') then     
      ShowDot := false
    else
    begin
      ShowDot := ShowDots[LeftType,RightType];
    end;

    if (FExpr1 Is TTDXExprUnary) and not ShowDot
    and (GetType(TTDXExprUnary(FExpr1).Expr) <> mtLiteral) then
       ShowDot := True;

    if LParen then
      T1 := AddParen(T1);
    if RParen then
      T2 := AddParen(T2);
    Result := T1;
    if ShowDot or (FOpName <> '') then
    begin
      if FOpName <> '' then
        Result := Result + FOpName
      else
        REsult := Result +  '*';
    end;
    Result := Result + T2;
  end;

begin
  T1 := FExpr1.GetEqText(ExprForm);
  T2 := FExpr2.GetEqText(ExprForm);
  case FBinaryType of
    tdxebAdd:       Result := FormatPlus;
    tdxebSubtract:  Result := FormatMinus;
    tdxebMultiply:  Result := FormatMult;
    tdxebDivide:    Result := FOrmatDiv;
    tdxebPower:     Result := FormatPower;
    tdxebMod:       Result := FormatMod;
    tdxebNRoot:     Result := '@RT['+ReduceParen(FExpr1)+';'+ReduceParen(FExpr2)+'}';
    tdxebAnd:       Result := FormatAnd;
    tdxebOr:        Result := T1+' or '+T2;
  else
    Result := T1 + '?' + T2;
  end;
end;

function TTDXExprBinary.IsNumericFraction:boolean;
begin
  Result := (FBinaryType = tdxebDivide) and FExpr1.IsInteger and FExpr2.IsInteger;
end;

function TTDXExprBinary.IsSimplifiedFraction:boolean;
begin
  Result := false;
  if (FBinaryType = tdxebDivide) and FExpr1.IsInteger and FExpr2.IsInteger then
  begin
    if GCF(FExpr1.ExprValue,FExpr2.ExprValue) = 1 then
      Result := true;
  end;
end;


function TTDXExprBinary.IsFactFraction:boolean;
begin
  Result := (FBinaryType = tdxebDivide) and FExpr1.IsTerm and FExpr2.IsTerm;
end;


function TTDXExprBinary.IsNumericMult:boolean;
begin
  Result := (FBinaryType = tdxebMultiply) and FExpr1.IsNumber and FExpr2.IsNumber;
end;

function TTDXExprBinary.IsValidCommaNumber: boolean;
begin
  Result := FExpr1.IsValidCommaNumber and FExpr2.IsValidCommaNumber;
end;


function TTDXExprBinary.FindDivide:TTDXExprBinary;
begin
  if FBinaryType = tdxebDivide then
    Result := Self
  else
  begin
    Result := FExpr1.FindDivide;
    if Result = nil then
      Result := FExpr2.FindDivide;
  end;
end;

function TTDXExprBinary.FindOrderedPair:TTDXExprOrderedPair;
begin
  Result := FExpr1.FindOrderedPair;
  if Result = nil then
    Result := FExpr2.FindOrderedPair;
end;



Destructor TTDXExprBinary.Destroy;
begin
  inherited Destroy;
  FExpr1.Free;
  FExpr2.Free;
end;

function TTDXExprBinary.IsTerm:boolean;
begin
  (*Added tdxebDivide since they don't want parens with -@DIV{A;B} 3/18/03*)
  (*Added tdexMultiply - naturally it Is "term"/required PM - DV 1/03/07*)
  (*DV 8/21/07 removing tdxexMultiply - it Is right not to have it - otherwise leadds to (7x)^2 rendered as 7x^2*)
  Result := (BinaryType = tdxebPower) or (BinaryType = tdxebDivide);
end;

function TTDXExprBinary.GetToString:String;
begin
  Result := TDXExprBinaryTypeNames[FBinaryType]+'['+FExpr1.ToString+','+FExpr2.ToString+']';
end;

function TTDXExprBinary.IsConstant:boolean;
begin
  Result := FExpr1.IsConstant and FExpr2.IsConstant;
end;

function TTDXExprBinary.IsEqual(Expr:TTDXExpr):boolean;
begin
  if (Expr Is TTDXExprBinary) and (TTDXExprBinary(Expr).BinaryType = BinaryType) then
    Result := TTDXExprBinary(Expr).FExpr1.IsEqual(FExpr1) and
              TTDXExprBinary(Expr).FExpr2.IsEqual(FExpr2)
  else if (BinaryType = tdxebDivide) and (Expr Is TTDXExprFunction) then
    Result := TTDXExprFunction(Expr).IsEqual(Self)
  else
    Result := false;
end;


function TTDXExprBinary.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;

  procedure BuildAddList(Expr:TTDXExpr; AList:TList);
  begin
    if (Expr Is TTDXExprBinary) and (TTDXExprBinary(Expr).BinaryType=tdxebAdd) then
    begin
      BuildAddList(TTDXExprBinary(Expr).Expr1,AList);
      BuildAddList(TTDXExprBinary(Expr).Expr2,AList)
    end
    else
    begin
      if not((Expr Is TTDXExprLiteral) and (Expr.ExprValue = 0)) then
        AList.Add(Expr);
    end;
  end;




  procedure BuildMultList(Expr:TTDXExpr; AList:TList);
  begin
    if (Expr Is TTDXExprBinary) and (TTDXExprBinary(Expr).BinaryType=tdxebMultiply) then
    begin
      BuildMultList(TTDXExprBinary(Expr).Expr1,AList);
      BuildMultList(TTDXExprBinary(Expr).Expr2,AList)
    end
    else
    begin
      (* I'm removing the =1 check but I think
         it might Break other stuff. *)
      //if not((Expr Is TTDXExprLiteral) and (Expr.ExprValue = 1)) then
        AList.Add(Expr);
    end;

  end;

  function ReduceMultList(AList:TList): Double;
  var
   I : Integer;
   Expr : TTDXExpr;
   ItemDeleted : boolean;
  begin
    I := 0;
    Result := 1.0;
    while I < AList.Count do
    begin
      ItemDeleted := false;
      Expr := TTDXExpr(AList[I]);
      if Expr.IsConstant then
      begin
        try
          Result := Result * Expr.GetExprValue;
          AList.Delete(I);
          ItemDeleted := true;
        except
        end;
      end;
      if not ItemDeleted then
        Inc(I);
    end;

  end;

  procedure ReduceOnes(AList:TList);
  var
   I : Integer;
   Expr : TTDXExpr;
   ItemDeleted : boolean;
   ExprLiteral : TTDXExprLiteral;
   LitValue : Integer;
  begin
    I := 0;
    LitValue := 1;
    ExprLiteral := nil;
    while I < AList.Count do
    begin
      ItemDeleted := false;
      Expr := TTDXExpr(AList[I]);
      if Expr Is TTDXExprLiteral then
      begin
        try
          if SameValue(Abs(Expr.GetExprValue),1) then
          begin
            LitValue := LitValue * Expr.GetExprValue;
            AList.Delete(I);
            ItemDeleted := true;
            //Reuse the literal to put back in the List
            ExprLiteral := TTDXExprLiteral(Expr);
          end;
        except
        end;
      end;
      if not ItemDeleted then
        Inc(I);
    end;

    if LitValue < 0 then
    begin
      ExprLiteral.FExprValue := LitValue;
      AList.Add(ExprLiteral);
    end;

  end;


  function SimilarAdd:boolean;
  var
    List1,List2 : TList;
    I,J : Integer;
    Expr1 : TTDXExpr;
    MatchFound : boolean;
  begin
    Result := false;
    List1 := TList.Create;
    List2 := TList.Create;
    try

      BuildAddList(Self,List1);
      BuildAddList(Expr,List2);

      if List1.Count <> List2.Count then
        Exit;

      for I := 0 to List1.Count-1 do
      begin
        Expr1 := TTDXExpr(List1[I]);
        MatchFound := false;
        for J := 0 to List2.Count-1 do
        begin
          if Expr1.IsSimilar(TTDXExpr(List2[J]),ReduceMult) then
          begin
            MatchFound := true;
            List2.Delete(J);
            Break;
          end;
        end;
        if not MatchFound then
          Exit;
      end;

      Result := true;

    finally
      List1.Free;
      List2.Free;
    end;
  end;

  function SimilarMult:boolean;
  var
    List1,List2 : TList;
    I,J : Integer;
    Expr1 : TTDXExpr;
    MatchFound : boolean;
    RedConst1,RedConst2 : Double;
  begin
    Result := false;
    List1 := TList.Create;
    List2 := TList.Create;
    try

      BuildMultList(Self,List1);
      BuildMultList(Expr,List2);

      //This reduction Is added to handle stuff like -2/-4 == 2/4
      ReduceOnes(List1);
      ReduceOnes(List2);

      if ReduceMult then
      begin
        RedConst1 := ReduceMultList(List1);
        RedConst2 := ReduceMultList(List2);
        if not SameValue(RedConst1,RedConst2) then
          Exit;
      end;

      if List1.Count <> List2.Count then
        Exit;

      for I := 0 to List1.Count-1 do
      begin
        Expr1 := TTDXExpr(List1[I]);
        MatchFound := false;
        for J := 0 to List2.Count-1 do
        begin
          if Expr1.IsSimilar(TTDXExpr(List2[J]),ReduceMult) then
          begin
            MatchFound := true;
            List2.Delete(J);
            Break;
          end;
        end;
        if not MatchFound then
          Exit;
      end;

      Result := true;

    finally
      List1.Free;
      List2.Free;
    end;
  end;

  function SimilarOther:boolean;
  begin
    Result := false;
    if TTDXExprBinary(Expr).BinaryType = BinaryType then
      Result := Expr1.IsSimilar(TTDXExprBinary(Expr).Expr1,ReduceMult) and
                Expr2.IsSimilar(TTDXExprBinary(Expr).Expr2,ReduceMult);
  end;

begin
  if Expr Is TTDXExprBinary then
  begin
    if BinaryType = tdxebAdd then
      Result := SimilarAdd
    else if BinaryType = tdxebMultiply then
      Result := SimilarMult
    else
      Result := SimilarOther;
  end
  else
    Result := false;
end;

function TTDXExprBinary.GetExprCount:Integer;
begin
  Result := 2;
end;

function TTDXExprBinary.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := nil; //Compiler messages cleanup 3/2/05
  case Index of
    0 : Result := FExpr1;
    1 : Result := FExpr2;
  else
    ExprOutOfBounds;
  end;
end;

procedure TTDXExprBinary.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  case Index of
    0 : begin
          FExpr1.Free;
          FExpr1 := Value;
        end;
    1 : begin
          FExpr2.Free;
          FExpr2 := Value;
        end;
  else
    ExprOutOfBounds;
  end;
end;

{****************************************************************************}
{**** TTDXExprRelational *******************************************************}
{****************************************************************************}

Constructor TTDXExprRelational.Create(AType:TTDXExprRelationalType; Expr1,Expr2:TTDXExpr);
begin
  FExpr1 := Expr1;
  FExpr2 := Expr2;
  FRelationalType := AType;
end;

function TTDXExprRelational.Clone:TTDXExpr;
begin
  Result := TTDXExprRelational.Create(FRelationalType,FExpr1.Clone,FExpr2.Clone);
end;

function TTDXExprRelational.SimilarExpr:TTDXExpr;
begin
  Result := TTDXExprRelational.Create(FRelationalType,FExpr1.SimilarExpr,FExpr2.SimilarExpr);
end;

function TTDXExprRelational.Contains(V1,V2:TTDXExprValue; EqRule:TTDXExprEqRule; Exact:boolean = false):TTDXExprValue;
var
  I,J : Integer;
  FoundMatch : boolean;
  VList : TStringList;
begin

  //both Expressions must be Lists in order for this to work.
  if not (VarIsArray(V1) and VarIsArray(V2)) then
  begin
    if Exact then
      raise Exception.Create('Need a List on each side of an ONLY operator.')
    else
      raise Exception.Create('Need a List on each side of a CONTAINS operator.');
  end;

  VList := TStringList.Create;
  try
    //Put left List in alternate List so we can remove things as we go.
    for I := VarArrayLowBound(V1,1) to VarArrayHighBound(V1,1) do
      VList.Add(V1[I]);

    //Now loop through List on right and check for existence in List on left
    for I := VarArrayLowBound(V2,1) to VarArrayHighBound(V2,1) do
    begin

      FoundMatch := false;
      for J := 0 to VList.Count-1 do
      begin
        if Equals(VList[J],V2[I],EqRule) then
        begin
          FoundMatch := true;
          VList.Delete(J);
          Break;
        end;
      end;

      if not FoundMatch then
      begin
        Result := false;
        Exit;
      end;

    end;

    if Exact then
      Result := VList.Count=0
    else
      Result := true;

  finally
    VList.Free;
  end;
end;

function TTDXExprRelational.Only(V1,V2:TTDXExprValue; EqRule:TTDXExprEqRule):TTDXExprValue;
begin
  Result := Contains(V1,V2,EqRule,true);
end;

function TTDXExprRelational.Equals(V1,V2:TTDXExprValue; EqRule:TTDXExprEqRule):TTDXExprValue;
begin
  Result := False;
end;

function TTDXExprRelational.GetExprValue:TTDXExprValue;
var
  V1,V2 : TTDXExprValue;
  EqRule : TTDXExprEqRule;
begin

  EqRule := tdxeqValue;

  if (FRelationalType in [tdxerEQ,tdxerNE,tdxerContains,tdxerOnly]) and (FExpr1.EqRule <> tdxeqValue) then
  begin
    EqRule := FExpr1.EqRule;
    V1 := FExpr1.ExprText;
    V2 := FExpr2.ExprText;
  end
  else
  begin
    V1 := FExpr1.ExprValue;
    V2 := FExpr2.ExprValue;
  end;

  //Encode any string variables inorder to normalize items into one form (I.e., '>', &gt;).

  //**MAD** Added support for arrays - 5/19/05
  V1 := VarArrayEncodeForCompare(V1);
  V2 := VarArrayEncodeForCompare(V2);

  if VarArrayIsStr(V1) or VarArrayIsStr(V2) then
  begin
    V1 := VarArrayUpperCase(V1);
    V2 := VarArrayUpperCase(V2);
  end;

  if not (FRelationalType in [tdxerContains,tdxerOnly]) then
  begin
    if IsSingleValueVarArray(V1) then
      V1:=V1[VarArrayLowBound(V1,1)];
    if IsSingleValueVarArray(V2) then
      V2:=V2[VarArrayLowBound(V2,1)];
  end;

  case FRelationalType of
    tdxerGT:       Result := V1>V2;
    tdxerGE:       Result := V1>=V2;
    tdxerLT:       Result := V1<V2;
    tdxerLE:       Result := V1<=V2;
    tdxerEQ:       Result := Equals(V1,V2,EqRule);
    tdxerNE:       Result := not Equals(V1,V2,EqRule);
    tdxerContains: Result := Contains(V1,V2,EqRule);
    tdxerOnly:     Result := Only(V1,V2,EqRule);
  else
    raise Exception.Create('invalid relational operation');
  end;
  Result := VarComplexSimplify(Result);
end;

function TTDXExprRelational.GetEqText(ExprForm:boolean = false):String;
var
  T1,T2 : String;
begin
  T1 := FExpr1.GetEqText(ExprForm);
  T2 := FExpr2.GetEqText(ExprForm);
  case FRelationalType of
    tdxerGT:       Result := T1+'>'+T2;
    tdxerGE:       Result := T1+'>='+T2;
    tdxerLT:       Result := T1+'<'+T2;
    tdxerLE:       Result := T1+'<='+T2;
    tdxerEQ:       Result := T1+'='+T2;
    tdxerNE:       Result := T1+'<>'+T2;
    tdxerContains: Result := T1+' CONTAINS '+T2;
    tdxerOnly:     Result := T1+' ONLY '+T2;
  else
    Result := T1+'?'+T2;
  end;
end;



Destructor TTDXExprRelational.Destroy;
begin
  inherited Destroy;
  FExpr1.Free;
  FExpr2.Free;
end;

function TTDXExprRelational.GetToString:String;
begin
  Result := TDXExprRelationalTypeNames[FRelationalType]+'['+FExpr1.ToString+','+FExpr2.ToString+']';
end;

function TTDXExprRelational.IsEqual(Expr:TTDXExpr):boolean;
begin
  if (Expr Is TTDXExprRelational) and (TTDXExprRelational(Expr).FRelationalType = FRelationalType) then
    Result := TTDXExprRelational(Expr).FExpr1.IsEqual(FExpr1) and
              TTDXExprRelational(Expr).FExpr2.IsEqual(FExpr2)
  else
    Result := false;
end;

function TTDXExprRelational.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;
begin
  if (Expr Is TTDXExprRelational) and (TTDXExprRelational(Expr).FRelationalType = FRelationalType) then
    Result := TTDXExprRelational(Expr).FExpr1.IsSimilar(FExpr1,ReduceMult) and
              TTDXExprRelational(Expr).FExpr2.IsSimilar(FExpr2,ReduceMult)
  else
    Result := false;
end;

function TTDXExprRelational.GetExprCount:Integer;
begin
  Result := 2;
end;

function TTDXExprRelational.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := nil; //Compiler messages cleanup 3/2/05
  case Index of
    0 : Result := FExpr1;
    1 : Result := FExpr2;
  else
    ExprOutOfBounds;
  end;
end;

procedure TTDXExprRelational.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  case Index of
    0 : begin
          FExpr1.Free;
          FExpr1 := Value;
        end;
    1 : begin
          FExpr2.Free;
          FExpr2 := Value;
        end;
  else
    ExprOutOfBounds;
  end;
end;


{****************************************************************************}
{**** TTDXExprUnary *********************************************************}
{****************************************************************************}

Constructor TTDXExprUnary.Create(AType:TTDXExprUnaryType; Expr:TTDXExpr);
begin
  FExpr := Expr;
  FUnaryType := AType;
end;

function TTDXExprUnary.Clone:TTDXExpr;
begin
  Result := TTDXExprUnary.Create(FUnaryType,FExpr.Clone);
end;

function TTDXExprUnary.SimilarExpr:TTDXExpr;
begin
    Result := TTDXExprBinary.Create(tdxebMultiply,TTDXExprLiteral.Create(-1),FExpr.SimilarExpr);
end;

function TTDXExprUnary.GetExprValue:TTDXExprValue;
var
  V : TTDXExprValue;
begin
  V := FExpr.ExprValue;
  case FUnaryType of
    tdxeuMinus:    Result := -V;
    tdxeuNot:      Result := not V;
  else
    raise Exception.Create('invalid unary operation');
  end;
  Result := VarComplexSimplify(Result);
end;

function TTDXExprUnary.GetEqText(ExprForm:boolean = false):String;
var
  T : String;

  function DoParen:String;
  begin
    //DV 8/21/07 Now changing here to get rid of wrong display -(2x).
    //2x should return IsTerm = False
    //It needs to be displayed without parens in -2x
    //and with parens in ex. (2x)^2

    if not  (FExpr.IsTerm
            or (FExpr Is TTDXExprBinary)and(TTDXExprBinary(FExpr).BinaryType = tdxebMultiply))
    then
      T := AddParen(T);
    Result := T;
  end;

begin

  //Check to see if Expression Is same as mySelf if so then
  //jump to contents for Expression in my Expression to avoid
  //double minus or not.
  if (FExpr Is TTDXExprUnary) and (TTDXExprUnary(FExpr).FUnaryType = FUnaryType) then
  begin
    Result := TTDXExprUnary(FExpr).FExpr.GetEqText(ExprForm);
    Exit;
  end;

  T := FExpr.GetEqText(ExprForm);
  case FUnaryType of
    tdxeuMinus:    Result := '-'+DoParen;
    tdxeuNot:      Result := ' not '+DoParen;
  else
    Result := '?'+T;
  end;
end;


Destructor TTDXExprUnary.Destroy;
begin
  inherited Destroy;
  FExpr.Free;
end;

function TTDXExprUnary.GetToString:String;
begin
  Result := TDXExprUnaryTypeNames[FUnaryType]+'['+FExpr.ToString+']';
end;

function TTDXExprUnary.IsNumber:boolean;
begin
  Result := FExpr.IsNumber;
end;

function TTDXExprUnary.IsConstant:boolean;
begin
  Result := FExpr.IsConstant;
end;

function TTDXExprUnary.IsInteger:boolean;
begin
  Result := FExpr.IsInteger;
end;

function TTDXExprUnary.IsEqual(Expr:TTDXExpr):boolean;
begin
  if (Expr Is TTDXExprUnary) then
    Result := TTDXExprUnary(Expr).FExpr.IsEqual(FExpr)
  else
    Result := false;
end;

function TTDXExprUnary.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;
begin
  if (Expr Is TTDXExprUnary) then
    Result := TTDXExprUnary(Expr).FExpr.IsSimilar(FExpr,ReduceMult)
  else
    Result := false;
end;


function TTDXExprUnary.IsValidCommaNumber: boolean;
begin
  Result := FExpr.IsValidCommaNumber;
end;

function TTDXExprUnary.IsNumericFraction:boolean;
begin
  Result := FExpr.IsNumericFraction;
end;

function TTDXExprUnary.IsSimplifiedFraction:boolean;
begin
  Result := FExpr.IsSimplifiedFraction;
end;

function TTDXExprUnary.IsFactFraction:boolean;
begin
  Result := FExpr.IsFactFraction;
end;


function TTDXExprUnary.IsNumericMult:boolean;
begin
  Result := FExpr.IsNumericMult;
end;

function TTDXExprUnary.IsMixedNumber:boolean;
begin
  Result := FExpr.IsMixedNumber;
end;


function TTDXExprUnary.FindDivide:TTDXExprBinary;
begin
  Result := FExpr.FindDivide;
end;

function TTDXExprUnary.FindOrderedPair:TTDXExprOrderedPair;
begin
  Result := FExpr.FindOrderedPair;
end;

function TTDXExprUnary.GetExprCount:Integer;
begin
  Result := 1;
end;

function TTDXExprUnary.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := nil; //Compiler messages cleanup 3/2/05
  if Index = 0 then
    Result := FExpr
  else
    ExprOutOfBounds;
end;

procedure TTDXExprUnary.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  if Index = 0 then
  begin
    FExpr.Free;
    FExpr := Value;
  end
  else
    ExprOutOfBounds;
end;

{****************************************************************************}
{**** TTDXExprParen*********************************************************}
{****************************************************************************}

Constructor TTDXExprParen.Create(Expr:TTDXExpr);
begin
  FExpr := Expr;
end;

function TTDXExprParen.Clone:TTDXExpr;
begin
  Result := TTDXExprParen.Create(FExpr.Clone);
end;

function TTDXExprParen.SimilarExpr:TTDXExpr;
begin
  Result := TTDXExprParen.Create(FExpr.SimilarExpr);
end;

function TTDXExprParen.IsConstant:boolean;
begin
  Result := FExpr.IsConstant;
end;


function TTDXExprParen.GetExprValue:TTDXExprValue;
begin
  Result := FExpr.ExprValue;
end;

function TTDXExprParen.GetEqText(ExprForm:boolean = false):String;
var
  T : String;
begin
  T := FExpr.GetEqText(ExprForm);
  Result := '('+T+')';
end;


Destructor TTDXExprParen.Destroy;
begin
  inherited Destroy;
  FExpr.Free;
end;

function TTDXExprParen.GetToString:String;
var
  T : String;
begin
  T := FExpr.GetEqText;
  Result := '('+T+')';
end;

function TTDXExprParen.IsEqual(Expr:TTDXExpr):boolean;
begin
  if (Expr Is TTDXExprParen) then
    Result := TTDXExprParen(Expr).FExpr.IsEqual(FExpr)
  else
    Result := false;
end;

function TTDXExprParen.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;
begin
  if (Expr Is TTDXExprParen) then
    Result := TTDXExprParen(Expr).FExpr.IsSimilar(FExpr,ReduceMult)
  else
    Result := false;
end;


function TTDXExprParen.IsValidCommaNumber: boolean;
begin
  Result := FExpr.IsValidCommaNumber;
end;

function TTDXExprParen.GetExprCount:Integer;
begin
  Result := 1;
end;

function TTDXExprParen.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := nil; //Compiler messages cleanup 3/2/05
  if Index = 0 then
    Result := FExpr
  else
    ExprOutOfBounds;
end;

procedure TTDXExprParen.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  if Index = 0 then
  begin
    FExpr.Free;
    FExpr := Value;
  end
  else
    ExprOutOfBounds;
end;


{****************************************************************************}
{**** TTDXMixedNum*********************************************************}
{****************************************************************************}

Constructor TTDXExprMixedNum.Create(ANum,ATop,ABottom:TTDXExpr);
begin
  FExprNum := ANum;
  FExprTop := ATop;
  FExprBottom := ABottom;
end;

function TTDXExprMixedNum.Clone:TTDXExpr;
begin
  Result := TTDXExprMixedNum.Create(FExprNum.Clone,FExprTop.Clone,FExprBottom.Clone);
end;

function TTDXExprMixedNum.GetExprValue:TTDXExprValue;
begin
  Result := FExprNum.ExprValue + Double(FExprTop.ExprValue)/FExprBottom.ExprValue;
end;

function TTDXExprMixedNum.GetEqText(ExprForm:boolean = false):String;
begin
  Result := '@MNUM{'+FExprNum.GetEqText(ExprForm)+';'+
       FExprTop.GetEqText(ExprForm)+';'+FExprBottom.GetEqText(ExprForm)+'}';
end;


Destructor TTDXExprMixedNum.Destroy;
begin
  inherited Destroy;
  FExprNum.Free;
  FExprTop.Free;
  FExprBottom.Free;
end;

function TTDXExprMixedNum.GetToString:String;
begin
  Result := 'Mnum['+FExprNum.GetEqText+','+FExprTop.GetEqText+','+FExprBottom.GetEqText+']';
end;

function TTDXExprMixedNum.IsEqual(Expr:TTDXExpr):boolean;
begin
  if (Expr Is TTDXExprMixedNum) then
    Result := TTDXExprMixedNum(Expr).FExprNum.IsEqual(FExprNum) and
              TTDXExprMixedNum(Expr).FExprTop.IsEqual(FExprTop) and
              TTDXExprMixedNum(Expr).FExprBottom.IsEqual(FExprBottom)
  else
    Result := false;
end;

function TTDXExprMixedNum.IsTerm:boolean;
begin
  Result := FExprNum.ExprValue >= 0;
end;

function TTDXExprMixedNum.IsNumericMult:boolean;
begin
  Result := true;
end;

function TTDXExprMixedNum.IsNumber:boolean;
begin
  Result := true;
end;

function TTDXExprMixedNum.IsConstant:boolean;
begin
  Result := true;
end;

function TTDXExprMixedNum.IsMixedNumber:boolean;
begin
  Result := true;
end;

function TTDXExprMixedNum.IsValidCommaNumber: boolean;
begin
  Result := FExprNum.IsValidCommaNumber and
            FExprTop.IsValidCommaNumber and
            FExprbottom.IsValidCommaNumber;
end;

function TTDXExprMixedNum.GetExprCount:Integer;
begin
  Result := 3;
end;

function TTDXExprMixedNum.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := nil; //Compiler messages cleanup 3/2/05
  case Index of
    0 : Result := FExprNum;
    1 : Result := FExprTop;
    2 : Result := FExprBottom;
  else
    ExprOutOfBounds;
  end;
end;

procedure TTDXExprMixedNum.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  case Index of
    0 : begin FExprNum.Free; FExprNum := Value; end;
    1 : begin FExprTop.Free; FExprTop := Value; end;
    2 : begin FExprBottom.Free; FExprBOttom := Value; end;
  else
    ExprOutOfBounds;
  end;
end;


{****************************************************************************}
{**** TTDXExprOrderedPair*********************************************************}
{****************************************************************************}

Constructor TTDXExprOrderedPair.Create(AExpr1,AExpr2:TTDXExpr);
begin
  FExpr1 := AExpr1;
  FExpr2 := AExpr2;
end;

function TTDXExprOrderedPair.Clone:TTDXExpr;
begin
  Result := TTDXExprOrderedPair.Create(FExpr1.Clone,FExpr2.Clone);
end;

function TTDXExprOrderedPair.SimilarExpr:TTDXExpr;
begin
  Result := TTDXExprOrderedPair.Create(FExpr1.SimilarExpr,FExpr2.SimilarExpr);
end;


Destructor TTDXExprOrderedPair.Destroy;
begin
  inherited Destroy;
  FExpr1.Free;
  FExpr2.Free;
end;

function TTDXExprOrderedPair.GetExprValue:TTDXExprValue;
begin
  //Not sure what to return here.  What would be cool Is a custom
  //variant that supported things like binary operations.
  Result := VarTDXOrderedPairCreate(FExpr1.ExprValue,FExpr2.ExprValue);
end;

function TTDXExprOrderedPair.GetToString:String;
begin
  Result := 'Pair['+FExpr1.ToString+','+FExpr2.ToString+']';
end;

function TTDXExprOrderedPair.GetEqText(ExprForm:boolean = false):String;
begin
  Result := '(' + FExpr1.GetEqText(ExprForm) + ',' + FExpr2.GetEqText(ExprForm) + ')';
end;

function TTDXExprOrderedPair.IsEqual(Expr:TTDXExpr):boolean;
begin
  Result := false;
  if Expr Is TTDXExprOrderedPair then
  begin
    if TTDXExprOrderedPair(Expr).FExpr1.IsEqual(FExpr1) and
       TTDXExprOrderedPair(Expr).FExpr2.IsEqual(FExpr2) then
        Result := true;
  end;
end;

function TTDXExprOrderedPair.IsSimilar(Expr:TTDXExpr; ReduceMult:boolean):boolean;
begin
  Result := false;
  if Expr Is TTDXExprOrderedPair then
  begin
    if TTDXExprOrderedPair(Expr).FExpr1.IsSimilar(FExpr1,ReduceMult) and
       TTDXExprOrderedPair(Expr).FExpr2.IsSimilar(FExpr2,ReduceMult) then
        Result := true;
  end;
end;


function TTDXExprOrderedPair.IsValidCommaNumber: boolean;
begin
  Result := false;
end;

function TTDXExprOrderedPair.IsTerm:boolean;
begin
  Result := true;
end;

function TTDXExprOrderedPair.FindDivide:TTDXExprBinary;
begin
  Result := FExpr1.FindDivide;
  if not Assigned(Result) then
    Result := FExpr2.FindDivide;
end;

function TTDXExprOrderedPair.FindOrderedPair:TTDXExprOrderedPair;
begin
  Result := Self;
end;

function TTDXExprOrderedPair.IsNumber:boolean;
begin
  Result := FExpr1.IsNumber and FExpr2.IsNumber;
end;

function TTDXExprOrderedPair.IsNumericFraction:boolean;
begin
  Result := (FExpr1.IsNumericFraction or FExpr1.IsInteger) and
            (FExpr2.IsNumericFraction or FExpr2.IsInteger);
end;

function TTDXExprOrderedPair.IsSimplifiedFraction:boolean;
begin
  Result := (FExpr1.IsSimplifiedFraction) and
            (FExpr2.IsSimplifiedFraction);
end;


function TTDXExprOrderedPair.IsFactFraction:boolean;
begin
  Result := (FExpr1.IsFactFraction or FExpr1.IsInteger) and
            (FExpr2.IsFactFraction or FExpr2.IsInteger);
end;


function TTDXExprOrderedPair.IsNumericMult:boolean;
begin
  Result := (FExpr1.IsNumericMult or FExpr1.IsNumber) and
            (FExpr2.IsNumericMult or FExpr2.IsNumber);
end;

function TTDXExprOrderedPair.GetExprCount:Integer;
begin
  Result := 2;
end;

function TTDXExprOrderedPair.GetExpr(Index:Integer):TTDXExpr;
begin
  Result := nil; //Compiler messages cleanup 3/2/05
  case Index of
    0 : Result := FExpr1;
    1 : Result := FExpr2;
  else
    ExprOutOfBounds;
  end;
end;

procedure TTDXExprOrderedPair.SetExpr(Index:Integer; Value:TTDXExpr);
begin
  case Index of
    0 : begin FExpr1.Free; FExpr1 := Value; end;
    1 : begin FExpr2.Free; FExpr2 := Value; end;
  else
    ExprOutOfBounds;
  end;
end;

function TTDXExprUnary.ReducedExpr(Fmt: String): TTDXExpr;
var
  Expr : TTDXExpr;
begin
  Expr := FExpr.ReducedExpr(Fmt);
  try
    if IsUM(Self) and IsUM(Expr) then
      Result := TTDXExprUnary(Expr).Expr.Clone
    else
      Result := TTDXExprUnary.Create(tdxeuMinus,Expr.Clone);
  finally
    Expr.Free
  end;
end;

initialization
  ONEExpr := TTDXExprLiteral.Create(1);
  MINUSONEExpr := TTDXExprLiteral.Create(-1);
finalization
  ONEExpr.Free;
  MINUSONEExpr.Free;
end.
