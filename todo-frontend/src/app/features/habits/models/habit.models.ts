export interface Habit { id: number; name: string; emoji: string; createdAt: string; completedDays: number[]; }
export interface HabitCreateRequest { name: string; emoji: string; }
