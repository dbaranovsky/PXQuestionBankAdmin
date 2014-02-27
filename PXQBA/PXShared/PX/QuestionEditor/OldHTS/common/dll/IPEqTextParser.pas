unit IPEqTextParser;

interface

uses classes, IPEqNode;

type

  TIPEqToken = (ttNone,ttText,ttInteger,ttFloat,ttFunction,ttComma,ttVariable);

type
  TIPInputOption = (ioAllowCommaNumber,ioAllowDollar,ioAllowMinus,ioAllowNegParens);
  TIPInputOptions = set of TIPInputOption;

const
  IPEqTextTokenNames : array[TIPEqToken] of string = (
    'none','text','Integer','float','function','Comma','variable');

type

  TIPEqTextParser = class
  private
    FFunctionList : TStringList;
    FPos : Integer;
    FText : String;
    FTokenVal : String;
    FNumberValue : Double;
    FCaseSensitive : boolean;
    FAllowCommaNumbers : boolean;
    FInterpretFunctions : boolean;
    FOnVarExists:TIPEqOnVarExistsEvent;
    FAllowDollarNumbers : Boolean;
    FAllowNegParens : Boolean;
    FAllowMinus : Boolean;
    FCurrencyType:TIPEqCurrency;
    FPlainText:Boolean;
    procedure SetFunctionList(Value : TStringList);
    procedure SetText(Value:String);
    function ParseNumber:TIPEqToken;
    function ParseFunction:TIPEqToken;
    function ParseVariable:TIPEqToken;
    procedure SetCaseSensitive(const Value: Boolean);
    function CompareStrings(const S1, S2: string): Integer;
  protected
  public
    Constructor Create;
    Destructor Destroy; override;
    function MoreTokens:Boolean;
    function NextToken:TIPEqToken;

    property FunctionList:TStringList read FFunctionList write SetFunctionList;
    property Text:String read FText write SetText;
    property TokenVal:String read FTokenVal;
    property CaseSensitive:boolean read FCaseSensitive write SetCaseSensitive;
    property InterpretFunctions:boolean read FInterpretFunctions write FInterpretFunctions;
    property AllowCommaNumbers:boolean read FAllowCommaNumbers write FAllowCommaNumbers;
    property AllowDollarNumbers:Boolean read FAllowDollarNumbers write FAllowDollarNumbers;
    property AllowNegParens:Boolean read FAllowNegParens write FAllowNegParens;
    property AllowMinus:Boolean read FAllowMinus write FAllowMinus;
    property OnVarExists:TIPEqOnVarExistsEvent read FOnVarExists write FOnVarExists;
    property NumberValue:Double read FNumberValue;
    property CurrencyType:TIPEqCurrency read FCurrencyType write FCurrencyType;
    property PlainText:Boolean read FPlainText write FPlainText;
  end;

  Function IPInputToFloat(Str:String; var Value:Double;  CType:TIPEqCurrency; Options:TIPInputOptions=
      [ioAllowCommaNumber,ioAllowDollar,ioAllowMinus,ioAllowNegParens]):Boolean;

implementation

uses sysutils;

Function IPInputToFloat(Str:String; var Value:Double; CType:TIPEqCurrency; Options:TIPInputOptions=
      [ioAllowCommaNumber,ioAllowDollar,ioAllowMinus,ioAllowNegParens]):Boolean;
var
  P : TIPEqTextParser;
begin
  P := TIPEqTextParser.Create;
  try
    P.CurrencyType := CType;
    P.AllowCommaNumbers := ioAllowCommaNumber in Options;
    P.AllowDollarNumbers := ioAllowDollar in Options;
    P.AllowNegParens := ioAllowNegParens in Options;
    P.AllowMinus := ioAllowMinus in Options;
    P.Text := Str;
    if (P.NextToken in [ttInteger,ttFloat]) and not P.MoreTokens then
    begin
      Value := P.NumberValue;
      Result := True;
    end
    else
      Result := False;
  finally
    P.Free;
  end;
end;


Constructor TIPEqTextParser.Create;
begin
  FCaseSensitive := false;
