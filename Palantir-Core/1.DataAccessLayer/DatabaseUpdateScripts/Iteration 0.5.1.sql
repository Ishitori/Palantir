CREATE OR REPLACE FUNCTION Rerunnable0_5_1() RETURNS INTEGER AS '

DECLARE indexExists INTEGER;
DECLARE tableExists INTEGER;

BEGIN
  -- Create index on post if required
  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_post_vkgroupid_creatorid'';

  IF indexExists = 0 THEN
    CREATE INDEX ix_post_vkgroupid_creatorid ON post USING btree (vkgroupid , creatorid );
  END IF;

  -- Create index on postcomment if required
  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_postcomment_vkgroupid_creatorid'';

  IF indexExists = 0 THEN
    CREATE INDEX ix_postcomment_vkgroupid_creatorid ON postcomment USING btree (vkgroupid , creatorid );
  END IF;

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''topic'';

  IF tableExists = 0 THEN
      CREATE TABLE topic (
         id INTEGER NOT NULL,
         vkgroupid INTEGER DEFAULT 0 NOT NULL,
         posteddate TIMESTAMP WITHOUT TIME ZONE,
         vkid CHARACTER VARYING(100),
         year INTEGER DEFAULT 0,
         month INTEGER DEFAULT 0,
         week INTEGER DEFAULT 0,
         day INTEGER DEFAULT 0,
         hour INTEGER DEFAULT 0,
         minute INTEGER DEFAULT 0,
         second INTEGER DEFAULT 0,
         createdbyvkid BIGINT DEFAULT 0,
         title TEXT,
         commentscount INTEGER DEFAULT 0,
         lastcommentdate TIMESTAMP WITHOUT TIME ZONE
    );
	
    ALTER TABLE public.topic OWNER TO postgres;

    CREATE SEQUENCE topic_id_seq
      START WITH 1
      INCREMENT BY 1
      NO MINVALUE
      NO MAXVALUE
      CACHE 1;
	  
    ALTER TABLE public.topic_id_seq OWNER TO postgres;
    ALTER SEQUENCE topic_id_seq OWNED BY topic.id;

	ALTER TABLE topic ADD CONSTRAINT topic_pkey PRIMARY KEY(id);
  
  END IF;

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''topiccomment'';

  IF tableExists = 0 THEN

	CREATE TABLE topiccomment (
		id integer NOT NULL,
		vkid character varying(100) DEFAULT 0 NOT NULL,
		creatorid bigint DEFAULT 0,
		posteddate timestamp without time zone,
		year integer DEFAULT 0,
		month integer DEFAULT 0,
		week integer DEFAULT 0,
		day integer DEFAULT 0,
		hour integer DEFAULT 0,
		minute integer DEFAULT 0,
		second integer DEFAULT 0,
		vkgroupid integer DEFAULT 0,
		vktopicid character varying(100)
	);


	ALTER TABLE public.topiccomment OWNER TO postgres;

	CREATE SEQUENCE topiccomment_id_seq
		START WITH 1
		INCREMENT BY 1
		NO MINVALUE
		NO MAXVALUE
		CACHE 1;


	ALTER TABLE public.topiccomment_id_seq OWNER TO postgres;
	ALTER SEQUENCE topiccomment_id_seq OWNED BY topiccomment.id;

	ALTER TABLE topiccomment ADD CONSTRAINT topiccomment_pkey PRIMARY KEY(id);

  END IF;

  -- Create index on topic if required
  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topic_vkgroupid_vkid'';

  IF indexExists = 0 THEN
    CREATE INDEX ix_topic_vkgroupid_vkid ON topic USING btree (vkgroupid, vkid COLLATE pg_catalog."default" );
  END IF;

  -- Create index on topic if required
  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topiccomment_vkgroupid'';

  IF indexExists = 0 THEN
    CREATE INDEX ix_topiccomment_vkgroupid ON topiccomment USING btree (vkgroupid);
  END IF;

RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_1();
DROP FUNCTION Rerunnable0_5_1();