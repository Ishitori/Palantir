CREATE OR REPLACE FUNCTION Rerunnable0_6_6() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;
DECLARE indexExists INTEGER;

BEGIN

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''memberupdate'';

  IF tableExists = 0 THEN
	CREATE TABLE memberupdate
	(
	  id serial NOT NULL,
	  vkgroupid integer,
	  vkmemberid bigint,
	  createddate timestamp without time zone,
	  isnew boolean default(false),
	  CONSTRAINT memberupdate_pk_id PRIMARY KEY (id)
	)
	WITH (
	  OIDS=FALSE
	);

	ALTER TABLE memberupdate SET (FILLFACTOR = 70);
	CREATE INDEX ix_memberupdate_vkgroupid ON memberupdate USING btree (vkgroupid);
	CLUSTER memberupdate USING ix_memberupdate_vkgroupid;

  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''member'' and column_name=''isnew'';

  IF columnExists <> 0 THEN
	ALTER TABLE member DROP COLUMN isnew;
	ALTER TABLE member DROP COLUMN version;
	ALTER TABLE member ADD COLUMN isdeleted boolean default(false);
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_6_6();
DROP FUNCTION Rerunnable0_6_6();