{$A+,B-,C+,D+,E-,F-,G+,H+,I+,J-,K-,L+,M-,N+,O+,P+,Q-,R-,S-,T-,U-,V+,W-,X+,Y+,Z1}
{$MINSTACKSIZE $00004000}
{$MAXSTACKSIZE $00100000}
{$IMAGEBASE $00400000}
{$APPTYPE GUI}
unit IPEqText;

interface

uses IPEqNode,Graphics,Windows,IPEqTextParser,Classes,IPEqSuperScript;

const
  TEXT_NUMBERSPACE = 0;
  TEXT_FUNCTIONSPACE = 25;
  TEXT_ITALICOVERHANG:double = 0.5;
  TEXT_ITALICLEFT:double = 1.5/8;
  TEXT_FITALICLEFT:double = 1/10;
  TEXT_FRIGHT:double = 2/12;
  TEXT_FITALICRIGHT:double = 2/10;


type

TIPEqText = class(TIPEqNode)
  private
    FText : String;
    FTType : TIPEqToken;
    FOffset : Integer;
    FVOffset : Integer;
    FOverhang : Integer;
    procedure SetText(Value:String);
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Draw(ACanvas:TCanvas); override;
    procedure Layout; override;
    function GetLeftOffset:integer; override;
  public
    constructor Create(const Txt:String);
    constructor CreateType(const Txt:String; TextType:TIPEqToken);
    function InsertNode(Node:array of TIPEqNode; Position:Integer):TIPEqNode; override;
    function ToString:String; override;
    function GetLastCaretPosition:Integer; override;
    function GetCaretLocation(Pos:Integer):TRect; override;
    function GetCaretPositionAt(X,Y:Integer):Integer; override;
    procedure DeleteCharacter(CaretEvent:TIPEqCaretEvent); override;
    function isEmpty:boolean; override;
    function GetSelectedRect(Pos1,Pos2:Integer):TRect; override;
    procedure SplitNode(Pos:Integer); override;
    procedure MergeText(TextNode : TIPEqText);
    procedure ParseText(Parser:TIPEqTextParser); virtual;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function GetText:String; override;
    function IsInteger:boolean; override;
    function IsNumber:boolean; override;

    property Text:String read FText write SetText;
    property TType:TIPEqToken read FTType write FTType;
end;

Function IsDescending(Text:String):boolean;

implementation

uses types, sysutils,Math, IPEqUtils,StrUtils,IPEqPlainText;


Function IsDescending(Text:String):boolean;
var
 I : Integer;
begin
  Result := false;
  for I := 1 to Length(Text) do
  begin
    if (Text[I] < '0') or (Text[I] > 'z') or (Text[I] in ['g','j','p','q','y','Q']) then
    begin
      Result := true;
      Exit;
    end;
  end;
end;


constructor TIPEqText.Create(const Txt:String);
begin
  CreateType(Txt,ttText);
end;

constructor TIPEqText.CreateType(const Txt:String; TextType:TIPEqToken);
begin
  inherited Create;
  FText := Txt;
  FTType := TextType;
end;

function  TIPEqText.Clone:TIPEqNode;
begin
  Result := TIPEqText.Create(Text);
end;

function TIPEqText.IsInteger:boolean;
begin
 Result := TType = ttInteger;
end;

function TIPEqText.IsNumber:boolean;
begin
 Result := (TType = ttInteger) or (TType = ttFloat);
end;


procedure TIPEqText.BuildMathML(Buffer:TStrings; Level:Integer);
begin
  case FTType of
    ttFunction : begin
        Buffer.Add('<mo>'+FText+'</mo>');
      end;
    ttInteger,ttFloat: begin
        Buffer.Add('<mn>'+FText+'</mn>');
      end;
    else
      Buffer.Add('<mi>'+FText+'</mi>');
  end;
end;

function TIPEqText.GetText:String;
begin
  if Document.AllowCommaNumbers and
     ((FTType = ttInteger) or (FTType = ttFloat)) and
     ( Pos(',',FText) > 0 ) then
//Using new CNUM construct 9/17/03
//    Result :='NUM("'+FText+'")'
//      Result := '@CNUM{'+AnsiReplaceStr(FText,',','')+'}'
      Result := AnsiReplaceStr(FText,',','.')
  else
    Result := EqEncode(FText);
end;

function TIPEqText.CalcMetrics:TIPEqMetrics;
var
  TextMetric : TTextMetric;
  W : Integer;
  Em : Integer;
  Descent : Integer;
  Idx: Integer;
  MainParent:TIPEqList;
  AddItalicMargins,EdgeLeft,EdgeRight:Boolean;
