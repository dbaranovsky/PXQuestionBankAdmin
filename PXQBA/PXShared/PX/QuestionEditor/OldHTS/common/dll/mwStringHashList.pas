{-----------------------------------------------------------------------------
The contents of this file are subject to the Mozilla Public License Version
1.1 (the "License"); you may not use this file except in compliance with the
License. You may obtain a copy of the License at
http://www.mozilla.org/NPL/NPL-1_1Final.html

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

The Original Code is: MWStringHashList.pas, released December 18, 2000.

The Initial Developer of the Original Code is Martin Waldenburg
(Martin.Waldenburg@T-Online.de).
Portions created by Martin Waldenburg are Copyright (C) 2000 Martin Waldenburg.
All Rights Reserved.

Contributor(s): ___________________.

Last Modified: 1/2/2001
Current Version: 2.0

Notes: This is a very fast Hash list for strings.

Known Issues:
-----------------------------------------------------------------------------}
{$R-}
unit MWStringHashList;

interface

uses Classes, SysUtils, Windows;

var
  MWHashTable: array[#0..#255] of byte;
  MWInsensitiveHashTable: array[#0..#255] of Byte;

type
  TMWStringHash = function(const AString: string): Integer;
  TMWStringHashCompare = function(const Str1: string; const Str2: string): Boolean;

  TMWHashWord = class
    S: string;
    Id: Integer;
    ExID: Integer;
    constructor Create(AString: string; AnId, AnExId: Integer);
  end;

  PHashPointerList = ^THashPointerList;
  THashPointerList = array[1..1] of TObject;

  TMWBaseStringHashList = class(TObject)
    FList: PHashPointerList;
    FCapacity: Integer;
  protected
    FHash: TMWStringHash;
    function Get(Index: Integer): Pointer;
    procedure Put(Index: Integer; Item: Pointer);
    procedure SetCapacity(NewCapacity: Integer);
  public
    destructor Destroy; override;
    procedure Clear;
    property Capacity: Integer read FCapacity;
    property Items[Index: Integer]: Pointer read Get write Put; default;
  end;

  TMWHashStrings = class(TMWBaseStringHashList)
  public
    procedure AddString(AString: string; AnId, AnExId: Integer);
  end;

  TMWHashItems = class(TMWBaseStringHashList)
  public
    constructor Create(AHash: TMWStringHash);
    procedure AddString(AString: string; AnId, AnExId: Integer);
  end;

  TMWStringHashList = class(TMWBaseStringHashList)
  private
    FSecondaryHash: TMWStringHash;
    FCompare: TMWStringHashCompare;
  public
    constructor Create(Primary, Secondary: TMWStringHash; ACompare: TMWStringHashCompare);
    procedure AddString(AString: string; AnId, AnExId: Integer);
    function Hash(const S: string; var AnId: Integer; var AnExId: Integer): Boolean;
    function HashEX(const S: string; var AnId: Integer; var AnExId: Integer; HashValue: Integer): Boolean;
  end;

function CrcHash(const AString: string): integer;
function ICrcHash(const AString: string): integer;
function SmallCrcHash(const AString: string): integer;
function ISmallCrcHash(const AString: string): integer;
function TinyHash(const AString: string): Integer;
function ITinyHash(const AString: string): Integer;
function HashCompare(const Str1: string; const Str2: string): Boolean;
function IHashCompare(const Str1: string; const Str2: string): Boolean;

function HashSecondaryOne(const AString: string): Integer;
function HashSecondaryTwo(const AString: string): Integer;

procedure InitTables;

implementation

procedure InitTables;
var
  I, K: Char;
  Temp: Byte;
begin
  for I := #0 to #255 do
  begin
    MWHashTable[I] := Ord(I);
  end;
  RandSeed := 255;
  for I := #1 to #255 do
  begin
    repeat
      K := Char(Random(255));
    until K <> #0;
    Temp := MWHashTable[I];
    MWHashTable[I] := MWHashTable[K];
    MWHashTable[K] := Temp;
  end;
  for I := #0 to #255 do
    MWInsensitiveHashTable[I] := MWHashTable[AnsiLowerCase(string(I))[1]];
end;

{ based on a Hasch function by Cyrille de Brebisson }

function CrcHash(const AString: string): integer;
var
  I: Integer;
begin
  Result := 0;
  for i := 1 to length(AString) do
  begin
    Result := (Result shr 4) xor (((Result xor MWHashTable[AString[I]]) and $F) * $1000);
    Result := (Result shr 4) xor (((Result xor (ord(MWHashTable[AString[I]]) shr 4)) and $F) * $1000);
  end;
  if Result = 0 then Result := Length(AString) mod 8 + 1;
end;

function ICrcHash(const AString: string): integer;
var
  I: Integer;
begin
  Result := 0;
  for i := 1 to length(AString) do
  begin
    Result := (Result shr 4) xor (((Result xor MWInsensitiveHashTable[AString[I]]) and $F) * $1000);
    Result := (Result shr 4) xor (((Result xor (ord(MWInsensitiveHashTable[AString[I]]) shr 4)) and $F) * $1000);
  end;
  if Result = 0 then Result := Length(AString) mod 8 + 1;
end;

function SmallCrcHash(const AString: string): integer;
var
  I: Integer;
begin
  Result := 0;
  for I := 1 to length(AString) do
  begin
    Result := (Result shr 4) xor (((Result xor MWHashTable[AString[I]]) and $F) * $80);
    Result := (Result shr 4) xor (((Result xor (ord(MWHashTable[AString[I]]) shr 4)) and $F) * $80);
    if I = 3 then break;
  end;
  if Result = 0 then Result := Length(AString) mod 8 + 1;
end;

function ISmallCrcHash(const AString: string): integer;
var
  I: Integer;
begin
  Result := 0;
  for I := 1 to length(AString) do
  begin
    Result := (Result shr 4) xor (((Result xor MWInsensitiveHashTable[AString[I]]) and $F) * $80);
    Result := (Result shr 4) xor (((Result xor (ord(MWInsensitiveHashTable[AString[I]]) shr 4)) and $F) * $80);
    if I = 3 then break;
  end;
  if Result = 0 then Result := Length(AString) mod 8 + 1;
end;

function TinyHash(const AString: string): Integer;
var
  I: Integer;
begin
  Result := Length(AString);
  for I := 1 to length(AString) do
  begin
    Inc(Result, MWHashTable[AString[I]]);
    Result := Result mod 128 + 1;
    if I = 2 then break;
  end;
end;

function ITinyHash(const AString: string): Integer;
var
  I: Integer;
begin
  Result := Length(AString);
  for I := 1 to length(AString) do
  begin
    Inc(Result, MWInsensitiveHashTable[AString[I]]);
    Result := Result mod 128 + 1;
    if I = 2 then break;
  end;
end;

function HashCompare(const Str1: string; const Str2: string): Boolean;
var
  I: Integer;
begin
  if Length(Str1) <> Length(Str2) then
  begin
    Result := False;
    Exit;
  end;
  Result := True;
  for I := 1 to Length(Str1) do
    if Str1[I] <> Str2[I] then
    begin
      Result := False;
      Exit;
    end;
end;

function IHashCompare(const Str1: string; const Str2: string): Boolean;
var
  I: Integer;
begin
  if Length(Str1) <> Length(Str2) then
  begin
    Result := False;
    Exit;
  end;
  Result := True;
  for I := 1 to Length(Str1) do
    if MWInsensitiveHashTable[Str1[I]] <> MWInsensitiveHashTable[Str2[I]] then
    begin
      Result := False;
      Exit;
    end;
end;

function HashSecondaryOne(const AString: string): Integer;
begin
  Result := Length(AString);
  Inc(Result, MWInsensitiveHashTable[AString[Length(AString)]]);
  Result := Result mod 16 + 1;
  Inc(Result, MWInsensitiveHashTable[AString[1]]);
  Result := Result mod 16 + 1;
end;

function HashSecondaryTwo(const AString: string): Integer;
var
  I: Integer;
begin
  Result := Length(AString);
  for I := Length(AString) downto 1 do
  begin
    Inc(Result, MWInsensitiveHashTable[AString[I]]);
    Result := Result mod 32 + 1;
  end;
end;

{ TMWHashString }

constructor TMWHashWord.Create(AString: string; AnId, AnExId: Integer);
begin
  inherited Create;
  S := AString;
  Id := AnId;
  ExID := AnExId;
end;

{ TMWBaseStringHashList }

procedure TMWBaseStringHashList.Clear;
var
  I: Integer;
begin
  for I := 1 to FCapacity do
    if FList[I] <> nil then
      FList[I].Free;
  ReallocMem(FList, 0);
  FCapacity := 0;
end;

destructor TMWBaseStringHashList.Destroy;
begin
  Clear;
  inherited Destroy;
end;

function TMWBaseStringHashList.Get(Index: Integer): Pointer;
begin
  Result := nil;
  if (Index > 0) and (Index <= FCapacity) then
    Result := FList[Index];
end;

procedure TMWBaseStringHashList.Put(Index: Integer; Item: Pointer);
begin
  if (Index > 0) and (Index <= FCapacity) then
    FList[Index] := Item;
end;

procedure TMWBaseStringHashList.SetCapacity(NewCapacity: Integer);
var
  I, OldCapacity: Integer;
begin
  if NewCapacity > FCapacity then
  begin
    ReallocMem(FList, (NewCapacity) * SizeOf(Pointer));
    OldCapacity := FCapacity;
    FCapacity := NewCapacity;
    for I := OldCapacity + 1 to NewCapacity do Items[I] := nil;
  end;
end;

{ TMWHashStrings }

procedure TMWHashStrings.AddString(AString: string; AnId, AnExId: Integer);
begin
  SetCapacity(Capacity + 1);
  FList[Capacity] := TMWHashWord.Create(AString, AnId, AnExId);
end;

{ TMWHashItems }

procedure TMWHashItems.AddString(AString: string; AnId, AnExId: Integer);
var
  HashWord: TMWHashWord;
  HashStrings: TMWHashStrings;
  HashVal: Integer;
begin
  HashVal := FHash(AString);
  SetCapacity(HashVal);
  if Items[HashVal] = nil then
  begin
    Items[HashVal] := TMWHashWord.Create(AString, AnId, AnExId);
  end else
    if FList[HashVal] is TMWHashStrings then
    begin
      TMWHashStrings(Items[HashVal]).AddString(AString, AnId, AnExId);
    end else
    begin
      HashWord := Items[HashVal];
      HashStrings := TMWHashStrings.Create;
      Items[HashVal] := HashStrings;
      HashStrings.AddString(HashWord.S, HashWord.Id, HashWord.ExId);
      HashWord.Free;
      HashStrings.AddString(AString, AnId, AnExId)
    end;
end;

constructor TMWHashItems.Create(AHash: TMWStringHash);
begin
  inherited Create;
  FHash := AHash;
end;

{ TMWStringHashList }

constructor TMWStringHashList.Create(Primary, Secondary: TMWStringHash; ACompare: TMWStringHashCompare);
begin
  inherited Create;
  FHash := Primary;
  FSecondaryHash := Secondary;
  FCompare := ACompare;
end;

procedure TMWStringHashList.AddString(AString: string; AnId, AnExId: Integer);
var
  HashWord: TMWHashWord;
  HashValue: Integer;
  HashItems: TMWHashItems;
begin
  HashValue := FHash(AString);
  if HashValue >= FCapacity then SetCapacity(HashValue);
  if Items[HashValue] = nil then
  begin
    Items[HashValue] := TMWHashWord.Create(AString, AnId, AnExId);
  end else
    if FList[HashValue] is TMWHashItems then
    begin
      TMWHashItems(Items[HashValue]).AddString(AString, AnId, AnExId);
    end else
    begin
      HashWord := Items[HashValue];
      HashItems := TMWHashItems.Create(FSecondaryHash);
      Items[HashValue] := HashItems;
      HashItems.AddString(HashWord.S, HashWord.Id, HashWord.ExId);
      HashWord.Free;
      HashItems.AddString(AString, AnId, AnExId);
    end;
end;

function TMWStringHashList.Hash(const S: string; var AnId: Integer; var AnExId: Integer): Boolean;
begin
  Result := HashEX(S, AnId, AnExId, FHash(S));
end;

function TMWStringHashList.HashEX(const S: string; var AnId: Integer; var AnExId: Integer; HashValue: Integer): Boolean;
var
  Temp: TObject;
  Hashword: TMWHashWord;
  HashItems: TMWHashItems;
  I, ItemHash: Integer;
begin
  Result := False;
  AnId := -1;
  AnExId := -1;
  if HashValue < 1 then Exit;
  if HashValue > Capacity then Exit;
  if Items[HashValue] <> nil then
  begin
    if FList[HashValue] is TMWHashWord then
    begin
      Hashword := Items[HashValue];
      Result := FCompare(HashWord.S, S);
      if Result then
      begin
        AnId := HashWord.Id;
        AnExId := HashWord.ExID;
      end;
    end else
    begin
      HashItems := Items[HashValue];
      ItemHash := HashItems.FHash(S);
      if ItemHash > HashItems.Capacity then Exit;
      Temp := HashItems[ItemHash];
      if Temp <> nil then
        if Temp is TMWHashWord then
        begin
          Result := FCompare(TMWHashWord(Temp).S, S);
          if Result then
          begin
            AnId := TMWHashWord(Temp).Id;
            AnExId := TMWHashWord(Temp).ExID;
          end;
        end else
          for I := 1 to TMWHashStrings(Temp).Capacity do
          begin
            HashWord := TMWHashStrings(Temp)[I];
            Result := FCompare(HashWord.S, S);
            if Result then
            begin
              AnId := HashWord.Id;
              AnExId := HashWord.ExID;
              Exit;
            end;
          end;
    end;
  end;
end;

initialization
  InitTables;
{$R+}
end.

