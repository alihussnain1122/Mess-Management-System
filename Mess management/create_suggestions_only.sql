-- Create Suggestions table only
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Suggestions' and xtype='U')
BEGIN
    CREATE TABLE [Suggestions] (
        [SuggestionId] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [Subject] nvarchar(100) NOT NULL,
        [Message] nvarchar(1000) NOT NULL,
        [Category] nvarchar(50) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [Priority] nvarchar(50) NOT NULL,
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

    -- Create indexes
    CREATE INDEX [IX_Suggestions_MemberId] ON [Suggestions] ([MemberId]);
    CREATE INDEX [IX_Suggestions_RespondedByUserId] ON [Suggestions] ([RespondedByUserId]);
    CREATE INDEX [IX_Suggestions_Status] ON [Suggestions] ([Status]);
    
    PRINT 'Suggestions table created successfully.'
END
ELSE
BEGIN
    PRINT 'Suggestions table already exists.'
END
