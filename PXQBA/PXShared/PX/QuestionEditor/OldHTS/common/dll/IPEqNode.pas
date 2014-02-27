unit IPEqNode;

interface

uses graphics,windows,types,classes,controls,CocoBase;

var
  EqDefaultFont : TFont;

const
  EQ_SMALLFONTSCALE = 0.66;
  EQ_MINFONTSIZE = 8;

  SP_SUPERSCRIPTHEIGHT = 45;  //Distance from bottom of left and bottom of right
  SP_SUPERSCRIPTMIN    = 30;  //Distance from bottom of left and bottom of right
  SP_SUPSCRIPTDEPTH = 25;     //Distance from bottom of left and bottom of right
  SP_SUBSUPGAP = 8;           //Distance from right of left and left of right
  SP_NUMERATORHEIGHT = 35;    //Distance form divisor line and baseline of numerator(left)
  SP_DENOMINATORDEPTH = 100;  //Distance from divisor line to bottom of denominator (right)
  SP_MINIMUMGAP = 10 ;       //Minimum Distance from top of numerator to divisor line
  SP_FRACTIONBAROVERHANG = 8; //Size of divisor line overhang
  SP_FRACTIONBARMARGIN = 12;    //Whitespace to each side of fraction bar
  SP_FRACTIONBARTHICKNESS = 5; //Thickness of fraction bar
  SP_SUBFRACTIONBARTHICKNESS = 2;  //Thickness of subraction bar
  SP_FENCEOVERHAND = 8;
  SP_HORIZONTALFENCEGAP = 10;

type


  TIPEqCaretDirection = (cdNone,cdLeft,cdRight,cdUp,cdDown,cdHome,cdEnd,cdTab,cdBackTab);

  TIPEqNode = class;
  TIPEqRow = class;

  TIPEqEnumEvent = procedure(Sender : TObject; Node : TIPEqNode; var Stop : Boolean;
                 UserData : Pointer; Level:Integer) of object;

  TIPEqUndoProcedure = procedure of object;

  TIPEqGetVarTextEvent = procedure(Sender:TObject; const VarName:String;
     var varValue:String) of object;

  TIPEqOnVarExistsEvent = function(Sender:TObject; const Buf:string; BufStart:Integer; var NCount:Integer):boolean of object;

  TIPEqCurrency = (dollar, pound, euro, yen);

  TIPEqCaretEvent = class
    private
      FDirection : TIPEqCaretDirection;
      FRow : TIPEqRow;
      FPosition : Integer;
      FPositionstart : Integer;
      FExtendSelection : boolean;
      FCharacterDeleted : boolean;
      FStoreUndoState : TIPEqUndoProcedure;
      FUpdateUndoState : TIPEqUndoProcedure;
    public
      constructor Create(ADirection:TIPEqCaretDirection; ARow:TIPEqRow; AStartPosition,APosition:Integer);
      property Row:TIPEqRow read FRow write FRow;
      property Position:Integer read FPosition write FPosition;
      property PositionStart:Integer read FPositionstart write FPositionstart;
      property ExtendSelection:boolean read FExtendSelection write FExtendSelection;
      property Direction: TIPEqCaretDirection read FDirection write FDirection;
      property CharacterDeleted:boolean read FCharacterDeleted write FCharacterDeleted;
      property StoreUndoState : TIPEqUndoProcedure read FStoreUndoState write FStoreUndoState;
      property UpdateUndoState : TIPEqUndoProcedure read FUpdateUndoState write FUpdateUndoState;
  end;

  TIPEqCharEvent = class
    private
      FChar : Char;
      FRow  : TIPEqRow;
      FPosition : Integer;
    public
      constructor Create(AChar:Char; ARow:TIPEqRow; APosition:Integer);
      property InsertedChar: Char read FChar write FChar;
      property Row:TIPEqRow read FRow write FRow;
      property Position:Integer read FPosition write FPosition;
  end;

  TIPEqMetrics = class
    private
      FDescent : Integer;
      FAscent  : Integer;
      FWidth   : Integer;
      FEm     : Integer;
      function GetHeight:Integer;
    public
      Constructor Create(AAscent,ADescent,AWidth:Integer); overload;
      Constructor Create(AAscent,ADescent,AWidth,AEm:Integer); overload;
      function EmPart(Percent:Integer):Integer;
      property Ascent:Integer read FAscent write FAscent;
      property Descent:Integer read FDescent write FDescent;
      property Width:Integer read FWidth write FWidth;
      property Height:Integer read GetHeight;
      property Em:Integer read FEm write FEm;

  end;

  TIPEqList = class;
  TIPEqDocument = class;
  TIPEqNodeList = array of TIPEqNode;


  TIPEqCreateControlEvent = function(Sender:TIPEqDocument; AName:String):TControl of object;

  TIPEqNode = class(TPersistent)
    private
      FParent : TIPEqList;
      FMetrics : TIPEqMetrics;
      FCalculatingMetrics : Boolean;
      FXLoc : Integer;
      FYLoc : Integer;
      FFont : TFont;
      FRemoving : Boolean;
      FDocumentCache : TIPEqDocument;
      FLayedOut : boolean;
      procedure SetFont(AFont:TFont);
      function GetFont:TFont;
      function GetMetrics:TIPEqMetrics;
      function GetWidth:Integer;
      function GetHeight:Integer;
      function GetAscent:Integer;
      function GetDescent:Integer;
      function GetXLoc:Integer;
      function GetYLoc:Integer;
      function GetEm:Integer;
      function GetDocument:TIPEqDocument;
    protected
      function GetTextMetrics(AFont:TFont = nil):TTextMetric;
      function GetTextExtent(const Txt:String; AFont:TFont = nil):TSize;
      function CalcMetrics:TIPEqMetrics; virtual; abstract;
      procedure Layout; virtual; abstract;
      procedure Draw(ACanvas:TCanvas); virtual; abstract;
      function GetEmPart(Percent:Integer):Integer; overload;
      function GetEmPart(Percent:Integer; AEm:Integer):Integer; overload;
    protected
      function GetLeftOffset: integer; virtual;
    public
      constructor Create;
      destructor Destroy; override;
      procedure Paint(ACanvas:TCanvas); virtual;
      procedure InvalidateAll; virtual;
      procedure Invalidate; virtual;
      procedure Validate; virtual;
      procedure SetLocation(X,Y:Integer); virtual;
      procedure DoLayout;
      function Contains(X,Y:Integer):boolean; virtual;
      function GetNodeAt(X,Y:Integer):TIPEqNode; virtual;
      function GetCaretLocation(pos:Integer):TRect; virtual;
      function GetComponentLocation : TPoint; overload;
      function GetComponentLocation(X,Y:Integer):TPoint; overload;
      function GetCaretPositionAt(X,Y:Integer):Integer; virtual;
      function GetLastRow:TIPEqRow; virtual;
      function GetFirstRow:TIPEqRow; virtual;
      function InsertNode(Node:array of TIPEqNode; Position:Integer):TIPEqNode; virtual;
      function toString:String; virtual;
      procedure DeleteCharacter(CaretEvent:TIPEqCaretEvent); virtual;
      function GetLastCaretPosition:Integer; virtual;
      function isEmpty:Boolean; virtual;
      function IsNumber:boolean; virtual;
      procedure InitCaret(var FocusedRow: TIPEqRow; var CaretPos:Integer); virtual;
      function GetSelectedRect(Pos1,Pos2:Integer):TRect; virtual;
      procedure InitFont; virtual;
      procedure ReduceFontSize; virtual;
      procedure IncreaseFontSize(Pts:Integer = 1); virtual;
      procedure SplitNode(Pos:Integer); virtual;
      function  Clone:TIPEqNode; virtual; abstract;
      procedure BuildMathML(Buffer:TStrings; Level:Integer); virtual; abstract;
      function  GetText:String; virtual; abstract;
      function  CheckParens:boolean; virtual;
      function  RowsFilled:boolean; virtual;
      function  IsInteger:boolean; virtual;
      function  GetParentRow:TIPEqRow;

      property XLoc:Integer read GetXLoc write FXLoc;
      property YLoc:Integer read GetYLoc write FYLoc;
      property Metrics:TIPEqMetrics read GetMetrics;
      property Font:TFont read GetFont write SetFont;
      property Width:Integer read GetWidth;
      property Height:Integer read GetHeight;
      property Ascent:Integer read GetAscent;
      property Descent:Integer read GetDescent;
      property Em:Integer read GetEm;
      property Parent: TIPEqList read FParent;
      property Document:TIPEqDocument read GetDocument;
      property Text:String read GetText;
      property leftOffset:integer read GetLeftOffset;
  end;


  TIPEqList = class (TIPEqNode)
    private
      FChildren : TList;
      function GetChildCount:Integer;
      function GetChild(Index:Integer):TIPEqNode;
    protected
      procedure PaintChildren(ACanvas:TCanvas); virtual;
      procedure AddNodes(Nodes:TIPEqNodeList); virtual;
    public
      constructor Create;
      destructor Destroy; override;
      procedure FreeAllChildren;
      procedure InvalidateAll; override;
      procedure Validate; override;
      procedure Paint(ACanvas:TCanvas); override;
      function  AddChild(Node:TIPEqNode):TIPEqNode; overload;
      function  AddChild(Node:TIPEqNode; Position:Integer):TIPEqNode; overload;
      function  AddChildAfter(AChild:TIPEqNode; Position:Integer):TIPEqNode;
      procedure AddChildrenAfter(Children: array of TIPEqNode; Node:TIPEqNode); overload;
      procedure AddChildrenAfter(Children: array of TIPEqNode; Position:Integer); overload;
      procedure AddChildrenBefore(Children: array of TIPEqNode; Node:TIPEqNode); overload;
      procedure AddChildrenBefore(Children: array of TIPEqNode; Position:Integer); overload;
      procedure AddChildren(Children: array of TIPEqNode);
      function  GetChildIndex(Node:TIPEqNode):Integer;
      procedure GotoPrevRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent); virtual;
      procedure GotoNextRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent); virtual;
      procedure CopyChildren(SourceNodeList:TIPEqList);
      function GetLastChild:TIPEqNode;
      function GetNodeAt(X,Y:Integer):TIPEqNode; override;
      function RemoveChild(Node:TIPEqNode):TIPEqNode;
      function RemoveChildAt(Index:Integer):TIPEqNode;
      function GetNodeList:TIPEqNodeList;
      procedure Clear; virtual;
      procedure ReduceFontSize; override;
      function  CheckParens:boolean; override;
      function  RowsFilled:boolean; override;


      property ChildCount:Integer read GetChildCount;
      property Child[Index:Integer]:TIPEqNode read GetChild;
      property LastChild:TIPEqNode read GetLastChild;
  end;

  TIPEqRow = class(TIPEqList)
    private
      FFocusedPositions : array of Integer;
      FLastFocusedElement:Integer;
      FDummyOffset:Integer;
      FDisableTextMerge : boolean;
      procedure CalcFocusedPositions;
      function GetFocusedPosition(Index:Integer):Integer;
      function GetFocusedPositionCount:Integer;
      function GetFocusedElementIndex(Position:Integer):Integer;
      procedure MoveUp(CaretEvent:TIPEqCaretEvent);
      procedure MoveDown(CaretEvent:TIPEqCaretEvent);
      procedure MoveLeft(CaretEvent:TIPEqCaretEvent);
      procedure MoveRight(CaretEvent:TIPEqCaretEvent);
      procedure MoveHome(CaretEvent:TIPEqCaretEvent);
      procedure MoveEnd(CaretEvent:TIPEqCaretEvent);
      procedure TabForward(CaretEvent:TIPEqCaretEvent);
      procedure TabBack(CaretEvent:TIPEqCaretEvent);
      procedure MatchParens;
      procedure MergeTextFields;
      procedure ParseTextFields;
      procedure RemoveNonEmptyRows;
    protected
      function  CalcMetrics:TIPEqMetrics; override;
      procedure Layout; override;
      procedure Draw(ACanvas:TCanvas); override;
    public
      Constructor CreateDummy;
      function  Clone:TIPEqNode; override;
      function GetLastCaretPosition:Integer; override;
      procedure InvalidateAll; override;
      procedure Invalidate; override;
      function  GetCaretLocation(Position:Integer):TRect; override;
      function Contains(X,Y:Integer):boolean; override;
      procedure MoveCaret(CaretEvent:TIPEqCaretEvent);
      procedure GotoPrevRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent); override;
      procedure GotoNextRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent); override;
      function GetLastRow:TIPEqRow; override;
      function GetFirstRow:TIPEqRow; override;
      procedure InsertCharacter(CharEvent: TIPEqCharEvent);
      function InsertNode(Node:array of TIPEqNode; Position:Integer):TIPEqNode; override;
      function InsertNodeRange(Node:array of TIPEqNode;StartPos, EndPos:Integer; PrimaryNode:Integer):TIPEqNode;
      procedure InitCaret(var FocusedRow: TIPEqRow; var CaretPos:Integer); override;
      function GetNodeAt(X,Y:Integer):TIPEqNode; override;
      function GetCaretPositionAt(X,Y:Integer):Integer; override;
      procedure DeleteCharacter(CaretEvent:TIPEqCaretEvent); override;
      procedure DeleteSingleCharacter(CaretEvent:TIPEqCaretEvent);
      function GetSelectedRect(Pos1,Pos2:Integer):TRect; override;
      function IsDummyRow:Boolean;
      Function ExtractNodes(StartPos,EndPos:Integer):TIPEqNodeList;
      Function GetNodeBeforeCaret(CaretPos:Integer):TIPEqNode;
      procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
      function  GetText:String; override;
      function  RowsFilled:boolean; override;
      function  IsInteger:boolean; override;
      function IsNumber:boolean; override;
      function GetEqPosition:Integer;

      property FocusedPosition[Index:Integer]:Integer read GetFocusedPosition;
      property FocusedPositionCount:Integer read GetFocusedPositionCount;
