unit TDXExpression;

interface

uses TDXExpr,Variants,Classes,CocoBase;

type

  TTDXExpression = class(TPersistent)
  private
    FExprText : String;
    FExpr : TTDXExpr;
    FAllowEmptyExpression : boolean;
    FErrorInExpr : boolean;
    FLastExpr : String;
    FLastErrorCol : Integer;
    FLastErrorLine : Integer;
    FOnSymVarCreate : TTDXSymVarCreateEvent;
    FOnSymVarGetValue: TTDXSymVarGetValueEvent;
    FOnObjectGetValue: TTDXObjectGetValueEvent;
    FOnObjectCheckSyntax: TTDXObjectCheckSyntaxEvent;
    FMathExpand : boolean;
    FMMASyntax : boolean;
    FForceAltTrigPowerForm : boolean;
    procedure SetExprText(Value:String);
    procedure SetOnObjectGetValue(Value:TTDXObjectGetValueEvent);
    procedure SetOnObjectCheckSyntax(Value:TTDXObjectCheckSYntaxEvent);
    function GetValue: TTDXExprValue;
    procedure ParserError (Sender : TObject; Error : TCocoError);
    function GetEqText:String;
    function GetEqTextExpr:String;
    function GetEqTextValue : TTDXExprValue;
    function GetErrorInExpr:boolean;
    function GetIsEmpty:Boolean;
  protected
  public
    Constructor Create(Value: string); overload;
    Constructor CreateExpand();
    Destructor Destroy; override;
    procedure Assign(Source:TPersistent); override;
    procedure ParseExpression;
    procedure Reset;
    property ExprText:String read FExprText write SetExprText;
    property Value:TTDXExprValue read GetValue;
    property EqText:String read GetEqText;
    property EqTextExpr:String read GetEqTextExpr;
    property EqTextValue:TTDXExprValue read GetEqTextValue;
    property AllowEmptyExpression:boolean read FAllowEmptyExpression write FAllowEmptyExpression;
    property ErrorInExpr:boolean read GetErrorInExpr;
    property LastErrorCol:Integer read FLastErrorCol;
    property LastErrorLine:Integer read FLastErrorLine;
    property OnSymVarCreate:TTDXSymVarCreateEvent read FOnSymVarCreate write FOnSymVarCreate;
    property OnSymVarGetValue:TTDXSymVarGetValueEvent read FOnSymVarGetValue write FOnSymVarGetValue;
    property OnObjectGetValue:TTDXObjectGetValueEvent read FOnObjectGetValue write SetOnObjectGetValue;
    property OnObjectCheckSyntax:TTDXObjectCheckSyntaxEvent read FOnObjectCheckSyntax write SetOnObjectCheckSyntax;
    property Expr:TTDXExpr read FExpr;
    property MathExpand:boolean read FMathExpand write FMathExpand;
    property MMASyntax:boolean read FMMASyntax write FMMASyntax;
    property ForceAltTrigPowerForm:boolean read FForceAltTrigPowerForm write FForceAltTrigPowerForm;
    property IsEmpty:Boolean read GetIsEmpty;
  end;


implementation

