-- =============================================================================
--  Pet Health API - Evidências de CRUD no Oracle (vídeo da Sprint 4 DevOps)
--  Rodar no SQL Developer conectado em oracle.fiap.com.br:1521/ORCL (usuário RM)
--  Selecione a linha desejada e pressione Ctrl+Enter.
-- =============================================================================

-- 0) Estrutura criada pela migration Sprint4Inicial
SELECT table_name FROM user_tables WHERE table_name LIKE 'TB_PH_%' ORDER BY table_name;

-- 1) Tutores (após POST / PUT / DELETE)
SELECT ID_TUTOR, NM_TUTOR, DS_EMAIL, NR_TELEFONE, DS_ENDERECO, DT_CADASTRO
  FROM TB_PH_TUTOR
 ORDER BY ID_TUTOR;

-- 2) Pets com o nome do tutor (após POST / PUT / DELETE)
SELECT p.ID_PET, p.NM_PET, p.DS_ESPECIE, p.DS_RACA, p.NR_IDADE, p.NR_PESO,
       p.FL_CASTRADO, p.ID_TUTOR, t.NM_TUTOR
  FROM TB_PH_PET p
  JOIN TB_PH_TUTOR t ON t.ID_TUTOR = p.ID_TUTOR
 ORDER BY p.ID_PET;

-- 3) Contagem rápida
SELECT (SELECT COUNT(*) FROM TB_PH_TUTOR) AS TOTAL_TUTORES,
       (SELECT COUNT(*) FROM TB_PH_PET)   AS TOTAL_PETS
  FROM DUAL;

-- 4) Administrador criado automaticamente pela API (a senha é guardada só como hash)
SELECT ID_USUARIO, NM_USUARIO, DS_EMAIL, DS_PERFIL FROM TB_PH_USUARIO;
