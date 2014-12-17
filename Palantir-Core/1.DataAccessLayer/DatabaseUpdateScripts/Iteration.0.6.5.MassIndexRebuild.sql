ALTER TABLE member SET (FILLFACTOR = 70);
DROP INDEX member_vkid_vkgroupid;
DROP INDEX ix_member_vkgroupid_version;
CREATE INDEX ix_member_vkgroupid ON member USING btree (vkgroupid);
CLUSTER member USING ix_member_vkgroupid;

DROP INDEX ix_membersubscriptions_vkgroupid_vkmemberid_subscribedvkgroupid;
ALTER TABLE membersubscriptions SET (FILLFACTOR = 70);
CLUSTER membersubscriptions USING ix_membersubscriptions_vkgroupid;
CREATE INDEX ix_membersubscriptions_vkgroupid_vkmemberid ON membersubscriptions USING btree (vkgroupid, vkmemberid);

DROP INDEX ix_vkgroupid_title;
DROP INDEX ix_vkgroupid_vkmemberid;
delete from memberinterest where length(title) > 50;
delete FROM memberinterest WHERE title LIKE '%```%';
delete from memberinterest WHERE title LIKE '%###%';
delete from memberinterest WHERE title LIKE '%^^^%';
delete from memberinterest WHERE title LIKE '%████%';
delete from memberinterest WHERE title LIKE '%[][][]%';
delete from memberinterest WHERE title LIKE '%$$$%';
delete from memberinterest WHERE title LIKE '%@@@@%';
delete from memberinterest WHERE title ~* '^[`!@#$%^&*()8]{3,}$';
delete from memberinterest WHERE title LIKE '%XXXX%';
delete from memberinterest WHERE title LIKE '%8888%';
delete from memberinterest WHERE title LIKE '%¶¶¶¶%';
delete from memberinterest WHERE title LIKE '%۩۩۩۩%';
delete from memberinterest WHERE title LIKE '%)))))%';
delete from memberinterest WHERE title LIKE '%(((((%';
delete from memberinterest WHERE title LIKE '%oooo%';
delete from memberinterest WHERE title = '';
delete from memberinterest where title ~* '^[.]{3,}$';
delete from memberinterest where title ~* '^[_]{3,}$';
delete from memberinterest where title ~* '^[\d]{5,}$';
ALTER TABLE memberinterest SET (FILLFACTOR = 70);
CREATE INDEX ix_memberinterest_vkgroupid ON memberinterest USING btree (vkgroupid);
CLUSTER memberinterest USING ix_memberinterest_vkgroupid;

drop table feed;
drop table getfeedqueue;

DROP INDEX ix_administrator_id;
CLUSTER administrator USING ix_administrator_vkgroupid;

DROP INDEX ix_memberlike_id;
ALTER TABLE memberlike SET (FILLFACTOR = 70);
CREATE INDEX ix_memberlike_vkgroupid ON memberlike USING btree (vkgroupid);
CLUSTER memberlike USING ix_memberlike_vkgroupid;

DROP INDEX ix_membershare_id;
ALTER TABLE membershare SET (FILLFACTOR = 70);
CREATE INDEX ix_membershare_vkgroupid ON membershare USING btree (vkgroupid);
CLUSTER membershare USING ix_membershare_vkgroupid;

ALTER TABLE membersdelta SET (FILLFACTOR = 70);
CREATE INDEX ix_membersdelta_vkgroupid ON membersdelta USING btree (vkgroupid);
CLUSTER membersdelta USING ix_membersdelta_vkgroupid;

DROP INDEX ix_users_vkgroupid;
CREATE INDEX ix_membersmetainfo_vkgroupid ON membersmetainfo USING btree (vkgroupid);
ALTER TABLE membersmetainfo SET (FILLFACTOR = 70);
CLUSTER membersmetainfo USING ix_membersmetainfo_vkgroupid;

DROP INDEX ix_photo_id;
ALTER TABLE photo SET (FILLFACTOR = 70);
CREATE INDEX ix_photo_vkgroupid ON photo USING btree (vkgroupid);
CLUSTER photo USING ix_photo_vkgroupid;

DROP INDEX ix_post_id;
ALTER TABLE post SET (FILLFACTOR = 70);
CREATE INDEX ix_post_vkgroupid ON post USING btree (vkgroupid);
CLUSTER post USING ix_post_vkgroupid;

DROP INDEX ix_postcomment_id;
ALTER TABLE postcomment SET (FILLFACTOR = 70);
CLUSTER postcomment USING ix_postcomment_vkgroupid;

CREATE INDEX ix_project_id ON project USING btree (id);

DROP INDEX ix_topic_id;
ALTER TABLE topic SET (FILLFACTOR = 70);
CREATE INDEX ix_topic_vkgroupid ON topic USING btree (vkgroupid);
CLUSTER topic USING ix_topic_vkgroupid;

DROP INDEX ix_topiccomment_id;
ALTER TABLE topiccomment SET (FILLFACTOR = 70);
CLUSTER topiccomment USING ix_topiccomment_vkgroupid;

CREATE INDEX ix_user_accountid ON "user" USING btree (accountid);

CREATE INDEX ix_userfilter_userid ON userfilter USING btree (userid);

DROP INDEX ix_video_id;
ALTER TABLE video SET (FILLFACTOR = 70);
CLUSTER video USING ix_video_vkgroupid;

DROP INDEX ix_videocomment_id;
ALTER TABLE videocomment SET (FILLFACTOR = 70);
CLUSTER videocomment USING ix_videocomment_vkgroupid;

CREATE INDEX ix_vkgroup_id ON vkgroup USING btree (id);

DROP INDEX ix_vkgroupprocessinghistory_id;
ALTER TABLE vkgroupprocessinghistory SET (FILLFACTOR = 70);
CREATE INDEX ix_vkgroupprocessinghistory_vkgroupid ON vkgroupprocessinghistory USING btree (vkgroupid);
CLUSTER vkgroupprocessinghistory USING ix_vkgroupprocessinghistory_vkgroupid;

ALTER TABLE vkgroupprocessingstate SET (FILLFACTOR = 70);
CREATE INDEX ix_vkgroupprocessingstate_vkgroupid ON vkgroupprocessingstate USING btree (vkgroupid);
CLUSTER vkgroupprocessingstate USING ix_vkgroupprocessingstate_vkgroupid;

CREATE INDEX ix_account_id ON account USING btree (id);
