/****** Object:  Table [dbo].[Competition]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Competition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[TypeId] [int] NOT NULL,
	[RowStatus] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[ReferenceId] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Competition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompetitionPlayer]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompetitionPlayer](
	[CompetitionId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[Rank] [int] NULL,
 CONSTRAINT [PK_CompetitionPlayer] PRIMARY KEY CLUSTERED 
(
	[CompetitionId] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompetitionType]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompetitionType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[RowStatus] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
 CONSTRAINT [PK_CompetitionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Player]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Player](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IdNumber] [nvarchar](50) NOT NULL,
	[Rank] [int] NULL,
	[RowStatus] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
 CONSTRAINT [PK_Player] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](56) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[webpages_Membership]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Membership](
	[UserId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](128) NULL,
	[IsConfirmed] [bit] NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordChangedDate] [datetime] NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[PasswordVerificationToken] [nvarchar](128) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[webpages_OAuthMembership]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_OAuthMembership](
	[Provider] [nvarchar](30) NOT NULL,
	[ProviderUserId] [nvarchar](100) NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[ProviderUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[webpages_Roles]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[webpages_UsersInRoles]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[Users]    Script Date: 03/10/2012 02:27:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[Users]
AS
SELECT        UP.UserId, UserName, U.IsConfirmed IsActive
FROM            dbo.UserProfile UP INNER JOIN
                         dbo.webpages_Membership U ON UP.UserId = U.UserId 


GO
ALTER TABLE [dbo].[Competition] ADD  CONSTRAINT [DF_Competition_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Competition] ADD  CONSTRAINT [DF_Competition_Created]  DEFAULT (getutcdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Competition] ADD  CONSTRAINT [DF_Competition_Created1]  DEFAULT (getutcdate()) FOR [Updated]
GO
ALTER TABLE [dbo].[CompetitionType] ADD  CONSTRAINT [DF_CompetitionType_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[CompetitionType] ADD  CONSTRAINT [DF_CompetitionType_Created]  DEFAULT (getutcdate()) FOR [Created]
GO
ALTER TABLE [dbo].[CompetitionType] ADD  CONSTRAINT [DF_CompetitionType_Updated]  DEFAULT (getutcdate()) FOR [Updated]
GO
ALTER TABLE [dbo].[Player] ADD  CONSTRAINT [DF_Player_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Player] ADD  CONSTRAINT [DF_Player_Created]  DEFAULT (getutcdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Player] ADD  CONSTRAINT [DF_Player_Updated]  DEFAULT (getutcdate()) FOR [Updated]
GO
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [IsConfirmed]
GO
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [PasswordFailuresSinceLastSuccess]
GO
ALTER TABLE [dbo].[Competition]  WITH CHECK ADD  CONSTRAINT [FK_Competition_CompetitionType] FOREIGN KEY([TypeId])
REFERENCES [dbo].[CompetitionType] ([Id])
GO
ALTER TABLE [dbo].[Competition] CHECK CONSTRAINT [FK_Competition_CompetitionType]
GO
ALTER TABLE [dbo].[CompetitionPlayer]  WITH CHECK ADD  CONSTRAINT [FK_CompetitionPlayer_Competition] FOREIGN KEY([CompetitionId])
REFERENCES [dbo].[Competition] ([Id])
GO
ALTER TABLE [dbo].[CompetitionPlayer] CHECK CONSTRAINT [FK_CompetitionPlayer_Competition]
GO
ALTER TABLE [dbo].[CompetitionPlayer]  WITH CHECK ADD  CONSTRAINT [FK_CompetitionPlayer_Player] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[Player] ([Id])
GO
ALTER TABLE [dbo].[CompetitionPlayer] CHECK CONSTRAINT [FK_CompetitionPlayer_Player]
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[webpages_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_RoleId]
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserProfile] ([UserId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_UserId]
GO