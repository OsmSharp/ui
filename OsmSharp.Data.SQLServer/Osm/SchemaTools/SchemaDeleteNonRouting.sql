-- Use this script to delete non-routing information from the database
-- For the UK, this resulted in 34.4 million rows being deleted
DELETE FROM way_tags WHERE way_id NOT IN (SELECT way_id FROM way_tags WHERE [key]='highway')
DELETE FROM way_nodes WHERE way_id NOT IN (SELECT way_id FROM way_tags WHERE [key]='highway')
DELETE FROM node_tags WHERE node_id NOT IN (SELECT node_id FROM way_nodes)
DELETE FROM node WHERE id NOT IN (SELECT node_id FROM way_nodes)
DELETE FROM relation_members WHERE member_type='Way' AND member_id NOT IN (SELECT way_id FROM way_tags WHERE [key]='highway')
DELETE FROM relation_members WHERE member_type='Node' AND member_id NOT IN (SELECT id FROM node)
DELETE FROM relation_members WHERE member_type='Relation' AND member_id NOT IN (SELECT id FROM relation)
DELETE FROM relation_tags WHERE relation_id NOT IN (SELECT id FROM relation)
