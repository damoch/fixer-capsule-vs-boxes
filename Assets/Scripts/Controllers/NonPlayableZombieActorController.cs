using Assets.Scripts.Actors;
using Assets.Scripts.Enums;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(Actor))]
    public class NonPlayableZombieActorController : MonoBehaviour
    {
        [SerializeField]
        private Actor _actor;

        [SerializeField]
        private Team _team;

        private Actor _target;

        private void Start()
        {
            _actor = GetComponent<Actor>();
            _actor.MoveTowards(Vector2.zero);
            _actor.Team = _team;
        }

        private void Update()
        {
            if (!_actor.IsAlive)
            {
                return;
            }

            if (!_actor.IsSelected)
            {
                _actor.IsSelected = true;
            }

            if (_target == null)
            {
                return;
            }

            if (!_target.IsAlive)
            {
                _target = null;
                _actor.MoveTowards(Vector2.zero);
                return;
            }

            _actor.LookAt(_target.transform.position);
            _actor.MoveTowards(_target.transform.position);

            if(_actor.Weapon.IsAttackPossible(_target.transform.position))
            {
                _actor.GetCommand(Commands.Shoot);
            }
        }

        internal void NotifyAboutNewActorInTheRoom(Actor actor)
        {
            if(actor.Team != _team)
            {
                _target = actor;
            }
        }
    }
}
