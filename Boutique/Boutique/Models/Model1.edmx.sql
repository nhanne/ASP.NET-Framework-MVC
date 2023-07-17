
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/10/2023 23:01:10
-- Generated from EDMX file: C:\Users\nhanc\Desktop\BoutiqueStore\Boutique\Boutique\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Boutique];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__Order__CustomerI__4BAC3F29]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK__Order__CustomerI__4BAC3F29];
GO
IF OBJECT_ID(N'[dbo].[FK__Order__Status__4CA06362]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK__Order__Status__4CA06362];
GO
IF OBJECT_ID(N'[dbo].[FK__OrderDeta__Order__4F7CD00D]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK__OrderDeta__Order__4F7CD00D];
GO
IF OBJECT_ID(N'[dbo].[FK__OrderDeta__Produ__5070F446]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetail] DROP CONSTRAINT [FK__OrderDeta__Produ__5070F446];
GO
IF OBJECT_ID(N'[dbo].[FK__Product__Categor__267ABA7A]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK__Product__Categor__267ABA7A];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Category];
GO
IF OBJECT_ID(N'[dbo].[Customer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customer];
GO
IF OBJECT_ID(N'[dbo].[Order]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Order];
GO
IF OBJECT_ID(N'[dbo].[OrderDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderDetail];
GO
IF OBJECT_ID(N'[dbo].[OrderStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderStatus];
GO
IF OBJECT_ID(N'[dbo].[Product]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Product];
GO
IF OBJECT_ID(N'[dbo].[Sizes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sizes];
GO
IF OBJECT_ID(N'[dbo].[Staff]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Staff];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Categories'
CREATE TABLE [dbo].[Categories] (
    [Id] int  NOT NULL,
    [Code] nvarchar(10)  NULL,
    [Name] nvarchar(20)  NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FullName] nvarchar(50)  NULL,
    [Email] varchar(50)  NULL,
    [Password] varchar(50)  NULL,
    [Avatar] nvarchar(max)  NULL,
    [Address] nvarchar(250)  NULL,
    [Phone] char(10)  NULL,
    [Member] bit  NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CustomerId] int  NULL,
    [Status] nvarchar(1)  NULL,
    [Address] nvarchar(250)  NULL,
    [Payment] bit  NULL,
    [OrdTime] datetime  NULL,
    [DeliTime] datetime  NULL,
    [TotalPrice] float  NULL,
    [TotalQuantity] int  NULL
);
GO

-- Creating table 'OrderDetails'
CREATE TABLE [dbo].[OrderDetails] (
    [OrderId] int  NOT NULL,
    [ProductId] int  NOT NULL,
    [Quantity] int  NULL,
    [unitPrice] float  NULL
);
GO

-- Creating table 'OrderStatus'
CREATE TABLE [dbo].[OrderStatus] (
    [Status] nvarchar(1)  NOT NULL
);
GO

-- Creating table 'Products'
CREATE TABLE [dbo].[Products] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CategoryId] int  NULL,
    [Picture] nvarchar(max)  NULL,
    [Name] nvarchar(100)  NULL,
    [Code] varchar(20)  NULL,
    [costPrice] float  NULL,
    [unitPrice] float  NULL,
    [Quantity] int  NULL,
    [Sold] int  NULL,
    [Sale] int  NULL,
    [stockInDate] datetime  NULL
);
GO

-- Creating table 'Staffs'
CREATE TABLE [dbo].[Staffs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FullName] nvarchar(50)  NULL,
    [Email] varchar(50)  NULL,
    [Password] varchar(50)  NULL,
    [Avatar] nvarchar(max)  NULL,
    [Address] nvarchar(250)  NULL,
    [DateOfBirth] datetime  NULL,
    [CMT] char(1)  NULL,
    [Phone] char(1)  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'Sizes'
CREATE TABLE [dbo].[Sizes] (
    [Id] int  NOT NULL,
    [Name] varchar(10)  NULL,
    [Ghichu] nvarchar(50)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [PK_Categories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [OrderId], [ProductId] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [PK_OrderDetails]
    PRIMARY KEY CLUSTERED ([OrderId], [ProductId] ASC);
GO

-- Creating primary key on [Status] in table 'OrderStatus'
ALTER TABLE [dbo].[OrderStatus]
ADD CONSTRAINT [PK_OrderStatus]
    PRIMARY KEY CLUSTERED ([Status] ASC);
GO

-- Creating primary key on [Id] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [PK_Products]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Staffs'
ALTER TABLE [dbo].[Staffs]
ADD CONSTRAINT [PK_Staffs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'Sizes'
ALTER TABLE [dbo].[Sizes]
ADD CONSTRAINT [PK_Sizes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [CategoryId] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [FK__Product__Categor__267ABA7A]
    FOREIGN KEY ([CategoryId])
    REFERENCES [dbo].[Categories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Product__Categor__267ABA7A'
CREATE INDEX [IX_FK__Product__Categor__267ABA7A]
ON [dbo].[Products]
    ([CategoryId]);
GO

-- Creating foreign key on [CustomerId] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK__Order__CustomerI__2D27B809]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Order__CustomerI__2D27B809'
CREATE INDEX [IX_FK__Order__CustomerI__2D27B809]
ON [dbo].[Orders]
    ([CustomerId]);
GO

-- Creating foreign key on [Status] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK__Order__Status__2E1BDC42]
    FOREIGN KEY ([Status])
    REFERENCES [dbo].[OrderStatus]
        ([Status])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Order__Status__2E1BDC42'
CREATE INDEX [IX_FK__Order__Status__2E1BDC42]
ON [dbo].[Orders]
    ([Status]);
GO

-- Creating foreign key on [OrderId] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK__OrderDeta__Order__30F848ED]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[Orders]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ProductId] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK__OrderDeta__Produ__31EC6D26]
    FOREIGN KEY ([ProductId])
    REFERENCES [dbo].[Products]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__OrderDeta__Produ__31EC6D26'
CREATE INDEX [IX_FK__OrderDeta__Produ__31EC6D26]
ON [dbo].[OrderDetails]
    ([ProductId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------