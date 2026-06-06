BEGIN TRANSACTION;
CREATE TABLE [AppIdentitySettings] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Prefix] nvarchar(50) NOT NULL,
    [CurrentNumber] int NOT NULL,
    [StepNumber] int NOT NULL,
    CONSTRAINT [PK_AppIdentitySettings] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260606044754_AddIdentitySettingTable', N'10.0.2');

COMMIT;
GO