end;


  TIPEqDocument = class(TIPEqRow)
    private
      FEmptyBoxPen : TPen;
      FEmptyBoxBrush : TBrush;
      FFunctionList : TStringList;
      FItalicizeVariables : Boolean;
      FUseSymbolFont:Boolean;
      FEnabled : boolean;
      FNumberColor : TColor;
      FVarCOlor : TCOlor;
      FUnmatchedParenColor : TColor;
      FLastErrorPos : Integer;
      FOnGetVarText:TIPEqGetVarTextEvent;
      FOnVarExists:TIPEqOnVarExistsEvent;
      FEqText : String;
      FAllowCommaNumbers : boolean;
      FAllowDollarNumbers : Boolean;
      FTextOnly : boolean;
      FInterpretFunctions : boolean;
      FOnCreateControl : TIPEqCreateControlEvent;
      FContainer : TControl;
      FEqObjects : TList;
      FEditable : boolean;
      FAuthoring : boolean;
      FAlignByEq : boolean;
      FCurrencyType : TIPEqCurrency;
      procedure SetUseSymbolFont(Value:boolean);
      procedure SetItalicizeVariables(Value:boolean);
      procedure SetEmptyBoxPen(Value:TPen);
      procedure SetEmptyBoxBrush(Value:TBrush);
      function  GetMathML:String;
      procedure SetEnabled(Value:boolean);
      procedure ParserError (Sender : TObject; Error : TCocoError);
      function  GetTextOnly:String;
      procedure SetInterpretFunctions(Value:boolean);
      procedure SetFunctionList(StrList:TStringList);
      function  GetEqControlCount:Integer;
      function  GetEqControl(Index:Integer):TControl;
    protected
    public
      constructor Create;
      destructor Destroy; override;
      procedure Clear; override;
      procedure Paint(ACanvas:TCanvas); override;
      function  Clone:TIPEqNode; override;
      Function CreateNode(Ch:Char):TIPEqNode;
      function  GetControl(AName:String):TControl;
      procedure SetText(const AText:String);
      procedure SetControlVOffset(AControl:TControl; VOffset:Integer);
      procedure UpdateControlSize(AControl:TControl);
      procedure RemoveEqObject(Node:TIPEqNode);
      procedure AddEqObject(Node:TIPEqNode);
      property MathML:String read GetMathML;
      property Enabled:Boolean read FEnabled write SetEnabled;
      property LastErrorPos : Integer read FLastErrorPos;
      property TextOnly:String read GetTextOnly;
      property EqControlCount:Integer read GetEqControlCount;
      property EqControl[Index:Integer]:TControl read GetEqControl;
    published
      property EmptyBoxPen:TPen read FEmptyBoxPen write SetEmptyBoxPen;
      property EmptyBoxBrush:TBrush read FEmptyBoxBrush write SetEmptyBoxBrush;
      property ItalicizeVariables:boolean read FItalicizeVariables write SetItalicizeVariables;
      property UseSymbolFont:boolean read FUseSymbolFont write SetUseSymbolFont;
      property NumberCOlor:TColor read FNumberCOlor write FNumberColor;
      property VarCOlor:TColor read FVarCOlor write FVarColor;
      property UnMatchedParenColor:TColor read FUnMatchedParenColor write FUnMatchedParenColor;
      property OnGetVarText:TIPEqGetVarTextEvent read FOnGetVarText write FOnGetVarText;
      property OnVarExists:TIPEqOnVarExistsEvent read FOnVarExists write FOnVarExists;
      property AllowCommaNumbers:boolean read FAllowCommaNumbers write FAllowCommaNumbers;
      property AllowDollarNumbers:boolean read FAllowDollarNumbers write FAllowDollarNumbers;
      property InterpretFunctions:boolean read FInterpretFunctions write SetInterpretFunctions;
      property FunctionList:TStringList read FFunctionList write SetFunctionList;
      property OnCreateControl:TIPEqCreateControlEvent read FOnCreateControl write FOnCreateControl;
      property Container:TControl read FContainer write FContainer;
      property Editable:boolean read FEditable write FEditable;
      property Authoring:boolean read FAuthoring write FAuthoring;
      property AlignByEq:boolean read FAlignByEq write FAlignByEq;
      property CurrencyType:TIPEqCurrency read FCurrencyType write FCurrencyType;      
  end;

