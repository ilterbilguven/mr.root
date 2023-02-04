using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MrRoot.Player
{
    public class RaySlicer : MonoBehaviour
    {
        public static event Action FlagHit;

        private MrRootInput _input;

        [ReadOnly] [SerializeField] private bool _pressed;
        [ReadOnly] [SerializeField] private Vector2 _position;
        

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

            _position = obj.ReadValue<Vector2>();
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

            if (Physics.Raycast(ray, out var planeHit, LayerMask.GetMask("Water")))
            {
                transform.position = planeHit.point + Vector3.up;
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