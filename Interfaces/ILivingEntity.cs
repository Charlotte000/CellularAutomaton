namespace CellularAutomaton.Interfaces;

public interface ILivingEntity : IGameObject
{
    public bool IsOnGround { get; set; }

    public bool IsOnWater { get; set; }

    public bool IsClimbing { get; set; }
}