const


  IPEqDefFunctions : array[0..27] of string = (
    'abs','sin','cos','tan','arcsin','arccos','arctan',
    'sinh','cosh','tanh','arcsinh','arccosh','arctanh',
    'ln','log','exp','e','i','sec','csc','cot',
    'arcsec','arccsc','arccot','csch','sech','coth','lim'
  );

  CurrencyChars : array[TIPEqCurrency] of Char =
    ('$',#0163,#0128,#0165);


  Function MathMLEncode(Value:String):String;

  Function EqEncode(const Value:String):String;

  procedure AssignDefFunctions(StrList:TStringList);


implementation

  uses Math, IPEqUtils,sysutils, IPEqComposite, IPEqStack,
  IPEqSuperScript, IPEqParen, IPEqText, IPEqTextParser, IPEqOp,StStrL,
  IPEqDivide, IPEqSqrt, FormEqInfo, IPEqSymbolW, IPEqParser,IPEqChar,
  IPEqObject,IPEqPlainText;


  Function MathMLEncode(Value:String):String;
  begin
    Result := Value;
  end;

  Function EqEncode(const Value:String):String;
  var
    I : integer;
  begin
    Result := '';
    for I := 1 to Length(Value) do
    begin
      if Value[I] in ['\','&','{','}','@','^'] then
        Result := Result + '\' + Value[I]
      else
        Result := Result + Value[I];
    end;
  end;

  procedure AssignDefFunctions(StrList:TStringList);
  var
   I  : integer;
  begin
    for I := Low(IPEqDefFunctions) to High(IPEqDefFunctions) do
      StrList.Add(IPEqDefFunctions[I]);
  end;

  function FindControl(EqDoc:TIPEqDocument; AControl:TControl):TIPEqObject;
  var
   I : integer;
  begin
    with EqDoc do
    begin
      Result := nil;
      for I := 0 to FEqObjects.Count-1 do
      begin
        if TIPEqObject(FEqObjects[I]).Control = AControl then
        begin
          Result:= TIPEqObject(FEqObjects[I]);
          Exit;
        end;
      end;
    end;
  end;


{*************************************************************************}
{*********************** TIPEqCaretEvent Methods ******************************}
{*************************************************************************}

constructor TIPEqCaretEvent.Create(ADirection:TIPEqCaretDirection; ARow:TIPEqRow; AStartPosition,APosition:Integer);
begin
  inherited Create;
  FDirection := ADirection;
  FRow := ARow;
  FPosition := APosition;
  FPositionstart := AStartPosition;
end;



{*************************************************************************}
{*********************** TIPEqCaretEvent Methods ******************************}
{*************************************************************************}

constructor TIPEqCharEvent.Create(AChar:Char; ARow:TIPEqRow; APosition:Integer);
begin
  inherited Create;
  FChar := AChar;
  FRow := ARow;
  FPosition := APosition;
end;


{*************************************************************************}
{*********************** TIPEqMetrics Methods ******************************}
{*************************************************************************}

Constructor TIPEqMetrics.Create(AAscent,ADescent,AWidth:Integer);
begin
  inherited Create;
  FAscent := AAscent;
  FDescent := ADescent;
  FWidth := AWidth;
end;

Constructor TIPEqMetrics.Create(AAscent,ADescent,AWidth,AEm:Integer);
begin
  inherited Create;
  FAscent := AAscent;
  FDescent := ADescent;
  FWidth := AWidth;
  FEm := AEm;
end;

function TIPEqMetrics.EmPart(Percent:Integer):Integer;
begin
  EmPart := Round(Percent*FEm/100.0);
end;

function TIPEqMetrics.getHeight;
begin
  Result := FAscent+FDescent;

end;

{*************************************************************************}
{************************** TIPEqNode Methods ******************************}
{*************************************************************************}

constructor TIPEqNode.Create;
begin
  inherited Create;
  FParent := nil;
end;

destructor TIPEqNode.Destroy;
begin
  if Assigned(Parent) and not FRemoving then
    Parent.RemoveChild(Self);
  inherited Destroy;
  FMetrics.Free;
  FFont.Free;
end;

function TIPEqNode.toString:String;
begin
  Result := ClassName;
end;

function  TIPEqNode.IsInteger:boolean;
begin
  Result := false;
end;


function  TIPEqNode.IsNumber:boolean;
begin
  Result := false;
end;

function TIPEqNode.GetTextMetrics(AFont:TFont = nil):TTextMetric;
var
  DisplayDC: HDC;
  TxtMetric: TTEXTMETRIC;
begin
  if AFont = nil then
    AFont := Font;
  DisplayDC := GetDC(0);
  if (DisplayDC <> 0) then
  begin
    if (SelectObject(DisplayDC, AFont.Handle) <> 0) then
      Windows.GetTextMetrics(DisplayDC, TxtMetric);
    ReleaseDC(0, DisplayDC);
  end;
  Result := TxtMetric;
end;

function TIPEqNode.GetTextExtent(const Txt:String; AFont:TFont = nil):TSize;
begin
  if AFont = nil then
    AFont := Font;
  Result := GetTextSize(AFont,Txt);
end;

procedure TIPEqNode.DoLayout;
begin
  GetMetrics;
  if not FLayedOut then
  begin
    FLayedOut := true;
    Layout;
  end;
end;


function TIPEqNode.GetMetrics:TIPEqMetrics;
begin
  if not Assigned(FMetrics) and not FCalculatingMetrics then
  begin
    FCalculatingMetrics := true;
    try
      FLayedOut := false;
      FMetrics := CalcMetrics;
    finally
      FCalculatingMetrics := false;
    end;
  end;
  Result := FMetrics;
end;

function TIPEqNode.GetDocument:TIPEqDocument;
var
  Node : TIPEqNode;
begin
  if not Assigned(FDocumentCache) then
  begin
    Node := Self;
    repeat
      Node := Node.Parent;
    until (not Assigned(Node)) or (Node is TIPEqDocument);
    FDocumentCache := TIPEqDocument(Node);
  end;
  Result := FDocumentCache;
end;

function TIPEqNode.GetLastRow:TIPEqRow;
begin
  Result := nil;
end;

function TIPEqNode.GetFirstRow:TIPEqRow;
begin
  Result := nil;
end;

procedure TIPEqNode.InvalidateAll;
begin
  FMetrics.Free;
  FMetrics := nil;
  FLayedOut := false;
  if Assigned(Parent) then
  begin
    FFont.Free;
    FFont := nil;
  end;
end;

procedure TIPEqNode.Invalidate;
begin
  FMetrics.Free;
  FMetrics := nil;
  FLayedOut := false;
  if Assigned(Parent) then
  begin
    FFont.Free;
    FFont := nil;
  end;
  if Assigned(FParent) and Assigned(FParent.FMetrics) then
    FParent.invalidate;
end;


procedure TIPEqNode.Validate;
begin
  DoLayout;
end;

procedure TIPEqNode.SetFont(AFont:TFont);
begin
  if not Assigned(FFont) then
    FFont := TFont.Create;
  FFont.Assign(AFont);
  InvalidateAll;
end;

function TIPEqNode.GetFont:TFont;
begin
  if Assigned(FFont) then
    Result := FFont
  else if Assigned(FParent) then
    Result := FParent.Font
  else
    Result := EqDefaultFont;
end;

procedure TIPEqNode.SetLocation(X,Y:Integer);
begin
  FXLoc := X;
  FYLoc := Y;
end;

function TIPEqNode.GetWidth:Integer;
begin
  Result := Metrics.Width;
end;

function TIPEqNode.GetHeight:Integer;
begin
  Result := Metrics.Height;
end;

function TIPEqNode.GetAscent:Integer;
begin
  Result := Metrics.Ascent;
end;

function TIPEqNode.GetEm:Integer;
begin
  Result := Metrics.Em;
end;

function TIPEqNode.GetEmPart(Percent:Integer):Integer;
begin
  Result := Metrics.EmPart(Percent);
end;

function TIPEqNode.GetEmPart(Percent:Integer;AEm:Integer):Integer;
begin
  Result := Round(Percent*AEm/100.0);
end;

function TIPEqNode.GetDescent:Integer;
begin
  Result := Metrics.Descent;
end;

function TIPEqNode.GetXLoc:Integer;
var
  Node : TIPEqNode;
begin
  Node := Self;
  while Assigned(Node.Parent) and not Node.Parent.FLayedOut do
    Node := Node.Parent;
  if Node <> Self then
    Node.Validate;
  Result := FXLoc;
end;

function TIPEqNode.GetYLoc:Integer;
var
  Node : TIPEqNode;
begin
  Node := Self;
  while Assigned(Node.Parent) and not Node.Parent.FLayedOut do
    Node := Node.Parent;
  if Node <> Self then
    Node.Validate;
  Result := FYLoc;
end;


procedure TIPEqNode.Paint(ACanvas:TCanvas);
var
  FontRecall : TFontRecall;
  PenRecall : TPenRecall;
begin

  //Call this to make sure we have all sizing and layout info set
  Validate;

  FontRecall := nil;
  PenRecall := nil;
  if Assigned(FFont) then
  begin
    FontRecall := TFontRecall.Create(ACanvas.Font);
    PenRecall := TPenRecall.Create(ACanvas.Pen);
    ACanvas.Font := FFont;
    Acanvas.Pen.Color := FFont.Color;
  end;
  try
    Draw(ACanvas);
  finally
    PenRecall.Free;
    FontRecall.Free;
  end;
end;

function TIPEqNode.Contains(X,Y:Integer):boolean;
begin
  Result := (X >= 0) and (X < Width) and (Y >= 0) and (Y < Height);
end;

function TIPEqNode.GetNodeAt(X,Y:Integer):TIPEqNode;
begin
  DoLayout;
  X := X - FXLoc;
  Y := Y - FYLoc;
  if Contains(X,Y) then
    Result := Self
  else
    Result := nil;
end;


function TIPEqNode.GetLastCaretPosition:Integer;
begin
  Result := 1;
end;

function TIPEqNode.GetComponentLocation : TPoint;
begin
  Result := GetComponentLocation(0,0);
end;

function  TIPEqNode.GetParentRow:TIPEqRow;
var
  N : TIPEqNode;
begin
  N := Parent;
  while Assigned(N) and not (N is TIPEqRow) do
    N := N.Parent;
  Result := TIPEqRow(N);
end;


function TIPEqNode.GetComponentLocation(X,Y:Integer):TPoint;
var
  Xc,Yc : Integer;
  Node : TIPEqList;
begin
  DoLayout;
  Xc := XLoc+X;
  Yc := YLoc+Y;
  Node := FParent;
  while Assigned(Node) do
  begin
    Xc := Xc + Node.XLoc;
    Yc := Yc + Node.YLoc;
    Node := Node.Parent;
  end;
  Result.X := Xc;
  Result.Y := Yc;
end;


{ Assume two Positions 0 --> Begining and 1--> End }
function TIPEqNode.GetCaretLocation(Pos:Integer):TRect;
var
  Pt : TPoint;
begin
  Pt := GetComponentLocation;
  if Pos <= 0 then
    Result := Bounds(Pt.X-1,Pt.Y,1,Height)
  else
    Result := Bounds(Pt.X-1+Width,Pt.Y,1,Height);
end;

{Assume two Positions}
function TIPEqNode.GetCaretPositionAt(X,Y:Integer):Integer;
var
  Pt: TPoint;
begin
  Pt := GetComponentLocation(Width div 2,0);
  if X <= Pt.X then
    Result := 0
  else
    Result := 1;
end;

function TIPEqNode.InsertNode(Node:array of TIPEqNode; Position:Integer):TIPEqNode;
begin
  if Position = 0 then
     Parent.AddChildrenBefore(Node,Self)
  else
     Parent.AddChildrenAfter(Node,Self);
  Result := Node[0];
end;

procedure TIPEqNode.DeleteCharacter(CaretEvent:TIPEqCaretEvent);
begin
end;

function TIPEqNode.isEmpty:Boolean;
begin
  Result := false;
end;


procedure TIPEqNode.InitCaret(var FocusedRow: TIPEqRow; var CaretPos:Integer);
var
  Index : Integer;
begin
  if Parent.InheritsFrom(TIPEqRow) then
  begin
    FocusedRow := TIPEqRow(Parent);
    Index := FocusedRow.GetChildIndex(Self);
    CaretPos := FocusedRow.FocusedPosition[Index]+GetLastCaretPosition;
  end;
end;


function TIPEqNode.GetSelectedRect(Pos1,Pos2:Integer):TRect;
var
  Pt : TPoint;
begin
  Pt := GetComponentLocation;
  Result := Bounds(Pt.X,Pt.Y,Width,Height);
end;

procedure TIPEqNode.IncreaseFontSize(Pts:Integer = 1);
begin
  InvalidateAll;
  InitFont;
  Font.Size := Max(1,Font.Size+Pts);
end;

procedure TIPEqNode.ReduceFontSize;
begin
  InvalidateAll;
  InitFont;
  Font.Size := Max(EQ_MINFONTSIZE,Round(EQ_SMALLFONTSCALE*Font.Size));
end;

procedure TIPEqNode.InitFont;
var
  Siz : Integer;
begin

  if not Assigned(FFont) then
    FFont := TFont.Create;

  if Assigned(Parent) then
  begin
    FFont.Assign(Parent.Font);
    if Parent.Font.PixelsPerInch <> FFont.PixelsPerInch then
    begin
      Siz := FFont.Size;
      FFont.PixelsPerInch := Parent.Font.PixelsPerInch;
      FFont.Size := Siz;
    end;
  end
  else
    FFont.Assign(EqDefaultFont);

end;

procedure TIPEqNode.SplitNode(Pos:Integer);
begin
end;

function  TIPEqNode.CheckParens:boolean;
begin
  Result := true;
end;

function  TIPEqNode.RowsFilled:boolean;
begin
  Result := true;
end;


{*************************************************************************}
{************************** TIPEqList Methods ******************************}
{*************************************************************************}

constructor TIPEqList.Create;
begin
  inherited Create;
  FChildren := TList.Create;
end;



destructor TIPEqList.Destroy;
begin
  inherited Destroy;
  FreeAllChildren;
  FChildren.Free;
end;

procedure TIPEqList.CopyChildren(SourceNodeList:TIPEqList);
var
  I : Integer;
begin
  FreeAllChildren;
  for I := 0 to SourceNodeList.ChildCount -1 do
  begin
    AddChild(SourceNodeList.Child[I].Clone);
  end;
end;


procedure TIPEqList.FreeAllChildren;
var
  I : integer;
begin
  for I := 0 to FChildren.Count-1 do
  begin
    with TIPEqNode(FChildren.Items[I]) do
    begin
      FRemoving := true;
      Free;
    end;
  end;
  FChildren.Clear;
end;

function TIPEqList.GetChildCount:Integer;
begin
  Result := FChildren.Count;
end;


function TIPEqList.GetChild(Index:Integer):TIPEqNode;
begin
  Result := TIPEqNode(FChildren.Items[Index]);
end;

function  TIPEqList.GetChildIndex(Node:TIPEqNode):Integer;
begin
  Result := FChildren.IndexOf(Node);
end;

procedure TIPEqList.InvalidateAll;
var
 I : Integer;
begin
  inherited InvalidateAll;
  for I := 0 to ChildCount-1 do
  begin
    Child[I].InvalidateAll;
  end;
end;


procedure TIPEqList.Validate;
var
 I : Integer;
begin
  begin
    //I moved validate to the top to get it to set the font correctly.  However,
    //I should really have the sizes of all children first.  In theory, they should
    //be gotten on deman when access metrics.  Let's see if we  have any strange
    //side affects.   12/28/05 - MAD
    inherited Validate;  //This is just an experiment.

    for I := 0 to ChildCount-1 do
      Child[I].Validate;
  end;
end;

procedure TIPEqList.Paint(ACanvas:TCanvas);
var
  FontRecall : TFontRecall;
  PenRecall : TPenRecall;
begin

  FontRecall := nil;
  PenRecall := nil;
  if Assigned(FFont) then
  begin
    FontRecall := TFontRecall.Create(ACanvas.Font);
    PenRecall := TPenRecall.Create(ACanvas.Pen);
    ACanvas.Font := FFont;
    ACanvas.Pen.Color := FFont.Color;
  end;
  try
    //Call this to make sure we have all sizing and layout stuff set
    Validate;
    PaintChildren(ACanvas);
    inherited Paint(ACanvas);
  finally
    PenRecall.Free;
    FontRecall.Free;
  end;
end;


procedure TIPEqList.PaintChildren(ACanvas:TCanvas);
var
 I : Integer;
 Eq : TIPEqNode;
 X,Y : Integer;
 Dc : HDC;
begin
  for I := 0 to ChildCount-1 do
  begin
    Eq := Child[I];
    X := Eq.XLoc;
    Y := Eq.YLoc;
    Dc := ACanvas.Handle;
    MoveWindowOrg(Dc,X,Y);
    try
      Eq.Paint(ACanvas);
    finally
     MoveWindowOrg(Dc,-X,-Y);
    end;
  end;
end;

{Note: this routine will not free child}
function TIPEqList.RemoveChildAt(Index:Integer):TIPEqNode;
var
  Node : TIPEqNode;
begin
  Node := FChildren[Index];
  Node.FParent := nil;

  if Node is TIPEqObject then
    Document.FEqObjects.Remove(Node);

  FChildren.Delete(Index);
  Result := Node;
  Invalidate;
end;


function TIPEqList.RemoveChild(Node:TIPEqNode):TIPEqNode;
begin
  Node.FParent := nil;
  if Node is TIPEqObject then
    Document.FEqObjects.Remove(Node);
  FChildren.Remove(Node);
  Result := Node;
  Invalidate;
end;

procedure TIPEqList.GotoPrevRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent);
var
  Index : Integer;
  R : TIPEqRow;
begin
  Index := -1;
  if Assigned(Node) then
    Index := GetChildIndex(Node);

  //If Row can not be round, start looking from right
  if Index < 0 then
    Index := ChildCount;

  Dec(Index);

  if Index >= 0 then
  begin
    R := Child[Index].GetLastRow;
    if Assigned(R) then
    begin
      CaretEvent.Row := R;
      CaretEvent.Position := R.GetLastCaretPosition;
    end;
  end
  else
  begin
    if Assigned(Parent) then
      Parent.GotoPrevRow(Self,CaretEvent);
  end;

end;

procedure TIPEqList.GotoNextRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent);
var
  Index : Integer;
  R : TIPEqRow;