begin
  InitFont;

  case FTType of
    ttText : if Document.ItalicizeVariables and not NodeHasParent(Self,TIPEqPlainText) then
                  Font.Style := Font.Style + [fsItalic];
    ttInteger,ttFloat : if Document.Enabled then Font.Color := Document.NumberCOlor;
    ttFunction : begin
        //DV 2/27/07 Worth does not want functions in bold.
        //Font.Style := Font.Style + [fsBold];
        if (FText = 'e') or (FText = 'i') then
        begin
          Font.Style := Font.Style + [fsItalic];// -[fsBold]; 10/27/08 Removed -[fsBold]
        end;
    end;
    ttVariable :  if Document.Enabled then Font.Color := Document.VarColor;
  end;

  TextMetric := GetTextMetrics;
  Em := GetEmWidth(Font);
  W := GetTextExtent(FText).Cx;
  case FTType of
    ttFloat,ttInteger: FOffset := GetEMPart(TEXT_NUMBERSPACE,Em);
    ttFunction : FOffset := GetEMPart(TEXT_FUNCTIONSPACE,Em);
    else
      FOffset := 0;
  end;
  Inc(W,2*FOffset);
  FVOffset := TextMetric.tmInternalLeading;

  AddItalicMargins := (fsItalic in Font.Style) and (FTType=ttText);

  Idx := Parent.GetChildIndex(Self);
  MainParent := Parent.Parent;
  EdgeLeft := (Idx=0)
     and ( (Parent is TIPEqDocument)
            or (((Parent is TIPEqSubScript) or (Parent is TIPEqSuperScript)) and (MainParent.getChildIndex(Parent)=0)));
  EdgeRight := (Idx=Parent.ChildCount-1)
     and ( (Parent is TIPEqDocument)
            or (((Parent is TIPEqSubScript) or (Parent is TIPEqSuperScript)) and (MainParent.getChildIndex(Parent)=MainParent.ChildCount-1)));

  if AddItalicMargins then
  begin
     if EdgeLeft and EdgeRight then
     begin
       FOverhang := Max(TextMetric.tmOverhang,Round(TEXT_ITALICOVERHANG*Em));
       Inc(W,FOverhang);
       Inc(FOffset,Round(TEXT_ITALICLEFT*Font.Size));
     end
     else if EdgeLeft then
     begin
       Inc(W,Round(TEXT_ITALICLEFT*Font.Size));
       Inc(FOffset,Round(TEXT_ITALICLEFT*Font.Size));
     end
     else if EdgeRight then
     begin
       FOverhang := Max(TextMetric.tmOverhang,Round(TEXT_ITALICOVERHANG*Em))-Round(TEXT_ITALICLEFT*Font.Size);
       Inc(W,FOverhang);
     end;
  end;

  if (FText[1]='f') and AddItalicMargins and EdgeLeft then
  begin
     Inc(W,Round(TEXT_FITALICLEFT*Font.Size));
     Inc(FOffset,Round(TEXT_FITALICLEFT*Font.Size));
  end;

  if (FText[Length(FText)]='f') and EdgeRight then
  begin
     Inc(W,Round(TEXT_FRIGHT*Font.Size));
     if AddItalicMargins then
       Inc(W,Round(TEXT_FITALICRIGHT*Font.Size));
  end;

  if IsDescending(FText) then
    Descent := TextMetric.tmDescent
  else
    Descent := 0;
  Result := TIPEqMetrics.Create(TextMetric.tmAscent-FVOffset,Descent,W,Em);
end;

procedure TIPEqText.Layout;
begin
end;

procedure TIPEqText.Draw(ACanvas:TCanvas);
begin

  ACanvas.TextOut(FOffset,-FVOffset,FText);
end;

procedure TIPEqText.SetText(Value:String);
begin
  FText := Value;
  Invalidate;
end;

function TIPEqText.InsertNode(Node:array of TIPEqNode; Position:Integer):TIPEqNode;
var
  newTextNode : TIPEqText;
begin
  //If we're here then it wasn't a Text Node.
  if Position = 0 then
    Parent.AddChildrenBefore(Node,Self)
  else if Position >= Length(Text) then
    Parent.AddChildrenAfter(Node,Self)
  else
  begin
    newTextNode := TIPEqText.Create(Copy(Text,1,position));
    Delete(FText,1,position);
    Parent.AddChildrenBefore(Node,Self);
    Parent.AddChildrenBefore([newTextNode],Node[0]);
    Invalidate;
  end;
  Result := Node[0];

end;

function TIPEqText.ToString:String;
begin
  Result := inherited ToString + ': ' + Text +'('+IPEqTextTokenNames[FTType]+')';
end;

function TIPEqText.GetLastCaretPosition:Integer;
begin
  Result := Length(Text);
end;

function TIPEqText.GetCaretLocation(Pos:Integer):TRect;
var
  X,Y : Integer;
  Pt : TPoint;
