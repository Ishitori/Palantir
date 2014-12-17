CREATE OR REPLACE FUNCTION DeleteDubs() RETURNS INTEGER AS '

DECLARE indexExists INTEGER;

BEGIN
	create table dubs
	(
	  id integer not null,
	  constraint id primary key (id)
	);

	CREATE INDEX ix_dubs_id ON dubs USING btree (id);

	-- Remove post dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_post_id'';
    IF indexExists = 0 THEN
		create index ix_post_id ON post USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM post As dup GROUP BY dup.vkgroupid, dup.vkid);
	delete from post p where not exists (select * from dubs where dubs.id = p.id);
	delete from dubs;

	-- Remove postcomment dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_postcomment_id'';
    IF indexExists = 0 THEN
		create index ix_postcomment_id ON postcomment USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM postcomment As dup GROUP BY dup.vkgroupid, dup.vkid);
	delete from postcomment p where not exists (select * from dubs where dubs.id = p.id);
	delete from dubs;

	-- Remove video dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_video_id'';
    IF indexExists = 0 THEN
		create index ix_video_id ON video USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM video As dup GROUP BY dup.vkgroupid, dup.vkid);
	delete from video p where not exists (select * from dubs where dubs.id = p.id);
	delete from dubs;

	-- Remove photo dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_photo_id'';
    IF indexExists = 0 THEN
		create index ix_photo_id ON photo USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM photo As dup GROUP BY dup.vkgroupid, dup.vkid);
	delete from photo p where not exists (select * from dubs where dubs.id = p.id);
	delete from dubs;
		
	-- Remove topic dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topic_id'';
    IF indexExists = 0 THEN
		create index ix_topic_id ON topic USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM topic As dup GROUP BY dup.vkgroupid, dup.vkid);
	delete from topic p where not exists (select * from dubs where dubs.id = p.id);
	delete from dubs;

	-- Remove topiccomment dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_topiccomment_id'';
    IF indexExists = 0 THEN
		create index ix_topiccomment_id ON topiccomment USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM topiccomment As dup GROUP BY dup.vkgroupid, dup.vkid);
	delete from topiccomment p where not exists (select * from dubs where dubs.id = p.id);
	delete from dubs;

	-- Remove membersubscriptions dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_membersubscriptions_id'';
    IF indexExists = 0 THEN
		create index ix_membersubscriptions_id ON membersubscriptions USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM membersubscriptions As dup GROUP BY dup.vkgroupid, dup.vkmemberid, dup.subscribedvkgroupid);
	delete from membersubscriptions ms where not exists (select * from dubs where dubs.id = ms.id);
	delete from dubs;

	-- Remove membershare dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_membershare_id'';
    IF indexExists = 0 THEN
		create index ix_membershare_id ON membershare USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM membershare As dup GROUP BY dup.vkgroupid, dup.vkmemberid, dup.itemtype, dup.itemid);
	delete from membershare ms where not exists (select * from dubs where dubs.id = ms.id);
	delete from dubs;

	-- Remove memberlike dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_memberlike_id'';
    IF indexExists = 0 THEN
		create index ix_memberlike_id ON memberlike USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM memberlike As dup GROUP BY dup.vkgroupid, dup.vkmemberid, dup.itemtype, dup.itemid);
	delete from memberlike ml where not exists (select * from dubs where dubs.id = ml.id);
	delete from dubs;

	-- Remove administrator dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_administrator_id'';
    IF indexExists = 0 THEN
		create index ix_administrator_id ON administrator USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM administrator As dup GROUP BY dup.vkgroupid, dup.userid);
	delete from administrator a where not exists (select * from dubs where dubs.id = a.id);
	delete from dubs;

	-- Remove vkprocessinghistory dubs
	SELECT INTO indexExists COUNT(*) FROM pg_class WHERE relname = ''ix_vkgroupprocessinghistory_id'';
    IF indexExists = 0 THEN
		create index ix_vkgroupprocessinghistory_id ON vkgroupprocessinghistory USING btree (id);
    END IF;
	insert into dubs(id) (SELECT MAX(dup.id) FROM vkgroupprocessinghistory As dup GROUP BY dup.vkgroupid, dup.feedtype);
	delete from vkgroupprocessinghistory a where not exists (select * from dubs where dubs.id = a.id);
	delete from dubs;

	drop table dubs;

 	RETURN NULL;
END;' language 'plpgsql';

SELECT DeleteDubs();
DROP FUNCTION DeleteDubs();