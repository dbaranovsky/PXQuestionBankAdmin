unit IPEqLabel;

interface

uses
  Windows, Messages, SysUtils, Classes, Controls,IPEqNode,Graphics,Contnrs,ExtCtrls;


type


  TIPEqLabel = class(TGraphicControl)
  private
    FEqDoc : TIPEqDocument;
    procedure CMFontChanged(var Message: TMessage); message CM_FONTCHANGED;
    function  GetText:String;
    procedure SetText(Value:String);
    function GetTransparent: Boolean;
    procedure SetTransparent(Value:boolean);
  protected
    procedure Paint; override;
    procedure Loaded; override;
    function CanAutoSize(var NewWidth, NewHeight: Integer): Boolean; override;
  public
    constructor Create(AOwner:TComponent); override;
    destructor Destroy; override;
    procedure InitSize;
    procedure GetOutline(List:TStrings);
    procedure Clear;
  published
    property Align;
    property Anchors;
    property Color;
    property Constraints;
    property DragCursor;
    property DragKind;
    property DragMode;
    property Enabled;
    property Font;
    property ParentColor;
    property ParentFont;
    property ParentShowHint;
    property PopupMenu;
    property ShowHint;
    property Visible;
    property OnCanResize;
    property OnClick;
    property OnConstrainedResize;
    property OnContextPopup;
    property OnDblClick;
    property OnDragDrop;
    property OnDragOver;
    property OnEndDrag;
    property OnMouseDown;
    property OnMouseMove;
    property OnMouseUp;
    property OnResize;
    property OnStartDrag;

    { Custom properties }
    property EqDocument:TIPEqDocument read FEqDoc write FEqDoc stored true;
    property AutoSize;
    property Text:String read GetText write SetText;
    property Transparent:boolean read GetTransparent write SetTransparent;
  end;

procedure Register;

implementation


uses IPEqSqrt,IPEqDivide,IPEqText,IPEqChar,Math,ststrl, IPEqSuperScript,
  IPEqComposite, IPEqUtils, IPEqParen, IPEqOp,Forms;

constructor TIPEqLabel.Create(AOwner:TComponent);
begin
  inherited Create(AOwner);
  ControlStyle := ControlStyle - [csOpaque];
  Width := 250;
  Height := 250;
  FEqDoc := TIPEqDocument.Create;
  FEqDoc.Container := self;
  FEqDoc.Font := Font;


  FEqDoc.Enabled := false;

  AutoSize := true;

end;

destructor TIPEqLabel.Destroy;
begin
  inherited Destroy;
  FEqDoc.Free;
end;

procedure TIPEqLabel.Loaded;
begin
  inherited Loaded;
  AdjustSize;
end;


function TIPEqLabel.GetTransparent: Boolean;
begin
  Result := not (csOpaque in ControlStyle);
end;

procedure TIPEqLabel.SetTransparent(Value: Boolean);
begin
  if Transparent <> Value then
  begin
    if Value then
      ControlStyle := ControlStyle - [csOpaque] else
      ControlStyle := ControlStyle + [csOpaque];
    Invalidate;
  end;
end;

procedure TIPEqLabel.InitSize;
begin
   AdjustSize;
end;

function TIPEqLabel.CanAutoSize(var NewWidth, NewHeight: Integer): Boolean;
begin
  Result := True;
  if not (csDesigning in ComponentState) or (FEqDoc.Width > 0) and
    (FEqDoc.Height > 0) then
  begin
    if Align in [alNone, alLeft, alRight] then
      NewWidth := FEqDoc.Width;
    if Align in [alNone, alTop, alBottom] then
      NewHeight := FEqDoc.Height;
  end;
end;


procedure TIPEqLabel.Paint;
begin
  with Canvas do
  begin
    if not Transparent then
    begin
      Brush.Color := Self.Color;
      Brush.Style := bsSolid;
      FillRect(ClientRect);
    end;
    Brush.Style := bsClear;
    Pen.Color := Font.Color;
  end;

  try
    FEqDoc.Paint(Canvas);
  finally
  end;
end;


function  TIPEqLabel.GetText:String;
begin
  Result := FEqDoc.Text;
end;

procedure TIPEqLabel.SetText(Value:String);
begin
  FEqDoc.SetText(Value);
  AdjustSize;
  Invalidate;
end;

procedure TIPEqLabel.GetOutline(List:TStrings);
var
  Level : Integer;

  procedure WriteChildren(P:TIPEqList);
  var
    I : Integer;
  begin
    for I := 0 to P.ChildCount-1 do
    begin
      List.Add(CharStrL(' ',Level)+P.Child[I].toString);
      if P.Child[I].InheritsFrom(TIPEqList) then
      begin
        Inc(Level);
        WriteChildren(TIPEqList(P.Child[I]));
        Dec(Level);
      end;
    end;
  end;

begin
  Level := 0;
  WriteChildren(FEqDoc);
end;

procedure TIPEqLabel.CMFontChanged(var Message: TMessage);
begin
  FEqDoc.Font := Font;
  AdjustSize;
  inherited;
end;

procedure TIPEqLabel.Clear;
begin
  FEqDoc.Clear;
  AdjustSize;
end;

procedure Register;
begin
  RegisterComponents('Intellipro', [TIPEqLabel]);
end;

end.
