-- ============================================
-- Database Initialization Script
-- AI Base Framework - Tích hợp AI cho mọi hệ thống
-- ============================================

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";
CREATE EXTENSION IF NOT EXISTS "vector";

-- ============================================
-- ENUMS
-- ============================================

-- User Roles
DO $$ BEGIN
    CREATE TYPE user_role AS ENUM ('admin', 'manager', 'teacher', 'student', 'guest');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- Document Status
DO $$ BEGIN
    CREATE TYPE document_status AS ENUM ('pending', 'processing', 'indexed', 'failed', 'deleted');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- Message Role
DO $$ BEGIN
    CREATE TYPE message_role AS ENUM ('system', 'user', 'assistant');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- ============================================
-- TABLES
-- ============================================

-- Users table
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    avatar_url VARCHAR(500),
    department VARCHAR(255),
    role user_role NOT NULL DEFAULT 'student',
    is_active BOOLEAN NOT NULL DEFAULT true,
    email_verified_at TIMESTAMP WITH TIME ZONE,
    last_login_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_role ON users(role);
CREATE INDEX IF NOT EXISTS idx_users_department ON users(department);
CREATE INDEX IF NOT EXISTS idx_users_is_active ON users(is_active) WHERE is_active = true;

-- Refresh Tokens table
CREATE TABLE IF NOT EXISTS refresh_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token_hash VARCHAR(255) NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    revoked_at TIMESTAMP WITH TIME ZONE,
    revoked_by_user_id UUID REFERENCES users(id),
    user_agent VARCHAR(500),
    ip_address VARCHAR(45),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_id ON refresh_tokens(user_id);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_token_hash ON refresh_tokens(token_hash);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_expires_at ON refresh_tokens(expires_at);

-- Document Categories table
CREATE TABLE IF NOT EXISTS document_categories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) NOT NULL UNIQUE,
    description TEXT,
    parent_id UUID REFERENCES document_categories(id) ON DELETE SET NULL,
    color VARCHAR(7) DEFAULT '#3B82F6',
    icon VARCHAR(50) DEFAULT 'file-text',
    sort_order INT DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_document_categories_slug ON document_categories(slug);
CREATE INDEX IF NOT EXISTS idx_document_categories_parent_id ON document_categories(parent_id);

-- Documents table
CREATE TABLE IF NOT EXISTS documents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    title VARCHAR(500) NOT NULL,
    original_filename VARCHAR(500) NOT NULL,
    storage_path VARCHAR(1000) NOT NULL,
    file_size BIGINT NOT NULL,
    mime_type VARCHAR(100) NOT NULL,
    file_hash VARCHAR(64) NOT NULL,
    category_id UUID REFERENCES document_categories(id) ON DELETE SET NULL,
    uploader_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    status document_status NOT NULL DEFAULT 'pending',
    extracted_text TEXT,
    page_count INT,
    language VARCHAR(10) DEFAULT 'vi',
    metadata JSONB DEFAULT '{}',
    is_public BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    indexed_at TIMESTAMP WITH TIME ZONE
);

CREATE INDEX IF NOT EXISTS idx_documents_category_id ON documents(category_id);
CREATE INDEX IF NOT EXISTS idx_documents_uploader_id ON documents(uploader_id);
CREATE INDEX IF NOT EXISTS idx_documents_status ON documents(status);
CREATE INDEX IF NOT EXISTS idx_documents_created_at ON documents(created_at);
CREATE INDEX IF NOT EXISTS idx_documents_file_hash ON documents(file_hash);
CREATE INDEX IF NOT EXISTS idx_documents_metadata ON documents USING GIN(metadata);
CREATE INDEX IF NOT EXISTS idx_documents_title_trgm ON documents USING GIN(title gin_trgm_ops);

