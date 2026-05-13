'use client';

import { useState, useEffect } from 'react';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { apiClient } from '@/lib/api';

interface SearchResult {
  documentId: string;
  title: string;
  excerpt: string;
  score: number;
  pageNumber: number | null;
  source: string | null;
}

export default function SearchPage() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchType, setSearchType] = useState<'semantic' | 'hybrid'>('semantic');
  const [searchTime, setSearchTime] = useState(0);
  const [showFilters, setShowFilters] = useState(false);
  const [topK, setTopK] = useState(10);

  useEffect(() => {
    const debounceTimer = setTimeout(() => {
      if (query.trim().length >= 2) {
        handleSearch();
      } else {
        setResults([]);
      }
    }, 500);

    return () => clearTimeout(debounceTimer);
  }, [query, searchType, topK]);

  const handleSearch = async () => {
    if (!query.trim()) return;

    setLoading(true);
    const startTime = Date.now();

    try {
      const endpoint = searchType === 'semantic' ? '/api/v1/search' : '/api/v1/search/hybrid';
      const response = await apiClient.post<{ results: SearchResult[]; time: number }>(endpoint, {
        query,
        topK,
      });
      setResults(response.data.results || []);
      setSearchTime(Date.now() - startTime);
    } catch (err) {
      console.error('Search failed:', err);
      setResults([]);
    } finally {
      setLoading(false);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  const getScoreColor = (score: number) => {
    if (score >= 0.8) return 'text-green-600';
    if (score >= 0.6) return 'text-blue-600';
    if (score >= 0.4) return 'text-yellow-600';
    return 'text-gray-600';
  };

  const highlightText = (text: string, query: string) => {
    if (!query) return text;
    const parts = text.split(new RegExp(`(${query})`, 'gi'));
    return parts.map((part, i) =>
      part.toLowerCase() === query.toLowerCase() ? (
        <mark key={i} className="bg-yellow-200 px-0.5 rounded">{part}</mark>
      ) : (
        part
      )
    );
  };

  return (
    <DashboardLayout>
      <div className="max-w-5xl mx-auto space-y-6">
        {/* Header */}
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900">Tìm kiếm Ngữ nghĩa</h1>
          <p className="mt-2 text-gray-600">
            Tìm kiếm thông tin trong tài liệu với độ chính xác cao
          </p>
        </div>

        {/* Search Input */}
        <div className="relative">
          <div className="relative flex items-center">
            <span className="absolute left-4 text-gray-400 text-xl">🔍</span>
            <input
              type="text"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              onKeyPress={handleKeyPress}
              placeholder="Nhập câu hỏi hoặc từ khóa để tìm kiếm..."
              className="w-full pl-12 pr-32 py-4 text-lg border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 shadow-sm"
            />
            <button
              onClick={handleSearch}
              disabled={loading || !query.trim()}
              className="absolute right-2 px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {loading ? 'Đang tìm...' : 'Tìm kiếm'}
            </button>
          </div>

          {/* Search Options */}
          <div className="mt-4 flex items-center justify-between">
            <div className="flex gap-4">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="radio"
                  name="searchType"
                  value="semantic"
                  checked={searchType === 'semantic'}
                  onChange={() => setSearchType('semantic')}
                  className="text-blue-600"
                />
                <span className="text-sm text-gray-700">Tìm kiếm ngữ nghĩa</span>
              </label>
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="radio"
                  name="searchType"
                  value="hybrid"
                  checked={searchType === 'hybrid'}
                  onChange={() => setSearchType('hybrid')}
                  className="text-blue-600"
                />
                <span className="text-sm text-gray-700">Tìm kiếm kết hợp</span>
              </label>
            </div>

            <button
              onClick={() => setShowFilters(!showFilters)}
              className="text-sm text-blue-600 hover:text-blue-700 flex items-center gap-1"
            >
              <span>⚙️</span> Tùy chọn
            </button>
          </div>

          {/* Filters */}
          {showFilters && (
            <div className="mt-4 p-4 bg-gray-50 rounded-lg border border-gray-200">
              <div className="flex items-center gap-6">
                <div>
                  <label className="text-sm font-medium text-gray-700">Số kết quả:</label>
                  <select
                    value={topK}
                    onChange={(e) => setTopK(Number(e.target.value))}
                    className="ml-2 px-3 py-1 border rounded-lg"
                  >
                    <option value={5}>5 kết quả</option>
                    <option value={10}>10 kết quả</option>
                    <option value={20}>20 kết quả</option>
                    <option value={50}>50 kết quả</option>
                  </select>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Results Info */}
        {(results.length > 0 || loading) && (
          <div className="flex items-center justify-between text-sm text-gray-500">
            <span>
              {loading ? 'Đang tìm kiếm...' : `${results.length} kết quả`}
            </span>
            {!loading && searchTime > 0 && (
              <span>Thời gian: {searchTime}ms</span>
            )}
          </div>
        )}

        {/* Loading State */}
        {loading && (
          <div className="space-y-4">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="animate-pulse">
                <div className="bg-gray-200 h-6 rounded w-1/3 mb-2"></div>
                <div className="bg-gray-200 h-4 rounded w-full mb-2"></div>
                <div className="bg-gray-200 h-4 rounded w-2/3"></div>
              </div>
            ))}
          </div>
        )}

        {/* Results */}
        {!loading && results.length > 0 && (
          <div className="space-y-4">
            {results.map((result, index) => (
              <div
                key={index}
                className="bg-white border border-gray-200 rounded-xl p-6 hover:shadow-md transition-shadow"
              >
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    <h3 className="text-lg font-semibold text-gray-900">
                      {highlightText(result.title, query)}
                    </h3>
                    {result.pageNumber && (
                      <span className="text-xs text-gray-500">Trang {result.pageNumber}</span>
                    )}
                  </div>
                  <div className={`text-sm font-medium ${getScoreColor(result.score)}`}>
                    {Math.round(result.score * 100)}% phù hợp
                  </div>
                </div>
                <p className="mt-3 text-gray-600 leading-relaxed">
                  {highlightText(result.excerpt, query)}
                </p>
                <div className="mt-4 flex gap-2">
                  <button className="px-4 py-2 text-sm bg-blue-50 text-blue-600 rounded-lg hover:bg-blue-100 transition-colors">
                    Xem chi tiết
                  </button>
                  <button className="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors">
                    Tìm trong tài liệu
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Empty State */}
        {!loading && query.trim().length >= 2 && results.length === 0 && (
          <div className="text-center py-16 bg-gray-50 rounded-xl">
            <span className="text-6xl">🔍</span>
            <h3 className="mt-4 text-lg font-medium text-gray-900">Không tìm thấy kết quả</h3>
            <p className="mt-2 text-gray-500">
              Thử thay đổi từ khóa hoặc tăng số kết quả tìm kiếm
            </p>
          </div>
        )}

        {/* Initial State */}
        {!loading && query.trim().length < 2 && (
          <div className="text-center py-16">
            <span className="text-6xl">💡</span>
            <h3 className="mt-4 text-lg font-medium text-gray-900">Bắt đầu tìm kiếm</h3>
            <p className="mt-2 text-gray-500 max-w-md mx-auto">
              Nhập ít nhất 2 ký tự để bắt đầu tìm kiếm trong tài liệu của bạn
            </p>
            <div className="mt-6 flex flex-wrap justify-center gap-2">
              {['Hướng dẫn sử dụng', 'Chính sách', 'Báo cáo', 'Hợp đồng'].map((suggestion) => (
                <button
                  key={suggestion}
                  onClick={() => setQuery(suggestion)}
                  className="px-4 py-2 text-sm bg-gray-100 text-gray-700 rounded-full hover:bg-gray-200 transition-colors"
                >
                  {suggestion}
                </button>
              ))}
            </div>
          </div>
        )}

        {/* Search Tips */}
        <div className="mt-8 p-6 bg-blue-50 border border-blue-200 rounded-xl">
          <h3 className="font-semibold text-blue-800 mb-3 flex items-center gap-2">
            <span>💡</span> Mẹo tìm kiếm
          </h3>
          <ul className="space-y-2 text-blue-700 text-sm">
            <li>• Sử dụng câu hỏi tự nhiên thay vì từ khóa đơn lẻ</li>
            <li>• Tìm kiếm ngữ nghĩa hiểu được ý định của bạn</li>
            <li>• Tìm kiếm kết hợp cho kết quả chính xác hơn</li>
            <li>• Tăng số kết quả nếu không tìm thấy thông tin cần thiết</li>
          </ul>
        </div>
      </div>
    </DashboardLayout>
  );
}
