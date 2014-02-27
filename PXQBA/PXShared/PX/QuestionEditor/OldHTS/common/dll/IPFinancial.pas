unit IPFinancial;

interface

uses Variants;

function PV(Rate: Extended; Periods:Integer; Payment:Variant): Extended;

function FillArray(N:Integer; Start:Variant; Step:Double=0): Variant;
function SubArray(A:Variant; Start:Integer; Count:Integer): Variant;

type TDoubleFunctionMethod=function(X:double):double of object;

type TTDXNewtonRamson=class
  private
    FFunc:TDoubleFunctionMethod;
  public
    constructor Create(AFunc:TDoubleFunctionMethod);
    function Solve(Guess1,Step1:double):double;
end;


implementation

uses Math, SysUtils;

function PV(Rate: Extended; Periods:Integer; Payment:Variant): Extended;
var
  A : Extended;
  I : Integer;
begin
  Result := 0;
  A := Payment;
  for I := 1 to Periods do
  begin
    Result := Result + A / Power(1+Rate,I);
  end;
end;


function FillArray(N:Integer; Start:Variant; Step:Double=0): Variant;
var
 I :Integer;
begin
  Result := VarArrayCreate([0, N-1], varVariant);
  for I := 0 to N-1 do
  begin
    if Step > 0 then
      Result[I] := Start + i*Step
    else
      Result[I] := Start;
  end;
end;

function SubArray(A:Variant; Start:Integer; Count:Integer): Variant;
var
 I :Integer;
begin
  if VarIsArray(A) then
  begin
    if Start - 1 >= VarArrayHighBound(A,1) then
      Exception.Create('Start element is outside of array');
    if Count < 1 then
      Count := 1
    else if (Count+Start-1) > VarArrayHighBound(A,1) + 1 then
      Count := VarArrayHighBound(A,1)- Start + 2 ;

    if Count = 1 then
      Result := A[Start-1]
    else
    begin
      Result := VarArrayCreate([0,Count-1],varVariant);
      for I := 0 to Count-1 do
        Result[I] := A[Start+I-1];
    end;
  end
  else
    Exception.Create('First parameter is not an array.');

end;

//function NewtonRamsonNZ(F:TDoubleFunction;guess1,step1:double):double;
{ TTDXNewtonRamson }

constructor TTDXNewtonRamson.Create(AFunc: TDoubleFunctionMethod);
begin
  inherited Create;
  FFunc := AFunc;
end;

const RELAX=0.5;
function TTDXNewtonRamson.Solve(Guess1, Step1: double): double;
var IOld,INew,DeltaF,Fnew,FOld,IStar: double;
    Iter:integer;
begin
  IOld := Guess1;
  if IOld=0 then IOld:=1e-8;
  INew := IOld + Step1;
  if INew=0 then INew:=1e-8;

  FOld := FFunc(IOld);
  FNew := FFunc(INew);

  DeltaF := ABS(FNew);
  Iter :=0;
  while DeltaF > 5e-3 do begin
    IStar := IOld - RELAX*FOld*(IOld-INew)/(FOld-FNew);
    if IStar=0 then IStar:=1e-8;

    IOld := INew;
    INew := IStar;
    FOld := FNew;
    FNew := FFunc(INew);

    DeltaF := ABS(FNew);

    Inc(Iter);
    If Iter=1000 then
      Raise Exception.Create('Newton-Ramson algorithm reached 1000 iterations');
    IF Fold=Fnew then
      Raise Exception.Create('Newton-Ramson algorithm reached a constant function point');
  end;
  Result := INew;
end;

end.
