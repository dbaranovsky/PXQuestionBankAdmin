unit AttrUtils;

interface

uses SysUtils;

  function GetIntAttributeValue(AttrValue:String; DefValue:Integer):Integer;
  function GetPercentAttributeValue(AttrValue:String; DefValue:Integer):Integer;
  function GetBoolAttributeValue(AttrValue:String; DefValue:boolean):Boolean;
  function GetChoiceAttributeValue(AttrValue:String; Choices:array of String; DefValue:Integer):Integer;
  function GetTextAttributeValue(AttrValue:String; DefValue:String):String;
  function GetDoubleAttributeValue(AttrValue:String; DefValue:Double):Double;

implementation

function GetBoolAttributeValue(AttrValue:String; DefValue:boolean):Boolean;
begin
  AttrValue := Trim(attrValue);
  if AttrValue = '' then
    Result := DefValue
  else
  begin
    if SameText(AttrValue,'yes') or SameText(AttrValue,'true') or
       SameText(AttrValue,'on') or SameText(AttrValue,'1') then
      Result := true
    else
      Result := false;
  end;
end;

function GetDoubleAttributeValue(AttrValue:String; DefValue:Double):Double;
begin
  Result := DefValue;
  if Trim(AttrValue) <> '' then
  begin
    try
      Result := StrToFloat(AttrValue);
    except
    end;
  end;
end;

function GetIntAttributeValue(AttrValue:String; DefValue:Integer):Integer;
begin
  Result := DefValue;
  if Trim(AttrValue) <> '' then
  begin
    try
      Result := StrToInt(AttrValue);
    except
    end;
  end;
end;

function GetPercentAttributeValue(AttrValue:String; DefValue:Integer):Integer;
begin
  Result := DefValue;
  if Trim(AttrValue) <> '' then
  begin
    try
      if LastDelimiter('%',AttrValue) = Length(AttrValue) then
        Result := StrToInt(Copy(AttrValue,1,Length(AttrValue)-1))
      else
        Result := StrToInt(AttrValue);
    except
    end;
  end;
end;

function GetChoiceAttributeValue(AttrValue:String; Choices:array of String; DefValue:Integer):Integer;
var
 i : Integer;
begin
  for i := 0 to High(Choices) do
  begin
    if SameText(attrValue,Choices[i]) then
    begin
      Result := i;
      exit;
    end;
  end;
  Result := DefValue;
end;

function GetTextAttributeValue(AttrValue:String; DefValue:String):String;
begin
  if AttrValue <> '' then
    Result := AttrValue
  else
    Result := DefValue;
end;

end.
