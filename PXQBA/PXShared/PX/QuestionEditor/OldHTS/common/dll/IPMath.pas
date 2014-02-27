unit IPMath;

interface

uses Variants;

type
  TPrimeList = array of Integer;
// dynarray used to return a list of Primes like the factor list

// Prime #'s < 1000 (first 200)
const
  NUM_PRIMES = 168+125*4+1+140*4; //1129
  PRIME_TABLE: array[0..NUM_PRIMES-1] of Integer =
                            (2,3,5,7,11,13,17,19,23,29,31,37,41,
                            43,47,53,59,61,67,71,73,79,83,89,97,
                            101,103,107,109,113,127,131,137,139,
                            149,151,157,163,167,173,179,181,191,
                            193,197,199,211,223,227,229,233,239,
                            241,251,257,263,269,271,277,281,283,
                            293,307,311,313,317,331,337,347,349,
                            353,359,367,373,379,383,389,397,401,
                            409,419,421,431,433,439,443,449,457,
                            461,463,467,479,487,491,499,503,509,
                            521,523,541,547,557,563,569,571,577,
                            587,593,599,601,607,613,617,619,631,
                            641,643,647,653,659,661,673,677,683,
                            691,701,709,719,727,733,739,743,751,
                            757,761,769,773,787,797,809,811,821,
                            823,827,829,839,853,857,859,863,877,
                            881,883,887,907,911,919,929,937,941,
                            947,953,967,971,977,983,991,997,
                            1009,     1013,     1019,     1021,
                            1031,     1033,     1039,     1049,
                            1051,     1061,     1063,     1069,
                            1087,     1091,     1093,     1097,
                            1103,     1109,     1117,     1123,
                            1129,     1151,     1153,     1163,
                            1171,     1181,     1187,     1193,
                            1201,     1213,     1217,     1223,
                            1229,     1231,     1237,     1249,
                            1259,     1277,     1279,     1283,
                            1289,     1291,     1297,     1301,
                            1303,     1307,     1319,     1321,
                            1327,     1361,     1367,     1373,
                            1381,     1399,     1409,     1423,
                            1427,     1429,     1433,     1439,
                            1447,     1451,     1453,     1459,
                            1471,     1481,     1483,     1487,
                            1489,     1493,     1499,     1511,
                            1523,     1531,     1543,     1549,
                            1553,     1559,     1567,     1571,
                            1579,     1583,     1597,     1601,
                            1607,     1609,     1613,     1619,
                            1621,     1627,     1637,     1657,
                            1663,     1667,     1669,     1693,
                            1697,     1699,     1709,     1721,
                            1723,     1733,     1741,     1747,
                            1753,     1759,     1777,     1783,
                            1787,     1789,     1801,     1811,
                            1823,     1831,     1847,     1861,
                            1867,     1871,     1873,     1877,
                            1879,     1889,     1901,     1907,
                            1913,     1931,     1933,     1949,
                            1951,     1973,     1979,     1987,
                            1993,     1997,     1999,     2003,
                            2011,     2017,     2027,     2029,
                            2039,     2053,     2063,     2069,
                            2081,     2083,     2087,     2089,
                            2099,     2111,     2113,     2129,
                            2131,     2137,     2141,     2143,
                            2153,     2161,     2179,     2203,
                            2207,     2213,     2221,     2237,
                            2239,     2243,     2251,     2267,
                            2269,     2273,     2281,     2287,
                            2293,     2297,     2309,     2311,
                            2333,     2339,     2341,     2347,
                            2351,     2357,     2371,     2377,
                            2381,     2383,     2389,     2393,
                            2399,     2411,     2417,     2423,
                            2437,     2441,     2447,     2459,
                            2467,     2473,     2477,     2503,
                            2521,     2531,     2539,     2543,
                            2549,     2551,     2557,     2579,
                            2591,     2593,     2609,     2617,
                            2621,     2633,     2647,     2657,
                            2659,     2663,     2671,     2677,
                            2683,     2687,     2689,     2693,
                            2699,     2707,     2711,     2713,
                            2719,     2729,     2731,     2741,
                            2749,     2753,     2767,     2777,
                            2789,     2791,     2797,     2801,
                            2803,     2819,     2833,     2837,
                            2843,     2851,     2857,     2861,
                            2879,     2887,     2897,     2903,
                            2909,     2917,     2927,     2939,
                            2953,     2957,     2963,     2969,
                            2971,     2999,     3001,     3011,
                            3019,     3023,     3037,     3041,
                            3049,     3061,     3067,     3079,
                            3083,     3089,     3109,     3119,
                            3121,     3137,     3163,     3167,
                            3169,     3181,     3187,     3191,
                            3203,     3209,     3217,     3221,
                            3229,     3251,     3253,     3257,
                            3259,     3271,     3299,     3301,
                            3307,     3313,     3319,     3323,
                            3329,     3331,     3343,     3347,
                            3359,     3361,     3371,     3373,
                            3389,     3391,     3407,     3413,
                            3433,     3449,     3457,     3461,
                            3463,     3467,     3469,     3491,
                            3499,     3511,     3517,     3527,
                            3529,     3533,     3539,     3541,
                            3547,     3557,     3559,     3571,
                            3581,     3583,     3593,     3607,
                            3613,     3617,     3623,     3631,
                            3637,     3643,     3659,     3671,
                            3673,     3677,     3691,     3697,
                            3701,     3709,     3719,     3727,
                            3733,     3739,     3761,     3767,
                            3769,     3779,     3793,     3797,
                            3803,     3821,     3823,     3833,
                            3847,     3851,     3853,     3863,
                            3877,     3881,     3889,     3907,
                            3911,     3917,     3919,     3923,
                            3929,     3931,     3943,     3947,
                            3967,     3989,     4001,     4003,
                            4007,     4013,     4019,     4021,
                            4027,     4049,     4051,     4057,
                            4073,     4079,     4091,     4093,
                            4099,     4111,     4127,     4129,
                            4133,     4139,     4153,     4157,
                            4159,     4177,     4201,     4211,
                            4217,     4219,     4229,     4231,
                            4241,     4243,     4253,     4259,
                            4261,     4271,     4273,     4283,
                            4289,     4297,     4327,     4337,
                            4339,     4349,     4357,     4363,
                            4373,     4391,     4397,     4409,
                            4421,     4423,     4441,     4447,
                            4451,     4457,     4463,     4481,
                            4483,     4493,     4507,     4513,
                            4517,     4519,     4523,     4547,
                            4549,     4561,     4567,     4583,
                            4591,     4597,     4603,     4621,
                            4637,     4639,     4643,     4649,
                            4651,     4657,     4663,     4673,
                            4679,     4691,     4703,     4721,
                            4723,     4729,     4733,     4751,
                            4759,     4783,     4787,     4789,
                            4793,     4799,     4801,     4813,
                            4817,     4831,     4861,     4871,
                            4877,     4889,     4903,     4909,
                            4919,     4931,     4933,     4937,
                            4943,     4951,     4957,     4967,
                            4969,     4973,     4987,     4993,
                            4999,
                            5003,     5009,     5011,     5021,
                            5023,     5039,     5051,     5059,
                            5077,     5081,     5087,     5099,
                            5101,     5107,     5113,     5119,
                            5147,     5153,     5167,     5171,
                            5179,     5189,     5197,     5209,
                            5227,     5231,     5233,     5237,
                            5261,     5273,     5279,     5281,
                            5297,     5303,     5309,     5323,
                            5333,     5347,     5351,     5381,
                            5387,     5393,     5399,     5407,
                            5413,     5417,     5419,     5431,
                            5437,     5441,     5443,     5449,
                            5471,     5477,     5479,     5483,
                            5501,     5503,     5507,     5519,
                            5521,     5527,     5531,     5557,
                            5563,     5569,     5573,     5581,
                            5591,     5623,     5639,     5641,
                            5647,     5651,     5653,     5657,
                            5659,     5669,     5683,     5689,
                            5693,     5701,     5711,     5717,
                            5737,     5741,     5743,     5749,
                            5779,     5783,     5791,     5801,
                            5807,     5813,     5821,     5827,
                            5839,     5843,     5849,     5851,
                            5857,     5861,     5867,     5869,
                            5879,     5881,     5897,     5903,
                            5923,     5927,     5939,     5953,
                            5981,     5987,     6007,     6011,
                            6029,     6037,     6043,     6047,
                            6053,     6067,     6073,     6079,
                            6089,     6091,     6101,     6113,
                            6121,     6131,     6133,     6143,
                            6151,     6163,     6173,     6197,
                            6199,     6203,     6211,     6217,
                            6221,     6229,     6247,     6257,
                            6263,     6269,     6271,     6277,
                            6287,     6299,     6301,     6311,
                            6317,     6323,     6329,     6337,
                            6343,     6353,     6359,     6361,
                            6367,     6373,     6379,     6389,
                            6397,     6421,     6427,     6449,
                            6451,     6469,     6473,     6481,
                            6491,     6521,     6529,     6547,
                            6551,     6553,     6563,     6569,
                            6571,     6577,     6581,     6599,
                            6607,     6619,     6637,     6653,
                            6659,     6661,     6673,     6679,
                            6689,     6691,     6701,     6703,
                            6709,     6719,     6733,     6737,
                            6761,     6763,     6779,     6781,
                            6791,     6793,     6803,     6823,
                            6827,     6829,     6833,     6841,
                            6857,     6863,     6869,     6871,
                            6883,     6899,     6907,     6911,
                            6917,     6947,     6949,     6959,
                            6961,     6967,     6971,     6977,
                            6983,     6991,     6997,     7001,
                            7013,     7019,     7027,     7039,
                            7043,     7057,     7069,     7079,
                            7103,     7109,     7121,     7127,
                            7129,     7151,     7159,     7177,
                            7187,     7193,     7207,     7211,
                            7213,     7219,     7229,     7237,
                            7243,     7247,     7253,     7283,
                            7297,     7307,     7309,     7321,
                            7331,     7333,     7349,     7351,
                            7369,     7393,     7411,     7417,
                            7433,     7451,     7457,     7459,
                            7477,     7481,     7487,     7489,
                            7499,     7507,     7517,     7523,
                            7529,     7537,     7541,     7547,
                            7549,     7559,     7561,     7573,
                            7577,     7583,     7589,     7591,
                            7603,     7607,     7621,     7639,
                            7643,     7649,     7669,     7673,
                            7681,     7687,     7691,     7699,
                            7703,     7717,     7723,     7727,
                            7741,     7753,     7757,     7759,
                            7789,     7793,     7817,     7823,
                            7829,     7841,     7853,     7867,
                            7873,     7877,     7879,     7883,
                            7901,     7907,     7919,     7927,
                            7933,     7937,     7949,     7951,
                            7963,     7993,     8009,     8011,
                            8017,     8039,     8053,     8059,
                            8069,     8081,     8087,     8089,
                            8093,     8101,     8111,     8117,
                            8123,     8147,     8161,     8167,
                            8171,     8179,     8191,     8209,
                            8219,     8221,     8231,     8233,
                            8237,     8243,     8263,     8269,
                            8273,     8287,     8291,     8293,
                            8297,     8311,     8317,     8329,
                            8353,     8363,     8369,     8377,
                            8387,     8389,     8419,     8423,
                            8429,     8431,     8443,     8447,
                            8461,     8467,     8501,     8513,
                            8521,     8527,     8537,     8539,
                            8543,     8563,     8573,     8581,
                            8597,     8599,     8609,     8623,
                            8627,     8629,     8641,     8647,
                            8663,     8669,     8677,     8681,
                            8689,     8693,     8699,     8707,
                            8713,     8719,     8731,     8737,
                            8741,     8747,     8753,     8761,
                            8779,     8783,     8803,     8807,
                            8819,     8821,     8831,     8837,
                            8839,     8849,     8861,     8863,
                            8867,     8887,     8893,     8923,
                            8929,     8933,     8941,     8951,
                            8963,     8969,     8971,     8999,
                            9001,     9007,     9011,     9013,
                            9029,     9041,     9043,     9049,
                            9059,     9067,     9091,     9103,
                            9109,     9127,     9133,     9137,
                            9151,     9157,     9161,     9173,
                            9181,     9187,     9199,     9203,
                            9209,     9221,     9227,     9239,
                            9241,     9257,     9277,     9281,
                            9283,     9293,     9311,     9319,
                            9323,     9337,     9341,     9343,
                            9349,     9371,     9377,     9391,
                            9397,     9403,     9413,     9419,
                            9421,     9431,     9433,     9437,
                            9439,     9461,     9463,     9467,
                            9473,     9479,     9491,     9497,
                            9511,     9521,     9533,     9539,
                            9547,     9551,     9587,     9601,
                            9613,     9619,     9623,     9629,
                            9631,     9643,     9649,     9661,
                            9677,     9679,     9689,     9697,
                            9719,     9721,     9733,     9739,
                            9743,     9749,     9767,     9769,
                            9781,     9787,     9791,     9803,
                            9811,     9817,     9829,     9833,
                            9839,     9851,     9857,     9859,
                            9871,     9883,     9887,     9901,
                            9907,     9923,     9929,     9931,
                            9941,     9949,     9967,     9973);

  MAX_FACTOR_LIST_SIZE = 100;
  FACTORIAL_MAX = 65;

  MAX_PRIME : Integer = 9973;

  POINT5 : Extended = 0.5;

