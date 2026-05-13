'use client';

import { useState, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { documentApi, formatFileSize, getFileIcon } from '@/lib/documents';

export default function UploadDocumentPage() {
  const router = useRouter();
  const [file, setFile] = useState<File | null>(null);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [uploadResult, setUploadResult] = useState<{ id: string; title: string; status: string } | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [dragActive, setDragActive] = useState(false);

  const allowedTypes = ['.pdf', '.docx', '.doc', '.txt', '.md', '.xlsx', '.xls'];
  const maxSize = 50 * 1024 * 1024; // 50MB

  const handleDrag = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === 'dragenter' || e.type === 'dragover') {
      setDragActive(true);
    } else if (e.type === 'dragleave') {
      setDragActive(false);
    }
  }, []);

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);
    setError(null);

    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      const droppedFile = e.dataTransfer.files[0];
      validateAndSetFile(droppedFile);
    }
  }, []);

  const validateAndSetFile = (selectedFile: File) => {
    const extension = '.' + selectedFile.name.split('.').pop()?.toLowerCase();
    
    if (!allowedTypes.includes(extension)) {
      setError(`Định dạng file không được hỗ trợ. Chỉ chấp nhận: ${allowedTypes.join(', ')}`);
      return;
    }
    
    if (selectedFile.size > maxSize) {
      setError(`File vượt quá kích thước cho phép (${formatFileSize(maxSize)})`);
      return;
    }

    setFile(selectedFile);
    setError(null);
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    setError(null);
    setUploadResult(null);
    
    if (e.target.files && e.target.files[0]) {
      validateAndSetFile(e.target.files[0]);
    }
  };

  const handleUpload = async () => {
    if (!file) return;

    setIsUploading(true);
    setError(null);
    setUploadProgress(0);

    try {
      // Simulate progress
      const progressInterval = setInterval(() => {
        setUploadProgress(prev => Math.min(prev + 10, 90));
      }, 200);

      const result = await documentApi.upload(file);
      
      clearInterval(progressInterval);
      setUploadProgress(100);
      setUploadResult(result);
      
      setTimeout(() => {
        router.push('/documents');
      }, 1500);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Upload thất bại. Vui lòng thử lại.');
      setUploadProgress(0);
    } finally {
      setIsUploading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'indexed': return 'text-green-600 bg-green-50';
      case 'processing': return 'text-yellow-600 bg-yellow-50';
      case 'failed': return 'text-red-600 bg-red-50';
      default: return 'text-gray-600 bg-gray-50';
    }
  };

  return (
    <DashboardLayout>
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Tải lên Tài liệu</h1>
          <p className="mt-2 text-gray-600">
            Tải lên tài liệu để được phân tích và tạo chỉ mục tìm kiếm ngữ nghĩa
          </p>
        </div>

        {/* Upload Area */}
        <div
          className={`relative border-2 border-dashed rounded-xl p-12 text-center transition-colors ${
            dragActive
              ? 'border-blue-500 bg-blue-50'
              : file
              ? 'border-green-500 bg-green-50'
              : 'border-gray-300 hover:border-blue-400 hover:bg-gray-50'
          }`}
          onDragEnter={handleDrag}
          onDragLeave={handleDrag}
          onDragOver={handleDrag}
          onDrop={handleDrop}
        >
          <input
            type="file"
            className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
            onChange={handleFileSelect}
            accept={allowedTypes.join(',')}
            disabled={isUploading}
          />
          
          {file ? (
            <div className="space-y-4">
              <div className="text-6xl">{getFileIcon(file.type)}</div>
              <div>
                <p className="text-lg font-medium text-gray-900">{file.name}</p>
                <p className="text-gray-500">{formatFileSize(file.size)}</p>
              </div>
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  setFile(null);
                  setError(null);
                }}
                className="text-red-600 hover:text-red-700 text-sm font-medium"
                disabled={isUploading}
              >
                Xóa file
              </button>
            </div>
          ) : (
            <div className="space-y-4">
              <div className="text-6xl">📁</div>
              <div>
                <p className="text-lg font-medium text-gray-900">
                  Kéo thả file hoặc click để chọn
                </p>
                <p className="text-gray-500">
                  Hỗ trợ: PDF, DOCX, DOC, TXT, MD, XLSX, XLS (tối đa 50MB)
                </p>
              </div>
            </div>
          )}
        </div>

        {/* Error Message */}
        {error && (
          <div className="mt-4 p-4 bg-red-50 border border-red-200 rounded-lg">
            <div className="flex items-center gap-2 text-red-700">
              <span className="text-xl">⚠️</span>
              <p>{error}</p>
            </div>
          </div>
        )}

        {/* Upload Progress */}
        {isUploading && (
          <div className="mt-6">
            <div className="flex justify-between text-sm text-gray-600 mb-2">
              <span>Đang tải lên...</span>
              <span>{uploadProgress}%</span>
            </div>
            <div className="w-full bg-gray-200 rounded-full h-2">
              <div
                className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                style={{ width: `${uploadProgress}%` }}
              />
            </div>
          </div>
        )}

        {/* Upload Result */}
        {uploadResult && (
          <div className="mt-6 p-6 bg-green-50 border border-green-200 rounded-xl">
            <div className="flex items-center gap-3">
              <span className="text-3xl">✅</span>
              <div className="flex-1">
                <h3 className="font-semibold text-green-800">Tải lên thành công!</h3>
                <p className="text-green-600">{uploadResult.title}</p>
              </div>
              <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(uploadResult.status)}`}>
                {uploadResult.status === 'Processing' ? 'Đang xử lý...' : uploadResult.status}
              </span>
            </div>
            <p className="mt-3 text-sm text-green-600">
              File đang được xử lý. Bạn sẽ có thể tìm kiếm sau khi hoàn thành.
            </p>
          </div>
        )}

        {/* Upload Button */}
        {file && !uploadResult && (
          <div className="mt-6 flex gap-4">
            <button
              onClick={handleUpload}
              disabled={isUploading}
              className={`flex-1 py-3 px-6 rounded-lg font-medium text-white transition-colors ${
                isUploading
                  ? 'bg-gray-400 cursor-not-allowed'
                  : 'bg-blue-600 hover:bg-blue-700'
              }`}
            >
              {isUploading ? 'Đang tải lên...' : 'Tải lên'}
            </button>
            <button
              onClick={() => router.push('/documents')}
              className="py-3 px-6 rounded-lg font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 transition-colors"
            >
              Hủy
            </button>
          </div>
        )}

        {/* Info Box */}
        <div className="mt-8 p-6 bg-blue-50 border border-blue-200 rounded-xl">
          <h3 className="font-semibold text-blue-800 mb-3 flex items-center gap-2">
            <span>💡</span> Thông tin
          </h3>
          <ul className="space-y-2 text-blue-700 text-sm">
            <li>• File sẽ được phân tích và chia thành các đoạn nhỏ để tạo chỉ mục</li>
            <li>• Thời gian xử lý phụ thuộc vào kích thước file</li>
            <li>• Sau khi xử lý, bạn có thể tìm kiếm ngữ nghĩa trong tài liệu</li>
            <li>• Nội dung tài liệu sẽ được sử dụng để trả lời câu hỏi</li>
          </ul>
        </div>
      </div>
    </DashboardLayout>
  );
}
