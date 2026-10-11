import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HabitApiService } from '../../data-access/habit-api.service';
import { Habit } from '../../models/habit.models';

@Component({
  selector: 'app-habit-tracker', standalone: true, imports: [CommonModule, FormsModule],
  templateUrl: './habit-tracker.html', styleUrl: './habit-tracker.css',
})
export class HabitTrackerComponent implements OnInit {
  private readonly api = inject(HabitApiService);
  readonly month = signal(new Date(new Date().getFullYear(), new Date().getMonth(), 1));
  readonly habits = signal<Habit[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly draftName = signal('');
  readonly draftEmoji = signal('✅');
  readonly days = computed(() => Array.from({ length: new Date(this.month().getFullYear(), this.month().getMonth() + 1, 0).getDate() }, (_, i) => i + 1));
  readonly label = computed(() => new Intl.DateTimeFormat('en', { month: 'long', year: 'numeric' }).format(this.month()));
  readonly weekdays = computed(() => this.days().map((day) => new Date(this.month().getFullYear(), this.month().getMonth(), day).toLocaleDateString('en', { weekday: 'short' })));
  readonly doneTotal = computed(() => this.habits().reduce((sum, habit) => sum + habit.completedDays.length, 0));
  readonly possibleTotal = computed(() => this.days().length * this.habits().length);
  readonly progress = computed(() => this.possibleTotal() ? Math.round(this.doneTotal() / this.possibleTotal() * 100) : 0);
  readonly dayProgress = computed(() => this.days().map((day) => ({
    day, done: this.habits().filter((habit) => habit.completedDays.includes(day)).length,
    percent: this.habits().length ? Math.round(this.habits().filter((habit) => habit.completedDays.includes(day)).length / this.habits().length * 100) : 0,
  })));
  ngOnInit(): void { void this.load(); }
  async load(): Promise<void> {
    this.loading.set(true);
    try { this.habits.set(await this.api.get(this.month().getFullYear(), this.month().getMonth() + 1)); this.error.set(''); }
    catch { this.error.set('Could not load habits. Check that the API is running and its database is available.'); }
    finally { this.loading.set(false); }
  }
  shiftMonth(amount: number): void { this.month.update((date) => new Date(date.getFullYear(), date.getMonth() + amount, 1)); void this.load(); }
  dateFor(day: number): string { const date = this.month(); return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`; }
  isDone(habit: Habit, day: number): boolean { return habit.completedDays.includes(day); }
  async toggle(habit: Habit, day: number): Promise<void> {
    const completed = !this.isDone(habit, day);
    this.habits.update((rows) => rows.map((row) => row.id !== habit.id ? row : { ...row, completedDays: completed ? [...row.completedDays, day].sort((a,b) => a-b) : row.completedDays.filter((value) => value !== day) }));
    try { await this.api.setCompletion(habit.id, this.dateFor(day), completed); }
    catch { await this.load(); this.error.set('Could not save that check-in.'); }
  }
  async addHabit(): Promise<void> {
    const name = this.draftName().trim(); if (!name) return;
    try { await this.api.create({ name, emoji: this.draftEmoji().trim() || '✅' }); this.draftName.set(''); await this.load(); }
    catch { this.error.set('Could not create the habit.'); }
  }
  async deleteHabit(habit: Habit): Promise<void> {
    try { await this.api.delete(habit.id); this.habits.update((rows) => rows.filter((row) => row.id !== habit.id)); }
    catch { this.error.set('Could not delete the habit.'); }
  }
  monthGroups(): { label: string; days: number[] }[] {
    const groups = new Map<string, number[]>();
    for (const day of this.days()) { const week = Math.ceil(day / 7); const label = `Week ${week}`; groups.set(label, [...(groups.get(label) ?? []), day]); }
    return [...groups].map(([label, days]) => ({ label, days }));
  }
  dayOfWeek(day: number): string { return this.weekdays()[day - 1]; }
  isToday(day: number): boolean { const current = new Date(); const selected = this.month(); return current.getDate() === day && current.getMonth() === selected.getMonth() && current.getFullYear() === selected.getFullYear(); }
  habitPercent(habit: Habit): number { return Math.round(habit.completedDays.length / this.days().length * 100); }
}
