unit IpEqEvaluator;

{$WARN SYMBOL_PLATFORM OFF}

interface

uses
  ComObj, ActiveX, IPEQEval_TLB, StdVcl,TDXEvaluator, SysUtils,CLasses,
  IpEqTExtParser,TDXExpr,StStrl,IpStrUtils,StrUtils;


type
  TIpEqEvaluator = class(TAutoObject, IIpEqEvaluator)
  private
    FAnswerRule : Integer;
    FExprText1 : String;
    FExprTExt2 : String;
    FEvaluator : TTDXEvaluator;
    FTextOptions : TTDXTextCompareOptions;
    function PreParseCommaNumbers(Value:String):String;
    function CheckText: TTDXEvaluatorStatus;
    function BothExceptions : Boolean;
    function EvaluateExpr(const Correct:String;const Answer:String) : TTDXEvaluatorStatus;
    procedure PreParseAdditional(const Expr:String;const AdditionalSymbol:String;
        out OutTop:String; out OutBottom:String; out OutExpr:String);
    function PreParseRT(const AExpr:String):String;

  protected
    function Get_AnswerRuleID: Integer; safecall;
    function Get_ExprText1: WideString; safecall;
    function Get_ExprText2: WideString; safecall;
    procedure Set_AnswerRuleID(Value: Integer); safecall;
    procedure Set_ExprText1(const Value: WideString); safecall;
    procedure Set_ExprText2(const Value: WideString); safecall;
    function Get_CommaOptions: Integer; safecall;
    procedure Set_CommaOptions(Value: Integer); safecall;
    function Get_AllowOrderedPairs: WordBool; safecall;
    function Get_AnswerRuleText(Index: Integer): WideString; safecall;
    function Get_AnwerRuleCount: Integer; safecall;
    procedure Set_AllowOrderedPairs(Value: WordBool); safecall;
    function Evaluate: WideString; safecall;
    function Get_ListOptions: Integer; safecall;
    procedure Set_ListOptions(Value: Integer); safecall;
    function Get_TextOptions: Integer; safecall;
    procedure Set_TextOptions(Value: Integer); safecall;

  public
    procedure Initialize;override;
    destructor Destroy;override;
  end;

implementation

uses ComServ;

procedure TIpEqEvaluator.Initialize;
begin
  inherited;
  FEvaluator := TTDXEValuator.Create;
  FEvaluator.Tolerance := 0;
  FEvaluator.AllowOrderedPairs := True;
  FTextOptions := [tcIgnoreSpaces,tcIgnoreCase];
  InitLocalInfo;
end;

destructor TIpEqEvaluator.Destroy;
begin
  FEvaluator.Free;
  inherited;
end;

function TIpEqEvaluator.Get_AnswerRuleID: Integer;
begin
  Result := FAnswerRule;
end;

function TIpEqEvaluator.Get_ExprText1: WideString;
begin
  Result := FExprText1;
end;

function TIpEqEvaluator.Get_ExprText2: WideString;
begin
  Result := FExprText2;
end;

procedure TIpEqEvaluator.Set_AnswerRuleID(Value: Integer);
begin
  FAnswerRule := Value;
  FEvaluator.AnswerRule := TTDXAnswerRule(Value);
end;

procedure TIpEqEvaluator.Set_ExprText1(const Value: WideString);
begin
  FExprText1 := Value;
end;

procedure TIpEqEvaluator.Set_ExprText2(const Value: WideString);
begin
  FExprText2 := Value;
end;

function TIpEqEvaluator.Get_CommaOptions: Integer;
begin
  Result := Integer(FEvaluator.CommaOptions);
end;

procedure TIpEqEvaluator.Set_CommaOptions(Value: Integer);
begin
  FEvaluator.CommaOptions := TTDXCommaOptions(Value);
end;

function TIpEqEvaluator.Get_AllowOrderedPairs: WordBool;
begin
  Result := FEvaluator.AllowOrderedPairs;
end;

