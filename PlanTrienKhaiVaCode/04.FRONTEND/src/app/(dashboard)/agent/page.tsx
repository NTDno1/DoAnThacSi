'use client';

import React from 'react';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { Header } from '@/components/layout/DashboardLayout';
import { AgenticChat } from '@/components/chat/AgenticChat';
import { Bot, Zap, Shield, Globe, Database, MessageSquare } from 'lucide-react';

export default function AgentPage() {
  return (
    <DashboardLayout>
      <Header
        title="AI Agent"
        subtitle="Tự động hóa tác vụ với AI Agent thông minh"
      />

      <div className="flex h-[calc(100vh-4rem)]">
        {/* Agent Chat */}
        <div className="flex-1">
          <AgenticChat />
        </div>

        {/* Agent Info Panel */}
        <div className="hidden xl:block w-80 bg-white border-l border-gray-200 p-6 overflow-y-auto">
          <div className="space-y-6">
            <div>
              <h3 className="text-lg font-semibold text-gray-900 mb-4">AI Agent có thể làm gì?</h3>
              <div className="space-y-3">
                {[
                  { icon: <MessageSquare className="w-5 h-5" />, title: 'Trả lời câu hỏi', desc: 'Tìm kiếm và tổng hợp thông tin' },
                  { icon: <Database className="w-5 h-5" />, title: 'Truy vấn dữ liệu', desc: 'Tìm kiếm tài liệu theo ngữ cảnh' },
                  { icon: <Globe className="w-5 h-5" />, title: 'Hỗ trợ đa ngôn ngữ', desc: 'Tiếng Việt, Anh, và nhiều hơn' },
                  { icon: <Zap className="w-5 h-5" />, title: 'Tự động hóa', desc: 'Thực hiện tác vụ theo yêu cầu' },
                  { icon: <Shield className="w-5 h-5" />, title: 'Bảo mật', desc: 'Dữ liệu được bảo vệ an toàn' },
                ].map((item, i) => (
                  <div key={i} className="flex gap-3">
                    <div className="w-10 h-10 rounded-lg bg-violet-100 text-violet-600 flex items-center justify-center flex-shrink-0">
                      {item.icon}
                    </div>
                    <div>
                      <p className="font-medium text-gray-900">{item.title}</p>
                      <p className="text-sm text-gray-500">{item.desc}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            <div className="pt-6 border-t border-gray-200">
              <h4 className="font-medium text-gray-900 mb-3">Ví dụ câu hỏi</h4>
              <div className="space-y-2">
                {[
                  'Tìm tài liệu về quy trình phê duyệt',
                  'Tạo báo cáo tổng hợp tháng này',
                  'Cập nhật trạng thái hợp đồng',
                  'Liệt kê các công việc cần duyệt',
                ].map((example, i) => (
                  <button
                    key={i}
                    className="w-full text-left px-3 py-2 text-sm bg-gray-50 hover:bg-gray-100 rounded-lg text-gray-700 transition"
                    onClick={() => {
                      // Copy to chat
                    }}
                  >
                    {example}
                  </button>
                ))}
              </div>
            </div>

            <div className="pt-6 border-t border-gray-200">
              <div className="bg-gradient-to-br from-violet-500 to-purple-600 rounded-xl p-4 text-white">
                <div className="flex items-center gap-2 mb-2">
                  <Bot className="w-5 h-5" />
                  <span className="font-medium">Pro Tip</span>
                </div>
                <p className="text-sm text-violet-100">
                  Càng mô tả chi tiết yêu cầu, AI Agent càng đưa ra câu trả lời chính xác hơn.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
