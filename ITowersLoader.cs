namespace UltimateCrosspathing
{
    public interface ITowersLoader
    {
        Assets.Scripts.Models.Towers.TowerModel Load(byte[] bytes);
    }
}