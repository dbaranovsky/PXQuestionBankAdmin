unit IPEqSymbolW;

interface

uses IPEqNode,Graphics,Windows,Classes;

type

TIPEqSymbolType =
  (eqsNull,eqsAlpha,eqsBeta,eqsGamma,
   eqsDelta,eqsEpsi,eqsZeta,eqsEta,
   eqsTheta,eqsIota,eqsKappa,eqsLambda,
   eqsMu,eqsNu,eqsXi,eqsOmicron,
   eqsPi,eqsRho,eqsSigma,eqsTau,
   eqsUpsi,eqsPhi,eqsChi,eqsPsi,
   eqsOmega,eqsLe,eqsGe,eqsNe,
   eqsInfin,eqsCap,eqsCup,eqsAnd,
   eqsOr,eqsNotIn,eqsIn,eqsSubE,
   eqsDeg,eqsCent,eqsRAngle,eqsInf,
   eqsEmpty,eqsUpArrow,eqsDownArrow,eqsLeftArrow,
   eqsRightArrow,eqsLeftRightArrow,eqsDLeftRightArrow,eqsLAngle,eqsAngle,eqsSqrt,eqsMinusplus,
   eqsCDelta,eqslogNot,eqsSubset,eqsNotSubset,
   eqsPropSubset,eqsNotPropSubset,eqsCGamma,eqsCLambda,
   eqsCOmega,eqsCPhi,eqsphiv,eqsCPi,
   eqspiv,eqsCPsi,eqsCSigma,eqssigmav,
   eqsCTheta,eqsthetav,eqsCUpsi,eqsCXi,
   eqsPrime,eqsPrime2,eqsPrime3,eqsTilde,
   eqsOtimes,eqsOPlus,eqsForAll,eqsExists,
   eqsPropsupset,eqsSupset,eqsNotPropsupset,eqsNotSupset,eqsCompfn,eqsHyphen,
   eqsPound,eqsYen,eqsEuro,eqsEMDash,eqsGrad,eqsPartialD,eqsEquiv,eqsAmp

  );

TIPEqSymbolInfo = record
  Name : String[15];
  SymChar : Char;
  UniChar : WideChar;
  MM : String[20];
end;

