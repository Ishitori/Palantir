CREATE OR REPLACE FUNCTION Rerunnable0_5_6() RETURNS INTEGER AS '

DECLARE indexExists INTEGER;

BEGIN

  -- Create index on memberinterest if required
  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_post_vkgroupid_creatorid_posteddate'';

  IF indexExists = 0 THEN
    CREATE INDEX ix_post_vkgroupid_creatorid_posteddate ON post USING btree (vkgroupid, creatorid, posteddate);
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_6();
DROP FUNCTION Rerunnable0_5_6();

