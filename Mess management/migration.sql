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
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(100) NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [PasswordSalt] nvarchar(max) NOT NULL,
        [Role] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE TABLE [WeeklyMenus] (
        [Id] int NOT NULL IDENTITY,
        [DayOfWeek] nvarchar(450) NOT NULL,
        [DishName] nvarchar(100) NOT NULL,
        [Price] decimal(10,2) NOT NULL,
        [MealType] nvarchar(450) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_WeeklyMenus] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE TABLE [Members] (
        [MemberId] int NOT NULL IDENTITY,
        [FullName] nvarchar(100) NOT NULL,
        [RoomNumber] nvarchar(20) NOT NULL,
        [JoinDate] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Members] PRIMARY KEY ([MemberId]),
        CONSTRAINT [FK_Members_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE TABLE [PasswordResetTokens] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Code] nvarchar(6) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsUsed] bit NOT NULL,
        CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PasswordResetTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE TABLE [Attendances] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [Date] datetime2 NOT NULL,
        [BreakfastPresent] bit NOT NULL,
        [LunchPresent] bit NOT NULL,
        [DinnerPresent] bit NOT NULL,
        [MarkedBy] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Attendances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Attendances_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([MemberId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Attendances_Users_MarkedBy] FOREIGN KEY ([MarkedBy]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [Amount] decimal(10,2) NOT NULL,
        [Date] datetime2 NOT NULL,
        [PaymentMode] nvarchar(max) NOT NULL,
        [StripePaymentId] nvarchar(200) NULL,
        [Notes] nvarchar(500) NULL,
        [Status] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([MemberId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE TABLE [WaterTeaRecords] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [Date] datetime2 NOT NULL,
        [WaterCount] int NOT NULL,
        [TeaCount] int NOT NULL,
        [Cost] decimal(10,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_WaterTeaRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_WaterTeaRecords_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([MemberId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'DayOfWeek', N'DishName', N'MealType', N'Price', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[WeeklyMenus]'))
        SET IDENTITY_INSERT [WeeklyMenus] ON;
    EXEC(N'INSERT INTO [WeeklyMenus] ([Id], [CreatedAt], [DayOfWeek], [DishName], [MealType], [Price], [UpdatedAt])
    VALUES (1, ''2025-12-14T12:01:55.6075829Z'', N''Monday'', N''Rice with Dal'', N''Lunch'', 50.0, NULL),
    (2, ''2025-12-14T12:01:55.6077965Z'', N''Monday'', N''Roti with Sabzi'', N''Dinner'', 45.0, NULL),
    (3, ''2025-12-14T12:01:55.6077970Z'', N''Tuesday'', N''Biryani'', N''Lunch'', 80.0, NULL),
    (4, ''2025-12-14T12:01:55.6077972Z'', N''Tuesday'', N''Chapati with Curry'', N''Dinner'', 50.0, NULL),
    (5, ''2025-12-14T12:01:55.6077974Z'', N''Wednesday'', N''Fried Rice'', N''Lunch'', 60.0, NULL),
    (6, ''2025-12-14T12:01:55.6077981Z'', N''Wednesday'', N''Paratha with Curd'', N''Dinner'', 55.0, NULL),
    (7, ''2025-12-14T12:01:55.6077983Z'', N''Thursday'', N''Paneer Curry with Rice'', N''Lunch'', 75.0, NULL),
    (8, ''2025-12-14T12:01:55.6077984Z'', N''Thursday'', N''Khichdi'', N''Dinner'', 40.0, NULL),
    (9, ''2025-12-14T12:01:55.6077986Z'', N''Friday'', N''Chicken Curry'', N''Lunch'', 100.0, NULL),
    (10, ''2025-12-14T12:01:55.6077989Z'', N''Friday'', N''Egg Curry with Rice'', N''Dinner'', 70.0, NULL),
    (11, ''2025-12-14T12:01:55.6077990Z'', N''Saturday'', N''Special Thali'', N''Lunch'', 120.0, NULL),
    (12, ''2025-12-14T12:01:55.6077992Z'', N''Saturday'', N''Pulao'', N''Dinner'', 65.0, NULL),
    (13, ''2025-12-14T12:01:55.6077994Z'', N''Sunday'', N''Fish Curry'', N''Lunch'', 110.0, NULL),
    (14, ''2025-12-14T12:01:55.6077995Z'', N''Sunday'', N''Mixed Veg Rice'', N''Dinner'', 55.0, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'DayOfWeek', N'DishName', N'MealType', N'Price', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[WeeklyMenus]'))
        SET IDENTITY_INSERT [WeeklyMenus] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Attendances_MarkedBy] ON [Attendances] ([MarkedBy]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Attendances_MemberId_Date] ON [Attendances] ([MemberId], [Date]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Members_RoomNumber] ON [Members] ([RoomNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Members_UserId] ON [Members] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PasswordResetTokens_UserId] ON [PasswordResetTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payments_Date] ON [Payments] ([Date]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payments_MemberId] ON [Payments] ([MemberId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_WaterTeaRecords_MemberId_Date] ON [WaterTeaRecords] ([MemberId], [Date]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_WeeklyMenus_DayOfWeek_MealType] ON [WeeklyMenus] ([DayOfWeek], [MealType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214120159_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251214120159_InitialCreate', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    CREATE TABLE [PasswordHistories] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [PasswordSalt] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PasswordHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PasswordHistories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3586592Z''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589342Z''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589347Z''
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589349Z''
    WHERE [Id] = 4;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589351Z''
    WHERE [Id] = 5;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589362Z''
    WHERE [Id] = 6;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589364Z''
    WHERE [Id] = 7;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589365Z''
    WHERE [Id] = 8;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589367Z''
    WHERE [Id] = 9;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589370Z''
    WHERE [Id] = 10;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589371Z''
    WHERE [Id] = 11;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589373Z''
    WHERE [Id] = 12;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589374Z''
    WHERE [Id] = 13;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-14T22:27:57.3589376Z''
    WHERE [Id] = 14;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    CREATE INDEX [IX_PasswordHistories_UserId] ON [PasswordHistories] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251214222758_AddPasswordHistory'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251214222758_AddPasswordHistory', N'9.0.0');
END;

COMMIT;
GO

