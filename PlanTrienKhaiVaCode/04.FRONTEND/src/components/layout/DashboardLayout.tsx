'use client';

import React from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { 
  Home, Search, MessageSquare, Database, Bot, Settings, 
  FileText, Users, Shield, ChevronDown, ChevronRight, LogOut,
  Menu, X, ChevronLeft
} from 'lucide-react';
import { useAuthStore } from '@/stores/authStore';
import { cn } from '@/lib/utils';

// ============================================================
// LAYOUT COMPONENTS
// ============================================================

interface NavItem {
  label: string;
  icon: React.ReactNode;
  href: string;
  badge?: string;
  children?: NavItem[];
}

const mainNavItems: NavItem[] = [
  {
    label: 'Trang chủ',
    icon: <Home className="w-5 h-5" />,
    href: '/dashboard'
  },
  {
    label: 'Tìm kiếm',
    icon: <Search className="w-5 h-5" />,
    href: '/search'
  },
  {
    label: 'AI Chat',
    icon: <MessageSquare className="w-5 h-5" />,
    href: '/chat/history'
  },
  {
    label: 'AI Agent',
    icon: <Bot className="w-5 h-5" />,
    href: '/agent',
    badge: 'NEW'
  },
  {
    label: 'Tài liệu',
    icon: <FileText className="w-5 h-5" />,
    href: '/documents'
  },
  {
    label: 'Cơ sở dữ liệu',
    icon: <Database className="w-5 h-5" />,
    href: '/database'
  },
];

const adminNavItems: NavItem[] = [
  {
    label: 'Người dùng',
    icon: <Users className="w-5 h-5" />,
    href: '/admin/users'
  },
  {
    label: 'Bảo mật',
    icon: <Shield className="w-5 h-5" />,
    href: '/admin/security'
  },
  {
    label: 'Cấu hình',
    icon: <Settings className="w-5 h-5" />,
    href: '/admin/settings'
  },
];

// ============================================================
// SIDEBAR
// ============================================================

interface SidebarProps {
  isCollapsed: boolean;
  onToggle: () => void;
}

export function Sidebar({ isCollapsed, onToggle }: SidebarProps) {
  const pathname = usePathname();
  const { user, logout } = useAuthStore();
  const [expandedItems, setExpandedItems] = React.useState<Set<string>>(new Set());

  const toggleExpand = (label: string) => {
    const newExpanded = new Set(expandedItems);
    if (newExpanded.has(label)) {
      newExpanded.delete(label);
    } else {
      newExpanded.add(label);
    }
    setExpandedItems(newExpanded);
  };

  const isActive = (href: string) => pathname === href || pathname.startsWith(href + '/');

  return (
    <aside
      className={cn(
        'h-screen bg-gray-900 border-r border-gray-800 flex flex-col transition-all duration-300',
        isCollapsed ? 'w-16' : 'w-64'
      )}
    >
      {/* Logo */}
      <div className="h-16 flex items-center px-4 border-b border-gray-800">
        {isCollapsed ? (
          <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-violet-500 to-purple-600" />
        ) : (
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-violet-500 to-purple-600" />
            <span className="font-bold text-white">AI Platform</span>
          </div>
        )}
      </div>

      {/* Navigation */}
      <nav className="flex-1 py-4 overflow-y-auto">
        <div className="px-3 space-y-1">
          {mainNavItems.map((item) => (
            <NavLink
              key={item.href}
              item={item}
              isCollapsed={isCollapsed}
              isActive={isActive(item.href)}
              isExpanded={expandedItems.has(item.label)}
              onToggle={() => toggleExpand(item.label)}
            />
          ))}
        </div>

        {/* Admin Section */}
        {user?.role === 'Admin' && (
          <>
            <div className={cn('px-3 py-2', isCollapsed && 'px-1')}>
              <div className={cn('text-xs font-semibold text-gray-500 uppercase tracking-wider', !isCollapsed && 'px-2')}>
                {!isCollapsed && 'Quản trị'}
              </div>
            </div>
            <div className="px-3 space-y-1">
              {adminNavItems.map((item) => (
                <NavLink
                  key={item.href}
                  item={item}
                  isCollapsed={isCollapsed}
                  isActive={isActive(item.href)}
                  isExpanded={expandedItems.has(item.label)}
                  onToggle={() => toggleExpand(item.label)}
                />
              ))}
            </div>
          </>
        )}
      </nav>

      {/* User */}
      <div className="p-3 border-t border-gray-800">
        {isCollapsed ? (
          <div className="w-8 h-8 rounded-full bg-violet-600 flex items-center justify-center text-white text-sm font-medium">
            {user?.fullName?.charAt(0) || 'U'}
          </div>
        ) : (
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-full bg-violet-600 flex items-center justify-center text-white text-sm font-medium">
              {user?.fullName?.charAt(0) || 'U'}
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-white truncate">
                {user?.fullName || 'User'}
              </p>
              <p className="text-xs text-gray-500 truncate">
                {user?.email}
              </p>
            </div>
            <button
              onClick={logout}
              className="p-2 rounded-lg hover:bg-gray-800 text-gray-400 hover:text-white transition"
              title="Đăng xuất"
            >
              <LogOut className="w-4 h-4" />
            </button>
          </div>
        )}
      </div>

      {/* Collapse Toggle */}
      <button
        onClick={onToggle}
        className="absolute -right-3 top-20 w-6 h-6 rounded-full bg-gray-800 border border-gray-700 flex items-center justify-center text-gray-400 hover:text-white transition"
      >
        {isCollapsed ? (
          <ChevronRight className="w-4 h-4" />
        ) : (
          <ChevronLeft className="w-4 h-4" />
        )}
      </button>
    </aside>
  );
}