begin
  Index := -1;
  if Assigned(Node) then
    Index := GetChildIndex(Node);
  Inc(Index);
  if Index < ChildCount then
  begin
    R := Child[Index].GetFirstRow;
    if Assigned(R) then
    begin
      CaretEvent.Row := R;
      CaretEvent.Position := 0;
    end;
  end
  else
  begin
    if Assigned(Parent) then
      Parent.GotoNextRow(Self,CaretEvent);
  end;
end;

function TIPEqList.GetLastChild:TIPEqNode;
begin
  if ChildCount > 0 then
    Result := Child[ChildCount-1]
  else
    Result := nil;
end;

function  TIPEqList.AddChild(Node:TIPEqNode):TIPEqNode;
begin
  Result := AddChild(Node,-1);
end;

function  TIPEqList.AddChild(Node:TIPEqNode; Position:Integer):TIPEqNode;
begin
  if Assigned(Node.FParent) then
    Node.FParent.removeChild(Node);

  if (Position < 0) or (Position >= ChildCount) then
    FChildren.Add(Node)
  else
    FChildren.Insert(Position,Node);
  Node.FParent := Self;
  
  if (Node is TIPEqObject) and (Document <> nil) then
  begin
    Document.RemoveEqObject(Node);
    Document.AddEqObject(Node);
  end;

  Result := TIPEqNode(Node);
  Node.Invalidate;
end;

function TIPEqList.AddChildAfter(AChild:TIPEqNode; Position:Integer):TIPEqNode;
var
  Pos : Integer;
begin
  Pos := Position + 1;
  if Pos < ChildCount then
    Result := AddChild(AChild,Pos)
  else if Pos = ChildCount then
    Result := AddChild(AChild)
  else
    raise Exception.Create('Could not add child to list at this Position.');
end;

procedure TIPEqList.AddChildrenAfter(Children: array of TIPEqNode; Node:TIPEqNode);
var
  Pos : Integer;
begin
  Pos := GetChildIndex(Node);
  if Pos >= 0 then
    AddChildrenAfter(Children,Pos);
end;

procedure TIPEqList.AddChildrenAfter(Children: array of TIPEqNode; Position:Integer);
var
  I : Integer;
begin
  for I := 0 to High(Children) do
    AddChildAfter(Children[I],Position+i);
end;

procedure TIPEqList.AddChildrenBefore(Children: array of TIPEqNode; Node:TIPEqNode);
var
 Pos : Integer;
begin
  Pos := GetChildIndex(Node);
  if Pos >= 0 then
    AddChildrenBefore(children,Pos);
end;

procedure TIPEqList.AddChildrenBefore(Children: array of TIPEqNode; Position:Integer);
var
 I : Integer;
begin
  for I := 0 to High(Children) do
    AddChild(children[I],Position+i);
end;

procedure TIPEqList.AddChildren(Children: array of TIPEqNode);
var
 I : Integer;
begin
  for I := 0 to High(Children) do
    AddChild(children[I]);
end;

function TIPEqList.GetNodeAt(X,Y:Integer):TIPEqNode;
var
 I : Integer;
 Node : TIPEqNode;
begin
  DoLayout;

  X := X - XLoc;
  Y := Y - YLoc;

  if not Contains(x,y) then
  begin
    Result := nil;
    Exit;
  end;

  for I := 0 to ChildCount-1 do
  begin
    Node := Child[I];
    if Node.Contains(X-Node.XLoc,Y-Node.YLoc) then
    begin
      Result := Node.GetNodeAt(X,Y);
      Exit;
    end;
  end;
  Result := Self;
end;

function TIPEqList.GetNodeList:TIPEqNodeList;
var
 I : integer;
begin
  SetLength(Result,ChildCount);
  for I := 0 to ChildCount-1 do
  begin
    Result[I] := Child[I];
  end;
end;

{Remove all children and free them}
procedure TIPEqList.Clear;
begin
  FreeAllChildren;
  Invalidate;
end;

procedure TIPEqList.AddNodes(Nodes:TIPEqNodeList);
begin
end;

procedure TIPEqList.ReduceFontSize;
begin
  inherited ReduceFontSize;
end;

function  TIPEqList.CheckParens:boolean;
var
 I : Integer;
begin
  Result := false;
  for I := 0 to ChildCount-1 do
  begin
    if not Child[I].CheckParens then
      Exit;
  end;
  Result := true;
end;


function  TIPEqList.RowsFilled:boolean;
var
 I : Integer;
begin
  Result := false;
  for I := 0 to ChildCount-1 do
  begin
    if not Child[I].RowsFilled then
      Exit;
  end;
  Result := true;
end;


{**************************************************************************}
{************* TIPEqRow ***************************************************}
{**************************************************************************}

Constructor TIPEqRow.CreateDummy;
begin
  Create;
  FDummyOffset := 15;
end;

function TIPEqRow.Clone:TIPEqNode;
begin
  Result := TIPEqRow.Create;
  with TIPEqRow(Result) do
  begin
    FDummyOffset := Self.FDummyOffset;
    CopyChildren(Self);
  end;
end;

procedure TIPEqRow.InvalidateAll;
begin
  inherited InvalidateAll;
  FFocusedPositions := nil;
end;

procedure TIPEqRow.Invalidate;
begin
  FFocusedPositions := nil;
  inherited Invalidate;
end;

function TIPEqRow.CalcMetrics:TIPEqMetrics;
var
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  TextMetric : TTextMetric;

  PrevAscent : Integer;
  NodeAscent : Integer;
  NodeDescent : Integer;
  Node : TIPEqNode;

  I : Integer;

  Eem : Integer;

begin


  TextMetric := GetTextMetrics;
  Eem := getEMWidth(Font);

  if ChildCount > 0 then
  begin
    RemoveNonEmptyRows;

    if not FDisableTextMerge then
      MergeTextFields;

    ParseTextFields;
    MatchParens;

    Ascent := 0;
    //***MAD*** If this is top most Row then limit descent to normal descent
    if Parent = nil then
      Descent := TextMetric.tmDescent
    else
      Descent := 0;

    Width := 0;

    PrevAscent := TextMetric.tmAscent-TextMetric.tmInternalLeading;

    for I := 0 to ChildCount-1 do
    begin
      Node := Child[I];

      if Node.InheritsFrom(TIPEqSubScript) then
      begin
        NodeAscent := PrevAscent div 3;
        NodeDescent := Node.Height-NodeAscent;
      end
      else if Node.InheritsFrom(TIPEqSuperScript) then
      begin
        //We need to limit this.  If the superscript Node
        //has a large descent we need to shift things up.
        if Node.Descent > (PrevAscent div 3) then
          NodeAscent := (PrevAscent div 3) + Node.Descent + Node.Ascent
        else
          NodeAscent := (2*PrevAscent) div 3 + Node.Ascent;
        NodeDescent := 0;
      end
      else
      begin
        NodeAscent := Node.Ascent;
        NodeDescent := Node.Descent;
      end;

      PrevAscent := NodeAscent;

      Ascent := Max(Ascent,NodeAscent);
      Descent := Max(Descent,NodeDescent);
      Inc(Width,Node.Width);
    end;
    Result := TIPEqMetrics.Create(Ascent,Descent,Width,Eem);
  end
  else
  begin
    Result := TIPEqMetrics.Create(TextMetric.tmAscent-TextMetric.tmInternalLeading,TextMetric.tmDescent,(2*Eem div 3)+GetEmPart(FDummyOffset,Eem),Eem);
  end;

  CalcFocusedPositions;

end;

procedure TIPEqRow.Layout;
var
  TextMetric : TTextMetric;
  PrevAscent : Integer;
  X : Integer;
  I : Integer;
  Node : TIPEqNode;
begin
  TextMetric := GetTextMetrics;
  PrevAscent := TextMetric.tmAscent-TextMetric.tmInternalLeading;
  X := GetEmPart(FDummyOffset);
  for I := 0 to ChildCount-1 do
  begin
    Node := Child[I];
    if Node.InheritsFrom(TIPEqSubScript) then
      PrevAscent := PrevAscent div 3
    else if Node.InheritsFrom(TIPEqSuperScript) then
    begin
      if Node.Descent > (PrevAscent div 3) then
        PrevAscent := (PrevAscent div 3) + Node.Descent + Node.Ascent
      else
        PrevAscent := (2*PrevAscent) div 3 + Node.Ascent;
    end
    else
      PrevAscent := Node.Ascent;
    Node.SetLocation(x,Ascent-PrevAscent);
    Inc(X,Node.Width);
  end;
end;

procedure TIPEqRow.Draw(ACanvas:TCanvas);
begin
end;


function TIPEqRow.GetLastRow:TIPEqRow;
begin
  Result := Self;
end;

function TIPEqRow.GetFirstRow:TIPEqRow;
begin
  Result := Self;
end;

function  TIPEqRow.GetCaretLocation(Position:Integer):TRect;
var
  Pt : TPoint;
  Index : Integer;
  FocusedElement : TIPEqNode;
  H : Integer;
begin
  if ChildCount = 0 then
  begin
    Pt := GetComponentLocation;
    Result := Bounds(GetEMPart(FDummyOffset)+Pt.X+1,Pt.Y+1,1,Height-2);
    Exit;
  end;

  if Position = GetLastCaretPosition then
  begin
    Pt := GetComponentLocation;
    Result := Bounds(Pt.X+Width,Pt.Y,1,Height);
    Exit;
  end;

  Index := GetFocusedElementIndex(Position);
  if Index >= 0 then
  begin
    FocusedElement := Child[Index];
    if FocusedPosition[Index] = Position then
    begin
      if (Index > 0) and not (Child[Index-1].InheritsFrom(TIPEqComposite) or
                               Child[Index-1].InheritsFrom(TIPEqRow)) then
      begin
        //If previous Node is a text or character or something similar use
        //it's height to Position cursor.
        Result := Child[Index-1].GetCaretLocation(Position-FocusedPosition[Index-1]);
      end
      else if FocusedElement.InheritsFrom(TIPEqSuperScript) {****or (FocusedElement is TIPEqRow)} then
      begin
        //If is is a superscript show caret at previous location
        if (Index > 0) then
        begin
          Result := Child[Index-1].GetCaretLocation(Position-FocusedPosition[Index-1]);
        end
        else
        begin
          if (FocusedElement is TIPEqRow) then
          begin
            Result := FocusedElement.GetCaretLocation(0);
          end
          else
          begin
            Pt := GetComponentLocation;
            Result := Bounds(Pt.x-1,Pt.y,1,Height);
          end;
        end;
      end
      else
      begin
        Pt := FocusedElement.GetComponentLocation;
        H  := FocusedElement.Height;
        Result := Bounds(Pt.X-1,Pt.Y,1,H);
      end;
    end
    else
    begin
      Result := FocusedElement.GetCaretLocation(Position-FocusedPosition[Index]);
    end;
  end
  else
    SetRectEmpty(Result);
