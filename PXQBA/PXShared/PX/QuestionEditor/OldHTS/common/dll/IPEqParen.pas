unit IPEqParen;

interface

uses IPEqNode,IPEqChar,Graphics;

const
  OpenParens: String = '({[';
  CloseParens: String= ')}]';

  PAREN_GAP = 10;
  PAREN_THICKFONT = 13;


type

  TIPEqParen = class(TIPEqChar)
    private
      FContentAscent : Integer;
      FContentDescent : Integer;
      FParenWidth : Integer;
      FBaseHeight : Integer;
      FMatchedParen : TIPEqParen;
      FVOffset : Integer;
      procedure PaintParen(ACanvas:TCanvas);
      procedure PaintBracket(ACanvas:TCanvas);
      procedure PaintCBrace(ACanvas:TCanvas);
      procedure SetContentAscent(Value:Integer);
      procedure SetContentDescent(Value:Integer);
    protected
      function CalcMetrics:TIPEqMetrics; override;
      procedure Draw(ACanvas:TCanvas); override;
      procedure Layout; override;
    public
      constructor Create(Ch:Char);
      function CanClose(ACloseParen: TIPEqParen):boolean;
      function IsOpenParen:Boolean;
      function IsCloseParen:Boolean;
      function  Clone:TIPEqNode; override;
      function  CheckParens:boolean; override;
      function GetText:String; override;
      property MatchedParen : TIPEqParen read FMatchedParen write FMatchedParen;
      property ContentAscent:Integer read FContentAscent write SetContentAscent;
      property ContentDescent:Integer read FContentDescent write SetContentDescent;
  end;

function GetEqParenType(Txt:String; var ParenChar:Char):boolean;

implementation

uses windows,Math,IPEqUtils,SysUtils;

function GetEqParenType(Txt:String; var ParenChar:Char):boolean;
begin
  ParenChar := #0;
  if SameText(Txt,'&lbracket;') then
    ParenChar := '['
  else if SameText(Txt,'&rbracket;') then
    ParenChar := ']'
  else if SameText(Txt,'&lbrace;') then
    ParenChar := '{'
  else if SameText(Txt,'&rbrace;') then
    ParenChar := '}'
  else if SameText(Txt,'(') then
    ParenChar := '('
  else if SameText(Txt,'}') then
    ParenChar := ')';

  Result := ParenChar <> #0;
end;

constructor TIPEqParen.Create(Ch:Char);
begin
  inherited Create(Ch);
end;

function  TIPEqParen.Clone:TIPEqNode;
begin
  Result := TIPEqParen.Create(Character);
end;


function TIPEqParen.CanClose(ACloseParen: TIPEqParen):boolean;
begin
  Result := Pos(Character,OpenParens) = Pos(ACloseParen.Character,CloseParens);
end;

function  TIPEqParen.CheckParens:boolean;
begin
  Result := Assigned(MatchedParen) or not Document.InterpretFunctions;
end;

function TIPEqParen.CalcMetrics:TIPEqMetrics;
var
  TextMetric : TTextMetric;
  BaseAscent : Integer;
  BaseDescent : Integer;
  Ascent,Descent,Em : Integer;
begin
  TextMetric := GetTextMetrics;
  FVOffset := TextMetric.tmInternalLeading;
  BaseAscent := TextMetric.tmAscent-FVOffset;
  BaseDescent := TextMetric.tmDescent;
  FBaseHeight := BaseAscent+BaseDescent;
  FParenWidth := GetTextExtent(Character).Cx;
  Em := GetEMWidth(Font);
  if (contentAscent > 0) and (contentDescent > 0) then
  begin
    Ascent := contentAscent;
    Descent := contentDescent;
  end
  else
  begin
    Ascent := BaseAscent;
    Descent := BaseDescent;
  end;
  Result := TIPEqMetrics.Create(Ascent,Descent,FParenWidth+2*GetEmPart(PAREN_GAP,Em),Em);
end;

procedure TIPEqParen.Draw(ACanvas:TCanvas);
var
  PenRecall : TPenRecall;
  FontRecall : TFontRecall;
  Size : Integer;
begin
  PenRecall := TPenRecall.Create(ACanvas.Pen);
  FontRecall := TFontRecall.Create(ACanvas.Font);
  try
    if Assigned(MatchedParen) or (not Document.InterpretFunctions) then
    begin
    end
    else
    begin
      if Document.Enabled then
      begin
        ACanvas.Pen.Color := Document.UnMatchedParenColor;
        ACanvas.Font.Color := Document.UnMatchedParenColor;
      end;
    end;

    Size := ContentAscent + ContentDescent;

    if Size > Round(1.33*FBaseHeight) then
    begin
      case Character of
        '(',')' : PaintParen(ACanvas);
        '[',']' : PaintBracket(ACanvas);
        '{','}' : PaintCBrace(ACanvas);
        else
          ACanvas.TextOut(GetEMPart(PAREN_GAP),-FVOffset,Character);
      end;
    end
    else
      ACanvas.TextOut(GetEMPart(PAREN_GAP),-FVOffset,Character);
  finally
    PenRecall.Free;
    FontRecall.Free;
  end;
end;

procedure TIPEqParen.Layout;
begin
end;

procedure TIPEqParen.PaintParen(ACanvas:TCanvas);
var
  h : Integer;
  XOff : Integer;
