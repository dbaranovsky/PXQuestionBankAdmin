use BrainHoneyDLAP;
go

declare @sql varchar(max);
declare @date varchar(50);
set @date = 'user2_' + replace(replace(replace(replace(convert(char(23), getdate(), 121),':','_'), '-','_'), ' ', '_'), '.','_');
set @sql='select * into ' + @date + ' from user2';
exec(@sql);
go

declare @Tranname VARCHAR(20);
set @Tranname = 'LMSTransaction';

Begin Transaction @Tranname;
   update User2
   set Username = Reference
   where rtrim(ltrim(isnull(Reference, ''))) not in ('0', '') 
   and   flags = 0;

   select email, domainid
   from user2
   where Flags = 0
   group by Email, DomainId
   having COUNT(*) > 1;
   
   update user2
   set Reference = ''
   where rtrim(ltrim(isnull(Reference, ''))) not in ('0', '') 
   and   flags =0;

   --replace lmsid column with email address if present and not duplicated across domains
   update User2
   set Reference = isnull(email, '')
   from user2 
   where Flags = 0 
   and   ISNULL(email, '') <> ''
   and   UserId > 13  --reserved ids
   and   email not in (select email from (select email, domainid, count=COUNT(*)
                       from user2
                       group by Email, DomainId
                       having COUNT(*) > 1) as dup_table);

Commit Transaction @Tranname;
go