end;

function TIPEqRow.GetLastCaretPosition:Integer;
begin
  if ChildCount = 0 then
    Result := 1
  else
    Result := FocusedPosition[FocusedPositionCount-1];
end;

procedure TIPEqRow.CalcFocusedPositions;
var
  Pos : Integer;
  I : Integer;
begin
    SetLength(FFocusedPositions,ChildCount+1);
    if ChildCount > 0 then
      FFocusedPositions[0] := 0;
    Pos := 0;
    for I := 0 to ChildCount-1 do
    begin
      with Child[I] do
      begin
        GetMetrics;
        Inc(pos,GetLastCaretPosition);
        FFocusedPositions[I+1] := Pos;
      end;
    end;
end;

function TIPEqRow.GetFocusedPosition(Index:Integer):Integer;
begin
  GetMetrics;
  if Index < 0 then
    Result := 0
  else
    Result := FFocusedPositions[Index];
end;

function TIPEqRow.GetFocusedPositionCount:Integer;
begin
  GetMetrics;
  Result := Length(FFocusedPositions);
end;

function TIPEqRow.GetFocusedElementIndex(Position:Integer):Integer;
var
  Len,I : Integer;
  Node : TIPEqNode;
begin
  {Do check using property so we know stuff has been
    calculated.  After that use array for speed.}

  if ChildCount = 0 then
  begin
    Result := -1;
    Exit;
  end;

  Len := FocusedPositionCount;

  if Len = 0 then
  begin
    Result := -1;
    Exit;
  end;

  //Make suer cached Index is still good.
  if FLastFocusedElement >= (Len-1) then
    FLastFocusedElement := 0;

  //Check starting pos
  if Position < FFocusedPositions[FLastFocusedElement] then
  begin
    I := FLastFocusedElement;
    while(i > 0) do
    begin
      Dec(I);
      if FFocusedPositions[I] <= Position then
      begin
        Node := Child[I];
        if Node.GetLastCaretPosition > 0 then
        begin
          FLastFocusedElement := I;
          break;
        end;
      end;
    end;
  end
  else
  begin
    //Look forward through list
    I := Max(FLastFocusedElement,0);
    while I < (Len-1) do
    begin
      if FFocusedPositions[I] <= Position then
      begin
        Node := Child[I];
        if Node.GetLastCaretPosition > 0 then
          FLastFocusedElement := I;
      end
      else
      begin
        break;
      end;
      Inc(I);
    end;
  end;
  Result := FLastFocusedElement;
end;

procedure TIPEqRow.MoveCaret(CaretEvent:TIPEqCaretEvent);
begin
  case CaretEvent.Direction of
    cdLeft : MoveLeft(CaretEvent);
    cdRight : MoveRight(CaretEvent);
    cdUp : MoveUp(CaretEvent);
    cdDown : MoveDown(CaretEvent);
    cdHome : MoveHome(CaretEvent);
    cdEnd  : MoveEnd(CaretEvent);
    cdTab  : TabForward(CaretEvent);
    cdBackTab : TabBack(CaretEvent);
  end;
end;



procedure TIPEqRow.MoveUp(CaretEvent:TIPEqCaretEvent);
var
  CurRowNum : Integer;
  Node : TIPEqRow;
  CurLocation:TRect;
  NewPos : Integer;
  NodeDist : Integer;
  Dist : Integer;
  CompLoc : TPoint;
  I : Integer;
begin

  //First check children to see if there's a child composite that higher than
  //Position of caret.
  CurLocation := getCaretLocation(CaretEvent.Position);
  CompLoc := GetComponentLocation(0,0);
  OffsetRect(CurLocation,-CompLoc.x,-CompLoc.y);

  Node := nil;
  NodeDist := 100000;
  for I := 0 to ChildCount-1 do
  begin
    if (Child[I] is TIPEqSuperScript) and not (Child[I] is TIPEqSubScript) then
    begin
      if CurLocation.Left <= Child[I].XLoc then
        Dist := Child[I].XLoc-CurLocation.Left
      else if CurLocation.Right >= (Child[I].XLoc+Child[I].Width) then
        Dist := CurLocation.Right - (Child[I].XLoc+Child[I].Width)
      else
        Dist := 100000;

      if Dist < NodeDist then
      begin
        Node := TIPEqRow(Child[I].GetFirstRow);
        NodeDist := Dist;
      end;
    end;
  end;

  if not Assigned(Node) and Assigned(Parent) and Parent.InheritsFrom(TIPEqStack) then
  begin
    CurRowNum := Parent.GetChildIndex(Self);
    Node := TIPEqStack(Parent).GetRowAbove(CurRowNum);
  end;

  if Assigned(Node) then
  begin
    CurLocation := getCaretLocation(CaretEvent.Position);
    NewPos := Node.GetCaretPositionAt(CurLocation.Left,Node.yLoc);
    CaretEvent.Row := Node;
    CaretEvent.Position := NewPos;
  end;

end;

procedure TIPEqRow.MoveDown(CaretEvent:TIPEqCaretEvent);
var
  CurRowNum : Integer;
  Node : TIPEqRow;
  CurLocation:TRect;
  NewPos : Integer;
  NodeDist : Integer;
  Dist : Integer;
  CompLoc : TPoint;
  I : Integer;
begin

  //First check children to see if there's a child composite that higher than
  //Position of caret.
  CurLocation := GetCaretLocation(CaretEvent.Position);
  CompLoc := GetComponentLocation(0,0);
  OffsetRect(CurLocation,-CompLoc.X,-CompLoc.Y);

  Node := nil;
  NodeDist := 100000;
  for I := 0 to ChildCount-1 do
  begin
    if (Child[I] is TIPEqSubScript)  then
    begin
      if CurLocation.Left <= Child[I].XLoc then
        Dist := Child[I].XLoc-CurLocation.Left
      else if CurLocation.Right >= (Child[I].XLoc+Child[I].Width) then
        Dist := CurLocation.Right - (Child[I].XLoc+Child[I].Width)
      else
        Dist := 100000;

      if Dist < NodeDist then
      begin
        Node := TIPEqRow(Child[I].GetFirstRow);
        NodeDist := Dist;
      end;
    end;
  end;

  if not Assigned(Node) and  Assigned(Parent) and Parent.InheritsFrom(TIPEqStack) then
  begin
    CurRowNum := Parent.GetChildIndex(Self);
    Node := TIPEqStack(Parent).getRowBelow(CurRowNum);
  end;

  if Assigned(Node) then
  begin
    CurLocation := getCaretLocation(CaretEvent.Position);
    NewPos := Node.GetCaretPositionAt(CurLocation.Left,Node.yLoc);
    CaretEvent.Row := Node;
    CaretEvent.Position := NewPos;
  end;

end;

procedure TIPEqRow.MoveLeft(CaretEvent:TIPEqCaretEvent);
var
  Pos : Integer;
  Index : Integer;
  Row : TIPEqRow;
begin
  Pos := CaretEvent.Position;

  //Handle Extended Selection
  if CaretEvent.ExtendSelection then
  begin
    if Pos > 0 then
    begin
      CaretEvent.Position := Pos-1;
      Exit;
    end;
  end;

  //Normal movement handling
  if (Pos <= 0) or (ChildCount = 0) then
  begin
    if Assigned(Parent) then
      Parent.GotoPrevRow(Self,CaretEvent);
  end
  else
  begin
    //Determine if we are currently on a Node boundary
    Index := GetFocusedElementIndex(Pos);
    if (Pos = FocusedPosition[Index]) or (Pos = GetLastCaretPosition) then
    begin
      //We're on a boundary drill down if necessary.
      if Pos <> GetLastCaretPosition then
        Dec(Index);
      if Index >= 0 then
      begin
        Row := Child[Index].GetLastRow;
        if Assigned(Row) then
        begin
          CaretEvent.Row := Row;
          CaretEvent.Position := Row.GetLastCaretPosition;
          Exit;
        end;
      end;
    end;
    CaretEvent.Position := Pos-1;
  end;
end;

procedure TIPEqRow.MoveRight(CaretEvent:TIPEqCaretEvent);
var
  Pos : Integer;
  Index : Integer;
  Row : TIPEqRow;
begin
  pos := CaretEvent.Position;

  //Handle Extended Selection
  if CaretEvent.ExtendSelection then
  begin
    if (Pos < GetLastCaretPosition) and (ChildCount > 0)  then
    begin
      CaretEvent.Position := Pos+1;
      Exit;
    end
  end;

  if (Pos >= GetLastCaretPosition) or (ChildCount = 0) then
  begin
    if Assigned(Parent) then
      Parent.GotoNextRow(Self,CaretEvent);
  end
  else
  begin
    Index := GetFocusedElementIndex(Pos);
    //Determine if we're currently on Node boundary.
    if Pos = FocusedPosition[Index] then
    begin
      { We're on a boundary so check for potential focus control and
        drill down if necessary.}
      Row := Child[Index].GetFirstRow;
      if Assigned(Row) then
      begin
        CaretEvent.Row := Row;
        CaretEvent.Position := 0;
        Exit;
      end;
    end;
    CaretEvent.Position := Pos+1;
  end;
end;

procedure TIPEqRow.MoveHome(CaretEvent:TIPEqCaretEvent);
begin
  if CaretEvent.Position > 0 then
  begin
    CaretEvent.Position := 0;
  end
  else
  begin
    if Assigned(Parent) then
    begin
      Parent.GotoPrevRow(Self,CaretEvent);
      CaretEvent.Position := 0;
    end;
  end;
end;

procedure TIPEqRow.MoveEnd(CaretEvent:TIPEqCaretEvent);
begin
  if CaretEvent.Position < GetLastCaretPosition then
  begin
    CaretEvent.Position := GetLastCaretPosition;
  end
  else
  begin
    if Assigned(Parent) then
    begin
      Parent.GotoNextRow(Self,CaretEvent);
      CaretEvent.Position := CaretEvent.Row.GetLastCaretPosition;
    end;
  end;
end;

procedure TIPEqRow.TabForward(CaretEvent:TIPEqCaretEvent);
var
  IStart : Integer;
begin
  if (CaretEvent.Position = GetLastCaretPosition) or (ChildCount = 0) then
  begin
    if Assigned(Parent) then
      Parent.GotoNextRow(Self,CaretEvent);
    Exit;
  end;
  IStart := GetFocusedElementIndex(CaretEvent.Position);
  if IStart < ChildCount then
  begin
    if (CaretEvent.Position = FocusedPosition[IStart]) and
       (Child[IStart].InheritsFrom(TIPEqComposite) or Child[IStart].InheritsFrom(TIPEqRow)) then
    begin
      CaretEvent.Position := 0;
      CaretEvent.Row := Child[IStart].GetFirstRow;
    end
    else
      CaretEvent.Position := FocusedPosition[IStart+1];
  end;
end;

procedure TIPEqRow.TabBack(CaretEvent:TIPEqCaretEvent);
var
  IStart : Integer;