begin
  h := Height;
  XOff := GetEmPart(PAREN_GAP);

  if Character = '(' then
  begin
    ACanvas.Arc(XOff,0,2*(XOff+FParenWidth)+1,2*FParenWidth+1,XOff+FParenWidth,0,XOff,FParenWidth);
    ACanvas.Arc(XOff,h-2*FParenWidth,2*(XOff+FParenWidth)+1,h+1,XOff,h-FParenWidth,XOff+FParenWidth,h);
    ACanvas.MoveTo(XOff,FParenWidth-1);
    ACanvas.LineTo(XOff,h-FParenWidth);
    if Font.Size > PAREN_THICKFONT then
    begin
      ACanvas.MoveTo(XOff+1,FParenWidth-1);
      ACanvas.LineTo(XOff+1,h-FParenWidth);
      ACanvas.Arc(XOff+1,0,2*(XOff+FParenWidth),2*FParenWidth+1,XOff+FParenWidth,0,XOff,FParenWidth);
      ACanvas.Arc(XOff+1,h-2*FParenWidth,2*(XOff+FParenWidth),h+1,XOff,h-FParenWidth,XOff+FParenWidth,h);
    end;
  end
  else
  begin
    ACanvas.Arc(XOff-FParenWidth,0,XOff+FParenWidth+1,2*FParenWidth+1,XOff+FParenWidth,FParenWidth,XOff,0);
    ACanvas.Arc(XOff-FParenWidth,h-2*FParenWidth,XOff+FParenWidth+1,h+1,XOff,h,XOff+FParenWidth,h-FParenWidth);
    ACanvas.MoveTo(XOff+FParenWidth,FParenWidth);
    ACanvas.LineTo(XOff+FParenWidth,h-FParenWidth+1);
    if Font.Size > PAREN_THICKFONT then
    begin
      ACanvas.MoveTo(XOff+FParenWidth-1,FParenWidth);
      ACanvas.LineTo(XOff+FParenWidth-1,h-FParenWidth+1);
      ACanvas.Arc(XOff-FParenWidth+1,0,XOff+FParenWidth,2*FParenWidth+1,XOff+FParenWidth-1,FParenWidth,XOff,0);
      ACanvas.Arc(XOff-FParenWidth+1,h-2*FParenWidth,XOff+FParenWidth,h+1,XOff,h,XOff+FParenWidth-1,h-FParenWidth);
    end;
  end;
end;

procedure TIPEqParen.PaintBracket(ACanvas:TCanvas);
var
  H : Integer;
  XOff : Integer;
  Pts : array [0..3] of TPoint;
begin
  H := Height-1;
  XOff := GetEmPart(PAREN_GAP);

  if Character = '[' then
  begin
    Pts[0].X := XOff+FParenWidth;
    Pts[0].Y := 0;
    Pts[1].X := XOff;
    Pts[1].Y := 0;
    Pts[2].X := XOff;
    Pts[2].Y := H;
    Pts[3].X := Pts[0].X;
    Pts[3].Y := H;
  end
  else
  begin
    Pts[0].X := XOff;
    Pts[0].Y := 0;
    Pts[1].X := XOff+FParenWidth;
    Pts[1].Y := 0;
    Pts[2].X := Pts[1].X;
    Pts[2].Y := H;
    Pts[3].X := Pts[0].X;
    Pts[3].Y := H;
  end;

  ACanvas.PolyLine(Pts);

end;

procedure TIPEqParen.PaintCBrace(ACanvas:TCanvas);
var
  H : Integer;
  XOff : Integer;
  P2 : Integer;
  Pts : array [0..6] of TPoint;
begin
  h := Height;
  XOff := GetEmPart(PAREN_GAP);
  P2 := Max(2,FParenWidth div 2);

  if Character = '{' then
  begin
    Pts[0].X := XOff+FParenWidth;
    Pts[0].Y := 0;
    Pts[1].X := XOff+FParenWidth-P2;
    Pts[1].Y := P2;
    Pts[2].X := Pts[1].X;
    Pts[2].Y := H div 2 -P2;
    Pts[3].X := XOff;
    Pts[3].Y := H div 2;
    Pts[4].X := Pts[1].X;
    Pts[4].Y := Pts[3].Y+P2;
    Pts[5].X := Pts[1].X;
    Pts[5].Y := H - P2;
    Pts[6].X := Pts[0].X;
    Pts[6].Y := H;
  end
  else
  begin
    Pts[0].X := XOff;
    Pts[0].Y := 0;
    Pts[1].X := XOff+FParenWidth-P2;
    Pts[1].Y := P2;
    Pts[2].X := Pts[1].X;
    Pts[2].Y := H div 2 - P2;
    Pts[3].X := XOff+FParenWidth;
    Pts[3].Y := H div 2;
    Pts[4].X := Pts[1].X;
    Pts[4].Y := Pts[3].Y+P2;
    Pts[5].X := Pts[1].X;
    Pts[5].Y := H - P2;
    Pts[6].X := Pts[0].X;
    Pts[6].Y := H;
  end;

  ACanvas.PolyLine(Pts);

end;

procedure TIPEqParen.SetContentAscent(Value:Integer);
begin
  if Value <> FContentAscent then
  begin
    FContentAscent := Value;
    Invalidate;
  end;
end;

procedure TIPEqParen.SetContentDescent(Value:Integer);
begin
  if Value <> FContentDescent then
  begin
    FContentDescent := Value;
    Invalidate;
  end;
end;

function TIPEqParen.IsOpenParen:Boolean;
begin
  Result := Pos(Character,OpenParens) > 0;
end;

function TIPEqParen.IsCloseParen:Boolean;
begin
  Result := Pos(Character,CloseParens) > 0;
end;

function TIPEqParen.GetText:String;
begin
  case Character of
  '[' : Result := '&lbracket;';
  ']' : Result := '&rbracket;';
  '{' : Result := '&lbrace;';
  '}' : Result := '&rbrace;';
  else
    Result := inherited GetText;
  end;

end;

end.
