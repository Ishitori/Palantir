CREATE OR REPLACE FUNCTION Rerunnable0_6_4() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;
DECLARE indexExists INTEGER;

BEGIN

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''account'' and column_name=''candeleteprojects'';

  IF columnExists = 0 THEN
	ALTER TABLE account ADD COLUMN candeleteprojects boolean default(true);
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_6_4();
DROP FUNCTION Rerunnable0_6_4();