begin
  if (CaretEvent.Position = 0) or (ChildCount = 0) then
  begin
    if Assigned(Parent) then
      Parent.GotoPrevRow(Self,CaretEvent);
    Exit;
  end;
  IStart := GetFocusedElementIndex(CaretEvent.Position);
  if IStart < ChildCount then
  begin
    if CaretEvent.Position > FocusedPosition[IStart] then
    begin
       if Child[IStart].InheritsFrom(TIPEqComposite) or
          Child[IStart].InheritsFrom(TIPEqRow) then
       begin
         CaretEvent.Row := Child[IStart].GetLastRow;
         CaretEvent.Position := CaretEvent.Row.GetLastCaretPosition;
       end
       else
         CaretEvent.Position := FocusedPosition[IStart]
    end
    else if CaretEvent.Position = FocusedPosition[IStart] then
    begin
      if IStart > 0 then
      begin
       if Child[IStart-1].InheritsFrom(TIPEqComposite) or
           Child[IStart-1].InheritsFrom(TIPEqRow) then
       begin
         CaretEvent.Row := TIPEqComposite(Child[IStart-1]).GetLastRow;
         CaretEvent.Position := CaretEvent.Row.GetLastCaretPosition;
       end
       else
         CaretEvent.Position := FocusedPosition[IStart-1];
      end
      else
      begin
        if Assigned(Parent) then
          Parent.GotoPrevRow(Self,CaretEvent);
      end;
    end;
  end;
end;


procedure TIPEqRow.GotoPrevRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent);
var
  Index : Integer;
begin
  Index := GetChildIndex(Node);

  if CaretEvent.ExtendSelection then
  begin
    CaretEvent.Position := FocusedPosition[Index];
    CaretEvent.PositionStart := FocusedPosition[Index+1];
    CaretEvent.Row := Self;
    Exit;
  end;


  Dec(Index);
  if Index >= 0 then
  begin
    if Child[Index] is TIPEqRow then
    begin
      //This will handle empty dummy Row for mixed numbers.
      CaretEvent.Row := TIPEqRow(Child[Index]);
      CaretEvent.Position := 0;
    end
    else
    begin
      CaretEvent.Row := Self;
      CaretEvent.Position := FocusedPosition[Index+1];
    end;
  end
  else
  begin
    CaretEvent.Row := Self;
    CaretEvent.Position := 0;
  end;
end;

procedure TIPEqRow.GotoNextRow(Node: TIPEqNode; CaretEvent:TIPEqCaretEvent);
var
  Index : Integer;
begin
  Index := GetChildIndex(Node);

  if CaretEvent.ExtendSelection then
  begin
    CaretEvent.Position := FocusedPosition[Index]+1;
    CaretEvent.PositionStart := FocusedPosition[Index];
    CaretEvent.Row := Self;
    Exit;
  end;

  Inc(Index);
  if Index = ChildCount then
  begin
    CaretEvent.Row := Self;
    CaretEvent.Position := GetLastCaretPosition;
  end
  else if Index < ChildCount then
  begin
    if Child[Index] is TIPEqRow then
    begin
      //This will handle navigating into dummy Rows.
      CaretEvent.Row := TIPEqRow(Child[Index]);
      CaretEvent.Position := 0;
    end
    else if (Node is TIPEqRow) and Child[Index].InheritsFrom(TIPEqComposite) then
    begin
      CaretEvent.Row := Child[Index].GetFirstRow;
      CaretEvent.Position := 0;
    end
    else
    begin
      CaretEvent.Row := Self;
      CaretEvent.Position := FocusedPosition[Index];
    end;
  end
  else
  begin
    if Assigned(Parent) then
      Parent.GotoNextRow(Self,CaretEvent);
  end;
end;

procedure TIPEqRow.InsertCharacter(CharEvent: TIPEqCharEvent);
var
  NewNode : TIPEqNode;
  NodeIndex : Integer;
  CurNode : TIPEqNode;
  NewPos : Integer;
  Index : Integer;
begin

  //Special code to handle >= and <=
  if (CharEvent.InsertedChar = '=') and (ChildCount > 0) then
  begin
    NodeIndex := GetFocusedElementIndex(CharEvent.Position);
    if (CharEvent.Position <> GetLastCaretPosition) and (NodeIndex > 0) then
      Dec(NodeIndex);
    CurNode := Child[NodeIndex];
    NewPos := CharEvent.Position - FocusedPosition[NodeIndex];
    if (CurNode is TIPEqOp) and (NewPos = 1) then
    begin
      with TIPEqOp(CurNode) do
      begin
        if Op in [eqoLT,eqoGT] then
        begin
          if Op = eqoLT  then
            Op := eqoLE
          else
            Op := eqoGE;
          Exit;
        end;
      end;
    end;
  end;

  //Create a Node based on the character typed
  NewNode := Document.CreateNode(CharEvent.InsertedChar);

  if not Assigned(NewNode) then
  begin
    CharEvent.FRow := nil;  //This signifies that character was ignored.
    Exit;
  end;


  if ChildCount = 0 then
  begin
    if not (Parent is TIPEqRow) then
    begin
      CharEvent.Position := CharEvent.Position+1;
    end
    else
    begin
      //This code is so we change the focus for dummy Rows.
      CharEvent.Row := TIPEqRow(Parent);
      Index := CharEvent.Row.GetChildIndex(Self);
      CharEvent.Position := CharEvent.Row.FocusedPosition[Index]+1;
    end;
    InsertNode([NewNode],0);
    Exit;
  end;

  NodeIndex := GetFocusedElementIndex(CharEvent.Position);
  CurNode := Child[NodeIndex];
  if (CurNode is TIPEqRow) then
  begin
    if TIPEqRow(CurNode).IsDummyRow then
    begin
      InsertNode([NewNode],CharEvent.Position);
      CharEvent.Position := CharEvent.Position + newNode.GetLastCaretPosition;
    end
    else
    begin
      NewPos := CharEvent.Position - FocusedPosition[NodeIndex-1];
      CurNode := Child[NodeIndex-1];
      CurNode.InsertNode([NewNode],NewPos);
      CharEvent.Position := CharEvent.Position + 1;
    end;
  end
  else
  begin
    NewPos := CharEvent.Position - FocusedPosition[NodeIndex];
    CurNode.InsertNode([NewNode],NewPos);
    CharEvent.Position := CharEvent.Position + 1;
  end;

end;

Function TIPEqRow.InsertNode(Node:array of TIPEqNode; Position:Integer):TIPEqNode;
var
  NodeIndex : Integer;
  CurNode : TIPEqNode;
  Offset  : Integer;
begin

  if ChildCount = 0 then
  begin
    AddChildren(Node);
    Result := Node[0];
  end
  else
  begin
    NodeIndex := GetFocusedElementIndex(Position);
    CurNode := Child[NodeIndex];
    Offset := FocusedPosition[NodeIndex];
    if Position-Offset = 0 then
      AddChildrenBefore(Node,NodeIndex)
    else
      CurNode.InsertNode(Node,Position-Offset);
    Result := Node[0];
  end;
end;

Function TIPEqRow.ExtractNodes(StartPos,EndPos:Integer):TIPEqNodeList;
var
  StartIndex,EndIndex : Integer;
  N : TIPEqNode;
  I : Integer;
  TPos : Integer;
begin

  if EndPos < StartPos then
  begin
    TPos := EndPos;
    EndPos := StartPos;
    StartPos := TPos;
  end;

  FDisableTextMerge := true;
  try
    StartIndex := GetFocusedElementIndex(StartPos);
    //Split first and last elements if necessary
    if (FocusedPosition[StartIndex] <> StartPos) then
    begin
      N := Child[StartIndex];
      N.SplitNode(StartPos-FocusedPosition[StartIndex]);
      CalcFocusedPositions;
    end;

    EndIndex := GetFocusedElementIndex(EndPos);
    if FocusedPosition[EndIndex] <> EndPos then
    begin
      N := Child[EndIndex];
      N.SplitNode(EndPos-FocusedPosition[EndIndex]);
      CalcFocusedPositions;
    end;

    //Gather all Nodes and remove them, then add them to Node being inserted
    StartIndex := getFocusedElementIndex(StartPos);
    EndIndex := getFocusedElementIndex(EndPos);

    //When we're at the end of a Row, the EndIndex is the last element
    //we need to fake it out so when we include everything preceding the
    //EndIndex, we actually include the last element.
    if (EndPos >= GetLastCaretPosition) then
      Inc(EndIndex);

    SetLength(Result,EndIndex-StartIndex);
    for I:=StartIndex to EndIndex-1 do
    begin
     Result[I-StartIndex] := Child[StartIndex];
     RemoveChildAt(StartIndex);
    end;
  finally
    FDisableTextMerge := false;
  end;
    
end;

Function TIPEqRow.InsertNodeRange(Node:array of TIPEqNode;StartPos, EndPos:Integer; PrimaryNode:Integer):TIPEqNode;
var
  Nodes : TIPEqNodeList;
  I : Integer;
  StartIndex : Integer;
  Pos : Integer;
begin

  Result := nil;

  Nodes := ExtractNodes(StartPos,EndPos);

  FDisableTextMerge := true;
  try
    Pos := Min(StartPos,EndPos);
    StartIndex := GetFocusedElementIndex(Pos);

    if StartIndex < 0 then
      AddChildrenBefore(Node,0)
    else if FocusedPosition[StartIndex] = Pos then
      AddChildrenBefore(Node,StartIndex)
    else
      AddChildrenAfter(Node,StartIndex);

    //If this is some sort of composite then add the Nodes to it.
    if Node[PrimaryNode] is TIPEqRow then
    begin
      AddChildrenBefore(Nodes,GetChildIndex(Node[PrimaryNode]));
      removeChild(Node[PrimaryNode]).Free;
      if Length(Node) > (PrimaryNode+1) then
        Result := Node[PrimaryNode+1]
      else if StartIndex >= 0 then
        Result := Child[StartIndex]
      else
        Result := Child[0];
    end
    else if Node[PrimaryNode].InheritsFrom(TIPEqList) then
      (Node[PrimaryNode] as TIPEqList).AddNodes(Nodes)
    else
    begin
      for I := 0 to High(Nodes) do
        Nodes[I].Free;
    end;
  finally
     FDisableTextMerge := false;
  end;


  if not Assigned(Result) then
    Result :=  Node[PrimaryNode];
end;


function TIPEqRow.Contains(X,Y:Integer):boolean;
var
 Offset : Integer;
begin

  Offset := 0;
  if IsDummyRow then
    Offset := GetEmpart(FDummyOffset);
  Result := (X >= Offset) and (X < Width) and (Y >= 0) and (Y < Height);
  
end;

function TIPEqRow.GetNodeAt(X,Y:Integer):TIPEqNode;
var
  Node : TIPEqNode;
begin

  Node := Inherited GetNodeAt(X,Y);
  if Assigned(Node) and Node.InheritsFrom(TIPEqRow) then
    Result := Node
  else
  begin
    Result := Self;
  end;
end;

function TIPEqRow.GetCaretPositionAt(X,Y:Integer):Integer;
var
  Pt : TPoint;
  NewX:Integer;
  N,Node : TIPEqNode;
  I : Integer;
  NodeIndex : Integer;
  Pos : Integer;
begin

  if ChildCount = 0 then
  begin
    Result := 0;
    Exit;
  end;

  Pt := GetComponentLocation;

  NewX := X-Pt.X;
  Node := nil;

  if NewX < Child[0].XLoc then
    Node := Child[0]
  else if newX > (LastChild.XLoc+LastChild.Width) then
    Node := LastChild
  else
  begin
    for I := 0 to ChildCount-1 do
    begin
      N := Child[I];
      if (NewX >= n.XLoc) and (NewX <= (N.XLoc+N.Width)) then
        Node := N;
    end;
  end;

  if Assigned(Node) then
  begin
    NodeIndex := GetChildIndex(Node);
    Pos := Node.GetCaretPositionAt(X,Y);
    Result := FocusedPosition[NodeIndex]+Pos;
  end
  else
    Result := -1;
