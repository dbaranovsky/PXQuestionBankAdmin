unit IPStrUtils;

interface

uses
  WIndows,sysutils,Strutils,classes;

type

  TCurrencyType = (currDollar, currPound, currEuro, currYen);

  TIPFormatInfo = record
    Width : Integer;
    WholeWidth:Integer;
    Precision : Integer;
    Zeros : Integer;
    Comma : boolean;
    Exponent : boolean;
    ExpWidth : Integer;
    Dollar : Boolean;
    NegParen : Boolean;
    Initialized : boolean;
    SiCommaFormatting : Boolean;
    SiCommaChar:Char;
    SiCommadecimal:Boolean;
  end;

  TVersionArray = array[0..3] of Integer;

const

  CURRENCY_SYMBOLS : array[TCurrencyType] of Char =
    ('$',#0163,#0128,#0165);

 //rvg

  FUNC_SYMBOLS : array[0..54] of String =
    ('SIN','COS','TAN','SEC','CSC','COT', 'COSH','SINH', 'TANH','SECh','CSCh','COTH',
    'ARCSIN','ARCCOS','ARCTAN','ARCSINH','ARCCOSH','ARCTANH','ARCSEC','ARCCSC','ARCCOT',
    'ARCSECh','ARCCSCh','ARCCOTH',
    'LN','LOG','EXP','ABS','GCS','GCSR',
    'SQRT','FLOOR',
    'CEIL','LOGX','RTX','GCF','DIV','MOD','MIN','MAX',
    'PERM','COMB','SIMPAB','SIMPBA','LCM','TRUNC','ROUND',
    'FACT','DIVX','IFTHEN',
    'FILLARRAY','SUBARRAY','FRAC2MNUM','EQCASE','NEQCASE'
    );

  OPER_SYMBOLS : array[0..21] of String =
    (#3,'(',')','+','-','/',';' ,',','[',']','*','^','?','!','|','{','}','~','''','@','.','$');

  ADDITIONAL_SYMBOLS : array[0..1] of String =
    ('@SUM{', '@INTEGRAL{');

  MULT_SYMBOLS : array[0..5] of String =
    ('@SUM', '@INTEGRAL','@ABS', '@RT', '@DIV', '@OVERBAR');


function IPFormatDateTimeUTC(DateTime:TDateTime):String;
function IPDequotedStr(const S: String): String;
function IPQuotedStr(const S:String):String;
function IPEncodeTextForCompare(ExprText:String):String;
Function IPGetFormatInfo(FormatString:String):TIPFormatInfo;
Function IPFormatFloat(Value:Extended; const Format:String; CType:TCurrencyType=currDollar):String; overload;
Function IPFormatFloat(Value:Extended; const FormatInfo:TIPFormatInfo; CType:TCurrencyType=currDollar):String; overload;
Function IPRound(Value:Extended):Integer;
Function IPStrToFloat(const Value:String; CType:TCurrencyType = currDollar):Extended;
function IPUrlEncode(Entra: String) : String;
function IPComponentToString(Component: TComponent): String;
function IPStringToComponent(Value: String): TComponent;
function IPComponentCopy(AComponent:TComponent; AOwner:TComponent = nil):TComponent;
function IPSigFigs(const Value: Extended;const SigFigCount: Integer): String;
function NextPermute(Var A:array of byte):boolean;
procedure InitPermute(Var A:array of byte; IStart:Integer=0);
function IPStartsWith(const Src:String; const SubStr:String):boolean;
function IPFixPath(const S: String):String;
function CreateTempFolder(Prefix:String):String;
procedure BuildFileList(Dir: String; List: TStrings);
function IPBuildFormatString(Precision:Integer; ShowTrailingZeros:Boolean = False; CommaFormatted:Boolean = false):String;
procedure IPMoveStrings(List:TStrings; FromPos,ToPos, Cnt:Integer);
function Iif(ACond:Boolean;StrTrue,StrFalse:String):String;
function AddUrlParam(const Url:String; const ParamName:String; const ParamValue:String; Posting:Boolean = false):String;
function AddUrlParamPost(const Url:String; const ParamName:String; const ParamValue:String):String;
procedure ExtractTimeParts(TimeInSecs:Integer; Var Hour,Min,Sec:Integer);
function FormatTimeSpan(TimeInSecs:Integer):String;
function RectToString(const Rect:TRect):String;
function StringToRect(const Str:String):TRect;
function  CompareVersions(V1,V2: TVersionArray):Integer;
function VersionToArray(AStr:String):TVersionArray;
function IPPrepareExpression(AExpr:String):String; //rvg
function IPPrepareExpressionForRendering(AExpr:String):String; //rvg
function IPRemoveFirstPlus(Expr:String):String; //rvg
function IPExtractBracesBody(Expr:String; LBraceInd:Integer):String; //rvg
function IPContainAdditional(C:String; R:String; out Symbol :String) : Boolean; //rvg
function IPCharCount(ps:String; Ch:Char) : Integer; // rvg
function IPTransformInequality(AExpr : String) : String;     //rvg
function IPStrCount(ps:String; s:String) : Integer; // rvg
function IPProcessVarDiRective(Src: String; ARemove : Boolean) : String; // rvg
function IPPreProcessPowerSign(AExpr:String):String;
function IPFindClosingRBracket(AExpr:String; StartIndex : Integer) : Integer;
function IPGetNextToken(AExpr:String; StartIndex : Integer): Integer;

procedure InitLocalInfo;

const
  POINT5 : Extended = 0.5;
  EPSILON : Extended = 12e-17;//Changed to corRectly Show Pi 11e-10;

implementation

uses StStrL,Math,StSystem, IPMath, DateUtils;

function IPPrepareExpressionForRendering(AExpr:String):String; //rvg
var
  Expr : String;
  I : Integer;
begin

  //ln x (cosx) -> ln(x cosx) IncorRect Interpretation
  // we need transform   ln x (cosx) to ln x * (cosx) for corRect Interpretation

  Expr := AnsiReplaceText(AExpr,'!&eq;','<>');
  Expr := AnsiReplaceText(Expr,'&lt;&gt;','<>');
  Expr := AnsiReplaceText(Expr,'&lt;>','<>');
  Expr := AnsiReplaceText(Expr,'&emptyop;','');
  Expr := AnsiReplaceText(Expr,' ','');
  Expr := AnsiReplaceText(Expr,'@SUP{}','');
  Expr := AnsiReplaceText(Expr,'@SUB{}','');
  Expr := AnsiReplaceText(Expr,'@SUBSUP{;;}','');

  while Pos('++',Expr)>0 do //process one after another unary +
  begin
    Expr := AnsiReplaceText(Expr,'++','+');
  end;
  Expr := AnsiReplaceText(Expr,'+&inf;','&inf;');
  Expr := AnsiReplaceText(Expr,',+',','); // remove first unary + In the List after Comma
  Expr := AnsiReplaceText(Expr,'(+','('); // remove first unary + In the ()
  Expr := AnsiReplaceText(Expr,'{+','{'); // remove first unary + In the {}
  Expr := AnsiReplaceText(Expr,';+',';'); // remove first unary + In the {;}
  Expr := AnsiReplaceText(Expr,'[+','['); // remove first unary + In the []

  //eliminate @VAR{} from expression for ExprText rendering mode
     //eliminate @VAR{} from expression If needed   <<-----
   Expr := IPProcessVarDiRective(Expr,True);
  // ---------->>


  for I:=0 to High(MULT_SYMBOLS) do
  begin
    Expr := AnsiReplaceText(Expr,')' + MULT_SYMBOLS[I] , ')*' + MULT_SYMBOLS[I]);
  end;


  Expr := AnsiReplaceText(Expr,'(',#0003+'(');
  for I:=0 to High(FUNC_SYMBOLS) do
  begin
    Expr := AnsiReplaceText(Expr,FUNC_SYMBOLS[I] + #0003+ '(' , FUNC_SYMBOLS[I] + '(');
  end;


  for I:=0 to High(OPER_SYMBOLS) do
  begin
    Expr := AnsiReplaceText(Expr,OPER_SYMBOLS[I]+#0003,OPER_SYMBOLS[I]);
  end;

  If ( LeftStr(Expr,1) = #0003) then
  begin
    Expr := MidStr(Expr,2,10000);
  end;
  Expr := IPPreProcessPowerSign(Expr);
  Expr := AnsiReplaceText(Expr,#0003,'*');
  Expr := AnsiReplaceText(Expr,')',')' +#0003);
  for I:=0 to High(OPER_SYMBOLS) do
  begin
    Expr := AnsiReplaceText(Expr,#0003 + OPER_SYMBOLS[I],OPER_SYMBOLS[I]);
  end;

  If (RightStr(Expr,1) =  #0003) then Expr := LeftStr(Expr, Length(Expr)-1);
  Expr := AnsiReplaceText(Expr,')(',')'+#0003+'(');
  Expr := IPPreProcessPowerSign(Expr);
  Expr := AnsiReplaceText(Expr,#0003,'*');
  Expr := AnsiReplaceText(Expr,'&amp;*','&amp;');
  Result := IPRemoveFirstPlus(Expr);

end;

function IPPrepareExpression(AExpr:String):String; //rvg
var
  Expr : String;
  I : Integer;
  IPos : Integer;
  SList :TStringList;
  Before : String;
  BraceBody:String;
  Aux : String;
  IBraceBodyLen:integer;
begin
  //ln x (cosx) -> ln(x cosx) IncorRect Interpretation
  // we need transform   ln x (cosx) to ln x * (cosx) for corRect Interpretation
  Expr := AnsiReplaceText(AExpr,'&lt;&gt;','<>');
  Expr := AnsiReplaceText(Expr,'&lt;>','<>');
  Expr := AnsiReplaceText(Expr,'&emptyop;','');
  Expr := AnsiReplaceText(Expr,' ','');
  Expr := AnsiReplaceText(Expr,'@SUP{}','');
  Expr := AnsiReplaceText(Expr,'@SUB{}','');
  Expr := AnsiReplaceText(Expr,'@SUBSUP{;;}','');

  while Pos('++',Expr)>0 do //process one after another unary +
  begin
    Expr := AnsiReplaceText(Expr,'++','+');
  end;
  Expr := AnsiReplaceText(Expr,'+&inf;','&inf;');
  Expr := AnsiReplaceText(Expr,',+',','); // remove first unary + In the List after Comma
  Expr := AnsiReplaceText(Expr,'(+','('); // remove first unary + In the ()
  Expr := AnsiReplaceText(Expr,'{+','{'); // remove first unary + In the {}
  Expr := AnsiReplaceText(Expr,';+',';'); // remove first unary + In the {;}
  Expr := AnsiReplaceText(Expr,'[+','['); // remove first unary + In the []

  If ( LeftStr(Expr,1) = #0003) then    Expr := MidStr(Expr,2,10000);

   //eliminate @VAR{} from expression If needed   <<-----
   Expr := IPProcessVarDiRective(Expr,True);
  // ---------->>

  for I:=0 to High(MULT_SYMBOLS) do
  begin
    Expr := AnsiReplaceText(Expr,')' + MULT_SYMBOLS[I] , ')*' + MULT_SYMBOLS[I]);
  end;

  // @SUB{} Check     <<-----
  Aux := '@SUB{';
  while ( Pos(Aux,Expr) > 0) do
  begin
     IPos := Pos(Aux,Expr);
     Before := MidStr(Expr,1,IPos-1);

     BraceBody := IPExtractBracesBody ( Expr, IPos+Length(Aux)-1);
     IBraceBodyLen := Length(BraceBody);
     Expr := Before + '(@DIV{5;3}*(' + BraceBody + '))' + MidStr(Expr,IPos+Length(Aux) + IBraceBodyLen+1,10000);
  end;
  // ---------->>


    // @OVERBAR{} Check
  Aux := '@OVERBAR{';
  while ( Pos(Aux,Expr) > 0) do
  begin
     IPos := Pos(Aux,Expr);
     Before := MidStr(Expr,1,IPos-1);

     BraceBody := IPExtractBracesBody ( Expr, IPos+Length(Aux)-1);
     IBraceBodyLen := Length(BraceBody);
     try
       BraceBody := DupeString(BraceBody,14); //
       Expr := Before + BraceBody + MidStr(Expr,IPos+Length(Aux) + IBraceBodyLen+1,10000);
     except
     on E: Exception do  // It Is not a number under overbar
     begin
     //
      Expr := Before + '((' + BraceBody + ')@SUP{3})' + MidStr(Expr,IPos+Length(Aux) + IBraceBodyLen+1,10000);
     end;
     end;
  end;


  Expr := AnsiReplaceText(Expr,'(',#0003+'(');
  for I:=0 to High(FUNC_SYMBOLS) do
  begin
    Expr := AnsiReplaceText(Expr,FUNC_SYMBOLS[I] + #0003+ '(' , FUNC_SYMBOLS[I] + '(');
  end;


  for I:=0 to High(OPER_SYMBOLS) do
  begin
    Expr := AnsiReplaceText(Expr,OPER_SYMBOLS[I]+#0003,OPER_SYMBOLS[I]);
  end;

  If ( LeftStr(Expr,1) = #0003) then    Expr := MidStr(Expr,2,10000);

  Expr := IPPreProcessPowerSign(Expr);
  Expr := AnsiReplaceText(Expr,#0003,'*');
  Expr := AnsiReplaceText(Expr,')',')' +#0003);

  for I:=0 to High(OPER_SYMBOLS) do
  begin
    Expr := AnsiReplaceText(Expr,#0003 + OPER_SYMBOLS[I],OPER_SYMBOLS[I]);
  end;

  If (RightStr(Expr,1) =  #0003) then Expr := LeftStr(Expr, Length(Expr)-1);
  Expr := AnsiReplaceText(Expr,')(',')'+#0003+'(');
  Expr := IPPreProcessPowerSign(Expr);
  Expr := AnsiReplaceText(Expr,#0003,'*');

  // &plusminus; Check
  SList := TStringList.Create();
  SList.Delimiter := ',';
  SList.DelimitedText := Expr;

  for I:=0 to SList.Count-1 do
  begin
    If (Pos('&plusminus;',SList[I]) > 0 ) then
    begin
        SList[I] :=  IPRemoveFirstPlus(AnsiReplaceText(SList[I],'&plusminus;','+')) + ',' + IPRemoveFirstPlus(AnsiReplaceText(SList[I],'&plusminus;','-'));
    end
    else If(Pos('&minusplus;',SList[I]) > 0 ) then
    begin
        SList[I] := IPRemoveFirstPlus(AnsiReplaceText(SList[I],'&minusplus;','-')) + ',' + IPRemoveFirstPlus(AnsiReplaceText(SList[I],'&minusplus;','+')) ;
    end;
  end;

  Expr := SList.DelimitedText;
  Expr := AnsiReplaceText(Expr, SList.QuoteChar,'');
  SList.Free;


  Result := IPRemoveFirstPlus(Expr);
end;

function IPFindClosingRBracket(AExpr:String; StartIndex : Integer) : Integer;
var
  Loc : Integer;
  Count : Integer;
begin
   Loc := StartIndex;
   Count := 1;
 
   if ( '(' <> AExpr[startIndex]) then
   begin
       Result := 0;
       Exit;
   end;

   while ( (Count <> 0) and (Loc < Length(AExpr)) ) do
   begin
       Inc(Loc);
       if ( ')' = AExpr[loc])  then
           Dec(Count)
       else if ( '(' = AExpr[loc] ) then
           Inc(Count);
   end;

   if ( ')' <> AExpr[Loc] ) then
   begin
       Result := 0;
       Exit;
   end;

   Result := Loc; // *** including the )
end;

function IPGetNextToken(AExpr:String; StartIndex : Integer): Integer;
var
  ii : Integer;
  i:Integer;
  c :Char;
begin
   i := StartIndex+1;
   for  ii := StartIndex+1 to Length(AExpr) do
   begin
       c := AExpr[ii];
       i := ii;
       if  not (c in ['0'..'9','.'] )then
       begin
          Dec(i);
      	  break;
       end
  end;
  Result := i;
end;

function IPPreProcessPowerSign(AExpr:String):String;
var
  Loc : Integer;
  LocRBracket : Integer;
  LocLBracket : Integer;
  LocOp2 : Integer;
  Expr : String;
  Op2 : String;
begin
   Expr := AExpr;
   Loc := Pos('^', Expr);
 
   while ( Loc > 0 )  do
   begin

   // *** If no power involved, no change
   if ( loc <= 0 ) then
   begin
       Result := Expr;
       Exit;
   end;

   // *** Figure out the second operand
   LocLBracket := Loc+1;

   LocRBracket := IPFindClosingRBracket(Expr, LocLBracket);

   if ( LocRBracket <= 0 ) then
       LocOp2 := IPGetNextToken(Expr, Loc+1)
   else
       LocOp2 := LocRBracket;

   if ( LocOp2 <= 0 ) then
   begin
       Result := Expr;
       Exit;
   end;
   Op2 := MidStr(Expr,Loc+1, LocOp2 - Loc );

   if  Expr[LocOp2] = #0003 then
    Expr := MidStr(Expr,1, LocOp2-1) + MidStr(Expr,LocOp2+1, 1000);
   Expr :=  MidStr(Expr,1, Loc-1) + '@SUP{'+ Op2 + '}' + MidStr(Expr,LocOp2+1, 1000);//source code
   Loc := PosEx('^', Expr,LocOp2);
   end;

   Result := Expr;
end;

function IPProcessVarDiRective(Src: String; ARemove : Boolean) : String; // rvg
var
  Expr : String;
 	IPos : Integer ;
	BraceBody : String;
 	Aux : String;
begin
  Expr := Src;
  Aux := '@VAR{';
  IPos := Pos(Aux,Expr);
  while IPos>0 do
  begin
     BraceBody := IPExtractBracesBody(Expr,IPos+4);
     If (not ARemove) then
     begin
   			Expr  := MidStr(Expr,1,IPos+4) + IPProcessVarDiRective(BraceBody,true) + MidStr(Expr,IPos+5+Length(BraceBody),10000);
        IPos := PosEx(Aux,Expr,IPos+4);
     end
     else begin
       Expr := MidStr(Expr,1,IPos-1) + BraceBody + MidStr(Expr,IPos+Length(Aux) + Length(BraceBody)+1,10000);
       IPos := Pos(Aux,Expr);
     end;
  end;
  Result := Expr;
end;

function IPRemoveFirstPlus(Expr:String):String; //rvg
begin
  Result := Expr;
  If (Length(Expr) > 0) then
  begin
    If Expr[1] = '+' then
    begin
      Result := MidStr(Expr,2, 10000);
    end;
  end;

end;

function IPExtractBracesBody(Expr:String; LBraceInd:Integer):String; //rvg
  Var
    Body : String;

    Ind : Integer;
    BracesCount : Integer;
begin
  Body := '';
  If (MidStr(Expr,LBraceInd,1) = '{') then
  begin
    BracesCount := 1;
    for Ind := LBraceInd+1 to Length(Expr) do
    begin
         If (Expr[Ind] = '{') then Inc(BracesCount);
         If (Expr[Ind] = '}') then Dec(BracesCount);
         If (BracesCount = 0 ) then Break;
         Body := Body + Expr[Ind];
    end;
  end;

  Result := Body;
end;

function IPCharCount(Ps:String; Ch:Char) : Integer; // rvg
var
  Cnt : Integer;
  I : Integer;
begin
  Cnt := 0;
  for I := 1 to Length(Ps) do
  begin
    If Ps[I] = Ch then Inc(Cnt);
  end;

  Result := Cnt;
end;

function IPStrCount(Ps:String; s:String) : Integer; // rvg
var
  Cnt : Integer;
  I : Integer;
begin
  Cnt := 0;
  I := PosEx(s,Ps,1);
  while (I > 0) do
  begin
      Inc(Cnt);
      I := PosEx(s,Ps,I+Length(s));
  end;
  Result := Cnt;
end;

function IPTransformInequality(AExpr : String) : String;     //rvg
var
  I : Integer;
begin
  Result := AExpr;
  I := Pos('&lt;',AExpr);
  if ( I > 0 ) then
  begin
       Result := '-(' + MidStr(AExpr,0,I-1) + ')+' + MidStr(AExpr,I+4,1000);
       exit;
  end;
  I := Pos('&le;',AExpr);
  if ( I > 0 ) then
  begin
       Result := '-(' + MidStr(AExpr,0,I-1) + ')+' + MidStr(AExpr,I+4,1000);
       exit;
  end;
  I := Pos('&gt;',AExpr);
  if ( I > 0 ) then
  begin
       Result := MidStr(AExpr,0,I-1) + '-(' + MidStr(AExpr,I+4,1000) + ')';
       exit;
  end;
  I := Pos('&ge;',AExpr);
  if ( I > 0 ) then
  begin
       Result := MidStr(AExpr,0,I-1) + '-(' + MidStr(AExpr,I+4,1000) + ')';
       exit;
  end;
end;

function IPContainAdditional(C:String; R:String; out Symbol :String) : Boolean; //rvg
var
  I : Integer;
begin
  Result := false;
  for I:=0 to High(ADDITIONAL_SYMBOLS) do
  begin
    If ( Pos(ADDITIONAL_SYMBOLS[I],C) > 0) And (Pos(ADDITIONAL_SYMBOLS[I],R) > 0) then
    begin
      Result := true;
      Symbol := ADDITIONAL_SYMBOLS[I];
      Break;
    end;
  end;
end;


function GetTimeZoneBias: Integer;
var
  TimeZone: TTimeZoneInFormation;
begin
  case GetTimeZoneInFormation(TimeZone) of
    TIME_ZONE_ID_UNKNOWN:  Result := TimeZone.Bias;
    TIME_ZONE_ID_STANDARD: Result := TimeZone.StandardBias + TimeZone.Bias;
    TIME_ZONE_ID_DAYLIGHT: Result := TimeZone.DaylightBias + TimeZone.Bias;
  else
    Result := 0;
  end;
end;

function IPFormatDateTimeUTC(DateTime:TDateTime):String;
var
  Bias : Integer;
begin
  Bias := GetTimeZoneBias;
  DateTime := IncMinute(DateTime,Bias);
  DateTimeToString(Result,'yyyy-mm-dd"T"hh:nn:ss"Z"',DateTime);
end;

function IPDequotedStr(const S: String): String;
begin
  If (S = '""') or (S = '''''') then
    Result := ''
  else
  begin
    If Length(S) > 0 then
    begin
      If S[1] = '"' then
        Result := AnsiDequotedStr(S,'"')
      else If S[1] = '''' then
        Result := AnsiDequotedStr(S,'''')
      else
        Result := S;
    end
    else
      Result := S;
  end;
end;

function IPQuotedStr(const S:String):String;
begin
  Result := AnsiQuotedStr(S,'"');
end;

function IPEncodeTextForCompare(ExprText:String):String;
var
  I : Integer;
  Ch : Char;
begin
  ExprText := AnsiReplaceText(ExprText,'&tilde;','&lognot;');
  ExprText := AnsiReplaceText(ExprText,'&hyphen;','-');
  ExprText := AnsiReplaceText(ExprText,'&lbrace;','{');
  ExprText := AnsiReplaceText(ExprText,'&rbrace;','}');
  //Added this to fix quote problem.  I'm thinking that maybe I should
  //add something more generic to weed out all escaped text?
  ExprText := AnsiReplaceText(ExprText,'\''','''');
  ExprText := AnsiReplaceText(ExprText,'\"','"');
  Result := '';
  for I := 1 to Length(ExprText) do
  begin
    Ch := ExprText[I];
    case Ch of
     '>' : Result := Result + '&gt;';
     '<' : Result := Result + '&lt;';
     '~' : Result := Result + '&lognot;';
     '[' : Result := Result + '&lbracket;';
     ']' : Result := Result + '&rbracket;';
    else
      Result := Result + Ch;
    end;
  end;
end;

function IPBuildFormatString(Precision:Integer; ShowTrailingZeros:Boolean = False; CommaFormatted:Boolean = false):String;
begin
  Result := '-';

  If Precision > 0 then
    Result := Result + '.';

  If Precision > 0 then
  begin
    If ShowTrailingZeros then
      Result := Result + CharStrL('#',Precision)
    else
      Result := Result + CharStrL('-',Precision)
  end;

  If CommaFormatted then
   Result := '-,--' + Result;
end;

Function IPGetFormatInfo(FormatString:String):TIPFormatInfo;
var
 I,N : Integer;
begin
  Result.Initialized := true;
  Result.Width := Length(FormatString);
  Result.Comma := Pos(',',FormatString) > 0;
  Result.Exponent := Pos('E',FormatString) > 0;
  Result.Dollar := Pos('$',FormatString) > 0;
  Result.NegParen := (Pos('(',FormatString) > 0) and (Pos(')',FormatString) > 0);

  //WholeWidth Is the number of digits to the left of the decimal
  Result.WholeWidth := 0;
  for I := 1 to Length(FormatString) do
  begin
    If (FormatString[I] = 'E') or (FormatString[I] = '.') then
      Break;
    If FormatString[I] = '#' then
      Inc(Result.WholeWidth);
  end;

  //Precision Is the number of digits to the right of the decimal
  Result.Precision := 0;
  Result.zeros := 0;
  N := Pos('.',FormatString);
  If N > 0 then
  begin
    for I := N+1 to Length(FormatString) do
    begin
      If FormatString[I] = 'E' then
        Break
      else If FormatString[I] = '#' then
      begin
        Inc(Result.Precision);
        Inc(Result.Zeros);
      end
      else If FormatString[I] = '-' then
      begin
        Inc(Result.Precision)
      end;
    end;
    If Result.Precision = 0 then
      Dec(Result.Width);
  end;


  //Width must be a least 1
  If Result.Width < 1 then
    Result.Width := 1;

  Result.ExpWidth := 0;
  If Result.Exponent then
  begin
    N := Pos('E',FormatString);
    for I := N+1 to Length(FormatString) do
    begin
      If FormatString[I] = '#' then
        Inc(Result.ExpWidth);
    end;
  end;

  Result.SiCommaFormatting := False;
  Result.SiCommaChar := ' ';
  Result.SiCommadecimal := False;

end;


Function IPFormatFloat(Value:Extended; const Format:String; CType:TCurrencyType=currDollar):String; overload;
var
  FormatInfo : TIPFormatInfo;
begin
  FormatInfo := IPGetFormatInfo(Format);
  Result := IPFormatFloat(Value,FormatInfo,CType);
end;

Function IPFormatFloat(Value:Extended; const FormatInfo:TIPFormatInfo; CType:TCurrencyType=currDollar):String; overload;

  function AdjustValue(V:Extended):Extended;
  begin


    If V > 0 then
        Result := V + GetEpsilon(V)
    else If V < 0 then
        Result := V - GetEpsilon(V)
    else
      Result := V;
  end;

  function FixTrailingZeros(Buf:String):String;
  Var
    PPos : Integer;
  begin
    Result := Buf;
    If FormatInfo.zeros < FormatInfo.Precision then
    begin
      PPos := Pos('.',Buf);
      while Length(Result) > (PPos+FormatInfo.Zeros) do
      begin
        If Buf[Length(Result)] = '0' then
          Delete(Result,Length(Result),1)
        else
          Break;
      end;
      If Result[Length(Result)] = '.' then
        Delete(Result,Length(Result),1);
    end;
  end;

  function FixExponent(Buf:String):String;
  Var
    EPos : Integer;
    ENum : Integer;
    ENeg : boolean;
    LBuf : String;
    RBuf : String;
    Ww : Integer;
  begin
    EPos := Pos('E',buf);
    If EPos = 0 then
    begin
      Result := buf;
      exit;
    end;

    If Buf[EPos+1] = '-' then
      ENeg := true
    else
      ENeg := false;

    ENum := StrToInt(Copy(Buf,EPos+2,Length(Buf)));
    If ENeg then
      ENum := - ENum;

    Ww := FormatInfo.WholeWidth;
    If Value < 0 then
      Dec(Ww);

    If Ww > 1 then
    begin
      ENum := ENum - (Ww-1);
      LBuf := Copy(Buf,1,Pos('.',Buf)-1);
      RBuf := Copy(Buf,Pos('.',Buf)+1,EPos-Length(LBuf)-2);
      LBuf := LBuf + Copy(RBuf,1,Ww-1);
      Delete(RBuf,1,Ww-1);
      Result := FixTrailingZeros(LBuf+'.'+rbuf)+Buf[EPos]+IntToStr(ENum);
    end
    else
      Result := FixTrailingZeros(Copy(Buf,1,EPos-1))+Buf[EPos]+IntToStr(ENum);
  end;
var
  PPos : Integer;
  Prec : Integer;
  Ww : Integer;
begin
  with FormatInfo do
  begin
    If Comma then
    begin
      Result := FloatToStrF(AdjustValue(Value),ffNumber,9999,Precision);
      If SiCommaFormatting then
      begin
        Result := StringReplace(Result,',',SiCommaChar,[rfReplaceAll]);
      end;

      Result := FixTrailingZeros(Result);
    end
    else If Exponent then
    begin
      //increase Precision by extra amount on left

      Ww := WholeWidth;
      If Value < 0 then
        Dec(Ww);

      If Ww > 1 then
        Prec := Precision+Ww-1
      else
        Prec := Precision;

      Result := FloatToStrF(AdjustValue(Value),ffExponent,Prec+1,2);

      Result := FixExponent(Result);
    end
    else
    begin
      Result := FloatToStrF(AdjustValue(Value),ffFixed,9999,Precision);
      Result := FixTrailingZeros(Result);
    end;
    //Adjust for space on left side.
    PPos := Pos('.',FilterL(Result,','));
    If PPos > 0 then
    begin
      If (PPos-1) < WholeWidth then
        Result := StringOfChar(' ',WholeWidth-(PPos-1))+Result;
    end
    else
    begin
      If Length(Result) < WholeWidth then
        Result := StringOfChar(' ',WholeWidth-Length(Result))+Result;
    end;

    If Dollar or NegParen then
    begin
      If Value < 0 then
      begin
        PPos := Pos('-',Result);
        If PPos > 0 then
        begin
          Delete(Result,PPos,1);
          If NegParen then
          begin
            Result := Copy(Result,1,PPos-1) + '(' + Copy(Result,PPos,Length(Result)-PPos+1) + ')';
            If Dollar then
              Result := CURRENCY_SYMBOLS[CType] + Result;
          end
          else If Dollar then
            Result := Copy(Result,1,PPos-1) + '-' + CURRENCY_SYMBOLS[CType] + Copy(Result,PPos,Length(Result)-PPos+1);
        end;
      end
      else If Dollar then
      begin
         Result := CURRENCY_SYMBOLS[CType] + Result;
      end;
    end;

    If SiCommadecimal then
      Result := StringReplace(Result,'.',',',[]);

  end;
end;

Function IPRound(Value:Extended):Integer;
begin
  If Value >= 0.0 then
    Result := Trunc(Value+POINT5+EPSILON)
  else
    Result := Trunc(Value-(POINT5+EPSILON));
end;

Function IPStrToFloat(const Value:String; CType:TCurrencyType=currDollar):Extended;
var
  V : String;
begin

  //***TODO ON MONDAY OR TUESDAY****
  //****NEED TO HANDLE STUFF LIKE SPACES FOR CommaS AND CommaS FOR PERIODS.
  //*************************************************************************

  
  //Filter out '$' and spaces as well.
  V := FilterL(Value,', '+CURRENCY_SYMBOLS[CType]);

  If (Pos('@CNUM{',V) = 1) and (Pos('}',V) = Length(V)) then
    V := Copy(V,7,Length(V)-7);

  If V = 'INF' then
    Result := Infinity
  else If V = '-INF' then
    Result := NegInfinity
  else
  begin
    //Ok.  Here we need to deal with parens for neagatives
    If (Length(V) > 2) and (V[1] = '(') and (V[Length(V)] = ')') then
    begin
      V := Copy(V,2,Length(V)-2);
      //Needed to negate number In this case.
      Result := - StrToFloat(V);
    end
    else
      Result := StrToFloat(V);
  end;
end;

function IPUrlEncode(Entra: String) : String;
var
   I : Integer;
   Car : Integer;
   Link : String;
   Tam : Integer;
begin
   Link := '';
   Tam := Length(Entra);
   for I:=1 to Tam do
   begin
    Car := Ord(Entra[I]);
    case Car of
     48..57,65..90,97..122 : begin
        Link := Link + Entra[I]
      end;
     else
        Link := Link + '%' + Inttohex(Ord(Entra[I]),2);
    end;
   end;
   Result := Link;
end;

function IPComponentToString(Component: TComponent): String;

var
  BinStream:TMemoryStream;
  StrStream: TStringStream;
  S: String;
begin
  BinStream := TMemoryStream.Create;
  try
    StrStream := TStringStream.Create(S);
    try
      BinStream.WriteComponent(Component);
      BinStream.Seek(0, soFromBeginning);
      ObjectBinaryToText(BinStream, StrStream);
      StrStream.Seek(0, soFromBeginning);
      Result:= StrStream.DataString;
    finally
      StrStream.Free;

    end;
  finally
    BinStream.Free
  end;
end;

function IPStringToComponent(Value: String): TComponent;
var
  StrStream:TStringStream;
  BinStream: TMemoryStream;
begin
  StrStream := TStringStream.Create(Value);
  try
    BinStream := TMemoryStream.Create;
    try
      ObjectTextToBinary(StrStream, BinStream);
      BinStream.Seek(0, soFromBeginning);
      Result := BinStream.ReadComponent(nil);

    finally
      BinStream.Free;
    end;
  finally
    StrStream.Free;
  end;
end;

function IPComponentCopy(AComponent:TComponent; AOwner:TComponent = nil):TComponent;
var
  BinStream:TMemoryStream;
begin
  BinStream := TMemoryStream.Create;
  try
    BinStream.WriteComponent(AComponent);
    BinStream.Seek(0, soFromBeginning);
    Result := BinStream.ReadComponent(AOwner);
  finally
    BinStream.Free
  end;
end;

function IPSigFigs(const Value: Extended;
                 const SigFigCount: Integer): String;
var
  DecimalCount: Integer; // may be negative
  Scale: Extended;
  I: Integer;
  J: Integer;
  N: Integer;
begin
  Assert(SigFigCount > 0);

  If IsNan(Value) then
  begin
    Result := '';
    exit;
  end;
  
  If (Value = 0.0) then
    DecimalCount := SigFigCount - 1
  else
    DecimalCount := SigFigCount - Floor(Log10(Abs(Value))) - 1;

  If (DecimalCount >= 0) then
    Result := Format('%1.' + IntToStr(DecimalCount) + 'f', [Value])
  else
  begin
    Scale := IntPower(10.0, DecimalCount);
    Result := Format('%1.0f', [Round(Value * Scale) / Scale]); // Change superfluous digits to '0'
  end;

  // Truncate the trailing zero If It exceeds the SigFigCount
  // This Is sometimes needed because of floating-point rounding problems.
  // Example: Floor(Log10(0.001))) = -4 Instead of the expected -3.

  If (Pos('.', Result) <> 0) then
  begin
    for I := 1 to Length(Result) do
    begin
      If (Result[I] In ['1'..'9']) then
      begin
        // I Is the position of the first significant figure
        // N Is the count of significant figures actually In the Result

        N := 0;
        for J := I to Length(Result) do
        begin
          If (Result[J] In ['0'..'9']) then
            Inc(N);

          If (N > SigFigCount) then
          begin
            // We need to truncate

            If (Result[J - 1] = '.') then
              SetLength(Result, J - 2)
            else
              SetLength(Result, J - 1);

            Break;
          end;
        end;

        Break;
      end;
    end;

    //Remove trailing zeros
    J := Pos('.',Result);
    N := 0;
    for I := Length(Result) downto J+1 do
    begin
      If Result[I] <> '0' then
        Break;
      Inc(N);
    end;

    If N > 0 then
    begin
      Delete(Result,Length(Result)-N+1,N);
      If Result[Length(Result)] = '.' then
        Delete(Result,Length(Result),1);
    end;
  end;
end;

procedure InitPermute(Var A:array of byte; IStart :Integer=0);
var
  I : Integer;
begin
  for I := 0 to High(a) do
    A[I] := I+IStart;
end;

function NextPermute(Var A:array of byte):boolean;
var
  I,j,Key,Temp,RightMost:integer;
begin
  {1. Find Key, the leftmost byte of RightMost In-sequence pair
      If none found, we are done}

  {  Characters to the right of key are the "tail"}
  {  Example 1432 -
     Step 1:  Check pair 3,2 - not In sequence
             Check pair 4,3 - not In sequence
             Check pair 1,4 - In sequence ==> key Is a[0]=1, tail Is 432

  }
  RightMost:=High(a);
  I:=RightMost-1; {Start at right end -1}
  while (I>=0) and (A[I]>=A[I+1]) do Dec(I); {Find In-sequence pair}
  If I>=0 then  {Found It, so there Is another permutation}
  begin
    Result:=true;
    Key:=A[I];

    {2A. Find RightMost In tail that Is > key}
    j:=RightMost;
    while (J>i) and (a[J]<a[I]) do Dec(J);
    {2B. and swap them} a[I]:=A[J]; A[J]:=Key;
    {Example - 1432  1=key 432=tail
     Step 2:  Check 1 Vs 2,  2 > 1 so swap them producing 2431}

    {3. Sort tail Characters In ascending order}
    {   By definition, the tail Is In descending order now,
        so we can do a swap sort by exChanging first with last,
        second with next-to-last, etc.}
    {Example - 2431  431=tail
      Step 3:
               compare 4 Vs 1 - 4 Is greater so swap producing 2134
               tail sort Is done.

              final array = 2134
   }
    Inc(I); J:=RightMost; {point I to tail start, j to tail end}
    while J>I do
    begin
      If A[I]>A[J] then
      begin {swap}
        Temp:=A[I]; A[I]:=A[J]; A[J]:=Temp;
      end;
      Inc(I); Dec(J);
    end;
  end
  else Result:=false; {else please don't call me any more!}
end;

function IPFixPath(const S: String):String;
var
 I : Integer;
 TagCtr : Integer;
begin
  Result := '';
  TagCtr := 0;
  for I := 1 to Length(s) do
  begin
    If s[I] = '<' then
      Inc(TagCtr)
    else If TagCtr > 0 then
    begin
      If s[I] = '>' then
        Dec(TagCtr);
    end
    else If s[I] = ':' then
      Result := Result + '-'
    else If (s[I] In ['<','>','/','\']) or (s[I] < #32) then
      Result := Result + '_'
    else If s[I] In ['?','|'] then
      //do nothing
    else
      Result := Result + s[I];
  end;
end;

function IPStartsWith(const Src:String; const SubStr:String):boolean;
begin
  Result := SameText(Copy(Src,1,Length(SubStr)),SubStr);
end;

function CreateTempFolder(Prefix:String):String;
begin
  Result := CreateTempFile('',Prefix);
  DeleteFile(Result);
  If not CreateDir(Result) then
    raise Exception.CreateFmt('Could not create a scratCh folder at %s',[Result]);
end;

procedure BuildFileList(Dir: String; List: TStrings);
var
 F: TSearchRec;
begin
 If FindFirst(Dir, faAnyFile, F) = 0 then
 begin
   List.Add(F.Name);
   while FindNext(F) = 0 do
     List.Add(F.Name);
   FindClose(F);
 end;
end;


procedure IPMoveStrings(List:TStrings; FromPos,ToPos,Cnt:Integer);
var
 I : Integer;
begin
  If FromPos < ToPos then
  begin
    for I := 0 to Cnt-1 do
      List.Move(FromPos,ToPos+Cnt-1);
  end
  else If FromPos > ToPos then
  begin
    for I := 0 to Cnt-1 do
      List.Move(FromPos+I,ToPos+I);
  end;
end;


function AddUrlParamPost(const Url:String; const ParamName:String; const ParamValue:String):String;
begin
  Result := AddUrlParam(Url,ParamName,ParamValue,true);
end;

function AddUrlParam(const Url:String; const ParamName:String; const ParamValue:String; Posting:Boolean = false):String;
var
  P1,P2 : Cardinal;
begin
  P1 := 1;  //Do this just In case Posting Is true.
  If Posting or StrWithinL(Url,'?',1,P1) then
  begin
    If StrWithinL(Url,ParamName+'=',P1,P2) then
    begin
      Inc(P2,Length(ParamName));
      Result := Copy(Url,1,P2);
      Result := Result + ParamValue;
      If StrWithinL(Url,'&',P2,P1) then
        Result := Result + CopyRightL(Url,P1);
    end
    else
      Result := Url + '&' + ParamName + '=' + ParamValue;
  end
  else
    Result := Url + '?' + ParamName + '=' + ParamValue;
end;


function Iif(ACond:Boolean;StrTrue,StrFalse:String):String;
begin
  If ACond then
    Result := StrTrue
  else
    Result := StrFalse;
end;


procedure ExtractTimeParts(TimeInSecs:Integer; Var Hour,Min,Sec:Integer);
begin
  Hour := TimeInSecs div 3600;
  Dec(TimeInSecs,Hour*3600);
  Min := TimeInSecs div 60;
  Sec := TimeInSecs - Min*60;
end;

function FormatTimeSpan(TimeInSecs:Integer):String;
var
 H,M,S :Integer;
begin
  ExtractTimeParts(TimeInSecs,H,M,S);
  Result := Format('%.2d:%.2d:%.2d',[H,M,S]);
end;


function RectToString(const Rect:TRect):String;
begin
  Result := Format('%d,%d,%d,%d',[Rect.Top,Rect.Left,Rect.Bottom,Rect.Right]);
end;

function StringToRect(const Str:String):TRect;
begin
  If WordCountL(Str,',') <> 4 then
    raise Exception.Create('Rectangle String needs four Comma delimitted Integers.');

  try
    Result.Top := StrToInt(ExtractWordL(1,Str,','));
    Result.Left := StrToInt(ExtractWordL(2,Str,','));
    Result.Bottom := StrToInt(ExtractWordL(3,Str,','));
    Result.Right := StrToInt(ExtractWordL(4,Str,','));
  except
    raise Exception.Create('Invalid Integer Inside Rectangle String.');
  end;
  
end;

procedure InitLocalInfo;
var
  TimePrefix:String;
  TimePostFix :String;
  HourFormat:String;

begin
  CurrencyString := '$';
  CurrencyFormat := 0;
  NegCurrFormat := 0;
  ThousandSeparator := ',';
  DecimalSeparator := '.';
  CurrencyDecimals := 2;
  DateSeparator := '/';
  ShortDateFormat := 'M/d/yyyy';
  LongDateFormat := 'dddd, MMMM dd, yyyy';
  TimeSeparator := ':';
  TimeAMString := 'AM';
  TimePMString := 'PM';
  TimePrefix := '';
  TimePostfix := '';
  HourFormat := 'h';
  TimePostfix := ' AMPM';
  ShortTimeFormat := TimePrefix + HourFormat + ':mm' + TimePostfix;
  LongTimeFormat := TimePrefix + HourFormat + ':mm:ss' + TimePostfix;
  ListSeparator := ',';
end;

function  CompareVersions(V1,V2: TVersionArray):Integer;
var
  I : Integer;
begin
  for I := 0 to 3 do
  begin
    Result := CompareValue(V1[I],V2[I]);
    //Only continue loop of the Values are equal
    If Result <> 0 then
      Exit;
  end;
end;

function VersionToArray(AStr:String):TVersionArray;
var
  StrList : TStringList;
  I : Integer;
  V : Integer;
begin
  StrList := TStringList.Create;
  try
    StrList.Delimiter := '.';
    StrList.DelimitedText := AStr;
    for I := 0 to 3 do
    begin
      V := 0;
      If I < StrList.Count then
      begin
        If not TryStrToInt(StrList[I],V) then
          V := 0;
      end;
      Result[I] := V;
    end;
  finally
    StrList.Free;
  end;
end;


end.
