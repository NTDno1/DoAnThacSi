'use client';

import { useState, useEffect } from 'react';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { apiClient } from '@/lib/api';

interface DashboardStats {
  totalUsers: number;
  activeUsers: number;
  totalDocuments: number;
  indexedDocuments: number;
  totalChunks: number;
  totalSearches: number;
  searchesToday: number;
  totalChatSessions: number;
  totalMessages: number;
  avgSearchTimeMs: number;
  storageUsedGB: number;
  dailyStats: { date: string; searches: number; documents: number; users: number }[];
}

interface ActivityLog {
  id: string;
  action: string;
  userEmail: string;
  entityType?: string;
  entityId?: string;
  createdAt: string;
}

interface SearchTrend {
  date: string;
  count: number;
  avgTimeMs: number;
  resultsFound: number;
}

interface SystemHealth {
  isHealthy: boolean;
  status: string;
  checkedAt: string;
  components: Record<string, { name: string; isHealthy: boolean; message?: string }>;
}

export default function AdminDashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [activities, setActivities] = useState<ActivityLog[]>([]);
  const [searchTrends, setSearchTrends] = useState<SearchTrend[]>([]);
  const [systemHealth, setSystemHealth] = useState<SystemHealth | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'overview' | 'users' | 'documents' | 'system'>('overview');

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    setLoading(true);
    try {
      const [statsRes, activitiesRes, trendsRes, healthRes] = await Promise.all([
        apiClient.get<DashboardStats>('/api/v1/admin/dashboard/stats'),
        apiClient.get<ActivityLog[]>('/api/v1/admin/dashboard/activities'),
        apiClient.get<SearchTrend[]>('/api/v1/admin/dashboard/search-trends'),
        apiClient.get<SystemHealth>('/api/v1/admin/health'),
      ]);
      
      setStats(statsRes.data);
      setActivities(activitiesRes.data);
      setSearchTrends(trendsRes.data);
      setSystemHealth(healthRes.data);
    } catch (error) {
      console.error('Failed to load dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <DashboardLayout>
        <div className="flex items-center justify-center h-96">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-4 text-gray-600">Đang tải dữ liệu...</p>
          </div>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Bảng điều khiển Quản trị</h1>
            <p className="mt-1 text-gray-600">Theo dõi và quản lý hệ thống AI Platform</p>
          </div>
          <button
            onClick={loadDashboardData}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            Làm mới dữ liệu
          </button>
        </div>

        {/* Tabs */}
        <div className="border-b border-gray-200">
          <nav className="flex gap-6">
            {['overview', 'users', 'documents', 'system'].map((tab) => (
              <button
                key={tab}
                onClick={() => setActiveTab(tab as typeof activeTab)}
                className={`pb-4 px-1 font-medium border-b-2 transition-colors capitalize ${
                  activeTab === tab
                    ? 'border-blue-600 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700'
                }`}
              >
                {tab === 'overview' ? 'Tổng quan' : tab === 'users' ? 'Người dùng' : tab === 'documents' ? 'Tài liệu' : 'Hệ thống'}
              </button>
            ))}
          </nav>
        </div>

        {/* Overview Tab */}
        {activeTab === 'overview' && stats && (
          <>
            {/* Key Metrics */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              <div className="bg-white rounded-xl p-6 border border-gray-200">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-500">Người dùng</p>
                    <p className="text-3xl font-bold text-gray-900">{stats.totalUsers}</p>
                    <p className="text-sm text-green-600">{stats.activeUsers} đang hoạt động</p>
                  </div>
                  <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center text-2xl">👥</div>
                </div>
              </div>

              <div className="bg-white rounded-xl p-6 border border-gray-200">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-500">Tài liệu</p>
                    <p className="text-3xl font-bold text-gray-900">{stats.totalDocuments}</p>
                    <p className="text-sm text-green-600">{stats.indexedDocuments} đã lập chỉ mục</p>
                  </div>
                  <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center text-2xl">📄</div>
                </div>
              </div>

              <div className="bg-white rounded-xl p-6 border border-gray-200">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-500">Tìm kiếm</p>
                    <p className="text-3xl font-bold text-gray-900">{stats.totalSearches.toLocaleString()}</p>
                    <p className="text-sm text-blue-600">{stats.searchesToday} hôm nay</p>
                  </div>
                  <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center text-2xl">🔍</div>
                </div>
              </div>

              <div className="bg-white rounded-xl p-6 border border-gray-200">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-500">Cuộc trò chuyện</p>
                    <p className="text-3xl font-bold text-gray-900">{stats.totalChatSessions}</p>
                    <p className="text-sm text-gray-600">{stats.totalMessages} tin nhắn</p>
                  </div>
                  <div className="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center text-2xl">💬</div>
                </div>
              </div>
            </div>

            {/* Search Trends Chart */}
            <div className="bg-white rounded-xl p-6 border border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Xu hướng tìm kiếm (7 ngày)</h3>
              <div className="h-64 flex items-end gap-2">
                {searchTrends.map((trend, i) => (
                  <div key={i} className="flex-1 flex flex-col items-center">
                    <div
                      className="w-full bg-blue-500 rounded-t transition-all hover:bg-blue-600"
                      style={{ height: `${Math.max(10, (trend.count / Math.max(...searchTrends.map(t => t.count))) * 100)}%` }}
                      title={`${trend.count} tìm kiếm`}
                    ></div>
                    <p className="text-xs text-gray-500 mt-2">
                      {new Date(trend.date).toLocaleDateString('vi-VN', { weekday: 'short' })}
                    </p>
                  </div>
                ))}
              </div>
            </div>

            {/* Recent Activities */}
            <div className="bg-white rounded-xl p-6 border border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Hoạt động gần đây</h3>
              <div className="space-y-3">
                {activities.slice(0, 5).map((activity) => (
                  <div key={activity.id} className="flex items-center gap-4 p-3 bg-gray-50 rounded-lg">
                    <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                      <span>📋</span>
                    </div>
                    <div className="flex-1">
                      <p className="text-sm font-medium text-gray-900">{activity.action}</p>
                      <p className="text-xs text-gray-500">bởi {activity.userEmail}</p>
                    </div>
                    <p className="text-xs text-gray-400">
                      {new Date(activity.createdAt).toLocaleString('vi-VN')}
                    </p>
                  </div>
                ))}
                {activities.length === 0 && (
                  <p className="text-center text-gray-500 py-4">Chưa có hoạt động nào</p>
                )}
              </div>
            </div>
          </>
        )}

        {/* Users Tab */}
        {activeTab === 'users' && (
          <div className="bg-white rounded-xl p-6 border border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Quản lý người dùng</h3>
            <p className="text-gray-600">Tính năng quản lý người dùng đang được phát triển...</p>
            <div className="mt-4 grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-lg">
                <p className="text-2xl font-bold text-gray-900">{stats?.totalUsers || 0}</p>
                <p className="text-sm text-gray-500">Tổng người dùng</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-lg">
                <p className="text-2xl font-bold text-gray-900">{stats?.activeUsers || 0}</p>
                <p className="text-sm text-gray-500">Đang hoạt động</p>
              </div>
            </div>
          </div>
        )}

        {/* Documents Tab */}
        {activeTab === 'documents' && (
          <div className="bg-white rounded-xl p-6 border border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Thống kê tài liệu</h3>
            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-blue-50 rounded-lg">
                <p className="text-2xl font-bold text-blue-600">{stats?.totalDocuments || 0}</p>
                <p className="text-sm text-gray-600">Tổng tài liệu</p>
              </div>
              <div className="p-4 bg-green-50 rounded-lg">
                <p className="text-2xl font-bold text-green-600">{stats?.indexedDocuments || 0}</p>
                <p className="text-sm text-gray-600">Đã lập chỉ mục</p>
              </div>
              <div className="p-4 bg-purple-50 rounded-lg">
                <p className="text-2xl font-bold text-purple-600">{stats?.totalChunks || 0}</p>
                <p className="text-sm text-gray-600">Tổng chunks</p>
              </div>
              <div className="p-4 bg-orange-50 rounded-lg">
                <p className="text-2xl font-bold text-orange-600">{(stats?.storageUsedGB || 0).toFixed(2)} GB</p>
                <p className="text-sm text-gray-600">Dung lượng lưu trữ</p>
              </div>
            </div>
          </div>
        )}

        {/* System Tab */}
        {activeTab === 'system' && systemHealth && (
          <div className="space-y-6">
            {/* System Health Status */}
            <div className={`rounded-xl p-6 border ${
              systemHealth.isHealthy ? 'bg-green-50 border-green-200' : 'bg-red-50 border-red-200'
            }`}>
              <div className="flex items-center gap-4">
                <div className={`w-12 h-12 rounded-full flex items-center justify-center ${
                  systemHealth.isHealthy ? 'bg-green-500' : 'bg-red-500'
                }`}>
                  <span className="text-2xl text-white">{systemHealth.isHealthy ? '✓' : '✕'}</span>
                </div>
                <div>
                  <h3 className={`text-xl font-bold ${
                    systemHealth.isHealthy ? 'text-green-800' : 'text-red-800'
                  }`}>
                    Hệ thống {systemHealth.isHealthy ? 'hoạt động tốt' : 'có vấn đề'}
                  </h3>
                  <p className="text-sm text-gray-600">
                    Kiểm tra lúc: {new Date(systemHealth.checkedAt).toLocaleString('vi-VN')}
                  </p>
                </div>
              </div>
            </div>

            {/* Components Status */}
            <div className="bg-white rounded-xl p-6 border border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Trạng thái thành phần</h3>
              <div className="space-y-3">
                {Object.entries(systemHealth.components).map(([key, component]) => (
                  <div key={key} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                    <div className="flex items-center gap-3">
                      <div className={`w-3 h-3 rounded-full ${
                        component.isHealthy ? 'bg-green-500' : 'bg-red-500'
                      }`}></div>
                      <span className="font-medium text-gray-900">{component.name}</span>
                    </div>
                    <span className="text-sm text-gray-600">{component.message}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}
