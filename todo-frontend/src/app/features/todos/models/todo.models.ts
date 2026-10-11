export interface Todo {
  id: number;
  title: string;
  description?: string | null;
  isCompleted: boolean;
  dueDate?: string | null;
  startAt?: string | null;
  endAt?: string | null;
  reminderMinutesBefore?: number | null;
  repeatEveryHours?: number | null;
  category: string;
  priority: number;
  createdAt: string;
}

export interface CreateTodoRequest {
  title: string;
  description?: string | null;
  dueDate?: string | null;
  startAt?: string | null;
  endAt?: string | null;
  reminderMinutesBefore?: number | null;
  repeatEveryHours?: number | null;
  category: string;
  priority: number;
}

export interface UpdateTodoRequest {
  id: number;
  title: string;
  description?: string | null;
  isCompleted: boolean;
  dueDate?: string | null;
  startAt?: string | null;
  endAt?: string | null;
  reminderMinutesBefore?: number | null;
  repeatEveryHours?: number | null;
  category: string;
  priority: number;
}
