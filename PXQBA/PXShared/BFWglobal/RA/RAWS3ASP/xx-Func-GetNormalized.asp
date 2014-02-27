<% ' [ RA_version="v.b8" ]
Call writeLog("2", " ****** Entering xx-Func-GetNormalized.asp:main() ****** ")

'*******************************************************
'*     getNormalized                                   *
'*                                                     *
'* Filter out white space and non alphanumeric         *
'* chars from a string.  Converts to lower case        *
'*******************************************************
Function getNormalized(str) 
  Call writeLog("2", " ****** Entering xx-Func-GetNormalized.asp:getNormalized(" & str & ") ****** ")
   dim i,arrVals,sngVal,sngAsc
   ' If empty, return empty
   if str = "" then
      getNormalized = ""
      exit function
   end if
   ' do for all chars
   for i = 1 to Len(str)
       sngVal = Mid(str,i,1)
       sngAsc = Asc(sngVal)
       ' 0 thru 9
       if sngAsc > 47 and sngAsc < 58 then
          arrVals = arrVals & sngVal
       ' A thru Z, convert to lower case
       ElseIf sngAsc > 64 and sngAsc < 91 then
          arrVals = arrVals & LCase(sngVal)
       ' a thru z
       ElseIf sngAsc > 96 and sngAsc < 123 then
          arrVals = arrVals & sngVal
       ElseIf sngAsc = 45 then
          arrVals = arrVals & sngVal
       end if
   next
   getNormalized = arrVals
End Function

%>