function IPRound(Value:Extended):Integer;
function IPRoundEx(Num1: Extended; Places: Integer): Extended;
function IPTruncEx(Num1: Extended; Places: Integer): Extended;
function Factorial(Num: Extended): Extended;
function RelativePrime(RelNum,Min,Max:Integer; ResultPrime:boolean; ExclList:TPrimeList):Integer;
function GetFactors(N: Integer; var FactorList: TPrimeList): Integer;
function GCF(Num1,Num2:Extended):Integer;
function LCM(Num1,Num2:Extended):Extended;
function GreatestCommonSquare(A: Extended): Extended;
function GreatestCommonSquareRem(A: Extended): Extended;
procedure ReduceFraction(var Num,Denom:Extended);
function Perm(N,R: Extended): Extended;
function Comb(N,R: Extended): Extended;
function SimpAB(Num1,Num2: Extended): Extended;

function IsFloatEqual(Num1,Num2:Extended;Tolerance:Extended=0):boolean;
function isVariantEqual(V1,V2:Variant):boolean;
function NormalCurve(Mu,Sd,X:Extended):Extended;
procedure SortNumbers(var A : array of Double; Descending:Boolean=false);
Function GetEPSilon(Value:Extended):Extended;
function NormalDist(X:Extended):Extended;

