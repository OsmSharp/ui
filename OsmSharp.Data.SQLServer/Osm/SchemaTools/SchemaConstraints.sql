-- Fix any faults
UPDATE node_tags SET [key] = LTRIM(RTRIM([key])) WHERE [key] <> LTRIM(RTRIM([key]))




-- Create indexes
IF NOT EXISTS (SELECT  1 FROM sysindexes WHERE name = 'IDX_NODE_TILE' AND id = OBJECT_ID('dbo.node'))
    CREATE INDEX IDX_NODE_TILE ON node(tile  ASC) ;

IF NOT EXISTS (SELECT 1 FROM sysindexes WHERE name = 'IDX_WAY_NODES_NODE' AND id = OBJECT_ID('dbo.way_nodes'))
    CREATE INDEX IDX_WAY_NODES_NODE ON dbo.way_nodes(node_id  ASC) ;

--IF NOT EXISTS (SELECT 1 FROM  sysindexes WHERE name = 'IDX_WAY_NODES_WAY_SEQUENCE' AND id = OBJECT_ID('dbo.way_nodes'))
--    CREATE INDEX IDX_WAY_NODES_WAY_SEQUENCE ON dbo.way_nodes(way_id  ASC,sequence_id  ASC) ;




-- Remove duplicates
-- SELECT id, COUNT(1) AS n FROM node GROUP BY id HAVING COUNT(1) > 1
DELETE f
FROM (
    SELECT ROW_NUMBER() OVER (PARTITION BY id ORDER BY id) AS recID
    FROM node
) AS f
WHERE recID > 1

-- SELECT node_id,[key], COUNT(1) AS n FROM node_tags GROUP BY node_id,[key] HAVING COUNT(1) > 1
DELETE f
FROM (
    SELECT ROW_NUMBER() OVER (PARTITION BY node_id,[key] ORDER BY node_id) AS recID
    FROM node_tags
) AS f
WHERE recID > 1

-- SELECT id, COUNT(1) AS n FROM way GROUP BY id HAVING COUNT(1) > 1
DELETE f
FROM (
    SELECT ROW_NUMBER() OVER (PARTITION BY id ORDER BY id) AS recID
    FROM way
) AS f
WHERE recID > 1

-- SELECT way_id, [key], COUNT(1) AS n FROM way_tags GROUP BY way_id, [key] HAVING COUNT(1) > 1
DELETE f
FROM (
    SELECT ROW_NUMBER() OVER (PARTITION BY way_id, [key] ORDER BY way_id) AS recID
    FROM way_tags
) AS f
WHERE recID > 1

-- SELECT way_id, node_id, sequence_id, COUNT(1) AS n FROM way_nodes GROUP BY way_id, node_id, sequence_id HAVING COUNT(1) > 1
DELETE f
FROM (
    SELECT ROW_NUMBER() OVER (PARTITION BY way_id, node_id, sequence_id ORDER BY way_id) AS recID
    FROM way_nodes
) AS f
WHERE recID > 1

-- SELECT id, COUNT(1) AS n FROM relation GROUP BY id HAVING COUNT(1) > 1
DELETE f
FROM (
    SELECT ROW_NUMBER() OVER (PARTITION BY id ORDER BY id) AS recID
    FROM relation
) AS f
WHERE recID > 1

-- SELECT relation_id, [key], COUNT(1) AS n FROM relation_tags GROUP BY relation_id, [key] HAVING COUNT(1) > 1
DELETE f
FROM (
    SELECT ROW_NUMBER() OVER (PARTITION BY relation_id, [key] ORDER BY relation_id) AS recID
    FROM relation_tags
) AS f
WHERE recID > 1

-- SELECT relation_id, member_id, sequence_id, COUNT(1) AS n FROM relation_members GROUP BY relation_id, member_id, sequence_id HAVING COUNT(1) > 1
DELETE f
FROM (
    SELECT ROW_NUMBER() OVER (PARTITION BY relation_id, member_id, sequence_id ORDER BY relation_id) AS recID
    FROM relation_members
) AS f
WHERE recID > 1







-- Create constraints. Primary keys are clustered for read performance
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type_desc = 'PRIMARY_KEY_CONSTRAINT' AND OBJECT_NAME(OBJECT_ID) = 'PK_node')
    ALTER TABLE node ADD CONSTRAINT PK_node PRIMARY KEY CLUSTERED (id)

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type_desc = 'PRIMARY_KEY_CONSTRAINT' AND OBJECT_NAME(OBJECT_ID) = 'PK_node_tags')
    ALTER TABLE node_tags ADD CONSTRAINT PK_node_tags PRIMARY KEY CLUSTERED (node_id,[key])

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type_desc = 'PRIMARY_KEY_CONSTRAINT' AND OBJECT_NAME(OBJECT_ID) = 'PK_way')
    ALTER TABLE way ADD CONSTRAINT PK_way PRIMARY KEY CLUSTERED (id)

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type_desc = 'PRIMARY_KEY_CONSTRAINT' AND OBJECT_NAME(OBJECT_ID) = 'PK_way_tags')
    ALTER TABLE way_tags ADD CONSTRAINT PK_way_tags PRIMARY KEY CLUSTERED (way_id, [key])

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type_desc = 'PRIMARY_KEY_CONSTRAINT' AND OBJECT_NAME(OBJECT_ID) = 'PK_way_nodes')
    ALTER TABLE way_nodes ADD CONSTRAINT PK_way_nodes PRIMARY KEY CLUSTERED (way_id, node_id, sequence_id)

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type_desc = 'PRIMARY_KEY_CONSTRAINT' AND OBJECT_NAME(OBJECT_ID) = 'PK_relation')
    ALTER TABLE relation ADD CONSTRAINT PK_relation PRIMARY KEY CLUSTERED (id)

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type_desc = 'PRIMARY_KEY_CONSTRAINT' AND OBJECT_NAME(OBJECT_ID) = 'PK_relation_tags')
    ALTER TABLE relation_tags ADD CONSTRAINT PK_relation_tags PRIMARY KEY CLUSTERED (relation_id, [key])

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type_desc = 'PRIMARY_KEY_CONSTRAINT' AND OBJECT_NAME(OBJECT_ID) = 'PK_relation_members')
    ALTER TABLE relation_members ADD CONSTRAINT PK_relation_members PRIMARY KEY CLUSTERED (relation_id, member_id, sequence_id)



-- Create foreign keys
ALTER TABLE dbo.node_tags ADD CONSTRAINT FK_node_tags_node FOREIGN KEY (node_id) REFERENCES dbo.node (id)
ALTER TABLE dbo.way_tags ADD CONSTRAINT FK_way_tags_way FOREIGN KEY (way_id) REFERENCES dbo.way (id)
ALTER TABLE dbo.way_nodes ADD CONSTRAINT FK_way_nodes_way FOREIGN KEY (way_id) REFERENCES dbo.way (id)
-- ALTER TABLE dbo.way_nodes ADD CONSTRAINT FK_way_nodes_node FOREIGN KEY (node_id) REFERENCES dbo.node (id) (not all nodes will be there in some circumstances)
ALTER TABLE dbo.relation_members ADD CONSTRAINT FK_relation_members_relation FOREIGN KEY (relation_id) REFERENCES dbo.relation (id)
ALTER TABLE dbo.relation_tags ADD CONSTRAINT FK_relation_tags_relation FOREIGN KEY (relation_id) REFERENCES dbo.relation (id)