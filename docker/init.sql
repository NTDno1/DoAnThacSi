-- ==============================================================================
-- DATABASE INITIALIZATION SCRIPT
-- Đề tài Thạc sĩ: Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ
-- Sử dụng Semantic Search và RAG Chatbot
-- ==============================================================================
-- Chạy tự động khi PostgreSQL container khởi tạo lần đầu
-- ==============================================================================

-- ==============================================================================
-- PHẦN 1: EXTENSIONS - Các extension cần thiết
-- ==============================================================================

-- Enable pgvector extension cho vector similarity search
CREATE EXTENSION IF NOT EXISTS vector;

-- Enable UUID generation
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Enable pg_trgm cho text search (fallback)
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- ==============================================================================
-- PHẦN 2: TABLES - Bảng chính của hệ thống
-- ==============================================================================

-- Bảng người dùng
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL DEFAULT 'user',
    department VARCHAR(100),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    last_login_at TIMESTAMP WITH TIME ZONE,
    CONSTRAINT users_email_unique UNIQUE (email)
);

-- Index cho tìm kiếm người dùng
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_role ON users(role);
CREATE INDEX IF NOT EXISTS idx_users_department ON users(department);

-- Bảng danh mục tài liệu
CREATE TABLE IF NOT EXISTS document_categories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) NOT NULL UNIQUE,
    description TEXT,
    parent_id UUID REFERENCES document_categories(id),
    sort_order INT DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_categories_slug ON document_categories(slug);
CREATE INDEX IF NOT EXISTS idx_categories_parent ON document_categories(parent_id);

-- Bảng tài liệu
CREATE TABLE IF NOT EXISTS documents (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    title VARCHAR(500) NOT NULL,
    slug VARCHAR(500) NOT NULL,
    description TEXT,
    file_path VARCHAR(1000),
    file_size BIGINT,
    mime_type VARCHAR(100),
    category_id UUID REFERENCES document_categories(id),
    uploaded_by UUID REFERENCES users(id),
    status VARCHAR(50) DEFAULT 'draft',
    version INT DEFAULT 1,
    is_public BOOLEAN DEFAULT FALSE,
    tags TEXT[],
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    published_at TIMESTAMP WITH TIME ZONE,
    CONSTRAINT documents_slug_unique UNIQUE (category_id, slug)
);

CREATE INDEX IF NOT EXISTS idx_documents_slug ON documents(slug);
CREATE INDEX IF NOT EXISTS idx_documents_category ON documents(category_id);
CREATE INDEX IF NOT EXISTS idx_documents_uploaded_by ON documents(uploaded_by);
CREATE INDEX IF NOT EXISTS idx_documents_status ON documents(status);
CREATE INDEX IF NOT EXISTS idx_documents_tags ON documents USING GIN(tags);

-- ==============================================================================
-- PHẦN 3: VECTOR EMBEDDINGS - Bảng cho Semantic Search
-- ==============================================================================

-- Bảng lưu trữ vector embeddings cho semantic search
CREATE TABLE IF NOT EXISTS document_chunks (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    document_id UUID NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    chunk_index INT NOT NULL,
    content TEXT NOT NULL,
    content_hash VARCHAR(64) NOT NULL,
    embedding VECTOR(1536),  -- OpenAI text-embedding-3-small: 1536 dimensions
    metadata JSONB,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(document_id, chunk_index)
);

-- Index cho vector similarity search (HNSW algorithm - hiệu năng cao)
CREATE INDEX IF NOT EXISTS idx_chunks_embedding_hnsw 
    ON document_chunks USING hnsw (embedding vector_cosine_ops);

-- Index cho tìm kiếm hybrid (vector + keyword)
CREATE INDEX IF NOT EXISTS idx_chunks_content_trgm 
    ON document_chunks USING GIN(content gin_trgm_ops);

CREATE INDEX IF NOT EXISTS idx_chunks_document ON document_chunks(document_id);

-- ==============================================================================
-- PHẦN 4: CHAT SESSIONS - Quản lý phiên hỏi đáp
-- ==============================================================================

CREATE TABLE IF NOT EXISTS chat_sessions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id),
    title VARCHAR(255),
    context JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    ended_at TIMESTAMP WITH TIME ZONE,
    message_count INT DEFAULT 0
);

CREATE INDEX IF NOT EXISTS idx_sessions_user ON chat_sessions(user_id);
CREATE INDEX IF NOT EXISTS idx_sessions_created ON chat_sessions(created_at DESC);

