'use client';

import React, { useState } from 'react';
import Link from 'next/link';
import { DashboardLayout, Header } from '@/components/layout/DashboardLayout';
import { AgenticChat } from '@/components/chat/AgenticChat';
import { 
  Search, Bot, FileText, Database, BarChart3, Activity,
  TrendingUp, Users, Clock, ArrowRight, Zap, Shield, Sparkles
} from 'lucide-react';

export default function DashboardPage() {
  const stats = [
    { label: 'Tổng truy vấn', value: '12,847', change: '+12%', icon: Activity, color: 'violet' },
    { label: 'Người dùng hoạt động', value: '1,234', change: '+8%', icon: Users, color: 'blue' },
    { label: 'Tài liệu được index', value: '5,678', change: '+15%', icon: FileText, color: 'green' },
    { label: 'Thời gian phản hồi TB', value: '1.2s', change: '-25%', icon: Clock, color: 'orange' },
  ];

  const features = [
    {
      title: 'Tìm kiếm Semantic',
      description: 'Tìm kiếm thông minh với AI hiểu ngữ cảnh',
      icon: <Search className="w-6 h-6" />,
      href: '/search',
      color: 'bg-violet-500'
    },
    {
      title: 'AI Agent',
      description: 'Tự động hóa tác vụ với AI Agent thông minh',
      icon: <Bot className="w-6 h-6" />,
      href: '/agent',
      color: 'bg-blue-500'
    },
    {
      title: 'Phân tích dữ liệu',
      description: 'Truy vấn cơ sở dữ liệu bằng ngôn ngữ tự nhiên',
      icon: <Database className="w-6 h-6" />,
      href: '/database',
      color: 'bg-green-500'
    },
    {
      title: 'Dashboard',
      description: 'Theo dõi và phân tích hoạt động AI',
      icon: <BarChart3 className="w-6 h-6" />,
      href: '/analytics',
      color: 'bg-orange-500'
    },
  ];

  const recentQueries = [
    { query: 'Tìm tài liệu về quy trình phê duyệt', time: '2 phút trước', user: 'Nguyễn Văn A' },
    { query: 'Tổng hợp báo cáo tháng 3', time: '5 phút trước', user: 'Trần Thị B' },
    { query: 'Cập nhật trạng thái hợp đồng', time: '10 phút trước', user: 'Lê Văn C' },
    { query: 'Tạo công văn mới cho phòng HC', time: '15 phút trước', user: 'Phạm Thị D' },
  ];

  return (
    <DashboardLayout>
      <Header
        title="Dashboard"
        subtitle="Tổng quan về Enterprise AI Platform"
      />

      <div className="p-6 space-y-6">
        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {stats.map((stat) => (
            <div key={stat.label} className="bg-white rounded-xl p-6 border border-gray-200">
              <div className="flex items-center justify-between">
                <div className={`p-2 rounded-lg ${stat.color === 'violet' ? 'bg-violet-100 text-violet-600' : 
                  stat.color === 'blue' ? 'bg-blue-100 text-blue-600' :
                  stat.color === 'green' ? 'bg-green-100 text-green-600' :
                  'bg-orange-100 text-orange-600'}`}>
                  <stat.icon className="w-5 h-5" />
                </div>
                <span className={`text-sm font-medium ${
                  stat.change.startsWith('+') ? 'text-green-600' : 'text-orange-600'
                }`}>
                  {stat.change}
                </span>
              </div>
              <p className="mt-4 text-3xl font-bold text-gray-900">{stat.value}</p>
              <p className="text-sm text-gray-500">{stat.label}</p>
            </div>
          ))}
        </div>

        {/* Features */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {features.map((feature) => (
            <Link
              key={feature.title}
              href={feature.href}
              className="group bg-white rounded-xl p-6 border border-gray-200 hover:border-violet-300 hover:shadow-lg transition-all"
            >
              <div className={`w-12 h-12 rounded-xl ${feature.color} flex items-center justify-center text-white mb-4`}>
                {feature.icon}
              </div>
              <h3 className="text-lg font-semibold text-gray-900 group-hover:text-violet-600 transition">
                {feature.title}
              </h3>
              <p className="mt-2 text-sm text-gray-500">
                {feature.description}
              </p>
              <div className="mt-4 flex items-center text-sm font-medium text-violet-600 group-hover:gap-2 transition-all">
                Truy cập <ArrowRight className="w-4 h-4 ml-1 group-hover:translate-x-1 transition-transform" />
              </div>
            </Link>
          ))}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Recent Queries */}
          <div className="lg:col-span-2 bg-white rounded-xl border border-gray-200">
            <div className="p-6 border-b border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900">Truy vấn gần đây</h3>
            </div>
            <div className="divide-y divide-gray-100">
              {recentQueries.map((item, i) => (
                <div key={i} className="p-4 hover:bg-gray-50 transition">
                  <div className="flex items-start justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-900">{item.query}</p>
                      <p className="text-xs text-gray-500 mt-1">
                        {item.user} · {item.time}
                      </p>
                    </div>
                    <button className="text-violet-600 hover:text-violet-700">
                      <ArrowRight className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* AI Capabilities */}
          <div className="bg-gradient-to-br from-violet-600 to-purple-700 rounded-xl p-6 text-white">
            <div className="flex items-center gap-2 mb-6">
              <Sparkles className="w-6 h-6" />
              <h3 className="text-lg font-semibold">Khả năng AI</h3>
            </div>
            
            <div className="space-y-4">
              <div className="flex items-center gap-3">
                <div className="w-8 h-8 rounded-lg bg-white/20 flex items-center justify-center">
                  <Zap className="w-4 h-4" />
                </div>
                <div>
                  <p className="font-medium">Offline AI</p>
                  <p className="text-sm text-violet-200">100% Local, không phí API</p>
                </div>
              </div>
              
              <div className="flex items-center gap-3">
                <div className="w-8 h-8 rounded-lg bg-white/20 flex items-center justify-center">
                  <Shield className="w-4 h-4" />
                </div>
                <div>
                  <p className="font-medium">Bảo mật cao</p>
                  <p className="text-sm text-violet-200">Dữ liệu không rời khỏi hệ thống</p>
                </div>
              </div>
              
              <div className="flex items-center gap-3">
                <div className="w-8 h-8 rounded-lg bg-white/20 flex items-center justify-center">
                  <TrendingUp className="w-4 h-4" />
                </div>
                <div>
                  <p className="font-medium">Multi-provider</p>
                  <p className="text-sm text-violet-200">OpenAI, Anthropic, Ollama...</p>
                </div>
              </div>
            </div>

            <div className="mt-6 pt-6 border-t border-white/20">
              <Link
                href="/agent"
                className="inline-flex items-center gap-2 px-4 py-2 bg-white text-violet-600 rounded-lg font-medium hover:bg-violet-50 transition"
              >
                Thử AI Agent <ArrowRight className="w-4 h-4" />
              </Link>
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
