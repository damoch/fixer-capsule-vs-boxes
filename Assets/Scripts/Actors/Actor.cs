using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Items;
using UnityEngine;
namespace Assets.Scripts.Actors
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(BoxCollider))]
    public class Actor : MonoBehaviour
    {
        #region Private variables
        [SerializeField]
        private bool _isSelected;

        [SerializeField]
        private int _healthPoints;

        [SerializeField]
        private bool _isAlive;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private string _name;

        [SerializeField]
        private Team _team;

        [SerializeField]
        private GameObject _mainWeaponObject;

        [SerializeField]
        private GameObject _secondaryWeaponObject;

        [SerializeField]
        private ActorType _actorType;

        [SerializeField]
        private Vector2 _targetPosition;

        [SerializeField]
        private bool _isInfected;

        [SerializeField]
        private GameObject _armorObject;

        [SerializeField]
        private float _weaponSwapLength;


        private GameObject _actorDisplayer;
        private Rigidbody2D _rigidbody2D;
        private Armor _armor;
        private float _weaponSwapPassed;
        private bool _isSwapingWeapons;
        private Dictionary<Commands, Vector2> _commandToDirection;
        private ActorDisplayerController _actorDisplayerController;
        private bool _isAnimatingFrame;
        #endregion

        #region Accesors
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (!_isAlive)
                {
                    return;
                }
                _isSelected = value;
                if (_rigidbody2D == null)
                {
                    _rigidbody2D = GetComponent<Rigidbody2D>();
                }
                _rigidbody2D.bodyType = _isSelected ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;

                if(_actorDisplayer != null)
                {
                    _actorDisplayer.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
        }

        public int HealthPoints
        {
            get
            {
                return _healthPoints;
            }

            set
            {
                if(value <= 0)
                {
                    KillActor();
                }
                _healthPoints = value;
            }
        }


        public bool IsAlive
        {
            get
            {
                return _isAlive;
            }
        }

        public Team Team
        {
            get
            {
                return _team;
            }

            set
            {
                _team = value;
            }
        }

        public Weapon Weapon { get; set; }

        public ActorType ActorType
        {
            get
            {
                return _actorType;
            }

            set
            {
                _actorType = value;
            }
        }

        public bool IsInfected
        {
            get
            {
                return _isInfected;
            }

            set
            {
                _isInfected = value;
            }
        }
        #endregion

        #region Methods
        private void Start()
        {
            _commandToDirection = new Dictionary<Commands, Vector2>
            {
                {Commands.Up, Vector2.up },
                {Commands.Down, Vector2.down },
                {Commands.Left, Vector2.left },
                {Commands.Right, Vector2.right }
            };

            Weapon = _mainWeaponObject?.GetComponent<Weapon>();
            if(_secondaryWeaponObject != null)
            {
                _secondaryWeaponObject.SetActive(false);

            }

            if(_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }
            _actorDisplayer = transform.GetChild(0).gameObject;
            _actorDisplayerController = _actorDisplayer.GetComponent<ActorDisplayerController>();
            _actorDisplayerController.SetAnimationState(false);
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _isSelected = false;

            if (_healthPoints > 0)
            {
                _isAlive = true;
            }
        }

        public bool GetCommand(Commands command, object param = null)
        {
            if(!_isAlive || !_isSelected)
            {
                return false;
            }
            switch (command)
            {
                case Commands.Up:
                case Commands.Down:
                case Commands.Right:
                case Commands.Left:
                    GetMoveCommand(command);
                    break;
                case Commands.Shoot:
                    FireWeapon();
                    break;
                case Commands.SwapWeapons:
                    SwapWeapons();
                    break;
                case Commands.Aim:
                    LookAt((Vector2)param);
                    break;
            }
            return true;
        }

        private void GetMoveCommand(Commands command)
        {
            var spd = _speed * Time.deltaTime;
            _actorDisplayerController.SetAnimationState(true);
            _isAnimatingFrame = true;
            transform.Translate(_commandToDirection[command] * spd);
        }

        private void FireWeapon()
        {
            if (!_isSwapingWeapons)
            {
                Weapon.Shoot(_actorDisplayer.transform.localRotation);
            }
        }

        public void LookAt(Vector2 position)
        {
            if(_actorDisplayer == null)
            {
                return;
            }
            _actorDisplayer.transform.localRotation = Quaternion.Euler(0f, 0f, AngleBetweenTwoPoints(transform.position, position));
        }

        private void KillActor()
        {
            _isAlive = false;
            _isSelected = false;
            _speed = 0;
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _actorDisplayerController.SetAnimationState(false);
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        public void MoveTowards(Vector2 position)
        {
            if(_actorType != ActorType.NonPlayable)
            {
                return;
            }
            _targetPosition = position;
        }

        private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        private void Update()
        {
            if (_isSwapingWeapons)
            {
                _weaponSwapPassed += Time.deltaTime;

                if (_weaponSwapPassed >= _weaponSwapLength)
                {
                    PerformWeaponSwap();
                }
            }
            //_displayerController.SetAnimationState(false);
            if (_actorType != ActorType.NonPlayable || !_isSelected)
            {
                return;
            }
            var spd = Time.deltaTime * _speed;
            if(_targetPosition != Vector2.zero)
            {
                transform.position = Vector2.MoveTowards(transform.position, _targetPosition, spd);

                if (!_actorDisplayerController.IsAnimating)
                {
                    _actorDisplayerController.SetAnimationState(true);
                }
            } else if (_actorDisplayerController.IsAnimating)
            {
                _actorDisplayerController.SetAnimationState(false);
            }

        }

        private void LateUpdate()
        {
            if (_isAnimatingFrame)
            {
                _isAnimatingFrame = false;
                _actorDisplayerController.SetAnimationState(false);
            }
        }

        public int GetTimesAttackShouldBeWeaker()
        {
            if(_armorObject == null)
            {
                return 1;
            }

            if(_armor == null)
            {
                _armor = _armorObject.GetComponent<Armor>();
            }

            return _armor.DamageDecreaseValue > 0 ? _armor.DamageDecreaseValue : 1; //return 1 to avoid dividing by zero
        }

        public void SwapWeapons()
        {
            if (_isSwapingWeapons)
            {
                return;
            }
            _isSwapingWeapons = true;
        }

        private void PerformWeaponSwap()
        {
            if (_secondaryWeaponObject == null)
            {
                return;
            }

            if (_secondaryWeaponObject.activeInHierarchy)
            {
                _secondaryWeaponObject.SetActive(false);
                _mainWeaponObject.SetActive(true);
                Weapon = _mainWeaponObject.GetComponent<Weapon>();
            }
            else
            {
                _mainWeaponObject.SetActive(false);
                _secondaryWeaponObject.SetActive(true);
                Weapon = _secondaryWeaponObject.GetComponent<Weapon>();
            }
            _isSwapingWeapons = false;
            _weaponSwapPassed = 0;
        }
    }
    #endregion
}

