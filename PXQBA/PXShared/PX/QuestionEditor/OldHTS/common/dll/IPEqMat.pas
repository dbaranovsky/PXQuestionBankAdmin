unit IPEqMat;

interface

uses IPEqNode,IPEqComposite,IPEqStack,Graphics,Windows,Classes;


type

TIPEqMat = class(TIPEqStack)
  private
    FType : Integer;
    FRowIndex : TList;
//    ColCount,RowCount : Integer;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Layout; override;
    procedure Draw(ACanvas:TCanvas); override;
    function GetName:String; override;
    function GetMatColCount:Integer;
    function GetMatRowCount:Integer;
    function GetMatEqRow(R,C:Integer):TIPEqRow;
    function GetMaxColEq(TheCol : Integer) : Integer;
    function GetMaxColWidth(TheCol,TheMargin : Integer) :Integer;
    function GetMaxRowAscent(TheRow,TheMargin : Integer) :Integer;
    function GetMaxRowDescent(TheRow,TheMargin : Integer) :Integer;
    function GetMaxRowHeight(TheRow,TheMargin : Integer) :Integer;
    function GetPreviousColWidth(TheCol,TheMargin : Integer) :Integer;
    function GetPreviousRowAscent(TheRow,TheMargin : Integer) :Integer;
    function GetPreviousRowDescent(TheRow,TheMargin : Integer) :Integer;
  public
    constructor Create; overload;
    constructor CreateL; overload;
    constructor CreateR; overload;
    constructor CreateAug; overload;
    constructor CreateAugL; overload;
    constructor CreateAugR; overload;
    constructor CreateRow; overload;
    constructor CreateRowL; overload;
    constructor CreateRowR; overload;
    constructor CreateColumn; overload;
    constructor CreateColumnL; overload;
    constructor CreateColumnR; overload;
    constructor CreateTab; overload;
    constructor CreateTabL; overload;
    constructor CreateTabR; overload;
    constructor CreateTable; overload;
    constructor CreateTableL; overload;
    constructor CreateTableR; overload;
    constructor CreateN(Row,Col,TheType:Integer); overload;
    destructor Destroy; override;
    function  Clone:TIPEqNode; override;
    procedure BuildMathML(Buffer:TStrings; level:Integer); override;
    function AddEqRow(NewRow:boolean):TIPEqRow;
    function GetText:String; override;
    function getRowAbove(ARow:Integer):TIPEqRow; override;
    function getRowBelow(ARow:Integer):TIPEqRow; override;

    property MatColCount:Integer read GetMatColCount;
    property MatRowCount:Integer read GetMatRowCount;
    property MatEqRow[R,C:Integer]:TIPEqRow read GetMatEqRow;
end;

const
  MAT_INTERNALMARGIN = 30;

implementation

uses Math,IPEqUtils,StStrL;

constructor TIPEqMat.Create;
begin
  FType := 1;
  CreateN(3,2,FType);
end;


constructor TIPEqMat.CreateL;
begin
  FType := 2;
  CreateN(3,2,FType);
end;


constructor TIPEqMat.CreateR;
begin
  FType := 3;
  CreateN(3,2,FType);
end;


constructor TIPEqMat.CreateAug;
begin
  FType := 4;
  CreateN(3,2,FType);
end;


constructor TIPEqMat.CreateAugL;
begin
  FType := 5;
  CreateN(3,2,FType);
end;


constructor TIPEqMat.CreateAugR;
begin
  FType := 6;
  CreateN(3,2,FType);
end;


constructor TIPEqMat.CreateRow;
begin
  FType := 7;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateRowL;
begin
  FType := 8;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateRowR;
begin
  FType := 9;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateColumn;
begin
  FType := 10;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateColumnL;
begin
  FType := 11;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateColumnR;
begin
  FType := 12;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateTab;
begin
  FType := 13;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateTabL;
begin
  FType := 14;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateTabR;
begin
  FType := 15;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateTable;
begin
  FType := 16;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateTableL;
begin
  FType := 17;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateTableR;
begin
  FType := 18;
  CreateN(3,3,FType);
end;


constructor TIPEqMat.CreateN(Row,Col,TheType:Integer);
var
  I, J : Integer;
  Index : Integer;
begin
  FType := TheType;
  inherited Create;
  FRowIndeX := TList.Create;
   for I := 0 to Row-1 do
   begin
     IndeX := I*Col;
     FRowIndex.Add(Pointer(Index));
     for J := 0 to Col-1 do
       AddRow(TIPEqRow.Create);
   end;
