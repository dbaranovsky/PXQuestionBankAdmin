unit IPEqPrime;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqPrimeType = (eqPrime,eqPrime2,eqPrime3);

  TIPEqPrime = class(TIPEqComposite)
  private
    FType : TIPEqPrimeType;
    FSymOffset : Integer;
    function GetSymbolString:String;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create;
    constructor CreatePrime2;
    constructor CreatePrime3;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
  end;

implementation


uses IPEqUtils,Math,Types,ststrL;

constructor TIPEqPrime.Create;
begin
  inherited Create;
  AddRow(TIPEqRow.Create);
  FType := eqPrime;
end;

constructor TIPEqPrime.CreatePrime2;
begin
  Create;
  FType := eqPrime2;
end;

constructor TIPEqPrime.CreatePrime3;
begin
  Create;
  FType := eqPrime3;
end;

procedure TIPEqPrime.BuildMathML(Buffer:TStrings; Level:Integer);
var
  Str : String;
begin
  Case FType of
    eqPrime : Str := 'prime';
    eqPrime2: Str := 'prime2';
    eqPrime3: Str := 'prime3';
  end;
  Buffer.Add(CharStrL(' ',Level)+'<m'+Str+'>');
  Child[0].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrL(' ',Level)+'</m'+Str+'>');
end;

function TIPEqPrime.GetName:String;
begin
  Case FType of
    eqPrime : Result := 'PRIME';
    eqPrime2: Result := 'PRIME2';
    eqPrime3: Result := 'PRIME3';
  end;
end;

function  TIPEqPrime.Clone:TIPEqNode;
begin
  Result := TIPEqPrime.Create;
  (Result as TIPEqPrime).FType := FType;
  TIPEqPrime(Result).CopyChildren(Self);
end;

function TIPEqPrime.GetSymbolString:String;
begin
  Case FType of
    eqPrime : Result := #$A2;
    eqPrime2: Result := #$A2#$A2;
    eqPrime3: Result := #$A2#$A2#$A2;
  end;
end;

function TIPEqPrime.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  TextMetric : TTextMetric;
  Str : String;
  Siz : TSize;
  AFont : TFont;
begin
  Node := Row[0];
  Em := getEMWidth(Font);

  TextMetric := GetTextMetrics;

  Ascent := Node.Ascent;
  Descent := Node.Descent;
  Width := Node.Width;

  Str := GetSymbolString;

  AFont := TFont.Create;
  AFont.Assign(Font);
  AFont.Name := 'Symbol';
  Siz := GetTextExtent(Str,AFont);
  TextMetric := GetTextMetrics(AFont);
  FSymOffset := TextMetric.tmInternalLeading;
  AFont.Free;

  Inc(Width,Siz.Cx);

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);

end;

procedure TIPEqPrime.Layout;
var
  Node : TIPEqNode;
begin
  Node := Row[0];
  if assigned(Node) then
  begin
    Node.SetLocation(0,0);
  end;
end;

procedure TIPEqPrime.Draw(ACanvas:TCanvas);
var
  X : Integer;
  Y : Integer;
  Str : String;
begin
  Str := GetSymbolString;

  X := Row[0].Width;
  Y := 0;
  ACanvas.Font := Font;
  ACanvas.Font.Name := 'Symbol';
  ACanvas.TextOut(X,Y-FSymOffset,Str);
  ACanvas.Font := Font;

end;


end.



