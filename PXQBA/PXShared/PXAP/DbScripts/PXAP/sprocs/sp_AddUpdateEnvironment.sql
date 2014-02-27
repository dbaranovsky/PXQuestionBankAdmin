SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[sp_AddUpdateEnvironment]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[sp_AddUpdateEnvironment]
END
GO
CREATE PROCEDURE [dbo].[sp_AddUpdateEnvironment]
	@envXml xml
AS
BEGIN

	declare @EnvironmentId int
	declare @Description  varchar(255)
	declare @DlapServer varchar(255)
	declare @BrainHoneyServer varchar(255)
	declare @PxDocs varchar(255)
	declare @Title varchar(255)
	declare @BrainHoneyDocs varchar(255)
	
	
	select  @EnvironmentId = Node.value('@EnvironmentId[1]', 'int')
			,@Description = Node.value('@Description[1]', 'varchar(200)')
			,@DlapServer = Node.value('@DlapServer[1]', 'varchar(200)')
			,@BrainHoneyServer = Node.value('@BrainHoneyServer[1]', 'varchar(200)')
			,@PxDocs = Node.value('@PxDocs[1]', 'varchar(200)')
			,@Title = Node.value('@Title[1]', 'varchar(200)')
			,@BrainHoneyDocs = Node.value('@BrainHoneyDocs[1]', 'varchar(200)')
	from @envXml.nodes('/Environment') TempXML (Node)
	
	--print @BrainHoneyDocs
		
	if (@EnvironmentId = 0)  -- add new env
	begin
	
		declare @counter int = 0
		select @counter = COUNT(1) from Environment where Title = @Title
		
		if (@counter > 0) 
		begin
				select -1 as EnvironmentId , 'Environment with this Title already exists.' as Message
				return
		end
		
		insert into Environment (Title, Description, DlapServer, BrainHoneyServer, PxDocs, BrainHoneyDocs)
		values (@Title, @Description, @DlapServer, @BrainHoneyServer, @PxDocs, @BrainHoneyDocs)
		set @EnvironmentId =  SCOPE_IDENTITY()
	end	
	else 
	begin  -- update env
		update Environment
		set Description = @Description
			,DlapServer = @DlapServer
			,BrainHoneyServer = @BrainHoneyServer
			,PxDocs = @PxDocs
			,BrainHoneyDocs = @BrainHoneyDocs
		where EnvironmentId = @EnvironmentId
	end
	
	 --at this point we have @EnvironmentId, for both cases (add or update)
	 --using the @EnvironmentId, add records in table dbo.EnvironmentSources
	
	 --since user could have added as well as deleted sources 
	 --we will delete all sources for the env and add again
	
	delete from EnvironmentSources where EnvironmentId = @EnvironmentId
	
	insert into EnvironmentSources
	select @EnvironmentId as EnvironmentId
		   ,Node.value('.[1]', 'varchar(255)') as Source
	from @envXml.nodes('/Environment/Sources') as TempXML (Node)
		
--select  @EnvironmentId, @Description, @DlapServer, @BrainHoneyServer, @PxDocs

	-- return env id, so that for new env we have the id in the code
	select @EnvironmentId as EnvironmentId , 'Success' as Message
		
END

GO
-- sp_AddUpdateEnvironment '<Environment EnvironmentId="0" Title="GHHHH" Description="testEnv Environment for PX" DlapServer="http://testEnv.dlap.bfwpub.com/callmethod.aspx" BrainHoneyServer="http://testEnv.brainhoney.comsdf" PxDocs="http://testEnv.px-docs.com"><Sources>Source 1 testEnv</Sources>   <Sources>Source 2 testEnv</Sources>   <Sources>Source 3 testEnv</Sources>   </Environment>'
-- sp_AddUpdateEnvironment '<Environment EnvironmentId="2" Description="Env Environment for PX" DlapServer="http://dev.dlap.bfwpub.com/callmethod.aspx" BrainHoneyServer="http://dev.brainhoney.com" PxDocs="http://testEnv.px-docs.com"> </Environment>'
-- sp_AddUpdateEnvironment '<Environment EnvironmentId="1" Description="Dev Environment for PX" DlapServer="http://dev.dlap.bfwpub.com/callmethod.aspx" BrainHoneyServer="http://dev.brainhoney.com" BrainHoneyDocs="aaa" PxDocs="http://dev.pc-docs.com"><Sources>Source 1 DEV</Sources><Sources>Source 2 DEV</Sources><Sources>Source 3 DEV</Sources><Sources>VSPXDEV02</Sources></Environment>'