begin
  if Length(Text) > 0 then
  begin
    if (Pos > 0) and (Pos <= Length(Text)) then
    begin
      X := GetTextExtent(Copy(FText,1,Pos)).Cx;
      Y := 0;
      Pt := GetComponentLocation(X,Y);
      Result := Bounds(Pt.X-1+FOffset,Pt.Y,1,Height);
      Exit;
    end
  end;

  if pos = 0 then
  begin
    Pt := GetComponentLocation;
    Result := Bounds(Pt.X-1,Pt.Y,1,Height);
    Exit;
  end;
  SetRectEmpty(Result);
end;

function TIPEqText.GetSelectedRect(Pos1,Pos2:Integer):TRect;
var
  R : TRect;
  Pt : TPoint;
begin
   UnionRect(R,GetCaretLocation(Pos1),getCaretLocation(Pos2));
   Pt := getComponentLocation();
   Result := R;
end;

function TIPEqText.GetCaretPositionAt(X,Y:Integer):Integer;
var
  Pt : TPoint;
  TextX : Integer;
  OldX : Integer;
  I : Integer;
begin
  if Length(Text) = 0 then
  begin
    Result := 0;
    Exit;
  end;

  Pt := GetComponentLocation;

  X := X-Pt.X-FOffset;

  TextX := 0;
  OldX := 0;
  I := 0;

  if X < 0 then
  begin
    Result := 0;
    Exit;
  end;

  while TextX < x do
  begin
    OldX := TextX;
    Inc(I);
    TextX := GetTextExtent(Copy(FText,0,I)).Cx;
    if I >= Length(FText) then
      break;
  end;

  if (X-OldX) < (TextX-x) then
    Result := I-1
  else
    Result := I;
end;

procedure TIPEqText.DeleteCharacter(CaretEvent:TIPEqCaretEvent);
var
  Pos1 : Integer;
  Pos2 : Integer;
  Len : Integer;
begin
  Pos1 := Min(CaretEvent.Position,CaretEvent.PositionStart);
  Pos2 := Max(CaretEvent.Position,CaretEvent.PositionStart);

  Len := Length(Text);

  if (Pos1 < 0) or (Pos2 > GetLastCaretPosition) then
    Exit;
  if len > 0 then
  begin
    Delete(FText,Pos1+1,Max(Pos2-Pos1,1));
    Invalidate;
    CaretEvent.CharacterDeleted := true;
  end;
end;

function TIPEqText.isEmpty:boolean;
begin
  Result := Length(FText) = 0;
end;

procedure TIPEqText.SplitNode(Pos:Integer);
var
  NewText : TIPEqText;
begin
  if (Pos > 0) and (Pos < Length(FText)) then
  begin
    NewText := TIPEqText.Create(Copy(FText,Pos+1,Length(FText)-Pos+1));
    Text := Copy(FText,1,Pos);
    Parent.AddChildrenAfter([NewText],Self);
  end;
end;

procedure TIPEqText.MergeText(TextNode : TIPEqText);
begin
  Text := Text + TextNode.Text;
end;

procedure TIPEqText.ParseText(Parser:TIPEqTextParser);
var
  CurToken : TIPEqToken;
  CurValue : String;
  NodeList : TList;
  Nodes : TIPEqNodeList;
  I : Integer;
begin
  Parser.Text := Text;
  CurValue := '';
  NodeList := TList.Create;
  try
    while Parser.moreTokens do
    begin
     CurToken := Parser.nextToken;
     if CurToken = ttText then
       CurValue := CurValue + Parser.TokenVal
     else
     begin
       if CurValue <> '' then
       begin
         NodeList.Add(TIPEqText.Create(CurValue));
         CurValue := '';
       end;
       NodeList.Add(TIPEqText.CreateType(Parser.TokenVal,CurToken));
     end;
    end;
  finally
    if CurValue <> '' then
      NodeList.Add(TIPEqText.Create(CurValue));

    {This is tricky.  We'll assume there's always one Node.
     The first Node will always get assigned to the existing Node.
     Any other Nodes will be Added.  This way we don't have to worry
     about freeing ourSelf.}
    if NodeList.Count > 0 then
    begin
      TType := TIPEqText(NodeList[0]).TType;
      Text := TIPEqText(NodeList[0]).Text;

      TIPEqText(NodeList[0]).Free;

      if NodeList.Count > 1 then
      begin
        SetLength(Nodes,NodeList.Count-1);
        for I := 1 to NodeList.Count-1 do
          Nodes[I-1] := NodeList[I];
        Parent.AddChildrenAfter(Nodes,Self);
      end;
    end;
    NodeList.Free;
  end;

end;


function TIPEqText.GetLeftOffset: integer;
begin
  Result := FOverhang;
end;


end.
