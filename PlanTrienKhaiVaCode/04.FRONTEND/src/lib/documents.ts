// API client for Documents
import { apiClient } from './api';

export interface DocumentDto {
  id: string;
  title: string;
  originalFilename: string;
  fileSize: number;
  mimeType: string;
  status: string;
  createdAt: string;
  categoryId: string | null;
  categoryName: string | null;
}

export interface DocumentUploadResult {
  id: string;
  title: string;
  status: string;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const documentApi = {
  upload: async (file: File, categoryId?: string): Promise<DocumentUploadResult> => {
    const formData = new FormData();
    formData.append('file', file);
    if (categoryId) {
      formData.append('categoryId', categoryId);
    }
    const response = await apiClient.post<DocumentUploadResult>('/api/v1/documents/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  list: async (page = 1, pageSize = 20, categoryId?: string, status?: string): Promise<PagedResult<DocumentDto>> => {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
    });
    if (categoryId) params.append('categoryId', categoryId);
    if (status) params.append('status', status);
    
    const response = await apiClient.get<PagedResult<DocumentDto>>(`/api/v1/documents?${params}`);
    return response.data;
  },

  getById: async (id: string): Promise<DocumentDto> => {
    const response = await apiClient.get<DocumentDto>(`/api/v1/documents/${id}`);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/api/v1/documents/${id}`);
  },

  getStatus: async (id: string): Promise<string> => {
    const response = await apiClient.get<{ id: string; status: string }>(`/api/v1/documents/${id}/status`);
    return response.data.status;
  },

  download: async (id: string): Promise<Blob> => {
    const response = await apiClient.get(`/api/v1/documents/${id}/download`, {
      responseType: 'blob',
    });
    return response.data;
  },
};

export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 B';
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

export function getFileIcon(mimeType: string): string {
  if (mimeType.includes('pdf')) return '📄';
  if (mimeType.includes('word') || mimeType.includes('doc')) return '📝';
  if (mimeType.includes('sheet') || mimeType.includes('excel')) return '📊';
  if (mimeType.includes('image')) return '🖼️';
  if (mimeType.includes('text')) return '📃';
  return '📎';
}
