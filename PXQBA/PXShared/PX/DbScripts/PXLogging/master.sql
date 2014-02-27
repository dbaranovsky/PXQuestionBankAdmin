-- MASTER FILE FOR CREATING TABLES AND STORED PROCEDURES FOR Platform-X Logging Subsystem

--******************************************************************************
-- Please enable SQLCMD mode (In Microsoft SQL Server Management Studio, select 
--  "Query --> SQLCMD Mode" from the menu).
--
-- MODIFY:	(1) The database name (DATABASENAME)
--			(2) The path to the SQL Scripts \Database directory (PATH)
--******************************************************************************

-- The following variables must be set properly if this script is run, BUT DO NOT CHECK THEM IN
:setvar DATABASENAME "PXLogging2"
:setvar PATH "C:\Development\PX\Trunk\PX\DbScripts\PXLogging\"

if not exists(select * from sys.databases where name = '$(DATABASENAME)')
    :r $(PATH)"create.sql"

-- For now this script only kicks off a slightly modified version of the stock EL5 logging DB create script