<% ' [ RA_version="v.b8" ]
Call writeLog("2", " ****** Entering xx-Func-GetCheckSum.asp:main() ****** ")


'*******************************************************
'*     Checksum                                        *
'* A checksum is a value computed by adding together   *
'* all the numbers in the input data. If the sum of    *
'* all the numbers exceeds the highest value that a    *
'* checksum can hold, the checksum equals the modulus  *
'* of the total--that is, the remainder that's left    *
'* over when the total is divided by the checksum's    *
'* maximum possible value plus 1. In mathematical      *
'* terms, a checksum is computed with the equation:    *
'* Checksum = Total modulo (MaxValue + 1)              *
'* This implementation will set a limit of a MaxValue  *
'* of 255 or 'FF', returning a 2 byte checksum         *
'*                                                     *
'*******************************************************
'

' ----------------------------------------------------
' Function getChecksum(str)
'
' Returns a 2 byte checksum based on the input string
' Assumes the string is trimmed alphanumeric 
' -----------------------------------------------------
Function getChecksumOLD(str) 
  Call writeLog("2", " ****** Entering xx-Func-getChecksumOLD.asp:getChecksumOLD(" & str & ") ****** ")
     'If empty string, checksum is 0
     if str = "" then
        getChecksumOLD = 0
        exit Function
     end if
     'Max total to guarantee nothing greater than 1 byte
     const intMaxValue = 255
     dim i,intTotal,sngTmp,bytVal
     intTotal = 0
     'Do for all characters in string
     for i = 1 to Len(str) 
        'Get the next char
        sngTmp = Mid(str,i,1)
        'Get its' ascii number
        bytVal = Asc(sngTmp)
        'Add it to the total
        intTotal = intTotal + bytVal
     next
     'If the total is greater than max value
     if intTotal > intMaxValue then
        'Use the remainder
        getChecksumOLD = intTotal Mod (intMaxValue + 1)
     else 
        getChecksumOLD = intTotal
     end if
End Function

Function getChecksum(str) 
Call writeLog("2", " ****** Entering xx-Func-GetChecksum.asp:getChecksum(" & str & ") ****** ")
     'If empty string, checksum is 0
     if str = "" then
        getChecksum = 0
        exit Function
     end if
     'Max total to guarantee nothing greater than 1 byte
     const intMaxValue = 255
     dim i,intTotal,sngTmp,bytVal
     intTotal = 0
     'Do for all characters in string
     for i = 1 to Len(str) 
        'Get the next char
        sngTmp = Mid(str,i,1)
        'Get its' ascii number
        bytVal = b30_IntValue(sngTmp)
        'Add it to the total
        intTotal = intTotal + bytVal
     next
     'If the total is greater than max value
     if intTotal > intMaxValue then
        'Use the remainder
        getChecksum = intTotal Mod (intMaxValue + 1)
     else 
        getChecksum = intTotal
     end if
	Call writeLog("2", "calculated checksum:" & getChecksum )
End Function

function b30_IntValue(ch)
	if ch <> "" then
'		Lookup=Array("","","2","3","4","","6","7","8","9", _
'		             "a","b","c","d","e","f","g","h","i","j", _
'		             "k","","m","n","","p","q","r","","t", _
'		             "u","v","w","x","y","z")
		Lookup30=Array("2","3","4","6","7","8","9", _
		             "a","b","c","d","e","f","g","h","i","j", _
		             "k","m","n","p","q","r","t", _
		             "u","v","w","x","y","z")
		for i=0 to UBound(Lookup30)
			if ch = Lookup30(i) then
				b30_IntValue = i
			end if
		next
	end if
end function
%>