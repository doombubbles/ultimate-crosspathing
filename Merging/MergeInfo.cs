using Assets.Scripts.Models.Towers;

namespace UltimateCrosspathing.Merging
{
    public struct MergeInfo
    {
        public TowerModel result;
        public TowerModel left;
        public TowerModel right;
        public TowerModel commonAncestor;

        public MergeInfo(TowerModel result, TowerModel left, TowerModel right, TowerModel commonAncestor)
        {
            this.result = result;
            this.left = left;
            this.right = right;
            this.commonAncestor = commonAncestor;
        }

        public void Deconstruct(out TowerModel result, out TowerModel left, out TowerModel right,
            out TowerModel commonAncestor)
        {
            result = this.result;
            left = this.left;
            right = this.right;
            commonAncestor = this.commonAncestor;
        }
    }
}