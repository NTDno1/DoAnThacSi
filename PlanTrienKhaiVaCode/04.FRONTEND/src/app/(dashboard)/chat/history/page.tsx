'use client';

import { useState, useEffect } from 'react';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { apiClient } from '@/lib/api';
import { useRouter } from 'next/navigation';
import Link from 'next/link';

interface ChatSession {
  id: string;
  title: string;
  messageCount: number;
  lastMessagePreview: string | null;
  createdAt: string;
}

export default function ChatHistoryPage() {
  const router = useRouter();
  const [sessions, setSessions] = useState<ChatSession[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deleteModal, setDeleteModal] = useState<{ show: boolean; session: ChatSession | null }>({ show: false, session: null });

  useEffect(() => {
    loadSessions();
  }, []);

  const loadSessions = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await apiClient.get<ChatSession[]>('/api/v1/chat/sessions');
      setSessions(response.data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Không thể tải lịch sử chat');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteModal.session) return;
    
    try {
      await apiClient.delete(`/api/v1/chat/sessions/${deleteModal.session.id}`);
      setDeleteModal({ show: false, session: null });
      loadSessions();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Xóa thất bại');
    }
  };

  const handleCreateNew = async () => {
    try {
      const response = await apiClient.post<ChatSession>('/api/v1/chat/sessions');
      router.push(`/chat/${response.data.id}`);
    } catch (err) {
      alert('Tạo cuộc trò chuyện mới thất bại');
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24));
    
    if (diffDays === 0) return 'Hôm nay';
    if (diffDays === 1) return 'Hôm qua';
    if (diffDays < 7) return `${diffDays} ngày trước`;
    return date.toLocaleDateString('vi-VN');
  };

  return (
    <DashboardLayout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Lịch sử Chat</h1>
            <p className="mt-1 text-gray-600">
              {sessions.length > 0 ? `${sessions.length} cuộc trò chuyện` : 'Chưa có cuộc trò chuyện nào'}
            </p>
          </div>
          <button
            onClick={handleCreateNew}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium flex items-center gap-2"
          >
            <span>+</span> Cuộc trò chuyện mới
          </button>
        </div>

        {/* Sessions List */}
        {loading ? (
          <div className="space-y-4">
            {[...Array(5)].map((_, i) => (
              <div key={i} className="animate-pulse">
                <div className="bg-gray-200 h-20 rounded-xl"></div>
              </div>
            ))}
          </div>
        ) : error ? (
          <div className="text-center py-12">
            <p className="text-red-600">{error}</p>
            <button onClick={loadSessions} className="mt-4 text-blue-600 hover:underline">
              Thử lại
            </button>
          </div>
        ) : sessions.length === 0 ? (
          <div className="text-center py-16 bg-gray-50 rounded-xl">
            <span className="text-6xl">💬</span>
            <h3 className="mt-4 text-lg font-medium text-gray-900">Chưa có cuộc trò chuyện nào</h3>
            <p className="mt-2 text-gray-500">Bắt đầu một cuộc trò chuyện mới với AI</p>
            <button
              onClick={handleCreateNew}
              className="mt-4 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
            >
              Bắt đầu trò chuyện
            </button>
          </div>
        ) : (
          <div className="space-y-3">
            {sessions.map((session) => (
              <div
                key={session.id}
                className="bg-white border border-gray-200 rounded-xl p-4 hover:shadow-md transition-shadow cursor-pointer"
                onClick={() => router.push(`/chat/${session.id}`)}
              >
                <div className="flex items-start justify-between">
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-3">
                      <span className="text-xl">💬</span>
                      <h3 className="font-semibold text-gray-900 truncate">
                        {session.title}
                      </h3>
                    </div>
                    {session.lastMessagePreview && (
                      <p className="mt-2 text-sm text-gray-500 truncate">
                        {session.lastMessagePreview}
                      </p>
                    )}
                    <div className="mt-2 flex items-center gap-4 text-xs text-gray-400">
                      <span>{formatDate(session.createdAt)}</span>
                      <span>•</span>
                      <span>{session.messageCount} tin nhắn</span>
                    </div>
                  </div>
                  
                  <div className="flex items-center gap-2 ml-4">
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        router.push(`/chat/${session.id}`);
                      }}
                      className="p-2 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                      title="Mở cuộc trò chuyện"
                    >
                      <span className="text-lg">→</span>
                    </button>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        setDeleteModal({ show: true, session });
                      }}
                      className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                      title="Xóa cuộc trò chuyện"
                    >
                      <span className="text-lg">🗑️</span>
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Quick Actions */}
        <div className="mt-8 p-6 bg-gradient-to-r from-blue-50 to-indigo-50 rounded-xl border border-blue-100">
          <h3 className="font-semibold text-blue-800 mb-4">Bắt đầu nhanh</h3>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <Link
              href="/search"
              className="p-4 bg-white rounded-lg border border-gray-200 hover:border-blue-300 hover:shadow-md transition-all"
            >
              <div className="flex items-center gap-3">
                <span className="text-2xl">🔍</span>
                <div>
                  <h4 className="font-medium text-gray-900">Tìm kiếm ngữ nghĩa</h4>
                  <p className="text-sm text-gray-500">Tìm kiếm trong tài liệu</p>
                </div>
              </div>
            </Link>
            <Link
              href="/agent"
              className="p-4 bg-white rounded-lg border border-gray-200 hover:border-blue-300 hover:shadow-md transition-all"
            >
              <div className="flex items-center gap-3">
                <span className="text-2xl">🤖</span>
                <div>
                  <h4 className="font-medium text-gray-900">AI Agent</h4>
                  <p className="text-sm text-gray-500">Trợ lý thông minh</p>
                </div>
              </div>
            </Link>
            <Link
              href="/documents"
              className="p-4 bg-white rounded-lg border border-gray-200 hover:border-blue-300 hover:shadow-md transition-all"
            >
              <div className="flex items-center gap-3">
                <span className="text-2xl">📁</span>
                <div>
                  <h4 className="font-medium text-gray-900">Quản lý tài liệu</h4>
                  <p className="text-sm text-gray-500">Upload và xem tài liệu</p>
                </div>
              </div>
            </Link>
          </div>
        </div>
      </div>

      {/* Delete Confirmation Modal */}
      {deleteModal.show && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 max-w-md w-full mx-4">
            <h3 className="text-lg font-semibold text-gray-900">Xác nhận xóa</h3>
            <p className="mt-2 text-gray-600">
              Bạn có chắc muốn xóa cuộc trò chuyện "{deleteModal.session?.title}"? Hành động này không thể hoàn tác.
            </p>
            <div className="mt-6 flex gap-3">
              <button
                onClick={handleDelete}
                className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700"
              >
                Xóa
              </button>
              <button
                onClick={() => setDeleteModal({ show: false, session: null })}
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
