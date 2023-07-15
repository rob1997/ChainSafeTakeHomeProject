using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Utils
{
    public class Beamer : MonoBehaviour
    {
        [Tooltip("Maximum allowed Hits, for Performance")]
        public int allowedHits = 3;
        
        [Tooltip("Max Distance for Cast, 0 equals Infinity")]
        public float maxDistance;
        
        public LayerMask layerMask;
        
        [Tooltip("Cast will Ignored Checked Tags")]
        public TagMask tagMask;

        private RaycastHit[] _cachedHitInfo;

        private bool _beamed;
        
        private Vector3 Origin => transform.position;
        
        private Vector3 Direction => transform.forward;
        
        public RaycastHit[] Beam()
        {
            if (_beamed) return _cachedHitInfo;

            _cachedHitInfo = new RaycastHit[allowedHits];

            Vector3 origin = Origin;
            
            float distance = maxDistance <= 0 ? Mathf.Infinity : maxDistance;
            
            for (int i = 0; i < allowedHits; i++)
            {
                if (Physics.Raycast(origin, Direction, out RaycastHit hitInfo, distance, layerMask))
                {
                    //skip ignored tag (redo loop)
                    if (tagMask.Contains(hitInfo.collider.tag)) i--;
                    
                    else _cachedHitInfo[i] = hitInfo;

                    Vector3 point = hitInfo.point;
                    
                    //remove already cast distance for next cast
                    distance -= Vector3.Distance(point, origin) + Physics.defaultContactOffset;
                    
                    //new origin for next cast!,
                    //add Physics.defaultContactOffset so that cast doesn't hit the same collider again!
                    origin = point + Direction.normalized * Physics.defaultContactOffset;

                    //avoid default value
                    if (distance < 0) distance = 0;
                }

                else break;
            }

            _cachedHitInfo = _cachedHitInfo.Where(h => h.collider != null)
                .OrderBy(h => Vector3.Distance(h.point, Origin)).ToArray();
            
            _beamed = true;
            
            return _cachedHitInfo;
        }

        private void LateUpdate()
        {
            if (_beamed) _beamed = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            
            if (_cachedHitInfo != null)
            {
                foreach (RaycastHit hitInfo in _cachedHitInfo)
                {
                    Gizmos.DrawLine(Origin, hitInfo.point);
                    
                    Gizmos.DrawSphere(hitInfo.point, .125f);
                }
            }
        }
    }
}
