using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;

namespace NodeCanvasAddons.AStarPathfinding
{
    [Category("A* Pathfinding/Rich AI")]
    [Name("Is Slow When Not Facing Target")]
    [Description("Checks to see if a rich AI agent is slow when not facing target")]
    [ParadoxNotion.Design.Icon("PathfindingPath")]
    public class IsSlowWhenNotFacingTargetCondition : ConditionTask<RichAI>
    {
        [GetFromAgent]
        private RichAI _richAI = default;

        protected override string info
        {
            get { return "Is Rich AI Slow When Not Facing Target"; }
        }

        protected override bool OnCheck()
        {
            return _richAI.slowWhenNotFacingTarget;
        }
    }
}