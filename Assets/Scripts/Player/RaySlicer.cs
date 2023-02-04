using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using MrRoot.Root;
using UnityEngine.Serialization;

namespace MrRoot.Player
{
    public class RaySlicer : MonoBehaviour
    {
        public static event Action FlagHit;

        private MrRootInput _input;

        [ReadOnly] [SerializeField] private bool _pressed;
        [ReadOnly] [SerializeField] private Vector2 _screenPosition;
        [ReadOnly] [SerializeField] private Vector3 _previousWorldPosition;
        

        private Camera _mainCamera;
        private Camera MainCamera => _mainCamera ??= Camera.main;

        private void Start()
        {
            _input = new MrRootInput();
            
            _input.Player.Press.performed += OnPressPerformed;
            _input.Player.Press.canceled += OnPressCanceled;
            
            _input.Player.Position.performed += OnPositionPerformed;
            
            _input.Player.Enable();
        }
        
        private void OnDestroy()
        {
            _input.Player.Press.performed -= OnPressPerformed;
            _input.Player.Press.canceled -= OnPressCanceled;
            
            _input.Player.Position.performed -= OnPositionPerformed;
        }
        
        private void OnPositionPerformed(InputAction.CallbackContext obj)
        {
            if (!_pressed)
            {
                return;
            }

            _screenPosition = obj.ReadValue<Vector2>();
        }

        private void OnPressCanceled(InputAction.CallbackContext obj)
        {
            _pressed = false;
        }

        private void OnPressPerformed(InputAction.CallbackContext obj)
        {
            _pressed = true;
        }

        private void FixedUpdate()
        {
            if (!_pressed)
                return;
            
            var ray = MainCamera.ScreenPointToRay(_position);

            if (Physics.Raycast(ray, out var planeHit, 1000f, LayerMask.GetMask("Water")))
            {
                _previousWorldPosition = transform.position;
                transform.position = planeHit.point + Vector3.up;
                var velocity = transform.position - _previousWorldPosition;
                
                if (Physics.Raycast(ray, out var rootHit, 1000f, LayerMask.GetMask("Root")))
                {
                    if (rootHit.transform.TryGetComponent(out RootSlice rootSlice))
                    {
                        var normal = Vector3.Cross(-velocity, Vector3.up);
                        
                        DrawPlane(rootHit.point, normal);
                    
                        rootSlice.Slice(normal, rootHit.point);
                    }
                }
            }

            if (Physics.Raycast(ray, out var rootHit, LayerMask.GetMask("Root")))
            {
                // todo: get root and cut
            }
            
            if (Physics.Raycast(ray, out var flagHit, LayerMask.GetMask("Flag")))
            {
                if (!flagHit.collider.TryGetComponent<FlagBehaviour>(out var flag))
                    return;
                
                Debug.Log("Flag Hit");
                FlagHit?.Invoke();
            }
        }
    }
}