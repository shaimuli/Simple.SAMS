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
	[Status] [int] NOT NULL,
	[EndTime] [datetime] NULL,
	[Site] [nvarchar](200) NULL,
	[MainReferee] [nvarchar](100) NULL,
	[MainRefereePhone] [nvarchar](20) NULL,
	[SitePhone] [nvarchar](20) NULL,
 CONSTRAINT [PK_Competition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompetitionPlayer](
	[CompetitionId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[Rank] [int] NULL,
	[Section] [int] NOT NULL,
	[Source] [int] NOT NULL,
	[Status] [int] NULL,
	[Reason] [nvarchar](100) NULL,
 CONSTRAINT [PK_CompetitionPlayer] PRIMARY KEY CLUSTERED 
(
	[CompetitionId] ASC,
	[PlayerId] ASC,
	[Section] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
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
	[PlayersCount] [int] NOT NULL,
	[CompetitionMethod] [int] NOT NULL,
	[WildcardPlayersCount] [int] NOT NULL,
	[QualifyingPlayersCount] [int] NOT NULL,
	[QualifyingPairsCount] [int] NOT NULL,
	[WildcardPairsCount] [int] NOT NULL,
	[PairsCount] [int] NOT NULL,
	[HasConsolation] [bit] NOT NULL,
	[Ranking] [int] NOT NULL,
	[QualifyingWildcardPlayersCount] [int] NOT NULL,
	[QualifyingWildcardPairsCount] [int] NOT NULL,
	[QualifyingToFinalPlayersCount] [int] NOT NULL,
	[QualifyingToFinalPaisCount] [int] NOT NULL,
 CONSTRAINT [PK_CompetitionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Match](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RowStatus] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[CompetitionId] [int] NOT NULL,
	[StartTime] [datetime] NULL,
	[Status] [int] NOT NULL,
	[Round] [int] NOT NULL,
	[Position] [int] NOT NULL,
	[SectionId] [int] NOT NULL,
	[EndTime] [datetime] NULL,
	[StartTimeType] [int] NOT NULL,
	[Player1] [int] NULL,
	[Player2] [int] NULL,
	[Player3] [int] NULL,
	[Player4] [int] NULL,
	[Player1Points] [int] NOT NULL,
	[Player2Points] [int] NOT NULL,
	[BreakPoints] [int] NOT NULL,
	[Result] [int] NULL,
	[Winner] [int] NULL,
	[RoundRelativePosition] [int] NOT NULL,
	[IsFinal] [bit] NOT NULL,
	[IsSemiFinal] [bit] NOT NULL,
 CONSTRAINT [PK_Match] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MatchScore](
	[MatchId] [int] NOT NULL,
	[SetNumber] [int] NOT NULL,
	[Player1Points] [int] NOT NULL,
	[Player2Points] [int] NOT NULL,
	[BreakPoints] [int] NOT NULL,
 CONSTRAINT [PK_MatchScore] PRIMARY KEY CLUSTERED 
(
	[MatchId] ASC,
	[SetNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Player](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LocalFirstName] [nvarchar](50) NOT NULL,
	[IdNumber] [nvarchar](50) NOT NULL,
	[NationalRank] [int] NULL,
	[RowStatus] [int] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[LocalLastName] [nvarchar](50) NULL,
	[EnglishFirstName] [nvarchar](50) NULL,
	[EnglishLastName] [nvarchar](50) NULL,
	[IsFemale] [bit] NULL,
	[EuropeInternationalRank] [int] NULL,
	[YouthInternationalRank] [int] NULL,
	[Country] [nvarchar](100) NULL,
	[BirthDate] [datetime] NULL,
	[IPIN] [nvarchar](10) NULL,
	[Phone] [nvarchar](15) NULL,
	[AccumulatedScore] [int] NULL,
	[AverageScore] [int] NULL,
 CONSTRAINT [PK_Player] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Section](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
 CONSTRAINT [PK_Section] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
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
SET ANSI_PADDING ON

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Competition_ReferenceId] ON [dbo].[Competition]
(
	[ReferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Competition] FOREIGN KEY([CompetitionId])
REFERENCES [dbo].[Competition] ([Id])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Competition]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Player] FOREIGN KEY([Player1])
REFERENCES [dbo].[Player] ([Id])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Player]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Player1] FOREIGN KEY([Player2])
REFERENCES [dbo].[Player] ([Id])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Player1]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Player2] FOREIGN KEY([Player3])
REFERENCES [dbo].[Player] ([Id])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Player2]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Player3] FOREIGN KEY([Player4])
REFERENCES [dbo].[Player] ([Id])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Player3]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Section] FOREIGN KEY([SectionId])
REFERENCES [dbo].[Section] ([Id])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Section]
GO
ALTER TABLE [dbo].[MatchScore]  WITH CHECK ADD  CONSTRAINT [FK_MatchScore_Match] FOREIGN KEY([MatchId])
REFERENCES [dbo].[Match] ([Id])
GO
ALTER TABLE [dbo].[MatchScore] CHECK CONSTRAINT [FK_MatchScore_Match]
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
