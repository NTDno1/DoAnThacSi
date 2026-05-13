-- ============================================
-- Database Migration Script
-- AI Base Framework - Enterprise AI Platform
-- Version: 1.0.0
-- ============================================

-- This script handles database schema migrations
-- Run this script after initial setup or when updating

-- ============================================
-- MIGRATION LOG TABLE
-- ============================================

CREATE TABLE IF NOT EXISTS schema_migrations (
    id SERIAL PRIMARY KEY,
    version VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    applied_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    checksum VARCHAR(64)
);

-- ============================================
-- VERSION 1.0.0 - Initial Schema
-- ============================================

DO $$
DECLARE
    version_exists BOOLEAN;
BEGIN
    -- Check if migration already applied
    SELECT EXISTS (
        SELECT 1 FROM schema_migrations WHERE version = '1.0.0'
    ) INTO version_exists;
    
    IF NOT version_exists THEN
        -- Record migration start
        INSERT INTO schema_migrations (version, description) 
        VALUES ('1.0.0', 'Initial schema with pgvector support');
        
        -- Enable extensions (if not already enabled)
        CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
        CREATE EXTENSION IF NOT EXISTS "pg_trgm";
        CREATE EXTENSION IF NOT EXISTS "vector";
        
        -- ENUMS
        DO $$ BEGIN
            CREATE TYPE user_role AS ENUM ('admin', 'manager', 'teacher', 'student', 'guest');
        EXCEPTION WHEN duplicate_object THEN null;
        END $$;
        
        DO $$ BEGIN
            CREATE TYPE document_status AS ENUM ('pending', 'processing', 'indexed', 'failed', 'deleted');
        EXCEPTION WHEN duplicate_object THEN null;
        END $$;
        
        DO $$ BEGIN
            CREATE TYPE message_role AS ENUM ('system', 'user', 'assistant');
        EXCEPTION WHEN duplicate_object THEN null;
        END $$;
        
        RAISE NOTICE 'Migration 1.0.0 applied successfully';
    ELSE
        RAISE NOTICE 'Migration 1.0.0 already applied, skipping';
    END IF;
END $$;

-- ============================================
-- HELPER FUNCTIONS
-- ============================================

-- Function to get schema version
CREATE OR REPLACE FUNCTION get_schema_version()
RETURNS VARCHAR(50) AS $$
DECLARE
    latest_version VARCHAR(50);
BEGIN
    SELECT version INTO latest_version
    FROM schema_migrations
    ORDER BY applied_at DESC
    LIMIT 1;
    
    RETURN COALESCE(latest_version, '0.0.0');
END;
$$ LANGUAGE plpgsql;

-- Function to check if table exists
CREATE OR REPLACE FUNCTION table_exists(table_name VARCHAR)
RETURNS BOOLEAN AS $$
DECLARE
    exists_flag BOOLEAN;
BEGIN
    SELECT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'public' 
        AND table_name = get_schema_version.table_name
    ) INTO exists_flag;
    RETURN exists_flag;
END;
$$ LANGUAGE plpgsql;

-- ============================================
-- MAINTENANCE FUNCTIONS
-- ============================================

-- Function to vacuum and analyze all tables
CREATE OR REPLACE FUNCTION vacuum_analyze_all()
RETURNS void AS $$
BEGIN
    -- Vacuum to reclaim space
    VACUUM (VERBOSE, ANALYZE);
    
    -- Specifically for document_chunks which grows large
    VACUUM (VERBOSE, ANALYZE) document_chunks;
    VACUUM (VERBOSE, ANALYZE) documents;
    VACUUM (VERBOSE, ANALYZE) chat_messages;
    VACUUM (VERBOSE, ANALYZE) search_logs;
    
    RAISE NOTICE 'Vacuum and analyze completed';
END;
$$ LANGUAGE plpgsql;

