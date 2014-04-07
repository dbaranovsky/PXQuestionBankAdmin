
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 04/07/2014 14:25:15
-- Generated from EDMX file: D:\Projects\MacMillan\Source\PXQuestionBankAdmin\PXQBA\DataAccess\Macmillan.PXQBA.DataAccess.Data\QBADummyModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [QBADummyData];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_QuestionProductCourse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductCourses] DROP CONSTRAINT [FK_QuestionProductCourse];
GO
IF OBJECT_ID(N'[dbo].[FK_CourseCourseMetaField]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CourseMetaFields] DROP CONSTRAINT [FK_CourseCourseMetaField];
GO
IF OBJECT_ID(N'[dbo].[FK_CourseMetaFieldCourseMetaFieldValue]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CourseMetaFieldValues] DROP CONSTRAINT [FK_CourseMetaFieldCourseMetaFieldValue];
GO
IF OBJECT_ID(N'[dbo].[FK_QuestionNote]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Notes] DROP CONSTRAINT [FK_QuestionNote];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Questions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Questions];
GO
IF OBJECT_ID(N'[dbo].[ProductCourses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductCourses];
GO
IF OBJECT_ID(N'[dbo].[Courses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Courses];
GO
IF OBJECT_ID(N'[dbo].[CourseMetaFields]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CourseMetaFields];
GO
IF OBJECT_ID(N'[dbo].[CourseMetaFieldValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CourseMetaFieldValues];
GO
IF OBJECT_ID(N'[dbo].[Notes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Notes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Questions'
CREATE TABLE [dbo].[Questions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [InteractionType] nvarchar(max)  NULL,
    [DlapId] nvarchar(max)  NULL,
    [Status] int  NOT NULL,
    [Type] int  NULL,
    [Preview] nvarchar(max)  NULL
);
GO

-- Creating table 'ProductCourses'
CREATE TABLE [dbo].[ProductCourses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProductCourseDlapId] nvarchar(max)  NOT NULL,
    [Difficulty] nvarchar(max)  NULL,
    [Chapter] nvarchar(max)  NULL,
    [Bank] nvarchar(max)  NULL,
    [Title] nvarchar(max)  NULL,
    [Sequence] int  NULL,
    [QuestionId] int  NOT NULL,
    [ExcerciseNo] nvarchar(max)  NULL,
    [Guidance] nvarchar(max)  NULL,
    [Version] nvarchar(max)  NULL,
    [CognitiveLevel] nvarchar(max)  NULL
);
GO

-- Creating table 'Courses'
CREATE TABLE [dbo].[Courses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DlapId] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CourseMetaFields'
CREATE TABLE [dbo].[CourseMetaFields] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [XMLFieldName] nvarchar(max)  NOT NULL,
    [Filterable] bit  NOT NULL,
    [FriendlyName] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [CourseId] int  NOT NULL
);
GO

-- Creating table 'CourseMetaFieldValues'
CREATE TABLE [dbo].[CourseMetaFieldValues] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Sequence] nvarchar(max)  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [CourseMetaFieldId] int  NOT NULL
);
GO

-- Creating table 'Notes'
CREATE TABLE [dbo].[Notes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [IsFlagged] bit  NOT NULL,
    [QuestionId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [PK_Questions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductCourses'
ALTER TABLE [dbo].[ProductCourses]
ADD CONSTRAINT [PK_ProductCourses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Courses'
ALTER TABLE [dbo].[Courses]
ADD CONSTRAINT [PK_Courses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CourseMetaFields'
ALTER TABLE [dbo].[CourseMetaFields]
ADD CONSTRAINT [PK_CourseMetaFields]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CourseMetaFieldValues'
ALTER TABLE [dbo].[CourseMetaFieldValues]
ADD CONSTRAINT [PK_CourseMetaFieldValues]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Notes'
ALTER TABLE [dbo].[Notes]
ADD CONSTRAINT [PK_Notes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [QuestionId] in table 'ProductCourses'
ALTER TABLE [dbo].[ProductCourses]
ADD CONSTRAINT [FK_QuestionProductCourse]
    FOREIGN KEY ([QuestionId])
    REFERENCES [dbo].[Questions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_QuestionProductCourse'
CREATE INDEX [IX_FK_QuestionProductCourse]
ON [dbo].[ProductCourses]
    ([QuestionId]);
GO

-- Creating foreign key on [CourseId] in table 'CourseMetaFields'
ALTER TABLE [dbo].[CourseMetaFields]
ADD CONSTRAINT [FK_CourseCourseMetaField]
    FOREIGN KEY ([CourseId])
    REFERENCES [dbo].[Courses]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CourseCourseMetaField'
CREATE INDEX [IX_FK_CourseCourseMetaField]
ON [dbo].[CourseMetaFields]
    ([CourseId]);
GO

-- Creating foreign key on [CourseMetaFieldId] in table 'CourseMetaFieldValues'
ALTER TABLE [dbo].[CourseMetaFieldValues]
ADD CONSTRAINT [FK_CourseMetaFieldCourseMetaFieldValue]
    FOREIGN KEY ([CourseMetaFieldId])
    REFERENCES [dbo].[CourseMetaFields]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CourseMetaFieldCourseMetaFieldValue'
CREATE INDEX [IX_FK_CourseMetaFieldCourseMetaFieldValue]
ON [dbo].[CourseMetaFieldValues]
    ([CourseMetaFieldId]);
GO

-- Creating foreign key on [QuestionId] in table 'Notes'
ALTER TABLE [dbo].[Notes]
ADD CONSTRAINT [FK_QuestionNote]
    FOREIGN KEY ([QuestionId])
    REFERENCES [dbo].[Questions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_QuestionNote'
CREATE INDEX [IX_FK_QuestionNote]
ON [dbo].[Notes]
    ([QuestionId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------