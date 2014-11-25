-- Use this script to delete non-routing information from the database
-- For the UK, this resulted in 34.4 million rows being deleted
DELETE FROM way_tags WHERE way_id NOT IN (SELECT way_id FROM way_tags WHERE [key]='highway')
DELETE FROM way_nodes WHERE way_id NOT IN (SELECT way_id FROM way_tags WHERE [key]='highway')
DELETE FROM node_tags WHERE node_id NOT IN (SELECT node_id FROM way_nodes)
DELETE FROM node WHERE id NOT IN (SELECT node_id FROM way_nodes)
DELETE FROM relation_members WHERE member_type=1 AND member_id NOT IN (SELECT way_id FROM way_tags WHERE [key]='highway')
DELETE FROM relation_members WHERE member_type=0 AND member_id NOT IN (SELECT id FROM node)
DELETE FROM relation_members WHERE member_type=2 AND member_id NOT IN (SELECT id FROM relation)
DELETE FROM relation_tags WHERE NOT EXISTS (SELECT relation_id FROM relation_members WHERE relation_members.relation_id = relation_tags.relation_id)
DELETE FROM relation WHERE NOT EXISTS (SELECT relation_id FROM relation_members WHERE relation_id = id)