const

  IPEqCurrencySymbols : array[TIPEqCurrency] of  TIPEqSymbolType = (eqsNull,eqsPound,eqsEuro,eqsYen);

  IPEqSymbolData : array[TIPEqSymbolType] of TIPEqSymbolInfo = (

      ( Name:'null';          SymChar:#$00;  UniChar:#$0000;    MM:'' ),
      ( Name:'alpha';         SymChar:#$61;  UniChar:#$03B1;    MM:'Alpha' ),
      ( Name:'beta';          SymChar:#$62;  UniChar:#$03B2;    MM:'Beta' ),
      ( Name:'gamma';         SymChar:#$67;  UniChar:#$03B3;    MM:'Gamma' ),

      ( Name:'delta';         SymChar:#$64;  UniChar:#$03B4;    MM:'Delta' ),
      ( Name:'epsi';          SymChar:#$65;  UniChar:#$03B5;    MM:'Epsilon' ),
      ( Name:'zeta';          SymChar:#$7A;  UniChar:#$03B6;    MM:'Zeta' ),
      ( Name:'eta';           SymChar:#$68;  UniChar:#$03B7;    MM:'Eta' ),

      ( Name:'theta';         SymChar:#$71;  UniChar:#$03B8;    MM:'Theta' ),
      ( Name:'iota';          SymChar:#$69;  UniChar:#$03B9;    MM:'Iota' ),
      ( Name:'kappa';         SymChar:#$6B;  UniChar:#$03BA;    MM:'Kappa' ),
      ( Name:'lambda';        SymChar:#$6C;  UniChar:#$03BB;    MM:'Lambda' ),

      ( Name:'mu';            SymChar:#$6D;  UniChar:#$03BC;    MM:'Mu' ),
      ( Name:'nu';            SymChar:#$6E;  UniChar:#$03BD;    MM:'Nu' ),
      ( Name:'xi';            SymChar:#$78;  UniChar:#$03BE;    MM:'Xi' ),
      ( Name:'omicron';       SymChar:#$78;  UniChar:#$03BF;    MM:'Omicron' ),

      ( Name:'pi';            SymChar:#$70;  UniChar:#$03C0;    MM:'Pi' ),
      ( Name:'rho';           SymChar:#$72;  UniChar:#$03C1;    MM:'Rho' ),
      ( Name:'sigma';         SymChar:#$73;  UniChar:#$03C3;    MM:'Sigma' ),
      ( Name:'tau';           SymChar:#$74;  UniChar:#$03C4;    MM:'Tau' ),

      ( Name:'upsi';          SymChar:#$75;  UniChar:#$03C5;    MM:'Upsilon' ),
      ( Name:'phi';           SymChar:#$66;  UniChar:#$03C6;    MM:'Phi' ),
      ( Name:'chi';           SymChar:#$63;  UniChar:#$03C7;    MM:'Chi' ),
      ( Name:'psi';           SymChar:#$79;  UniChar:#$03C8;    MM:'Psi' ),

      ( Name:'omega';         SymChar:#$77;  UniChar:#$03C9;    MM:'Omega' ),
      ( Name:'le';            SymChar:#$A3;  UniChar:#$2264;    MM:'LessEqual' ),
      ( Name:'ge';            SymChar:#$B3;  UniChar:#$2265;    MM:'GreaterEqual' ),
      ( Name:'ne';            SymChar:#$B9;  UniChar:#$2260;    MM:'NotEqual' ),

      ( Name:'infin';         SymChar:#$A5;  UniChar:#$221E;    MM:'Infinity' ),
      ( Name:'cap';           SymChar:#$C7;  UniChar:#$2229;    MM:'Cap' ),
      ( Name:'cup';           SymChar:#$C8;  UniChar:#$222A;    MM:'Cup' ),
      ( Name:'and';           SymChar:#$D9;  UniChar:#$2227;    MM:'And' ),

      ( Name:'or';            SymChar:#$DA;  UniChar:#$2228;    MM:'Or' ),
      ( Name:'notin';         SymChar:#$CF;  UniChar:#$2209;    MM:'NotElement' ),
      ( Name:'in';            SymChar:#$CE;  UniChar:#$2208;    MM:'Element' ),
      ( Name:'sube';          SymChar:#$CD;  UniChar:#$2286;    MM:'SubsetEqual' ),

      ( Name:'deg';           SymChar:#$B0;  UniChar:#$00B0;    MM:'Degree' ),
      ( Name:'cent';          SymChar:#$00;  UniChar:#$00A2;    MM:'Cent' ),
      ( Name:'rangle';        SymChar:#$F1;  UniChar:#$232A;    MM:'RightAngleBracket' ),
      ( Name:'inf';           SymChar:#$A5;  UniChar:#$221E;    MM:'Infinity' ),

      ( Name:'empty';         SymChar:#$C6;  UniChar:#$2205;    MM:'EmptySet' ),
      ( Name:'uparrow';       SymChar:#$AD;  UniChar:#$2191;    MM:'UpArrow' ),
      ( Name:'downarrow';     SymChar:#$AF;  UniChar:#$2193;    MM:'DownArrow' ),
      ( Name:'leftarrow';     SymChar:#$AC;  UniChar:#$2190;    MM:'LeftArrow' ),

      ( Name:'rightarrow';    SymChar:#$AE;  UniChar:#$2192;    MM:'RightArrow' ),
      ( Name:'leftrightarrow';SymChar:#$AB;  UniChar:#$2194;    MM:'LeftRightArrow' ),
      ( Name:'dleftrightarrow';SymChar:#$DB;  UniChar:#$21D4;    MM:'DLeftRightArrow' ),
      ( Name:'langle';        SymChar:#$E1;  UniChar:#$232A;    MM:'LeftAngleBracket' ),
      ( Name:'angle';         SymChar:#$D0;  UniChar:#$2220;    MM:'Angle' ),
      ( Name:'sqrt';          SymChar:#$D6;  UniChar:#$221A;    MM:'Sqrt' ),
      ( Name:'minusplus';     SymChar:#$D6;  UniChar:#$2213;    MM:'Minusplus' ),

      ( Name:'cdelta';        SymChar:#$44;  UniChar:#$0394;    MM:'CapitalDelta' ),
      ( Name:'lognot';        SymChar:#$D8;  UniChar:#$00AC;    MM:'Not' ),
      ( Name:'subset';        SymChar:#$CD;  UniChar:#$2286;    MM:'SubsetEqual' ),
      ( Name:'notsubset';     SymChar:#$CD;  UniChar:#$2288;    MM:'NotSubsetEqual' ),

      ( Name:'propsubset';    SymChar:#$CC;  UniChar:#$2282;    MM:'Subset' ),
      ( Name:'notpropsubset'; SymChar:#$CB;  UniChar:#$2284;    MM:'NotSubset' ),
      ( Name:'cgamma';        SymChar:#$47;  UniChar:#$0393;    MM:'CapitalGamma' ),
      ( Name:'clambda';       SymChar:#$4C;  UniChar:#$039B;    MM:'CapitalLambda' ),

      ( Name:'comega';        SymChar:#$57;  UniChar:#$03A9;    MM:'CapitalOmega' ),
      ( Name:'cphi';          SymChar:#$46;  UniChar:#$03A6;    MM:'CapitalPhi' ),
      ( Name:'phiv';          SymChar:#$6A;  UniChar:#$03C6;    MM:'CurlyPhi' ),
      ( Name:'cpi';           SymChar:#$50;  UniChar:#$03A0;    MM:'CapitalPi' ),

      ( Name:'piv';           SymChar:#$76;  UniChar:#$03D6;    MM:'CurlyPi' ),
      ( Name:'cpsi';          SymChar:#$59;  UniChar:#$03A8;    MM:'CapitalPsi' ),
      ( Name:'csigma';        SymChar:#$53;  UniChar:#$03A3;    MM:'CapitalSigma' ),
      ( Name:'sigmav';        SymChar:#$56;  UniChar:#$03C2;    MM:'FinalSigma'),

      ( Name:'ctheta';        SymChar:#$51;  UniChar:#$0398;    MM:'CapitalTheta' ),
      ( Name:'thetav';        SymChar:#$4A;  UniChar:#$03D1;    MM:'CurlyTheta' ),
      ( Name:'cupsi';         SymChar:#$A1;  UniChar:#$03D2;    MM:'CapitalUpsilon' ),
      ( Name:'cxi';           SymChar:#$58;  UniChar:#$039E;    MM:'CapitalXi' ),
      ( Name:'prime';         SymChar:#$A2;  UniChar:#$2023;    MM:'Prime' ),
      ( Name:'prime2';        SymChar:#$A2;  UniChar:#$2023;    MM:'DoublePrime' ),
      ( Name:'prime3';        SymChar:#$A2;  UniChar:#$2023;    MM:'TriplePrime' ),
      ( Name:'tilde';         SymChar:'~';   UniChar:'~';       MM:'Tilde' ),
      ( Name:'otimes';        SymChar:#$C4;  UniChar:#$2297;    MM:'CircleTimes' ),
      ( Name:'oplus';         SymChar:#$C5;  UniChar:#$2295;    MM:'CirclePlus' ),
      ( Name:'forall';        SymChar:#$22;  UniChar:#$2200;    MM:'ForAll' ),
      ( Name:'exists';        SymChar:#$24;  UniChar:#$2203;    MM:'Exists' ),
      ( Name:'propsupset';    SymChar:#$C9;  UniChar:#$2283;    MM:'Superset' ),
      ( Name:'supset';        SymChar:#$CA;  UniChar:#$2287;    MM:'SupersetEqual' ),
      ( Name:'notpropsupset';    SymChar:#$C9;  UniChar:#$2284;    MM:'Superset' ),
      ( Name:'notsupset';        SymChar:#$CA;  UniChar:#$2286;    MM:'SupersetEqual' ),


      ( Name:'compfn';        SymChar:#$00;  UniChar:#$25E6 {2218};     MM:'EmptySmallCircle' ),
      ( Name:'hyphen';        SymChar:'-';   UniChar:'-';       MM:'Dash' ),
      ( Name:'pound';         SymChar:#$00;  UniChar:#$00A3;    MM:'' ),
      ( Name:'yen';           SymChar:#$00;  UniChar:#$00A5;    MM:'Yen' ),
      ( Name:'euro';          SymChar:#$00;  UniChar:#$20AC;    MM:'' ),
      ( Name:'mdash';        SymChar:#$be;   UniChar:#$2014;    MM:'LongDash' ),
      ( Name:'grad' ;         SymChar:#$D1;  UniChar:#$2207;    MM:'Del'),
      ( Name:'partiald';      SymChar:#$B6;   UniChar:#$2202;   MM:'PartialD'),
      ( Name:'equiv';      SymChar:#$BA;   UniChar:#$2261;   MM:''),
      ( Name:'amp';      SymChar:'&';   UniChar:'&';   MM:'')

  );

(*
  IPEqSymbolValues : array[TIPEqSymbolType] of Char =
  (#$00,#$61,#$62,#$67,#$64,#$65,#$7a,#$68,#$71,
   #$69,#$6b,#$6c,#$6d,#$6e,#$78,#$6f,#$70,
   #$72,#$73,#$74,#$75,#$6a,#$63,#$79,#$77,
   #$a3,#$b3,#$b9,#$a5,#$c7,#$c8,#$d9,#$da,
   #207,#206,#205,#176,#0,#241,#165,#198,
   #173,#175,#172,#174,#225,#208,#214,
   #$44,#$D8,#$CD,#$CD,#$CC,#$CB);

  IPEqSymbolValuesW : array[TIPEqSymbolType] of WideChar =
  (#$0000,#$03b1,#$03b2,#$03b3,#$03b4,#$03b5,#$03b6,#$03b7,#$03b8,
   #$03b9,#$03ba,#$03bb,#$03bc,#$03bd,#$03be,#$03bf,#$03c0,
   #$03c1,#$03c3,#$03c4,#$03c5,#$0366,#$03c7,#$03c8,#$03c9,
   #$2264,#$2265,#$2260,#$221E,#$2229,#$222a,#$2227,#$2228,
   #$2209,#$2208,#$2286,#$00b0,#$00a2,#$232a,#$221E,#$2205,
   #$2191,#$2193,#$2190,#$2192,#$232a,#$2220,#$221a,
   #$0394,#$00AC,#$0000,#$0000,#$0000,#$0000);

  IPEqSymbolNames : array[TIPEqSymbolType] of String =
   ('null','alpha','beta','gamma','delta','epsi','zeta','eta','theta',
    'iota','kappa','lambda','mu','nu','xi','phi','pi',
    'rho','sigma','tau','upsi','phi','chi','psi','omega',
    'le','ge','ne','infin','cap','cup','and','or',
    'notin','in','sube','deg','cent','rangle','inf','empty',
    'uparrow','downarrow','leftarrow','rightarrow','langle','angle','sqrt',
    'cdelta','lognot','subset','notsubset','propsubset','notpropsubset'
    );
*)

  IPEqForceSymbol : set of TIPEqSymbolType =
    [eqsCap,eqsCup,eqsOr,eqsAnd,eqsNotIn,eqsIn,eqsSubE,eqsRAngle,eqsLAngle,
     eqsEmpty,eqsAngle,eqsSubset,eqsNotSubset,eqsPropsubset,eqsNotPropSubset,
     eqsPhi,eqsCUpsi,eqsPiv,eqsthetav,eqsPrime,eqsPrime2,eqsPrime3, eqsDLeftRightArrow,
     eqsPropSupset,eqsSupSet,eqsNotPropSupset,eqsNotSupSet,eqsOTimes,eqsOPlus,eqsForAll,eqsExists,eqsGrad];

  IPEqForceUnicode : set of TIPEqSYmbolType =
    [eqsCent,eqsTilde,eqsPound,eqsYen,eqsEuro];

type

TIPEqSymbolW = class(TIPEqNode)
  private
    FSymbol : TIPEqSymbolType;
    FSpaceSize : Integer;
    FVOffset : Integer;
    procedure SetSymbol(Value:TIPEqSymbolType);
    function UseSymbolFont:Boolean;
    function GetSymbolChar:String;
    function GetUniChar:WideString;
  protected
    function CalcMetrics:TIPEqMetrics; override;
    procedure Draw(ACanvas:TCanvas); override;
    procedure Layout; override;
  public
    constructor Create(ASymbol:TIPEqSymbolType);
    procedure DeleteCharacter(CaretEvent:TIPEqCaretEvent); override;
    function  Clone:TIPEqNode; override;
    function IsEmpty:boolean; override;
    procedure BuildMathML(Buffer:TStrings; Level:Integer); override;
    function GetText:String; override;

    property Symbol:TIPEqSymbolType read FSymbol write SetSymbol;
    property SpaceSize:Integer read FSpaceSize write FSpaceSize;
end;

Function GetEntityNameFromUniChar(UniChar:WideChar):String;

Function GetEqSymbolType(Name:String; var SType:TIPEqSymbolType):boolean;

Function FindSymbolFromChar(Ch:Char; var SType:TIPEqSymbolType):boolean;

Function GetEqSymbolTypeMM(Name:String; var SType:TIPEqSymbolType):boolean;


implementation

uses IPEqUtils,Math,Sysutils;
var     FUniFont : TFont;

Function FindSymbolFromChar(Ch:Char; var SType:TIPEqSymbolType):boolean;
var
 I : TIPEqSymbolType;
begin
  for I := Low(IPEqSymbolData) to High(IPEqSymbolData) do
  begin
    if IPEqSymbolData[I].SymChar = Ch then
    begin
      SType := i;
      REsult := true;
      Exit;
    end;
  end;
  Result := false;
end;

Function GetEntityNameFromUniChar(UniChar:WideChar):String;
var
 I : TIPEqSymbolType;
begin
  for I := Low(IPEqSymbolData) to High(IPEqSymbolData) do
  begin
    if IPEqSymbolData[I].UniChar = UniChar then
    begin
      Result := '&'+IPEqSymbolData[I].Name+';';
      Exit;
    end;
  end;
  Result := '';
end;

Function GetEqSymbolType(Name:String; var SType:TIPEqSymbolType):boolean;
var
 I : TIPEqSymbolType;
begin
  for I := Low(IPEqSymbolData) to High(IPEqSymbolData) do
  begin
    if SameText(IPEqSymbolData[I].Name,Name) then
    begin
      SType := I;
      REsult := true;
      Exit;
    end;
  end;
  Result := false;
end;


Function GetEqSymbolTypeMM(Name:String; var SType:TIPEqSymbolType):boolean;
var
 I : TIPEqSymbolType;
begin
  for I := Low(IPEqSymbolData) to High(IPEqSymbolData) do
  begin
    if SameText(IPEqSymbolData[I].MM,Name) then
    begin
      SType := I;
      REsult := true;
      Exit;
    end;
  end;
  Result := false;
end;

constructor TIPEqSymbolW.Create(ASymbol:TIPEqSymbolType);
begin
  inherited Create;
  FSymbol := ASymbol;
  FSpaceSize := 0;
end;

function  TIPEqSymbolW.Clone:TIPEqNode;
begin
  Result := TIPEqSymbolW.Create(FSymbol);
end;

procedure TIPEqSymbolW.BuildMathML(Buffer:TStrings; Level:Integer);
begin
  Buffer.Add('<mo>&'+IPEqSymbolData[FSymbol].Name+';</mo>');
end;

function TIPEqSymbolW.GetText:String;
begin
  Result := '&'+IPEqSymbolData[FSymbol].Name+';';
end;


function TIPEqSymbolW.UseSymbolFont:Boolean;
begin
  Result := (Document.UseSymbolFont or (FSymbol in IPEqForceSymbol))
      and not (FSymbol in IPEqForceUnicode);
end;


function TIPEqSYmbolW.GetSymbolChar:String;
begin
 Result := IPEqSymbolData[FSymbol].SymChar;
 if (FSymbol = eqsPrime2) or (FSYmbol = eqsPrime3) then
   Result := Result + IPEqSymbolData[FSymbol].SymChar;
 if FSYmbol = eqsPrime3 then
   Result := Result + IPEqSymbolData[FSymbol].SymChar;
end;

function TIPEqSYmbolW.GetUniChar:WideString;
begin
 Result := IPEqSymbolData[FSymbol].UniChar;
 if (FSymbol = eqsPrime2) or (FSYmbol = eqsPrime3) then
   Result := Result + IPEqSymbolData[FSymbol].UniChar;
 if FSYmbol = eqsPrime3 then
   Result := Result + IPEqSymbolData[FSymbol].UniChar;
end;

function TIPEqSymbolW.CalcMetrics:TIPEqMetrics;
var
  TextMetric : TTextMetric;
  W : Integer;
  Em : Integer;
begin

  if assigned(Parent) and UseSymbolFont then
  begin
    InitFont;
    Font.Name := 'Symbol';
    Font.Size := Font.Size+1;
  end;

  Em := GetEMWidth(Font);
  TextMetric := GetTextMetrics;
  if UseSymbolFont then
    W := GetTextExtent(GetSymbolChar).Cx+2*GetEMPart(FSpaceSize,Em)
  else
    W := GetTextSizeW(Font,GetUniChar).Cx+2*GetEMPart(FSpaceSize,Em);

  FVOffset := TextMetric.tmInternalLeading;

  Result := TIPEqMetrics.Create(TextMetric.tmAscent-FVoffset,TextMetric.tmDescent,W,Em);
end;

procedure TIPEqSymbolW.Draw(ACanvas:TCanvas);
var
  Y : Integer;
  WStr : WideString;
  X : Integer;
  OldStyle : TBrushStyle;
  OldFont : TFont;
begin

  //Move gamma up a little
  if FSymbol = eqsGamma then
    Y := -Descent div 2
  else if FSymbol = eqsChi then
    Y := -Descent
  else
    Y := 0;

  if UseSymbolFont then
  begin
    ACanvas.TextOut(GetEmPart(FSpaceSize),y-FVOffset,GetSymbolChar);
    if (FSymbol = eqsNotSubSet) or (FSymbol = eqsNotSupSet) or (FSymbol = eqsNotPropSupSet) then
    begin
      X := (Width - ACanvas.TextWidth('/')) div 2;
      Y := y +(Descent div 2);
      OldStyle := ACanvas.Brush.Style;
      ACanvas.Brush.Style := bsClear;
      ACanvas.TextOut(x,y-FVOffset,'/'{#$CB});
      ACanvas.Brush.Style := OldStyle;
    end;
  end
  else
  begin
    WStr := GetUniChar;
    if (FSymbol = eqsMinusplus) then // or (FSymbol = eqsDLeftRightArrow) then
    begin
      OldFont := ACanvas.Font;
      FUniFont.Size := OldFont.Size;
      ACanvas.Font := FUniFont;
      TextOutW(ACanvas.Handle,GetEmPart(FSpaceSize),Y-FVOffset,@WStr[1],1);
      ACanvas.Font := OldFont;
    end else
    begin
      TextOutW(ACanvas.Handle,GetEmPart(FSpaceSize),Y-FVOffset,@WStr[1],1);
    end
  end;
end;

procedure TIPEqSymbolW.Layout;
begin
end;

procedure TIPEqSymbolW.SetSymbol(Value:TIPEqSymbolType);
begin
  if Value <> FSymbol then
  begin
    FSymbol := Value;
    Invalidate;
  end;
end;


procedure TIPEqSymbolW.DeleteCharacter(CaretEvent:TIPEqCaretEvent);
var
  Pos1 : Integer;
begin
  Pos1 := Min(CaretEvent.Position,CaretEvent.PositionStart);

  if Pos1 = 0 then
  begin
    FSymbol := eqsNull;
    Invalidate;
    CaretEvent.CharacterDeleted := true;
  end;
end;

function TIPEqSymbolW.IsEmpty:boolean;
begin
  Result := FSymbol = eqsNull;
end;


initialization
  FUniFont := TFont.Create;
  //FUniFont.Name := 'Arial Unicode MS';
  FUniFont.Name := 'FreeSans';
finalization
  FUniFont.Free;
end.
