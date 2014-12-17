CREATE OR REPLACE FUNCTION Rerunnable0_6_3() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;
DECLARE indexExists INTEGER;

BEGIN

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''member'' and column_name=''version'';

  IF columnExists = 0 THEN
	ALTER TABLE member ADD COLUMN version integer;
  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''member'' and column_name=''isnew'';

  IF columnExists = 0 THEN
	ALTER TABLE member ADD COLUMN isnew boolean;
  END IF;


  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''vkgroupprocessingstate'';
  IF tableExists = 0 THEN

	  CREATE TABLE vkgroupprocessingstate(
		id serial NOT NULL,
		vkgroupid integer,
		feedtype integer,
		version integer DEFAULT 0,
		fetchingdate timestamp without time zone,
		fetchingserver character varying(50),
		fetchingprocess character varying(50),
		processingdate timestamp without time zone,
		processingserver character varying(50),
		processingprocess character varying(50),
	  CONSTRAINT vkgroupprocessingstate_pk_id PRIMARY KEY (id)
	  );

	  INSERT INTO vkgroupprocessingstate(vkgroupid, feedtype, version, fetchingdate, fetchingserver, fetchingprocess, processingdate, processingserver, processingprocess)
	  SELECT 
		vkgroupid, 
		feedtype, 
		COUNT(*), 
		MAX(fetchingdate), 
		MAX(fetchingserver), 
		MAX(fetchingprocess), 
		MAX(processingdate), 
		MAX(processingserver), 
		MAX(processingprocess) 
      FROM vkgroupprocessinghistory 
	  GROUP BY vkgroupid, feedtype
	  ORDER BY vkgroupid, feedtype;
	  
	UPDATE member
	SET version = (select max(version) from vkgroupprocessingstate st where st.feedtype = 31 and st.vkgroupid = vkgroupid);

  END IF;

  IF NOT EXISTS (SELECT 0 FROM pg_class where relname = ''membersdelta_id_seq'') THEN
    CREATE SEQUENCE membersdelta_id_seq
		START WITH 1
		INCREMENT BY 1
		NO MINVALUE
		NO MAXVALUE
		CACHE 1;

    ALTER TABLE membersdelta ADD CONSTRAINT membersdelta_pkey PRIMARY KEY(id);
    ALTER TABLE membersdelta ALTER COLUMN id SET DEFAULT nextval(''membersdelta_id_seq''::regclass);

  END IF;

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_vkgroupprocessingstate_vkgroupid_feedtype'';
  IF indexExists = 0 THEN
	CREATE UNIQUE INDEX ix_vkgroupprocessingstate_vkgroupid_feedtype ON vkgroupprocessingstate USING btree (vkgroupid, feedtype);
  END IF;

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_member_vkgroupid_version'';
  IF indexExists = 0 THEN
	CREATE INDEX ix_member_vkgroupid_version ON member USING btree (vkgroupid, version);
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_6_3();
DROP FUNCTION Rerunnable0_6_3();