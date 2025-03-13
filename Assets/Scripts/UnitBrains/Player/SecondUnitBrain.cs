using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

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

        private static int _idCount = 0;
        private int _id = _idCount++;
        private const int SmartTargetNumber = 3;

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            int currentTemperature = GetTemperature();
            if (currentTemperature >= overheatTemperature)
            {
                return;
            }

            for (int i = 0; i <= currentTemperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            IncreaseTemperature();
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            Vector2Int result = new();
            if (_targetsOutOfRange.Count == 0 || IsTargetInRange(_targetsOutOfRange[0]))
            {
                result = unit.Pos;
            }
            else
            {
                result = unit.Pos.CalcNextStepTowards(_targetsOutOfRange[0]);
            }
            _targetsOutOfRange.Clear();
            return result;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> result = new();
            //IEnumerable<Vector2Int> allTargets = GetAllTargets();
            //float minDistance = float.MaxValue;
            //Vector2Int closestTarget = new();
            //foreach (Vector2Int target in allTargets)
            //{
            //    if (DistanceToOwnBase(target) < minDistance)
            //    {
            //        minDistance = DistanceToOwnBase(target);
            //        closestTarget = target;
            //    }
            //}
            //if (MathF.Abs(minDistance - float.MaxValue) > 1e-6)
            //{
            //    if (IsTargetInRange(closestTarget))
            //    {
            //        result.Add(closestTarget);
            //    }
            //    else
            //    {
            //        _targetsOutOfRange.Clear();
            //        _targetsOutOfRange.Add(closestTarget);
            //    }
            //}
            //else
            //{
            //    if (IsTargetInRange(runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]))
            //    {
            //        if (IsPlayerUnitBrain)
            //        {
            //            result.Add(runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
            //        }
            //        else
            //        {
            //            result.Add(runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
            //        }
            //    }
            //    else
            //    {
            //        _targetsOutOfRange.Clear();
            //        if (IsPlayerUnitBrain)
            //        {
            //            _targetsOutOfRange.Add(runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
            //        }
            //        else
            //        {
            //            _targetsOutOfRange.Add(runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
            //        }
            //    }
            //}

            List<Vector2Int> sortedTargets = GetAllTargets().ToList();
            SortByDistanceToOwnBase(sortedTargets);
            Vector2Int target = sortedTargets.Count < SmartTargetNumber ? sortedTargets[_id % sortedTargets.Count] : sortedTargets[_id % SmartTargetNumber];
            if (IsTargetInRange(target))
            {
                result.Clear();
                result.Add(target);
            }
            else
            {
                _targetsOutOfRange.Clear();
                _targetsOutOfRange.Add(target);
            }

            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
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
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}