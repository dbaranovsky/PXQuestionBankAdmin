USE [PXData2]
GO
/****** Object:  Table [dbo].[PxWebRights]    Script Date: 06/20/2014 12:38:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PxWebRights](
	[PxWebRightId] [int] IDENTITY(1,1) NOT NULL,
	[PxWebRightType] [varchar](50) NOT NULL,
 CONSTRAINT [PK_PxWebRights_1] PRIMARY KEY CLUSTERED 
(
	[PxWebRightId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[PxWebRights] ON
INSERT [dbo].[PxWebRights] ([PxWebRightId], [PxWebRightType]) VALUES (1, N'QuestionBank')
INSERT [dbo].[PxWebRights] ([PxWebRightId], [PxWebRightType]) VALUES (2, N'AdminTool')
INSERT [dbo].[PxWebRights] ([PxWebRightId], [PxWebRightType]) VALUES (3, N'Empty')
INSERT [dbo].[PxWebRights] ([PxWebRightId], [PxWebRightType]) VALUES (4, N'Empty2')
SET IDENTITY_INSERT [dbo].[PxWebRights] OFF
/****** Object:  Table [dbo].[PxWebUserRights]    Script Date: 06/20/2014 12:38:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PxWebUserRights](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CourseId] [nvarchar](50) NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[PxWebRightId] [int] NOT NULL,
	[Rights] [bigint] NOT NULL,
 CONSTRAINT [PK_PxWebUserRights] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[PxWebUserRights] ON
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (8, N'57704', N'8', 4, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (13, N'63237', N'8', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (14, N'22244', N'7', 2, 33)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (15, N'22244', N'8', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (17, N'57704', N'10452', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (18, N'57704', N'10452', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (19, N'57704', N'11', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (20, N'57704', N'11', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (25, N'57704', N'7', 2, 1)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (26, N'57704', N'7', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (29, N'71836', N'8', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (30, N'71836', N'7', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (32, N'70974', N'9', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (201, N'71836', N'9', 2, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (202, N'71836', N'9', 2, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (203, N'71836', N'9', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (204, N'71836', N'9', 2, 32)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (205, N'71836', N'9', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (208, N'71836', N'12', 2, 0)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (209, N'71836', N'12', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (226, N'22244', N'9', 2, 1)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (227, N'22244', N'9', 1, 2033)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (233, N'57704', N'6668572', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (234, N'57704', N'7', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (236, N'57704', N'6668572', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (239, N'56644', N'12', 2, 1)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (240, N'56644', N'12', 1, 0)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (241, N'57704', N'6668572', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (242, N'71836', N'7', 2, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (243, N'71836', N'6668622', 2, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (244, N'71836', N'6668572', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (245, N'71836', N'6668623', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (246, N'63237', N'6668623', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (253, N'1234', N'1234', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (254, N'63237', N'6668572', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (255, N'63237', N'6669203', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (256, N'58320', N'6669231', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (257, N'112197', N'9', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (258, N'61195', N'7260937', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (259, N'75652', N'7260937', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (260, N'61233', N'7260937', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (261, N'115728', N'6669171', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (262, N'71836', N'6669168', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (263, N'105975', N'6669168', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (264, N'124159', N'6669168', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (265, N'57704', N'6669168', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (266, N'70881', N'6668572', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (267, N'57704', N'8', 1, 2033)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (268, N'57704', N'8', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (269, N'57704', N'9', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (270, N'57704', N'9', 1, 1937)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (271, N'85256', N'8', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (272, N'85256', N'8', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (273, N'85256', N'9', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (274, N'85256', N'9', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (275, N'70295', N'9', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (276, N'70295', N'9', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (277, N'70295', N'8', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (278, N'70295', N'8', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (289, N'57510', N'8', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (290, N'57510', N'8', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (291, N'57510', N'9', 2, 17)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (292, N'57510', N'9', 1, 2001)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (293, N'57704', N'6669191', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (294, N'71836', N'6669191', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (295, N'71247', N'9', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (296, N'71680', N'9', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (297, N'71836', N'6669231', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (298, N'57704', N'6669231', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (299, N'57704', N'6668662', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (300, N'57704', N'6669208', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (301, N'57704', N'6669190', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (302, N'57704', N'6669185', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (304, N'71674', N'6669231', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (305, N'57510', N'dnewman', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (306, N'57510', N'6669203', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (307, N'57510', N'6669319', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (308, N'142835', N'6669231', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (309, N'85256', N'116649', 4, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (310, N'85256', N'116649', 3, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (311, N'85256', N'116649', 2, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (312, N'85256', N'116649', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (313, N'85256', N'6669243', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (314, N'85256', N'6669243', 2, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (315, N'85256', N'6669243', 4, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (316, N'85256', N'6670122', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (317, N'71836', N'6670122', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (318, N'85256', N'6670123', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (319, N'71836', N'6670123', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (320, N'85256', N'6670124', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (322, N'85256', N'6670128', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (323, N'71836', N'6670128', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (324, N'85256', N'6670125', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (325, N'71836', N'6670125', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (326, N'85256', N'6670126', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (327, N'71836', N'6670126', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (328, N'85256', N'6670127', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (329, N'71836', N'6670127', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (330, N'57704', N'6669208', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (332, N'57510', N'6669208', 2, 49)
GO
print 'Processed 100 total records'
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (333, N'57704', N'6669175', 2, 49)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (334, N'85256', N'6677070', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (335, N'71836', N'6677070', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (336, N'71674', N'6668572', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (337, N'71674', N'6669168', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (338, N'71674', N'6669191', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (339, N'71674', N'6669231', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (340, N'71674', N'6670122', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (341, N'71674', N'6670123', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (342, N'71674', N'6670124', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (343, N'71674', N'6670125', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (344, N'71674', N'6670126', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (345, N'71674', N'6670127', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (346, N'71674', N'6670128', 1, 1281)
INSERT [dbo].[PxWebUserRights] ([Id], [CourseId], [UserId], [PxWebRightId], [Rights]) VALUES (347, N'71674', N'6677070', 1, 1281)
SET IDENTITY_INSERT [dbo].[PxWebUserRights] OFF
/****** Object:  Default [DF_PxWebUserRights_Rights]    Script Date: 06/20/2014 12:38:24 ******/
ALTER TABLE [dbo].[PxWebUserRights] ADD  CONSTRAINT [DF_PxWebUserRights_Rights]  DEFAULT ((0)) FOR [Rights]
GO
/****** Object:  ForeignKey [FK_PxWebUserRights_PxWebRights]    Script Date: 06/20/2014 12:38:24 ******/
ALTER TABLE [dbo].[PxWebUserRights]  WITH CHECK ADD  CONSTRAINT [FK_PxWebUserRights_PxWebRights] FOREIGN KEY([PxWebRightId])
REFERENCES [dbo].[PxWebRights] ([PxWebRightId])
GO
ALTER TABLE [dbo].[PxWebUserRights] CHECK CONSTRAINT [FK_PxWebUserRights_PxWebRights]
GO
