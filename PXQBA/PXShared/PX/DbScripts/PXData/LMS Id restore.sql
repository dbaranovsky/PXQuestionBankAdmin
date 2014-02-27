--restore lms ids script
--change db name by environment, same name may not exist in all environments
use BrainHoneyDLAP 
go

--change user2_bak table by environment, this is just an example of restore script
update User2 
   set username =  ubk.UserName, Reference = ubk.reference
from user2 u, user2_bak ubk
where u.userid = ubk.userid;
