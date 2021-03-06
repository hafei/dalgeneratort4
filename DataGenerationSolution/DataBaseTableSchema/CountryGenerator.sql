USE [QStudy]
GO
/****** Object:  Table [dbo].[States]    Script Date: 03/07/2012 21:11:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[States](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountryId] [int] NOT NULL,
	[Name] [char](40) NOT NULL,
	[Abbrev] [char](3) NULL,
 CONSTRAINT [PK__States__08EA5793] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 03/07/2012 21:11:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ISO] [char](2) NOT NULL,
	[Name] [varchar](80) NOT NULL,
	[PrintableName] [varchar](80) NOT NULL,
	[ISO3] [char](3) NULL,
	[NumCode] [smallint] NULL,
	[HasState] [bit] NOT NULL CONSTRAINT [DF_Country_HasState]  DEFAULT ((1)),
 CONSTRAINT [PK_Countries_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[ISO] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