function TIpEqEvaluator.Get_AnswerRuleText(Index: Integer): WideString;
begin
  Result := TDXAnswerRulesLongName[TTDXAnswerRule(Index)];
end;

function TIpEqEvaluator.Get_AnwerRuleCount: Integer;
begin
  Result := Integer(High(TTDXAnswerRule)) + 1;
end;

procedure TIpEqEvaluator.Set_AllowOrderedPairs(Value: WordBool);
begin
  FEvaluator.AllowOrderedPairs := Value;
end;

function TIpEqEvaluator.Evaluate: WideString;
var
  CountLP : Integer;
  CountRP : Integer;
  LenC : Integer;
  LenR : Integer;
begin
  try
    Result := TDXEvaluatorStatusString[evIncorrect];

    FExprText1 := AnsiReplaceText(FExprText1,'!&eq;','&ne;');
    FExprText1 := AnsiReplaceText(FExprText1,'&eq;','=');
    FExprText1 := AnsiReplaceText(FExprText1,'>=','&ge;');
    FExprText1 := AnsiReplaceText(FExprText1,'&gt;=','&ge;');
    FExprText1 := AnsiReplaceText(FExprText1,'<=','&le;');
    FExprText1 := AnsiReplaceText(FExprText1,'&lt;=','&le;');
    FExprText1 := AnsiReplaceText(FExprText1,'!=','&ne;');
    FExprText1 := AnsiReplaceText(FExprText1,'&lt;&gt;','&ne;');
    FExprText1 := AnsiReplaceText(FExprText1,'&lt;>','&ne;');
    FExprText1 := AnsiReplaceText(FExprText1,'<>','&ne;');
    FExprText1 := AnsiReplaceText(FExprText1,'<','&lt;');
    FExprText1 := AnsiReplaceText(FExprText1,'>','&gt;');

    FExprText2 := AnsiReplaceText(FExprText2,'!&eq;','&ne;');
    FExprText2 := AnsiReplaceText(FExprText2,'&eq;','=');
    FExprText2 := AnsiReplaceText(FExprText2,'>=','&ge;');
    FExprText2 := AnsiReplaceText(FExprText2,'&gt;=','&ge;');
    FExprText2 := AnsiReplaceText(FExprText2,'<=','&le;');
    FExprText2 := AnsiReplaceText(FExprText2,'&lt;=','&le;');
    FExprText2 := AnsiReplaceText(FExprText2,'!=','&ne;');
    FExprText2 := AnsiReplaceText(FExprText2,'&lt;&gt;','&ne;');
    FExprText2 := AnsiReplaceText(FExprText2,'&lt;>','&ne;');
    FExprText2 := AnsiReplaceText(FExprText2,'<>','&ne;');
    FExprText2 := AnsiReplaceText(FExprText2,'<','&lt;');
    FExprText2 := AnsiReplaceText(FExprText2,'>','&gt;');

    CountLP := IPStrCount(FExprText1,'&lt;') + IPStrCount(FExprText1,'&gt;');
    CountRP := IPStrCount(FExprText2,'&lt;') + IPStrCount(FExprText2,'&gt;');
    if ( CountLP <> CountRP ) then Exit;

    CountLP := IPStrCount(FExprText1,'&le;') + IPStrCount(FExprText1,'&ge;');
    CountRP := IPStrCount(FExprText2,'&le;') + IPStrCount(FExprText2,'&ge;');
    if ( CountLP <> CountRP ) then Exit;

    FExprText1 := IPTransformInequality(FExprText1);
    FExprText2 := IPTransformInequality(FExprText2);

    FExprText1 := IPPrepareExpression(FExprText1);
    FExprText2 := IPPrepareExpression(FExprText2);
    // &cup;  &cap; processing
    LenC := Length(FExprText1);
    LenR := Length(FExprText2);

    FExprText1 := AnsiReplaceText(FExprText1,'&cup;',',');
    FExprText2 := AnsiReplaceText(FExprText2,'&cup;',',');
    if ((LenC - Length(FExprText1)) <>  (LenR - Length(FExprText2))) then Exit;

    LenC := Length(FExprText1);
    LenR := Length(FExprText2);

    FExprText1 := AnsiReplaceText(FExprText1,'&cap;',',');
    FExprText2 := AnsiReplaceText(FExprText2,'&cap;',',');
    if ((LenC - Length(FExprText1)) <>  (LenR - Length(FExprText2))) then Exit;
    //---  end &cup;  &cap; processing

    // Calc count  of brackets
    CountLP := IPCharCount(FExprText1,'[');
    CountRP := IPCharCount(FExprText2,'[');
    if ( CountLP <> CountRP ) then Exit;
    CountLP := IPCharCount(FExprText1,']');
    CountRP := IPCharCount(FExprText2,']');
    if ( CountLP <> CountRP ) then Exit;

    FExprText1 := AnsiReplaceText(FExprText1,'[','(');
    FExprText1 := AnsiReplaceText(FExprText1,']',')');
    FExprText2 := AnsiReplaceText(FExprText2,'[','(');
    FExprText2 := AnsiReplaceText(FExprText2,']',')');
    // -----------


    if (Pos(',',FExprText2) > 0) then
    begin
      if (FEvaluator.ListOptions = loNone) then FEvaluator.ListOptions := loList;
    end;

    if FAnswerRule = Integer(arText) then
      Result := TDXEvaluatorStatusString[CheckText]
    else
    begin
        FEvaluator.Correct := FExprText1;

        if FEvaluator.CommaOptions <> coNotAllowed then
           FEvaluator.Response := PreParseCommaNumbers(FExprText2)
        else
          FEvaluator.Response := FExprText2;

        Result := TDXEvaluatorStatusString[EvaluateExpr(FExprText1,FExprText2)];
    end;
    if FEvaluator.LastError <> '' then
      Result := Result + #10#13 + FEvaluator.LastError;
  except
    on E:Exception do
      if Result <> '' then
        Result := E.Message
      else
        Result := Result + #10#13 + E.Message;
  end;
