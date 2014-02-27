unit IPEqPlainText;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes,IPEqStack;


type


TIPEqPlainText = class(TIPEqStack)
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas: TCanvas);override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor Create(ARow: TIPEqRow);overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;

end;


implementation

uses IPEqUtils,StStrS;

constructor TIPEqPlainText.Create;
begin
  Create(TIPEqRow.Create);
end;

constructor TIPEqPlainText.Create(ARow:TIPEqRow);
begin
  inherited Create;
  AddRow(ARow);
end;

procedure TIPEqPlainText.BuildMathML(Buffer:TStrings; Level:Integer);
begin
  Buffer.Add('<mtext>');
  Child[0].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrS(' ',Level)+'</mtext>');
end;

function TIPEqPlainText.GetName:String;
begin
  Result := 'T';
end;

function  TIPEqPlainText.Clone:TIPEqNode;
begin
  Result := TIPEqPlainText.Create;
  TIPEqPlainText(Result).CopyChildren(Self);
end;


function TIPEqPlainText.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
begin
  InitFont;
  Font.Style := [];
  Node := Row[0];
  Em := GetFontHeight(GetTextMetrics);
  Descent := Node.Descent;
  Ascent  := Node.Ascent;
  Width := Node.Width;
  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);
end;

procedure TIPEqPlainText.Layout;
var
  Node : TIPEqNode;
begin
  Node := Row[0];
  Node.SetLocation(0,0);
end;

procedure TIPEqPlainText.Draw(ACanvas:TCanvas);
begin
end;

end.



