unit ipeqgifserver;

{$WARN SYMBOL_PLATFORM OFF}

interface

uses
  ComObj, ActiveX, IPEQGif_TLB, StdVcl,
  IpEqNode,Graphics, GifImage, Types,TDXExpression, TDXExpr,IpStrUtils;

type
  TIPEqGifServer = class(TAutoObject, IIPEqGifServer)
  private
    FEQText : String;
    FExprText : String;
    FBottomPadding:Integer;
    FTopPadding: Integer;
    FInterpretFunctions : Boolean;
    FInterpretSet : Boolean;
    FAlignByEq : Boolean;
    FEqDoc : TIPEqDocument;
    FBitmap : TBitmap;
    FBBitmap : TBitmap;
    FFontSize : Integer;
    FPath : String;
    FFileName : String;
    FDoNotReduce : Boolean;
    FErrorDisplayText : String;
    FError : String;
    FIsError : Boolean;
    FDoBorder : Boolean;
    procedure SetEqText(const Value: string);
    procedure SetExprText(const Value: String);
    procedure SetBottomPadding(const Value: Integer);
    procedure SetTopPadding(const Value: Integer);
    procedure SetInterpretFunctions(const Value: BOolean);
    procedure MakeGif;
    procedure MakeGifEq;
    procedure MakeGifExpr;
    procedure MakeErrorGif;
    function GetImageInfoEq: WideString;
    function GetImageInfoExpr: WideString;
    procedure SaveGif(Gif:TGifImage);
    procedure SaveGifFile;
    procedure ProcessPadding;
    procedure SetAlignByEq(const Value: Boolean);
    function GetPath: String;
    procedure SetPath(const Value: String);
    procedure SetFontsize(const Value: Integer);
    procedure InitObject;
    procedure FinalizeObject;
    procedure SetFileName(const Value: String);
    procedure SetErrorDisplayText(const Value: String);
    function GetErrorDisplayText: String;
    function GetInterpretFunctions: Boolean;
  protected
    property EqText:string read FEqText write SetEqText;
    property ExprText:String read FExprText write SetExprText;
    property TopPadding:Integer read FTopPadding write SetTopPadding default 0;
    property BottomPadding:Integer read FBottomPadding write SetBottomPadding default 0;
    property InterpretFunctions:Boolean read GetInterpretFunctions write SetInterpretFunctions Default True;
    property AlignByEq:Boolean read FAlignByEq write SetAlignByEq default True;
    property Path:String read GetPath write SetPath;
    property FileName:String read FFileName write SetFileName;
    property ErrorDisplayText:String read GetErrorDisplayText write SetErrorDisplayText;

    property Fontsize:Integer read FFontsize write SetFontsize default 10;

    function GetReducedExpr(ExprText: string): string;
    function Get_Bottom: Integer; safecall;
    function Get_EqText: WideString; safecall;
    function Get_ExprText: WideString; safecall;
    function Get_Top: Integer; safecall;
    procedure Set_Bottom(Value: Integer); safecall;
    procedure Set_EqText(const Value: WideString); safecall;
    procedure Set_ExprText(const Value: WideString); safecall;
    procedure Set_Top(Value: Integer); safecall;
    function Get_AlignByEq: WordBool; safecall;
    function Get_Fontsize: Integer; safecall;
    function Get_InterpretFunctions: WordBool; safecall;
    procedure Set_AlignByEq(Value: WordBool); safecall;
    procedure Set_Fontsize(Value: Integer); safecall;
    procedure Set_InterpretFunctions(Value: WordBool); safecall;
    function Get_Path: WideString; safecall;
    procedure Save; safecall;
    procedure Set_Path(const Value: WideString); safecall;
    function Get_Filename: WideString; safecall;
    procedure Set_Filename(const Value: WideString); safecall;
    function Get_DoNotReduce: WordBool; safecall;
    procedure Set_DoNotReduce(Value: WordBool); safecall;
    function Get_Error: WideString; safecall;
    function Get_ErrorDisplayText: WideString; safecall;
    function Get_IsError: WordBool; safecall;
    procedure Set_ErrorDisplayText(const Value: WideString); safecall;
    function Get_DoBorder: WordBool; safecall;
    procedure Set_DoBorder(Value: WordBool); safecall;
    function GetReduced(const ExprText: WideString): WideString; safecall;
    function GetImageInfo: WideString; safecall;
  end;

