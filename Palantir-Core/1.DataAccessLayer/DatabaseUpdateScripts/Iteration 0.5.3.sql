CREATE OR REPLACE FUNCTION Rerunnable0_5_3() RETURNS INTEGER AS '

DECLARE indexExists INTEGER;
DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;

BEGIN
    -- Create index on photo if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_photo_albumid'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_photo_albumid ON photo USING btree (albumid );
    END IF;
    
    -- Create index on photo if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_photo_albumid_posteddate'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_photo_albumid_posteddate ON photo USING btree (albumid , posteddate desc);
    END IF;
  	
	SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''vkgroup'' and column_name=''smallphoto'';

    IF columnExists = 0 THEN
      ALTER TABLE vkgroup ADD COLUMN smallphoto CHARACTER VARYING(100)
    END IF;

	RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_3();
DROP FUNCTION Rerunnable0_5_3();