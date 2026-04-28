-- 创建 Tenants 表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tenants' and xtype='U')
BEGIN
    CREATE TABLE Tenants (
        Id NVARCHAR(450) PRIMARY KEY,
        TenantName NVARCHAR(500),
        TenantCode NVARCHAR(200),
        ContactPhone NVARCHAR(100),
        ContactPerson NVARCHAR(200),
        Address NVARCHAR(1000),
        Status INT,
        Remark NVARCHAR(MAX),
        Founder NVARCHAR(200),
        FounderDate DATETIME2,
        Modifier NVARCHAR(200),
        ModifierDate DATETIME2,
        IsDelete BIT NOT NULL DEFAULT 0
    )
    PRINT 'Tenants table created successfully.'
END
ELSE
BEGIN
    PRINT 'Tenants table already exists.'
END

-- 创建 UserTenants 表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserTenants' and xtype='U')
BEGIN
    CREATE TABLE UserTenants (
        Id NVARCHAR(450) PRIMARY KEY,
        UserId NVARCHAR(450),
        TenantId NVARCHAR(450)
    )
    PRINT 'UserTenants table created successfully.'
END
ELSE
BEGIN
    PRINT 'UserTenants table already exists.'
END

-- 验证表是否创建成功
SELECT name FROM sysobjects WHERE xtype='U' AND name IN ('Tenants', 'UserTenants')