end;

Destructor TIPEqTextParser.Destroy;
begin
  inherited Destroy;
end;

procedure TIPEqTextParser.SetFunctionList(Value : TStringList);
begin
  FFunctionList := Value;
  if Assigned(FFunctionList) then
  begin
    FFunctionList.Sorted := false;
    FFunctionList.CaseSensitive := FCaseSensitive;
    FFunctionList.Sorted := true;
  end;
end;

procedure TIPEqTextParser.SetText(Value:String);
begin
  FText := Value;
  FPos := 1;
  FTokenVal := '';
end;


function TIPEqTextParser.MoreTokens:Boolean;
begin
  Result := FPos <= Length(FText);
end;

function TIPEqTextParser.NextToken:TIPEqToken;
var
  Ch : Char;
begin

  Result := ttNone;

  if FPos > Length(FText) then
    Exit;

  Ch := FText[FPos];

  if (Ch in ['0'..'9']) or
     ((Ch = CurrencyChars[FCurrencyType]) and FAllowDollarNumbers) or
     ((Ch = '-') and FAllowMinus) or
     ((Ch in ['(',')']) and FAllowNegParens) then
    Result := parseNumber
  else if ch = ',' then
  begin
    Result := ttComma;
    FTokenVal := Ch;
    Inc(FPos);
  end
  else if (Ch = '~') and Assigned(FOnVarExists) then
  begin
    Result := parseVariable
  end
  else
  begin
    if not FPlainText then
      Result := parseFunction;
  end;

  if Result = ttNone then
  begin
    Result := ttText;
    FTokenVal := FText[FPos];
    Inc(FPos);
  end;

end;

function TIPEqTextParser.ParseNumber:TIPEqToken;
var
  PeriodFound : boolean;
  MinusFound : Boolean;
  ParenCount : Integer;
  DollarFound : Boolean;
  NumberFound : Boolean;
  NumString : String;
  CommaFound : Boolean;

const
  IntChars: set of char = ['0'..'9'];

begin

  FNumberValue := 0;

  MinusFound := False;
  ParenCount := 0;
  DollarFound := False;
  NumberFound := False;

  CommaFound := False;

  FTokenVal := '';
  NumString := '';

  PeriodFound := false;
  while FPos <= Length(FText) do
  begin
    if (FText[FPos] = CurrencyChars[FCurrencyType]) and FAllowDollarNumbers and not DollarFound and not CommaFound then
    begin
      DollarFound := True;
      FTokenVal := FTokenVal + CurrencyChars[FCurrencyType];
      Inc(FPos);
    end
    else if FText[FPos] = ',' then
    begin

      if not FAllowCommaNumbers then
        Break;

      //If we already saw a Period jump out.
      if PeriodFound then
        Break;
      //If it's not after a valid number then jump out.
      if not NumberFound then
        Break;


      //Look ahead to the next three characters they must be a valid Integer
      //in order to accept this Comma.
      if Length(FText) < (FPos+3) then
        Break;

      if not((FText[FPos+1] in ['0'..'9']) and
             (FText[FPos+2] in ['0'..'9']) and
             (FText[FPos+3] in ['0'..'9']))
      then
        Break;

      CommaFound := true;

      FTokenVal := FTokenVal + Copy(FText,FPos,4);
      NumString := NumString + Copy(FText,FPos+1,3);
      Inc(FPos,4);

    end
    else if FText[FPos] = '.' then
    begin
      if PeriodFound then
        Break;
      CommaFound := False;
      PeriodFound := true;
      FTokenVal := FTokenVal + FText[FPos];
      NumString := NumString + FText[FPos];
      Inc(FPos);
    end
    else if FText[FPos] in ['0'..'9'] then
    begin
      if CommaFound then
          Break;
      NumberFound := True;
      FTokenVal := FTokenVal + FText[FPos];
      NumString := NumString + FText[FPos];
      Inc(FPos);
    end else if CommaFound then
    begin
      Break; //Don't consider Options after this when still parsing a Comma number
    end
    else if FAllowMinus and (FText[FPos] = '-') then
    begin
      if MinusFound or NumberFound or PeriodFound or (ParenCount > 0)  then
        Break;
      MinusFound := true;
      FTokenVal := FTokenVal + FText[FPos];
      Inc(FPos);
    end
    else if FAllowNegParens and (FText[FPos] = '(') then
    begin
      //already found a paren
      if (ParenCount > 0) then
        Break;
      if MinusFound or NumberFound or PeriodFound then
        Break;
      Inc(ParenCount);
      FTokenVal := FTokenVal + FText[FPos];
      Inc(FPos);
    end
    else if FAllowNegParens and (FText[FPos] = ')') then
    begin
      if (ParenCount <> 1) then
        Break;
      Inc(ParenCount);
      FTokenVal := FTokenVal + FText[FPos];
      Inc(FPos);
      //This state terminates a float so we just break out at this point.
      Break;
    end
    else
      Break;
  end;
  if Length(FTokenVal) > 0 then
  begin

    if Length(NumString) > 0 then
    begin
      if TryStrToFloat(NumString,FNumberValue) then
      begin
        if MinusFound or (ParenCount = 2) then
          FNumberValue := - FNumberValue;
      end;
    end;

    if PeriodFound then
      Result := ttFloat
    else
      Result := ttInteger;
  end
  else
    Result := ttNone;
