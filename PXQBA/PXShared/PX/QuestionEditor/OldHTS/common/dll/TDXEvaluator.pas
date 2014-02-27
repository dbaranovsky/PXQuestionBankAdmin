unit TDXEvaluator;

interface

uses TDXExprSymbolicVar,TDXExpr,TDXExprSimEq,classes,StDict,Contnrs,
  CocoBase,Windows,Variants,sysutils,Dialogs, TDXStrings;

const
//  MAX_EVALUATIONS = 5;
//After adding in negative Values, I think we need to increase this number
//to at least 8 (maybe 10).
  MAX_EVALUATIONS = 10;

  MIN_SYMBOLIC_VALUE = 1;
  MAX_SYMBOLIC_VALUE = 2;
  MAX_ERRORCOUNT = 20;

type

  TTDXAnswerRule = (arAcceptAnyForm,arAcceptSimilarForm,
     arExact,arText,arReducedNumber,arNumericOnly,arNumericFrac,
     arNumericMult,arNoEvalFrac,arNoReduceFrac,arNoDecimalEquiv);

  TTDXTextCompareOption = (tcIgnoreSpaces,tcIgnoreCase);
  TTDXTextCompareOptions = set of TTDXTextCompareOption;

  TTDXCommaOptions = (coAllowed,coNotAllowed,coRequired);

  TIPListOptions = (loNone,loList,loOrderedList);

  TTDXBadFormException = class(Exception);
  TTDXInCorrectException = class(Exception);
  TTDXTryAgainException = class(Exception);

  TTDXEvaluatorStatus = (evCorrect,evBadForm,evInCorrect,evSyntaxError,evCustomError);

const
   TDXAnswerRulesLongName : array[TTDXAnswerRule] of string = (
      'Accept any form',
      'Accept similar form',
      'Accept only specified Answer',
      'Text comparison',
      'Fully-Reduced Number, Fraction or Mixed Number',
      'Any Numeric',
      'Numeric Fraction no operations',
      'Numeric with multiplication only',
      'Single non-reduced fraction',
      'Any non-reduced fraction',
      'Do not accept decimal Equivalents');

   TDXAnswerRulesShortName : array[TTDXAnswerRule] of string = (
      'anyform','similarform',
      'exact','text','reducednum','numeric','numericfrac','numericmult','noevalfrac','noreducefrac','nodecimalequiv');


   TDXCommaOptionLongNames : array[TTDXCommaOptions] of string = (
      'Allowed','Not allowed','Required');


   TDXCommaOptionshortNames : array[TTDXCommaOptions] of string = (
      'allowed','notallowed','required');

   TDXEvaluatorStatusString : array[TTDXEvaluatorStatus] of string = (
     'Correct','Bad Form','InCorrect','Syntax Error','Error');
type
  TTDXEvalSymVar = class
  private
    FName : String;
    FValue : double;
  protected
  public
    Constructor Create(AName:String);
  end;

  TTDXEvaluator = class
  private
    //FProblem : TTDXProblem;
    FResponse : String;
    FCorrect : String;
    FCorrectAnswers : TStringList;
    FAnswerFormats : TStringList;
    FVariables : TObjectList;
    FVarNames:TStDictionary;
    FCurAnswer:Integer;
    FLastError : String;
    FAnswerRule : TTDXAnswerRule;
    FCommaOptions: TTDXCommaOptions;
    FAllowOrderedPairs : boolean;
    FAllSolutions : boolean;
    FNumericFormat : String;
    FTolerance: Double;
    FEnforceFormatting:boolean;
    FListOptions : TIPListOptions;
    FTry : Boolean;
    FTryError : Boolean;
    FListImitate : Boolean;
    procedure SymVarCreateAnswer(Sender:TTDXExpr; VarName:String);
    procedure SymVarCreateResponse(Sender:TTDXExpr; VarName:String);
    function SymVarGetValue(Sender:TTDXExpr; VarName:String):TTDXExprValue;
    function CheckSymbolic(CorrectExpr,ResponseExpr:TTDXExpr):TTDXEvaluatorStatus;
    function CheckSimilar(CorrectExpr,ResponseExpr:TTDXExpr; ReduceMult:boolean = false):TTDXEvaluatorStatus;
    procedure CheckDiagnosticMsg(CorrectExpr,ResponseExpr:TTDXExpr);
    function CheckExact(CorrectExpr,ResponseExpr:TTDXExpr):TTDXEvaluatorStatus;
    procedure ParserError (Sender : TObject; Error : TCocoError);
    function CheckAnswer(Index:Integer):TTDXEvaluatorStatus;
    function CheckAllSolutions:TTDXEvaluatorStatus;
    function CheckExpr(CorrectExpr,ResponseExpr:TTDXExpr; AnsFmt:String):TTDXEvaluatorStatus;
    function CheckExpression(CorrectExpr,ResponseExpr:TTDXExpr; AnsFmt:String):TTDXEvaluatorStatus;
    procedure SetCorrect(const Value: String);
    procedure SetListOptions(const Value: TIPListOptions);
  protected
  public
    procedure ClearSymVars;
    procedure GenerateSymbolicValues;
    
    function ParseExpression(ExprText:String;IsResponse:boolean;IsTry:boolean=False):TTDXExpr;
    Constructor Create();
    Destructor Destroy; override;
    function CheckAnswers:TTDXEvaluatorStatus;
    procedure AddCorrectAnswer(Answer:String; Fmt:String);
    property LastError:String read FLastError write FLastError;
    property Response:String read FResponse write FResponse;
    property Correct:String read FCorrect write SetCorrect;
    property AnswerRule : TTDXAnswerRule read FAnswerRule write FAnswerRule;
    property CommaOptions:TTDXCommaOptions read FCommaOptions write FCommaOptions;
    property AllowOrderedPairs:boolean read FAllowOrderedPairs write FAllowOrderedPairs;
    property AllSolutions:boolean read FAllSolutions write FAllSolutions;
    property NumericFormat:string read FNumericFormat write FNumericFOrmat;
    property CorrectAnswers:TStringList read FCorrectAnswers;
    property AnswerFormats:TStringList read FAnswerFormats;
    property Tolerance:Double read FTolerance write FTolerance;
    property EnforceFormatting:boolean read FEnforceFormatting write FEnforceFormatting;
    property ListOptions:TIPListOptions read FListOptions write SetListOptions;
    property TryError : Boolean read FTryError;
  end;


