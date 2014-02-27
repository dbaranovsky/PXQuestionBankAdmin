unit TDXExprSymbolicVar;

interface

uses TDXExpr,Contnrs,Sysutils,Classes;

type


  TTDXExprSymbolicVar = class(TTDXExpr)
  private
    FVarName:String;
    FOnGetValue:TTDXSymVarGetValueEvent;
  protected
    function GetExprValue:TTDXExprValue; override;
    function GetToString:String; override;
  public
    Constructor Create(AVarName:String);
    Destructor Destroy; override;
    function GetEqText(ExprForm:boolean = false):String; override;
    function IsEqual(Expr:TTDXExpr):boolean; override;
    function IsSpecialSymbol:Boolean; //added to handle non-variable symbols like &plusminus;&subset; etc.
    function IsTerm:boolean; override;
    function Clone:TTDXExpr; override;
    property OnGetValue:TTDXSymVarGetValueEvent read FOnGetValue write FOnGetValue;
    property VarName:String read FVarName write FVarName;
  end;

implementation

Constructor TTDXExprSymbolicVar.Create(AVarName:String);
begin
  FVarName := AVarName;
end;

function TTDXExprSymbolicVar.Clone:TTDXExpr;
begin
  Result := TTDXExprSymbolicVar.Create(FVarName);
end;

Destructor TTDXExprSymbolicVar.Destroy;
begin
  inherited Destroy;
end;

function TTDXExprSymbolicVar.GetExprValue:TTDXExprValue;
begin
  if Assigned(FOnGetValue) then
    Result := FOnGetValue(Self,FVarName)
  else
    Result := FVarName;
end;

function TTDXExprSymbolicVar.GetToString:String;
begin
  Result := FVarName;
end;

function TTDXExprSymbolicVar.GetEqText(ExprForm:boolean = false):String;
begin
  Result := FVarName;
end;

function TTDXExprSymbolicVar.IsEqual(Expr:TTDXExpr):boolean;
begin
  if (Expr Is TTDXExprSymbolicVar) then
    Result := SameText(TTDXExprSymbolicVar(Expr).FVarName,FVarName)
  else
    Result := false;
end;

function TTDXExprSymbolicVar.IsTerm:boolean;
begin
  Result := not IsSpecialSymbol;
end;

var SpecialSymbols:TStringList;

function TTDXExprSymbolicVar.IsSpecialSymbol: Boolean;
begin
  Result := False;
  if Pos(';',FVarName) <> 0 then
    Result := SpecialSymbols.IndexOf(FVarName) > -1;
end;

initialization
  SpecialSymbols := TStringList.Create;
  SpecialSymbols.CaseSensitive := False;
  SpecialSymbols.Add('&plusminus;');
  SpecialSymbols.Add('&minusplus;');
  SpecialSymbols.Add('&subset;');
  SpecialSymbols.Add('&propsubset;');
  SpecialSymbols.Add('&notsubset;');
  SpecialSymbols.Add('&notpropsubset;');
  SpecialSymbols.Add('&supset;');
  SpecialSymbols.Add('&propsupset;');
  SpecialSymbols.Add('&notsupset;');
  SpecialSymbols.Add('&notpropsupset;');
  SpecialSymbols.Add('&cap;');
  SpecialSymbols.Add('&cup;');
  SpecialSymbols.Add('&approxequal;');
  SpecialSymbols.Add('&approx;');
  SpecialSymbols.Add('&rightarrow;');
  SpecialSymbols.Add('&leftarrow;');
  SpecialSymbols.Add('&leftrightarrow;');
  SpecialSymbols.Add('&dleftrightarrow;');
  SpecialSymbols.Add('&uparrow;');
  SpecialSymbols.Add('&downarrow;');
  SpecialSymbols.Add('&in;');
  SpecialSymbols.Add('&notin;');
  SpecialSymbols.Add('&empty;');
  SpecialSymbols.Add('&amp;');

finalization
  SpecialSymbols.Free;

end.

