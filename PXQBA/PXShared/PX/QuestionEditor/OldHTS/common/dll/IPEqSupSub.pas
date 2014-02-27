unit IPEqSupSub;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;


type

TIPEqSupSub = class(TIPEqComposite)
  private
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor Create(MainRow,UpRow,DownRow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
end;

implementation

uses Math,IPEqUtils,StStrL;

constructor TIPEqSupSub.Create;
begin
  Create(TIPEqRow.Create,TIPEqRow.Create,TIPEqRow.Create);
end;

constructor TIPEqSupSub.Create(MainRow,UpRow,DownRow:TIPEqRow);
begin
  inherited Create;
  AddRow(MainRow);
  AddRow(UpRow);
  AddRow(DownRow);
end;

procedure TIPEqSupSub.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
begin
  Buffer.Add('<supsub>');
   for I := 0 to ChildCount-1 do
     Child[I].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrL(' ',Level)+'</supsub>');
end;

function TIPEqSupSub.GetName:String;
begin
  Result := 'SupSub';
end;

function  TIPEqSupSub.Clone:TIPEqNode;
begin
  Result := TIPEqSupSub.Create;
  TIPEqSupSub(Result).CopyChildren(Self);
end;


function TIPEqSupSub.CalcMetrics:TIPEqMetrics;
var
  MainRow,UpRow,DownRow : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  UpWidth,DownWidth : Integer;
  UpHeight : Integer;
  DownDescent:Integer;
begin
  MainRow := Row[0];
  UpRow   := Row[1];
  DownRow := Row[2];

  Em := GetFontHeight(GetTextMetrics);


  if Assigned(MainRow) then
  begin
    Ascent  := MainRow.Ascent;
    Width := MainRow.Width+GetEmPart(3*SP_SUBSUPGAP,Em);
  end
  else
  begin
    Ascent  := 0;
    Width := 0;
  end;

  UpWidth := 0;
  DownWidth := 0;
  UpHeight := 0;
  DownDescent := 0;

  if Assigned(UpRow) then
  begin
    UpRow.ReduceFontSize;
    UpWidth := UpRow.Width;
    UpHeight := UpRow.Height;
  end;
  if Assigned(DownRow) then
  begin
    DownRow.ReduceFontSize;
    DownWidth := DownRow.Width;
    DownDescent := DownRow.Descent;
  end;

  Ascent := UpHeight+2*Ascent div 3;
  if Assigned(MainRow) then
    if MainRow.Ascent > Ascent then
      Ascent := MainRow.Ascent;

  Descent := GetEmPart(SP_SUPSCRIPTDEPTH,Em)+DownDescent;
  if Assigned(MainRow) then
    if MainRow.Descent > Descent then
      Descent := DownDescent + MainRow.Descent;

  Width := Width+Max(UpWidth,DownWidth);
  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);
end;

procedure TIPEqSupSub.Layout;
var
  MainRow,UpRow,DownRow : TIPEqNode;
  X,Y:Integer;
begin

  MainRow := Row[0];
  UpRow   := Row[1];
  DownRow := Row[2];

  if Assigned(MainRow) then
  begin
    X := GetEmPart(SP_SUBSUPGAP);
    Y := Ascent-MainRow.Ascent;
    MainRow.SetLocation(X,Y);
  end;

  if Assigned(UpRow) then
  begin
    X := MainRow.Width+GetEmPart(2*SP_SUBSUPGAP);
    Y := 0;
    UpRow.SetLocation(X,Y);
  end;

  if Assigned(DownRow) then
  begin
    X := MainRow.Width+GetEmPart(2*SP_SUBSUPGAP);
    Y := Height-DownRow.Height;
    DownRow.SetLocation(X,Y);
  end;
end;

procedure TIPEqSupSub.Draw(ACanvas:TCanvas);
begin
end;

end.



