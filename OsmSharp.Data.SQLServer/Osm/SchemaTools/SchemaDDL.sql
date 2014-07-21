/*
NOTES:
    * Don't put 'go' in this script, it will be executed by SqlCommand.ExecuteNonQuery 
    * Please keep SQLServerSimpleSchemaConstants up to date with correct varchar() sizes
*/

/*
--drop tables
drop table dbo.node
drop table dbo.node_tags 
drop table dbo.way 
drop table dbo.way_tags 
drop table dbo.way_nodes 
drop table dbo.relation 
drop table dbo.relation_tags 
drop table dbo.relation_members 

--truncate tables
truncate table dbo.node
truncate table dbo.node_tags 
truncate table dbo.way 
truncate table dbo.way_tags 
truncate table dbo.way_nodes 
truncate table dbo.relation 
truncate table dbo.relation_tags 
truncate table dbo.relation_members 

--check
select count(*) from dbo.node
select count(*) from dbo.node_tags
select count(*) from dbo.way
select count(*) from dbo.way_tags
select count(*) from dbo.way_nodes
select count(*) from dbo.relation
select count(*) from dbo.relation_tags
select count(*) from dbo.relation_members


--drop constraints before load
alter table node             drop constraint PK_node
alter table node_tags        drop constraint PK_node_tags
alter table way              drop constraint PK_way
alter table way_tags         drop constraint PK_way_tags
alter table way_nodes        drop constraint PK_way_nodes
alter table relation         drop constraint PK_relation
alter table relation_tags    drop constraint PK_relation_tags
alter table relation_members drop constraint PK_relation_members

drop index IDX_NODE_TILE on node
drop index IDX_WAY_NODES_NODE ON dbo.way_nodes

--recreate constraints afterwards
alter table node              add constraint PK_node              primary key (id)
alter table node_tags         add constraint PK_node_tags         primary key (node_id,[key])
alter table way               add constraint PK_way               primary key (id)
alter table way_tags          add constraint PK_way_tags          primary key (way_id, [key])
alter table way_nodes         add constraint PK_way_nodes         primary key (way_id, node_id, sequence_id)
alter table relation          add constraint PK_relation          primary key (id)
alter table relation_tags     add constraint PK_relation_tags     primary key (relation_id, [key])
alter table relation_members  add constraint PK_relation_members  primary key (relation_id, member_id, sequence_id)

CREATE INDEX IDX_NODE_TILE ON node(tile  ASC);
CREATE INDEX IDX_WAY_NODES_NODE ON dbo.way_nodes(node_id  ASC);

*/

if object_id('dbo.node', 'U') is null
  CREATE TABLE dbo.node
  (
    id            bigint   not null,
    latitude      integer,
    longitude     integer,
    changeset_id  bigint   null,
    visible       bit      null,
    [timestamp]   datetime null,
    tile          bigint   null,
    [version]     integer  null,
    usr           varchar(100) null,
    usr_id        integer  null
  ); 



if object_id('dbo.node_tags', 'U') is null
  CREATE TABLE dbo.node_tags
  (
    node_id bigint       not null,
    [key]   varchar(100) not null,
    value   varchar(500) null
  );


if object_id('dbo.way', 'U') is null
  CREATE TABLE dbo.way 
  (
    id            bigint   not null,
    changeset_id  bigint   null,
    [timestamp]   datetime null,
    visible       bit      null,
    [version]     integer  null,
    usr           varchar(100) null,
    usr_id        integer  null
  ); 


if object_id('dbo.way_tags', 'U') is null
  CREATE TABLE dbo.way_tags 
  (
    way_id bigint       not null,
    [key]  varchar(255) not null,
    value  varchar(500) null
  ); 


if object_id('dbo.way_nodes', 'U') is null
  CREATE TABLE dbo.way_nodes 
  (
    way_id      bigint  not null,
    node_id     bigint  not null,
    sequence_id integer not null
  ); 


if object_id('dbo.relation', 'U') is null
  CREATE TABLE dbo.relation 
  (
    id            bigint   not null,
    changeset_id  bigint   null,
    [timestamp]   datetime null,
    visible       bit      null,
    [version]     integer  null,
    usr           varchar(100) null,
    usr_id        integer  null
  ); 


if object_id('dbo.relation_tags', 'U') is null
  CREATE TABLE dbo.relation_tags 
  (
    relation_id bigint       not null,
    [key]       varchar(100) not null,
    value       varchar(500) null
  ); 


if object_id('dbo.relation_members', 'U') is null
  CREATE TABLE dbo.relation_members 
  (
    relation_id bigint       not null,
    member_type int			 null,
    member_id   bigint       not null,
    member_role varchar(100) null,
    sequence_id integer      not null
  ); 
