﻿using Ididit.Data;
using Ididit.Data.Entities;
using Ididit.Data.Models;

namespace Ididit.Services;

public class HabitService(IDataAccess dataAccess)
{
    private readonly IDataAccess _dataAccess = dataAccess;

    public List<HabitModel>? Habits { get; set; }

    public HabitModel? NewHabit { get; set; }

    public HabitModel? EditHabit { get; set; }

    public async Task Initialize()
    {
        if (Habits is null)
        {
            IReadOnlyList<HabitEntity> habits = await _dataAccess.GetHabits();
            Habits = habits.Select(h => new HabitModel
            {
                Id  = h.Id,
                IsDeleted  = h.IsDeleted,
                Title  = h.Title,
                CreatedAt  = h.CreatedAt,
                UpdatedAt  = h.UpdatedAt,
                Priority  = h.Priority,
                Importance = h.Importance,

                AverageInterval = h.AverageInterval,
                DesiredInterval = h.DesiredInterval,
                LastTimeDoneAt = h.LastTimeDoneAt
            }).ToList();
        }

        if (NewHabit is null)
        {
            NewHabit = new();
        }
    }

    public async Task LoadTimesDone(long? id)
    {
        if (Habits is null)
            return;

        if (Habits.FirstOrDefault(h => h.Id == id) is HabitModel habit && habit.TimesDone is null)
        {
            IReadOnlyList<TimeEntity> timesDone = await _dataAccess.GetTimes(habit.Id);
            habit.TimesDone = timesDone.Select(t => t.Time).ToList();
        }
    }

    public async Task AddHabit()
    {
        if (Habits is null || NewHabit is null)
            return;

        DateTime utcNow = DateTime.UtcNow;

        NewHabit.CreatedAt = utcNow;
        NewHabit.UpdatedAt = utcNow;

        Habits.Add(NewHabit);

        HabitEntity habit = new()
        {
            IsDeleted = false,
            Title = NewHabit.Title,
            CreatedAt = utcNow,
            UpdatedAt = utcNow,
            Priority = NewHabit.Priority,
            Importance = NewHabit.Importance,

            AverageInterval = NewHabit.AverageInterval,
            DesiredInterval = NewHabit.DesiredInterval,
            LastTimeDoneAt = null
        };

        await _dataAccess.AddHabit(habit);

        NewHabit.Id = habit.Id;

        NewHabit = new();
    }

    public async Task UpdateHabit()
    {
        if (Habits is null || EditHabit is null)
            return;

        if (await _dataAccess.GetHabit(EditHabit.Id) is HabitEntity habit)
        {
            habit.IsDeleted = EditHabit.IsDeleted;
            habit.Title = EditHabit.Title;
            habit.CreatedAt = EditHabit.CreatedAt;
            habit.UpdatedAt = EditHabit.UpdatedAt;
            habit.Priority = EditHabit.Priority;
            habit.Importance = EditHabit.Importance;

            habit.AverageInterval = EditHabit.AverageInterval;
            habit.DesiredInterval = EditHabit.DesiredInterval;
            habit.LastTimeDoneAt = EditHabit.LastTimeDoneAt;

            await _dataAccess.UpdateHabit(habit);
        }

        EditHabit = null;
    }

    public async Task MarkAsDone(HabitModel habit)
    {
        if (Habits is null)
            return;

        DateTime utcNow = DateTime.UtcNow;

        habit.LastTimeDoneAt = utcNow;

        if (habit.TimesDone is null)
            habit.TimesDone = new();

        habit.TimesDone.Add(utcNow);

        TimeEntity time = new()
        {
            HabitId = habit.Id,
            Time = utcNow
        };

        await _dataAccess.AddTime(time);

        if (await _dataAccess.GetHabit(habit.Id) is HabitEntity habitEntity)
        {
            habitEntity.LastTimeDoneAt = utcNow;
            await _dataAccess.UpdateHabit(habitEntity);
        }
    }

    public async Task DeleteHabit(HabitModel habit)
    {
        if (Habits is null)
            return;

        Habits.Remove(habit);

        if (await _dataAccess.GetHabit(habit.Id) is HabitEntity habitEntity)
        {
            habitEntity.IsDeleted = true;
            await _dataAccess.UpdateHabit(habitEntity);
        }
    }
}
