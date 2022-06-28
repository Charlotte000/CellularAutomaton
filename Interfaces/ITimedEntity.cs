namespace CellularAutomaton.Interfaces;

public interface ITimedEntity : IEntity
{
    public bool IsLifeTimeActive { get; }

    public float LifeTimeStart { get; set; }

    public float LifeTimeEnd { get; set; }

    public float LifeTime { get; }

    public void OnTimeOut();
}
