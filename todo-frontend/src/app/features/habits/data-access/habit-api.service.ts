import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Habit, HabitCreateRequest } from '../models/habit.models';

@Injectable({ providedIn: 'root' })
export class HabitApiService {
  private readonly http = inject(HttpClient);
  private readonly url = 'http://localhost:5160/api/habits';
  get(year: number, month: number): Promise<Habit[]> { return firstValueFrom(this.http.get<Habit[]>(`${this.url}?year=${year}&month=${month}`)); }
  create(request: HabitCreateRequest): Promise<Habit> { return firstValueFrom(this.http.post<Habit>(this.url, request)); }
  setCompletion(id: number, date: string, completed: boolean): Promise<void> { return firstValueFrom(this.http.put<void>(`${this.url}/${id}/completion`, { date, completed })); }
  delete(id: number): Promise<void> { return firstValueFrom(this.http.delete<void>(`${this.url}/${id}`)); }
}
