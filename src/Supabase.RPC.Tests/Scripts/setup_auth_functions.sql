-- Definir explicitamente o esquema público
SET search_path TO public;

-- Função que verifica se o usuário está autenticado
CREATE OR REPLACE FUNCTION public.check_auth()
RETURNS boolean
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  -- Em um ambiente real, isso verificaria o token JWT
  -- Aqui, apenas simulamos a operação
  RETURN true;
END;
$$;

-- Função que retorna informações do usuário autenticado
CREATE OR REPLACE FUNCTION auth.get_auth_user()
RETURNS json
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  result json;
BEGIN
  -- Em um ambiente real, isso obteria informações do usuário a partir do token JWT
  -- Aqui, apenas simulamos a operação
  SELECT json_build_object(
    'id', 'user-123',
    'email', 'user@example.com',
    'role', 'authenticated'
  ) INTO result;
  
  RETURN result;
END;
$$;

-- Função que retorna o perfil do usuário autenticado
CREATE OR REPLACE FUNCTION public.get_my_profile()
RETURNS json
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  result json;
BEGIN
  -- Em um ambiente real, isso obteria o perfil do usuário a partir do token JWT
  -- Aqui, apenas simulamos a operação
  SELECT json_build_object(
    'id', 'user-123',
    'name', 'John Doe',
    'email', 'user@example.com',
    'avatar', 'https://example.com/avatar.jpg'
  ) INTO result;
  
  RETURN result;
END;
$$;

-- Conceder permissão para usuários autenticados
GRANT EXECUTE ON FUNCTION get_my_profile() TO authenticated; 