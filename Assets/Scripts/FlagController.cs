using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SRF;
using UnityEngine;

namespace MrRoot
{
    public class FlagController : MonoBehaviour
    {
        private FlagBehaviour _flag;
        private List<Waypoint> _waypoints;
        
        private void Awake()
        {
            _flag = GetComponentInChildren<FlagBehaviour>();
            _waypoints = GetComponentsInChildren<Waypoint>().ToList();
            Initialize();
        }

        private void OnDestroy()
        {
            _flag.WaypointReached -= OnWayointReached;
        }

        public void Initialize()
        {
            _flag.WaypointReached += OnWayointReached;
            _flag.MoveTo(_waypoints.Random());
        }

        private void OnWayointReached(Waypoint waypoint)
        {
            var nextWaypoint = _waypoints.Where(w => w != waypoint).ToList().Random();
            _flag.MoveTo(nextWaypoint);
        }
    }

}
