# CHECKLIST TRIỂN KHAI

## Pre-Deployment

- [ ] Docker Desktop installed
- [ ] .NET 8 SDK installed
- [ ] Node.js 20 installed
- [ ] Git configured

## Infrastructure Setup

- [ ] Docker services start successfully
- [ ] PostgreSQL with pgvector ready
- [ ] Redis cache working
- [ ] MinIO buckets created
- [ ] Ollama API accessible

## Backend Setup

- [ ] .NET packages restored
- [ ] Database migrations applied
- [ ] Seed data loaded
- [ ] Backend builds without errors
- [ ] Backend runs on port 5000

## Frontend Setup

- [ ] Node packages installed
- [ ] Frontend builds successfully
- [ ] Frontend runs on port 3000

## Ollama Models

- [ ] llama3.2:3b downloaded
- [ ] nomic-embed-text downloaded
- [ ] Models load successfully

## Testing

- [ ] Login works with admin user
- [ ] Document upload works
- [ ] Semantic search returns results
- [ ] Chatbot responds with RAG
- [ ] Citations displayed correctly
- [ ] Admin dashboard shows stats

## Production Preparation

- [ ] All passwords changed
- [ ] JWT secret regenerated
- [ ] SSL certificates configured (if needed)
- [ ] Backup script tested
- [ ] Monitoring configured
