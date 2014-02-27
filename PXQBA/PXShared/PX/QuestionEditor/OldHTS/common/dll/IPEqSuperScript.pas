unit IPEqSuperScript;

interface

uses IPEqNode,IPEqComposite,Graphics,Windows,Classes,IPEqStack;


type

TIPEqSuperScript = class(TIPEqStack)
  private
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
  public
    constructor Create; overload;
    constructor Create(ARow:TIPEqRow); overload;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function GetRowBelow(ARow:Integer):TIPEqRow; override;
end;

TIPEqSubScript = class(TIPEqSuperScript)
  private
  protected
    function GetName:String; override;
  public
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function GetRowBelow(ARow:Integer):TIPEqRow; override;
    function GetRowAbove(ARow:Integer):TIPEqRow; override;
end;


implementation

uses IPEqUtils,StStrS;

constructor TIPEqSuperScript.Create;
begin
  Create(TIPEqRow.Create);
end;

constructor TIPEqSuperScript.Create(ARow:TIPEqRow);
begin
  inherited Create;
  AddRow(ARow);
end;

procedure TIPEqSuperScript.BuildMathML(Buffer:TStrings; Level:Integer);
begin
  Buffer.Add('<sup><none/>');
  Child[0].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrS(' ',Level)+'</sup>');
end;

function TIPEqSuperScript.GetName:String;
begin
  Result := 'Sup';
end;

function  TIPEqSuperScript.Clone:TIPEqNode;
begin
  Result := TIPEqSuperScript.Create;
  TIPEqSuperScript(Result).CopyChildren(Self);
end;


function TIPEqSuperScript.CalcMetrics:TIPEqMetrics;
var
  Node : TIPEqNode;
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
begin
  Node := Row[0];
  ReduceFontSize;
  Em := GetFontHeight(GetTextMetrics);
  Descent := Node.Descent;
  Ascent  := Node.Ascent;
  Width := Node.Width+GetEmPart(2*SP_SUBSUPGAP,Em);
  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);
end;

procedure TIPEqSuperScript.Layout;
var
  Node : TIPEqNode;
begin
  Node := Row[0];
  Node.SetLocation(GetEMPart(SP_SUBSUPGAP),0);
end;

procedure TIPEqSuperScript.Draw(ACanvas:TCanvas);
begin
end;

function TIPEqSuperScript.GetRowBelow(ARow:Integer):TIPEqRow;
begin
  Result := GetParentRow;
end;


function  TIPEqSubScript.Clone:TIPEqNode;
begin
  Result := TIPEqSubScript.Create;
  TIPEqSubScript(Result).CopyChildren(Self);
end;

procedure TIPEqSubScript.BuildMathML(Buffer:TStrings; Level:Integer);
begin
  Buffer.Add('<sub><none/>');
  Child[0].BuildMathML(Buffer,Level+1);
  Buffer.Add(CharStrS(' ',Level)+'</sub>');
end;

function TIPEqSubScript.GetName:String;
begin
  Result := 'Sub';
end;

function TIPEqSubScript.GetRowBelow(ARow:Integer):TIPEqRow;
begin
 Result := nil;
end;

function TIPEqSubScript.GetRowAbove(ARow:Integer):TIPEqRow;
begin
  Result := GetParentRow;
end;

end.



