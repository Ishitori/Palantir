CREATE OR REPLACE FUNCTION Rerunnable0_5_5() RETURNS INTEGER AS '

DECLARE indexExists INTEGER;
DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;

BEGIN

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''videocomment'';

  IF tableExists = 0 THEN

	CREATE TABLE videocomment (
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
		vkvideoid character varying(100)
	);


	ALTER TABLE public.videocomment OWNER TO postgres;

	CREATE SEQUENCE videocomment_id_seq
		START WITH 1
		INCREMENT BY 1
		NO MINVALUE
		NO MAXVALUE
		CACHE 1;


	ALTER TABLE public.videocomment_id_seq OWNER TO postgres;
	ALTER SEQUENCE videocomment_id_seq OWNED BY videocomment.id;

	ALTER TABLE videocomment ADD CONSTRAINT videocomment_pkey PRIMARY KEY(id);
	ALTER TABLE videocomment ALTER COLUMN id SET DEFAULT nextval(''videocomment_id_seq''::regclass);  

	CREATE INDEX ix_videocomment_id ON videocomment USING btree (id);
	CREATE INDEX ix_videocomment_vkgroupid ON videocomment USING btree (vkgroupid);

	CREATE UNIQUE INDEX ix_videocomment_vkgroupid_vkid
	  ON videocomment
	  USING btree
	  (vkgroupid , vkid COLLATE pg_catalog."default" );

	CREATE INDEX ix_videocomment_vkgroupid_vkvideoid
	  ON videocomment
	  USING btree
	  (vkgroupid , vkvideoid COLLATE pg_catalog."default" );
  	
  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''video'' and column_name=''likescount'';

  IF columnExists = 0 THEN
    ALTER TABLE video ADD COLUMN likescount integer;
  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''member'' and column_name=''vkmemberid'' and data_type=''character varying'';

  IF columnExists = 1 THEN
    ALTER TABLE member alter COLUMN vkmemberid TYPE bigint USING CAST(vkmemberid AS bigint);
  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''memberinterest'' and column_name=''vkmemberid'' and data_type=''character varying'';

  IF columnExists = 1 THEN
    ALTER TABLE memberinterest alter COLUMN vkmemberid TYPE bigint USING CAST(vkmemberid AS bigint);
  END IF;


  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''vkgroupprocessinghistory'';

  IF tableExists = 0 THEN

	CREATE TABLE vkgroupprocessinghistory
	(
	  id integer NOT NULL,
	  vkgroupid integer,
	  feedtype integer,
	  fetchingdate timestamp without time zone,
	  fetchingserver character varying(50),
	  fetchingprocess character varying(50),
	  processingdate timestamp without time zone,
	  processingserver character varying(50),
	  processingprocess character varying(50),
	  CONSTRAINT vkgroupprocessinghistory_pk PRIMARY KEY (id )
	)
	WITH (
	  OIDS=FALSE
	);
	ALTER TABLE vkgroupprocessinghistory
	  OWNER TO postgres;

	CREATE INDEX ix_vkgroupprocessinghistory_vkgroup_feedtype_processingdate
	  ON vkgroupprocessinghistory
	  USING btree
	  (vkgroupid , feedtype , processingdate );

	CREATE SEQUENCE vkgroupprocessinghistory_id_seq
		START WITH 1
		INCREMENT BY 1
		NO MINVALUE
		NO MAXVALUE
		CACHE 1;

	ALTER TABLE vkgroupprocessinghistory ALTER COLUMN id SET DEFAULT nextval(''vkgroupprocessinghistory_id_seq''::regclass);

	CREATE INDEX ix_vkgroupprocessinghistory_vkgroup_processingdate
	  ON vkgroupprocessinghistory
	  USING btree
	  (vkgroupid , processingdate );
  
    END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''photo'' and column_name=''text'';

  IF columnExists = 0 THEN
    ALTER TABLE photo ADD COLUMN text text;
  END IF;
  
  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''projectconcurrent'';

  IF tableExists = 0 THEN

	CREATE TABLE projectconcurrent
	(
	  id serial NOT NULL,
	  projectid1 integer,
	  projectid2 integer,
	  CONSTRAINT projectconcurrent_pkey PRIMARY KEY (id )
	)
	WITH (
	  OIDS=FALSE
	);
	ALTER TABLE projectconcurrent
	  OWNER TO postgres;


	CREATE INDEX ix_projectconcurrent_projectid1
	  ON projectconcurrent
	  USING btree
	  (projectid1 );

  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_5();
DROP FUNCTION Rerunnable0_5_5();