end;

destructor TIPEqMat.Destroy;
begin
  inherited Destroy;
  FRowIndex.Free;
end;

function TIPEqMat.AddEqRow(NewRow:boolean):TIPEqRow;
begin
  AddRow(TIPEqRow.Create);
  if NewRow Then
    FRowIndex.Add(Pointer(ChildCount-1));
  Result := Row[ChildCount-1];
end;

function TIPEqMat.GetMatRowCount:Integer;
begin
  Result := FRowIndex.Count;
end;

function TIPEqMat.GetMatColCount:Integer;
var
 I : Integer;
 Count : Integer;
 Idx : Integer;
begin
  Count := 0;
  for i := 0 to FRowIndex.Count-1 do
  begin
    if i < FRowIndex.Count-1 Then
      IdX := Integer(FRowIndex[i+1])
    else
      IdX := ChildCount;
    Count := Max(Count,Idx-Integer(FRowIndex[I]));
  end;
  Result := Count;
end;

function TIPEqMat.GetMatEqRow(R,C:Integer):TIPEqRow;
var
 Index : Integer;
begin
  Index := Integer(FRowIndex[R])+C;
  if Index > ChildCount-1 Then
    Result := nil
  else if (R = FRowIndex.Count-1) and (Index < ChildCount) Then
    Result := Row[Index]
  else if Index >= Integer(FRowIndex[R+1]) Then
    Result := nil
  else
    Result := Row[Index];
end;

procedure TIPEqMat.BuildMathML(Buffer:TStrings; level:Integer);
var
  I : Integer;
  FTheText : String;
begin

  if FType=1 Then
    FTheText := 'Mat'
  else if FType=2 Then
    FTheText := 'MatL'
  else if FType=3 Then
    FTheText := 'MatR'
  else if FType=4 Then
    FTheText := 'Augment'
  else if FType=5 Then
    FTheText := 'AugmentL'
  else if FType=6 Then
    FTheText := 'AugmentR'
  else if FType=7 Then
    FTheText := 'Row'
  else if FType=8 Then
    FTheText := 'RowL'
  else if FType=9 Then
    FTheText := 'RowR'
  else if FType=10 Then
    FTheText := 'Column'
  else if FType=11 Then
    FTheText := 'ColumnL'
  else if FType=12 Then
    FTheText := 'ColumnR'
  else if FType=13 Then
    FTheText := 'Tab'
  else if FType=14 Then
    FTheText := 'TabL'
  else if FType=15 Then
    FTheText := 'TabR'
  else if FType=16 Then
    FTheText := 'Table'
  else if FType=17 Then
    FTheText := 'TableL'
  else if FType=18 Then
    FTheText := 'TableR';

  Buffer.Add('<'+FTheText+'>');
   for I := 0 to ChildCount-1 do
     Child[I].BuildMathML(Buffer,level+1);
  Buffer.Add(CharStrL(' ',level)+'</'+FTheText+'>');
end;

function TIPEqMat.GetName:String;
begin
  if FType=1 Then
    Result := 'MAT'
  else if FType=2 Then
    Result := 'MATL'
  else if FType=3 Then
    Result := 'MATR'
  else if FType=4 Then
    Result := 'AUGMENT'
  else if FType=5 Then
    Result := 'AUGMENTL'
  else if FType=6 Then
    Result := 'AUGMENTR'
  else if FType=7 Then
    Result := 'Row'
  else if FType=8 Then
    Result := 'RowL'
  else if FType=9 Then
    Result := 'RowR'
  else if FType=10 Then
    Result := 'ColUMN'
  else if FType=11 Then
    Result := 'ColUMNL'
  else if FType=12 Then
    Result := 'ColUMNR'
  else if FType=13 Then
    Result := 'TAB'
  else if FType=14 Then
    Result := 'TABL'
  else if FType=15 Then
    Result := 'TABR'
  else if FType=16 Then
    Result := 'TABLE'
  else if FType=17 Then
    Result := 'TABLEL'
  else if FType=18 Then
    Result := 'TABLER';
end;

function  TIPEqMat.Clone:TIPEqNode;
begin
  Result := TIPEqMat.Create;
  TIPEqMat(Result).CopyChildren(Self);
end;


function TIPEqMat.CalcMetrics:TIPEqMetrics;
var
  Ascent : Integer;
  Descent : Integer;
  Width : Integer;
  Em : Integer;
  RowCount,ColCount:Integer;
  TheM : Integer;
