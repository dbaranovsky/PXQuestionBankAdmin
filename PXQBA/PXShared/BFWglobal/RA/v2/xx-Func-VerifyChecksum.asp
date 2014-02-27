<% ' [ RA_version="v.b8" ]
%>
<!--#include file="xx-Func-GetChecksum.asp"-->
<%
Call writeLog("2", " ****** Entering xx-Func-VerifyChecksum.asp:main() ****** ")

' ----------------------------------------------------
' Function verifyChecksum(str)
'
' Verifies that a code checksums to its' 2 byte value
' -----------------------------------------------------
function verifyChecksum(str)
  Call writeLog("2", " ****** Entering xx-Func-VerifyChecksum.asp:verifyChecksum(" & str & ") ****** ")
   ' Empty String, no checksum
   trim(str)
   if str = "" then
      verifyChecksum = false
      exit function
   end if
   dim strChksum,strCand,intChksum,intStrLen,intCheckit,strCheckit
   intStrLen = Len(str)

   ' String len < 3 cannot have a 2 byte checksum at the end
   if intStrLen < 3 then
      verifyChecksum = false
      exit function
   end if

   ' Last two bytes are the checksum
   ' This assumes that a 1 byte checksum is
   ' prepended with a char '0'
   strChksum = Mid(str,intStrLen-1,2)
   ' Retrieve the checksum candidate as a substring
   strCand = Mid(str,1,intStrLen-2)

   ' Hugh modified to take out 0,1,5
   	if instr(strChksum,"x") > 0 then
   		strChksum = replace(strChksum,"x","1")
   	end if
   	if instr(strChksum,"y") > 0 then
   		strChksum = replace(strChksum,"y","5")
   	end if
   	if instr(strChksum,"z") > 0 then
   		strChksum = replace(strChksum,"z","0")
	end if

   ' Calculate the true checksum for the candidate
   intCheckit = getChecksum(strCand)
   ' Convert to a lower case hex string
   strCheckit = LCase(Hex(intCheckit))

   ' If the length is 1, prepend a '0'
   if Len(strCheckit) < 2 then
      strCheckit = "0" & strCheckit
   end if

   ' Verify the checksums match
   if strCheckit = strChksum then
      verifyChecksum = true
   else
      verifyChecksum = false
   end if
  Call writeLog("2", "     input checksum:" & strChksum )
  Call writeLog("2", "calculated checksum:" & strCheckit )

end function

function verifyChecksumOLD(str)
  Call writeLog("2", " ****** Entering xx-Func-verifyChecksumOLD.asp:verifyChecksumOLD(" & str & ") ****** ")
   ' Empty String, no checksum
   trim(str)
   if str = "" then
      verifyChecksumOLD = false
      exit function
   end if
   dim strChksum,strCand,intChksum,intStrLen,intCheckit,strCheckit
   intStrLen = Len(str)

   ' String len < 3 cannot have a 2 byte checksum at the end
   if intStrLen < 3 then
      verifyChecksumOLD = false
      exit function
   end if

   ' Last two bytes are the checksum
   ' This assumes that a 1 byte checksum is
   ' prepended with a char '0'
   strChksum = Mid(str,intStrLen-1,2)
   ' Retrieve the checksum candidate as a substring
   strCand = Mid(str,1,intStrLen-2)

   ' Calculate the true checksum for the candidate
   intCheckit = getChecksumOLD(strCand)
   ' Convert to a lower case string
   strCheckit = LCase(Hex(intCheckit))

   ' If the length is 1, prepend a '0'
   if Len(strCheckit) < 2 then
      strCheckit = "0" & strCheckit
   end if

   ' Verify the checksums match
   if strCheckit = strChksum then
      verifyChecksumOLD = true
   else
      verifyChecksumOLD = false
   end if

end function

%>