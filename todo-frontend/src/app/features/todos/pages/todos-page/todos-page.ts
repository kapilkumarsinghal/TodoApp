import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateTodoRequest, Todo, UpdateTodoRequest } from '../../models/todo.models';
import { TodoApiService } from '../../data-access/todo-api.service';

interface CalendarDay {
  date: Date;
  inCurrentMonth: boolean;
}

interface CalendarOccurrence { todo: Todo; start: Date; end: Date; }

@Component({
  selector: 'app-todos-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './todos-page.html',
  styleUrl: './todos-page.css',
})
export class TodosPageComponent implements OnInit {
  private readonly todoApi = inject(TodoApiService);

  readonly categories = ['Work', 'Personal', 'Health', 'Learning'] as const;
  readonly hours = Array.from({ length: 15 }, (_, index) => new Date(2000, 0, 1, index + 7));
  readonly selectedDate = signal(startOfDay(new Date()));
  readonly selectedCategory = signal<string | null>(null);
  readonly todos = signal<Todo[]>([]);
  readonly isComposerOpen = signal(false);
  readonly isSubmitting = signal(false);
  private reminderTimers: number[] = [];
  readonly errorMessage = signal('');
  readonly timezoneLabel = formatTimezone(new Date());

  readonly weekDays = computed(() => {
    const monday = startOfWeek(this.selectedDate());
    return Array.from({ length: 7 }, (_, index) => addDays(monday, index));
  });

  readonly weekLabel = computed(() => {
    const days = this.weekDays();
    const first = days[0];
    const last = days[6];
    const month = new Intl.DateTimeFormat('en', { month: 'long' });
    const year = new Intl.DateTimeFormat('en', { year: 'numeric' });

    if (first.getMonth() === last.getMonth()) {
      return `${month.format(first)} ${first.getDate()} – ${last.getDate()}, ${year.format(last)}`;
    }
    return `${month.format(first)} ${first.getDate()} – ${month.format(last)} ${last.getDate()}, ${year.format(last)}`;
  });

  readonly miniMonthLabel = computed(() =>
    new Intl.DateTimeFormat('en', { month: 'long', year: 'numeric' }).format(this.selectedDate()),
  );

  readonly miniCalendarDays = computed<CalendarDay[]>(() => {
    const selected = this.selectedDate();
    const firstOfMonth = new Date(selected.getFullYear(), selected.getMonth(), 1);
    const gridStart = startOfWeek(firstOfMonth);
    return Array.from({ length: 42 }, (_, index) => {
      const date = addDays(gridStart, index);
      return { date, inCurrentMonth: date.getMonth() === selected.getMonth() };
    });
  });

  readonly scheduledCount = computed(() => {
    const week = this.weekDays();
    return this.visibleTodos().filter((todo) =>
      todo.startAt && week.some((day) => sameDay(new Date(todo.startAt!), day)),
    ).length;
  });
  readonly completedCount = computed(() => this.visibleTodos().filter((todo) => todo.isCompleted).length);
  readonly unscheduledTodos = computed(() =>
    this.visibleTodos().filter((todo) => !todo.startAt && !todo.isCompleted),
  );
  readonly upcomingTodos = computed(() =>
    this.visibleTodos()
      .filter((todo) => todo.startAt && !todo.isCompleted)
      .sort((a, b) => Date.parse(a.startAt!) - Date.parse(b.startAt!))
      .slice(0, 5),
  );

  form: CreateTodoRequest = this.createEmptyForm();

  ngOnInit(): void {
    void this.loadTodos();
  }

  ngOnDestroy(): void { this.reminderTimers.forEach((timer) => window.clearTimeout(timer)); }

  visibleTodos(): Todo[] {
    const category = this.selectedCategory();
    return category ? this.todos().filter((todo) => todo.category === category) : this.todos();
  }

  eventsForDay(day: Date): CalendarOccurrence[] {
    const instances: CalendarOccurrence[] = [];
    for (const todo of this.visibleTodos()) {
      if (!todo.startAt || !sameDay(new Date(todo.startAt), day)) continue;
      const first = new Date(todo.startAt);
      const duration = todo.endAt ? Date.parse(todo.endAt) - first.getTime() : 60 * 60 * 1000;
      const interval = todo.repeatEveryHours ? todo.repeatEveryHours * 60 * 60 * 1000 : Infinity;
      for (let start = first.getTime(); start < startOfDay(day).getTime() + 22 * 60 * 60 * 1000; start += interval) {
        instances.push({ todo, start: new Date(start), end: new Date(start + duration) });
        if (!Number.isFinite(interval)) break;
      }
    }
    return instances.sort((a, b) => a.start.getTime() - b.start.getTime());
  }

  eventTop(event: CalendarOccurrence): number {
    const start = event.start;
    const minutes = start.getHours() * 60 + start.getMinutes() - 7 * 60;
    return Math.max(0, Math.min(100, (minutes / (15 * 60)) * 100));
  }

  eventHeight(event: CalendarOccurrence): number {
    const start = event.start;
    const end = event.end;
    const duration = Math.max(30, (end.getTime() - start.getTime()) / 60000);
    return Math.max(4, Math.min(100 - this.eventTop(event), (duration / (15 * 60)) * 100));
  }

  categoryClass(category: string): string {
    return `category-${category.toLowerCase().replace(/[^a-z0-9]+/g, '-')}`;
  }

  chooseDate(date: Date): void {
    this.selectedDate.set(startOfDay(date));
  }

