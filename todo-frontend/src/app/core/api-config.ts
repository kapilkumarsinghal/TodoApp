declare global {
  interface Window {
    todoAppConfig?: { apiBaseUrl?: string };
  }
}

export const apiBaseUrl = (window.todoAppConfig?.apiBaseUrl || 'http://localhost:5160/api').replace(/\/$/, '');

export function apiUrl(path: string): string {
  return `${apiBaseUrl}${path.startsWith('/') ? path : `/${path}`}`;
}
