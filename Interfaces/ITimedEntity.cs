namespace CellularAutomaton.Interfaces;

public interface ITimedEntity : IGameObject
{
    public bool IsLifeTimeActive { get; }

    public float LifeTimeStart { get; set; }

    public float LifeTimeEnd { get; set; }

    public float LifeTime { get; }

    public void OnTimeOut();
}
