-- Drop tables in order
IF OBJECT_ID('dbo.relation_tags', 'U') IS NOT NULL 
    DROP TABLE dbo.relation_tags 
IF OBJECT_ID('dbo.relation_members', 'U') IS NOT NULL 
    DROP TABLE dbo.relation_members 
IF OBJECT_ID('dbo.relation', 'U') IS NOT NULL 
    DROP TABLE dbo.relation 
IF OBJECT_ID('dbo.node_tags', 'U') IS NOT NULL 
    DROP TABLE dbo.node_tags 
IF OBJECT_ID('dbo.way_nodes', 'U') IS NOT NULL 
    DROP TABLE dbo.way_nodes 
IF OBJECT_ID('dbo.way_tags', 'U') IS NOT NULL 
    DROP TABLE dbo.way_tags 
IF OBJECT_ID('dbo.way', 'U') IS NOT NULL 
    DROP TABLE dbo.way 
IF OBJECT_ID('dbo.node', 'U') IS NOT NULL 
    DROP TABLE dbo.node
