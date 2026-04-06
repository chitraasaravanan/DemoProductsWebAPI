-- Create Product and Item tables if they do not already exist

IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE t.name = 'Product' AND s.name = 'dbo')
BEGIN
    CREATE TABLE [dbo].[Product]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
        [ProductName] NVARCHAR(255) NOT NULL,
        [CreatedBy]  NVARCHAR(100) NOT NULL,
        [CreatedOn]  DATETIME NOT NULL,
        [ModifiedBy]  NVARCHAR(100) NULL,
        [ModifiedOn]  DATETIME NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE t.name = 'Item' AND s.name = 'dbo')
BEGIN
    CREATE TABLE [dbo].[Item]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
        [ProductId] INT NOT NULL,
        [Quantity] INT NOT NULL,
        CONSTRAINT FK_Item_Product FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product]([Id]) ON DELETE CASCADE
    );
END

-- Optional: add an index on ProductName for faster lookups
IF NOT EXISTS (SELECT 1 FROM sys.indexes i JOIN sys.tables t ON i.object_id = t.object_id WHERE t.name = 'Product' AND i.name = 'IX_Product_ProductName')
BEGIN
    CREATE INDEX IX_Product_ProductName ON [dbo].[Product]([ProductName]);
END
