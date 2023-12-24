using System;
using System.Collections.Generic;

public interface ITimeContext
{
    int CurrentIdx { get; set; }
    float CurrentTime { get; set; }
    float LastTime { get; set; }
}

public class TimeContext : ITimeContext
{
    public int CurrentIdx { get; set; }
    public float CurrentTime { get; set; }
    public float LastTime { get; set; }
}

public interface ITimeContextContainer
{
    ITimeContext GetTimeContext<T>();
}

public class TimeContextContainer : ITimeContextContainer
{
    Dictionary<Type, ITimeContext> _store = new();

    public TimeContextContainer(params Type[] types) {
        foreach (var t in types) {
            _store[t] = new TimeContext();
        }
    }

    public ITimeContext GetTimeContext<T>() {
        return _store[typeof(T)];
    }
}