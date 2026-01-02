CREATE DATABASE RappiOrdersDb;
GO

USE RappiOrdersDb;
GO

CREATE TABLE dbo.Orders (
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Orders PRIMARY KEY,

    OrderCode NVARCHAR(50) NOT NULL
        CONSTRAINT UQ_Orders_OrderCode UNIQUE,

    AggregatorOrder NVARCHAR(50) NOT NULL,

    [Description] NVARCHAR(200) NOT NULL,

    CreatedAt DATETIMEOFFSET NOT NULL,

    Status NVARCHAR(15) NOT NULL,

    Value DECIMAL(18,2) NOT NULL,

    CancelledAt DATETIMEOFFSET NULL,

    CreatedBy NVARCHAR(100) NOT NULL,
    UpdatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIMEOFFSET NULL,

    CONSTRAINT CK_Orders_Status
        CHECK (Status IN ('Created', 'Paid', 'Cancelled', 'Shipped')),

    CONSTRAINT CK_Orders_Value
        CHECK (Value >= 0),

    CONSTRAINT CK_Orders_CancelledAt
        CHECK (Status <> 'Cancelled' OR CancelledAt IS NOT NULL)
);
GO