// ============================================================
// NAV LINK
// ============================================================

interface NavLinkProps {
  item: NavItem;
  isCollapsed: boolean;
  isActive: boolean;
  isExpanded: boolean;
  onToggle: () => void;
}

function NavLink({ item, isCollapsed, isActive, isExpanded, onToggle }: NavLinkProps) {
  const hasChildren = item.children && item.children.length > 0;

  const content = (
    <Link
      href={item.href}
      className={cn(
        'flex items-center gap-3 px-3 py-2 rounded-lg transition',
        isActive
          ? 'bg-violet-600/20 text-violet-400'
          : 'text-gray-400 hover:bg-gray-800 hover:text-white',
        isCollapsed && 'justify-center px-2'
      )}
      title={isCollapsed ? item.label : undefined}
    >
      <span className={cn(isActive && 'text-violet-400')}>{item.icon}</span>
      
      {!isCollapsed && (
        <>
          <span className="flex-1">{item.label}</span>
          
          {item.badge && (
            <span className="px-1.5 py-0.5 text-[10px] font-medium bg-violet-500 text-white rounded">
              {item.badge}
            </span>
          )}
          
          {hasChildren && (
            <button
              onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
                onToggle();
              }}
              className="p-1 hover:bg-gray-700 rounded"
            >
              {isExpanded ? (
                <ChevronDown className="w-4 h-4" />
              ) : (
                <ChevronRight className="w-4 h-4" />
              )}
            </button>
          )}
        </>
      )}
    </Link>
  );

  return content;
}

// ============================================================
// HEADER
// ============================================================

interface HeaderProps {
  title: string;
  subtitle?: string;
  actions?: React.ReactNode;
}

export function Header({ title, subtitle, actions }: HeaderProps) {
  return (
    <header className="h-16 bg-white border-b border-gray-200 flex items-center justify-between px-6">
      <div>
        <h1 className="text-xl font-semibold text-gray-900">{title}</h1>
        {subtitle && <p className="text-sm text-gray-500">{subtitle}</p>}
      </div>
      
      {actions && (
        <div className="flex items-center gap-3">
          {actions}
        </div>
      )}
    </header>
  );
}

// ============================================================
// MOBILE NAV
// ============================================================

interface MobileNavProps {
  isOpen: boolean;
  onClose: () => void;
}