end;

procedure TIPEqRow.DeleteSingleCharacter(CaretEvent:TIPEqCaretEvent);
var
  Index : Integer;
  FocusedElement : TIPEqNode;
  Offset : Integer;
  OldPosition : Integer;
  OldPositionStart : Integer;
  Node : TIPEqNode;
begin
  if CaretEvent.Position >= GetLastCaretPosition then
  begin
    Exit;
  end;

  //Handle backspacing on empty Row.  Delete Row (for now).
  if CaretEvent.Position < 0 then
  begin
    Node := Self;
    while Assigned(Node.Parent) and not Node.Parent.InheritsFrom(TIPEqRow) do
    begin
      Node := Node.Parent;
    end;

    if Assigned(Node.Parent) and Node.Parent.InheritsFrom(TIPEqRow) then
    begin
      CaretEvent.Row := TIPEqRow(Node.Parent);
      CaretEvent.PositionStart := CaretEvent.Row.FocusedPosition[CaretEvent.Row.GetChildIndex(Node)];
      CaretEvent.Position := CaretEvent.PositionStart + 1;
      CaretEvent.ExtendSelection := true;
    end;

    Exit;
  end;

  if ChildCount = 0 then
  begin
    if Assigned(Parent) and Parent.InheritsFrom(TIPEqComposite) then
    begin
      CaretEvent.ExtendSelection := true;
      Parent.GotoNextRow(Self,CaretEvent);
      Exit;
    end;
  end;

  //Now try to delete actual stuff
  Index := GetFocusedElementIndex(CaretEvent.Position);
  if Index >= 0 then
  begin
    FocusedElement := Child[Index];
    Offset := FocusedPosition[Index];
    OldPosition := CaretEvent.Position;
    OldPositionStart := CaretEvent.PositionStart;
    CaretEvent.Position := OldPosition - Offset;
    CaretEvent.PositionStart := OldPositionStart - Offset;

    FocusedElement.DeleteCharacter(CaretEvent);

    CaretEvent.Position := OldPosition;
    CaretEvent.PositionStart := OldPositionStart;

    //Only remove children if they're empty.
    if FocusedElement.isEmpty then
    begin
      RemoveChild(FocusedElement);
      FocusedElement.Free;
      if CaretEvent.Row = Self then
      begin
        CaretEvent.Position := OldPosition;
        CaretEvent.PositionStart := OldPositionStart;
      end;
    end;

    if CaretEvent.ExtendSelection then
    begin
      //Select element
      CaretEvent.Position := OldPosition;
      if OldPosition < GetLastCaretPosition then
      begin
        CaretEvent.Position := OldPosition +1;
      end;
    end;

  end;
end;


procedure TIPEqRow.DeleteCharacter(CaretEvent:TIPEqCaretEvent);
var
  IStart,IEnd : Integer;
  I : Integer;
  Node : TIPEqNode;
  DeletedNodes : TList;
  Pos1,Pos2 : Integer;
  FPositions: array of Integer;

  function isFullNode(NodeIndex:Integer):boolean;
  begin
    if (Pos1 <= FocusedPosition[NodeIndex]) and (Pos2 >= FocusedPosition[NodeIndex+1]) then
      Result := true
    else
      Result := false;
  end;

begin


  Pos1 := Min(CaretEvent.Position,CaretEvent.PositionStart);
  Pos2 := Max(CaretEvent.Position,CaretEvent.PositionStart);

  if Assigned(Parent) and Parent.InheritsFrom(TIPEqRow) then
  begin
    CaretEvent.Row := TIPEqRow(Parent);
    CaretEvent.PositionStart := CaretEvent.Row.FocusedPosition[CaretEvent.Row.GetChildIndex(Self)];
    CaretEvent.Position := CaretEvent.PositionStart + 1;
    CaretEvent.ExtendSelection := true;
    Exit;
  end;


  if Pos1 = Pos2 then
  begin
    DeleteSingleCharacter(CaretEvent);
    Exit;
  end;

  //Handle backspacing on empty Row.  Delete Row (for now).
  if Pos1  < 0 then
  begin
    Node := Self;
    while Assigned(Node.Parent) and not Node.Parent.InheritsFrom(TIPEqRow) do
    begin
      Node := Node.Parent;
    end;

    if Node.Parent.InheritsFrom(TIPEqRow) then
    begin
      CaretEvent.Row := TIPEqRow(Node.Parent);
      CaretEvent.PositionStart := CaretEvent.Row.FocusedPosition[CaretEvent.Row.GetChildIndex(Node)];
      CaretEvent.Position := CaretEvent.PositionStart + 1;
      CaretEvent.ExtendSelection := true;
    end;

    Exit;
  end;


  //We need to loop through all children involved and union their rectangles
  IStart := GetFocusedElementIndex(Pos1);
  IEnd := GetFocusedElementIndex(Pos2);

  DeletedNodes := TList.Create;

  SetLength(FPositions,FocusedPositionCount);
  for I := 0 to FocusedPositionCount-1 do
    FPositions[I] := FocusedPosition[I];

  try
    for I := IStart to IEnd do
    begin
      Node := Child[I];
      if (IStart = IEnd) and ((Pos1 <> FPositions[I]) or (Pos2 <> FPositions[i+1])) then
      begin
        CaretEvent.Position:= Pos2 - FPositions[I];
        CaretEvent.PositionStart := Pos1 - FPositions[I];
        Node.DeleteCharacter(CaretEvent);
        if Node.IsEmpty then
          DeletedNodes.Add(Node);
      end
      else if (i = IStart) and (FocusedPosition[I] < Pos1) then
      begin
        CaretEvent.Position := FPositions[i+1]-FPositions[I];
        CaretEvent.PositionStart := Pos1 - FPositions[I];
        Node.DeleteCharacter(CaretEvent);
        if Node.IsEmpty then
          DeletedNodes.Add(Node);
      end
      else if (I = IEnd) and (FPositions[I+1] > Pos2) then
      begin
        if Pos2 > FPositions[I] then
        begin
          CaretEvent.Position := Pos2-FPositions[I];
          CaretEvent.PositionStart := 0;
          Node.DeleteCharacter(CaretEvent);
          if Node.IsEmpty then
            DeletedNodes.Add(Node);
        end;
      end
      else
      begin
        DeletedNodes.Add(Node);
      end;
    end;
  finally
    for I := 0 to DeletedNodes.Count-1 do
    begin
      RemoveChild(TIPEqNode(DeletedNodes[I])).Free;
    end;
    DeletedNodes.Free;
    CaretEvent.Position := Pos1;
    CaretEvent.PositionStart := Pos1;
  end;
end;


function TIPEqRow.GetSelectedRect(Pos1,Pos2:Integer):TRect;
var
  TPos : Integer;
  IStart,IEnd : Integer;
  I : Integer;
  Node : TIPEqNode;
  NewRect : TRect;
  Pt : TPoint;
begin

  if ChildCount = 0 then
  begin
    Pt := GetComponentLocation;
    Result := Bounds(Pt.X,Pt.Y,Width,Height);
    Exit;
  end;

  if Pos1 = Pos2 then
    SetRectEmpty(Result);

  if Pos1 > Pos2 then
  begin
    TPos := Pos1;
    Pos1 := Pos2;
    Pos2 := tpos;
  end;

  //We need to loop through all children involved and union their rectangles
  IStart := GetFocusedElementIndex(Pos1);
  IEnd := GetFocusedElementIndex(Pos2);

  SetRectEmpty(NewRect);
  SetRectEmpty(Result);

  for I := IStart to IEnd do
  begin
    Node := Child[I];
    if IStart = IEnd then
    begin
      //Same Node
      NewRect := Node.GetSelectedRect(Pos1-FocusedPosition[I],Pos2-FocusedPosition[I]);
      //Need to determine if this is a partial Node or Node
    end
    else if ((I = IStart) and (FocusedPosition[I] <> Pos1)) then
    begin
      //This is a partial Node so call Node's method to get bounds
      NewRect := Node.getSelectedRect(Pos1-focusedPosition[I],Node.getLastCaretPosition());
    end
    else if (I = IEnd) then
    begin
      if (focusedPosition[I] = Pos2) then
       //Only at start of end element so don't inlcude
      else
      begin
         NewRect := Node.getSelectedRect(0,Pos2-FocusedPosition[I]);
      end;
    end
    else
    begin
      NewRect := Node.getSelectedRect(0,Node.GetLastCaretPosition);
    end;

    if not IsRectEmpty(NewRect) then
    begin
      if IsRectEmpty(Result) then
        Result := NewRect
      else
        UnionRect(Result,Result,NewRect);
    end;
  end;
end;


{This routine will match parens for this Row only.}
procedure TIPEqRow.MatchParens;
var
  OpenParens:TList;
  I,J : Integer;
  Op,P : TIPEqParen;
begin

  OpenParens := TList.Create;
  try
    for I := 0 to ChildCount-1 do
    begin
      if Child[I].InheritsFrom(TIPEqParen) then
      begin
        P := TIPEqParen(Child[I]);
        P.MatchedParen := nil;
        P.ContentAscent := 0;
        P.ContentDescent := 0;
        //If this is an open parenthesis, push it on the stack
        if P.isOpenParen then
        begin
          OpenParens.Add(P);
        end
        else if P.IsCloseParen then
        begin
          //If this is a close, pop stack until we find match one.
          while OpenParens.Count > 0  do
          begin
            Op := TIPEqParen(OpenParens.Last);
            OpenParens.Delete(OpenParens.Count-1);
            if Op.canClose(P) then
            begin
              P.ContentAscent := Op.ContentAscent;
              P.ContentDescent := Op.ContentDescent;
              P.MatchedParen := Op;
              Op.MatchedParen := P;
              Break;
            end;
          end;
        end;
      end
      else
      begin
        //Node is not a paren so transfer it's size to
        //all parens on stack
        for J := 0 to OpenParens.Count-1 do
        begin
          P := TIPEqParen(OpenParens[J]);
          P.ContentAscent := Max(P.ContentAscent,Child[I].Ascent);
          P.ContentDescent := Max(P.ContentDescent,Child[I].Descent);
        end;
      end;
    end;
  finally
    OpenParens.Free;
  end;
end;


procedure TIPEqRow.RemoveNonEmptyRows;
var
  NodeList : TIPEqNodeList;
  I : Integer;
  Row : TIPEqRow;
  RowNodes : TIPEqNodeList;
begin
  NodeList := GetNodeList;
  RowNodes := nil;
  for I := 0 to High(NodeList) do
  begin
    if NodeList[I] is TIPEqRow then
    begin
      Row := TIPEqRow(NodeList[I]);
      if Row.ChildCount > 0 then
      begin
        RowNodes := Row.GetNodeList;
        AddChildrenBefore(RowNodes,Row);
        RemoveChild(Row);
        Row.Free;
      end;
    end;
  end;
end;

procedure TIPEqRow.MergeTextFields;
var
  NodeList : TIPEqNodeList;
  I : Integer;
  PrevText : TIPEqText;
begin

  PrevText := nil;
  NodeList := GetNodeList;
  for I := 0 to High(NodeList) do
  begin
    if NodeList[I] is TIPEqText then
    begin
      if Assigned(PrevText) then
      begin
        PrevText.MergeText(TIPEqText(NodeList[I]));
        RemoveChild(NodeList[I]).Free;
      end
      else
        PrevText := TIPEqText(NodeList[I]);
    end
    else
      PrevText := nil;
  end;
end;

