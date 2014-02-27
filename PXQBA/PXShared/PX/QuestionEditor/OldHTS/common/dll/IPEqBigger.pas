unit IPEqBigger;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;

type

TIPEqBigger = class(TIPEqComposite)
  private
    FPoints : integer;
    procedure SetPoints(Value:integer);
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    function GetName:String; override;
    procedure Draw(ACanvas:TCanvas); override;
  public
    constructor Create; overload;
    constructor Create(ARow:TIPEqRow); overload;
    constructor Create(Pts:Integer); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    procedure SetPointsFromString(strPoints:String);
    function GetText:String; override;

    property Points:Integer read FPoints write SetPoints;

end;

implementation

uses IPEqUtils,Math,Types,ststrL,sysutils;

constructor TIPEqBigger.Create;
begin
  Create(TIPEqRow.Create);
end;

constructor TIPEqBigger.Create(Pts:Integer);
begin
  Create(TIPEqRow.Create);
  FPoints := Pts;
end;

constructor TIPEqBigger.Create(ARow:TIPEqRow);
begin
  inherited Create;
  FPoints := 1;
  AddRow(ARow);
end;

procedure TIPEqBigger.BuildMathML(Buffer:TStrings; Level:Integer);
var
  Str : String;
begin
  Str := '<size';
  if FPoints <> 1 then
    Str := Str + ' points="'+IntToStr(Fpoints)+'"';
  Str := Str + '>';
  Buffer.Add(CharStrL(' ',Level)+Str);
  Child[0].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrL(' ',Level)+'</size>');
end;

function TIPEqBigger.GetName:String;
begin
  Result := 'SIZE';
end;

procedure TIPEqBigger.Draw(ACanvas:TCanvas);
begin
end;

function  TIPEqBigger.Clone:TIPEqNode;
begin
  Result := TIPEqBigger.Create;
  TIPEqBigger(Result).CopyChildren(Self);
end;

function TIPEqBigger.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
begin
  IncreaseFontSize(FPoints);
  Node := Row[0];
  Em := GetEMWidth(Font);
  Descent := Node.Descent;
  Ascent := Node.Ascent;
  Width := Node.Width;
  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);
end;

procedure TIPEqBigger.Layout;
begin
end;

procedure TIPEqBigger.SetPoints(Value:integer);
begin
  if Value <> FPoints then
  begin
    FPoints := Value;
    InvalidateAll;
  end;
end;

procedure TIPEqBigger.SetPointsFromString(StrPoints:String);
begin
  try
    Points := StrToInt(StrPoints);
  except
    raise Exception.create('Invalid parameter for number of points in @SIZE.');
  end;

end;

function TIPEqBigger.GetText:String;
begin
    Result := '@'+GetName + '{' + Row[0].Text + ';' + IntToStr(FPoints) + '}';
end;


end.