  chooseTodoDate(todo: Todo): void {
    if (todo.startAt) this.chooseDate(new Date(todo.startAt));
  }

  isDateSelected(date: Date): boolean {
    return sameDay(date, this.selectedDate());
  }

  isToday(date: Date): boolean {
    return sameDay(date, new Date());
  }

  goToMonth(): void {
    const now = new Date();
    this.chooseDate(new Date(now.getFullYear(), now.getMonth(), 1));
  }

  shiftWeek(amount: number): void {
    this.selectedDate.update((date) => addDays(date, amount * 7));
  }

  shiftMonth(amount: number): void {
    this.selectedDate.update((date) => new Date(date.getFullYear(), date.getMonth() + amount, 1));
  }

  goToToday(): void {
    this.selectedDate.set(startOfDay(new Date()));
    this.selectedCategory.set(null);
  }

  openComposer(): void {
    this.form = this.createEmptyForm();
    this.isComposerOpen.set(true);
    this.errorMessage.set('');
  }

  closeComposer(): void {
    this.isComposerOpen.set(false);
  }

  async loadTodos(): Promise<void> {
    try {
      this.todos.set(await this.todoApi.getAll());
      this.scheduleReminders();
      this.errorMessage.set('');
    } catch {
      this.errorMessage.set('Could not load tasks from the API. Check that the API is running.');
    }
  }

  async submitTodo(): Promise<void> {
    const title = this.form.title.trim();
    if (!title) {
      this.errorMessage.set('A title is required.');
      return;
    }
    if (!this.form.startAt || !this.form.endAt || Date.parse(this.form.endAt) <= Date.parse(this.form.startAt)) {
      this.errorMessage.set('Choose an end time after the start time.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');
    try {
      await this.todoApi.create({
        ...this.form,
        title,
        description: this.form.description?.trim() || '',
        dueDate: this.form.endAt,
      });
      if (this.form.reminderMinutesBefore && 'Notification' in window && Notification.permission === 'default') {
        await Notification.requestPermission();
      }
      this.scheduleReminders();
      this.closeComposer();
      await this.loadTodos();
    } catch {
      this.errorMessage.set('The event could not be saved.');
    } finally {
      this.isSubmitting.set(false);
    }
  }

  async toggleTodo(todo: Todo): Promise<void> {
    const request: UpdateTodoRequest = {
      id: todo.id,
      title: todo.title,
      description: todo.description ?? '',
      isCompleted: !todo.isCompleted,
      dueDate: todo.dueDate ?? null,
      startAt: todo.startAt ?? null,
      endAt: todo.endAt ?? null,
      reminderMinutesBefore: todo.reminderMinutesBefore ?? null,
      repeatEveryHours: todo.repeatEveryHours ?? null,
      category: todo.category,
      priority: todo.priority,
    };

    try {
      await this.todoApi.update(todo.id, request);
      this.todos.update((items) => items.map((item) => item.id === todo.id
        ? { ...item, isCompleted: request.isCompleted }
        : item));
    } catch {
      this.errorMessage.set('Could not update the event.');
    }
  }

  async deleteTodo(id: number): Promise<void> {
    try {
      await this.todoApi.delete(id);
      this.todos.update((items) => items.filter((todo) => todo.id !== id));
    } catch {
      this.errorMessage.set('Could not delete the event.');
    }
  }

  private createEmptyForm(): CreateTodoRequest {
    const start = new Date(this.selectedDate());
    start.setHours(9, 0, 0, 0);
    const end = new Date(start.getTime() + 60 * 60 * 1000);
    return {
      title: '',
      description: '',
      startAt: toLocalInput(start),
      endAt: toLocalInput(end),
      dueDate: null,
      category: this.selectedCategory() ?? 'Work',
      priority: 2,
      reminderMinutesBefore: null,
      repeatEveryHours: null,
    };
  }

  private scheduleReminders(): void {
    this.reminderTimers.forEach((timer) => window.clearTimeout(timer));
    this.reminderTimers = [];
    if (!('Notification' in window) || Notification.permission !== 'granted') return;
    for (const todo of this.todos()) {
      if (!todo.startAt || !todo.reminderMinutesBefore || todo.isCompleted) continue;
      const notifyAt = Date.parse(todo.startAt) - todo.reminderMinutesBefore * 60_000;
      const delay = notifyAt - Date.now();
      if (delay <= 0 || delay > 2_147_000_000) continue;
      this.reminderTimers.push(window.setTimeout(() => new Notification('Upcoming event', {
        body: `${todo.title} starts in ${todo.reminderMinutesBefore} minutes.`,
      }), delay));
    }
  }
}

function startOfDay(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}

function startOfWeek(date: Date): Date {
  const result = startOfDay(date);
  result.setDate(result.getDate() - ((result.getDay() + 6) % 7));
  return result;
}

function addDays(date: Date, amount: number): Date {
  const result = new Date(date);
  result.setDate(result.getDate() + amount);
  return result;
}

function sameDay(first: Date, second: Date): boolean {
  return first.getFullYear() === second.getFullYear()
    && first.getMonth() === second.getMonth()
    && first.getDate() === second.getDate();
}

function toLocalInput(date: Date): string {
  const pad = (value: number) => String(value).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

function formatTimezone(date: Date): string {
  const offset = -date.getTimezoneOffset();
  const sign = offset >= 0 ? '+' : '−';
  const hours = String(Math.floor(Math.abs(offset) / 60)).padStart(2, '0');
  const minutes = String(Math.abs(offset) % 60).padStart(2, '0');
  return `GMT${sign}${hours}:${minutes}`;
}
