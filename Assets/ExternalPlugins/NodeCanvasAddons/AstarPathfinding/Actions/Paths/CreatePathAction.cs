﻿using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pathfinding;
using UnityEngine;

namespace NodeCanvasAddons.AStarPathfinding
{
    [Category("A* Pathfinding/Paths")]
    [Name("Create Path")]
    [Description("Creates a path from the current agent to the destination point")]
    [ParadoxNotion.Design.Icon("PathfindingPath")]
    public class CreatePathAction : ActionTask
    {
        [RequiredField]
        public BBParameter<Vector3> DestinationPosition;

        [BlackboardOnly]
        public BBParameter<Path> OutputPath = new BBParameter<Path>();

        Seeker Seaker;

        protected override string info
        {
            get { return string.Format("Creating basic path \nas {0}", OutputPath); }
        }

        protected override string OnInit()
        {
            Seaker = ownerSystemAgent.GetComponent<Seeker>();
            return base.OnInit();
        }

        protected override void OnExecute()
        {
            Seaker.StartPath(agent.transform.position, DestinationPosition.value, PathFinishedDelegate);
           // AstarPath.StartPath(currentPath);
        }

        private void PathFinishedDelegate(Path path)
        {
            OutputPath.value = path;
            EndAction(!path.error);
        }
    }
}