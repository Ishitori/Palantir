 CREATE OR REPLACE FUNCTION Rerunnable0_6_2() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;
DECLARE indexExists INTEGER;

BEGIN

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''membersubscriptions'' and column_name=''subscribedvkgroupid'';

  IF columnExists = 0 THEN
	ALTER TABLE "membersubscriptions" ADD COLUMN subscribedvkgroupid integer;

	UPDATE "user" AS u
	SET accountid = a.id
	FROM account AS a 
	WHERE a.title = u.email;

  END IF;

  SELECT INTO columnExists COUNT(*) FROM information_schema.columns WHERE table_name=''membersubscriptions'' and column_name=''screenname'';

  IF columnExists = 0 THEN
	ALTER TABLE "membersubscriptions" ADD COLUMN screenname character varying(100);
 
  END IF;

  ALTER TABLE membersubscriptions ALTER COLUMN namegroup TYPE character varying(500);
  ALTER TABLE membersubscriptions ALTER COLUMN photo TYPE character varying(500);
  ALTER TABLE membersubscriptions ALTER COLUMN screenname TYPE character varying(500);

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_membersubscriptions_vkgroupid_vkmemberid_subscribedvkgroupid'';
  IF indexExists = 0 THEN
	create index ix_membersubscriptions_vkgroupid_vkmemberid_subscribedvkgroupid ON membersubscriptions USING btree (vkgroupid, vkmemberid, subscribedvkgroupid);
  END IF;

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_membershare_vkgroupid_vkmemberid_itemtype_itemid'';
  IF indexExists = 0 THEN
	create unique index ix_membershare_vkgroupid_vkmemberid_itemtype_itemid ON membershare USING btree (vkgroupid, vkmemberid, itemtype, itemid);
  END IF;

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_memberlike_vkgroupid_vkmemberid_itemtype_itemid'';
  IF indexExists = 0 THEN
	create unique index ix_memberlike_vkgroupid_vkmemberid_itemtype_itemid ON memberlike USING btree (vkgroupid, vkmemberid, itemtype, itemid);
  END IF;

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_administrator_vkgroupid_userid'';
  IF indexExists = 0 THEN
	create unique index ix_administrator_vkgroupid_userid ON administrator USING btree (vkgroupid, userid);
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_6_2();
DROP FUNCTION Rerunnable0_6_2();