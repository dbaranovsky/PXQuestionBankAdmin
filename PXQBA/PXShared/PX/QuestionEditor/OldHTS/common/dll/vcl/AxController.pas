unit AxController;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs;

type
  TfrmAXController = class(TForm)
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
  private
    { Private declarations }
    FForms : TList;
//    FOnActiveControlChanged:TNotifyEvent;
//    procedure ActiveControlChanged(Sender:TObject);
    function GetFormCount:Integer;
    function GetForm(index:Integer):TCustomForm;
    procedure CMActionUpdate(var Message: TMessage); message CM_ACTIONUPDATE;
    procedure CMActionExecute(var Message: TMessage); message CM_ACTIONEXECUTE;
  protected
    procedure Notification(AComponent: TComponent;
      Operation: TOperation); override;
  public
    { Public declarations }
    procedure RegisterForm(AForm:TCustomForm);
    function UpdateAction(Action: TBasicAction): Boolean; override;
    property FormCount:Integer read GetFormCount;
    property Forms[index:Integer]:TCustomForm read GetForm;
//    property OnActiveControlChanged:TNotifyEvent read FOnActiveCOntrolChanged write FOnActiveControlChanged;
  end;

var
  frmAXController: TfrmAXController;

implementation

{$R *.dfm}

procedure TfrmAXController.FormCreate(Sender: TObject);
begin
//  Screen.OnActiveControlChange := ActiveControlChanged;
  FForms := TList.Create;
//  Application.Handle := Handle;
end;

procedure TfrmAXController.FormDestroy(Sender: TObject);
begin
  FForms.Free;
end;

procedure TfrmAXController.RegisterForm(AForm:TCustomForm);
begin
  if FForms.IndexOf(AForm) < 0 then
  begin
    FForms.Add(AForm);
    AForm.FreeNotification(Self);
  end;
end;

function TfrmAXController.GetFormCount:Integer;
begin
  Result := FForms.Count;
end;

function TfrmAXCOntroller.GetForm(index:Integer):TCustomForm;
begin
  Result := TCustomForm(FForms[index]);
end;

(*
procedure TfrmAXController.ActiveControlChanged(Sender:TObject);
var
  c : TWinControl;
  i : Integer;
  f : TCustomForm;
begin
  if Assigned(FOnActiveCOntrolChanged) then
    FOnActiveControlChanged(Sender);
end;
*)

procedure TfrmAXController.Notification(AComponent: TComponent;
    Operation: TOperation);
begin
  inherited Notification(AComponent, Operation);
  if Operation = opRemove then
  begin
    if AComponent is TCustomForm then
    begin
      FForms.Remove(AComponent);
    end;
  end;
end;

function TfrmAXController.UpdateAction(Action: TBasicAction): Boolean;
begin
  Result := inherited UpdateAction(Action);
//  for i := 0 to FormCount-1 do
 //   Forms[i].UpdateAction(Action);
end;

procedure TfrmAXController.CMActionUpdate(var Message: TMessage);


  function ProcessUpdate(Control: TControl): Boolean;
  begin
    Result := (Control <> nil) and
      Control.UpdateAction(TBasicAction(Message.LParam));
  end;

  function TraverseClients(Container: TWinControl): Boolean;
  var
    I: Integer;
    Control: TControl;
  begin
    if Container.Showing then
      for I := 0 to Container.ControlCount - 1 do
      begin
        Control := Container.Controls[I];
        if Control.Visible and ProcessUpdate(Control) or
          (Control is TWinControl) and TraverseClients(TWinControl(Control)) then
        begin
          Result := True;
          Exit;
        end;
      end;
    Result := False;
  end;
var
 i : integer;
begin
  if (csDesigning in ComponentState) {or not Showing} then Exit;
  { Find a target for given Command (Message.LParam). }
  if ProcessUpdate(ActiveControl) or ProcessUpdate(Self) or
    TraverseClients(Self) then
  begin
    Message.Result := 1;
    exit;
  end;

  for i := 0 to FormCount-1 do
  begin
    if TraverseClients(Forms[i]) then
    begin
      Message.Result := 1;
      break;
    end;
  end;
end;

procedure TfrmAXCOntroller.CMActionExecute(var Message: TMessage);

  function ProcessExecute(Control: TControl): Boolean;
  begin
    Result := (Control <> nil) and
      Control.ExecuteAction(TBasicAction(Message.LParam));
  end;

  function TraverseClients(Container: TWinControl): Boolean;
  var
    I: Integer;
    Control: TControl;
  begin
    if Container.Showing then
      for I := 0 to Container.ControlCount - 1 do
      begin
        Control := Container.Controls[I];
        if Control.Visible and ProcessExecute(Control) or
          (Control is TWinControl) and TraverseClients(TWinControl(Control)) then
        begin
          Result := True;
          Exit;
        end;
      end;
    Result := False;
  end;

var
 i : Integer;

begin
  if (csDesigning in ComponentState) {or not Showing} then Exit;
  { Find a target for given Command (Message.LParam). }
  if ProcessExecute(ActiveControl) or ProcessExecute(Self) or
    TraverseClients(Self) then
  begin
    Message.Result := 1;
    exit;
  end;

  for i := 0 to FormCount-1 do
  begin
    if TraverseClients(Forms[i]) then
    begin
      Message.Result := 1;
      break;
    end;
  end;

end;

end.
