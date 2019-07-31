using Assets.Scripts.Actors;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.Controllers
{
    public class PlayableActorController : MonoBehaviour
    {
        #region Private variables
        [SerializeField]
        private List<Actor> _playableActors;

        [SerializeField]
        private KeyCode _upKey;

        [SerializeField]
        private KeyCode _downKey;

        [SerializeField]
        private KeyCode _leftKey;

        [SerializeField]
        private KeyCode _rightKey;

        [SerializeField]
        private KeyCode _swapWeaponsCode;

        [SerializeField]
        private KeyCode _aimWeaponButton;

        [SerializeField]
        private CameraController _cameraController;

        private Dictionary<KeyCode, Commands> _keyCodesToCommands;
        private Camera _camera;

        #endregion

        #region Methods
        private void Start()
        {
            _camera = Camera.main;
            _keyCodesToCommands = new Dictionary<KeyCode, Commands>
            {
                { _upKey, Commands.Up },
                { _downKey, Commands.Down },
                { _leftKey, Commands.Left },
                { _rightKey, Commands.Right },
                { _swapWeaponsCode, Commands.SwapWeapons }
            };
        }

        private void FixedUpdate()
        {
            var selected = _playableActors.Where(x => x.IsSelected).ToList();

            if (selected.Count() > 0 && Input.GetKey(_aimWeaponButton))
            {
                selected.ForEach(x => x.LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition)));

                if (Input.GetMouseButton(0))
                {
                    selected.ForEach(x => x.GetCommand(Commands.Shoot));
                }
                return;
            }

            if (!Input.anyKey)
            {
                return;
            }
            for (int i = 0; i < _playableActors.Count(); ++i)
            {
                if (Input.GetKeyDown("" + (i + 1)))
                {
                    if (_playableActors[i].IsSelected)
                    {
                        _cameraController.TargetsToFollow.Remove(_playableActors[i].transform);
                    }
                    else
                    {
                        _cameraController.TargetsToFollow.Add(_playableActors[i].transform);
                    }
                    _playableActors[i].IsSelected = !_playableActors[i].IsSelected;
                }
            }

            var pressedCommand = _keyCodesToCommands.Keys.FirstOrDefault(x => Input.GetKey(x));

            if (pressedCommand != KeyCode.None)
            {
                selected.ForEach(x => x.GetCommand(_keyCodesToCommands[pressedCommand]));
            }
        }
        #endregion
    }
}

