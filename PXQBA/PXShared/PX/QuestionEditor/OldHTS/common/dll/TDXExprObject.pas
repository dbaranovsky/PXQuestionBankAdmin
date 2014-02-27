unit TDXExprObject;

interface

uses TDXExpr,Contnrs,Sysutils,Classes,Variants;

type

  TTDXExprObject = class(TTDXExpr)
  private
    FObjName:String;
    FPropName:String;
    FArgList : TObjectList;
    FOnObjectGetValue:TTDXObjectGetValueEvent;
    FOnObjectCheckSyntax:TTDXObjectCheckSyntaxEvent;
  protected
    function GetExprValue:TTDXExprValue; override;
    function GetToString:String; override;
    function GetArgCount:Integer;
    function GetArg(Index:Integer):TTDXExpr;
  public
    Constructor Create(AVarName:String);
    Destructor Destroy; override;
    function GetEqText(ExprForm:boolean = false):String; override;
    function Clone:TTDXExpr; override;
    function IsEqual(Expr:TTDXExpr):Boolean;override;
    procedure AddArg(Expr:TTDXExpr);
    procedure CheckSyntax;
    property ObjName: String read FObjName write FObjName;
    property PropName:String read FPropName write FPropName;
    property ArgCount:Integer read GetArgCount;
    property Arg[Index:Integer]:TTDXExpr read GetArg;
    property OnObjectGetValue:TTDXObjectGetValueEvent read FOnObjectGetValue write FOnObjectGetValue;
    property OnObjectCheckSyntax:TTDXObjectCheckSyntaxEvent read FOnObjectCheckSyntax write FOnObjectCheckSyntax;
  end;

implementation

uses StStrL;

Constructor TTDXExprObject.Create(AVarName:String);
var
  PPos : Integer;
begin
  FArgList := TObjectList.Create;
  //Break up var name into object name and propererty name;

  PPos := Pos('.',AVarName);
  if PPos > 0 then
  begin
    FObjName := Copy(AVarName,1,PPos-1);
    FPropName := Copy(AVarName,PPos+1,9999);
  end
  else
  begin
    FObjName := AVarName;
    FPropName := '';
  end;

  if FObjName[1] = '@' then
    Delete(FObjName,1,1);

  if FObjName[1] = '@' then
    FObjName[1] := '$';  //This will be the way we specify old points.
end;

function TTDXExprObject.Clone:TTDXExpr;
begin
  Result := TTDXExprObject.Create(FObjName+'.'+FPropName);
end;

Destructor TTDXExprObject.Destroy;
begin
  inherited Destroy;
  FArgList.Free;
end;

function TTDXExprObject.GetArgCount:Integer;
begin
  Result := FArgList.Count;
end;

function TTDXExprObject.GetArg(Index:Integer):TTDXExpr;
begin
  Result := TTDXExpr(FArgList[Index]);
end;

procedure TTDXExprObject.AddArg(Expr:TTDXExpr);
begin
  FArgList.Add(Expr);
end;


function TTDXExprObject.GetExprValue:TTDXExprValue;
var
  Args : Variant;
  I : Integer;
begin

    if assigned(FOnObjectGetValue) then
    begin
      if ArgCOunt > 0 then
      begin
        Args := VarArrayCreate([0,ArgCount-1],varVariant);
        for I := 0 to ArgCount-1 do
          Args[I] := Arg[I].ExprValue;
      end
      else
        VarClear(Args);
      Result := FOnObjectGetValue(Self,FObjName,FPropName,Args);
    end
    else
    begin
      if FPropName = '' then
         Result := '@' + FObjname
      else
        raise Exception.Create(Format('No event setup for object <%s.%s>',[FObjName,FPropName]));
    end;
end;


function TTDXExprObject.GetEqText(ExprForm:boolean = false):String;
var
 I : Integer;
begin
  Result := '@'+FObjName;
  if FPropName <> '' then
    Result := Result + '.'+FPropName;
  if ArgCount > 0 then
  begin
    Result := Result + '(';
    for I := 0 to ArgCount-1 do
    begin
      if I > 0 then
        Result := Result + ',';
      Result := Result + Arg[I].GetEqText(ExprForm);
    end;
    Result := Result + ')';
  end;
end;



function TTDXExprObject.GetToString:String;
var
 I : Integer;
begin
  Result := '@'+FObjName;
  if FPropName <> '' then
    Result := Result + '.'+FPropName;
  if ArgCount > 0 then
  begin
    Result := Result + '(';
    for I := 0 to ArgCount-1 do
    begin
      if I > 0 then
        Result := Result + ',';
      Result := Result + Arg[I].ToString;
    end;
    Result := Result + ')';
  end;
end;

procedure TTDXExprObject.CheckSyntax;
begin
  if assigned(FOnObjectCheckSyntax) then
  begin
    FOnObjectCheckSyntax(Self,FObjName,FPropName,ArgCount);
  end;
end;

function TTDXExprObject.IsEqual(Expr: TTDXExpr): Boolean;
begin
  Result := False; //Compiler messages cleanup 3/2/05
end;

end.
