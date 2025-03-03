-- Set public schema explicitly
SET search_path TO public;

-- Function that throws a business error
CREATE OR REPLACE FUNCTION public.throw_business_error(error_message text)
RETURNS void
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  RAISE EXCEPTION '%', error_message;
END;
$$;

-- Função que lança um erro quando o parâmetro é inválido
CREATE OR REPLACE FUNCTION public.validate_positive_number(num integer)
RETURNS integer
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  IF num <= 0 THEN
    RAISE EXCEPTION 'Number must be positive, got %', num;
  END IF;
  
  RETURN num;
END;
$$;

-- Função que lança um erro quando o parâmetro está fora do intervalo
CREATE OR REPLACE FUNCTION public.check_range(value integer, min_value integer, max_value integer)
RETURNS boolean
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  IF value < min_value OR value > max_value THEN
    RAISE EXCEPTION 'Value % is outside the range [%, %]', value, min_value, max_value;
  END IF;
  
  RETURN true;
END;
$$;

-- Função que lança um erro quando o parâmetro é nulo
CREATE OR REPLACE FUNCTION public.require_non_null(value text)
RETURNS text
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  IF value IS NULL THEN
    RAISE EXCEPTION 'Value cannot be null';
  END IF;
  
  RETURN value;
END;
$$;

-- Função que tenta consultar uma tabela
CREATE OR REPLACE FUNCTION query_table(table_name text)
RETURNS json
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  result json;
BEGIN
  -- Isso vai falhar se a tabela não existir
  EXECUTE format('SELECT json_agg(t) FROM %I t', table_name) INTO result;
  
  RETURN result;
END;
$$;

-- Função que retorna um texto
CREATE OR REPLACE FUNCTION return_text(text text)
RETURNS text
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
  RETURN text;
END;
$$; 