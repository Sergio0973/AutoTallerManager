DO $$
DECLARE
    item record;
    max_id bigint;
BEGIN
    FOR item IN
        SELECT
            table_schema,
            table_name,
            column_name,
            pg_get_serial_sequence(format('%I.%I', table_schema, table_name), column_name) AS sequence_name
        FROM information_schema.columns
        WHERE table_schema = 'public'
          AND column_name = 'Id'
          AND pg_get_serial_sequence(format('%I.%I', table_schema, table_name), column_name) IS NOT NULL
    LOOP
        EXECUTE format(
            'SELECT COALESCE(MAX(%I), 0) FROM %I.%I',
            item.column_name,
            item.table_schema,
            item.table_name)
        INTO max_id;

        EXECUTE format(
            'SELECT setval(%L, %s, true)',
            item.sequence_name,
            GREATEST(max_id, 1));
    END LOOP;
END $$;