uses sysutils, TDXExprParserX,dialogs;

  Constructor TTDXExpression.CreateExpand();
  begin
    Create;
    AllowEmptyExpression := true;
  end;

  Constructor TTDXExpression.Create(Value: string);
  begin
    SetExprText(Value);
  end;

  Destructor TTDXExpression.Destroy;
  begin
    inherited Destroy;
    FExpr.Free;
  end;

  procedure TTDXExpression.Assign(Source:TPersistent);
  var
    Src : TTDXExpression;
  begin

    if not Source.InheritsFrom(TTDXExpression) then
    begin
      inherited Assign(Source);
      Exit;
    end;

    Src := Source as TTDXExpression;
    FExprText := Src.ExprText;
    FAllowEmptyExpression := Src.AllowEmptyExpression;
  end;

  function TTDXExpression.GetIsEmpty:Boolean;
  begin
    Result := Trim(FExprText) = '';
  end;

  procedure TTDXExpression.SetOnObjectGetValue(Value:TTDXObjectGetValueEvent);
  begin
    FOnObjectGetValue := Value;
    Reset;
  end;

  procedure TTDXExpression.SetOnObjectCheckSyntax(Value:TTDXObjectCheckSYntaxEvent);
  begin
    FOnObjectCheckSyntax := Value;
    Reset;
  end;


  procedure TTDXExpression.SetExprText(Value:String);
  begin
    if Value <> FExprText then
    begin
      Reset;
      FExprText := Value;
    end;
  end;

  procedure TTDXExpression.Reset;
  begin
    FLastExpr := '';
    FErrorInExpr := false;
    FreeAndNil(FExpr);
  end;

  function TTDXExpression.GetErrorInExpr:boolean;
  begin
    ParseExpression;
    Result := FErrorInExpr;
  end;

  function TTDXExpression.GetEqText:String;
  begin
    ParseExpression;
    try
      if Assigned(FExpr) then
        Result := GetExprEqText(FExpr)
      else
        Result := Null;
    except
      on E:Exception do
      begin
        raise Exception.CreateFmt('Error evaluating expression: %s'#13#10'%s',[FExprText,E.Message]);
      end;
    end;
  end;


  function TTDXExpression.GetEqTextExpr:String;
  begin
    ParseExpression;
    try
      if Assigned(FExpr) then
        Result := GetExprEqText(FExpr,true)
      else
        Result := Null;
    except
      on E:Exception do
      begin
        raise Exception.CreateFmt('Error evaluating expression: %s'#13#10'%s',[FExprText,E.Message]);
      end;
    end;
  end;

  function TTDXExpression.GetEqTextValue : TTDXExprValue;
  begin
    ParseExpression;
    try
      if Assigned(FExpr) then
        Result := FExpr.ExprText
      else
        Result := Null;
    except
      on E:Exception do
      begin
        raise Exception.CreateFmt('Error evaluating expression: %s'#13#10'%s',[FExprText,E.Message]);
      end;
    end;
  end;

  function TTDXExpression.GetValue: TTDXExprValue;
  begin
    ParseExpression;
    try
      if Assigned(FExpr) then
        Result := FExpr.ExprValue
      else
        Result := Null;
    except
      on E:Exception do
      begin
        raise Exception.CreateFmt('Error evaluating expression: %s'#13#10'%s',[FExprText,E.Message]);
      end;
    end;
  end;


  procedure TTDXExpression.ParseExpression;
  var
    Parser : TTDXExprParserX;
    Strm   : TStringStream;
    S : String;
  begin

    //If expand variables is true then we need to reexpand each time to see if the
    //resultant expression text is different.
    if Assigned(FExpr) then
      Exit;

    FErrorInExpr := true;

    if (Trim(FExprText) = '') and FAllowEmptyExpression then
      FExpr := TTDXExprLiteral.Create(0)
    else
    begin
      FLastExpr := FExprText;

      S := FLastExpr;

      Parser := TTDXExprParserX.Create(nil);
      Parser.PreParse(S);
      Strm := TStringStream.Create(S);
//Expand variables first this will unroll all symbolics.   10/8/02
//*** THIS SCREWS UP NOW WHEN NUMBERS ARE COMMA FORMATTED.  NEED TO Expand in a more controlled way.
      try
        Parser.IsMath := FMMASyntax;
        Parser.ForceAltTrigPowerFOrm := FForceAltTrigPowerForm;
        FLastErrorCol := 0;
        FLastErrorLine := 0;
        Parser.OnError := ParserError;
        Parser.OnSymVarCreate := FOnSymVarCreate;
        Parser.OnSymVarGetValue := FOnSymVarGetValue;
        Parser.OnObjectGetValue := FOnObjectGetValue;
        Parser.OnObjectCheckSyntax := FOnObjectCheckSyntax;
        Parser.SourceStream.LoadFromStream(Strm);
        Parser.Execute;
        FExpr := Parser.RootNode;
        FErrorInExpr := false;
      finally
        Parser.Free;
        Strm.Free;
      end;
    end;
  end;

  procedure TTDXExpression.ParserError (Sender : TObject; Error : TCocoError);
  var
    ErrString:String;
  begin
    with (Sender as TTDXExprParserX) do
    begin
      FLastErrorCol := Error.Col;
      FLastErrorLine := Error.Line;
      ErrString := ErrorStr(Error.ErrorCode,Error.Data);
    end;
    raise Exception.CreateFmt('Error in expression: %s'#13#10'%s near column %d',[FExprText,ErrString,Error.Col-1]);//Typo C446 DV 2/25/05
  end;

end.