end;

// rvg added 12/07/2009
function TIpEqEvaluator.EvaluateExpr(const Correct:String;const Answer:String) : TTDXEvaluatorStatus;
var
  Ind : integer;
  After : String;
  Bottom:String;
  Top:String;

  AfterR : String;
  BottomR:String;
  TopR:String;

  AdditionalSymbol : String;
  
begin
    FEvaluator.Correct := PreParseRT(Correct);
    FEvaluator.Response := PreParseRT(Answer);

    if (  IPContainAdditional(Correct,Answer, AdditionalSymbol) ) then
    begin
      PreParseAdditional( Correct, AdditionalSymbol, Top, Bottom,After);
      PreParseAdditional( Answer, AdditionalSymbol, TopR, BottomR, AfterR);

      Result := EvaluateExpr(Bottom, BottomR);
      if (Result = evCorrect) then Result := EvaluateExpr(Top, TopR);
      if (Result = evCorrect) then Result := EvaluateExpr(After, AfterR);
      Exit;
    end
    else
    begin
      if ( Pos('=',Correct) > 0) And (Pos('=',Answer) > 0) then
      begin
        Ind := Pos('=',Correct);
        FEvaluator.Correct := MidStr(Correct,1,ind-1) + '-(' + MidStr(Correct,Ind+1,10000) + ')';
        Ind := Pos('=',Answer);
        FEvaluator.Response := MidStr(Answer,1,ind-1) + '-(' + MidStr(Answer,Ind+1,10000) + ')';
      end;
    end;

      if BothExceptions then
         Result := CheckText
      else
      begin
        Result := FEvaluator.CheckAnswers;
      end;
end;

function TIpEqEvaluator.PreParseRT(const AExpr:String):String;
var
  Ind : Integer;
  Shift : Integer;
  Expr : String;
  Body : String;
  SList : TStringList;
