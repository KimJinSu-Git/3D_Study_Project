using System.Collections.Generic;
using UnityEngine;

public class CameraObscurer : MonoBehaviour
    {
        [SerializeField] private LayerMask mObstructionLayer;
        private Transform _playerTransform;
        
        private List<Fader> _fadedObjects = new List<Fader>();
        
        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (_playerTransform == null) return;
            
            CheckForObstructions();
        }

        private void CheckForObstructions()
        {
            Vector3 cameraPosition = transform.position;
            Vector3 playerPosition = _playerTransform.position;
            Vector3 direction = playerPosition - cameraPosition;
            float distance = direction.magnitude;
            
            // 카메라에서 플레이어를 향해 Ray를 쏴서 모든 충돌체를 감지
            RaycastHit[] hits = Physics.RaycastAll(cameraPosition, direction.normalized, distance, mObstructionLayer);

            List<Fader> currentHits = new List<Fader>();
            
            foreach (Fader fader in _fadedObjects.ToArray())
            {
                if (fader == null) continue;

                bool isHit = false;
                foreach (RaycastHit hit in hits)
                {
                    Fader hitFader = hit.collider.GetComponent<Fader>();
                    if (hitFader == fader)
                    {
                        isHit = true;
                        currentHits.Add(fader);
                        break;
                    }
                }
                
                if (!isHit)
                {
                    fader.FadeIn();
                    _fadedObjects.Remove(fader);
                }
            }

            foreach (RaycastHit hit in hits)
            {
                Fader fader = hit.collider.GetComponent<Fader>();
                if (fader != null && !_fadedObjects.Contains(fader))
                {
                    fader.FadeOut();
                    _fadedObjects.Add(fader);
                }
            }
        }
    }
