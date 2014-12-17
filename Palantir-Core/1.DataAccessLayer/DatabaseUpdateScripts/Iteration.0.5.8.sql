 CREATE OR REPLACE FUNCTION Rerunnable0_5_8() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;

BEGIN

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''userfilter'';

  IF tableExists = 0 THEN
    CREATE TABLE userfilter (
   userid INTEGER NOT NULL,
   json text
 );
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_8();
DROP FUNCTION Rerunnable0_5_8();