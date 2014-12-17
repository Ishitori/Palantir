 CREATE OR REPLACE FUNCTION Rerunnable0_6_1() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;

BEGIN

  SELECT INTO tableExists COUNT(*) from information_schema.tables where table_name=''account'';

  IF tableExists = 0 THEN
	CREATE TABLE account
	(
	  id serial NOT NULL,
	  title character varying(100),
	  maxprojectscount integer,
	  CONSTRAINT account_pk_id PRIMARY KEY (id )
	)
	WITH (
	  OIDS=FALSE
	);

	INSERT INTO account(title, maxprojectscount)
	SELECT email, maxprojectscount FROM "user";

  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''user'' and column_name=''accountid'';

  IF columnExists = 0 THEN
	ALTER TABLE "user" ADD COLUMN accountid integer;

	UPDATE "user" AS u
	SET accountid = a.id
	FROM account AS a 
	WHERE a.title = u.email;

  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''user'' and column_name=''maxprojectscount'';

  IF columnExists <> 0 THEN
	
	UPDATE account AS a
	SET maxprojectscount = u.maxprojectscount
	FROM "user" AS u 
	WHERE a.title = u.email;

	ALTER TABLE "user" DROP COLUMN maxprojectscount;
  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''project'' and column_name=''accountid'';

  IF columnExists = 0 THEN
	ALTER TABLE project ADD COLUMN accountid integer;
	
	UPDATE project AS p
	SET accountid = a.id
	FROM account AS a
	INNER JOIN "user" u ON (u.email = a.title) 
	WHERE p.creatoruserid = u.id;

	ALTER TABLE project DROP COLUMN creatoruserid;
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_6_1();
DROP FUNCTION Rerunnable0_6_1();