-- Document Chunks table (CRITICAL for vector search)
CREATE TABLE IF NOT EXISTS document_chunks (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    document_id UUID NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    chunk_index INT NOT NULL,
    content TEXT NOT NULL,
    content_hash VARCHAR(64) NOT NULL,
    embedding VECTOR(768) NOT NULL,
    page_number INT,
    section_title VARCHAR(500),
    start_char_position INT,
    end_char_position INT,
    token_count INT,
    metadata JSONB DEFAULT '{}',
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_document_chunks_document_id ON document_chunks(document_id);
CREATE INDEX IF NOT EXISTS idx_document_chunks_content_hash ON document_chunks(content_hash);

-- Vector Index (HNSW) - Critical for semantic search performance
CREATE INDEX IF NOT EXISTS idx_document_chunks_embedding_hnsw
ON document_chunks
USING hnsw (embedding vector_cosine_ops)
WITH (m = 16, ef_construction = 64);

-- Full-text search index
CREATE INDEX IF NOT EXISTS idx_document_chunks_content_fts
ON document_chunks
USING GIN (to_tsvector('vietnamese', content));

-- Composite index for filtered search
CREATE INDEX IF NOT EXISTS idx_document_chunks_doc_embedding
ON document_chunks (document_id, embedding vector_cosine_ops)
WHERE is_active = true;

-- Chat Sessions table
CREATE TABLE IF NOT EXISTS chat_sessions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    title VARCHAR(255) NOT NULL DEFAULT 'New Chat',
    message_count INT NOT NULL DEFAULT 0,
    last_message_preview TEXT,
    model VARCHAR(100),
    is_archived BOOLEAN NOT NULL DEFAULT false,
    is_pinned BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_chat_sessions_user_id ON chat_sessions(user_id);
CREATE INDEX IF NOT EXISTS idx_chat_sessions_created_at ON chat_sessions(created_at);
CREATE INDEX IF NOT EXISTS idx_chat_sessions_is_archived ON chat_sessions(is_archived);

-- Chat Messages table
CREATE TABLE IF NOT EXISTS chat_messages (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    session_id UUID NOT NULL REFERENCES chat_sessions(id) ON DELETE CASCADE,
    role message_role NOT NULL,
    content TEXT NOT NULL,
    citations JSONB DEFAULT '[]',
    token_count INT,
    model_used VARCHAR(100),
    latency_ms INT,
    sources_used JSONB DEFAULT '[]',
    token_usage JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_chat_messages_session_id ON chat_messages(session_id);
CREATE INDEX IF NOT EXISTS idx_chat_messages_created_at ON chat_messages(created_at);

-- Search Logs table (Analytics)
CREATE TABLE IF NOT EXISTS search_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE SET NULL,
    session_id UUID REFERENCES chat_sessions(id) ON DELETE SET NULL,
    query_text TEXT NOT NULL,
    query_embedding VECTOR(768),
    results_count INT NOT NULL DEFAULT 0,
    latency_ms INT,
    search_type VARCHAR(20) DEFAULT 'hybrid',
    top_results JSONB DEFAULT '[]',
    user_feedback JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_search_logs_user_id ON search_logs(user_id);
CREATE INDEX IF NOT EXISTS idx_search_logs_session_id ON search_logs(session_id);
CREATE INDEX IF NOT EXISTS idx_search_logs_created_at ON search_logs(created_at);

-- Audit Logs table
CREATE TABLE IF NOT EXISTS audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id) ON DELETE SET NULL,
    action VARCHAR(100) NOT NULL,
    entity_type VARCHAR(100),
    entity_id UUID,
    details JSONB DEFAULT '{}',
    ip_address VARCHAR(45),
    user_agent VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_audit_logs_user_id ON audit_logs(user_id);
CREATE INDEX IF NOT EXISTS idx_audit_logs_action ON audit_logs(action);
CREATE INDEX IF NOT EXISTS idx_audit_logs_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX IF NOT EXISTS idx_audit_logs_created_at ON audit_logs(created_at);

-- System Settings table
CREATE TABLE IF NOT EXISTS system_settings (
    key VARCHAR(255) PRIMARY KEY,
    value JSONB NOT NULL,
    description TEXT,
    is_public BOOLEAN NOT NULL DEFAULT false,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- ============================================
-- SEED DATA
-- ============================================

-- Insert default admin user (password: admin123)
INSERT INTO users (email, password_hash, full_name, department, role)
VALUES (
    'admin@yourcompany.com',
    '$2a$11$rS.h.yQZ6R5eZKJZqXqGeO3VZrGvGvGvGvGvGvGvGvGvGvGvGvGv', -- admin123 (BCrypt hash - replace in production)
    'Quản trị viên',
    'IT',
    'admin'
) ON CONFLICT (email) DO NOTHING;

-- Insert demo users
INSERT INTO users (email, password_hash, full_name, department, role)
VALUES
    ('user@yourcompany.com', '$2a$11$rS.h.yQZ6R5eZKJZqXqGeO3VZrGvGvGvGvGvGvGvGvGvGvGvGvGv', 'Giáo viên Nguyễn Văn A', 'Khoa học máy tính', 'teacher'),
    ('guest@yourcompany.com', '$2a$11$rS.h.yQZ6R5eZKJZqXqGeO3VZrGvGvGvGvGvGvGvGvGvGvGvGvGv', 'Sinh viên Trần Thị B', 'Khach', 'student')
ON CONFLICT (email) DO NOTHING;

-- Insert default document categories
INSERT INTO document_categories (name, slug, description, color, icon, sort_order)
VALUES
    ('Tài liệu học thuật', 'academic', 'Các bài báo, luận văn, đề tài nghiên cứu', '#3B82F6', 'book-open', 1),
    ('Bài giảng', 'lectures', 'Tài liệu bài giảng của giảng viên', '#10B981', 'presentation', 2),
    ('Đề thi', 'exams', 'Đề thi các môn học', '#F59E0B', 'file-check', 3),
    ('Hướng dẫn', 'guides', 'Tài liệu hướng dẫn, tutorial', '#8B5CF6', 'help-circle', 4),
    ('Khác', 'others', 'Các tài liệu khác', '#6B7280', 'folder', 5)
ON CONFLICT (slug) DO NOTHING;

-- Insert default system settings
INSERT INTO system_settings (key, value, description, is_public)
VALUES
    ('embedding_model', '{"value": "nomic-embed-text", "dimensions": 768}', 'Embedding model for vector search', true),
    ('llm_model', '{"value": "llama3.2:3b", "temperature": 0.3, "max_tokens": 2000}', 'LLM model for chatbot', true),
    ('chunk_size', '{"value": 512, "overlap": 100}', 'Text chunking configuration', true),
    ('search_top_k', '{"value": 10, "min_similarity": 0.5}', 'Search configuration', true),
    ('rate_limit', '{"search": 60, "chat": 30, "upload": 10}', 'Rate limits per minute', true)
ON CONFLICT (key) DO NOTHING;

-- ============================================
-- FUNCTIONS & TRIGGERS
-- ============================================

-- Function to auto-update updated_at column
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Triggers for updated_at
DROP TRIGGER IF EXISTS update_users_updated_at ON users;
CREATE TRIGGER update_users_updated_at
    BEFORE UPDATE ON users
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_document_categories_updated_at ON document_categories;
CREATE TRIGGER update_document_categories_updated_at
    BEFORE UPDATE ON document_categories
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_documents_updated_at ON documents;
CREATE TRIGGER update_documents_updated_at
    BEFORE UPDATE ON documents
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_chat_sessions_updated_at ON chat_sessions;
CREATE TRIGGER update_chat_sessions_updated_at
    BEFORE UPDATE ON chat_sessions
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_system_settings_updated_at ON system_settings;
CREATE TRIGGER update_system_settings_updated_at
    BEFORE UPDATE ON system_settings
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- ============================================
-- COMMENTS
-- ============================================

COMMENT ON TABLE users IS 'Người dùng hệ thống';
COMMENT ON TABLE documents IS 'Tài liệu được upload lên hệ thống';
COMMENT ON TABLE document_chunks IS 'Các đoạn text đã được chunk và embedding cho semantic search';
COMMENT ON TABLE chat_sessions IS 'Phiên hỏi đáp với chatbot';
COMMENT ON TABLE chat_messages IS 'Tin nhắn trong phiên chatbot';
COMMENT ON TABLE search_logs IS 'Lịch sử tìm kiếm cho analytics';
COMMENT ON TABLE audit_logs IS 'Nhật ký audit cho security';

COMMENT ON COLUMN document_chunks.embedding IS 'Vector embedding với 768 dimensions (Ollama nomic-embed-text)';

-- ============================================
-- PERMISSIONS (for Row-Level Security)
-- ============================================

-- Enable Row-Level Security (optional - uncomment if needed)
-- ALTER TABLE documents ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE document_chunks ENABLE ROW LEVEL SECURITY;
