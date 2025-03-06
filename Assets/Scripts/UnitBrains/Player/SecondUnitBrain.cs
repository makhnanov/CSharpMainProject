using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using static Utilities.Extensions;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;

        private List<Vector2Int> _targetsOutOfRange = new();
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            float currentTemperature = GetTemperature();
            if (currentTemperature >= OverheatTemperature)
            {
                return;
            }

            int projectileCount = 0;
            do
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
                projectileCount++;
            } while (projectileCount <= currentTemperature);
            
            IncreaseTemperature();
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            IEnumerable<Vector2Int> allTargets = GetAllTargets();
            if (HasTargetsInRange() || !allTargets.Any() || !_targetsOutOfRange.Any())
            {
                return unit.Pos;
            }
            
            return unit.Pos.CalcNextStepTowards(_targetsOutOfRange.First());
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            var EnemyBase =
                runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
            
            IEnumerable<Vector2Int> targets = GetAllTargets();
            targets = targets.OrderBy(DistanceToOwnBase);
            
            List<Vector2Int> resultList = new();
            
            Vector2Int theMostDangerous = targets.FirstOrDefault();
            if (theMostDangerous != null)
            {
                if (IsTargetInRange(theMostDangerous))
                {
                    resultList.Add(theMostDangerous);
                    return resultList;
                }
                _targetsOutOfRange.Add(theMostDangerous);
            }
            
            var targetsList = targets.ToList();
            foreach (Vector2Int target in targetsList)
            {
                if (IsTargetInRange(target))
                {
                    resultList.Add(target);
                }
                else
                {
                    _targetsOutOfRange.Add(theMostDangerous);
                }
            }

            return resultList;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}