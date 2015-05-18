
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/08/2015 01:28:20
-- Generated from EDMX file: E:\github\BoomerangX\MockComponents\MockApplication\Data\ApplicationEntities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [aspnet-MockApplication-20150307072105];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Products]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Products];
GO
IF OBJECT_ID(N'[dbo].[SalesOrderDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SalesOrderDetails];
GO
IF OBJECT_ID(N'[dbo].[SalesOrders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SalesOrders];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Products'
CREATE TABLE [dbo].[Products] (
    [ProductId] bigint  NOT NULL,
    [ProductName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SalesOrderDetails'
CREATE TABLE [dbo].[SalesOrderDetails] (
    [SalesOrderHeaderId] bigint  NOT NULL,
    [ProductId] bigint  NOT NULL,
    [SalesOrderDetailId] bigint  NOT NULL,
    [CustomerNumber] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SalesOrders'
CREATE TABLE [dbo].[SalesOrders] (
    [SalesOrderId] bigint  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [CustomerId] bigint  NOT NULL,
    [CustomerNumber] nvarchar(max)  NOT NULL,
    [CustomerName] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ProductId] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [PK_Products]
    PRIMARY KEY CLUSTERED ([ProductId] ASC);
GO

-- Creating primary key on [ProductId], [SalesOrderDetailId] in table 'SalesOrderDetails'
ALTER TABLE [dbo].[SalesOrderDetails]
ADD CONSTRAINT [PK_SalesOrderDetails]
    PRIMARY KEY CLUSTERED ([ProductId], [SalesOrderDetailId] ASC);
GO

-- Creating primary key on [SalesOrderId] in table 'SalesOrders'
ALTER TABLE [dbo].[SalesOrders]
ADD CONSTRAINT [PK_SalesOrders]
    PRIMARY KEY CLUSTERED ([SalesOrderId] ASC);
GO

-- Creating primary key on [CustomerId] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([CustomerId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------