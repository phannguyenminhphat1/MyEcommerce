BEGIN TRANSACTION;
EXEC sp_rename N'[AppProducts].[Visiblity]', N'Visibility', 'COLUMN';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260529060308_UpdateVisibilityProduct', N'10.0.2');

COMMIT;
GO