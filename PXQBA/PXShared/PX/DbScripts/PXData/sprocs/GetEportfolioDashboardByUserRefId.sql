SET ANSI_NULLS ON
GO 
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEportfolioDashboardByUserRefId]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].GetEportfolioDashboardByUserRefId
END
GO

-- =============================================
-- Author:		Satya
-- Create date: 06/28/2013
-- Description:	Select data from "[userprograms]" table
-- =============================================
CREATE PROCEDURE [dbo].GetEportfolioDashboardByUserRefId
	@User_Ref_Id BIGINT	
AS
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 SET NOCOUNT ON;  
   
 SELECT [User_id],User_Ref_Id, user_domain_id as Domain_Id, user_dashboard_id as Dashboard_Id  
  FROM   
  dbo.userprograms  
  WHERE   
  User_Ref_Id = @User_Ref_Id  order by program_id desc   
END  
  

GO