-- Function to get table statistics
CREATE OR REPLACE FUNCTION get_table_stats()
RETURNS TABLE(
    table_name VARCHAR,
    row_count BIGINT,
    total_size TEXT,
    index_size TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        t.table_name::VARCHAR,
        (xpath('/row/cnt/text()', xml_count))[1]::BIGINT as row_count,
        pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as total_size,
        pg_size_pretty(pg_indexes_size(schemaname||'.'||tablename)) as index_size
    FROM information_schema.tables t
    JOIN pg_tables pt ON pt.schemaname = t.table_schema AND pt.tablename = t.table_name
    LEFT JOIN (
        SELECT tablename, query_to_xml('SELECT count(*) as cnt FROM '||tablename, false, false, '') as xml_count
        FROM pg_tables WHERE schemaname = 'public'
    ) x ON x.tablename = t.table_name
    WHERE t.table_schema = 'public' 
    AND t.table_type = 'BASE TABLE'
    ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
END;
$$ LANGUAGE plpgsql;

-- ============================================
-- INDEX MAINTENANCE
-- ============================================

-- Function to reindex vector columns (useful after bulk inserts)
CREATE OR REPLACE FUNCTION reindex_vector_columns()
RETURNS void AS $$
BEGIN
    -- Reindex vector similarity index
    REINDEX INDEX CONCURRENTLY IF EXISTS idx_document_chunks_embedding_hnsw;
    REINDEX INDEX CONCURRENTLY IF EXISTS idx_document_chunks_doc_embedding;
    
    -- Reindex full-text search index
    REINDEX INDEX CONCURRENTLY IF EXISTS idx_document_chunks_content_fts;
    
    -- Reindex trigram indexes for fuzzy search
    REINDEX INDEX CONCURRENTLY IF EXISTS idx_documents_title_trgm;
    
    RAISE NOTICE 'Vector and text indexes reindexed';
END;
$$ LANGUAGE plpgsql;

-- ============================================
-- DATA CLEANUP FUNCTIONS
-- ============================================

-- Function to cleanup old search logs (older than specified days)
CREATE OR REPLACE FUNCTION cleanup_search_logs(days_to_keep INTEGER DEFAULT 90)
RETURNS INTEGER AS $$
DECLARE
    deleted_count INTEGER;
BEGIN
    WITH deleted AS (
        DELETE FROM search_logs 
        WHERE created_at < NOW() - (days_to_keep || ' days')::INTERVAL
        RETURNING id
    )
    SELECT COUNT(*) INTO deleted_count FROM deleted;
    
    RAISE NOTICE 'Deleted % search log entries older than % days', deleted_count, days_to_keep;
    RETURN deleted_count;
END;
$$ LANGUAGE plpgsql;

-- Function to cleanup orphaned document chunks
CREATE OR REPLACE FUNCTION cleanup_orphaned_chunks()
RETURNS INTEGER AS $$
DECLARE
    deleted_count INTEGER;
BEGIN
    WITH orphaned AS (
        DELETE FROM document_chunks dc
        WHERE NOT EXISTS (
            SELECT 1 FROM documents d WHERE d.id = dc.document_id
        )
        RETURNING id
    )
    SELECT COUNT(*) INTO deleted_count FROM orphaned;
    
    RAISE NOTICE 'Deleted % orphaned document chunks', deleted_count;
    RETURN deleted_count;
END;
$$ LANGUAGE plpgsql;

-- ============================================
-- MONITORING VIEWS
-- ============================================

-- View for document statistics
CREATE OR REPLACE VIEW document_stats AS
SELECT 
    d.status,
    COUNT(*) as count,
    SUM(d.file_size) as total_size,
    AVG(d.file_size) as avg_size,
    SUM(d.page_count) as total_pages
FROM documents d
GROUP BY d.status;

-- View for user activity
CREATE OR REPLACE VIEW user_activity AS
SELECT 
    u.id,
    u.email,
    u.full_name,
    u.role,
    COUNT(DISTINCT cs.id) as chat_sessions,
    COUNT(DISTINCT d.id) as documents_uploaded,
    COALESCE(SUM(sl.results_count), 0) as total_searches,
    u.last_login_at
FROM users u
LEFT JOIN chat_sessions cs ON cs.user_id = u.id
LEFT JOIN documents d ON d.uploader_id = u.id
LEFT JOIN search_logs sl ON sl.user_id = u.id
GROUP BY u.id, u.email, u.full_name, u.role, u.last_login_at;

-- View for system health
CREATE OR REPLACE VIEW system_health AS
SELECT 
    'documents' as metric,
    COUNT(*) as count
FROM documents
UNION ALL
SELECT 
    'indexed_documents' as metric,
    COUNT(*) as count
FROM documents WHERE status = 'indexed'
UNION ALL
SELECT 
    'total_chunks' as metric,
    COUNT(*) as count
FROM document_chunks
UNION ALL
SELECT 
    'chat_sessions' as metric,
    COUNT(*) as count
FROM chat_sessions
UNION ALL
SELECT 
    'searches_today' as metric,
    COUNT(*) as count
FROM search_logs 
WHERE created_at >= CURRENT_DATE;

-- ============================================
-- GRANT PERMISSIONS
-- ============================================

-- Create application user (run as superuser)
-- CREATE USER aibfapp WITH PASSWORD 'secure_password';
-- GRANT CONNECT ON DATABASE aibaseframework TO aibfapp;
-- GRANT USAGE ON SCHEMA public TO aibfapp;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO aibfapp;
-- GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO aibfapp;

-- ============================================
-- FINALIZE
-- ============================================

SELECT 'Database migration script completed' as status;
SELECT get_schema_version() as current_version;
