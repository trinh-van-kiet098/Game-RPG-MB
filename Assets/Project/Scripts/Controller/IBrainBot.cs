public interface IBrainBot
{
    void Configure(UnitController unit, SystemGameManager system);
    void TickUpdate(float deltaTime);
}