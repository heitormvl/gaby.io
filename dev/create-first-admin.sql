-- Script para criar o primeiro administrador
-- Substitua '[EMAIL_DO_USUARIO]' pelo email do usuário que você deseja tornar administrador

DO $$
DECLARE
    admin_role_id TEXT;
    target_user_id TEXT;
    user_email TEXT := '[EMAIL_DO_USUARIO]'; -- ALTERE AQUI
BEGIN
    -- 1. Criar a role Admin (se não existir)
    IF NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'Admin') THEN
        admin_role_id := gen_random_uuid()::text;
        INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
        VALUES (admin_role_id, 'Admin', 'ADMIN', gen_random_uuid()::text);
        RAISE NOTICE 'Role Admin criada com sucesso';
    ELSE
        RAISE NOTICE 'Role Admin já existe';
    END IF;

    -- 2. Adicionar o usuário à role Admin
    SELECT "Id" INTO target_user_id FROM "AspNetUsers" WHERE "Email" = user_email;
    SELECT "Id" INTO admin_role_id FROM "AspNetRoles" WHERE "Name" = 'Admin';

    IF target_user_id IS NULL THEN
        RAISE NOTICE 'ERRO: Usuário com email % não encontrado', user_email;
    ELSIF admin_role_id IS NULL THEN
        RAISE NOTICE 'ERRO: Role Admin não encontrada';
    ELSE
        IF NOT EXISTS (
            SELECT 1 FROM "AspNetUserRoles"
            WHERE "UserId" = target_user_id AND "RoleId" = admin_role_id
        ) THEN
            INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
            VALUES (target_user_id, admin_role_id);
            RAISE NOTICE 'Usuário adicionado à role Admin com sucesso';
        ELSE
            RAISE NOTICE 'Usuário já é um administrador';
        END IF;
    END IF;
END $$;

-- 3. Verificar todos os administradores
SELECT
    u."Email",
    u."DisplayName",
    r."Name" as "Role"
FROM "AspNetUsers" u
INNER JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
INNER JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
WHERE r."Name" = 'Admin';
