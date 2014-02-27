unit FormEqInfo;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, IPEqEditor,IPEqNode, Outline, Grids;

type
  TfrmEqInfo = class(TForm)
    Outline: TOutline;
  private
    { Private declarations }
    procedure SetEqEditor(Value: TIPEqEditor);
  public
    { Public declarations }
    property EqEditor:TIPEqEditor write SetEqEditor;
  end;

var
  frmEqInfo: TfrmEqInfo;

implementation

{$R *.dfm}

procedure TfrmEqInfo.SetEqEditor(Value: TIPEqEditor);
begin
  Outline.Clear;
  Value.GetOutline(outline.Lines);
  Outline.FullExpand;
end;



end.