implementation

uses Math,Classes,Sysutils;

function IsFloatEqual(Num1,Num2:Extended;Tolerance:Extended=0):boolean;
const
  EPS = 1e-8;
var
  E : Extended;
begin
  if IsInfinite(Num1) or IsInfinite(Num2) then
  begin
    if IsInfinite(Num1) and IsInfinite(Num2) and (Num1*Num2 > 0) then
      Result := true
    else
      Result := false;
  end
  else if Tolerance = 0 then
  begin
    E := Max(Min(Num1,Num2)*EPS,EPS);
    Result := SameValue(Num1,Num2,E);
  end
  else
    Result := SameValue(Num1,Num2,Tolerance);
end;

function isVariantEqual(V1,V2:Variant):boolean;
var
  X1,X2 : Extended;
begin
  if VarIsFloat(V1) and VarIsFLoat(V2) then
  begin
     X1 := V1;
     X2 := V2;
     Result := IsFloatEqual(X1,X2,0);
  end
  else
    Result := V1 = V2;
end;

Function GetEPSilon(Value:Extended):Extended;
var
  E : Extended;
begin
  if Abs(Value) < 1e-10 then
    Result := 1e-10
  else
  begin
    E := Log10(Abs(Value));
    E := IntPower(10,Ceil(E)-14);
    Result := E;
  end;
