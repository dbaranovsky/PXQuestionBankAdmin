-- MASTER FILE FOR CREATING TABLES AND STORED PROCEDURES FOR 'PX- Admin Portal'

--******************************************************************************
-- Please enable SQLCMD mode (In Microsoft SQL Server Management Studio, select 
--  "Query --> SQLCMD Mode" from the menu).
--
-- MODIFY:	(1) The database name (DATABASENAME)
--			(2) The path to the SQL Scripts \Database directory (PATH)
--******************************************************************************

-- The following variables must be set properly if this script is run, BUT DO NOT CHECK THEM IN
--:setvar DATABASENAME "PXAP"
--:setvar PATH "C:\Development\MacMillan\PX\Trunk\PX\DbScripts\PXData\"

:setvar DATABASENAME "PXAP"
:setvar PATH "C:\Development\PX\Trunk\PXAP\DbScripts\PXAP\"


if not exists(select * from sys.databases where name = '$(DATABASENAME)')
    :r $(PATH)"create.sql"

USE $(DATABASENAME);
GO

-- CREATE TABLES
:r $(PATH)"tables\Environment.sql"
:r $(PATH)"tables\EnvironmentSources.sql"
:r $(PATH)"tables\ProgressStatus.sql"
--:r $(PATH)"tables\"
--:r $(PATH)"tables\"

-- ALTER SCRIPTS
:r $(PATH)"alter\tables\alter_Environment.sql"
--:r $(PATH)"alter\tables\alter_highlights_add_color.sql"

-- CREATE FUNCTIONS
--:r $(PATH)"functions\NotesSearchUserIds.sql"

-- CREATE SPROCS
:r $(PATH)"sprocs\sp_AddUpdateEnvironment.sql"
:r $(PATH)"sprocs\sp_DeleteEnvironment.sql"
:r $(PATH)"sprocs\sp_AddUpdateProgressStatus.sql"
:r $(PATH)"sprocs\sp_GetProgressStatus.sql"
--:r $(PATH)"sprocs\AddNote.sql"
--:r $(PATH)"sprocs\AddNote.sql"