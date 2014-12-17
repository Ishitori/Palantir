CREATE OR REPLACE FUNCTION Rerunnable0_5_4() RETURNS INTEGER AS '

DECLARE indexExists INTEGER;
DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;

BEGIN

  DROP INDEX ix_topic_vkgroupid_vkid;

  CREATE UNIQUE INDEX ix_topic_vkgroupid_vkid
    ON topic
    USING btree
    (vkgroupid , vkid COLLATE pg_catalog."default" );

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''photo'' and column_name=''likescount'';

  IF columnExists = 0 THEN
    ALTER TABLE photo ADD COLUMN likescount integer
  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''photo'' and column_name=''commentscount'';

  IF columnExists = 0 THEN
    ALTER TABLE photo ADD COLUMN commentscount integer
  END IF;

/*  -- Create index on member if required
  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_member_vkmemberid'';

  IF indexExists = 0 THEN
    CREATE INDEX ix_member_vkmemberid ON member USING btree (vkmemberid);
  END IF;

  -- Create index on member interest if required
  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_memberinterest_vkmemberid'';

  IF indexExists = 0 THEN
    CREATE INDEX ix_memberinterest_vkmemberid ON memberinterest USING btree (vkmemberid);
  END IF;*/

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_4();
DROP FUNCTION Rerunnable0_5_4();