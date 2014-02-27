unit ActiveControlManager;

interface

uses forms,classes,messages,controls,windows;

const
  WM_ACTIVECONTROLCHANGED = WM_USER+284;

type

TActiveControlManager = class(TComponent)
private
  FInitialized : Boolean;
  FListeners : TThreadList;
  procedure Initialize;
  procedure ActiveControlChanged(Sender:TObject);
  function  GetActiveControl:TWinControl;
protected
  procedure Notification(AComponent: TComponent; Operation: TOperation); override;
public
  Constructor Create(AOwner:TComponent); override;
  Destructor Destroy; override;
  procedure AddListener(AForm:TForm);
  procedure RemoveListener(AForm:TForm);
  property ActiveControl:TWinControl read GetActiveControl;
end;

var
  ActiveControlMgr : TActiveControlManager;

implementation

Constructor TActiveControlManager.Create(AOwner:TComponent);
begin
  inherited Create(AOwner);
  FListeners := TThreadList.Create;
end;

Destructor TActiveControlManager.Destroy;
begin
  inherited Destroy;
  FListeners.Free;
end;

procedure TActiveControlManager.Initialize;
begin
  if not FInitialized then
  begin
    Screen.OnActiveControlChange := ActiveControlChanged;
    FInitialized := True;
  end;
end;

function  TActiveControlManager.GetActiveControl:TWinControl;
begin
  Result := Screen.ActiveControl;
end;

procedure TActiveControlManager.Notification(AComponent: TComponent;
  Operation: TOperation);
begin
  inherited Notification(AComponent,Operation);
  if (Operation = opRemove) and (AComponent is TForm) then
  begin
    RemoveListener(TForm(AComponent));
  end;
end;

procedure TActiveControlManager.RemoveListener(AForm:TForm);
var
  list : TList;
begin
  list := FListeners.LockList;
  try
    list.Remove(AForm);
  finally
    FListeners.UnlockList;
  end;
end;

procedure TActiveControlManager.AddListener(AForm:TForm);
var
  list : TList;
begin

  //Initialize on add this way, we won't overwrite the Screen.OnActiveControlChanged if we just
  //include this unit.  We actaully have to call AddListener to mess with it
  Initialize;

  list := FListeners.LockList;
  try
    list.Add(AForm);
  finally
    FListeners.UnlockList;
  end;
  AForm.FreeNotification(Self);
end;

procedure TActiveControlManager.ActiveControlChanged(Sender:TObject);
var
  list : TList;
  i : Integer;
  f : TForm;
begin
  list := FListeners.LockList;
  try
    for i := 0 to list.Count-1 do
    begin
      f := TForm(list[i]);
      if not (csDestroying in f.ComponentState) then
        if f.HandleAllocated then
           PostMessage(f.Handle,WM_ACTIVECONTROLCHANGED,0,0);
    end;
  finally
    FListeners.UnlockList;
  end;
end;


initialization
  ActiveControlMgr := TActiveControlManager.Create(nil);
finalization
  ActiveControlMgr.Free;
  ActiveControlMgr := nil;
end.