end;

Function IPRound(Value:Extended):Integer;
begin
  if Value >= 0.0 then
    Result := Trunc(Value+POINT5+GetEPSilon(Value))
  else
    Result := Trunc(Value-(POINT5+GetEPSilon(Value)));
end;

function IPRoundEx(Num1: Extended; Places: Integer): Extended;
var
  RoundPos: Extended;
  Num2 : Extended;
begin
  RoundPos := IntPower(10,Places);

  Num2 := Num1/RoundPos;

  if (Num1 >= 0) then
    Result := Trunc((Num2 + POINT5 + GetEPSilon(Num2)))
  else
    Result := Trunc(-(Abs(Num2) + POINT5 + GetEPSilon(Num2)));

  Result := Result * RoundPos;
end;

function IPTruncEx(Num1: Extended; Places: Integer): Extended;
var
  RoundPos: Extended;
begin
  RoundPos := IntPower(10,Places);

  Result := Trunc(Num1 / RoundPos);
  Result := Result * RoundPos;
end;

function Factorial(Num: Extended): Extended;
var
  I, Ind: Integer;
begin

  Ind := Round(Num);

  if (Ind < 0) or (Ind > FACTORIAL_MAX) then
    raise Exception.Create('Factorial parameter out of range.');

  Result := 1.0;

  for I := 1 to Ind do
    Result := Result*I;

