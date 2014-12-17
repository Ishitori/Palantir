CREATE OR REPLACE FUNCTION Rerunnable0_6_7() RETURNS INTEGER AS '

DECLARE tableExists INTEGER;
DECLARE columnExists INTEGER;
DECLARE indexExists INTEGER;

BEGIN

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_country_vkid'';
  IF indexExists = 0 THEN
	CREATE UNIQUE INDEX ix_country_vkid ON country USING btree (vkid);
  END IF;

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_city_vkid'';
  IF indexExists = 0 THEN
	CREATE UNIQUE INDEX ix_city_vkid ON city USING btree (vkid);
  END IF;

  SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_membersubscriptions_vkgroupid_subscribedvkgroupid_vkmemberid'';
  IF indexExists = 0 THEN
	CREATE INDEX ix_membersubscriptions_vkgroupid_subscribedvkgroupid_vkmemberid ON membersubscriptions USING btree (vkgroupid, subscribedvkgroupid, vkmemberid);  
  END IF;

  RETURN NULL;
END;' language 'plpgsql';

SELECT Rerunnable0_6_7();
DROP FUNCTION Rerunnable0_6_7();