-- Set public schema explicitly
SET search_path TO public;

-- Function that adds two numbers
CREATE OR REPLACE FUNCTION public.add_numbers(a integer, b integer)
RETURNS integer
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  RETURN a + b;
END;
$$;

-- Função que cria um objeto de informações do usuário
CREATE OR REPLACE FUNCTION public.create_user_info(name text, email text)
RETURNS json
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  result json;
BEGIN
  SELECT json_build_object(
    'name', name,
    'email', email,
    'createdAt', now() AT TIME ZONE 'UTC'
  ) INTO result;
  
  RETURN result;
END;
$$;

-- Função que gera uma série de números
CREATE OR REPLACE FUNCTION public.generate_series(count integer)
RETURNS integer[]
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  result integer[];
BEGIN
  SELECT array_agg(i) INTO result
  FROM generate_series(1, count) AS i;
  
  RETURN result;
END;
$$;

-- Função que retorna o timestamp atual
CREATE OR REPLACE FUNCTION public.get_current_timestamp()
RETURNS timestamp with time zone
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  RETURN now() AT TIME ZONE 'UTC';
END;
$$;

-- Função que não retorna valor
CREATE OR REPLACE FUNCTION public.log_message(message text)
RETURNS void
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  -- Em um ambiente real, isso poderia registrar em uma tabela de log
  -- Aqui, apenas simulamos a operação
  PERFORM pg_sleep(0.1);
  RETURN;
END;
$$; 