export function MobileNav({ isOpen, onClose }: MobileNavProps) {
  const pathname = usePathname();
  const { user, logout } = useAuthStore();

  return (
    <>
      {/* Overlay */}
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 lg:hidden"
          onClick={onClose}
        />
      )}

      {/* Drawer */}
      <div
        className={cn(
          'fixed inset-y-0 left-0 w-64 bg-gray-900 z-50 transform transition-transform lg:hidden',
          isOpen ? 'translate-x-0' : '-translate-x-full'
        )}
      >
        <div className="h-16 flex items-center px-4 border-b border-gray-800">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-violet-500 to-purple-600" />
            <span className="font-bold text-white">AI Platform</span>
          </div>
          <button
            onClick={onClose}
            className="ml-auto p-2 text-gray-400 hover:text-white"
          >
            <X className="w-6 h-6" />
          </button>
        </div>

        <nav className="p-4 space-y-2">
          {mainNavItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              onClick={onClose}
              className={cn(
                'flex items-center gap-3 px-3 py-2 rounded-lg transition',
                pathname === item.href
                  ? 'bg-violet-600/20 text-violet-400'
                  : 'text-gray-400 hover:bg-gray-800 hover:text-white'
              )}
            >
              {item.icon}
              <span>{item.label}</span>
            </Link>
          ))}

          {user?.role === 'Admin' && (
            <>
              <div className="pt-4">
                <p className="px-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                  Quản trị
                </p>
              </div>
              {adminNavItems.map((item) => (
                <Link
                  key={item.href}
                  href={item.href}
                  onClick={onClose}
                  className={cn(
                    'flex items-center gap-3 px-3 py-2 rounded-lg transition',
                    pathname === item.href
                      ? 'bg-violet-600/20 text-violet-400'
                      : 'text-gray-400 hover:bg-gray-800 hover:text-white'
                  )}
                >
                  {item.icon}
                  <span>{item.label}</span>
                </Link>
              ))}
            </>
          )}
        </nav>

        <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-gray-800">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-full bg-violet-600 flex items-center justify-center text-white font-medium">
              {user?.fullName?.charAt(0) || 'U'}
            </div>
            <div className="flex-1">
              <p className="text-sm font-medium text-white">
                {user?.fullName || 'User'}
              </p>
              <p className="text-xs text-gray-500">{user?.email}</p>
            </div>
            <button
              onClick={logout}
              className="p-2 rounded-lg hover:bg-gray-800 text-gray-400 hover:text-white"
            >
              <LogOut className="w-5 h-5" />
            </button>
          </div>
        </div>
      </div>
    </>
  );
}

// ============================================================
// PAGE LAYOUT
// ============================================================

interface PageLayoutProps {
  children: React.ReactNode;
}

export function DashboardLayout({ children }: PageLayoutProps) {
  const [sidebarCollapsed, setSidebarCollapsed] = React.useState(false);
  const [mobileNavOpen, setMobileNavOpen] = React.useState(false);

  return (
    <div className="min-h-screen bg-gray-50 flex">
      {/* Desktop Sidebar */}
      <div className="hidden lg:block relative">
        <Sidebar
          isCollapsed={sidebarCollapsed}
          onToggle={() => setSidebarCollapsed(!sidebarCollapsed)}
        />
      </div>

      {/* Main Content */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Mobile Header */}
        <header className="lg:hidden h-16 bg-white border-b border-gray-200 flex items-center px-4">
          <button
            onClick={() => setMobileNavOpen(true)}
            className="p-2 text-gray-600 hover:text-gray-900"
          >
            <Menu className="w-6 h-6" />
          </button>
          <div className="flex-1 flex justify-center">
            <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-violet-500 to-purple-600" />
          </div>
        </header>

        {/* Mobile Navigation */}
        <MobileNav
          isOpen={mobileNavOpen}
          onClose={() => setMobileNavOpen(false)}
        />

        {/* Page Content */}
        <main className="flex-1 overflow-auto">
          {children}
        </main>
      </div>
    </div>
  );
}