begin
  Em := GetFonTheight(GetTextMetrics);

  TheM := GetEmPart(MAT_INTERNALMARGIN,Em);

  RowCount := MatRowCount;
  ColCount := MatColCount;

  Width := 0;
  Ascent := 0;
  Descent := 0;

  if (FType=1) OR (FType=2) OR (FType=3) Then
  begin
    Width := GetPreviousColWidth(ColCount-1,TheM);
    Ascent := GetPreviousRowAscent(RowCount-1,TheM);
    Descent := GetPreviousRowDescent(RowCount-1,TheM);
  end;

  if (FType=4) OR (FType=5) OR (FType=6) Then
  begin
    Width := GetPreviousColWidth(ColCount-1,TheM)+1;
    Ascent := GetPreviousRowAscent(RowCount-1,TheM);
    Descent := GetPreviousRowDescent(RowCount-1,TheM);
  end;

  if (FType=7) OR (FType=8) OR (FType=9) Then
  begin
    Width := GetPreviousColWidth(ColCount-1,TheM)+1;
    Ascent := GetPreviousRowAscent(RowCount-1,TheM);
    Descent := GetPreviousRowDescent(RowCount-1,TheM)+RowCount-1;
  end;

  if (FType=10) OR (FType=11) OR (FType=12) Then
  begin
    Width := GetPreviousColWidth(ColCount-1,TheM)+ColCount-1;
    Ascent := GetPreviousRowAscent(RowCount-1,TheM)+1;
    Descent := GetPreviousRowDescent(RowCount-1,TheM);
  end;

  if (FType=13) OR (FType=14) OR (FType=15) Then
  begin
    Width := GetPreviousColWidth(ColCount-1,TheM)+ColCount;
    Ascent := GetPreviousRowAscent(RowCount-1,TheM)+2;
    Descent := GetPreviousRowDescent(RowCount-1,TheM+1)+1;
  end;

  if (FType=16) OR (FType=17) OR (FType=18) Then
  begin
    Width := GetPreviousColWidth(ColCount-1,TheM)+1;
    Ascent := GetPreviousRowAscent(RowCount-1,TheM)+1;
    Descent := GetPreviousRowDescent(RowCount-1,TheM);
  end;

  Result := TIPEqMetrics.Create(Ascent,Descent,Width,Em);
end;

procedure TIPEqMat.Layout;
var
  I, J : Integer;
  X, Y : Integer;
  RowCount,ColCount:Integer;
  A : Array of Array of TIPEqNode;
  TheM : Integer;
  Eq : Integer;
