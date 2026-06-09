BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppProductAttributeVarchars]') AND [c].[name] = N'Value');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [AppProductAttributeVarchars] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [AppProductAttributeVarchars] ALTER COLUMN [Value] nvarchar(500) NULL;

DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppProductAttributeTexts]') AND [c].[name] = N'Value');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AppProductAttributeTexts] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [AppProductAttributeTexts] ALTER COLUMN [Value] nvarchar(max) NULL;

DECLARE @var2 nvarchar(max);
SELECT @var2 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppProductAttributes]') AND [c].[name] = N'Note');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AppProductAttributes] DROP CONSTRAINT ' + @var2 + ';');
ALTER TABLE [AppProductAttributes] ALTER COLUMN [Note] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609054337_UpdateFieldNullProductAttribute', N'10.0.2');

COMMIT;
GO