unit IPEqUtils;

interface

uses IPEqNode,Graphics,Windows,IPEqOp;

  type TIPEqClass = Class of TIPEqNode;

  Function IsParen(Ch:Char):Boolean;
  Function IsOpSymbol(Ch:Char; var Op:TIPEqOpType):Boolean;
  Function IsSpace(Ch:Char):Boolean;
  Function IsText(Ch:Char):Boolean;
  Function getEMWidth(Font:TFont):Integer;
  Function GetFontHeight(TextMetric:TTextMetric):Integer;
  Function GetTextSize(Font:TFont; const Txt:String):TSize;
  Function GetTextSizeW(Font:TFont; const Txt:WideString):TSize;
  function NodeHasParent(Node:TIPEQNode; ParentClass:TIPEqClass):Boolean;


implementation

  uses IPEqText,IPEqParen, IPEqChar;

  Function IsParen(Ch:Char):Boolean;
  begin
    //For now, only use ( and ).
    Result := Ch in ['(',')','[',']','{','}'];
  end;

  Function IsSpace(Ch:Char):Boolean;
  begin
    Result := Ch = ' ';
  end;

  Function IsText(Ch:Char):Boolean;
  begin
    Result := Ch in ['a'..'z','A'..'Z','0'..'9','.',',','~','$',#0163,#0128,#0165]
  end;

  Function IsOpSymbol(Ch:Char; var Op:TIPEqOpType):Boolean;
  const
    opChars: array[0..7] of Char = ('+','-','*','/','=','^','>','<');
    opTypes: array[0..7] of TIPEqOpType = (eqoPlus,eqoMinus,eqoMult,eqoDivide,
        eqoEqual,eqoPower,eqoGT,eqoLT);
  var
   I :Integer;
  begin
    for I := 0 to High(opChars) do
    begin
      if Ch = opChars[I] then
      begin
        Result := true;
        Op := opTypes[I];
        exit;
      end;
    end;
    Result := false;
    Op := eqoEmpty;
  end;


  Function GetTextSize(Font:TFont; const Txt:String):TSize;
  var
    DisplayDC: HDC;
    OldFont : HFont;
  begin
    DisplayDC := GetDC(0);
    if (DisplayDC <> 0) then
    begin
      OldFont := SelectObject(DisplayDC,Font.Handle);
      GetTextExtentPoint32(DisplayDC, PChar(Txt), Length(Txt), Result);
      SelectObject(DisplayDC,OldFont);
      ReleaseDC(0, DisplayDC);
    end;
  end;


  Function GetTextSizeW(Font:TFont; const Txt:WideString):TSize;
  var
    DisplayDC: HDC;
    OldFont : HFont;
  begin
    DisplayDC := GetDC(0);
    if (DisplayDC <> 0) then
    begin
      OldFont := SelectObject(DisplayDC,Font.Handle);
      GetTextExtentPoint32W(DisplayDC,PWideChar(Txt), Length(Txt), Result);
      SelectObject(DisplayDC,OldFont);
      ReleaseDC(0, DisplayDC);  
    end;
  end;



 Function getEMWidth(Font:TFont):Integer;
 begin
   Result := GetTextSize(Font,'m').Cx;
 end;


 Function GetFontHeight(TextMetric:TTextMetric):Integer;
 begin
   Result := TextMetric.tmAscent+TextMetric.tmDescent;
 end;

 function NodeHasParent(Node:TIPEQNode; ParentClass:TIPEqClass):Boolean;
 var Nd:TIPEqList;
 begin
   Result := False;
   Nd := Node.Parent;
   while Assigned(Nd) do
   begin
     if Nd is ParentClass then
     begin
       Result := True;
       Break;
     end;
     Nd := Nd.parent;
   end;
 end;


end.
