BEGIN TRANSACTION;
CREATE TABLE [Suggestions] (
    [SuggestionId] int NOT NULL IDENTITY,
    [MemberId] int NOT NULL,
    [Subject] nvarchar(100) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [Status] nvarchar(450) NOT NULL,
    [Priority] nvarchar(max) NOT NULL,
    [AdminResponse] nvarchar(1000) NULL,
    [RespondedByUserId] int NULL,
    [RespondedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsAnonymous] bit NOT NULL,
    CONSTRAINT [PK_Suggestions] PRIMARY KEY ([SuggestionId]),
    CONSTRAINT [FK_Suggestions_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([MemberId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Suggestions_Users_RespondedByUserId] FOREIGN KEY ([RespondedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9268124Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272674Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272684Z'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272687Z'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272689Z'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272835Z'
WHERE [Id] = 6;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272838Z'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272843Z'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272845Z'
WHERE [Id] = 9;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272856Z'
WHERE [Id] = 10;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272858Z'
WHERE [Id] = 11;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272863Z'
WHERE [Id] = 12;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272866Z'
WHERE [Id] = 13;
SELECT @@ROWCOUNT;


UPDATE [WeeklyMenus] SET [CreatedAt] = '2026-01-02T11:50:17.9272868Z'
WHERE [Id] = 14;
SELECT @@ROWCOUNT;


CREATE INDEX [IX_Suggestions_CreatedAt] ON [Suggestions] ([CreatedAt]);

CREATE INDEX [IX_Suggestions_MemberId] ON [Suggestions] ([MemberId]);

CREATE INDEX [IX_Suggestions_RespondedByUserId] ON [Suggestions] ([RespondedByUserId]);

CREATE INDEX [IX_Suggestions_Status] ON [Suggestions] ([Status]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260102115021_AddSuggestions', N'9.0.0');

COMMIT;
GO