begin
  Expr := AExpr;
  Ind := PosEx('@RT{',Expr,1);
  while Ind > 0 do
  begin
    Body := IPExtractBracesBody(AExpr, Ind + 3);
    Shift := Length(Body);
    SList := TStringList.Create;
    SList.Delimiter := ';';
    SList.DelimitedText := Body;
    if (SList.Count = 2) then
    begin
      if (SList[1] = '') then
      begin
        SList[0] := SList[0] + ';2';
      end
      else
        SList[0] := SList[0] + ';' +  SList[1];
    end;
    Expr := MidStr(Expr,1,Ind+3) + SList[0] + MidStr(Expr,Ind+4+Shift,1000);
    SList.Free;
    Ind := PosEx('@RT{',Expr,Ind+3);
  end;
  Result := Expr;
end;

procedure TIpEqEvaluator.PreParseAdditional(const Expr:String;const AdditionalSymbol:String;
        out OutTop:String; out OutBottom:String; out OutExpr:String);
var
  SList : TStringList;
  Before : String;
  After : String;
  Body : String;
  Bottom:String;
  Top:String;

  Ind : Integer;
begin
  Bottom:='';
  Top:='';
      Ind := Pos(AdditionalSymbol,Expr);
      Before := MidStr(Expr,1,Ind-1);
      Body := IPExtractBracesBody(Expr,Ind+Length(AdditionalSymbol)-1);
      After := Before + #0003 + MidStr(Expr,ind+Length(AdditionalSymbol) + Length(Body)+1,10000);
      OutExpr := IPPrepareExpression(After);

      if (Body <> '') then
      begin
          SList := TStringList.Create;
          SList.Delimiter := ';';
          SList.DelimitedText := Body;
          Ind := 1;
          Bottom := SList[0];
          if (Length(SList[0]) > 0) then
          begin
            if (SList[0][1] = '&') then
            begin
              Bottom := SList[0] + ';';
              Inc(Ind);
            end;
          end;
          if (Ind < SList.Count) then
          begin
            Top := SList[Ind];
            if (Length(SList[Ind]) > 0) then
            begin
              if (SList[Ind][1] = '&') then
              begin
                Top := SList[Ind] + ';';
              end;
            end;
          end;
          SList.Free;
      end;
      OutTop := Top;
      OutBottom := Bottom;

end;


function TIpEqEvaluator.PreParseCommaNumbers(Value: String): String;
var
  S,Res : string;
  Parser : TIpEqTextParser;
  Token : TIPEQToken;
begin
  Parser := TIpEqTextParser.Create;
  Parser.AllowCommaNumbers := True;
  Parser.Text := Value;
  Res := '';
  try
    while Parser.moreTokens do
    begin
      Token := Parser.NextToken;
      if pos(',',Parser.TokenVal) <> 0 then
      begin
        if Token = ttFloat then
          S := '@CNUM{'+ FloatToStr(Parser.NumberValue) + '}'
        else if Token = ttInteger then
          S := '@CNUM{'+ IntToStr(Round(Parser.NumberValue)) + '}'
        else
          S := Parser.TokenVal;
      end
      else
        S := Parser.TokenVal;
      Res := Res + S;
    end;
  finally
    Parser.Free;
    Result := res;
  end;

end;

function TIpEqEvaluator.Get_ListOptions: Integer;
begin
  Result := Integer(FEvaluator.ListOptions);
end;

procedure TIpEqEvaluator.Set_ListOptions(Value: Integer);
begin
  FEvaluator.ListOptions := TIpListOptions(Value);
end;

function TIpEqEvaluator.CheckText():TTDXEvaluatorStatus;
var
  I,Idx,SolutionCount : Integer;
  SolN : String;
  RespList : TStringList;
  Txt1,Txt2 : String;
