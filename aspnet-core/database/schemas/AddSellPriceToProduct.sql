BEGIN TRANSACTION;
ALTER TABLE [AppProducts] ADD [SellPrice] float NOT NULL DEFAULT 0.0E0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260527074032_AddSellPriceToProduct', N'10.0.2');

COMMIT;
GO