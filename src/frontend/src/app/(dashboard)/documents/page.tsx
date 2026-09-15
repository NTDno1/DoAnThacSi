'use client';

import { useState, useEffect } from 'react';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { documentApi, formatFileSize, getFileIcon, DocumentDto } from '@/lib/documents';
import Link from 'next/link';

export default function DocumentsPage() {
  const [documents, setDocuments] = useState<DocumentDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [deleteModal, setDeleteModal] = useState<{ show: boolean; doc: DocumentDto | null }>({ show: false, doc: null });

  const pageSize = 12;

  useEffect(() => {
    loadDocuments();
  }, [page, statusFilter]);

  const loadDocuments = async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await documentApi.list(page, pageSize, undefined, statusFilter || undefined);
      setDocuments(result.items);
      setTotalPages(result.totalPages);
      setTotalCount(result.totalCount);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Không thể tải danh sách tài liệu');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteModal.doc) return;
    
    try {
      await documentApi.delete(deleteModal.doc.id);
      setDeleteModal({ show: false, doc: null });
      loadDocuments();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Xóa thất bại');
    }
  };

  const handleDownload = async (doc: DocumentDto) => {
    try {
      const blob = await documentApi.download(doc.id);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = doc.originalFilename;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      alert('Tải xuống thất bại');
    }
  };

  const getStatusBadge = (status: string) => {
    switch (status.toLowerCase()) {
      case 'indexed':
        return <span className="px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-700">Đã lập chỉ mục</span>;
      case 'processing':
        return <span className="px-2 py-1 text-xs font-medium rounded-full bg-yellow-100 text-yellow-700">Đang xử lý</span>;
      case 'failed':
        return <span className="px-2 py-1 text-xs font-medium rounded-full bg-red-100 text-red-700">Thất bại</span>;
      default:
        return <span className="px-2 py-1 text-xs font-medium rounded-full bg-gray-100 text-gray-700">{status}</span>;
    }
  };

  return (
    <DashboardLayout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Tài liệu của tôi</h1>
            <p className="mt-1 text-gray-600">
              {totalCount > 0 ? `${totalCount} tài liệu` : 'Chưa có tài liệu nào'}
            </p>
          </div>
          <Link
            href="/documents/upload"
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium"
          >
            + Tải lên tài liệu
          </Link>
        </div>

        {/* Filters */}
        <div className="flex gap-4 items-center">
          <div className="flex-1">
            <input
              type="text"
              placeholder="Tìm kiếm tài liệu..."
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
          <select
            value={statusFilter}
            onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">Tất cả trạng thái</option>
            <option value="Indexed">Đã lập chỉ mục</option>
            <option value="Processing">Đang xử lý</option>
            <option value="Failed">Thất bại</option>
          </select>
        </div>

        {/* Documents Grid */}
        {loading ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="animate-pulse">
                <div className="bg-gray-200 rounded-xl h-48"></div>
                <div className="mt-3 bg-gray-200 h-4 rounded w-3/4"></div>
                <div className="mt-2 bg-gray-200 h-3 rounded w-1/2"></div>
              </div>
            ))}
          </div>
        ) : error ? (
          <div className="text-center py-12">
            <p className="text-red-600">{error}</p>
            <button onClick={loadDocuments} className="mt-4 text-blue-600 hover:underline">
              Thử lại
            </button>
          </div>
        ) : documents.length === 0 ? (
          <div className="text-center py-16 bg-gray-50 rounded-xl">
            <span className="text-6xl">📂</span>
            <h3 className="mt-4 text-lg font-medium text-gray-900">Chưa có tài liệu nào</h3>
            <p className="mt-2 text-gray-500">Tải lên tài liệu đầu tiên của bạn</p>
            <Link
              href="/documents/upload"
              className="mt-4 inline-block px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
            >
              Tải lên tài liệu
            </Link>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {documents.map((doc) => (
              <div
                key={doc.id}
                className="bg-white border border-gray-200 rounded-xl overflow-hidden hover:shadow-lg transition-shadow"
              >
                {/* Preview Area */}
                <div className="h-32 bg-gradient-to-br from-blue-50 to-indigo-50 flex items-center justify-center">
                  <span className="text-5xl">{getFileIcon(doc.mimeType)}</span>
                </div>
                
                {/* Content */}
                <div className="p-4">
                  <div className="flex justify-between items-start">
                    <div className="flex-1 min-w-0">
                      <h3 className="font-semibold text-gray-900 truncate" title={doc.title}>
                        {doc.title}
                      </h3>
                      <p className="text-sm text-gray-500 truncate">{doc.originalFilename}</p>
                    </div>
                    {getStatusBadge(doc.status)}
                  </div>
                  
                  <div className="mt-3 flex items-center gap-4 text-sm text-gray-500">
                    <span>{formatFileSize(doc.fileSize)}</span>
                    <span>•</span>
                    <span>{new Date(doc.createdAt).toLocaleDateString('vi-VN')}</span>
                  </div>

                  {/* Actions */}
                  <div className="mt-4 flex gap-2">
                    <button
                      onClick={() => handleDownload(doc)}
                      className="flex-1 px-3 py-1.5 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
                    >
                      Tải xuống
                    </button>
                    <button
                      onClick={() => setDeleteModal({ show: true, doc })}
                      className="px-3 py-1.5 text-sm text-red-600 border border-red-200 rounded-lg hover:bg-red-50 transition-colors"
                    >
                      Xóa
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex justify-center gap-2 mt-6">
            <button
              onClick={() => setPage(p => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-4 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
            >
              Trước
            </button>
            {[...Array(Math.min(5, totalPages))].map((_, i) => {
              const pageNum = i + 1;
              return (
                <button
                  key={pageNum}
                  onClick={() => setPage(pageNum)}
                  className={`px-4 py-2 border rounded-lg ${
                    page === pageNum ? 'bg-blue-600 text-white border-blue-600' : 'hover:bg-gray-50'
                  }`}
                >
                  {pageNum}
                </button>
              );
            })}
            <button
              onClick={() => setPage(p => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="px-4 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
            >
              Sau
            </button>
          </div>
        )}
      </div>

      {/* Delete Confirmation Modal */}
      {deleteModal.show && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 max-w-md w-full mx-4">
            <h3 className="text-lg font-semibold text-gray-900">Xác nhận xóa</h3>
            <p className="mt-2 text-gray-600">
              Bạn có chắc muốn xóa tài liệu "{deleteModal.doc?.title}"? Hành động này không thể hoàn tác.
            </p>
            <div className="mt-6 flex gap-3">
              <button
                onClick={handleDelete}
                className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
              >
                Xóa
              </button>
              <button
                onClick={() => setDeleteModal({ show: false, doc: null })}
                className="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
              >
                Hủy
              </button>
            </div>
          </div>
        </div>
      )}
    </DashboardLayout>
  );
}
