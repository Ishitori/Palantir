 CREATE OR REPLACE FUNCTION Rerunnable0_5_7() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;

BEGIN

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''membersdelta'';

  IF tableExists = 0 THEN
    CREATE TABLE membersdelta (
   id INTEGER NOT NULL,
   vkgroupid INTEGER DEFAULT 0 NOT NULL,
   posteddate timestamp NOT NULL,
   second int DEFAULT 0,
   minute int DEFAULT 0,
   hour int DEFAULT 0,
   week int DEFAULT 0,
   day int DEFAULT 0,
   month int DEFAULT 0,
   year int DEFAULT 0,
   inIds text,
   inCount int DEFAULT 0,
   outIds text,
   outCount int DEFAULT 0
 );
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_7();
DROP FUNCTION Rerunnable0_5_7();