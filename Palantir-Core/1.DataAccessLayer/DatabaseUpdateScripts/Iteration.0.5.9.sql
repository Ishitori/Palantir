 CREATE OR REPLACE FUNCTION Rerunnable0_5_9() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;

BEGIN

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''membersubscriptions'';

  IF tableExists = 0 THEN
    CREATE TABLE membersubscriptions (
       id serial NOT NULL,
       vkmemberid bigint NOT NULL DEFAULT 0,
       vkgroupid integer NOT NULL DEFAULT 0,
       namegroup character varying(100),
       photo character varying(100),
       CONSTRAINT membersubscriptions_pk_id PRIMARY KEY (id)
    );
  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''user'' and column_name=''maxprojectscount'';

  IF columnExists = 0 THEN
	ALTER TABLE "user" ADD COLUMN maxprojectscount integer;
  END IF;

  insert into administrator(userid, vkgroupid)
  select -vkgroup.id as userid, vkgroup.id as vkgroupid from vkgroup
  left outer join administrator a on (a.userid = -vkgroup.id)
  where a.userid is null;

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''memberlike'';

  IF tableExists = 0 THEN
	CREATE TABLE memberlike
	(
	  id serial NOT NULL,
	  vkmemberid bigint,
	  itemid integer,
	  itemtype integer,
	  vkgroupid integer,
	  CONSTRAINT memberlike_pk_id PRIMARY KEY (id)
	);
  END IF;

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''membershare'';

  IF tableExists = 0 THEN
	CREATE TABLE membershare
	(
	  id serial NOT NULL,
	  vkmemberid bigint,
	  itemid integer,
	  itemtype integer,
	  vkgroupid integer,
	  CONSTRAINT membershare_pk_id PRIMARY KEY (id)
	);
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_5_9();
DROP FUNCTION Rerunnable0_5_9();