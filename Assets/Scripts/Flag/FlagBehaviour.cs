using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace MrRoot
{
    public class FlagBehaviour : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 2f;
        public event Action<Waypoint> WaypointReached;
        private Tween _moveTween;

        public void MoveTo(Waypoint waypoint)
        {
            _moveTween.Kill();
            var dist = Vector3.Distance(waypoint.Position, transform.position);
            transform.LookAt(waypoint.Position);
            _moveTween = transform.DOMove(waypoint.Position, dist / _moveSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() => WaypointReached?.Invoke(waypoint));
        }
    }
}

