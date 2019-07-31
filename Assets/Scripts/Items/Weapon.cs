using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class Weapon : Item
    {
        #region Private variables
        [SerializeField]
        private GameObject _ammunitionObject;

        [SerializeField]
        private Ammunition _ammunition;

        [SerializeField]
        private float _cooldownTimeInSeconds;

        [SerializeField]
        private int _nuberOfRounds;

        [SerializeField]
        private int _magazineCapacity;

        [SerializeField]
        private float _reloadLength;

        [SerializeField]
        private bool _hasRecoil;

        [SerializeField]
        private float _recoilValueEveryShoot;

        [SerializeField]
        private float _decreaseRecoilTime;

        private int _currentMagzineRounds;
        private float _elapsedCooldownSeconds;
        private float _elapsedReloadSeconds;
        private bool _isCoolingDown;
        private bool _isReloading;
        private float _currentRecoilValue;
        private float _recoilCooldownTimer;
        #endregion

        #region Accessors
        public Ammunition Ammunition
        {
            get
            {
                return _ammunition;
            }

            set
            {
                _ammunition = value;
            }
        }

        public float CooldownTime
        {
            get
            {
                return _cooldownTimeInSeconds;
            }

            set
            {
                _cooldownTimeInSeconds = value;
            }
        }
        #endregion

        #region Methods
        private void Start()
        {
            if(_ammunition == null)
            {
                _ammunition = _ammunitionObject.GetComponent<Ammunition>();
            }

            _elapsedCooldownSeconds = 0;
            _isCoolingDown = false;
            ReloadWeapon();
        }

        public void Shoot(Quaternion direction)
        {
            if (_isCoolingDown || _isReloading || _currentMagzineRounds < 1)
            {
                return;
            }
            Quaternion newDirection;

            if (_hasRecoil)
            {
                var randomizer = Random.Range(0, 100) > 50 ? 1 : -1;
                newDirection = Quaternion.Euler(direction.eulerAngles.x, direction.eulerAngles.y, direction.eulerAngles.z + (_currentRecoilValue * randomizer));
                _currentRecoilValue += _recoilValueEveryShoot;
            }
            else
            {
                newDirection = direction;
            }

            Instantiate(_ammunitionObject, transform.position, newDirection);//change to pooling later on
            _isCoolingDown = true;
            if(_ammunition.AmmunitionType == AmmunitionType.Meele)
            {
                return;
            }
            _currentMagzineRounds--;
            _isReloading = _currentMagzineRounds < 1;
            _recoilCooldownTimer = 0;
        }

        private void Update()
        {
            if (_isCoolingDown)
            {
                _elapsedCooldownSeconds += Time.deltaTime;

                if(_elapsedCooldownSeconds >= _cooldownTimeInSeconds)
                {
                    _elapsedCooldownSeconds = 0;
                    _isCoolingDown = false;
                }
            }

            if (_isReloading)
            {
                _elapsedReloadSeconds += Time.deltaTime;

                if(_elapsedReloadSeconds >= _reloadLength)
                {
                    ReloadWeapon();
                }
            }

            if(_hasRecoil && _currentRecoilValue > 0)
            {
                _recoilCooldownTimer += Time.deltaTime;
                _currentRecoilValue -= (_recoilCooldownTimer / _decreaseRecoilTime) * _currentRecoilValue;

                if (_recoilCooldownTimer >= _decreaseRecoilTime)
                {
                    _currentRecoilValue = 0;
                }
            }
        }

        private void ReloadWeapon()
        {
            _elapsedReloadSeconds = 0;
            _isReloading = false;

            if(_magazineCapacity > _nuberOfRounds)
            {
                _currentMagzineRounds = _nuberOfRounds;
                _nuberOfRounds = 0;
                return;
            }
            _nuberOfRounds -= _magazineCapacity;
            _currentMagzineRounds = _magazineCapacity;
        }

        public bool IsAttackPossible(Vector2 targetPosition)
        {
            if(_ammunition.AmmunitionType == AmmunitionType.Projectile)
            {
                return true;
            }

            return Vector2.Distance(transform.position, targetPosition) <= _ammunition.MinimumAttackDistance;
        }
        #endregion
    }
}