end;

function GCF(Num1,Num2:Extended):Integer;
var
  N1,N2 : Integer;
  M,Temp:Integer;
const
  //EPS = 1e-9;
  EPS = 1e-5;
begin
  //First check to make sure numbers are actually integers
  N1 := Round(Num1);
  N2 := Round(Num2);
  //if not ( SameValue(N1,Num1,EPS) and SameValue(N2,Num2,EPS)) or
  //         (N1 = 0) or (N2 = 0) then
  if  (N1 = 0) or (N2 = 0) then
  begin
    Result := 0;
    Exit;
  end;

  if not ( SameValue(N1,Num1,EPS) and SameValue(N2,Num2,EPS)) then
  begin
    Result := 1;
    Exit;
  end;

  N1 := Abs(N1);
  N2 := Abs(N2);

  if (N1 < N2) then
  begin
    Temp := N1;
    N1 := N2;
    N2 := Temp;
  end;

  while (N2 <> 0) do
  begin
    M := N1 mod N2;
    N1 := N2;
    N2 := M;
  end;

  Result := N1;

end;

function LCM(Num1,Num2: Extended): Extended;
begin
  Result := Abs( (Num1 * Num2) / GCF(Num1,Num2) );
end;

{ Permutations }
function Perm(N,R: Extended): Extended;
begin
  Result := Factorial(N) / Factorial(N-R);
end;

{ Combinations }
function Comb(N,R: Extended): Extended;
begin
  Result := Factorial(N) / (Factorial(N-R) * Factorial(R));
end;

function SimpAB(Num1,Num2: Extended): Extended;
var
  GCFVal: Integer;
begin
  GCFVal := GCF(Num1,Num2);
  if GCFVal<>0 then
    Result := Num1/GCFVal
  else
    Result := Num1;
end;

function GetFactors(N: Integer; var FactorList: TPrimeList): Integer;
var
  PrimePos, FactorPos: Integer;
begin

  if N = 0 then
  begin
    SetLength(FactorList,0);
    Result := 0;
    Exit;
  end;

  // Make sure "n" is positive.
  N := Abs(N);


  SetLength(FactorList,MAX_FACTOR_LIST_SIZE);

  PrimePos := 0;
  FactorPos := 0;

  while (N > PRIME_TABLE[PrimePos])
     and(FactorPos < MAX_FACTOR_LIST_SIZE) and (PrimePos < NUM_PRIMES)
  do
  begin
    if ((N mod PRIME_TABLE[PrimePos]) = 0) then
    begin
      FactorList[FactorPos] := PRIME_TABLE[PrimePos];
      Inc(FactorPos);
      N := n div PRIME_TABLE[PrimePos];
    end
    else
      Inc(PrimePos);
  end;

  if (N <> 1) then
  begin
    FactorList[FactorPos] := PRIME_TABLE[PrimePos];
    Inc(FactorPos);
  end;

  SetLength(FactorList,FactorPos);
  Result := FactorPos;
end;

function GetTrunc(A:Extended):integer;
const
  E :extended = 1e-8;
begin
  if Abs(round(A)-A) < E then
    Result := round(A)
  else
    Result := trunc(A);    
end;

function GreatestCommonSquare(A: Extended): Extended;
var
  FactorA: TPrimeList;
  NumFactorsA: Integer;
  SquareValue, AInd: Integer;
begin

  SquareValue := 1;

  // Make sure "A" is positive.
  A := Abs(A);

  // Prime list is was generated to 1000.
  // Now to 10000. And this may be sqrt(A) C2770 DV 2/25/05

  if (A > 100000000) then
  begin
    Result := 0;
    Exit;
  end;

  NumFactorsA := GetFactors(GetTrunc(A),FactorA);

  for AInd:=1 to NumFactorsA-1 do
  begin
    if (FactorA[AInd-1] = FactorA[AInd]) then
    begin
      SquareValue := SquareValue * FactorA[AInd-1];
      FactorA[AInd] := 0;
      FactorA[AInd-1] := 0;
    end
  end;

  Result := SquareValue;
end;

function GreatestCommonSquareRem(A: Extended): Extended;
var
  Val: Extended;
begin
  Val := GreatestCommonSquare(A);
  Result := (A / (Val*Val));
end;

procedure ReduceFraction(Var Num,Denom:Extended);
var
  GCFVal: Integer;