implementation

uses TDXExprParserX,Math,VarCmplx, IPMath, IPStrUtils,
  IPEqTextParser, IPEqNode;

Constructor TTDXEvalSymVar.Create(AName:String);
begin
  FName := AName;
end;

Constructor TTDXEvaluator.Create();
begin
  FCorrectAnswers := TStringList.Create;
  FAnswerFormats := TStringList.Create;
  FVariables := TObjectList.Create;
  FVarNames := TStDictionary.Create(11);
  FCurAnswer := -1;
end;

Destructor TTDXEvaluator.Destroy;
begin
  inherited Destroy;
  FCorrectAnswers.Free;
  FAnswerFormats.Free;
  FVariables.Free;
  FVarNames.Free;
end;

procedure TTDXEvaluator.AddCorrectAnswer(Answer:String; Fmt:String);
begin
  FCorrectAnswers.Add(Answer);
  FAnswerFormats.Add(Fmt);
end;

procedure TTDXEvaluator.GenerateSymbolicValues;
var
  V : TTDXEvalSymVar;
  I : Integer;
  RValue : Double;
begin
  for I := 0 to FVariables.Count-1 do
  begin
    V := TTDXEvalSymVar(FVariables[I]);
//    V.FValue := Random*(MAX_SYMBOLIC_VALUE-MIN_SYMBOLIC_VALUE)+MIN_SYMBOLIC_VALUE;
//We need to get negative Values so absolute Value Answers will work ok.
//Let calcaulate a random Value between 0 and 2.  If the Value Is < 1 then alter it to be in the range
//of -2 to -1.  **MAD**11/11/05
    RValue := Random*2;
    if ((RValue < 1) and (Pos('!',FCorrect)=0) and (Pos('!',FResponse)=0)) then
    begin
     RValue := RValue - 2;
    end;
    V.FValue := RValue;
  end;
end;

{ This routine will return an Expression tree for a give piece
  of text. It Is up to the caller to free it. }
function TTDXEvaluator.ParseExpression(ExprText:String; IsResponse:boolean;IsTry:boolean=False):TTDXExpr;
var
  Parser:TTDXExprParserX;
  Strm : TStringStream;
