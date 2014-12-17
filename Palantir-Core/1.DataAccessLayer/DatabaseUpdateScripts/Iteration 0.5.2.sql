CREATE OR REPLACE FUNCTION Rerunnable0_5_2() RETURNS INTEGER AS '

DECLARE indexExists INTEGER;
DECLARE tableExists INTEGER;

BEGIN
	ALTER TABLE memberinterest ALTER COLUMN title TYPE text;

	ALTER TABLE topic ALTER COLUMN id SET DEFAULT nextval(''topic_id_seq''::regclass);
	
	ALTER TABLE topiccomment ALTER COLUMN id SET DEFAULT nextval(''topiccomment_id_seq''::regclass);

    -- Create index on memberinterest if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_vkgroupid_title'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_vkgroupid_title ON memberinterest USING btree (vkgroupid , title COLLATE pg_catalog."default" );
    END IF;
    
    -- Create index on post if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_post_vkgroupid_posteddate'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_post_vkgroupid_posteddate ON post USING btree (vkgroupid , posteddate desc);
    END IF;
    
    -- Create index on postcomment if required
    --SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_postcomment_vkgroupid_posteddate'';

    --IF indexExists = 0 THEN
    --  CREATE INDEX ix_postcomment_vkgroupid_posteddate ON postcomment USING btree (vkgroupid , posteddate desc);
    --END IF;
    
    -- Create index on topic if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topic_vkgroupid_posteddate'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_topic_vkgroupid_posteddate ON topic USING btree (vkgroupid , posteddate desc);
    END IF;
    
    -- Create index on topiccomment if required
    --SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topiccomment_vkgroupid_posteddate'';

    --IF indexExists = 0 THEN
    --  CREATE INDEX ix_topiccomment_vkgroupid_posteddate ON topiccomment USING btree (vkgroupid , posteddate desc);
    --END IF;
    
    -- Create index on photo if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_photo_vkgroupid_posteddate'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_photo_vkgroupid_posteddate ON photo USING btree (vkgroupid , posteddate desc);
    END IF;
    
    -- Create index on video if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_video_vkgroupid_posteddate'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_video_vkgroupid_posteddate ON video USING btree (vkgroupid , posteddate desc);
    END IF;
    
    -- CREATE UNIQUE INDEX on post if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_post_vkgroupid_vkid'';

    IF indexExists = 0 THEN
      CREATE UNIQUE INDEX ix_post_vkgroupid_vkid ON post USING btree (vkgroupid , vkid);
    END IF;
    
    -- CREATE UNIQUE INDEX on postcomment if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_postcomment_vkgroupid_vkid'';

    IF indexExists = 0 THEN
      CREATE UNIQUE INDEX ix_postcomment_vkgroupid_vkid ON postcomment USING btree (vkgroupid , vkid);
    END IF;
    
    -- CREATE UNIQUE INDEX on topic if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topic_vkgroupid_vkid'';

    IF indexExists = 0 THEN
      CREATE UNIQUE INDEX ix_topic_vkgroupid_vkid ON topic USING btree (vkgroupid , vkid);
    END IF;
    
    -- CREATE UNIQUE INDEX on topiccomment if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topiccomment_vkgroupid_vkid'';

    IF indexExists = 0 THEN
      CREATE UNIQUE INDEX ix_topiccomment_vkgroupid_vkid ON topiccomment USING btree (vkgroupid , vkid);
    END IF;
    
    -- CREATE UNIQUE INDEX on photo if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_photo_vkgroupid_vkid'';

    IF indexExists = 0 THEN
      CREATE UNIQUE INDEX ix_photo_vkgroupid_vkid ON photo USING btree (vkgroupid , vkid);
    END IF;
    
    -- CREATE UNIQUE INDEX on video if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_video_vkgroupid_vkid'';

    IF indexExists = 0 THEN
      CREATE UNIQUE INDEX ix_video_vkgroupid_vkid ON video USING btree (vkgroupid , vkid);
    END IF;

	-- Create index on post if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_post_vkgroupid_vkid'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_post_vkgroupid_vkid ON post USING btree (vkgroupid , vkid);
    END IF;

    -- Create index on post if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_postcomment_vkgroupid_vkpostid'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_postcomment_vkgroupid_vkpostid ON postcomment USING btree (vkgroupid , vkpostid);
    END IF;

	-- Create index on topic if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topic_vkgroupid_vkid'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_topic_vkgroupid_vkid ON topic USING btree (vkgroupid , vkid);
    END IF;

    -- Create index on post if required
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topiccomment_vkgroupid_vktopicid'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_topiccomment_vkgroupid_vktopicid ON topiccomment USING btree (vkgroupid , vktopicid);
    END IF;

	-- Create lowercase index on memberinterest
    SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_memberinterest_vkgroupid_loweredtitle'';

    IF indexExists = 0 THEN
      CREATE INDEX ix_memberinterest_vkgroupid_loweredtitle ON memberinterest USING btree (vkgroupid , lower(title));
    END IF;
  	
	RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_2();
DROP FUNCTION Rerunnable0_5_2();