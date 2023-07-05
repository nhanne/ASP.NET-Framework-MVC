CREATE DATABASE Boutique
GO



CREATE TABLE Category(
	[Id]            INT            NOT NULL,
    [Code]   NVARCHAR (10)  NULL,
    [Name]   NVARCHAR (20) NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([ID] ASC)
);

CREATE TABLE Product(
	[Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CategoryId]  INT            NULL,
    [Picture]     NVARCHAR (MAX) NULL,
    [Name]		  NVARCHAR (100) NULL,
    [Code]		  VARCHAR (20)   NULL,
    [costPrice]   FLOAT (53)     NULL,
    [unitPrice]   FLOAT (53)     NULL,
    [Quantity]	  INT            NULL,
    [Sold]		  INT			 NULL,
    [Sale]        INT            NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id])
);

CREATE TABLE [dbo].[Customer](
	[Id]		INT            IDENTITY (1, 1) NOT NULL,
    [FullName]  NVARCHAR (50)  NULL,
    [Email]     VARCHAR  (50)  NULL,
    [Password]  VARCHAR  (50)  NULL,
    [Avatar]    NVARCHAR (MAX) NULL,
    [Address]   NVARCHAR (250) NULL,
    [DateOfBirth]  DATE        NULL,
    [CMT]       CHAR	(12)   NULL,
    [Phone]     CHAR	(10)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


CREATE TABLE [dbo].[OrderStatus](
	Status NVARCHAR PRIMARY KEY not null,
);

CREATE TABLE [dbo].[Order](
	[Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CustomerId]  INT            NULL,
    [Status]      NVARCHAR		 NULL,
    [Address]     NVARCHAR (250) NULL,
    [Payment]     BIT            NULL,
    [OrdTime]     DATE	         NULL,
    [DeliTime]    DATE			 NULL,
	[TotalPrice]  FLOAT			 NULL,
	[TotalQuantity] INT			 NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([Id]),
    FOREIGN KEY ([Status]) REFERENCES [dbo].[OrderStatus] ([Status])
);


CREATE TABLE OrderDetail(
	[OrderId]    INT        NOT NULL,
    [ProductId]  INT        NOT NULL,
    [Quantity]   INT        NULL,
    [unitPrice]  FLOAT (53) NULL,
    PRIMARY KEY CLUSTERED ([OrderId] ASC, [ProductId] ASC),
    FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([Id])
);

CREATE TABLE Staff(
	[Id]		INT            IDENTITY (1, 1) NOT NULL,
    [FullName]  NVARCHAR (50)  NULL,
    [Email]     VARCHAR  (50)  NULL,
    [Password]  VARCHAR  (50)  NULL,
    [Avatar]    NVARCHAR (MAX) NULL,
    [Address]   NVARCHAR (250) NULL,
    [DateOfBirth]  DATE        NULL,
    [CMT]       CHAR		   NULL,
    [Phone]     CHAR	       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);