end;

function TIPEqTextParser.CompareStrings(const S1, S2: string): Integer;
begin
  if CaseSensitive then
    Result := AnsiCompareStr(S1, S2)
  else
    Result := AnsiCompareText(S1, S2);
end;

function TIPEqTextParser.ParseFunction:TIPEqToken;
var
  FIndex : Integer;
  TIndex : Integer;
  Len : Integer;
  Buf: String;
  EndPos : Integer;
  Comp : Integer;
  Cc : Integer;
begin

  Result := ttNone;

  if not Assigned(FunctionList) then
    Exit;

  FIndex := 0;
  FTokenVal := '';
  EndPos := -1;

  Len := Length(FText);
  TIndex := 0;

  try
    repeat

      while ((TIndex+FPos) <= Len) and (FIndex < FunctionList.Count) do
      begin
        Cc := CompareStrings(Copy(FText,FPos,TIndex+1),Copy(FunctionList[FIndex],1,TIndex+1));
        if Cc < 0 then
          Exit;
        if (Length(FunctionList[FIndex]) <= TIndex) or (Cc > 0) then
          Inc(FIndex)
        else
          Break;
      end;

      if FIndex >= FunctionList.Count then
        Exit;

      Buf := Copy(FText,Fpos,TIndex+1);
      if (FunctionList[FIndex] = 'e') or (FunctionList[FIndex] = 'i') then
        Comp :=  AnsiCompareStr(Buf,FunctionList[FIndex])
      else
        Comp := CompareStrings(Buf,FunctionList[FIndex]);

      if Comp = 0 then
      begin
        FTokenVal := Buf;
        EndPos := FPos+TIndex;
        Result := ttFunction;
      end;

      Inc(TIndex);

    until (TIndex+FPos) > Len;

  finally
    if EndPos >= 0 then
      FPos := EndPos + 1;
  end;

end;


function TIPEqTextParser.ParseVariable:TIPEqToken;
var
  TCount : Integer;
begin

  //First character should be a '~' so skip it.
  Inc(FPos);
  
  if Assigned(FOnVarExists) and FOnVarExists(Self,FText,FPos,TCount) then
  begin
    FTokenVal := '~'+Copy(FText,FPos,TCount);
    FPos := FPos+TCount;
    Result := ttVariable;
  end
  else
  begin
    Dec(FPos);
    Result := ttNone;
  end;

end;

procedure TIPEqTextParser.SetCaseSensitive(const Value: Boolean);
begin
  if Value <> FCaseSensitive then
  begin
    FCaseSensitive := Value;
    if Assigned(FFunctionList) then
      FFunctionList.CaseSensitive := FCaseSensitive;
  end;
end;

end.
