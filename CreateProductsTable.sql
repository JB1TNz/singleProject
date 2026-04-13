-- รันสคริปต์นี้ใน SQL Server Management Studio (SSMS) หรือ Azure Data Studio
-- เพื่อสร้างตาราง Products สำหรับ Database EBookBest

USE EBookBest;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        ProductId        INT IDENTITY(1,1) PRIMARY KEY,
        ProductName      NVARCHAR(200)      NULL,
        ProductDescription NVARCHAR(2000)   NULL,
        Price            DECIMAL(18,2)      NULL,
        FilePath         NVARCHAR(500)      NULL,
        CoverPicture     NVARCHAR(500)      NULL,
        SellerId         NVARCHAR(10)       NULL,
        CategoryId       INT                NULL,
        Status           INT                NULL DEFAULT 1,
        CreatedDate      DATETIME           NULL DEFAULT GETDATE(),
        UpdatedDate      DATETIME           NULL DEFAULT GETDATE(),

        CONSTRAINT FK_Products_Seller FOREIGN KEY (SellerId)
            REFERENCES UserData(UserId)
    );

    PRINT 'สร้างตาราง Products สำเร็จ';
END
ELSE
BEGIN
    PRINT 'ตาราง Products มีอยู่แล้ว';
END
GO
