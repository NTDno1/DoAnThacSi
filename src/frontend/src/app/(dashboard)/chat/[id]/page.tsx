'use client';

import { useState, useEffect, useRef } from 'react';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { apiClient } from '@/lib/api';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';

interface Message {
  id: string;
  role: string;
  content: string;
  citations?: string;
  createdAt: string;
}

interface Source {
  documentId: string;
  title: string;
  pageNumber: number;
  excerpt: string;
  score: number;
}

export default function ChatPage() {
  const params = useParams();
  const router = useRouter();
  const sessionId = params.id as string;
  
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showSources, setShowSources] = useState<Source[]>([]);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (sessionId) {
      loadMessages();
    }
  }, [sessionId]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const loadMessages = async () => {
    try {
      const response = await apiClient.get<Message[]>(`/api/v1/chat/sessions/${sessionId}/messages`);
      setMessages(response.data);
    } catch (err) {
      console.error('Failed to load messages:', err);
    }
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!input.trim() || loading) return;

    const userMessage = input.trim();
    setInput('');
    setLoading(true);
    setError(null);
    setShowSources([]);

    // Add user message immediately
    setMessages(prev => [...prev, {
      id: Date.now().toString(),
      role: 'user',
      content: userMessage,
      createdAt: new Date().toISOString()
    }]);

    try {
      const response = await apiClient.post<{
        content: string;
        sources: Source[];
        confidence: number;
        latencyMs: number;
      }>(`/api/v1/chat/sessions/${sessionId}/messages`, {
        content: userMessage
      });

      // Add assistant response
      setMessages(prev => [...prev, {
        id: Date.now().toString(),
        role: 'assistant',
        content: response.data.content,
        citations: JSON.stringify(response.data.sources),
        createdAt: new Date().toISOString()
      }]);

      setShowSources(response.data.sources);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Gửi tin nhắn thất bại');
      // Remove the user message if the request failed
      setMessages(prev => prev.slice(0, -1));
    } finally {
      setLoading(false);
    }
  };

  const parseCitations = (citationsJson?: string): Source[] => {
    if (!citationsJson) return [];
    try {
      return JSON.parse(citationsJson);
    } catch {
      return [];
    }
  };

  return (
    <DashboardLayout>
      <div className="flex flex-col h-[calc(100vh-4rem)]">
        {/* Chat Header */}
        <div className="flex items-center justify-between px-6 py-4 border-b bg-white">
          <div className="flex items-center gap-4">
            <button
              onClick={() => router.push('/chat/history')}
              className="p-2 text-gray-500 hover:bg-gray-100 rounded-lg transition-colors"
            >
              <span className="text-xl">←</span>
            </button>
            <div>
              <h2 className="font-semibold text-gray-900">Cuộc trò chuyện</h2>
              <p className="text-sm text-gray-500">AI Assistant - RAG Chatbot</p>
            </div>
          </div>
          <Link
            href="/search"
            className="px-4 py-2 text-sm text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
          >
            Tìm kiếm ngữ nghĩa
          </Link>
        </div>

        {/* Messages Area */}
        <div className="flex-1 overflow-y-auto px-6 py-4 space-y-4 bg-gray-50">
          {messages.length === 0 && (
            <div className="text-center py-16">
              <span className="text-6xl">🤖</span>
              <h3 className="mt-4 text-lg font-medium text-gray-900">Chào bạn!</h3>
              <p className="mt-2 text-gray-500 max-w-md mx-auto">
                Tôi có thể trả lời câu hỏi dựa trên tài liệu của bạn. Hãy hỏi tôi bất cứ điều gì!
              </p>
              <div className="mt-6 flex flex-wrap justify-center gap-2">
                {['Hướng dẫn sử dụng', 'Tóm tắt tài liệu', 'So sánh nội dung'].map((suggestion) => (
                  <button
                    key={suggestion}
                    onClick={() => setInput(suggestion)}
                    className="px-4 py-2 text-sm bg-white border border-gray-200 text-gray-700 rounded-full hover:bg-gray-50 transition-colors"
                  >
                    {suggestion}
                  </button>
                ))}
              </div>
            </div>
          )}

          {messages.map((message) => (
            <div
              key={message.id}
              className={`flex ${message.role === 'user' ? 'justify-end' : 'justify-start'}`}
            >
              <div
                className={`max-w-[70%] rounded-2xl px-4 py-3 ${
                  message.role === 'user'
                    ? 'bg-blue-600 text-white'
                    : 'bg-white border border-gray-200 text-gray-900'
                }`}
              >
                <div className="flex items-start gap-2">
                  {message.role === 'assistant' && (
                    <span className="text-xl">🤖</span>
                  )}
                  <div className="flex-1">
                    <p className="whitespace-pre-wrap leading-relaxed">{message.content}</p>
                  </div>
                  {message.role === 'user' && (
                    <span className="text-lg">👤</span>
                  )}
                </div>
                
                {/* Sources for assistant messages */}
                {message.role === 'assistant' && parseCitations(message.citations).length > 0 && (
                  <div className="mt-4 pt-3 border-t border-gray-200">
                    <div className="flex items-center justify-between mb-2">
                      <span className="text-sm font-medium text-gray-700">Nguồn trích dẫn:</span>
                      <button
                        onClick={() => setShowSources(prev => prev.length > 0 ? [] : parseCitations(message.citations))}
                        className="text-xs text-blue-600 hover:underline"
                      >
                        {showSources.length > 0 ? 'Ẩn' : 'Hiện'}
                      </button>
                    </div>
                    {showSources.length > 0 && parseCitations(message.citations).length > 0 && (
                      <div className="space-y-2">
                        {parseCitations(message.citations).map((source, idx) => (
                          <div key={idx} className="text-sm bg-gray-50 rounded-lg p-2">
                            <div className="font-medium text-gray-700">{source.title}</div>
                            <div className="text-gray-500 text-xs mt-1">{source.excerpt}</div>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                )}

                <div className={`text-xs mt-2 ${message.role === 'user' ? 'text-blue-100' : 'text-gray-400'}`}>
                  {new Date(message.createdAt).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}
                </div>
              </div>
            </div>
          ))}

          {loading && (
            <div className="flex justify-start">
              <div className="bg-white border border-gray-200 rounded-2xl px-4 py-3">
                <div className="flex items-center gap-2">
                  <span className="text-xl">🤖</span>
                  <div className="flex gap-1">
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '0ms' }}></div>
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '150ms' }}></div>
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '300ms' }}></div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {error && (
            <div className="flex justify-center">
              <div className="bg-red-50 border border-red-200 text-red-700 rounded-xl px-4 py-3">
                <span>{error}</span>
              </div>
            </div>
          )}

          <div ref={messagesEndRef} />
        </div>

        {/* Input Area */}
        <div className="px-6 py-4 bg-white border-t">
          <form onSubmit={handleSubmit} className="flex gap-3">
            <input
              type="text"
              value={input}
              onChange={(e) => setInput(e.target.value)}
              placeholder="Nhập câu hỏi của bạn..."
              className="flex-1 px-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              disabled={loading}
            />
            <button
              type="submit"
              disabled={loading || !input.trim()}
              className="px-6 py-3 bg-blue-600 text-white rounded-xl hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
            >
              {loading ? 'Đang gửi...' : 'Gửi'}
            </button>
          </form>
          <p className="mt-2 text-xs text-gray-500 text-center">
            AI có thể tạo ra thông tin không chính xác. Hãy kiểm tra nguồn trích dẫn.
          </p>
        </div>
      </div>
    </DashboardLayout>
  );
}