const
  DEFPATH = 'c:\windows\temp';
  DEFNAME = 'eq.gif';
  DEFERROR = '!ERROR';

implementation

uses ComServ, SysUtils, StrUtils;//, ThemeSrv;

function TIPEqGifServer.Get_Bottom: Integer;
begin
  Result := FBottomPadding;
end;

function TIPEqGifServer.Get_EqText: WideString;
begin
  Result := FEqtext;
end;

function TIPEqGifServer.Get_ExprText: WideString;
begin
  Result := FExprText;
end;

function TIPEqGifServer.Get_Top: Integer;
begin
  Result := FTopPadding;
end;

procedure TIPEqGifServer.MakeGif;
begin
  try
  if FExprText <> '' then
    try
      EqText :=  FExprText;
      FExprText := IPPrepareExpressionForRendering(FExprText);
      //ver 1.2 make ExprText fall back to eqtext if there is an error
      MakeGifExpr;
    except
      EqText := AnsiReplaceText(EqText,'!&eq;','!=');
      EqText :=IPPreProcessPowerSign(EqText);
      EqText :=IPProcessVarDirective(EqText,False);
      MakeGifEq;
    end
  else
    EqText := AnsiReplaceText(EqText,'!&eq;','!=');
    EqText :=IPPreProcessPowerSign(EqText);
    EqText :=IPProcessVarDirective(EqText,False);
    MakeGifEq;
  except
    on E:Exception do
    begin
      FIsError := True;
      FError := E.Message;
      MakeErrorGif;
    end;
  end;
end;

procedure TIPEqGifServer.MakeGifEq;
var
  Bm : TBitmap;
begin
  FEqDoc := TIPEQDocument.Create;
  FEqDoc.Clear;
  FEqDoc.InitFont;
  if FFontsize<>0 then FEQDoc.Font.Size := FFontsize;
  FEqDoc.ItalicizeVariables := True;
  if FInterpretset then
    FEqDoc.InterpretFunctions := FInterpretFunctions
  else
    FEqDoc.InterpretFunctions := True;
  FEQDoc.AlignByEq := FAlignByEq;

  Bm := TBitmap.Create;
  Bm.PixelFormat := pf32bit;

  try
    FEqDoc.Clear;
    FEqDoc.SetText(FEqtext);

    Bm.Height := FEqDoc.Height;
    Bm.Width := FEqDoc.Width;
    Bm.Canvas.Brush.Style := bsClear;

    FEqDoc.Paint(Bm.Canvas);
    //Bm.Canvas.MoveTo(0,FEqDoc.Ascent);
    //Bm.Canvas.LineTo(FEqDoc.Width,FEqDoc.Ascent);

    FBitmap.Assign(Bm);
  finally
    FEqDoc.Free;
    Bm.Free;
  end;
end;

procedure TIPEqGifServer.MakeGifExpr;
var
  Ex : TTDXExpression;
  Exr : TTDXExpr;
begin
  Exr := nil;
  Ex := TTDXExpression.Create;
  try
    Ex.ExprText := FExprText;
    Ex.ParseExpression;
    if not FDoNotReduce then
    begin
      Exr := Ex.Expr.ReducedExpr;
      Ex.ExprText := Exr.ExprText;
      Ex.ParseExpression;
    end;
    Eqtext := StringReplace(Ex.EqText,'&emptyop;','',[rfReplaceAll]);
    Eqtext := AnsiReplaceStr(Eqtext,'>=','&ge;');
    Eqtext := AnsiReplaceStr(Eqtext,'<=','&le;');
    Eqtext := AnsiReplaceStr(Eqtext,'<>','&ne;');
    Eqtext := AnsiReplaceStr(Eqtext,'!=','&ne;');
    MakeGifEq;
  finally
    Ex.Free;
    Exr.Free;
  end;
end;


procedure TIPEQGifServer.ProcessPadding;
var
 Bm : TBitmap;
 T,B : integer;
 R,R1,Rb : TRect;
 Pr : TPenRecall;
