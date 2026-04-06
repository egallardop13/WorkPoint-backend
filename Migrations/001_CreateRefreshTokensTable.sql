CREATE TABLE WorkPointSchema.RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(256) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    RevokedAt DATETIME2 NULL,
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId)
        REFERENCES WorkPointSchema.Users(UserId) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_RefreshTokens_Token
    ON WorkPointSchema.RefreshTokens(Token);

CREATE INDEX IX_RefreshTokens_UserId
    ON WorkPointSchema.RefreshTokens(UserId);
