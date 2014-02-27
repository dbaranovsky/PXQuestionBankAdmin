unit TDXExprParserX;

interface

uses CocoBase,TDXExprParser,Classes;

type

 TTDXExprParserX = class(TTDXExprParser)
 private
 protected
 public
   constructor Create(AOwner : TComponent); override;
 end;

 TTDXExprParserScannerX = class(TTDXExprParserScanner)
 private
    FSymbolicSymbol : Integer;
 protected
 public
    procedure Get(var Sym : integer); override;
    property SymbolicSymbol:Integer read FSymbolicSymbol write FSymbolicSymbol;
 end;


implementation

   constructor TTDXExprParserX.Create(AOwner : TComponent);
   begin
     inherited;
     Scanner.Free;
     Scanner := TTDXExprParserScannerX.Create;
     TTDXExprParserScannerX(Scanner).SymbolicSymbol := FSymbolicSymbol;
   end;

   procedure TTDXExprParserScannerX.Get(var Sym : integer);
   var
     Str : String;
     ExtraChars : Integer;
   begin
     inherited Get(sym);
     if Sym = maxT then {Assume maxT = nosymbol token.  Hope this doesn't change.}
     begin
       Str := GetString(NExtSymbol);
       if (Length(Str) > 0) and (Str[1] in ['a'..'z','A'..'Z']) then
       begin
         Sym := FSymbolicSymbol;
         ExtraChars := NextSymbol.Len-1;
         NextSymbol.Col := NextSymbol.Col - (ExtraChars);
         NextSymbol.Len := 1;
         BufferPosition := BufferPosition - ExtraChars;
         CurrInputCh := CurrentCh(BufferPosition);
       end;
     end;
   end;


end.
