-- =============================================
-- Script para crear tabla y stored procedures de RefreshTokens
-- =============================================

-- Crear tabla RefreshTokens
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
    CREATE TABLE RefreshTokens (
        Token NVARCHAR(200) PRIMARY KEY,
        UserId BIGINT NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        IsRevoked BIT NOT NULL DEFAULT 0,
        RevokedAt DATETIME2 NULL,
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );

    CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
    CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
END
GO

-- =============================================
-- SP: Obtener RefreshToken por token
-- =============================================
CREATE OR ALTER PROCEDURE usp_getRefreshToken
    @token NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Token,
        UserId,
        ExpiresAt,
        CreatedAt,
        IsRevoked
    FROM RefreshTokens
    WHERE Token = @token;
END
GO

-- =============================================
-- SP: Guardar RefreshToken
-- =============================================
CREATE OR ALTER PROCEDURE usp_saveRefreshToken
    @token NVARCHAR(200),
    @userId BIGINT,
    @expiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO RefreshTokens (Token, UserId, ExpiresAt, CreatedAt)
    VALUES (@token, @userId, @expiresAt, GETUTCDATE());
END
GO

-- =============================================
-- SP: Revocar un RefreshToken específico
-- =============================================
CREATE OR ALTER PROCEDURE usp_revokeRefreshToken
    @token NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET IsRevoked = 1,
        RevokedAt = GETUTCDATE()
    WHERE Token = @token;
END
GO

-- =============================================
-- SP: Revocar todos los RefreshTokens de un usuario
-- =============================================
CREATE OR ALTER PROCEDURE usp_revokeAllUserRefreshTokens
    @userId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RefreshTokens
    SET IsRevoked = 1,
        RevokedAt = GETUTCDATE()
    WHERE UserId = @userId
      AND IsRevoked = 0;
END
GO

-- =============================================
-- SP: Limpiar tokens expirados (job de mantenimiento)
-- =============================================
CREATE OR ALTER PROCEDURE usp_cleanupExpiredRefreshTokens
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM RefreshTokens
    WHERE ExpiresAt < DATEADD(DAY, -30, GETUTCDATE())
       OR (IsRevoked = 1 AND RevokedAt < DATEADD(DAY, -7, GETUTCDATE()));
END
GO
