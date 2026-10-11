import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import {
  CreateTodoRequest,
  Todo,
  UpdateTodoRequest,
} from '../models/todo.models';

@Injectable({ providedIn: 'root' })
export class TodoApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5160/api/todos';

  getAll(): Promise<Todo[]> {
    return firstValueFrom(this.http.get<Todo[]>(this.baseUrl));
  }

  create(request: CreateTodoRequest): Promise<Todo> {
    return firstValueFrom(this.http.post<Todo>(this.baseUrl, request));
  }

  update(id: number, request: UpdateTodoRequest): Promise<Todo> {
    return firstValueFrom(this.http.put<Todo>(`${this.baseUrl}/${id}`, request));
  }

  delete(id: number): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.baseUrl}/${id}`));
  }
}