procedure TIPEqRow.ParseTextFields;
var
  NodeList : TIPEqNodeList;
  I : Integer;
  Parser : TIPEqTextParser;
begin
  Parser := TIPEqTextParser.Create;
  Parser.AllowCommaNumbers := Document.AllowCommaNumbers;
  Parser.AllowDollarNumbers := Document.AllowDollarNumbers;
  Parser.InterpretFunctions := Document.InterpretFunctions;
  Parser.OnVarExists := Document.OnVarExists;
  if Document.InterpretFunctions then
    Parser.FunctionList := Document.FunctionList;

  Parser.PlainText := NodeHasParent(Self,TIPEqPlainText);

  try
    NodeList := GetNodeList;
    for I := 0 to High(NodeList) do
    begin
      if NodeList[I] is TIPEqText then
        (NodeList[I] as TIPEqText).ParseText(Parser);
    end;
  finally
    Parser.Free;
  end;

end;

procedure TIPEqRow.InitCaret(var FocusedRow: TIPEqRow; var CaretPos:Integer);
begin
  FocusedRow := Self;
  CaretPos := 0;
end;

function TIPEqRow.IsDummyRow:Boolean;
begin
  Result := FDummyOffset > 0;
end;

Function TIPEqRow.GetNodeBeforeCaret(CaretPos:Integer):TIPEqNode;
var
  Index : Integer;
begin
  Index := GetFocusedElementIndex(CaretPos);
  if (Index > 0) and (FocusedPosition[Index] = CaretPos) then
    Result := Child[Index-1]
  else if (Index >= 0) then
    Result := Child[Index]
  else
    Result := nil;
end;

function TIPEqRow.GetText:String;
var
  I : Integer;
  EqDiv : TIPEqDivide;
  Ix : Integer;
  Str : String;
begin
  Result := '';
  I := 0;
  while I < ChildCount do
  begin
    //Add code to check for mixed numbers
    if I < ChildCount-1 then
      if not Document.Authoring and Child[I].IsInteger then
      begin
        Ix := 1;
        //We need to skip spaces
        while ((I+Ix) < ChildCount) and (
             ((Child[I+Ix] is TIPEqChar) and (TIPEqChar(Child[I+Ix]).Character = ' ')) or
             ((Child[I+Ix] is TIPEqText) and (Trim(TIPEqText(Child[I+Ix]).Text) = ''))) do
          Inc(ix);
        if ((I+Ix) < ChildCount) and (Child[I+Ix] is TIPEqDivide) then
        begin
          EqDiv := Child[I+Ix] as TIPEqDivide;
          if EqDiv.IsIntDiv then
          begin
            Result := Result + '@MNUM{' + Child[I].Text +';' +
                EqDiv.Row[0].Text + ';' + EqDiv.Row[1].Text + '}';
            Inc(I,Ix+1);
            Continue;
          end;
        end;
      end;
    Str := Child[I].Text;
    //Add fix for log x.  Make sure there's a space **MAD**  11/19/04
    if (Child[I] is TIPEqText) and ((I+1) < ChildCount) and (Child[I+1] is TIPEqText) and
      (TIPEqText(Child[I]).TType = ttFunction) and (AnsiCompareText(Str,'log') = 0) and
      (TIPEqText(Child[I+1]).TType = ttText) and (AnsiCompareText(TIPEqText(Child[I+1]).Text,'x') = 0) then
       Str := Str + ' ';
    Result := Result + Str;
    Inc(I);
  end;
end;

procedure TIPEqRow.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
   Buffer.Add(CharStrL(' ',Level)+'<mRow>');
   for I := 0 to ChildCount-1 do
     Child[I].BuildMathML(Buffer,Level+1);
   Buffer.Add(CharStrL(' ',Level)+'</mRow>');
end;

function  TIPEqRow.RowsFilled:boolean;
begin
  if ChildCount = 0 then
    Result := false
  else
    Result := inherited RowsFilled;
end;



function  TIPEqRow.IsInteger:boolean;
begin
  Result := (ChildCount = 1) and Child[0].IsInteger;
end;

function  TIPEqRow.IsNumber:boolean;
begin
  Result :=  ((ChildCount = 1) and Child[0].IsNumber);
end;

{*****************************************************************************}
{*********************************** TIPEQDocument **************************}
{*****************************************************************************}
constructor TIPEqDocument.Create;
begin
  inherited Create;
  FEmptyBoxPen := TPen.Create;

  FFunctionList := TStringList.Create;
  FFunctionList.Sorted := true;
  FFunctionList.Duplicates := dupIgnore;
  AssignDefFunctions(FFunctionList);
  FEmptyBoxBrush := TBrush.Create;
  FDocumentCache := Self;
  FNumberColor := clBlue;
  FVarColor := clFuchsia;
  FUnmatchedParenColor := clRed;

  FAllowCommaNumbers := true;

  FEqObjects := TList.Create;

  //A document needs it own font otherwise when you make changes it becomes global
  //9/16/03 MAD
  FFont := TFont.Create;
  FFont.Assign(EqDefaultFont);

end;

function TIPEqDocument.Clone:TIPEqNode;
begin
  Result := TIPEqDocument.Create;
  with TIPEqDocument(Result) do
  begin
   FEditable := Self.FEditable;
   FEmptyBoxPen.Assign(Self.FEmptyBoxPen);
   FEmptyBoxBrush.Assign(Self.FEmptyBoxBrush);
   FunctionList := Self.FunctionList;
   FItalicizeVariables := Self.FItalicizeVariables;
   FFont.Assign(Self.FFont);
   CopyChildren(Self);
  end;
end;

destructor TIPEqDocument.Destroy;
begin
  inherited Destroy;
  FEmptyBoxPen.Free;
  FEmptyBoxBrush.Free;
  FEqObjects.Free;
  FFunctionList.Free;
end;

procedure TIPEqDocument.SetFunctionList(StrList:TStringList);
begin
  FFunctionList.Clear;
  FFunctionList.AddStrings(StrList);
end;

procedure TIPEqDocument.SetEmptyBoxPen(Value:TPen);
begin
  FEmptyBoxPen.Assign(Value);
end;

procedure TIPEqDocument.SetEmptyBoxBrush(Value:TBrush);
begin
  FEmptyBoxBrush.Assign(Value);
end;

procedure TIPEqDocument.Clear;
begin
  inherited Clear;
  FEqObjects.Clear;
end;


procedure TIPEqDocument.Paint(ACanvas:TCanvas);
var
  FontRecall : TFontRecall;
  PenRecall : TPenRecall;
begin
  FontRecall := TFontRecall.Create(ACanvas.Font);
  PenRecall := TPenRecall.Create(ACanvas.Pen);
  try
    ACanvas.Font := Font;
    ACanvas.Pen.Color := Font.Color;
    inherited Paint(ACanvas);
  finally
    PenRecall.Free;
    FontRecall.Free;
  end;
end;

procedure TIPEqDocument.SetUseSymbolFont(Value:boolean);
begin
  if Value <> FUseSymbolFont then
  begin
    FUseSymbolFont := Value;
    InvalidateAll;
  end;
end;

procedure TIPEqDocument.SetItalicizeVariables(Value:boolean);
begin
  if Value <> FItalicizeVariables then
  begin
    FItalicizeVariables := Value;
    InvalidateAll;
  end;
end;

function  TIPEqDocument.GetMathML:String;
var
  Buf:TStringList;
begin
  Buf := TSTringList.Create;
  try
    BuildMathML(Buf,0);
    Result := Buf.Text;
  finally
    Buf.Free;
  end;
end;

procedure TIPEqDocument.SetEnabled(Value:boolean);
begin
  if Value <> FEnabled then
  begin
    FEnabled := Value;
    InvalidateAll;
  end;
end;


procedure TIPEqDocument.ParserError (Sender : TObject; Error : TCocoError);
var
  ErrString:String;
begin
  with (Sender as TIPEqParser) do
  begin
    ErrString := ErrorStr(Error.ErrorCode,Error.Data);
  end;
  raise Exception.CreateFmt('Error in expression: %s'#13#10'%s near column %d',[FEqText,ErrString,Error.Col-1]);//Typo C446 DV 2/25/05
end;

procedure TIPEqDocument.SetText(const AText:String);
var
  Parser : TIPEqParser;
  Strm : TStringStream;
begin
  if AText <> FEqText then
  begin
    Clear;
    FEqText := AText;
    Strm := TStringStream.Create(AText);
    Parser := TIPEqParser.Create(nil);
    try
      Parser.EqDocument := Self;
      Parser.OnError := ParserError;
      Parser.SourceStream.LoadFromStream(Strm);
      Parser.Execute;
    finally
      Parser.Free;
      Strm.Free;
    end;
  end;
end;

Function TIPEqDocument.CreateNode(Ch:Char):TIPEqNode;
var
  Op : TIPEqOpType;
begin
  if IsOpSymbol(Ch,Op) then
    Result := TIPEqOp.Create(Op)
  else if IsParen(Ch) then
    Result := TIPEqParen.Create(Ch)
  else if Ch = ' ' then
    Result := TIPEqChar.Create(' ')
  else if IsText(Ch) then
    Result := TIPEqText.Create(ch)
  else if (Ch > #32) and (Ch < #127) then
    Result := TIPEqChar.Create(Ch)
  else
    Result := nil;
end;

function TIPEqDocument.GetTextOnly:String;
begin
  FTextOnly := true;
  try
    Result := Text;
  finally
    FTextOnly := false;
  end;
end;

procedure TIPEqDocument.SetInterpretFunctions(Value:boolean);
begin
  if Value <> FInterpretFunctions then
  begin
    FInterpretFunctions := Value;
    InvalidateAll;
  end;
end;


procedure TIPEqDocument.SetControlVOffset(AControl:TControl; VOffset:Integer);
var
 EqObj : TIPEqObject;
begin
  EqObj := FindControl(Self,AControl);
  if Assigned(EqObj) then
  begin
    EqObj.VOffset := VOffset;
  end;
end;

procedure TIPEqDocument.UpdateControlSize(AControl:TControl);
var
 EqObj : TIPEqObject;
begin
  EqObj := FindControl(Self,AControl);
  if Assigned(EqObj) then
    EqObj.Invalidate;
end;

function  TIPEqDocument.GetControl(AName:String):TControl;
begin
  if Assigned(FOnCreateControl) then
  begin
    if FContainer = nil then
      raise Exception.Create('EqDocument needs a container to create a control.');
    Result := FOnCreateControl(Self,AName);
  end
  else
    Result := nil;
end;

function  TIPEqDocument.GetEqControlCount:Integer;
begin
  Result := FEqObjects.count;
end;

function  TIPEqDocument.GetEqControl(Index:Integer):TControl;
begin
  Result := TIPEqObject(FEqObjects[Index]).Control;
end;


procedure TIPEqDocument.RemoveEQObject(Node:TIPEqNode);
begin
  FEqObjects.Remove(Node);
end;

procedure TIPEqDocument.AddEqObject(Node:TIPEqNode);
begin
  FEqObjects.Add(Node);
end;

function TIPEqRow.GetEqPosition: Integer;
var I : integer;
    Node : TIPEQNode;
begin
  Layout;

  Result := 0;
  for I := 0 to ChildCount - 1 do
  begin
    Node := Child[I];
    if (Node is TIPEQOP) and (TIPEQOP(Node).Op = eqoEqual) then
    begin
      Result := Node.FXLoc;
      Break;
    end;
  end;
end;

function TIPEqNode.GetLeftOffset: integer;
begin
  Result:=0;
end;

initialization
  EqDefaultFont := TFont.Create;
  EqDefaultFont.Name := 'Times New Roman';
  EqDefaultFont.Size := 13;
finalization
  EqDefaultFont.Free;
end.
