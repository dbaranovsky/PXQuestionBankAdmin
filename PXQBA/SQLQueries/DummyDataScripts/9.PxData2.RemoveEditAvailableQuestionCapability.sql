USE PxData2
delete from QBARoleCapability
where capabilityid = 16 and roleid in (select Id from qbarole where name in ('Administrator', 'Media Producer', 'Media Editor', 'Prouduction Developer', 'Author', 'Reviewer'))