begin
  if Denom = 0 then
    Exit;

  GCFVal := GCF(Num,Denom);
  if GCFVal <> 0 then
  begin
    Num := Num /GCFVal;
    Denom := Denom /GCFVal;
  end;
end;

function RelativePrime(RelNum,Min,Max:Integer; ResultPrime:boolean; ExclList:TPrimeList):Integer;
var
  Factors : TPrimeList;
  I,J : Integer;
  PList1 : TList;
  PList2 : TList;
  PList : TList;
  V : Integer;

  function IsValid(Val:Integer; AcceptOne:Boolean = False):boolean;
  var
   Ix : Integer;
  begin

    Result := (Val >= Min) and (Val <= Max);
    if not AcceptOne then
      Result := Result and (Val <> 1);

    if Result then
    begin
      for Ix := Low(ExclList) to High(ExclList) do
      begin
        if Val = ExclList[Ix] then
        begin
          Result := false;
          Exit;
        end;
      end;
    end;
  end;

  function InFactors(Num:Integer):boolean;
  var
    INum : Integer;
  begin
    for INum := Low(Factors) to High(Factors) do
    begin
      if Factors[INum] = Num then
      begin
        Result := true;
        Exit;
      end;
    end;
    Result := false;
  end;

begin
  Result := 0; //Compiler messages cleanup 3/1/05
  //Get Prime Factors for RelNum;
  GetFactors(RelNum,Factors);

  PList  := TList.Create;
  PList1 := TList.Create;
  PList2 := TList.Create;
  try
    I := 0;
    PList.Add(Pointer(1));
    while (i < NUM_PRIMES) and (PRIME_TABLE[I] <= Max) do
    begin
      if not InFactors(PRIME_TABLE[I]) then
        PList.Add(Pointer(PRIME_TABLE[I]));
      Inc(i);
    end;

    if PList.Count = 0 then
      raise Exception.CreateFmt('No Prime numbers found <= %d that are not Factors of %d',[Max,RelNum]);

    PList1.Assign(PList);

    while PList1.Count > 0 do
    begin
      I := RandomRange(0,PList1.Count);
      if ResultPrime then
      begin
        V := Integer(PList1[I]);
        if IsValid(V) then
        begin
          Result := V;
          Exit;
        end;
      end
      else
      begin
        PList2.Assign(PList);
        while PList2.Count > 0 do
        begin
          J := RandomRange(0,PList2.Count);
          V := Integer(PList1[I])*Integer(PList2[J]);
          if IsValid(V,True)  then //DV C7823 10/30/06 added v=1 as acceptable relative Prime
          begin
            Result := V;
            Exit;
          end;
          PList2.Delete(J);
        end;
      end;
      PList1.Delete(I);
    end;

    raise Exception.Create('Could not fInd a relative Prime given the constraints.');

  finally
    PList.Free;
    PList1.Free;
    PList2.Free;
  end;

end;

function NormalDist(X:Extended):Extended;
var
  X2 : Extended;
begin
  X2 := X * X;
  Result := 0.5+SIGN(X)*0.5*Sqrt(1-(1/30)*(7*EXP(-(X2)/2)+16*EXP(-(X2)*(2-SQRT(2)))+(7+PI/4*X2)*EXP(-(X2))))
end;

function NormalCurve(Mu,Sd,X:Extended):Extended;
var
 A,B : Extended;
begin
  A := (1.0/(Sd*Sqrt(2*pi)));
  B := Exp(-Sqr(X-Mu)/(2*Sqr(Sd)));
  Result := A*B;

//  1/(12*sqrt(2*pi))*(e^-((X-60)^2/(2*(12)^2)))
end;

procedure SortNumbers(Var A : array of Double; Descending:Boolean=false);
var
  Bis, I, J, K: LongInt;
  H: double;
begin
  Bis := High(A);
  K := Bis shr 1;// div 2
  while K > 0 do
  begin
    for I := 0 to bis - k do
    begin
      J := I;
      while (J >= 0) and ((Not Descending and (A[J] > a[J + K])) or (Descending and (A[J] < A[J+K]))) do
      begin
        H := A[J];
        A[J] := A[J + K];
        A[J + K] := H;
        if J > K then
          Dec(J, K)
        else
          J := 0;
      end; // {end while]
    end; // { end for}
    k := k shr 1; 
  end;  // {end while}

end;


end.
