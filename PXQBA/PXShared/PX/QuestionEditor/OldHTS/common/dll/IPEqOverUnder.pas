unit IPEqOverUnder;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes;


type

TIPEqOverUnder = class(TIPEqComposite)
  private
    FType : Integer;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor CreateOverUnder(MainRow,UpRow,DownRow:TIPEqRow); overload;
    constructor CreateOver; overload;
    constructor CreateOver(MainRow,UpRow:TIPEqRow); overload;
    constructor CreateUnder; overload;
    constructor CreateUnder(MainRow,DownRow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
end;

implementation

uses Math,IPEqUtils,StStrL;

constructor TIPEqOverUnder.Create;
begin
  CreateOverUnder(TIPEqRow.Create,TIPEqRow.Create,TIPEqRow.Create);
end;

constructor TIPEqOverUnder.CreateOver;
begin
  CreateOver(TIPEqRow.Create,TIPEqRow.Create);
end;

constructor TIPEqOverUnder.CreateUnder;
begin
  CreateUnder(TIPEqRow.Create,TIPEqRow.Create);
end;

constructor TIPEqOverUnder.CreateOverUnder(MainRow,UpRow,DownRow:TIPEqRow);
begin
  inherited Create;
  FType := 1;
  AddRow(MainRow);
  AddRow(UpRow);
  AddRow(DownRow);
end;

constructor TIPEqOverUnder.CreateOver(MainRow,UpRow:TIPEqRow);
begin
  inherited Create;
  FType := 2;
  AddRow(MainRow);
  AddRow(UpRow);
end;

constructor TIPEqOverUnder.CreateUnder(MainRow,DownRow:TIPEqRow);
begin
  inherited Create;
  FType := 3;
  AddRow(MainRow);
  AddRow(DownRow);
end;

procedure TIPEqOverUnder.BuildMathML(Buffer:TStrings; Level:Integer);
var
  I : Integer;
  FTheText : String;
begin

  if FType=1 then
    FTheText := 'overunder'
  else if FType=2 then
    FTheText := 'over'
  else if FType=3 then
    FTheText := 'under';

  Buffer.Add('<'+FTheText+'>');
   for I := 0 to ChildCount-1 do
     Child[I].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrL(' ',Level)+'</'+FTheText+'>');
end;

function TIPEqOverUnder.GetName:String;
begin
  if FType=1 then
    Result := 'OVERUNDER'
  else if FType=2 then
    Result := 'OVER'
  else if FType=3 then
    Result := 'UNDER'
  else
    Result := '';
end;

function  TIPEqOverUnder.Clone:TIPEqNode;
begin
  Result := TIPEqOverUnder.Create;
  TIPEqOverUnder(Result).CopyChildren(Self);
end;


function TIPEqOverUnder.CalcMetrics:TIPEqMetrics;
var
  MainRow,UpRow,DownRow : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  MainWidth,UpWidth,DownWidth : Integer;
  MainAscent,MainDescent : Integer;
  UpHeight,DownHeight:Integer;
begin
  MainRow := Row[0];
  if (FType=1) then
  begin
    UpRow   := Row[1];
    DownRow := Row[2];
  end
  else if (FType=2) then
  begin
    UpRow := Row[1];
    DownRow := nil;
  end
  else
  begin
    UpRow := nil;
    DownRow := Row[1];
  end;

  Em := GetFontHeight(GetTextMetrics);


  if Assigned(MainRow) then
  begin
    MainAscent  := MainRow.Ascent;
    MainDescent := MainRow.Descent;
    MainWidth := MainRow.Width;
  end
  else
  begin
    MainAscent  := 0;
    MainDescent := 0;
    MainWidth := 0;
  end;

  if Assigned(UpRow) then
  begin
    UpRow.ReduceFontSize;
    UpWidth := UpRow.Width;
    UpHeight := UpRow.Height;
  end
  else
  begin
    UpWidth := 0;
    UpHeight := 0;
  end;
  if Assigned(DownRow) then
  begin
    DownRow.ReduceFontSize;
    DownWidth := DownRow.Width;
    DownHeight := DownRow.Height;
  end
  else
  begin
    DownWidth := 0;
    DownHeight := 0;
  end;

  Ascent := MainAscent+UpHeight+GetEmPart(SP_MINIMUMGAP,Em);
  Descent := MainDescent+DownHeight+GetEmPart(SP_MINIMUMGAP,Em);
  Width := Max(Max(UpWidth,DownWidth),MainWidth);

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);
end;

procedure TIPEqOverUnder.Layout;
var
  MainRow,UpRow,DownRow : TIPEqNode;
  X,Y:Integer;
begin

  MainRow := Row[0];
  if (FType=1) then
  begin
    UpRow   := Row[1];
    DownRow := Row[2];
  end
  else if (FType=2) then
  begin
    UpRow := Row[1];
    DownRow := nil;
  end
  else
  begin
    UpRow := nil;
    DownRow := Row[1];
  end;

  if Assigned(UpRow) then
  begin
    X := (Width-UpRow.Width) div 2;
    Y := 0;
    UpRow.SetLocation(X,Y);
  end;

  if Assigned(MainRow) then
  begin
    X := (Width-MainRow.Width) div 2;
    Y := Ascent-MainRow.Ascent;
    MainRow.SetLocation(X,Y);
  end;

  if Assigned(DownRow) then
  begin
    X := (Width-DownRow.Width) div 2;
    Y := Height-DownRow.Height;
    DownRow.SetLocation(X,Y);
  end;
end;

procedure TIPEqOverUnder.Draw(ACanvas:TCanvas);
begin
end;

end.