-- Bảng tin nhắn trong chat
CREATE TABLE IF NOT EXISTS chat_messages (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    session_id UUID NOT NULL REFERENCES chat_sessions(id) ON DELETE CASCADE,
    role VARCHAR(20) NOT NULL,  -- 'user', 'assistant', 'system'
    content TEXT NOT NULL,
    tokens_used INT,
    model VARCHAR(100),
    sources JSONB,  -- Lưu trữ source documents đã sử dụng
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_messages_session ON chat_messages(session_id);
CREATE INDEX IF NOT EXISTS idx_messages_created ON chat_messages(created_at DESC);

-- ==============================================================================
-- PHẦN 5: AUDIT LOG - Log hoạt động
-- ==============================================================================

CREATE TABLE IF NOT EXISTS audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    action VARCHAR(100) NOT NULL,
    entity_type VARCHAR(100),
    entity_id UUID,
    details JSONB,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_audit_user ON audit_logs(user_id);
CREATE INDEX IF NOT EXISTS idx_audit_action ON audit_logs(action);
CREATE INDEX IF NOT EXISTS idx_audit_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX IF NOT EXISTS idx_audit_created ON audit_logs(created_at DESC);

-- ==============================================================================
-- PHẦN 6: SEARCH ANALYTICS - Thống kê tìm kiếm
-- ==============================================================================

CREATE TABLE IF NOT EXISTS search_analytics (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    query_text TEXT NOT NULL,
    query_type VARCHAR(50),  -- 'keyword', 'semantic', 'hybrid'
    results_count INT,
    clicked_document_id UUID REFERENCES documents(id),
    session_id UUID,
    response_time_ms INT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_search_user ON search_analytics(user_id);
CREATE INDEX IF NOT EXISTS idx_search_query ON search_analytics(query_text);
CREATE INDEX IF NOT EXISTS idx_search_created ON search_analytics(created_at DESC);

-- ==============================================================================
-- PHẦN 7: SEED DATA - Dữ liệu mẫu
-- ==============================================================================

-- Tạo admin user (password: admin123 - cần hash trong production)
INSERT INTO users (email, password_hash, full_name, role, department)
VALUES (
    'admin@hoalienvien.edu.vn',
    '$2a$11$K3H8QxJ3qYXwPqZ0R7JQ8.UqXqY0J8VvP5bD9N5sL8wY2xE3R4U6',
    'Quản trị viên',
    'admin',
    'Phòng Công nghệ Thông tin'
)
ON CONFLICT (email) DO NOTHING;

-- Tạo người dùng demo
INSERT INTO users (email, password_hash, full_name, role, department)
VALUES 
    ('gv.truongthiminhtu@hln.edu.vn', '$2a$11$demo_hash_1', 'Trương Thị Minh Tú', 'teacher', 'Khoa Toán'),
    ('gv.nguyenvana@hln.edu.vn', '$2a$11$demo_hash_2', 'Nguyễn Văn A', 'teacher', 'Khoa Văn'),
    ('sv.bui_thi_b@hln.edu.vn', '$2a$11$demo_hash_3', 'Bùi Thị B', 'student', 'Khoa Toán - K64')
ON CONFLICT (email) DO NOTHING;

-- Danh mục mẫu
INSERT INTO document_categories (name, slug, description, sort_order) VALUES
    ('Tài liệu giảng dạy', 'tai-lieu-giang-day', 'Giáo trình, bài giảng cho giảng viên', 1),
    ('Quy chế - Quy định', 'quy-che-quy-dinh', 'Các quy chế, quy định của trường', 2),
    ('Biểu mẫu', 'bieu-mau', 'Các loại biểu mẫu hành chính', 3),
    ('Nghiên cứu khoa học', 'nghien-cuu-khoa-hoc', 'Các công trình nghiên cứu', 4)
ON CONFLICT (slug) DO NOTHING;

-- ==============================================================================
-- PHẦN 8: FUNCTIONS - Các function hỗ trợ
-- ==============================================================================

-- Function để cập nhật updated_at tự động
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger cho các bảng có updated_at
DO $$
DECLARE
    tbl TEXT;
BEGIN
    FOREACH tbl IN ARRAY ARRAY['users', 'document_categories', 'documents', 'document_chunks', 'chat_sessions']
    LOOP
        EXECUTE format(
            'CREATE TRIGGER update_%s_updated_at 
             BEFORE UPDATE ON %s 
             FOR EACH ROW EXECUTE FUNCTION update_updated_at_column()',
            tbl, tbl
        );
    END LOOP;
END;
$$;

-- ==============================================================================
-- PHẦN 9: PERMISSIONS - Phân quyền cơ bản
-- ==============================================================================

-- Grant quyền cho application user (sẽ được tạo trong application)
-- Lưu ý: Trong production, nên tạo dedicated user cho app

-- ==============================================================================
-- HOÀN THÀNH
-- ==============================================================================

DO $$
BEGIN
    RAISE NOTICE 'Database initialization completed successfully!';
    RAISE NOTICE 'Vector dimensions: 1536 (OpenAI text-embedding-3-small)';
    RAISE NOTICE 'Admin user: admin@hoalienvien.edu.vn / admin123';
END;
$$;
