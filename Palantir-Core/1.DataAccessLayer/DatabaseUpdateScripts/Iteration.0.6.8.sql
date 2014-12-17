CREATE OR REPLACE FUNCTION Rerunnable0_6_8() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;
DECLARE indexExists INTEGER;

BEGIN

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''vkgroupreference'';

  IF tableExists = 0 THEN
	CREATE TABLE vkgroupreference
	(
	  vkgroupid integer not null default(0),
	  namegroup character varying(500),
	  photo character varying(500),
	  screenname character varying(500)
	)
	WITH (OIDS=FALSE);

	ALTER TABLE vkgroupreference SET (FILLFACTOR = 70);
	CREATE UNIQUE INDEX ix_vkgroupreference_vkgroupid ON vkgroupreference USING btree (vkgroupid);
	CLUSTER vkgroupreference USING ix_vkgroupreference_vkgroupid;

	INSERT INTO vkgroupreference(vkgroupid, namegroup, photo, screenname)
	SELECT subscribedvkgroupid as vkgroupid, MAX(namegroup), MAX(photo), MAX(screenname) FROM membersubscriptions
	GROUP BY subscribedvkgroupid;

	ALTER TABLE membersubscriptions DROP COLUMN namegroup;
	ALTER TABLE membersubscriptions DROP COLUMN photo;
	ALTER TABLE membersubscriptions DROP COLUMN screenname;
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_6_8();
DROP FUNCTION Rerunnable0_6_8();