begin
  Bm := TBitmap.Create;
  try
    T := FToppadding;
    B := FBottompadding;

    Bm.Width := FBitmap.Width;
    Bm.Height := T + B + FBitmap.Height;

    R.TopLeft := Point(0,0);
    R.Right := FBitmap.Width;
    R.Bottom := FBitmap.Height;

    if FDoBorder then
    begin
      Rb := R;
      Rb.Right := Rb.Right + 8;
      Rb.Bottom := Rb.Bottom + 8;
      Bm.Width := Bm.Width + 8;
      Bm.Height := Bm.Height + 8;
      Pr := TPenRecall.Create(Bm.Canvas.Pen);
      try
        //It is run on server, client has different settings. Let's just do default border.

        Bm.Canvas.Pen.Color := $B99D7F;
        Bm.Canvas.Rectangle(rb);
      finally
        Pr.Free;
      end;
    end;

    R1 := R;

    if FDoBorder then
      OffsetRect(R1,4,4);

    R1.Top := R1.Top + T ;
    R1.Bottom := R1.Bottom + T;
    Bm.Canvas.CopyRect(R1,FBitmap.Canvas,R);

    FBBitmap.Assign(Bm);

  finally
    Bm.Free;
  end;
end;




procedure TIPEqGifServer.SetBottomPadding(const Value: Integer);
begin
  FBottomPadding := Value;
end;

procedure TIPEqGifServer.SetEqText(const Value: string);
begin
  FEqText := StringReplace(Value,'@INTEGRAL{}','@INTEGRAL{;}',[rfReplaceAll,rfIgnoreCase]);
  FEqText := StringReplace(FEqText,'@SUM{}','@SUM{;}',[rfReplaceAll,rfIgnoreCase]);
  //FEqText := Value;
end;

procedure TIPEqGifServer.SetExprText(const Value: String);
begin
  FExprText := Value;
end;

procedure TIPEqGifServer.SetInterpretFunctions(const Value: BOolean);
begin
  FInterpretFunctions := Value;
  FInterpretSet := True;
end;

procedure TIPEqGifServer.SetTopPadding(const Value: Integer);
begin
  FTopPadding := Value;
end;

procedure TIPEqGifServer.Set_Bottom(Value: Integer);
begin
  BottomPadding := Value;
end;

procedure TIPEqGifServer.Set_EqText(const Value: WideString);
begin
  Eqtext := StringReplace(Value,'&emptyop;','',[rfReplaceAll]);
end;

procedure TIPEqGifServer.Set_ExprText(const Value: WideString);
begin
  ExprText := StringReplace(Value,'&emptyop;','',[rfReplaceAll]);
end;

procedure TIPEqGifServer.Set_Top(Value: Integer);
begin
  TopPadding := Value;
end;

procedure TIPEqGifServer.SaveGifFile;
var
  Gif : TGifImage;
  CExt : TGifCommentExtension;
  SubImg : TGifSubImage;
begin
  Gif := TGifImage.Create;
  Gif.ColorReduction := rmNone; //!! Default is rmNetscape - 216 web colors
  try
    Gif.Assign(FBBitmap);
    SubImg := Gif.Images.Items[0] as TGifSubImage;
    CExt := TGifCommentExtension.Create(SubImg);
    CExt.Text.Text := FEqText;
    SubImg.Extensions.Add(CExt);
    SaveGif(Gif);
  finally
    Gif.Free;
  end;
end;


procedure TIPEqGifServer.SaveGif(Gif: TGifImage);
var FName:string;
begin
  FName := Path + FileName;
  Gif.SaveToFile(FName);
end;

procedure TIPEqGifServer.SetAlignByEq(const Value: Boolean);
begin
  FAlignByEq := Value;
end;

function TIPEqGifServer.Get_AlignByEq: WordBool;
begin
  Result := AlignByEq;
end;

function TIPEqGifServer.Get_Fontsize: Integer;
begin
  Result := Fontsize;
end;

function TIPEqGifServer.Get_InterpretFunctions: WordBool;
begin
  Result := InterpretFunctions;
end;

procedure TIPEqGifServer.Set_AlignByEq(Value: WordBool);
begin
  AlignByEq := Value;
end;

procedure TIPEqGifServer.Set_Fontsize(Value: Integer);
begin
  FontSize := Value;
end;

procedure TIPEqGifServer.Set_InterpretFunctions(Value: WordBool);
begin
  InterpretFunctions := Value;
end;

function TIPEqGifServer.Get_Path: WideString;
begin
  Result := Path;
end;

