-- ============================================================================
-- Pet Health API - Sprint 4
-- Remove as tabelas TB_PH_* criadas nas Sprints 1 a 3 para que a migration
-- da Sprint 4 recrie o esquema do zero (com a nova tabela TB_PH_USUARIO).
--
-- ATENÇÃO: apaga os dados de teste dessas tabelas.
-- NÃO remove a tabela __EFMigrationsHistory compartilhada com outros projetos:
-- esta API usa a tabela própria __EFMigrationsHistory_PetHealth.
--
-- Como executar: SQL Developer > conectar com seu usuário > colar > F5 (Run Script)
-- ============================================================================
BEGIN
    FOR tabela IN (
        SELECT table_name
          FROM user_tables
         WHERE table_name IN ('TB_PH_MEDICAMENTO', 'TB_PH_CONSULTA', 'TB_PH_VACINA',
                              'TB_PH_PET', 'TB_PH_TUTOR', 'TB_PH_USUARIO',
                              '__EFMigrationsHistory_PetHealth')
    ) LOOP
        EXECUTE IMMEDIATE 'DROP TABLE "' || tabela.table_name || '" CASCADE CONSTRAINTS PURGE';
        DBMS_OUTPUT.PUT_LINE('Removida: ' || tabela.table_name);
    END LOOP;
END;
/
