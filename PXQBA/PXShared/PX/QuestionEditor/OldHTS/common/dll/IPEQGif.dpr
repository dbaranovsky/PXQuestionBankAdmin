library IPEQGif;

uses
  ComServ,
  IPEQGif_TLB in 'IPEQGif_TLB.pas',
  ipeqgifserver in 'ipeqgifserver.pas' {IPEqGifServer: CoClass},
  IPEqNode in 'IPEqNode.pas',
  IPEqUtils in 'IPEqUtils.pas',
  FormEqInfo in 'FormEqInfo.pas' {frmEqInfo},
  IPCBrace in 'IPCBrace.pas',
  IPEqABS in 'IPEqABS.pas',
  IPEqBar in 'IPEqBar.pas',
  IPEqBigger in 'IPEqBigger.pas',
  IPEqBrace in 'IPEqBrace.pas',
  IPEqCBrace in 'IPEqCBrace.pas',
  IPEqChar in 'IPEqChar.pas',
  IPEqCheck in 'IPEqCheck.pas',
  IPEqCIS in 'IPEqCIS.pas',
  IPEqComposite in 'IPEqComposite.pas',
  IPEqDivide in 'IPEqDivide.pas',
  IPEqEditor in 'IPEqEditor.pas',
  IPEqIntegral in 'IPEqIntegral.pas',
  IPEqItalic in 'IPEqItalic.pas',
  IPEqLabel in 'IPEqLabel.pas',
  IPEqLDiv in 'IPEqLDiv.pas',
  IPEqMat in 'IPEqMat.pas',
  IPEqMNum in 'IPEqMNum.pas',
  IPEqObject in 'IPEqObject.pas',
  IPEqOp in 'IPEqOp.pas',
  IPEqOverUnder in 'IPEqOverUnder.pas',
  IPEqParen in 'IPEqParen.pas',
  IPEqPrime in 'IPEqPrime.pas',
  IPEqSqrt in 'IPEqSqrt.pas',
  IPEqStack in 'IPEqStack.pas',
  IPEqSum in 'IPEqSum.pas',
  IPEqSuperScript in 'IPEqSuperScript.pas',
  IPEqSupSub in 'IPEqSupSub.pas',
  IPEqSymbolW in 'IPEqSymbolW.pas',
  IPEqText in 'IPEqText.pas',
  IPEqTextParser in 'IPEqTextParser.pas',
  VarCmplx in 'VarCmplx.pas',
  TDXExpr in 'TDXExpr.pas',
  TDXExpression in 'TDXExpression.pas',
  TDXExprFunction in 'TDXExprFunction.pas',
  TDXExprMath in 'TDXExprMath.pas',
  TDXExprModel in 'TDXExprModel.pas',
  TDXExprObject in 'TDXExprObject.pas',
  TDXExprParser in 'TDXExprParser.PAS',
  TDXExprParserX in 'TDXExprParserX.pas',
  TDXExprReduce in 'TDXExprReduce.pas',
  TDXExprSimEq in 'TDXExprSimEq.pas',
  TDXExprStringLiteral in 'TDXExprStringLiteral.pas',
  TDXExprSymbolicVar in 'TDXExprSymbolicVar.pas',
  TDXExprVariable in 'TDXExprVariable.pas',
  TDXOrderedPairVar in 'TDXOrderedPairVar.pas',
  TDXVar in 'TDXVar.pas',
  Graphics in 'Graphics.pas',
  IPEqParser in 'IPEqParser.PAS',
  CocoBase in 'CocoBase.pas',
  mwStringHashList in 'mwStringHashList.pas',
  IPEqPlainText in 'IPEqPlainText.pas';

{$E ocx}

exports
  DllGetClassObject,
  DllCanUnloadNow,
  DllRegisterServer,
  DllUnregisterServer;

{$R *.TLB}

{$R *.RES}

begin
end.