procedure TIPEqGifServer.Set_Path(const Value: WideString);
begin
  if not DirectoryExists(Value) then
    Path := DEFPATH
  else
    Path := Value;
end;

procedure TIPEqGifServer.Save;
begin
  InitObject;
  try
    MakeGif;
    ProcessPadding;
    SaveGifFile;
  finally
    FinalizeOBject;
  end;
end;

function TIPEqGifServer.GetPath: String;
begin
  if FPath = '' then
    Result := DEFPATH
  else
    Result := FPath;
end;

procedure TIPEqGifServer.SetPath(const Value: String);
begin
  FPath := Value;
end;

procedure TIPEqGifServer.SetFontsize(const Value: Integer);
begin
  FFontsize := Value;
end;

procedure TIPEqGifServer.FinalizeObject;
begin
  FBItmap.Free;
  FBBitmap.Free;
end;

procedure TIPEqGifServer.InitObject;
begin
  FBitmap := TBitmap.Create;
  FBitmap.PixelFormat := pf32bit;
  FBBitmap := TBitmap.Create;
  FBitmap.PixelFormat := pf32bit;
  FError := '';
  FIsError := False;
end;

procedure TIPEqGifServer.SetFileName(const Value: String);
begin
  FFileName := Value;
end;


function TIPEQGifServer.GetReducedExpr(ExprText:string):string;
var
  Ex : TTDXExpression;
  Exr : TTDXExpr;
  Eqtext : string;
begin
  Exr := nil;
  Ex := TTDXExpression.Create;
  try
    Ex.ExprText := ExprText;
    Ex.ParseExpression;

    Exr := Ex.Expr.ReducedExpr;

    Eqtext := AnsiReplaceStr(Exr.ExprText,'>=','&ge;');
    Eqtext := AnsiReplaceStr(Eqtext,'<=','&le;');
    Eqtext := AnsiReplaceStr(Eqtext,'<>','&ne;');
    Eqtext := AnsiReplaceStr(Eqtext,'!=','&ne;');

    Result := Eqtext;

  finally
    Ex.Free;
    Exr.Free;
  end;
end;



function TIPEqGifServer.Get_Filename: WideString;
begin
  Result := FFileName;
end;

procedure TIPEqGifServer.Set_Filename(const Value: WideString);
begin
  FFileName := Value;
end;

function TIPEqGifServer.Get_DoNotReduce: WordBool;
begin
  Result := FDoNotReduce;
end;

procedure TIPEqGifServer.Set_DoNotReduce(Value: WordBool);
begin
  FDoNotReduce := Value;
end;

procedure TIPEqGifServer.SetErrorDisplayText(const Value: String);
begin
  FErrorDisplayText := Value;
end;

function TIPEqGifServer.GetErrorDisplayText: String;
begin
  if FErrorDisplayText = '' then
    Result := DEFERROR
  else
    Result := FErrorDisplayText;
end;

function TIPEqGifServer.Get_Error: WideString;
begin
  Result := FError;
end;

function TIPEqGifServer.Get_ErrorDisplayText: WideString;
begin
  Result := ErrorDisplayText;
end;

function TIPEqGifServer.Get_IsError: WordBool;
begin
  Result := FIsError;
end;

procedure TIPEqGifServer.Set_ErrorDisplayText(const Value: WideString);
begin
  ErrorDisplayText := Value;
end;

procedure TIPEqGifServer.MakeErrorGif;
var
  Bm : TBitmap;
begin
  FeqDoc := TIPEQDocument.Create;
  Feqdoc.Clear;
  FeqDoc.InitFont;
  FEQDoc.Font.Size := FFontsize;
  FEqDoc.ItalicizeVariables := False;
  FEqDoc.InterpretFunctions := False;

  Bm := TBitmap.Create;

  try
    FeqDoc.Clear;
    FEqDoc.SetText(ErrorDisplayText);

    Bm.Height := FEqDoc.Height;
    Bm.Width := FEqDoc.Width;
    Bm.Canvas.Brush.Style := bsClear;

    FEqDoc.Paint(Bm.Canvas);
    FBitmap.Assign(Bm);
  finally
    FEqDoc.Free;
    Bm.Free;
  end;
end;

function TIPEqGifServer.GetInterpretFunctions: Boolean;
begin
  if not FInterpretSet then
  begin
    FInterpretFunctions := True;
    FInterpretSet := True;
  end;
  Result := FInterpretFunctions;
