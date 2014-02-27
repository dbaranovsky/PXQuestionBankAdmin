library IPEQEval;

uses
  ComServ,
  IPEQEval_TLB in 'IPEQEval_TLB.pas',
  IpEqEvaluator in 'IpEqEvaluator.pas' {IpEqEvaluator: CoClass},
  TDXEvaluator in 'TDXEvaluator.pas',
  IPEqTextParser in 'IPEqTextParser.pas',
  TDXExpr in 'TDXExpr.pas',
  Variants in 'Variants.pas',
  TDXStrings in 'TDXStrings.pas',
  TDXExprParser in 'TDXExprParser.PAS',
  VarCmplx in 'VarCmplx.pas';

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
