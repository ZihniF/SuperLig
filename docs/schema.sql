-- SSMS: DESKTOP-O7S7PCN / Windows Authentication
IF DB_ID(N'LeagueDb') IS NULL CREATE DATABASE [LeagueDb];
GO
USE [LeagueDb];
GO
-- Uygulanmış migration'lar tekrar çalıştırılmaz. Güncel sezon kayıtları uygulama açılışında yüklenir.
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Leagues] (
        [LeagueId] int NOT NULL IDENTITY,
        [LeagueName] nvarchar(200) NOT NULL,
        [Country] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Leagues] PRIMARY KEY ([LeagueId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Stadiums] (
        [StadiumId] int NOT NULL IDENTITY,
        [StadiumName] nvarchar(200) NOT NULL,
        [City] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Stadiums] PRIMARY KEY ([StadiumId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Teams] (
        [TeamId] int NOT NULL IDENTITY,
        [TeamName] nvarchar(200) NOT NULL,
        [ShortName] nvarchar(200) NOT NULL,
        [City] nvarchar(200) NOT NULL,
        [Color] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Teams] PRIMARY KEY ([TeamId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Seasons] (
        [SeasonId] int NOT NULL IDENTITY,
        [SeasonName] nvarchar(200) NOT NULL,
        [LeagueId] int NOT NULL,
        CONSTRAINT [PK_Seasons] PRIMARY KEY ([SeasonId]),
        CONSTRAINT [FK_Seasons_Leagues_LeagueId] FOREIGN KEY ([LeagueId]) REFERENCES [dbo].[Leagues] ([LeagueId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Matches] (
        [MatchId] int NOT NULL IDENTITY,
        [SeasonId] int NOT NULL,
        [HomeTeamId] int NOT NULL,
        [AwayTeamId] int NOT NULL,
        [StadiumId] int NOT NULL,
        [Week] int NOT NULL,
        [MatchDate] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [HomeScore] int NULL,
        [AwayScore] int NULL,
        CONSTRAINT [PK_Matches] PRIMARY KEY ([MatchId]),
        CONSTRAINT [CK_Match_Scores] CHECK ((Status = 0 AND HomeScore IS NULL AND AwayScore IS NULL) OR (Status IN (1,2) AND HomeScore IS NOT NULL AND AwayScore IS NOT NULL AND HomeScore >= 0 AND AwayScore >= 0)),
        CONSTRAINT [CK_Match_Status] CHECK (Status IN (0,1,2)),
        CONSTRAINT [CK_Match_Teams] CHECK (HomeTeamId <> AwayTeamId),
        CONSTRAINT [FK_Matches_Seasons_SeasonId] FOREIGN KEY ([SeasonId]) REFERENCES [dbo].[Seasons] ([SeasonId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Matches_Stadiums_StadiumId] FOREIGN KEY ([StadiumId]) REFERENCES [dbo].[Stadiums] ([StadiumId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Matches_Teams_AwayTeamId] FOREIGN KEY ([AwayTeamId]) REFERENCES [dbo].[Teams] ([TeamId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Matches_Teams_HomeTeamId] FOREIGN KEY ([HomeTeamId]) REFERENCES [dbo].[Teams] ([TeamId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[SeasonTeams] (
        [SeasonTeamId] int NOT NULL IDENTITY,
        [SeasonId] int NOT NULL,
        [TeamId] int NOT NULL,
        CONSTRAINT [PK_SeasonTeams] PRIMARY KEY ([SeasonTeamId]),
        CONSTRAINT [FK_SeasonTeams_Seasons_SeasonId] FOREIGN KEY ([SeasonId]) REFERENCES [dbo].[Seasons] ([SeasonId]) ON DELETE CASCADE,
        CONSTRAINT [FK_SeasonTeams_Teams_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams] ([TeamId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[MatchEvents] (
        [MatchEventId] int NOT NULL IDENTITY,
        [MatchId] int NOT NULL,
        [TeamId] int NOT NULL,
        [ActionType] int NOT NULL,
        [Minute] int NOT NULL,
        [Description] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_MatchEvents] PRIMARY KEY ([MatchEventId]),
        CONSTRAINT [CK_Event_Action] CHECK (ActionType IN (1,2,3,4)),
        CONSTRAINT [CK_Event_Minute] CHECK (Minute BETWEEN 0 AND 130),
        CONSTRAINT [FK_MatchEvents_Matches_MatchId] FOREIGN KEY ([MatchId]) REFERENCES [dbo].[Matches] ([MatchId]) ON DELETE CASCADE,
        CONSTRAINT [FK_MatchEvents_Teams_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams] ([TeamId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[MatchStatistics] (
        [MatchStatisticId] int NOT NULL IDENTITY,
        [MatchId] int NOT NULL,
        [FirstHalfHomeGoals] int NOT NULL,
        [FirstHalfAwayGoals] int NOT NULL,
        [SecondHalfHomeGoals] int NOT NULL,
        [SecondHalfAwayGoals] int NOT NULL,
        [HomePossession] int NOT NULL,
        [HomeShots] int NOT NULL,
        [AwayShots] int NOT NULL,
        [HomeCorners] int NOT NULL,
        [AwayCorners] int NOT NULL,
        CONSTRAINT [PK_MatchStatistics] PRIMARY KEY ([MatchStatisticId]),
        CONSTRAINT [FK_MatchStatistics_Matches_MatchId] FOREIGN KEY ([MatchId]) REFERENCES [dbo].[Matches] ([MatchId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Matches_AwayTeamId] ON [dbo].[Matches] ([AwayTeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Matches_HomeTeamId] ON [dbo].[Matches] ([HomeTeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Matches_SeasonId_HomeTeamId_AwayTeamId] ON [dbo].[Matches] ([SeasonId], [HomeTeamId], [AwayTeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Matches_StadiumId] ON [dbo].[Matches] ([StadiumId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MatchEvents_MatchId] ON [dbo].[MatchEvents] ([MatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MatchEvents_TeamId] ON [dbo].[MatchEvents] ([TeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MatchStatistics_MatchId] ON [dbo].[MatchStatistics] ([MatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Seasons_LeagueId] ON [dbo].[Seasons] ([LeagueId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SeasonTeams_SeasonId_TeamId] ON [dbo].[SeasonTeams] ([SeasonId], [TeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SeasonTeams_TeamId] ON [dbo].[SeasonTeams] ([TeamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020112426_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020112426_InitialCreate', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    ALTER TABLE [dbo].[Matches] DROP CONSTRAINT [FK_Matches_Stadiums_StadiumId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    ALTER TABLE [dbo].[Seasons] ADD [DataUpdatedAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    ALTER TABLE [dbo].[Seasons] ADD [DataVersion] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    ALTER TABLE [dbo].[Seasons] ADD [SourceUrl] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'SecondHalfHomeGoals');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [SecondHalfHomeGoals] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'SecondHalfAwayGoals');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [SecondHalfAwayGoals] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'HomeShots');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [HomeShots] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'HomePossession');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [HomePossession] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'HomeCorners');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [HomeCorners] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'FirstHalfHomeGoals');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [FirstHalfHomeGoals] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'FirstHalfAwayGoals');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [FirstHalfAwayGoals] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'AwayShots');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [AwayShots] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[MatchStatistics]') AND [c].[name] = N'AwayCorners');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[MatchStatistics] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [dbo].[MatchStatistics] ALTER COLUMN [AwayCorners] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    ALTER TABLE [dbo].[MatchEvents] ADD [AddedMinute] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Matches]') AND [c].[name] = N'StadiumId');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Matches] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [dbo].[Matches] ALTER COLUMN [StadiumId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    ALTER TABLE [dbo].[Matches] ADD [SourceMatchId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    ALTER TABLE [dbo].[Matches] ADD [TimeConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    EXEC(N'ALTER TABLE [dbo].[MatchEvents] ADD CONSTRAINT [CK_Event_AddedMinute] CHECK (AddedMinute BETWEEN 0 AND 30 AND (AddedMinute = 0 OR Minute IN (45,90,105,120)))');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Matches_SourceMatchId] ON [dbo].[Matches] ([SourceMatchId]) WHERE [SourceMatchId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    ALTER TABLE [dbo].[Matches] ADD CONSTRAINT [FK_Matches_Stadiums_StadiumId] FOREIGN KEY ([StadiumId]) REFERENCES [dbo].[Stadiums] ([StadiumId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020120130_CurrentSeasonSnapshot'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020120130_CurrentSeasonSnapshot', N'10.0.10');
END;

COMMIT;
GO