begin
  X:=0; Y:=0;

  TheM := GetEmPart(MAT_INTERNALMARGIN);

  RowCount := MatRowCount;
  ColCount := MatColCount;

  SetLength(A, RowCount,ColCount);

  for I := 0 to RowCount-1 do
  begin
    for J := 0 to ColCount-1 do
    begin
      A[I,J] := MatEqRow[I,J];
      if Assigned(A[I,J]) Then
      begin

        if FType=1 Then
        begin
          X := getPreviousColWidth(J-1,TheM)+(getMaxColWidth(J,TheM)-A[I,J].Width) div 2;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
        end;

        if FType=2 Then
        begin
          X := getPreviousColWidth(J-1,TheM) +TheM;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
        end;

        if FType=3 Then
        begin
          X := getPreviousColWidth(J,TheM)-A[I,J].Width - TheM;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
        end;

        if FType=4 Then
        begin
          X := getPreviousColWidth(J-1,TheM)+(getMaxColWidth(J,TheM)-A[I,J].Width) div 2;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
          if J=ColCount-1 Then
            Inc(X,1);
        end;

        if FType=5 Then
        begin
          X := getPreviousColWidth(J-1,TheM) +TheM;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
          if J=ColCount-1 Then
            Inc(X,1);
        end;

        if FType=6 Then
        begin
          X := getPreviousColWidth(J,TheM)-A[I,J].Width - TheM;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
          if J=ColCount-1 Then
            Inc(X,1);
        end;

        if FType=7 Then
        begin
          X := getPreviousColWidth(J-1,TheM)+(getMaxColWidth(J,TheM)-A[I,J].Width) div 2;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM+1)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM+1)-A[I,J].Height) div 2;
          if J>0 Then
            Inc(X,1);
        end;

        if FType=8 Then
        begin
          X := getPreviousColWidth(J-1,TheM) +TheM;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM+1)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM+1)-A[I,J].Height) div 2;
          if J>0 Then
            Inc(X,1);
        end;

        if FType=9 Then
        begin
          X := getPreviousColWidth(J,TheM)-A[I,J].Width - TheM;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM+1)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM+1)-A[I,J].Height) div 2;
          if J>0 Then
            Inc(X,1);
        end;

        if FType=10 Then
        begin
          X := getPreviousColWidth(J-1,TheM)+(getMaxColWidth(J,TheM)-A[I,J].Width) div 2+J;
          if (I=0) Then
            Y := (getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2
          else
            Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2 + 1
        end;

        if FType=11 Then
        begin
          X := getPreviousColWidth(J-1,TheM) +TheM+J;
          if (I=0) Then
            Y := (getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2
          else
            Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2 + 1
        end;

        if FType=12 Then
        begin
          X := getPreviousColWidth(J,TheM)-A[I,J].Width - TheM+J;
          if (I=0) Then
            Y := (getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2
          else
            Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2 + 1
        end;

        if FType=13 Then
        begin
          X := getPreviousColWidth(J-1,TheM)+(getMaxColWidth(J,TheM)-A[I,J].Width) div 2+1+J;
          if (I=0) Then
            Y := (getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2+1
          else
            Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2 + 2
        end;

        if FType=14 Then
        begin
          X := getPreviousColWidth(J-1,TheM) +TheM+1+J;
          if (I=0) Then
            Y := (getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2+1
          else
            Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2 + 2
        end;

        if FType=15 Then
        begin
          X := getPreviousColWidth(J,TheM)-A[I,J].Width - TheM+1+J;
          if (I=0) Then
            Y := (getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2+1
          else
            Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2 + 2
        end;

        if FType=16 Then
        begin
          X := getPreviousColWidth(J-1,TheM)+(getMaxColWidth(J,TheM)-A[I,J].Width) div 2;
          if J>0 Then
            Inc(X,1);
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
          if I>0 Then
            Inc(Y,1);
        end;

        if FType=17 Then
        begin
          X := getPreviousColWidth(J-1,TheM) +TheM;
          if J>0 Then
            Inc(X,1);
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
          if I>0 Then
            Inc(Y,1);
        end;

        if FType=18 Then
        begin
          X := getPreviousColWidth(J,TheM)-A[I,J].Width - TheM;
          if J>0 Then
            Inc(X,1);
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
          if I>0 Then
            Inc(Y,1);
        end;

        if Document.AlignByEq and (FType mod 3 = 1) Then
        begin
          Eq := GetMaxColEq(J);
          X := getPreviousColWidth(J - 1,TheM) + Eq - TIPEQRow(A[I,J]).GetEqPosition + TheM;
          Y := getPreviousRowAscent(I-1,TheM)+getPreviousRowDescent(I-1,TheM)+(getMaxRowAscent(I,TheM)+getMaxRowDescent(I,TheM)-A[I,J].Height) div 2;
        end;

        A[I,J].SetLocation(X,Y);
      end;
    end;
  end;

end;

procedure TIPEqMat.Draw(ACanvas:TCanvas);
var
  X0,Y0,TheM : Integer;
  R, C : Integer;
begin
  TheM := GetEmPart(MAT_INTERNALMARGIN);

  if (FType=4) OR (FType=5) OR (FType=6) Then
  begin
    if MatColCount>0 Then
    begin
      X0 := Width-getMaxColWidth(MatColCount-1,TheM)-1;
      Y0 := 0;
      ACanvas.MoveTo(X0,Y0);
      ACanvas.LineTo(X0,Height-1);
    end;
  end;
  if (FType=7) OR (FType=8) OR (FType=9) Then
  begin
    if MatColCount>0 Then
    begin
      X0 := getMaxColWidth(0,TheM);
      Y0 := 0;
      ACanvas.MoveTo(X0,Y0);
      ACanvas.LineTo(X0,Height-1);
    end;
    Y0 := 0;
    for r := 0 to MatRowCount-2 do
    begin
      X0 := 0;
      if r=0 Then
        Y0 := Y0+getMaxRowAscent(R,TheM)+getMaxRowDescent(R,TheM)
      else
        Y0 := Y0+getMaxRowAscent(R,TheM)+getMaxRowDescent(R,TheM+1);

      ACanvas.MoveTo(X0,Y0);
      ACanvas.LineTo(Width-1,Y0);
    end;
  end;
  if (FType=10) OR (FType=11) OR (FType=12) Then
  begin
    if MatRowCount>0 Then
    begin
      X0 := 0;
      Y0 := getMaxRowHeight(0,TheM);
      ACanvas.MoveTo(X0,Y0);
      ACanvas.LineTo(Width-1,Y0);
    end;
    X0 := 0;
    for C := 0 to MatColCount-2 do
    begin
      if C=0 Then
        X0 := X0+getMaxColWidth(C,TheM)
      else
        X0 := X0+getMaxColWidth(C,TheM)+1;
      Y0 := 0;
      ACanvas.MoveTo(X0,Y0);
      ACanvas.LineTo(X0,Height-1);
    end;
  end;
  if (FType=13) OR (FType=14) OR (FType=15) Then
  begin
    X0 := 0;
    Y0 := 0;
    ACanvas.MoveTo(X0,Y0);
    ACanvas.LineTo(Width-1,Y0);
    ACanvas.LineTo(Width-1,Height-1);
    ACanvas.LineTo(0,Height-1);
    ACanvas.LineTo(X0,Y0);
    if MatRowCount>0 Then
    begin
      X0 := 0;
      Y0 := getMaxRowHeight(0,TheM)+1;
      ACanvas.MoveTo(X0,Y0);
      ACanvas.LineTo(Width-1,Y0);
    end;
    X0 := 1;
    for c := 0 to MatColCount-2 do
    begin
      if c=0 Then
        X0 := X0+getMaxColWidth(C,TheM)
      else
        X0 := X0+getMaxColWidth(C,TheM)+1;
      Y0 := 0;
      ACanvas.MoveTo(X0,Y0);
      ACanvas.LineTo(X0,Height-1);
    end;
  end;
  if (FType=16) OR (FType=17) OR (FType=18) Then
  begin
    X0 := 0;
    Y0 := getMaxRowHeight(0,TheM);
    ACanvas.MoveTo(X0,Y0);
    ACanvas.LineTo(Width-1,Y0);

    X0 := getMaxColWidth(0,TheM);
    Y0 := 0;
    ACanvas.MoveTo(X0,Y0);
    ACanvas.LineTo(X0,Height-1);
  end;
end;

function TIPEqMat.GetText:String;
var
 R,C : Integer;
 Rcnt,Ccnt : Integer;
 Eq : TIPEqRow;
begin
  Result := '@'+GetName + '{';
  Rcnt := MatRowCount;
  Ccnt := MatColCount;
  for R := 0 to Rcnt-1 do
  begin
    if r > 0 Then
      Result := Result + ';';
    Result := Result + '{';
    for C := 0 to Ccnt-1 do
    begin
      if C > 0 Then
        Result := Result + ';';
      Eq := MatEqRow[R,C];
      if Assigned(Eq) Then
        Result := Result + Eq.GetText;
    end;
    Result := Result + '}';
  end;
  Result := Result + '}';
end;

function TIPEqMat.GetMaxColWidth(TheCol,TheMargin : Integer) :Integer;
var
   I : Integer;
   TheReturn : Integer;
   RowCount : Integer;
   Eq : Integer;
   IsDoAlignByEq : Boolean;

begin
   RowCount := MatRowCount;
   TheReturn := 0;
   Eq := 0;
   
   IsDoAlignByEq := Document.AlignByEq and (FType mod 3 = 1);

   if IsDoAlignByEq Then
     Eq := GetMaxColEq(TheCol);

   for I:=0 to RowCount-1 do
   begin
     if Assigned(MatEqRow[I,TheCol]) Then
     begin
       if IsDoAlignByEq Then
         TheReturn := Max(TheReturn, Eq + MatEqRow[I,TheCol].Width - MatEqRow[I,TheCol].GetEqPosition)
       else
         TheReturn := Max(TheReturn,MatEqRow[I,TheCol].Width);
     end;
   end;
   Result := TheReturn+2*TheMargin;
end;

function TIPEqMat.GetMaxRowAscent(TheRow,TheMargin : Integer) :Integer;
var
   I : Integer;
   TheReturn : Integer;
   RowCount, ColCount : Integer;
begin
   RowCount := MatRowCount;
   ColCount := MatColCount;
   TheReturn := 0;
   for I:=0 to ColCount-1 do
   begin
     if Assigned(MatEqRow[TheRow,I]) Then
     begin
       if ((RowCount mod 2) = 0) Then
       begin
         if (TheRow<(RowCount div 2)) Then
           TheReturn := Max(TheReturn,MatEqRow[TheRow,I].Height)
         else
           TheReturn := 0;
       end
       else
       begin
         if (TheRow=(RowCount div 2)) Then
           TheReturn := Max(TheReturn,MatEqRow[TheRow,I].Ascent)
         else if (TheRow<(RowCount div 2)) Then
           TheReturn := Max(TheReturn,MatEqRow[TheRow,I].Height)
         else
           TheReturn := 0;
       end;
     end;
   end;
   Result := TheReturn+TheMargin;
end;

function TIPEqMat.GetMaxRowDescent(TheRow,TheMargin : Integer) :Integer;
var
   I : Integer;
   TheReturn : Integer;
   RowCount, ColCount : Integer;
begin
   RowCount := MatRowCount;
   ColCount := MatColCount;
   TheReturn := 0;
   for I:=0 to ColCount-1 do
   begin
     if Assigned(MatEqRow[TheRow,I]) Then
     begin
       if ((RowCount mod 2) = 0) Then
       begin
         if (TheRow>=(RowCount div 2)) Then
           TheReturn := Max(TheReturn,MatEqRow[TheRow,I].Height)
         else
           TheReturn := 0;
       end
       else
       begin
         if (TheRow=(RowCount div 2)) Then
           TheReturn := Max(TheReturn,MatEqRow[TheRow,I].Descent)
         else if (TheRow>(RowCount div 2)) Then
           TheReturn := Max(TheReturn,MatEqRow[TheRow,I].Height)
         else
           TheReturn := 0;
       end;
     end;
   end;
   Result := TheReturn+TheMargin;
end;

function TIPEqMat.GetMaxRowHeight(TheRow,TheMargin : Integer) :Integer;
begin
  Result := GetMaxRowAscent(TheRow,TheMargin)+getMaxRowDescent(TheRow,TheMargin);
end;

function TIPEqMat.GetPreviousColWidth(TheCol,TheMargin : Integer) :Integer;
var
  J : Integer;
  TheReturn : Integer;
Begin
  TheReturn := 0;
  for J:=0 to TheCol do
  begin
    inc(TheReturn, getMaxColWidth(J,TheMargin));
  end;
  Result := TheReturn;
end;

function TIPEqMat.GetPreviousRowAscent(TheRow,TheMargin : Integer) :Integer;
var
  I : Integer;
  TheReturn : Integer;
begin
  TheReturn := 0;
  for I:=0 to TheRow do
  begin
    inc(TheReturn, getMaxRowAscent(I,TheMargin));
  end;
  Result := TheReturn;
end;

function TIPEqMat.GetPreviousRowDescent(TheRow,TheMargin : Integer) :Integer;
var
  I : Integer;
  TheReturn : Integer;
begin
  TheReturn := 0;
  for I:=0 to TheRow do
  begin
    inc(TheReturn, getMaxRowDescent(I,TheMargin));
  end;
  Result := TheReturn;
end;

function TIPEqMat.getRowAbove(ARow:Integer):TIPEqRow;
var
  R,C : Integer;
  Node : TIPEqRow;
  Found : boolean;
begin
  Found := false;
  C:=-1;
  Result := nil;
  Node := Row[ARow];
  for R := 0 to MatRowCount-1 do
  begin
    for C := 0 to MatColCount-1 do
    begin
      if GetMatEqRow(R,C) = Node Then
      begin
        Found := true;
        Break;
      end;
    end;
    if Found Then
      Break;
  end;

  if Found and (R > 0) Then
  begin
    Result := GetMatEqRow(R-1,C);
  end;
end;

function TIPEqMat.getRowBelow(ARow:Integer):TIPEqRow;
var
  R,C : Integer;
  Node : TIPEqRow;
  Found : boolean;
begin
  Found := false;
  C:=-1;
  Result := nil;
  Node := Row[ARow];
  for R := 0 to MatRowCount-1 do
  begin
    for C := 0 to MatColCount-1 do
    begin
      if GetMatEqRow(R,C) = Node Then
      begin
        Found := true;
        Break;
      end;
    end;
    if Found Then
      Break;
  end;

  if Found and (R < (MatRowCount-1)) Then
  begin
    Result := GetMatEqRow(R+1,C);
  end;
end;

function TIPEqMat.GetMaxColEq(TheCol: Integer): Integer;
var I, Eq : integer;

begin
  Result := 0;
  for I := 0 to MatRowCount - 1 do
  begin
    if Assigned(MatEqRow[I,TheCol]) Then
    begin
    Eq := MatEqRow[I,TheCol].GetEqPosition;
    if Eq > Result Then
      Result := Eq;
    end;
  end;
end;

end.
