--the users id for the program and user program needs to be updated based on the environment
/*
insert into [dbo].[programs]
	(Program_manager_id, Program_manager_ref_id, Program_manager_domain_id) values
	(1, 1, 1)
	
declare	@programid int

select @programid = @@identity

insert into [dbo].[userprograms]
	(Program_id, [User_id], User_ref_id, User_domain_id) values
	(@programid, 1, 1, 1)
go*/