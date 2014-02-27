unit IPEqEditor;

interface

uses
  Windows, Messages, SysUtils, Classes, Controls,IPEqNode,Graphics,Contnrs,ExtCtrls,
  LMDCustomBevelPanel,IPEqSymbolW,Dialogs,IPEqOp;

const
  EQAlwaysCreateCaret = true;
  
type

  TIPEqEditor = class;

  TIPEqCreateEditorEvent = function(Sender:TIPEqEditor; AName:String):TIPEqEditor of object;


  TIPEqEditor = class(TLMDCustomBevelPanel)
  private
    FEqDoc : TIPEqDocument;
    FCaretPos : Integer;
    FCurrentRow: TIPEqRow;
    FCaretHeight : Integer;
    FOnEnumerate : TIPEqEnumEvent;
    FOldCaretPos : Integer;
    FHighlightColor : TColor;
    FUndoBuffer : TStack;
    FControlPressed : Boolean;
    FInternalLeading : Integer;
    FOnUndoChanged : TNotifyEvent;
    FOnChanged:TNotifyEvent;
    FReadOnly: boolean;
    FParentEditor: TIPEqEditor;
    FOnCreateEditor : TIPEqCreateEditorEvent;
    Function GetCaretLocation:TRect;
    procedure WMGetDlgCode(var Msg: TWMGetDlgCode); message WM_GETDLGCODE;
    procedure UpdateCaret;
    procedure CMFontChanged(var Message: TMessage); message CM_FONTCHANGED;
    procedure DrawSelection(ACanvas:TCanvas);
    procedure InsertNode(Node:array of TIPEqNode); overload;
    procedure InsertNode(Node:array of TIPEqNode;PrimaryNode:Integer); overload;
    procedure ResetMultiSelect;
    procedure Delete(IsBackspace:Boolean);
    procedure ClearUndoItems;
    function ProcessShortCuts(Key:Char):Boolean;
    function ProcessControlKeys(Key:Char):Boolean;
    procedure CMChildKey(var Message: TCMChildKey); message CM_CHILDKEY;
    procedure WMNCHitTest(var Message: TWMNCHitTest); message WM_NCHITTEST;
    procedure WMKillFocus(var Msg : TWMKillFocus); message WM_KILLFOCUS;
    procedure WMSetFocus(var Msg : TWMSetFocus); message WM_SETFOCUS;
    procedure CMEnabledChanged(var Message: TMessage); message CM_ENABLEDCHANGED;
    procedure DoChanged;
    function  GetText:String;
    procedure SetText(Value:String);
    procedure SetReadOnly(Value:Boolean);
    function  GetTextMetrics:TTextMetric;
    function  CreateEqControl(Sender:TIPEqDocument; AName:String):TControl;
  protected
    procedure Paint; override;
    procedure DoEnter; override;
    procedure DoExit; override;
    procedure MouseDown(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;
    procedure MouseUp(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;
    procedure MouseMove(Shift: TShiftState; X, Y: Integer); override;
    procedure KeyUp(var Key: Word; Shift: TShiftState); override;
    procedure KeyDown(var Key: Word; Shift: TShiftState); override;
    procedure KeyPress(var Key: Char); override;
    procedure StoreUndoState;
    procedure UpdateUndoState;
    procedure RefreshUndoState;
    procedure Loaded; override;
    procedure AdjustSize; override;
    procedure DoUndoChanged;
    function CanAutoSize(var NewWidth, NewHeight: Integer): Boolean; override;
    function  GetDescent:Integer;
    procedure Resize; override;
  public
    constructor Create(AOwner:TComponent); override;
    destructor Destroy; override;
    procedure InitSize;
    procedure InsertSymbol(Symbol:TIPEqSymbolType);
    procedure InsertSqrt;
    procedure InsertNroot;
    procedure InsertMixedNumber;
    procedure InsertOrderedPair;
    procedure InsertDivide;
    procedure InsertDivM;
    procedure InsertSuperScript;
    procedure InsertSupSub;
    procedure InsertSuperScriptX;
    procedure InsertSubSub;
    procedure InsertSubScript;
    procedure InsertOverUnder;
    procedure InsertOver;
    procedure InsertUnder;
    procedure InsertLDiv;
    procedure InsertLDivQ;
    procedure InsertCIS;
    procedure InsertSyndiv;
    procedure InsertABS;
    procedure InsertABSL;
    procedure InsertABSR;
    procedure InsertNorm;
    procedure InsertNormL;
    procedure InsertNormR;
    procedure InsertOverbar;
    procedure InsertUndrbar;
    procedure InsertOvdbbar;
    procedure InsertHLine;
    procedure InsertDHLine;
    procedure InsertUndbbar;
    procedure InsertArrowL;
    procedure InsertArrowR;
    procedure InsertArrowDB;
    procedure InsertRayL;
    procedure InsertRayR;
    procedure InsertRayDB;
    procedure InsertOverbrc;
    procedure InsertUndrbrc;
    procedure InsertArc;
    procedure InsertSlash;
    procedure InsertHat;
    procedure InsertTilde;
    procedure InsertAccent;
    procedure InsertUmlaut;
    procedure InsertPrime;
    procedure InsertPrime2;
    procedure InsertPrime3;
    procedure InsertRep;
    procedure InsertMat;
    procedure InsertMatN(Rows,Columns,Just:Integer);
    procedure InsertMatL;
    procedure InsertMatR;
    procedure InsertAugment;
    procedure InsertAugmentL;
    procedure InsertAugmentR;
    procedure InsertRow;
    procedure InsertRowL;
    procedure InsertRowR;
    procedure InsertColumn;
    procedure InsertColumnL;
    procedure InsertColumnR;
    procedure InsertTab;
    procedure InsertTabL;
    procedure InsertTabR;
    procedure InsertTable;
    procedure InsertTableL;
    procedure InsertTableR;
    procedure InsertCheck;
    procedure InsertCheckN(Rows: integer);
    procedure InsertIntegral;
    procedure InsertIntegralN;
    procedure InsertIntegralSymbol;
    procedure InsertIntegralContour;
    procedure InsertSum;
    procedure InsertSumN;
    procedure InsertSumSymbol;
    procedure InsertCBrace;
    procedure InsertCBraceL;
    procedure InsertCBraceR;
    procedure InsertParen;
    procedure InsertParenL;
    procedure InsertParenR;
    procedure InsertBrace;
    procedure InsertBraceL;
    procedure InsertBraceR;
    procedure InsertVector;
    procedure InsertVectorL;
    procedure InsertVectorR;
    procedure InsertGrint;
    procedure InsertDBraceL;
    procedure InsertDBraceR;
    procedure InsertHBrace;
    procedure InsertHBraceT;
    procedure InsertHBraceB;
    procedure InsertBold;
    procedure InsertItalic;
    procedure InsertText(Txt:String);
    procedure InsertBigger(Pts:Integer);
    procedure InsertChar(Ch:Char);
    procedure InsertOp(Op:TIPEqOpType);
    procedure InsertObject(AName:String);
    procedure GetOutline(List:TStrings);
    procedure EnumerateNodes(UserData:Pointer);
    function  IsMultiSelect:Boolean;
    procedure Clear;
    procedure Undo;
    function CanFocus: Boolean; override;
    function  CanUndo:boolean;
    function IsEmpty:boolean;
    procedure DrawToCanvas(ACanvas:TCanvas); virtual;
    property ParentEditor:TIPEqEditor read FParentEditor write FParentEditor;
  published
    property Bevel;
    property BevelEdges;
    property BevelKind default bkNone;
    property DoubleBuffered;
    property Align;
    property Anchors;
    property BevelInner;
    property BevelOuter;
    property BevelWidth;
    property BorderWidth;
    property Color;
    property Constraints;
    property Ctl3D;
    property DragCursor;
    property DragKind;
    property DragMode;
    property Enabled;
    property Font;
    property ParentColor;
    property ParentCtl3D;
    property ParentFont;
    property ParentShowHint;
    property PopupMenu;
    property ShowHint;
    property TabOrder;
    property TabStop;
    property Visible;
    property OnCanResize;
    property OnClick;
    property OnConstrainedResize;
    property OnContextPopup;
    property OnDblClick;
    property OnDragDrop;
    property OnDragOver;
    property OnEndDrag;
    property OnEnter;
    property OnExit;
    property OnMouseDown;
    property OnMouseMove;
    property OnMouseUp;
    property OnResize;
    property OnStartDrag;
    property OnKeyPress;
    property OnCreateEditor:TIPEqCreateEditorEvent read FOnCreateEditor write FOnCreateEditor;

    { Custom properties }
    property HighlightColor : TColor read FHighlightColor write FHighlightColor;
    property EqDocument:TIPEqDocument read FEqDoc write FEqDoc stored true;
    property AutoSize;
    property OnUndoChanged:TNotifyEvent read FOnUndoChanged write FOnUndoChanged;
    property OnChanged:TNotifyEvent read FOnChanged write FOnChanged;
    property Text:String read GetText write SetText;
    property ReadOnly:boolean read FReadOnly write SetReadOnly;
    property Descent:Integer read GetDescent;
  end;

  TIPEqUndo = class
  private
    FEqDoc : TIPEqDocument;
    FCaretPos : Integer;
    FOldCaretPos : Integer;
    FCurrentRow : TIPEqRow;
    FUpdateAble : boolean;
  public
    Constructor Create(EqEditor:TIPEqEditor; AUpdatable:boolean);
    Destructor Destroy; override;
    procedure DetermineCurrentRow(EqEditor:TIPEqEditor);
    procedure Undo(EqEditor:TIPEqEditor);
  end;


procedure Register;

implementation


uses IPEqSqrt,IPEqDivide,IPEqText,IPEqChar,Math,ststrl, IPEqSuperScript,
  IPEqComPosite, IPEqUtils, IPEqParen, Forms,IPEqSupSub,IPEqOverUnder,
  IPEqLDiv,IPEqCIS,IPEqABS,IPEqBar,IPEqMat,IPEqIntegral,IPEqSum,IPEqCBrace,
  IPEqObject,IPEqPrime,IPEqCheck,IPEqItalic,IPEqBigger;

constructor TIPEqEditor.Create(AOwner:TComponent);
begin
  inherited Create(AOwner);
  ControlStyle := ControlStyle + [csOpaque];
  Width := 250;
  Height := 250;
  FEqDoc := TIPEqDocument.Create;
  FEqDoc.Editable := true;
  FEqDoc.OnCreateControl := CreateEqControl;
  FEqDoc.Font := Font;
  FCurrentRow := FEqDoc;
  FEqDoc.Container := Self;
  FCaretPos := 0;
  TabStop := true;
  FHighlightColor := clHighlight;

  FUndoBuffer := TStack.Create;

  FEqDoc.Enabled := Enabled and not FReadOnly;

end;

destructor TIPEqEditor.Destroy;
begin
  inherited Destroy;
  FEqDoc.Free;
  ClearUndoItems;
  FUndoBuffer.Free;
end;

procedure TIPEqEditor.Loaded;
begin
  inherited Loaded;
  FEqDoc.SetLocation(BevelExt,BevelExt+FInternalLeading);
  AdjustSize;
end;

function TIPEqEditor.GetDescent:Integer;
begin
  Result := FEqDoc.Descent + BevelExt;
end;


function  TIPEqEditor.CreateEqControl(Sender:TIPEqDocument; AName:String):TControl;
var
  ED : TIPEqEditor;
begin
  ED := nil;
  if Assigned(FOnCreateEditor) then
    ED := FOnCreateEditor(Self,AName);
  if Assigned(ED) then
  begin
    ED.FParentEditor := Self;
  end;
  Result := ED;
end;

procedure TIPEqEditor.DoChanged;
begin
  if Assigned(FOnChanged) then
    FOnChanged(Self);
end;

function TIPEqEditor.CanFocus: Boolean;
begin
  Result := inherited CanFocus and not Readonly;
end;

procedure TIPEqEditor.SetReadOnly(Value:boolean);
var
  Frm: TCustomForm;
begin
  if Value <> FReadOnly then
  begin
    FReadOnly := Value;
    FEqDoc.Enabled := Enabled and not FReadonly;
    if FReadOnly and Focused then
    begin
      Frm := GetParentForm(Self);
      if Frm <> nil then Frm.DefocusControl(Self,false);
      //For some reason in activeX component the editor will not loose the focus
      //and the caret stays.  Let's force it away here.  It shouldn't hurt.
      DestroyCaret;
    end;
  end;
end;

procedure TIPEqEditor.CMEnabledChanged(var Message: TMessage);
begin
  inherited;
  FEqDoc.Enabled := Enabled and not FReadOnly;
  ResetMultiSelect;
end;


procedure TIPEqEditor.InitSize;
begin
   AdjustSize;
end;

function TIPEqEditor.CanAutoSize(var NewWidth, NewHeight: Integer): Boolean;
begin
  Result := AutoSize;
  if not Result then
    Exit;
  if not (csDesigning in ComponentState) or (FEqDoc.Width > 0) and
    (FEqDoc.Height > 0) then
  begin

    //Added this as a last resort.  When adding a wincontrol this gets called
    //before while in the middle of calculating the metrics.  So the metrics comes up
    //as nil;
    if FEqDoc.Metrics = nil then
      Exit;

    if Align in [alNone, alLeft, alRight] then
      NewWidth := FEqDoc.Width+2*(BevelExt+BorderWidth);
    if Align in [alNone, alTop, alBottom] then
      NewHeight := FEqDoc.Height+2*(BevelExt+BorderWidth)+FInternalLeading;
  end;
end;


procedure TIPEqEditor.ClearUndoItems;
begin
  while FUndoBuffer.Count > 0 do
    TObject(FUndoBuffer.Pop).Free;
end;

procedure TIPEqEditor.DrawToCanvas(ACanvas:TCanvas);
var
  Dc : Hdc;
  BrushRecall : TBrushRecall;
  PenRecall : TPenRecall;
begin
  BrushRecall := TBrushRecall.Create(ACanvas.Brush);
  PenRecall := TPenRecall.Create(ACanvas.Pen);
  ACanvas.Brush.Style := bsClear;
  ACanvas.Pen.Color := Font.Color;
  Dc := ACanvas.Handle;
  try
    MoveWindowOrg(Dc,BevelExt,BevelExt+FInternalLeading);
    FEqDoc.SetLocation(BevelExt,BevelExt+FInternalLeading);
    FEqDoc.Paint(ACanvas);
    MoveWindowOrg(Dc,-BevelExt,-(BevelExt+FInternalLeading));
    if IsMultiSelect and Focused and Enabled and not Readonly then
      DrawSelection(ACanvas);
  finally
    BrushRecall.Free;
    PenRecall.Free;
  end;
end;

procedure TIPEqEditor.Paint;
begin
  inherited Paint;
  DrawToCanvas(Canvas);
end;

procedure TIPEqEditor.DrawSelection(ACanvas:TCanvas);
var
  BrushRecall : TBrushRecall;
  PenRecall : TPenRecall;
  Rect : TRect;
  OldRop : Integer;
  Dc : Hdc;
begin
  BrushRecall := TBrushRecall.Create(ACanvas.Brush);
  PenRecall := TPenRecall.Create(ACanvas.Pen);
  try
    ACanvas.Brush.Color := FHighlightColor;
    ACanvas.Pen.Color := FHighlightColor;
    Rect := FCurrentRow.GetSelectedRect(FCaretPos,FOldCaretPos);
    Inc(Rect.Bottom);
    Inc(Rect.Right);
    Dc := ACanvas.Handle;
    OldRop := SetRop2(Dc,R2_MERGEPENNOT);
    ACanvas.Rectangle(Rect);
    SetRop2(Dc,OldRop);
  finally
    BrushRecall.Free;
    PenRecall.Free;
  end;

end;


procedure TIPEqEditor.InsertNode(Node:array of TIPEqNode);
begin
  InsertNode(Node,0);
end;

procedure TIPEqEditor.InsertNode(Node:array of TIPEqNode; PrimaryNode:Integer);
var
  N : TIPEqNode;
  RowParent : TIPEqRow;
  Index : Integer;
  Pos : Integer;
begin

  if not Assigned(Node[PrimaryNode]) then
    Exit;

  StoreUndoState;

  if FCurrentRow.isDummyRow  then
  begin
    RowParent := TIPEqRow(FCurrentRow.Parent);
    Index := RowParent.GetChildIndex(FCurrentRow);
    Pos := RowParent.FocusedPosition[Index];
    FCurrentRow := RowParent;
    FCaretPos := Pos;
    FOldCaretPos := Pos;
    if not((Node[0] is TIPEqRow) and TIPEqRow(Node[0]).IsDummyRow) then
      FCurrentRow.RemoveChildAt(Index).Free;
  end;

  if IsMultiSelect then
    N := FCurrentRow.InsertNodeRange(Node,FOldCaretPos,FCaretPos,PrimaryNode)
  else
  begin
    FCurrentRow.InsertNode(Node,FCaretPos);
    N := Node[PrimaryNode];
  end;

  N.InitCaret(FCurrentRow,FCaretPos);

  AdjustSize;

  ResetMultiSelect;

  DoChanged;

end;

function  TIPEqEditor.GetText:String;
begin
  Result := FEqDoc.Text;
end;

procedure TIPEqEditor.SetText(Value:String);
begin
  FEqDoc.SetText(Value);
  AdjustSize;
  FCurrentRow := FEqDoc;
  FCaretPos := 0;
  FOldCaretPos := 0;
  Invalidate;
  UpdateCaret;
end;

procedure TIPEqEditor.InsertSqrt;
begin
  InsertNode([TIPEqSqrt.Create]);
end;

procedure TIPEqEditor.InsertMixedNumber;
var
  Node : TIPEqNode;
  Pos : Integer;
begin

  Pos := Min(FCaretPos,FoldCaretPos);
  if Pos > 0  then
  begin
    Node := FCurrentRow.GetNodeBeforeCaret(Pos);
    if (Node is TIPEqText) then
    begin
      InsertNode([TIPEqOp.Create(eqoSpace),TIPEqRow.CreateDummy,TIPEqDivide.Create],1);
      Exit;
    end;
  end;

  InsertNode([TIPEqRow.CreateDummy,TIPEqDivide.Create]);
end;



procedure TIPEqEditor.InsertOrderedPair;
begin

  InsertNode([TIPEqParen.Create('('),TIPEqRow.CreateDummy,
               TIPEqChar.Create(','),TIPEqRow.CreateDummy,
               TIPEqParen.Create(')')],1);

end;


procedure TIPEqEditor.InsertSymbol(Symbol:TIPEqSymbolType);
begin
  InsertNode([TIPEqSymbolW.Create(Symbol)]);
end;

procedure TIPEqEditor.InsertNRoot;
begin
  InsertNode([TIPEqSqrt.CreateN]);
end;

procedure TIPEqEditor.InsertDivide;
begin
  InsertNode([TIPEqDivide.Create]);
end;

procedure TIPEqEditor.InsertDivM;
begin
  InsertNode([TIPEqDivide.CreateM]);
end;

procedure TIPEqEditor.InsertOverUnder;
begin
  InsertNode([TIPEqOverUnder.Create]);
end;

procedure TIPEqEditor.InsertOver;
begin
  InsertNode([TIPEqOverUnder.CreateOver]);
end;

procedure TIPEqEditor.InsertUnder;
begin
  InsertNode([TIPEqOverUnder.CreateUnder]);
end;

procedure TIPEqEditor.InsertLDiv;
begin
  InsertNode([TIPEqLDiv.Create]);
end;

procedure TIPEqEditor.InsertLDivQ;
begin
  InsertNode([TIPEqLDiv.CreateN]);
end;

procedure TIPEqEditor.InsertCIS;
begin
  InsertNode([TIPEqCIS.Create]);
end;

procedure TIPEqEditor.InsertSyndiv;
begin
  InsertNode([TIPEqCIS.CreateN]);
end;

procedure TIPEqEditor.InsertABS;
begin
  InsertNode([TIPEqABS.Create]);
end;

procedure TIPEqEditor.InsertABSL;
begin
  InsertNode([TIPEqABS.CreateABSL]);
end;

procedure TIPEqEditor.InsertABSR;
begin
  InsertNode([TIPEqABS.CreateABSR]);
end;

procedure TIPEqEditor.InsertNorm;
begin
  InsertNode([TIPEqABS.CreateN]);
end;

procedure TIPEqEditor.InsertNormL;
begin
  InsertNode([TIPEqABS.CreateDL]);
end;

procedure TIPEqEditor.InsertNormR;
begin
  InsertNode([TIPEqABS.CreateDR]);
end;

procedure TIPEqEditor.InsertSuperScript;
begin
    InsertNode([TIPEqSuperScript.Create]);
end;

procedure TIPEqEditor.InsertSuperScriptX;
begin
    InsertNode([TIPEqRow.Create,TIPEqSuperScript.Create]);
end;


procedure TIPEqEditor.InsertSubSub;
begin
    InsertNode([TIPEqSubScript.Create,TIPEqRow.Create,TIPEqSubScript.Create]);
end;


procedure TIPEqEditor.InsertSupSub;
begin
  InsertNode([TIPEqSupSub.Create]);
end;

procedure TIPEqEditor.InsertOverbar;
begin
  InsertNode([TIPEqBar.Create]);
end;

procedure TIPEqEditor.InsertUndrbar;
begin
  InsertNode([TIPEqBar.CreateUN]);
end;

procedure TIPEqEditor.InsertOvdbbar;
begin
  InsertNode([TIPEqBar.CreateOVDB]);
end;

procedure TIPEqEditor.InsertHLine;
begin
  InsertNode([TIPEqBar.CreateHLine]);
end;

procedure TIPEqEditor.InsertDHLine;
begin
  InsertNode([TIPEqBar.CreateDHline]);
end;

procedure TIPEqEditor.InsertUndbbar;
begin
  InsertNode([TIPEqBar.CreateUNDB]);
end;

procedure TIPEqEditor.InsertArrowL;
begin
  InsertNode([TIPEqBar.CreateArrowL]);
end;

procedure TIPEqEditor.InsertArrowR;
begin
  InsertNode([TIPEqBar.CreateArrowR]);
end;

procedure TIPEqEditor.InsertArrowDB;
begin
  InsertNode([TIPEqBar.CreateArrowDB]);
end;

procedure TIPEqEditor.InsertRayL;
begin
  InsertNode([TIPEqBar.CreateRayL]);
end;

procedure TIPEqEditor.InsertRayR;
begin
  InsertNode([TIPEqBar.CreateRayR]);
end;

procedure TIPEqEditor.InsertRayDB;
begin
  InsertNode([TIPEqBar.CreateRayDB]);
end;

procedure TIPEqEditor.InsertOverbrc;
begin
  InsertNode([TIPEqBar.CreateOverbrc]);
end;

procedure TIPEqEditor.InsertUndrbrc;
begin
  InsertNode([TIPEqBar.CreateUndrbrc]);
end;

procedure TIPEqEditor.InsertArc;
begin
  InsertNode([TIPEqBar.CreateArc]);
end;

procedure TIPEqEditor.InsertSlash;
begin
  InsertNode([TIPEqBar.CreateSlash]);
end;

procedure TIPEqEditor.InsertHat;
begin
  InsertNode([TIPEqBar.CreateHat]);
end;

procedure TIPEqEditor.InsertTilde;
begin
  InsertNode([TIPEqBar.CreateTilde]);
end;

procedure TIPEqEditor.InsertAccent;
begin
  InsertNode([TIPEqBar.CreateAccent]);
end;

procedure TIPEqEditor.InsertUmlaut;
begin
  InsertNode([TIPEqBar.CreateUmlaut]);
end;

procedure TIPEqEditor.InsertPrime;
begin
  InsertNode([TIPEqPrime.Create]);
end;

procedure TIPEqEditor.InsertPrime2;
begin
  InsertNode([TIPEqPrime.CreatePrime2]);
end;

procedure TIPEqEditor.InsertPrime3;
begin
  InsertNode([TIPEqPrime.CreatePrime3]);
end;

procedure TIPEqEditor.InsertRep;
begin
  InsertNode([TIPEqBar.CreateRep]);
end;

procedure TIPEqEditor.InsertMat;
begin
  InsertNode([TIPEqMat.Create]);
end;

procedure TIPEqEditor.InsertMatN(Rows,Columns,Just:Integer);
begin
  InsertNode([TIPEqMat.CreateN(Rows,Columns,Just)]);
end;

procedure TIPEqEditor.InsertMatL;
begin
  InsertNode([TIPEqMat.CreateL]);
end;

procedure TIPEqEditor.InsertMatR;
begin
  InsertNode([TIPEqMat.CreateR]);
end;

procedure TIPEqEditor.InsertAugment;
begin
  InsertNode([TIPEqMat.CreateAug]);
end;

procedure TIPEqEditor.InsertAugmentL;
begin
  InsertNode([TIPEqMat.CreateAugL]);
end;

procedure TIPEqEditor.InsertAugmentR;
begin
  InsertNode([TIPEqMat.CreateAugR]);
end;

procedure TIPEqEditor.InsertRow;
begin
  InsertNode([TIPEqMat.CreateRow]);
end;

procedure TIPEqEditor.InsertRowL;
begin
  InsertNode([TIPEqMat.CreateRowL]);
end;

procedure TIPEqEditor.InsertRowR;
begin
  InsertNode([TIPEqMat.CreateRowR]);
end;

procedure TIPEqEditor.InsertColumn;
begin
  InsertNode([TIPEqMat.CreateColumn]);
end;

procedure TIPEqEditor.InsertColumnL;
begin
  InsertNode([TIPEqMat.CreateColumnL]);
end;

procedure TIPEqEditor.InsertColumnR;
begin
  InsertNode([TIPEqMat.CreateColumnR]);
end;

procedure TIPEqEditor.InsertTab;
begin
  InsertNode([TIPEqMat.CreateTab]);
end;

procedure TIPEqEditor.InsertTabL;
begin
  InsertNode([TIPEqMat.CreateTabL]);
end;

procedure TIPEqEditor.InsertTabR;
begin
  InsertNode([TIPEqMat.CreateTabR]);
end;

procedure TIPEqEditor.InsertTable;
begin
  InsertNode([TIPEqMat.CreateTable]);
end;

procedure TIPEqEditor.InsertTableL;
begin
  InsertNode([TIPEqMat.CreateTableL]);
end;

procedure TIPEqEditor.InsertTableR;
begin
  InsertNode([TIPEqMat.CreateTableR]);
end;

procedure TIPEqEditor.InsertCheck;
begin
  InsertNode([TIPEqCheck.CreateN(3)]);
end;
procedure TIPEqEditor.InsertCheckN(Rows:integer);
begin
  InsertNode([TIPEqCheck.CreateN(Rows)]);
end;


procedure TIPEqEditor.InsertIntegral;
begin
  InsertNode([TIPEqIntegral.Create]);
end;

procedure TIPEqEditor.InsertIntegralN;
begin
  InsertNode([TIPEqIntegral.CreateN]);
end;

procedure TIPEqEditor.InsertIntegralSymbol;
begin
  InsertNode([TIPEqIntegral.CreateSymbol]);
end;

procedure TIPEqEditor.InsertIntegralContour;
begin
  InsertNode([TIPEqIntegral.CreateContour]);
end;

procedure TIPEqEditor.InsertSum;
begin
  InsertNode([TIPEqSum.Create]);
end;

procedure TIPEqEditor.InsertSumN;
begin
  InsertNode([TIPEqSum.CreateN]);
end;

procedure TIPEqEditor.InsertSumSymbol;
begin
  InsertNode([TIPEqSum.CreateSymbol]);
end;

procedure TIPEqEditor.InsertCBrace;
begin
  InsertNode([TIPEqCBrace.Create]);
end;

procedure TIPEqEditor.InsertCBraceL;
begin
  InsertNode([TIPEqCBrace.CreateCBraceL]);
end;

procedure TIPEqEditor.InsertCBraceR;
begin
  InsertNode([TIPEqCBrace.CreateCBraceR]);
end;

procedure TIPEqEditor.InsertParen;
begin
  InsertNode([TIPEqCBrace.CreateParen]);
end;

procedure TIPEqEditor.InsertParenL;
begin
  InsertNode([TIPEqCBrace.CreateParenL]);
end;

procedure TIPEqEditor.InsertParenR;
begin
  InsertNode([TIPEqCBrace.CreateParenR]);
end;

procedure TIPEqEditor.InsertBrace;
begin
  InsertNode([TIPEqCBrace.CreateBrace]);
end;

procedure TIPEqEditor.InsertBraceL;
begin
  InsertNode([TIPEqCBrace.CreateBraceL]);
end;

procedure TIPEqEditor.InsertBraceR;
begin
  InsertNode([TIPEqCBrace.CreateBraceR]);
end;

procedure TIPEqEditor.InsertVector;
begin
  InsertNode([TIPEqCBrace.CreateVector]);
end;

procedure TIPEqEditor.InsertVectorL;
begin
  InsertNode([TIPEqCBrace.CreateVectorL]);
end;

procedure TIPEqEditor.InsertVectorR;
begin
  InsertNode([TIPEqCBrace.CreateVectorR]);
end;

procedure TIPEqEditor.InsertGrint;
begin
  InsertNode([TIPEqCBrace.CreateGrint]);
end;

procedure TIPEqEditor.InsertDBraceL;
begin
  InsertNode([TIPEqCBrace.CreateDBraceL]);
end;

procedure TIPEqEditor.InsertDBraceR;
begin
  InsertNode([TIPEqCBrace.CreateDBraceR]);
end;

procedure TIPEqEditor.InsertHBrace;
begin
  InsertNode([TIPEqCBrace.CreateHBrace]);
end;

procedure TIPEqEditor.InsertHBraceT;
begin
  InsertNode([TIPEqCBrace.CreateHBraceT]);
end;

procedure TIPEqEditor.InsertHBraceB;
begin
  InsertNode([TIPEqCBrace.CreateHBraceB]);
end;

procedure TIPEqEditor.InsertBold;
begin
  InsertNode([TIPEqItalic.Create(fsBold)]);
  FEqDoc.InvalidateAll;
  Invalidate;
  AdjustSize;
  UpdateCaret;
end;

procedure TIPEqEditor.InsertItalic;
begin
  InsertNode([TIPEqItalic.Create(fsItalic)]);
  FEqDoc.InvalidateAll;
  Invalidate;
  AdjustSize;
  UpdateCaret;
end;

procedure TIPEqEditor.InsertBigger(Pts:Integer);
begin
  InsertNode([TIPEqBigger.Create(Pts)]);
  FEqDoc.InvalidateAll;
  Invalidate;
  AdjustSize;
  UpdateCaret;
end;

procedure TIPEqEditor.InsertSubScript;
begin
//  if FCaretPos = 0 then
//    InsertNode([TIPEqRow.CreateDummy,TIPEqSubScript.Create])
//  else
  InsertNode([TIPEqSubScript.Create]);
end;

procedure TIPEqEditor.InsertText(Txt:String);
var
 I : Integer;
begin
  for I := 1 to Length(Txt) do
    KeyPress(Txt[I]);

  Invalidate;
  AdjustSize;
end;

procedure TIPEqEditor.InsertChar(Ch:Char);
begin
  KeyPress(Ch);
end;

procedure TIPEqEditor.InsertObject(AName:String);
begin
  InsertNode([TIPEqObject.Create(AName)]);
end;


procedure TIPEqEditor.InsertOp(Op:TIPEqOpType);
begin
  InsertNode([TIPEqOp.Create(Op)]);
end;

function TIPEqEditor.GetCaretLocation:TRect;
var
  R1,R2 : TRect;
begin

  FEqDoc.SetLocation(BevelExt,BevelExt+FInternalLeading);

  if IsMultiSelect then
  begin
    R1 := FCurrentRow.GetCaretLocation(FCaretPos);
    R2 := FCurrentRow.GetCaretLocation(FOldCaretPos);
    UnionRect(Result,R1,R2);
  end
  else
    Result := FCurrentRow.GetCaretLocation(FCaretPos);
end;

procedure TIPEqEditor.DoEnter;
begin
  inherited DoEnter;
end;

procedure TIPEqEditor.DoExit;
begin
  inherited DoExit;
end;

procedure TIPEqEditor.UpdateCaret;
var
  Rect : TRect;
  H : Integer;
begin
  if not Focused then
    Exit;

  if not Enabled or Readonly or IsMultiSelect then
  begin
    if FCaretHeight > 0 then
    begin
      DestroyCaret;
      FCaretHeight := -1;
    end;
    Invalidate;
    Exit;
  end;

  Rect := GetCaretLocation;
//***MAD***
//***MAY not be needed now that I set location in EqDoc
//***  OffsetRect(Rect,BevelExt,BevelExt+FInternalLeading);

  H := Rect.Bottom-Rect.Top;
  if EqAlwaysCreateCaret or (FCaretHeight <> H) then
  begin
    if FCaretHeight > 0 then
      DestroyCaret;
    FCaretHeight := H;
    CreateCaret(Handle,0,1,H);
    SetCaretPos(Max(Rect.Left,0),Math.Max(Rect.Top,0));
    if not ShowCaret(Handle) then
    begin
//      RaiseLastOSError;
    end;
  end
  else
  begin
    SetCaretPos(Max(Rect.Left,0),Math.Max(Rect.Top,0));
  end;
end;

procedure TIPEqEditor.MouseDown(Button: TMouseButton; Shift: TShiftState;
  X, Y: Integer);
var
  Node : TIPEqNode;
  OldRow :TIPEqRow;
begin
  inherited;
  if ReadOnly then
    Exit;

  Node := FEqDoc.GetNodeAt(X,Y);
  OldRow := FCurrentRow;
  if Node.InheritsFrom(TIPEqRow) then
  begin
    FCurrentRow := TIPEqRow(Node);
    FCaretPos := Node.GetCaretPositionAt(X,Y);
    FOldCaretPos := FCaretPos;
    if FCurrentRow <> OldRow then
      RefreshUndoState;
  end;
  if Focused then
    ResetMultiSelect
  else
    SetFocus;

end;

procedure TIPEqEditor.MouseMove(Shift: TShiftState; X, Y: Integer);
var
  Node :TIPEqNode;
begin
  inherited;
  if ssLeft in Shift then
  begin
    Node := FEqDoc.GetNodeAt(X,Y);
    if Node.InheritsFrom(TIPEqRow) then
    begin
      if Node = FCurrentRow then
      begin
        FCaretPos := Node.GetCaretPositionAt(X,Y);
        UpdateCaret;
      end
      else
      begin
      end;
    end;
  end;
end;


procedure TIPEqEditor.MouseUp(Button: TMouseButton; Shift: TShiftState;
  X, Y: Integer);
begin
  inherited;
end;

procedure TIPEqEditor.WMGetDlgCode(var Msg: TWMGetDlgCode);
begin
  Msg.Result := DLGC_WANTARRows or DLGC_WANTTAB or DLGC_WANTCHARS;
end;

procedure TIPEqEditor.Delete(IsBackspace:Boolean);
var
  CaretEvent : TIPEqCaretEvent;
  WasMultiSelect : boolean;
begin
  WasMultiSelect := IsMultiSelect;

  if FCaretPos >= 0then
  begin

    if not IsBackspace then
      CaretEvent := TIPEqCaretEvent.Create(cdNone,FCurrentRow,FOldCaretPos,FCaretPos)
    else
    begin
      if not WasMultiSelect then
      begin
        Dec(FCaretPos);
        FOldCaretPos := FCaretPos;
      end;
      CaretEvent := TIPEqCaretEvent.Create(cdNone,FCurrentRow,FOldCaretPos,FCaretPos);
    end;
    try
      FCurrentRow.DeleteCharacter(CaretEvent);
      if Assigned(CaretEvent.Row) then
      begin
        FCurrentRow := CaretEvent.Row;
        if CaretEvent.ExtendSelection then
        begin
          FOldCaretPos := Max(CaretEvent.PositionStart,0);
          FCaretPos := Max(CaretEvent.Position,0);
          AdjustSize;
          UpdateCaret;
        end
        else
        begin
          if WasMultiSelect then
            FCaretPos := Max(CaretEvent.PositionStart,0)
          else
            FCaretPos := Max(CaretEvent.Position,0);
          AdjustSize;
          ResetMultiSelect;
        end;
      end;
    finally
      CaretEvent.Free;
    end;
  end;
  DoChanged;
end;

procedure TIPEqEditor.KeyUp(var Key: Word; Shift: TShiftState);
begin
  inherited KeyUp(Key,Shift);
  if ssCtrl in Shift then
  begin
  end;
end;


procedure TIPEqEditor.KeyDown(var Key: Word; Shift: TShiftState);
var
  CaretEvent : TIPEqCaretEvent;
  Direction : TIPEqCaretDirection;
  WasMultiSelect : boolean;
  OldRow : TIPEqRow;
begin
  inherited KeyDown(Key,Shift);

  WasMultiSelect := IsMultiSelect;

  FControlPressed := ssCtrl in Shift;

  if (Key = VK_DELETE) or ((Key = VK_BACK) and (FCaretPos >= 0))then
  begin
    StoreUndoState;
    Delete(Key = VK_BACK);
    Exit;
  end;

  case Key of
    VK_LEFT  : Direction := cdLeft;
    VK_RIGHT : Direction := cdRight;
    VK_UP    : Direction := cdUp;
    VK_DOWN  : Direction := cdDown;
    VK_HOME  : Direction := cdHome;
    VK_END   : Direction := cdEnd;
    VK_TAB   :
      begin
        if ssShift in Shift then
          Direction := cdBackTab
        else
          Direction := cdTab;
      end;
    else
      Direction := cdNone;
  end;

  if ssShift in Shift then
  begin
    if Direction = cdUp then
    begin
      InsertSuperScript;
      Exit;
    end
    else if Direction = cdDown then
    begin
      InsertSubScript;
      Exit;
    end;
  end;

  //Handle Caret Movement
  if Direction <> cdNone then
  begin

    OldRow := FCurrentRow;

    CaretEvent := TIPEqCaretEvent.Create(Direction,FCurrentRow,FOldCaretPos,FCaretPos);
    CaretEvent.PositionStart := FOldCaretPos;
    if (ssShift in Shift) and (Key <> VK_TAB) then
      CaretEvent.ExtendSelection := true;
    try
      FCurrentRow.MoveCaret(CaretEvent);
      if Assigned(CaretEvent.Row) then
      begin
        if not CaretEvent.ExtendSelection then
          FOldCaretPos := CaretEvent.Position
        else
          FOldCaretPos := CaretEvent.PositionStart;
        FCurrentRow := CaretEvent.Row;
        FCaretPos := CaretEvent.Position;
        if FCurrentRow <> OldRow then
          RefreshUndoState;
        if IsMultiSelect or (IsMultiSelect <> WasMultiSelect) then
          Invalidate;
        UpdateCaret;
      end;
    finally
      CaretEvent.Free;
    end;
  end
  else
  begin
    if FControlPressed then
    begin
      ProcessControlKeys(Char(Key))
    end;
  end;
end;

function TIPEqEditor.ProcessShortCuts(Key:Char):Boolean;
begin
  Result := true;
  case Key of
    '^' : InsertSuperScript;
    '_' : InsertSubScript;
  else
   Result := false;
  end;
end;


function TIPEqEditor.ProcessControlKeys(Key:Char):Boolean;

begin
  if Key = #17 then
  begin
    Result := true;
    Exit;
  end;
  Result := true;
  case Key of
    'a','A' : InsertSymbol(eqsAlpha);
    'b','B' : InsertSymbol(eqsBeta);
    'g','G' : InsertSymbol(eqsGamma);
    'i','I' : InsertSymbol(eqsInf);
    'n','N' : InsertSymbol(eqsCap);
    'o','O' : InsertSymbol(eqsTheta);
    'p','P' : InsertSymbol(eqsPi);
    'u','U' : InsertSymbol(eqsCup);
    'z','Z' : Undo;
    '6'     : InsertSymbol(eqsOr);
    'r','R' : ; //do nothing, but this will stop bRowser from refreshing.
    'm','M' : ;//do nothing, this will stop the enter
    '4'     : InsertChar(CurrencyChars[FEqDoc.CurrencyType]);
  else
   Result := false;
  end;
end;

procedure TIPEqEditor.KeyPress(var Key: Char);
var
  CharEvent : TIPEqCharEvent;

begin
  inherited KeyPress(Key);

  if processShortCuts(key) then
    Exit;

  //Ignore non printable characters
  if (Key < #32) or ((Key > #126) and not (Key in [#0163,#0128,#0165])) then
    Exit;

  if isText(Key) then
    UpdateUndoState
  else
    StoreUndoState;

  if IsMultiSelect then
    Delete(false);

  CharEvent := TIPEqCharEvent.Create(Key,FCurrentRow,FCaretPos);
  try
    FCurrentRow.InsertCharacter(CharEvent);
    if Assigned(CharEvent.Row) then
    begin
     FCurrentRow := CharEvent.Row;
     FCaretPos := CharEvent.Position;
     AdjustSize;
     ResetMultiSelect;
     DoChanged;
    end;
  finally
    CharEvent.Free;
  end;

end;

procedure TIPEqEditor.EnumerateNodes(UserData:Pointer);
var
  Level : Integer;
  Stop : boolean;

  procedure EnumChildren(P:TIPEqList);
  var
    I : Integer;
  begin
    for I := 0 to P.ChildCount-1 do
    begin
      Stop := false;
      FOnEnumerate(Self,P.Child[I],Stop,UserData,Level);
      if stop then
        Exit;
      if p.Child[I].InheritsFrom(TIPEqList) then
      begin
        Inc(Level);
        EnumChildren(TIPEqList(P.Child[I]));
        if Stop then
          Exit;
        Dec(Level);
      end;
    end;
  end;
begin
  if Assigned(FOnEnumerate) then
  begin
    Level := 0;
    EnumChildren(FEqDoc);
  end;
end;

procedure TIPEqEditor.GetOutline(List:TStrings);
var
  Level : Integer;

  procedure WriteChildren(P:TIPEqList);
  var
    I : Integer;
    FString:String;
  begin
    for I := 0 to P.ChildCount-1 do
    begin
      if P.Child[I] = FCurrentRow then
        FString := ' FOCUSED at Position '+IntToStr(FCaretPos)
      else
        FString := '';
      List.Add(CharStrL(' ',Level)+P.Child[I].toString+FString);
      if P.Child[I].InheritsFrom(TIPEqList) then
      begin
        Inc(Level);
        WriteChildren(TIPEqList(P.Child[I]));
        dec(Level);
      end;
    end;
  end;

begin
  Level := 0;
  WriteChildren(FEqDoc);
end;

function TIPEqEditor.GetTextMetrics:TTextMetric;
var
  DisplayDC: HDC;
  TxtMetric: TTEXTMETRIC;
begin
  DisplayDC := GetDC(0);
  if (DisplayDC <> 0) then
  begin
    if (SelectObject(DisplayDC, Font.Handle) <> 0) then
      Windows.GetTextMetrics(DisplayDC, TxtMetric);
    ReleaseDC(0, DisplayDC);
  end;
  Result := TxtMetric;
end;

procedure TIPEqEditor.CMFontChanged(var Message: TMessage);
var
  TextMetric : TTextMetric;
begin
  FEqDoc.Font := Font;
  TextMetric := GetTextMetrics;
  AdjustSize;
  inherited;
end;

procedure TIPEqEditor.ResetMultiSelect;
begin
  FOldCaretPos := FCaretPos;
  Invalidate;
  UpdateCaret;
end;

function  TIPEqEditor.IsMultiSelect:Boolean;
begin
  Result := FCaretPos <> FOldCaretPos;
end;

procedure TIPEqEditor.Clear;
begin
  FCurrentRow := FEqDoc;
  FCaretPos := 0;
  FOldCaretPos := 0;
  FEqDoc.Clear;
  ClearUndoItems;
  AdjustSize;
  ResetMultiSelect;
  DoUndoChanged;
  DoChanged;
end;

procedure TIPEqEditor.StoreUndoState;
begin
  FUndoBuffer.Push(TIPEqUndo.Create(Self,false));
  DoUndoChanged;
end;

procedure TIPEqEditor.UpdateUndoState;
var
  Undo : TIPEqUndo;
  UState : boolean;
begin
  UState := false;
  if FUndoBuffer.Count > 0 then
  begin
    Undo := TIPEqUndo(FUndoBuffer.Peek);
    UState := Undo.FUpdateAble;
  end;
  if not UState then
  begin
    FUndoBuffer.Push(TIPEqUndo.Create(Self,true));
    DoUndoChanged;
  end;

end;

procedure TIPEqEditor.RefreshUndoState;
var
  Undo : TIPEqUndo;
begin
  if FUndoBuffer.Count > 0 then
  begin
    Undo := TIPEqUndo(FUndoBuffer.Peek);
    Undo.FUpdateAble := false;
  end;
end;

function  TIPEqEditor.CanUndo:boolean;
begin
  Result := FUndoBuffer.Count > 0;
end;

procedure TIPEqEditor.Undo;
var
  Undo : TIPEqUndo;
begin
  if FUndoBuffer.Count > 0 then
  begin
    Undo := TIPEqUndo(FUndoBuffer.Pop);
    Undo.Undo(Self);
    Undo.Free;
    AdjustSize;
    UpdateCaret;
    DoUndoChanged;
    DoChanged;
    Invalidate;
  end;
end;


Constructor TIPEqUndo.Create(EqEditor:TIPEqEditor; AUpdatable:boolean);
begin
  FEqDoc := TIPEqDocument(EqEditor.FEqDoc.Clone);
  FCaretPos := EqEditor.FCaretPos;
  FOldCaretPos := EqEditor.FOldCaretPos;
  FUpdateable := AUpdatable;
  DetermineCurrentRow(EqEditor);
end;

Destructor TIPEqUndo.Destroy;
begin
  inherited Destroy;
  FEqDoc.Free;
end;

procedure TIPEqUndo.DetermineCurrentRow(EqEditor:TIPEqEditor);
var
  PosList : TStack;
  Node : TIPEqList;
  Index : Integer;
begin
  PosList := TStack.Create;
  try
    Node := EqEditor.FCurrentRow;
    while Assigned(Node.Parent) do
    begin
      Index := Node.Parent.GetChildIndex(Node);
      PosList.Push(Pointer(Index));
      Node := Node.parent;
    end;

    Node := FEqDoc;
    while PosList.Count > 0 do
    begin
      Index := Integer(PosList.Pop);
      Node := TIPEqList(Node.Child[Index]);
    end;

    FCurrentRow := Node as TIPEqRow;

  finally
    PosList.Free;
  end;
end;


procedure TIPEqUndo.Undo(EqEditor:TIPEqEditor);
begin
  EqEditor.FEqDoc.Free;
  EqEditor.FEqDoc := FEqDoc;
  EqEditor.FEqDoc.Font := EqEditor.Font;
  FEqDoc := nil;
  EqEditor.FCaretPos := FCaretPos;
  EqEditor.FOldCaretPos := FOldCaretPos;
  EqEditor.FCurrentRow := FCurrentRow;
end;

procedure TIPEqEditor.CMChildKey(var Message: TCMChildKey);
begin
  if FControlPressed and ProcessControlKeys(Char(Message.CharCode)) then
  begin
    Message.Result := 1;
  end
  else
    inherited;
end;

procedure TIPEqEditor.WMNCHitTest(var Message: TWMNCHitTest);
begin
  if not (csDesigning in ComponentState) then
    Message.Result := HTCLIENT
  else
    inherited;
end;



procedure TIPEqEditor.DoUndoChanged;
begin
  if Assigned(FOnUndoChanged) then  FOnUndoChanged(self);
end;

procedure TIPEqEditor.WMKillFocus(var Msg : TWMKillFocus);
begin
  inherited;
  DestroyCaret;
  FCaretHeight := -1;
  Invalidate;
end;

procedure TIPEqEditor.WMSetFocus(var Msg : TWMSetFocus);
begin
  inherited;
  UpdateCaret;
  Invalidate;
end;

procedure TIPEqEditor.AdjustSize;
var
  W,H : Integer;
begin
  if not (csLoading in ComponentState) and HandleAllocated then
  begin
    inherited AdjustSize;
  end
  else
  begin
    if CanAutoSize(W,H) then
    begin
      Width := W;
      Height := H;
    end;
  end;
end;

procedure TIPEqEditor.Resize;
begin
  inherited Resize;
  if Assigned(FParentEditor) then
  begin
    FParentEditor.EqDocument.SetControlVOffset(Self,-Descent{***MAD***+1});
    FParentEditor.EqDocument.InvalidateAll;
    FParentEditor.AdjustSize;
    FParentEditor.Invalidate;
  end;
end;

function TIPEqEditor.IsEmpty:boolean;
begin
  Result := FEqDoc.ChildCount = 0;
end;


procedure Register;
begin
  RegisterComponents('Intellipro', [TIPEqEditor]);
end;

initialization
  RegisterClasses([TIPEqEditor,TIPEqDocument,TIPEqRow,TIPEqDivide,TIPEqParen,TIPEqSymbol,TIPEqOp,
    TIPEqSqrt,TIPEqSuperScript,TIPEqSubScript,TIPEqText,TIPEqSupSub,TIPEqOverUnder,TIPEqLDiv,
    TIPEqCIS,TIPEqBar,TIPEqMat,TIPEqIntegral,TIPEqSum,TIPEqCBrace]);

end.
