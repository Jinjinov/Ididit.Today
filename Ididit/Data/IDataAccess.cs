﻿using Ididit.Data.Entities;

namespace Ididit.Data;

public interface IDataAccess
{
    Task Initialize();

    Task AddHabit(HabitEntity habit);
    Task AddNote(NoteEntity note);
    Task AddTask(TaskEntity task);
    Task AddTime(TimeEntity time);

    Task<IReadOnlyList<HabitEntity>> GetHabits();
    Task<IReadOnlyList<NoteEntity>> GetNotes();
    Task<IReadOnlyList<TaskEntity>> GetTasks();
    Task<IReadOnlyList<TimeEntity>> GetTimes(long? habitId = null);

    Task<HabitEntity?> GetHabit(long id);
    Task<NoteEntity?> GetNote(long id);
    Task<TaskEntity?> GetTask(long id);
    Task<TimeEntity?> GetTime(DateTime time);

    Task UpdateHabit(HabitEntity habit);
    Task UpdateNote(NoteEntity note);
    Task UpdateTask(TaskEntity task);
    Task UpdateTime(TimeEntity time);

    Task RemoveHabit(long id);
    Task RemoveNote(long id);
    Task RemoveTask(long id);
    Task RemoveTime(DateTime time);

    Task RemoveHabits();
    Task RemoveNotes();
    Task RemoveTasks();
    Task RemoveTimes();
}
