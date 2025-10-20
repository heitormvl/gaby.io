-- Script para criar o primeiro administrador
-- Substitua '[EMAIL_DO_USUARIO]' pelo email do usuário que você deseja tornar administrador

-- 1. Criar a role Admin (se não existir)
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    DECLARE @AdminRoleId NVARCHAR(450) = NEWID()
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@AdminRoleId, 'Admin', 'ADMIN', NEWID())
    PRINT 'Role Admin criada com sucesso'
END
ELSE
BEGIN
    PRINT 'Role Admin já existe'
END

-- 2. Adicionar o usuário à role Admin
DECLARE @UserId NVARCHAR(450)
DECLARE @RoleId NVARCHAR(450)
DECLARE @UserEmail NVARCHAR(256) = '[EMAIL_DO_USUARIO]' -- ALTERE AQUI

SELECT @UserId = Id FROM AspNetUsers WHERE Email = @UserEmail
SELECT @RoleId = Id FROM AspNetRoles WHERE Name = 'Admin'

IF @UserId IS NULL
BEGIN
    PRINT 'ERRO: Usuário com email ' + @UserEmail + ' não encontrado'
END
ELSE IF @RoleId IS NULL
BEGIN
    PRINT 'ERRO: Role Admin não encontrada'
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT * FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@UserId, @RoleId)
        PRINT 'Usuário adicionado à role Admin com sucesso'
    END
    ELSE
    BEGIN
        PRINT 'Usuário já é um administrador'
    END
END

-- 3. Verificar todos os administradores
SELECT 
    u.Email,
    u.DisplayName,
    r.Name as Role
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Admin'
