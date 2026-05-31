BEGIN TRANSACTION;
CREATE INDEX [IX_AppProducts_CategoryId] ON [AppProducts] ([CategoryId]);

ALTER TABLE [AppProducts] ADD CONSTRAINT [FK_AppProducts_AppProductCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [AppProductCategories] ([Id]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260531050106_AddFKProductCategoryToProduct', N'10.0.2');

COMMIT;
GO