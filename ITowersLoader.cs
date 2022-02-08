namespace UltimateCrosspathing
{
    public abstract class TowersLoader
    {
        public object[] m;
        public abstract Assets.Scripts.Models.Towers.TowerModel Load(byte[] bytes);
    }
}