BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    ALTER TABLE [WeeklyMenus] ADD [MenuDate] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9743695Z'', [MenuDate] = NULL
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747275Z'', [MenuDate] = NULL
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747288Z'', [MenuDate] = NULL
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747291Z'', [MenuDate] = NULL
    WHERE [Id] = 4;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747293Z'', [MenuDate] = NULL
    WHERE [Id] = 5;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747308Z'', [MenuDate] = NULL
    WHERE [Id] = 6;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747310Z'', [MenuDate] = NULL
    WHERE [Id] = 7;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747313Z'', [MenuDate] = NULL
    WHERE [Id] = 8;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747315Z'', [MenuDate] = NULL
    WHERE [Id] = 9;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747318Z'', [MenuDate] = NULL
    WHERE [Id] = 10;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747321Z'', [MenuDate] = NULL
    WHERE [Id] = 11;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747323Z'', [MenuDate] = NULL
    WHERE [Id] = 12;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747325Z'', [MenuDate] = NULL
    WHERE [Id] = 13;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    EXEC(N'UPDATE [WeeklyMenus] SET [CreatedAt] = ''2025-12-18T18:37:00.9747327Z'', [MenuDate] = NULL
    WHERE [Id] = 14;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251218183705_AddMenuDate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251218183705_AddMenuDate', N'9.0.0');
END;

COMMIT;
GO