begin
  Result := evIncorrect;
  if FEvaluator.AllSolutions then
  begin
    SolutionCount := Integer(WordCountL(FExprText1,','));
    if Integer(WordCountL(FExprText2,',')) = SolutionCount then
    begin
      RespList := TStringList.Create;
      RespList.CaseSensitive := not (tcIgnoreCase in FTextOptions);
      try
        for I := 1 to SolutionCount do
        begin
          Txt1 := ExtractWordL(I,FExprText1,',');
          Txt1 := Trim(IPEncodeTextForCompare(Txt1));
          if tcIgnoreSpaces in FTextOptions then
            Txt1 := FilterL(Txt1,' ');
          RespList.Add(Txt1);
        end;

        for I := 0 to SolutionCount-1 do
        begin
          SolN := ExtractWordL(I,FExprText2,',');
          SolN := Trim(IPEncodeTextForCompare(SolN));
          if tcIgnoreSpaces in FTextOptions then
            SolN := FilterL(SolN,' ');
          Idx := RespList.IndexOf(SolN);

          if FEvaluator.ListOptions = loOrderedList then
          begin
            if i<>Idx then Exit;
          end
          else
          begin
            if Idx >= 0 then
              RespList.Delete(Idx)
            else
              Exit;
          end;
        end;
        Result := evCorrect;
      finally
        RespList.Free;
      end;
    end;
  end
  else
  begin

      Txt1 := Trim(IPEncodeTextForCompare(FExprText1));
      Txt2 := Trim(IPEncodeTextForCompare(FExprText2));

      if tcIgnoreSpaces in FTextOptions then
      begin
        Txt1 := FilterL(Txt1,' ');
        Txt2 := FilterL(Txt2,' ');
      end;

      if tcIgnoreCase in FTextOptions then
      begin
        if SameText(Txt1,Txt2) then
        begin
          Result := evCorrect;
          Exit;
        end;
      end
      else
      begin
        if Txt1 = Txt2 then
        begin
          Result := evCorrect;
          Exit;
        end;
      end;

  end;
end;

function TIpEqEvaluator.Get_TextOptions: Integer;
begin
  if FTextOptions = [] then
    Result := 0
  else if FTextOptions = [tcIgnoreSpaces] then
    Result := 1
  else if FTextOptions = [tcIgnoreCase] then
    Result := 2
  else if FTextOptions = [tcIgnoreSpaces,tcIgnoreCase] then
    Result := 3
end;

procedure TIpEqEvaluator.Set_TextOptions(Value: Integer);
begin
  case value of
   0: FTextOptions := [];
   1: FTextOptions := [tcIgnoreSpaces];
   2: FTextOptions := [tcIgnoreCase];
   3: FTextOptions := [tcIgnoreSpaces,tcIgnoreCase];
  end;
end;

function TIpEqEvaluator.BothExceptions: Boolean;
var
  RespExpr,CorrExpr : TTDXExpr;
  Exception1,Exception2 : Boolean;
  V : Variant;
begin
    RespExpr := nil;
    CorrExpr := nil;
    FEvaluator.ClearSymVars;

    try
      if FEvaluator.AllSolutions then
      begin
        CorrExpr := FEvaluator.ParseExpression('['+FExprText1+']',false,true);
        Exception1 := FEvaluator.TryError;
      end
      else
      begin
        CorrExpr := FEvaluator.ParseExpression(FExprText1,false,true);
        Exception1 := FEvaluator.TryError;
      end;

      //check for value without Exception
      if not Exception1 then
      begin
        //12/28/07
        FEvaluator.GenerateSymbolicValues;
        V := CorrExpr.ExprValue;
      end;
    except
      Exception1 := True;
    end;
    try
      CorrExpr.Free;
    except
    end;

    try
      if FEvaluator.AllSolutions then
      begin
        RespExpr := FEvaluator.ParseExpression('['+FExprText2+']',true,true);
        Exception2 := FEvaluator.TryError;
      end
      else
      begin
        RespExpr := FEvaluator.ParseExpression(FExprText2,true,true);
        Exception2 := FEvaluator.TryError;
      end;
      //check for value without Exception
      if not Exception2 then v := RespExpr.ExprValue;
    except
      Exception2 := True;
    end;
    try
    RespExpr.Free;
    except
    end;
    Result := Exception1 and Exception2;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TIpEqEvaluator, Class_IpEqEvaluator,
    ciMultiInstance, tmApartment);
end.
