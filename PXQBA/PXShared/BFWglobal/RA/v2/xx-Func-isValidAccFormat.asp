<% ' [ RA_version="v.b8" ]
Call writeLog("2", " ****** Entering xx-Func-isValidAccFormat.asp:main() ****** ")

'***********************************************************
'*     isValidAccFormat                                    *
'*                                                         *
'* Validates a user's activation code as to format. (Not   *
'* checksum).  Expects xxxxx-yyy-zzzzz as alpha            *
'* numeric.                                                *
'***********************************************************
Function isValidAccFormat(str) 
  Call writeLog("2", " ****** Entering xx-Func-isValidAccFormat.asp:isValidAccFormat(" & str & ") ****** ")
   trim(str)
   dim i,arrVals,sngVal,sngAsc
   ' If empty, return empty
   if str = "" then
      isValidAccFormat = false
      exit function
   end if
   ' do for all chars
   for i = 1 to Len(str)
       sngVal = Mid(str,i,1)
       sngAsc = Asc(sngVal)
       '-'
       if sngAsc = 45 then
          isValidAccFormat = true
       ' 0 thru 9
       ElseIf sngAsc > 47 and sngAsc < 58 then
          isValidAccFormat = true
       ' A thru Z
       ElseIf sngAsc > 64 and sngAsc < 91 then
          isValidAccFormat = true
       ' a thru z
       ElseIf sngAsc > 96 and sngAsc < 123 then
          isValidAccFormat = true
       else 
          isValidAccFormat = false
          exit Function
       end if
   next
   isValidAccFormat = true
End Function

%>