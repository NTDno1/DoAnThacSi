'use client';

import React, { useState, useRef, useEffect, useCallback } from 'react';
import { useAuthStore } from '@/stores/authStore';
import { Send, Bot, User, Loader2, Mic, Volume2, Settings, Plus, Trash2, ChevronDown, ChevronUp } from 'lucide-react';
import { cn } from '@/lib/utils';

// ============================================================
// TYPES
// ============================================================

interface Message {
  id: string;
  role: 'user' | 'assistant' | 'system';
  content: string;
  timestamp: Date;
  citations?: Citation[];
  intent?: string;
  actions?: AIAction[];
}

interface Citation {
  id: string;
  title: string;
  content: string;
  score: number;
}

interface AIAction {
  id: string;
  type: string;
  toolName: string;
  status: 'pending' | 'in_progress' | 'completed' | 'failed';
  result?: string;
}

interface AgentState {
  sessionId?: string;
  mode: 'auto' | 'confirm' | 'plan_only';
  pendingApprovals: AIAction[];
}

// ============================================================
// AGENTIC CHAT COMPONENT
// ============================================================

export function AgenticChat() {
  const { user } = useAuthStore();
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isStreaming, setIsStreaming] = useState(false);
  const [agentState, setAgentState] = useState<AgentState>({
    mode: 'auto',
    pendingApprovals: []
  });
  const [showSettings, setShowSettings] = useState(false);
  const [streamedContent, setStreamedContent] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLTextAreaElement>(null);

  const scrollToBottom = useCallback(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, []);

  useEffect(() => {
    scrollToBottom();
  }, [messages, streamedContent, scrollToBottom]);

  // Auto-scroll when streaming
  useEffect(() => {
    if (isStreaming) {
      scrollToBottom();
    }
  }, [streamedContent, isStreaming, scrollToBottom]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!input.trim() || isLoading) return;

    const userMessage: Message = {
      id: crypto.randomUUID(),
      role: 'user',
      content: input.trim(),
      timestamp: new Date()
    };

    setMessages(prev => [...prev, userMessage]);
    setInput('');
    setIsLoading(true);
    setIsStreaming(true);
    setStreamedContent('');

    try {
      const response = await fetch('/api/v1/agent/process/stream', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
        },
        body: JSON.stringify({
          input: userMessage.content,
          sessionId: agentState.sessionId,
          mode: agentState.mode,
          streamResponse: true
        })
      });

      const reader = response.body?.getReader();
      const decoder = new TextDecoder();

      if (!reader) throw new Error('No reader available');

      let fullContent = '';

      while (true) {
        const { done, value } = await reader.read();
        if (done) break;

        const chunk = decoder.decode(value);
        const lines = chunk.split('\n');

        for (const line of lines) {
          if (line.startsWith('data: ')) {
            const data = line.slice(6);
            
            if (data.startsWith('Intent:')) {
              // Intent classification result
              const intent = data.replace('Intent: ', '').trim();
              setMessages(prev => prev.map((m, i) => 
                i === prev.length - 1 ? { ...m, intent } : m
              ));
            } else if (data.startsWith('[Action]')) {
              // Action status
              // Handle action updates
            } else if (data.startsWith('ERROR:')) {
              // Error occurred
              fullContent += '\n' + data.replace('ERROR: ', '');
            } else {
              // Regular content
              fullContent += data;
              setStreamedContent(fullContent);
            }
          }
        }
      }

      // Add assistant message
      const assistantMessage: Message = {
        id: crypto.randomUUID(),
        role: 'assistant',
        content: fullContent,
        timestamp: new Date()
      };

      setMessages(prev => [...prev.slice(0, -1), assistantMessage]);
      setStreamedContent('');

    } catch (error) {
      console.error('Chat error:', error);
      
      const errorMessage: Message = {
        id: crypto.randomUUID(),
        role: 'assistant',
        content: 'Xin lỗi, đã có lỗi xảy ra. Vui lòng thử lại.',
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev.slice(0, -1), errorMessage]);
      setStreamedContent('');
    } finally {
      setIsLoading(false);
      setIsStreaming(false);
    }
  };

  const handleVoiceInput = async () => {
    // Voice input implementation
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      // Audio recording and transcription would go here
      stream.getTracks().forEach(track => track.stop());
    } catch (error) {
      console.error('Voice input error:', error);
    }
  };

  const handleApproveAction = async (actionId: string, approved: boolean) => {
    try {
      await fetch(`/api/v1/agent/sessions/${agentState.sessionId}/actions/${actionId}/approve`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
        },
        body: JSON.stringify({ approved })
      });

      setAgentState(prev => ({
        ...prev,
        pendingApprovals: prev.pendingApprovals.filter(a => a.id !== actionId)
      }));
    } catch (error) {
      console.error('Approval error:', error);
    }
  };

  const clearChat = () => {
    setMessages([]);
    setAgentState(prev => ({ ...prev, sessionId: undefined }));
  };

  return (
    <div className="flex flex-col h-full bg-gradient-to-b from-gray-900 to-gray-800">
      {/* Header */}
      <div className="flex items-center justify-between px-4 py-3 border-b border-gray-700 bg-gray-900/50">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-gradient-to-br from-violet-500 to-purple-600 flex items-center justify-center">
            <Bot className="w-6 h-6 text-white" />
          </div>
          <div>
            <h2 className="font-semibold text-white">AI Agent</h2>
            <p className="text-xs text-gray-400">
              {agentState.mode === 'auto' ? 'Chế độ tự động' : 
               agentState.mode === 'confirm' ? 'Chế độ xác nhận' : 'Chế độ lập kế hoạch'}
            </p>
          </div>
        </div>
        
        <div className="flex items-center gap-2">
          <button
            onClick={() => setShowSettings(!showSettings)}
            className="p-2 rounded-lg hover:bg-gray-700 text-gray-400 hover:text-white transition"
          >
            <Settings className="w-5 h-5" />
          </button>
          <button
            onClick={clearChat}
            className="p-2 rounded-lg hover:bg-gray-700 text-gray-400 hover:text-white transition"
          >
            <Plus className="w-5 h-5" />
          </button>
        </div>
      </div>

      {/* Settings Panel */}
      {showSettings && (
        <div className="px-4 py-3 bg-gray-800 border-b border-gray-700">
          <div className="flex gap-2">
            {(['auto', 'confirm', 'plan_only'] as const).map((mode) => (
              <button
                key={mode}
                onClick={() => setAgentState(prev => ({ ...prev, mode }))}
                className={cn(
                  'px-3 py-1.5 rounded-lg text-sm font-medium transition',
                  agentState.mode === mode
                    ? 'bg-violet-600 text-white'
                    : 'bg-gray-700 text-gray-300 hover:bg-gray-600'
                )}
              >
                {mode === 'auto' ? 'Tự động' : mode === 'confirm' ? 'Xác nhận' : 'Lập kế hoạch'}
              </button>
            ))}
          </div>
        </div>
      )}

      {/* Pending Approvals */}
      {agentState.pendingApprovals.length > 0 && (
        <div className="px-4 py-3 bg-amber-900/30 border-b border-amber-700/50">
          <p className="text-sm font-medium text-amber-400 mb-2">
            Cần xác nhận ({agentState.pendingApprovals.length})
          </p>
          {agentState.pendingApprovals.map((action) => (
            <div key={action.id} className="flex items-center justify-between py-2">
              <span className="text-sm text-gray-300">{action.type}</span>
              <div className="flex gap-2">
                <button
                  onClick={() => handleApproveAction(action.id, true)}
                  className="px-3 py-1 bg-green-600 text-white rounded text-sm hover:bg-green-500"
                >
                  Đồng ý
                </button>
                <button
                  onClick={() => handleApproveAction(action.id, false)}
                  className="px-3 py-1 bg-red-600 text-white rounded text-sm hover:bg-red-500"
                >
                  Từ chối
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Messages */}
      <div className="flex-1 overflow-y-auto px-4 py-4 space-y-4">
        {messages.length === 0 && (
          <div className="flex flex-col items-center justify-center h-full text-center">
            <div className="w-16 h-16 rounded-full bg-gradient-to-br from-violet-500 to-purple-600 flex items-center justify-center mb-4">
              <Bot className="w-10 h-10 text-white" />
            </div>
            <h3 className="text-xl font-semibold text-white mb-2">
              Chào bạn, {user?.fullName || 'User'}!
            </h3>
            <p className="text-gray-400 max-w-md">
              Tôi là AI Agent có thể giúp bạn tìm kiếm tài liệu, trả lời câu hỏi, 
              và thực hiện các tác vụ tự động.
            </p>
            <div className="mt-6 grid grid-cols-2 gap-3 max-w-lg">
              {[
                { icon: '📄', text: 'Tìm kiếm tài liệu' },
                { icon: '💬', text: 'Trả lời câu hỏi' },
                { icon: '📊', text: 'Phân tích dữ liệu' },
                { icon: '🔧', text: 'Hỗ trợ tác vụ' }
              ].map((item, i) => (
                <button
                  key={i}
                  onClick={() => setInput(item.text)}
                  className="flex items-center gap-2 px-4 py-2 bg-gray-800 hover:bg-gray-700 rounded-lg text-left transition"
                >
                  <span className="text-xl">{item.icon}</span>
                  <span className="text-sm text-gray-300">{item.text}</span>
                </button>
              ))}
            </div>
          </div>
        )}

        {messages.map((message) => (
          <MessageBubble key={message.id} message={message} />
        ))}

        {/* Streaming indicator */}
        {isStreaming && streamedContent && (
          <div className="flex gap-3">
            <div className="w-8 h-8 rounded-full bg-gradient-to-br from-violet-500 to-purple-600 flex items-center justify-center flex-shrink-0">
              <Bot className="w-5 h-5 text-white" />
            </div>
            <div className="flex-1">
              <div className="bg-gray-800 rounded-2xl rounded-tl-none px-4 py-3 text-gray-100">
                <p className="whitespace-pre-wrap">{streamedContent}</p>
                <span className="inline-block w-2 h-4 bg-violet-500 animate-pulse ml-1" />
              </div>
            </div>
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>

      {/* Input */}
      <div className="px-4 py-4 border-t border-gray-700 bg-gray-900/50">
        <form onSubmit={handleSubmit} className="flex items-end gap-3">
          <div className="flex-1 relative">
            <textarea
              ref={inputRef}
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                  e.preventDefault();
                  handleSubmit(e);
                }
              }}
              placeholder="Nhập câu hỏi hoặc yêu cầu..."
              className="w-full bg-gray-800 text-white rounded-xl px-4 py-3 pr-12 resize-none focus:outline-none focus:ring-2 focus:ring-violet-500 placeholder-gray-500"
              rows={1}
              disabled={isLoading}
            />
          </div>
          
          <button
            type="submit"
            disabled={!input.trim() || isLoading}
            className={cn(
              'p-3 rounded-xl transition',
              input.trim() && !isLoading
                ? 'bg-gradient-to-r from-violet-600 to-purple-600 text-white hover:from-violet-500 hover:to-purple-500'
                : 'bg-gray-700 text-gray-500 cursor-not-allowed'
            )}
          >
            {isLoading ? (
              <Loader2 className="w-5 h-5 animate-spin" />
            ) : (
              <Send className="w-5 h-5" />
            )}
          </button>
          
          <button
            type="button"
            onClick={handleVoiceInput}
            className="p-3 rounded-xl bg-gray-800 text-gray-400 hover:text-white hover:bg-gray-700 transition"
          >
            <Mic className="w-5 h-5" />
          </button>
        </form>
        
        <p className="text-xs text-gray-500 mt-2 text-center">
          AI Agent có thể sai. Hãy kiểm tra thông tin quan trọng.
        </p>
      </div>
    </div>
  );
}

// ============================================================
// MESSAGE BUBBLE COMPONENT
// ============================================================

function MessageBubble({ message }: { message: Message }) {
  const [showCitations, setShowCitations] = useState(false);
  const isUser = message.role === 'user';

  return (
    <div className={cn('flex gap-3', isUser && 'flex-row-reverse')}>
      <div className={cn(
        'w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0',
        isUser ? 'bg-violet-600' : 'bg-gradient-to-br from-violet-500 to-purple-600'
      )}>
        {isUser ? (
          <User className="w-5 h-5 text-white" />
        ) : (
          <Bot className="w-5 h-5 text-white" />
        )}
      </div>
      
      <div className={cn('flex-1 max-w-[80%]', isUser && 'flex flex-col items-end')}>
        <div className={cn(
          'rounded-2xl px-4 py-3',
          isUser 
            ? 'bg-gradient-to-r from-violet-600 to-purple-600 text-white rounded-tr-none'
            : 'bg-gray-800 text-gray-100 rounded-tl-none'
        )}>
          {message.intent && (
            <div className="text-xs text-violet-400 mb-2 flex items-center gap-1">
              <span className="px-2 py-0.5 bg-violet-900/50 rounded-full">
                Intent: {message.intent}
              </span>
            </div>
          )}
          
          <p className="whitespace-pre-wrap">{message.content}</p>
        </div>
        
        <span className="text-xs text-gray-500 mt-1">
          {message.timestamp.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}
        </span>
        
        {/* Citations */}
        {message.citations && message.citations.length > 0 && (
          <div className="mt-2">
            <button
              onClick={() => setShowCitations(!showCitations)}
              className="text-xs text-violet-400 flex items-center gap-1 hover:text-violet-300"
            >
              {showCitations ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
              {message.citations.length} nguồn trích dẫn
            </button>
            
            {showCitations && (
              <div className="mt-2 space-y-2">
                {message.citations.map((citation) => (
                  <div key={citation.id} className="bg-gray-800/50 rounded-lg p-3 text-sm">
                    <p className="font-medium text-gray-200">{citation.title}</p>
                    <p className="text-gray-400 text-xs mt-1 line-clamp-2">{citation.content}</p>
                    <p className="text-xs text-gray-500 mt-1">
                      Độ chính xác: {(citation.score * 100).toFixed(0)}%
                    </p>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}

export default AgenticChat;
