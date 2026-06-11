using CutTwice.Core.Lifecycle;
using UnityEngine;
using UnityEngine.Events;

namespace CutTwice.Gameplay.Runtime.Road.Components
{
    public class InfiniteRoadController : ITickable
    {
        private readonly InfiniteRoadPresenter _presenter;

        private bool _startStop;
        private bool stopped;
        
        public UnityEvent<Transform> OnSegmentSpawned = new();

        public InfiniteRoadController(InfiniteRoadPresenter presenter)
        {
            _presenter = presenter;
            _startStop = false;
        }

        public void Tick()
        {
            if (stopped)
                return;

            foreach (Transform segment in _presenter.roadSegments)
            {
                if (segment.position.z <= _presenter.recycleZ)
                {
                    float maxZ = FindMaxZ();
                    if (_startStop)
                    {
                        if (_presenter.finalSegment != null)
                        {
                            _presenter.finalSegment.position = new Vector3(segment.position.x, segment.position.y, maxZ + _presenter.segmentLength);
                        }
                        stopped = true;
                    }
                    else
                    {
                        OnSegmentSpawned?.Invoke(segment);
                        segment.position = new Vector3(segment.position.x, segment.position.y, maxZ + _presenter.segmentLength);
                    }
                }

                segment.Translate(Vector3.back * _presenter.moveSpeed * Time.deltaTime);
            }
        }

        private float FindMaxZ()
        {
            float maxZ = float.MinValue;
            foreach (Transform segment in _presenter.roadSegments)
            {
                if (segment.position.z > maxZ)
                    maxZ = segment.position.z;
            }
            return maxZ;
        }
    }
}