end;

function TIPEqGifServer.Get_DoBorder: WordBool;
begin
  Result := FDoBorder;
end;

procedure TIPEqGifServer.Set_DoBorder(Value: WordBool);
begin
  FDoBorder := Value;
end;

//--------------
function TIPEqGifServer.GetImageInfo: WideString;
begin
  try
  if FExprText <> '' then
    try
      EqText :=  FExprText;
      FExprText := IPPrepareExpressionForRendering(FExprText);
      Result := GetImageInfoExpr;
    except
      EqText :=IPPreProcessPowerSign(EqText);
      EqText :=IPProcessVarDirective(EqText,False);
      Result := GetImageInfoEq;
    end
  else
    EqText :=IPPreProcessPowerSign(EqText);
    EqText :=IPProcessVarDirective(EqText,False);
    Result := GetImageInfoEq;
  except
    on E:Exception do
    begin
      Result := DEFERROR;
    end;
  end;
end;

function TIPEqGifServer.GetImageInfoEq: WideString;
var
  Bm : TBitmap;
begin
  FEqDoc := TIPEQDocument.Create;
  FEqDoc.Clear;
  FEqDoc.InitFont;
  if FFontsize<>0 then FEQDoc.Font.Size := FFontsize;
  FEqDoc.ItalicizeVariables := True;
  if FInterpretset then
    FEqDoc.InterpretFunctions := FInterpretFunctions
  else
    FEqDoc.InterpretFunctions := True;
  FEQDoc.AlignByEq := FAlignByEq;

  Bm := TBitmap.Create;
  Bm.PixelFormat := pf32bit;

  try
    FEqDoc.Clear;
    FEqDoc.SetText(FEqtext);

    Bm.Height := FEqDoc.Height;
    Bm.Width := FEqDoc.Width;
    Bm.Canvas.Brush.Style := bsClear;

    FEqDoc.Paint(Bm.Canvas);
    Result := Format('baseline:%d;width:%d;height:%d',
      [FEqDoc.Ascent,FEqDoc.Width,FEqDoc.Height]);

    //Bm.Canvas.MoveTo(0,FEqDoc.Ascent);
    //Bm.Canvas.LineTo(FEqDoc.Width,FEqDoc.Ascent);

    //FBitmap.Assign(Bm);
  finally
    FEqDoc.Free;
    Bm.Free;
  end;
end;

function TIPEqGifServer.GetImageInfoExpr: WideString;
var
  Ex : TTDXExpression;
  Exr : TTDXExpr;
begin
  Exr := nil;
  Ex := TTDXExpression.Create;
  try
    Ex.ExprText := FExprText;
    Ex.ParseExpression;
    if not FDoNotReduce then
    begin
      Exr := Ex.Expr.ReducedExpr;
      Ex.ExprText := Exr.ExprText;
      Ex.ParseExpression;
    end;
    EqText := StringReplace(Ex.EqText,'&emptyop;','',[rfReplaceAll]);
    Result := GetImageInfoEq;
  finally
    Ex.Free;
    Exr.Free;
  end;
end;

function TIPEqGifServer.GetReduced(const ExprText: WideString): WideString;
var
  Ex : TTDXExpression;
  Exr : TTDXExpr;
  Eqtext : string;
begin
  InitLocalInfo;
  Exr := nil;
  Ex := TTDXExpression.Create;
  try
  try
    Ex.ExprText := IPPrepareExpressionForRendering(ExprText);
    Ex.ParseExpression;

    Exr := Ex.Expr.ReducedExpr;
    Eqtext := AnsiReplaceStr(Exr.ExprText,'>=','&ge;');
    Eqtext := AnsiReplaceStr(Eqtext,'<=','&le;');
    Eqtext := AnsiReplaceStr(Eqtext,'<>','&ne;');
    Eqtext := AnsiReplaceStr(Eqtext,'!=','&ne;');

    Result := Eqtext;
  except
    on E:Exception do
    begin
      FIsError := True;
      FError := E.Message;
      Result := 'ERROR!';
    end;
  end;
  finally
    Ex.Free;
    Exr.Free;
  end;
end;


initialization
  TAutoObjectFactory.Create(ComServer, TIPEqGifServer, Class_IPEqGifServer,
    ciMultiInstance, tmApartment);
end.

