-- Set public schema explicitly
SET search_path TO public;

-- Function that processes a person object
CREATE OR REPLACE FUNCTION public.process_person(person jsonb)
RETURNS jsonb
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  first_name text;
  last_name text;
  age integer;
  address jsonb;
  result jsonb;
BEGIN
  -- Extrair os campos do objeto pessoa
  first_name := person->>'firstName';
  last_name := person->>'lastName';
  age := (person->>'age')::integer;
  address := person->'address';
  
  -- Construir o objeto de resposta
  SELECT jsonb_build_object(
    'fullName', first_name || ' ' || last_name,
    'isAdult', age >= 18,
    'location', (address->>'city') || ', ' || (address->>'state') || ', ' || (address->>'country')
  ) INTO result;
  
  RETURN result;
END;
$$;

-- Função que processa um array de números
CREATE OR REPLACE FUNCTION public.process_numbers(numbers integer[])
RETURNS jsonb
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  sum_value integer := 0;
  avg_value float := 0;
  count_value integer := 0;
  result jsonb;
BEGIN
  -- Calcular a soma
  SELECT sum(n) INTO sum_value FROM unnest(numbers) AS n;
  
  -- Calcular a média
  SELECT avg(n) INTO avg_value FROM unnest(numbers) AS n;
  
  -- Contar os elementos
  SELECT count(n) INTO count_value FROM unnest(numbers) AS n;
  
  -- Construir o objeto de resposta
  SELECT jsonb_build_object(
    'sum', sum_value,
    'average', avg_value,
    'count', count_value
  ) INTO result;
  
  RETURN result;
END;
$$;

-- Função que retorna um array de objetos
CREATE OR REPLACE FUNCTION public.get_people(count integer)
RETURNS jsonb[]
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  result jsonb[];
  i integer;
BEGIN
  -- Inicializar o array
  result := ARRAY[]::jsonb[];
  
  -- Preencher o array com objetos
  FOR i IN 1..count LOOP
    result := array_append(result, jsonb_build_object(
      'id', i,
      'name', 'Person ' || i,
      'age', 20 + i
    ));
  END LOOP;
  
  RETURN result;
END;
$$;

-- Função que processa um objeto aninhado complexo
CREATE OR REPLACE FUNCTION public.process_complex_object(data jsonb)
RETURNS jsonb
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  user_info jsonb;
  preferences jsonb;
  settings jsonb;
  result jsonb;
BEGIN
  -- Extrair os campos do objeto
  user_info := data->'userInfo';
  preferences := data->'preferences';
  settings := data->'settings';
  
  -- Construir o objeto de resposta
  SELECT jsonb_build_object(
    'username', user_info->>'username',
    'email', user_info->>'email',
    'theme', preferences->>'theme',
    'language', preferences->>'language',
    'notifications', settings->>'notifications',
    'privacy', settings->>'privacy'
  ) INTO result;
  
  RETURN result;
END;
$$;

-- Função que retorna um objeto complexo com base em um ID
CREATE OR REPLACE FUNCTION public.get_complex_object(id uuid)
RETURNS jsonb
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  result jsonb;
  items jsonb[];
BEGIN
  -- Criar alguns itens para o objeto complexo
  items := ARRAY[
    jsonb_build_object('id', 1, 'name', 'Item 1', 'value', 10.5),
    jsonb_build_object('id', 2, 'name', 'Item 2', 'value', 20.75),
    jsonb_build_object('id', 3, 'name', 'Item 3', 'value', 30.25)
  ];
  
  -- Construir o objeto complexo
  SELECT jsonb_build_object(
    'id', id,
    'createdAt', now() AT TIME ZONE 'UTC',
    'meta', jsonb_build_object(
      'version', '1.0',
      'type', 'test',
      'status', 'active'
    ),
    'items', to_jsonb(items)
  ) INTO result;
  
  RETURN result;
END;
$$; 