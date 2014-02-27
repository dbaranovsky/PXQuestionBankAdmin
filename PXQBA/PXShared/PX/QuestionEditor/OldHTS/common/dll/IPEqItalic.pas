unit IPEqItalic;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes,IPEqStack;


type


TIPEqItalic = class(TIPEqStack)
  private
    FStyle : TFontStyle;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor Create(ARow:TIPEqRow); overload;
    constructor Create(AStyle:TFontStyle); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    property FontStyle:TFontStyle read FStyle write FStyle;
end;


implementation

uses IPEqUtils,StStrS;

constructor TIPEqItalic.Create;
begin
  Create(TIPEqRow.Create);
end;

constructor TIPEqItalic.Create(AStyle:TFontStyle);
begin
  Create(TIPEqRow.Create);
  FStyle := AStyle;
end;


constructor TIPEqItalic.Create(ARow:TIPEqRow);
begin
  FStyle := fsItalic;
  inherited Create;
  AddRow(ARow);
end;

procedure TIPEqItalic.BuildMathML(Buffer:TStrings; Level:Integer);
const
  tag : array[TFontStyle] of string = ('b','i','u','so');
begin
  Buffer.Add('<'+tag[FStyle]+'>');
  Child[0].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrS(' ',Level)+'</'+tag[FStyle]+'>');
end;

function TIPEqItalic.GetName:String;
const
  Tag : array[TFontStyle] of string = ('b','i','u','so');
begin
  Result := Tag[FStyle];
end;

function  TIPEqItalic.Clone:TIPEqNode;
begin
  Result := TIPEqItalic.Create;
  TIPEqItalic(Result).FontStyle := FStyle;
  TIPEqItalic(Result).CopyChildren(Self);
end;


function TIPEqItalic.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
begin
  InitFont;
  Font.Style := Font.Style + [FStyle];
  Node := Row[0];
  Em := GetFontHeight(GetTextMetrics);
  Descent := Node.Descent;
  Ascent  := Node.Ascent;
  Width := Node.Width;
  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);
end;

procedure TIPEqItalic.Layout;
var
  Node : TIPEqNode;
begin
  Node := Row[0];
  Node.SetLocation(0,0);
end;

procedure TIPEqItalic.Draw(ACanvas:TCanvas);
begin
end;




end.



