unit TDXExprModel;

interface

uses TDXExpr,Contnrs,Sysutils,Classes,Variants;

type

  TTDXExprModel = class(TTDXExpr)
  private
    FModelName:String;
    FArgList : TObjectList;
  protected
    function GetExprValue:TTDXExprValue; override;
    function GetToString:String; override;
    function GetArgCount:Integer;
    function GetArg(Index:Integer):TTDXExpr;
  public
    Constructor Create(AModelName:String);
    Destructor Destroy; override;
    function GetEqText(ExprForm:boolean = false):String; override;
    function Clone:TTDXExpr; override;
    function IsEqual(Expr:TTDXExpr):Boolean;override;
    procedure AddArg(Expr:TTDXExpr);
    procedure CheckSyntax;
    property ModelName: String read FModelName write FModelName;
    property ArgCount:Integer read GetArgCount;
    property Arg[Index:Integer]:TTDXExpr read GetArg;
  end;

implementation

uses StStrL;

Constructor TTDXExprModel.Create(AModelName:String);
begin
  FARgList := TObjectList.Create;
  FModelName := AModelName;
end;

function TTDXExprModel.Clone:TTDXExpr;
begin
  Result := TTDXExprModel.Create(FModelName);
end;

Destructor TTDXExprModel.Destroy;
begin
  inherited Destroy;
  FArgList.Free;
end;

function TTDXExprModel.GetArgCount:Integer;
begin
  Result := FArgList.Count;
end;

function TTDXExprModel.GetArg(Index:Integer):TTDXExpr;
begin
  Result := TTDXExpr(FArgList[Index]);
end;

procedure TTDXExprModel.AddArg(Expr:TTDXExpr);
begin
  FArgList.Add(Expr);
end;


function TTDXExprModel.GetExprValue:TTDXExprValue;
var
  Args : Variant;
  I : Integer;
begin
  if ArgCOunt > 0 then
  begin
    Args := VarArrayCreate([0,ArgCount-1],varVariant);
    for I := 0 to ArgCount-1 do
      Args[I] := Arg[I].ExprValue;
  end
  else
    VarClear(Args);

  Result := Unassigned;
end;


function TTDXExprModel.GetEqText(ExprForm:boolean = false):String;
var
 I : Integer;
begin
  Result := FModelName;
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



function TTDXExprModel.GetToString:String;
var
 I : Integer;
begin
  Result := FModelName;
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

procedure TTDXExprModel.CheckSyntax;
begin
end;

function TTDXExprModel.IsEqual(Expr: TTDXExpr): Boolean;
begin
  Result := False; //Compiler messages cleanup 3/2/05
end;

end.