begin
  FTry := IsTry;
  if IsTry then FTryError := False;

  Parser := TTDXExprParserX.Create(nil);
  Parser.PreParse(ExprText);
  Strm := TStringStream.Create(ExprText);
  Result := nil;
  if IsResponse then
    Parser.OnSymVarCreate := SymVarCreateResponse
  else
    Parser.OnSymVarCreate := SymVarCreateAnswer;
  Parser.OnSymVarGetValue := SymVarGetValue;
  Parser.OnError := ParserError;
  try
    try
      Parser.SourceStream.LoadFromStream(Strm);
      Parser.Execute;
      Result := Parser.RootNode;
    Except
      on Ex:Exception do
      if IsTry then
        FTryError := True
      else
        raise Exception.CreateFmt('Error parsing Expression: %s'#13#10'%s',[ExprText,Ex.Message]);
    end;
  finally
    Strm.Free;
    Parser.Free;
  end;
end;


function TTDXEvaluator.CheckAllSolutions:TTDXEvaluatorStatus;
var
  CorrectExpr:TTDXExpr;
  ResponseExpr:TTDXExpr;
  RIndex,CIndex : Integer;
  CorrectAnswers : TStringList;
  AnsFmt : TStringList;
  AnswerFound : boolean;
begin
  Result := evInCorrect;
  FLastError := '';
  FCurAnswer := 0;


  ClearSymVars;
  CorrectExpr := nil;
  ResponseExpr := nil;
  CorrectAnswers := TStringList.Create;
  AnsFmt := TStringList.Create;
  CorrectAnswers.Assign(FCorrectAnswers);
  AnsFmt.Assign(FAnswerFormats);
  try
    try
      //pass false.  If you pass true then symblic Variables will only be Valid
      //if they Exist in the Answer.  Since we haven't yet parsed Each Answer for get it.
      if FlistImitate then
        ResponseExpr := ParseExpression(FResponse,false)
      else
        ResponseExpr := ParseExpression('['+FResponse+ ']',false);

      //If not list type then get out.
      if not (ResponseExpr Is TTDXExprList) then
        Exit;

      //Now check number of Answers.
      if TTDXExprList(ResponseExpr).ExprCount <> FCorrectAnswers.Count then
        Exit;


      for RIndex := 0 to TTDXExprList(ResponseExpr).ExprCount-1 do
      begin
        AnswerFound := false;

        if FListOptions = loOrderedList then
        begin
          if Assigned(CorrectExpr) then
            FreeAndNil(CorrectExpr);
          CorrectExpr := ParseExpression(CorrectAnswers[RIndex],false);
          Result := CheckExpr(CorrectExpr,TTDXExprList(ResponseExpr).Expr[RIndex],AnsFmt[RIndex]);
          if Result = evCorrect then
          begin
            AnswerFound := true;
          end;
        end
        else
        for CIndex := 0 to CorrectAnswers.Count-1 do
        begin
          if Assigned(CorrectExpr) then
            FreeAndNil(CorrectExpr);
          CorrectExpr := ParseExpression(CorrectAnswers[CIndex],false);
          Result := CheckExpr(CorrectExpr,TTDXExprList(ResponseExpr).Expr[RIndex],AnsFmt[CIndex]);
          if Result = evCorrect then
          begin
            AnswerFound := true;
            CorrectAnswers.Delete(CIndex);
            AnsFmt.Delete(CIndex);
            break;
          end;
        end;

        if not AnswerFound then
        begin
          Result := evInCorrect;
          Exit;
        end;

      end;

    Except
      on Ex:TTDXBadFormException do
      begin
        Result := evBadForm;
        FLastError := Ex.Message;
      end;
      on Ex:TTDXInCorrectException do
      begin
        Result := evCustomError;
        FLastError := Ex.Message;
      end;
      on Ex:Exception do
      begin
        Result := evInCorrect;
        FLastError := Ex.Message;
      end;
    end;
  finally
    AnsFmt.Free;
    CorrectAnswers.Free;
    CorrectExpr.Free;
    ResponseExpr.Free;
  end;
end;

function TTDXEvaluator.CheckAnswers:TTDXEvaluatorStatus;
var
  I : Integer;
  W : Word;
begin

  W := Get8087CW;
  Set8087CW($1372);

  try
    if FAllSolutions and (FCorrectAnswers.Count > 1) then
    begin
      Result := CheckAllSolutions;
      Exit;
    end;

    Result := evCorrect;
    for I := 0 to FCorrectAnswers.Count-1 do
    begin
      Result := CheckAnswer(I);
      if Result = evCorrect then
        Exit;
    end;
  finally
    Set8087CW(W);
  end;
end;

function TTDXEvaluator.CheckExpr(CorrectExpr,ResponseExpr:TTDXExpr;AnsFmt:String):TTDXEvaluatorStatus;
var
  VFmt : String;
  V    : Double;
  FInfo : TIPFormatInfo;
  Options:TIPInputOptions;
const
  EqCurrencyTypes : array[TCurrencyType] of TIPEqCurrency = (dollar,pound,euro,yen);

begin

  if (CorrectExpr Is TTDXExprOrderedPair) and not (ResponseExpr Is TTDXExprOrderedPair) then
    raise TTDXInCorrectException.Create(tdxstOrderedPair);

  //Add a brute force check for -0 (or -0.0)
  try
    if (ResponseExpr Is TTDXExprUnary) and TTDXExprUnary(ResponseExpr).IsNumber
      and (TTDXExprUnary(ResponseExpr).UnaryType = tdxeuMinus) then
      begin
        V := ResponseExpr.ExprValue;
        if V = 0 then
        begin
          Result := evInCorrect;
          Exit;
        end;
      end;
  Except
  end;

  //First perform numerical check. Use monte-carlo if the are symbolic Variables.Self
    if FVariables.Count > 0 then
      Result := CheckSymbolic(CorrectExpr,ResponseExpr)
    else
      Result := CheckExpression(CorrectExpr,ResponseExpr,AnsFmt);

  //Get out if result Is not Correct
  if Result <> evCorrect then
    Exit;

  //Result Is numerically Correct so now check Answer rule specific stuff.

  case FAnswerRule of
    arNumericOnly : begin
      if not ResponseExpr.IsNumber then
        raise TTDXBadFormException.Create(tdxstNumOnly);

      if ResponseExpr.IsMixedNumber then
        raise TTDXBadFormException.Create(tdxstNoMixedNum);

      //Check user response against formatting supplied.
      if (AnsFmt <> '') then
      begin
        FInfo := IPGetFormatInfo(AnsFmt);

        if FInfo.negparen then
        begin
          Options := [IoAllowNegParens];
          if FInfo.Comma then
            Include(Options,ioAllowCommaNumber);
          if FInfo.Dollar then
            Include(Options,ioAllowDollar);

          if not IPInputToFloat(ResponseExpr.ExprValue, V, EqCurrencyTypes[currDollar], Options) then
          begin
            raise TTDXBadFormException.Create(tdxstNegParen);
          end;

        end;

        if FEnforceFormatting then
        begin
          try
            V := ResponseExpr.ExprValue;
            VFmt := IPFormatFLoat(V,FInfo);

            if FInfo.Dollar then
            begin
              if Pos(CURRENCY_SYMBOLS[currDollar],ResponseExpr.ExprValue) = 0 then
                raise TTDXBadFormException.Create(tdxstDollar);
            end;

            if not SameValue(IPStrToFloat(VFmt),V,1e-9) then
              raise TTDXInCorrectException.Create(tdxstNumRound);
          Except
            on TTDXBadFormException do
               raise;
            on Exception do
              raise TTDXInCorrectException.Create(tdxstNumRound);
          end;
        end;
      end;
    end;
    arNumericFrac : begin
      if not ResponseExpr.IsNumericFraction then
        raise TTDXBadFormException.Create(tdxstNumFrac);
    end;
    arNumericMult : begin
      if not (ResponseExpr.IsNumericMult or ResponseExpr.IsNumber) then
        raise TTDXBadFormException.Create(tdxstNumMult);
    end;
  end;

  if not AllowOrderedPairs and (ResponseExpr.FindOrderedPair <> nil) then
    raise Exception.Create('ordered pair not allowed.');

  if FAnswerRule in [arExact,arAcceptSimilarForm] then
  begin
    case FAnswerRule of
      arExact : Result := CheckExact(CorrectExpr,ResponseExpr);
      arAcceptSImilarForm : Result := CheckSimilar(CorrectExpr,ResponseExpr);
    end;
    if Result <> evCorrect then
      CheckDiagnosticMsg(CorrectExpr,ResponseExpr);
  end;


end;

procedure TTDXEvaluator.CheckDiagnosticMsg(CorrectExpr,ResponseExpr:TTDXExpr);
begin

  if CorrectExpr.IsMixedNumber then
    raise TTDXBadFormException.Create(tdxstMixedNum);

  if CorrectExpr.IsNumber then
    raise TTDXBadFormException.Create(tdxstNumOnly);

  if CorrectExpr.IsSimplifiedFraction then
    raise TTDXBadFormException.Create(tdxstSimilarSimplifiedFrac);

  if CorrectExpr.IsNumericFraction and not ResponseExpr.IsNumericFraction then
    raise TTDXBadFormException.Create(tdxstSimilarNumFrac);

  if CorrectExpr.IsFactFraction and not ResponseExpr.IsFactFraction then
    raise TTDXBadFormException.Create(tdxstSimilarFactFrac);

  if CorrectExpr.IsNumericMult and not ResponseExpr.IsNumericMult then
    raise TTDXBadFormException.Create(tdxstSimilarNumMult);


end;


function TTDXEvaluator.CheckAnswer(Index:Integer):TTDXEvaluatorStatus;
var
  CorrectExpr,Exc1,Exc2:TTDXExpr;
  ResponseExpr,Exr1,Exr2:TTDXExpr;
begin
  FLastError := '';
  FCurAnswer := Index;
  ClearSymVars;
  CorrectExpr := nil;
  ResponseExpr := nil;
  try
    //Parse both Expressions first
    try
      CorrectExpr := ParseExpression(FCorrectAnswers[Index],false);
      ResponseExpr := ParseExpression(FResponse,true);

    Except
      on Ex:Exception do
      begin
        if Assigned(CorrectExpr) and (CorrectExpr Is TTDXExprOrderedPair) and not Assigned(ResponseExpr) then
        begin
          FLastError := tdxstOrderedPair;
          Result := evCustomError;
        end
        else
        begin
          Result := evSyntaxError;
          FLastError := Ex.Message;
        end;
        Exit;
      end;
    end;

    //problem with ordered pairs with numbers and infinity. Let's bypass
      Exc1 := nil;
      Exc2 := nil;
      Exr1 := nil;
      Exr2 := nil;
    if AllowOrderedPairs and (CorrectExpr Is TTDXExprOrderedPair) and (ResponseExpr Is TTDXExprOrderedPair) then
    try
    try
      Exc1 := ParseExpression(TTDXExprOrderedPair(CorrectExpr).Expr1.ExprText,False);
      Exc2 := ParseExpression(TTDXExprOrderedPair(CorrectExpr).Expr2.ExprText,True);;
      Exr1 := ParseExpression(TTDXExprOrderedPair(ResponseExpr).Expr1.ExprText,False);;
      Exr2 := ParseExpression(TTDXExprOrderedPair(ResponseExpr).Expr2.ExprText,True);;

      Result := CheckExpr(Exc1,Exr1,'');
      if result <> evCorrect then Exit;
      Result := CheckExpr(Exc2,Exr2,'');
      Exit;
    Except
      on Ex:Exception do
      begin
        Result := evSyntaxError;
        FLastError := Ex.Message;
      end;
    end;
    finally
      Exc1.Free;
      Exc2.Free;
      Exr1.Free;
      Exr2.Free;
    end;

    //DV
    if (CorrectExpr Is TTDXExprList) and (ResponseExpr Is TTDXExprList) and not FAllSolutions then
    try
      FAllSolutions := True;
      FListImitate := True;
      FListOptions := loOrderedList;
      SetCorrect(FCorrect);
      Result := CheckAllSolutions;
      Exit;
    finally
      FAllSolutions := False;
      FListOptions := loNone;
      FListImitate := False;
    end;

    //Now we now we have Valid Expressions to so lets check them.  If any
    //exceptions occur at this point.  We will assume that the Answer Is not
    //in Correct form which Is signified by evNotCorrectForm.
    try
      Result := CheckExpr(CorrectExpr,ResponseExpr,FAnswerFormats[Index]);
    Except
      on Ex:Exception do
      begin
        if Ex Is TTDXBadFormException then
        begin
          FLastError := Ex.Message;
          Result := evBadForm;
        end
        else if Ex Is TTDXInCorrectException then
        begin
          FLastError := Ex.Message;
          Result := evCustomError;
        end
        else
        begin
          FLastError := '';
          Result := evInCorrect;
        end;
      end;
    end;

  finally
    CorrectExpr.Free;
    ResponseExpr.Free;
  end;
end;

function TTDXEvaluator.CheckSimilar(CorrectExpr,ResponseExpr:TTDXExpr; ReduceMult:boolean = false):TTDXEvaluatorStatus;
var
  CExpr,RExpr : TTDXExpr;
  corr : String;
  resp : String;
begin
  corr := CorrectExpr.ExprText;
  resp := ResponseExpr.ExprText;
  Result := evCorrect;

  if (IPCharCount(corr,'@') <> IPCharCount(resp,'@') ) then  Result := evBadForm;
  if (IPCharCount(corr,'+') <> IPCharCount(resp,'+') ) then  Result := evBadForm;
  if (IPCharCount(corr,'-') <> IPCharCount(resp,'-') ) then  Result := evBadForm;
  if (IPCharCount(corr,'*') <> IPCharCount(resp,'*') ) then  Result := evBadForm;
(*  CExpr := nil;
  RExpr := nil;
  try
    CExpr := CorrectExpr.SimilarExpr;
    RExpr := ResponseExpr.SimilarExpr;
    e1 :=  CExpr.ExprText;
    e2 :=  RExpr.ExprText;

    if CExpr.IsSimilar(RExpr,ReduceMult) then
      Result := evCorrect
    else
      Result := evBadForm;
  finally
    CExpr.Free;
    RExpr.Free;
  end;
  *)
end;


//Check Equations with symbolic Variables
function TTDXEvaluator.CheckSymbolic(CorrectExpr,ResponseExpr:TTDXExpr):TTDXEvaluatorStatus;
var
  I : integer;
  IVar : integer;
  ErrCount : INteger;
  V:TTDXEvalSymVar;
begin
  ErrCount := 0;

  I := MAX_EVALUATIONS;
  while I > 0 do
  begin
    try
      GenerateSymbolicValues;
      if (I<3) then
      begin
        for ivar := 0 to FVariables.Count-1 do
        begin
          V := TTDXEvalSymVar(FVariables[Ivar]);
          if ((I=2) and (Pos('!',FCorrect)=0) and (Pos('!',FResponse)=0) and (ErrCount<5)) then V.FValue := 1009;
          if ((I=1) and (Pos('!',FCorrect)=0) and (Pos('!',FResponse)=0) and (ErrCount<5)) then V.FValue := -1009;
        end;
      end;
      Result := CheckExpression(CorrectExpr,ResponseExpr,FNumericFormat);
      if Result <> evCorrect then
        Exit;
      Dec(I);
    Except
      on TTDXTryAgainException do
      begin
        //On Exception just try again.  Keep track of number of Errors for
        //for some reason they Exceed max_errorcount then return inCorrect.
        Inc(ErrCount);
        //FProblem.LogMsg('**Monte Carlo Exception occurred.  Trying again.');
        if ErrCount > MAX_ERRORCOUNT then
        begin
          ///TODO: ADD ERROR RETURN FProblem.LogMsg('**Monte Carlo Errors Exceeded limit');

          Result := evInCorrect;
          Exit;
        end;
      end;
    end;
  end;

  //If we made it here.  It's Correct.
  Result := evCorrect;

end;

//Check Equations without symboic Variables
function TTDXEvaluator.CheckExpression(CorrectExpr,ResponseExpr:TTDXExpr; AnsFmt:String):TTDXEvaluatorStatus;
var
  V1 : TTDXExprValue;
  V2 : TTDXExprValue;
  CDiv,RDiv : TTDXExprBinary;
  TExpr : TTDXExpr;
  ItemNum : Integer;
  I : Integer;
  CExpr,RExpr:TTDXExpr;
  D1,D2 : Double;
begin

    //**MAD** 5/14/06 - moved these two statements outside the try so they will
    //generate an Exception for the symbolic check.  THe problem Is that some symbolic
    //monte-carlo iterations will cause a divide by zero, which doesn't mean the
    //Answer Is inCorrect.  In this case we'll try again.

    //First perform normal Expression check
  try
    V1 := ResponseExpr.ExprValue;
    V2 := CorrectExpr.ExprValue;
  Except
    //Raise a special Exception so we can catch it later.
    on Ex:Exception do
      raise TTDXTryAgainException.Create(ex.Message);
  end;

  try


    if (AnswerRule = arNumericOnly) and (Trim(AnsFmt) <> '') then
    begin
      V2 := IPStrToFloat(IPFormatFloat(V2,AnsFmt));
    end;

    if (AnswerRule in [arReducedNumber,arNumericOnly,arNumericFrac,arNumericMult,arNoEvalFrac,arNoReduceFrac,arNoDecimalEquiv]) and
       not IsZero(FTolerance) then
    begin
      try
        D1 := V1;
        D2 := V2;
        if not SameValue(D1,D2,FTolerance+1e-10) then
        begin
          Result := evInCorrect;
          Exit;
        end;
      Except
        Result := evInCorrect;
        Exit;
      end;
    end
    else if   not IsVariantEqual(V1,V2) (*or (CompareText(V1,'-0') = 0)*)
    then
    begin
      Result := evInCorrect;
      Exit;
    end;

    //If we got here it must be Correct so far.
    Result := evCorrect;

    //Now check for special rules
    if AnswerRule = arNoEvalFrac then
    begin
      //Check that both sides are divisions.  If so, then compare numerator Values
      //and denominator Values separately.
      if (FAllowOrderedPairs) and (CorrectExpr Is TTDXExprOrderedPair) and
         (ResponseExpr Is TTDXExprOrderedPair) then
        ItemNum := 2
      else
        ItemNum := 1;

      for I := 0 to ItemNum-1 do
      begin
        CDiv := nil;
        RDiv := nil;

        if (ItemNum > 1) and (CorrectExpr Is TTDXExprOrderedPair) then
            TExpr := TTDXExprOrderedPair(CorrectExpr).Expr[I]
        else
            TExpr := CorrectExpr;

        if (TExpr Is TTDXExprUnary) then
          TExpr := TTDXExprUnary(TExpr).Expr;
        if (TExpr Is TTDXExprBinary) and (TTDXExprBinary(TExpr).BinaryType = tdxebDivide) then
           CDiv := TTDXExprBinary(TExpr);

        if (ItemNum > 1) and (ResponseExpr Is TTDXExprOrderedPair) then
            TExpr := TTDXExprOrderedPair(ResponseExpr).Expr[I]
        else
            TExpr := ResponseExpr;

        if (TExpr Is TTDXExprUnary) then
          TExpr := TTDXExprUnary(TExpr).Expr;
        if (TExpr Is TTDXExprBinary) and (TTDXExprBinary(TExpr).BinaryType = tdxebDivide) then
           RDiv := TTDXExprBinary(TExpr);
        if Assigned(CDiv) then
        begin

          if not Assigned(RDiv) then
            raise TTDXBadFormException.Create(tdxstSingleFracNone);

          //Check only absolute Values of top and bottom to take into
          //consideration unary operators.  We can conclude that if the
          //top portion of this code passed the Equations are indeed Equal
          //therefore just comparing the absolute Values of the tops and bottom
          //should be Enough.
          if (VarComplexAbs(CDiv.Expr1.ExprValue) <> VarComplexAbs(RDiv.Expr1.ExprValue)) or
             (VarComplexAbs(CDiv.Expr2.ExprValue) <> VarComplexAbs(RDiv.Expr2.ExprValue)) then
            raise TTDXBadFormException.Create(tdxstSingleFracBad);
        end;
      end;
    end
    else if AnswerRule = arNoReduceFrac then
    begin
      //Basic premise Is that if both solution and reponse contain a divide
      //then the numerators of Each divide are compared as well as the denominators.
      //If both compare then the Answer Is deemed Correct.
      if (FAllowOrderedPairs) and (CorrectExpr Is TTDXExprOrderedPair) and
         (ResponseExpr Is TTDXExprOrderedPair) then
        ItemNum := 2
      else
        ItemNum := 1;

      for I := 0 to ItemNum-1 do
      begin

        if (ItemNum > 1) and (CorrectExpr Is TTDXExprOrderedPair) then
            CDiv := TTDXExprOrderedPair(CorrectExpr).Expr[I].FindDivide
        else
            CDiv := CorrectExpr.FindDivide;

        if (ItemNum > 1) and (ResponseExpr Is TTDXExprOrderedPair) then
            RDiv := TTDXExprOrderedPair(ResponseExpr).Expr[I].FindDivide
        else
            RDiv := ResponseExpr.FindDivide;

        //Only do check if a divide Is found in Each
        if Assigned(CDiv) and Assigned(RDiv) then
        begin
          if (CDiv.Expr1.ExprValue <> RDiv.Expr1.ExprValue) or
             (CDiv.Expr2.ExprValue <> RDiv.Expr2.ExprValue) then
          begin
            raise TTDXBadFormException.Create(tdxstAnyFrac);
          end;
        end;
      end;
    end
    else if AnswerRule = arNoDecimalEquiv then
    begin
      if (FAllowOrderedPairs) and (CorrectExpr Is TTDXExprOrderedPair) and
         (ResponseExpr Is TTDXExprOrderedPair) then
        ItemNum := 2
      else
        ItemNum := 1;

      for I := 0 to ItemNum-1 do
      begin

        if (ItemNum > 1) and (CorrectExpr Is TTDXExprOrderedPair) then
            CExpr := TTDXExprOrderedPair(CorrectExpr).Expr[I]
        else
            CExpr := CorrectExpr;


        if (ItemNum > 1) and (ResponseExpr Is TTDXExprOrderedPair) then
            RExpr := TTDXExprOrderedPair(ResponseExpr).Expr[I]
        else
            RExpr := ResponseExpr;

        //This rule Is Very limited.  The first check Is to see if both the
        //solution and response evaluate to integers (no fractional parts).
        if not (IsZero(Frac(CExpr.ExprValue),1e-6) and
                IsZero(Frac(RExpr.ExprValue),1e-6)) and
        //One or both have evaluated to a fractional portion.  If Either doesn't
        //have a divide assume that it's not right.
        //This logic Is weird, but I replicated the original stuff(MAD)
             ((CExpr.FindDivide = nil) or (RExpr.FindDivide = nil)) then
        begin
          raise TTDXBadFormException.Create(tdxstNoDecEquiv);
        end;

      end;

    end
    else if AnswerRule = arReducedNumber then
    begin
     //C3491 DV 6/21/05 Added ItemNum/Expr[I] stuff to handle ordered pairs
     if (FAllowOrderedPairs) and (CorrectExpr Is TTDXExprOrderedPair) and
        (ResponseExpr Is TTDXExprOrderedPair) then
       ItemNum := 2
     else
       ItemNum := 1;

     for I := 0 to ItemNum-1 do
     begin

      if ItemNum=1 then begin
        if ResponseExpr Is TTDXExprUnary then
          TExpr := TTDXExprUnary(ResponseExpr).Expr
        else
          TExpr := ResponseExpr;
      end else
      begin
        //C3491 old code had just that
        if TTDXExprOrderedPair(ResponseExpr).Expr[I] Is TTDXExprUnary then
          TExpr := TTDXExprUnary(TTDXExprOrderedPair(ResponseExpr).Expr[I]).Expr
        else
          TExpr := TTDXExprOrderedPair(ResponseExpr).Expr[I]
      end;

      Result := evBadForm;

      if TExpr Is TTDXExprLiteral then
      begin
        Result := evCorrect;
      end
      else if (TExpr Is TTDXExprBinary) and (TTDXExprBinary(TExpr).BinaryType = tdxebDivide) then
      begin
        with TTDXExprBinary(TExpr) do
        begin
          if Expr1.IsInteger and Expr2.IsInteger then
          begin
            if (Abs(Expr2.ExprValue) <> 1) and (GCF(Expr1.ExprValue,Expr2.ExprValue) = 1) then
              Result := evCorrect;
          end;
        end;
      end
      else if (TExpr Is TTDXExprMixedNum) then
      begin
        with TTDXExprMixedNum(TExpr) do
        begin
          if ExprNum.IsInteger and ExprTop.IsInteger and ExprBottom.IsInteger then
          begin
            if (ExprBottom.ExprValue <> 1) and (GCF(ExprTop.ExprValue,ExprBottom.ExprValue) = 1)  and
               (ExprBottom.ExprValue > ExprTop.ExprValue) and (ExprBottom.ExprValue > 0) and (ExprTop.ExprValue > 0)then
              Result := evCorrect;
          end;
        end;
      end;

      if Result = evBadForm then
        raise TTDXBadFormException.Create(tdxstNotReduced);

     end;

    end;

    if (Result = evCorrect) and (FCommaOptions = coRequired) then
    begin
      if not ResponseExpr.IsValidCommaNumber then
        raise TTDXBadFormException.Create(tdxstCommaFormatted);
    end;

  Except
    on TTDXBadFormException do
    begin
      raise;
    end;
    on Ex:Exception do
    begin
      Result := evInCorrect;
    end;
  end;
end;

function TTDXEvaluator.CheckExact(CorrectExpr,ResponseExpr:TTDXExpr):TTDXEvaluatorStatus;
begin
  try
    if ResponseExpr.IsEqual(CorrectExpr) then
      Result := evCorrect
    else
      Result := evBadForm;
  Except
    Result := evBadForm;
  end;
end;



{This routine will create Variables if they don't already Exist}
procedure TTDXEvaluator.SymVarCreateAnswer(Sender:TTDXExpr; VarName:String);
var
  VNode:TTDXEvalSymVar;
begin
  if not FVarNames.Exists(VarName,Pointer(VNode)) then
  begin
    VNode := TTDXEvalSymVar.Create(VarName);
    FVariables.Add(VNode);
    FVarNames.Add(VarName,VNode);
  end;
end;

{This routine will raise an Exception if the Variable does not Exist.}
procedure TTDXEvaluator.SymVarCreateResponse(Sender:TTDXExpr; VarName:String);
var
  VNode:TTDXEvalSymVar;
begin
  if not FVarNames.Exists(VarName,Pointer(VNode)) then
  begin
    raise Exception.CreateFmt('Variable <%s> Is not found in Correct Answer.',[VarName]);
  end;
end;



function TTDXEvaluator.SymVarGetValue(Sender:TTDXExpr; VarName:String):TTDXExprValue;
var
  VNode:TTDXEvalSymVar;
begin
  if FVarNames.Exists(VarName,Pointer(VNode)) then
    Result := VNode.FValue
  else
    raise Exception.CreateFmt('Symblic Variable <%s> does not Exist.',[VarName]);

end;

procedure TTDXEvaluator.ClearSymVars;
begin
  FVariables.Clear;
  FVarNames.Clear;
end;

procedure TTDXEvaluator.ParserError (Sender : TObject; Error : TCocoError);
var
  ErrString:String;
begin
  with (Sender as TTDXExprParserX) do
  begin
    ErrString := ErrorStr(Error.ErrorCode,Error.Data);
  end;
  if FTry then
      FTryError := True
  else
  raise Exception.CreateFmt('%s near column %d',[ErrString,Error.Col]); //Typo C446 DV 2/25/05
end;


procedure TTDXEvaluator.SetCorrect(const Value: String);
var
  CorrExpr : TTDXExpr;
  I : integer;
begin
  FCorrect := Value;
  CorrectAnswers.Clear;
  CorrExpr := nil;

  if FAllSolutions then
  try
    if FListImitate then
      CorrExpr := ParseExpression(FCorrect,false)
    else
      CorrExpr := ParseExpression('['+FCorrect+ ']',false);
    if CorrExpr Is TTDXExprList then
    begin
      for I := 0 to TTDXExprLIst(CorrExpr).ExprCount - 1 do
        AddCorrectAnswer(TTDXExprList(CorrExpr).Expr[I].ExprText,'');
    end;
  finally
    CorrExpr.Free;
  end
  else
    AddCorrectAnswer(FCorrect,'');
end;

procedure TTDXEvaluator.SetListOptions(const Value: TIPListOptions);
begin
  FListOptions := Value;
  FAllSolutions := (Value <> loNone);
end;

end.
