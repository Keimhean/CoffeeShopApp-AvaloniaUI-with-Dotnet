-- Initialize CoffeeShopDB schema and seed data

IF DB_ID('CoffeeShopDB') IS NULL
BEGIN
    CREATE DATABASE CoffeeShopDB;
END
GO

USE CoffeeShopDB;
GO

-- Products table
IF OBJECT_ID('dbo.Products', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products (
        Id        INT IDENTITY(1,1) PRIMARY KEY,
        Name      NVARCHAR(100) NOT NULL,
        Category  NVARCHAR(100) NULL,
        Price     DECIMAL(18,2) NOT NULL,
        Stock     INT NOT NULL DEFAULT 0
    );
END
GO

-- Orders table
IF OBJECT_ID('dbo.Orders', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        OrderNumber NVARCHAR(50) NOT NULL,
        Cashier     NVARCHAR(100) NULL,
        CreatedAt   DATETIME2 NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END
GO

-- OrderItems table
IF OBJECT_ID('dbo.OrderItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems (
        Id         INT IDENTITY(1,1) PRIMARY KEY,
        OrderId    INT NOT NULL,
        ProductId  INT NOT NULL,
        Qty        INT NOT NULL,
        UnitPrice  DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
    );
END
GO

-- Seed some products if none exist
IF NOT EXISTS (SELECT 1 FROM dbo.Products)
BEGIN
    INSERT INTO dbo.Products (Name, Category, Price, Stock)
    VALUES
        (N'Espresso', N'Coffee', 2.50, 50),
        (N'Latte', N'Coffee', 3.50, 50),
        (N'Cappuccino', N'Coffee', 3.25, 50),
        (N'Muffin', N'Pastry', 2.00, 40),
        (N'Croissant', N'Pastry', 2.50, 40);
END
GO

-- Sales summary view
IF OBJECT_ID('dbo.v_SalesSummary', 'V') IS NOT NULL
    DROP VIEW dbo.v_SalesSummary;
GO

CREATE VIEW dbo.v_SalesSummary AS
SELECT
    CAST(o.CreatedAt AS date) AS [Date],
    COUNT(DISTINCT o.Id) AS Orders,
    ISNULL(SUM(oi.Qty * oi.UnitPrice), 0) AS Revenue
FROM dbo.Orders o
LEFT JOIN dbo.OrderItems oi ON oi.OrderId = o.Id
GROUP BY CAST(o.CreatedAt AS date);
GO
