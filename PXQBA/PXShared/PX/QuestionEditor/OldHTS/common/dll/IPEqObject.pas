unit IPEqObject;

interface

uses IPEqNode,Graphics,Windows,Classes,Controls,SysUtils;

type


TIPEqObject = class;

  TNotifyComponent = class(TComponent)
  private
    FEqObj : TIPEqObject;
  public
    procedure Notification(AComponent: TComponent; Operation: TOperation); override;
  end;

TIPEqObject = class(TIPEqNode)
  private
    FName : String;
    FSpaceSize : Integer;
    FVOffset : Integer;
    FControl : TControl;
    FNotifyComp : TNotifyComponent;
    FNoControl : boolean;
    procedure SetName(Value:String);
    function  GetControl:TCOntrol;
    procedure SetVOffset(Value:Integer);
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Draw(ACanvas:TCanvas); override;
    procedure Layout; override;
  public
    constructor Create(AName:String);
    destructor Destroy; override;
    procedure SetLocation(X,Y:Integer); override;
    procedure DeleteCharacter(CaretEvent:TIPEqCaretEvent); override;
    function  Clone:TIPEqNode; override;
    function IsEmpty:boolean; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function GetText:String; override;

    property Name:String read FName write SetName;
    property Control:TControl read GetControl;
    property VOffset:Integer read FVOffset write SetVOffset;

    property SpaceSize:Integer read FSpaceSize write FSpaceSize;
end;


implementation

uses IPEqUtils,Math, IPEqEditor;


procedure TNotifyComponent.Notification(AComponent: TComponent; Operation: TOperation);
begin
  inherited Notification(AComponent,Operation);
  if (AComponent = FEqObj.FControl) and (Operation = opRemove) then
    FEqObj.FControl := nil;
end;

constructor TIPEqObject.Create(AName:String);
begin
  inherited Create;
  FSpaceSize := 25;
  Name := AName;
  FNotifyComp := TNotifyComponent.Create(nil);
  FNotifyComp.FEqObj := Self;
end;

destructor TIPEqObject.Destroy;
begin

  if Document <> nil then
    Document.RemoveEqObject(Self);
    
  inherited Destroy;

  if Assigned(FControl) and not (csDestroying in FControl.ComponentState) then
    FreeAndNil(FControl);
  FNotifyComp.Free;
end;

function  TIPEqObject.Clone:TIPEqNode;
begin
  Result := TIPEqObject.Create(FName);
end;

procedure TIPEqObject.BuildMathML(Buffer:TStrings; Level:Integer);
begin
  Buffer.Add('<input>'+FName+'</input>');
end;

function TIPEqObject.GetText:String;
begin
  Result := '@INPUT{'+FName+'}'
end;

procedure TIPEqObject.SetVOffset(Value:Integer);
begin
  if Value <> FVOffset then
  begin
    FVOffset := Value;
    Invalidate;
  end;
end;

function TIPEqObject.GetControl:TControl;
begin
  if not Assigned(FControl) and (FName <> '') and not FNoControl then
  begin
    FControl := Document.GetControl(Fname);
    if Assigned(FControl) then
    begin
      FControl.FreeNotification(FNotifyComp);
      if (FControl is TIPEqEditor) then
        (FControl as TIPEqEditor).Font := Font;
    end
    else
      FNoControl := true;
  end;
  Result := FControl;
end;

function TIPEqObject.CalcMetrics:TIPEqMetrics;
var
  TextMetric : TTextMetric;
  W : Integer;
  Em : Integer;
  C : TCOntrol;
begin
  C := GetControl;
  Em := GetEMWidth(Font);
  TextMetric := GetTextMetrics;
  if C = nil then
  begin
    W := GetTextExtent(FName).Cx+2*GetEMPart(FSpaceSize,Em);
    Result := TIPEqMetrics.Create(TextMetric.tmAscent,TextMetric.tmDescent,W,Em);
  end
  else
  begin
    W := C.Width;
    Result := TIPEqMetrics.Create(C.HEight+FVOffset,-FVOffset,W,Em);
  end;
end;

procedure TIPEqObject.Draw(ACanvas:TCanvas);
begin
  if GetControl = nil then
  begin
    ACanvas.Brush := Document.EmptyBoxBrush;
    ACanvas.Pen := Document.EmptyBoxPen;
    ACanvas.Rectangle(0,0,Width,Height);
    ACanvas.Brush.Style := bsClear;
    ACanvas.TextOut(GetEmPart(FSpaceSize),0,FName);
  end;
end;

procedure TIPEqObject.Layout;
begin
end;

procedure TIPEqObject.SetLocation(X,Y:Integer);
var
  C : TControl;
  Pt : TPoint;
  Offset : TPoint;
begin
  inherited SetLocation(X,Y);
  C := GetControl;
  if Assigned(C) then
  begin
    if Assigned(Document.Container) then
    begin
      Offset := Point(0,0);
      if Document.Container is TWInControl then
        C.Parent := Document.Container as TWinControl
      else
      begin
        C.Parent := Document.Container.Parent;
        Offset := Document.Container.ClientToParent(Point(0,0));
      end;

      Pt := GetComponentLocation(0,0);
      Pt.X := Pt.X + Offset.X;
      Pt.Y := Pt.Y + Offset.Y;
      C.SetBounds(Pt.X,Pt.Y,width,height);
      C.Visible := true;
    end;
  end;
end;


procedure TIPEqObject.SetName(Value:String);
begin
  if Value <> FName then
  begin
    FName := Value;
    if Assigned(FControl) then
      FreeAndNil(FCOntrol);
    FNoControl := false;
    Invalidate;
  end;
end;


procedure TIPEqObject.DeleteCharacter(CaretEvent:TIPEqCaretEvent);
var
  Pos1 : Integer;
begin
  Pos1 := Min(CaretEvent.Position,CaretEvent.PositionStart);

  if Pos1 = 0 then
  begin
    Name := '';
    Invalidate;
    CaretEvent.CharacterDeleted := true;
  end;
end;

function TIPEqObject.IsEmpty:boolean;
begin
  Result := FName = '';
end;



end.
