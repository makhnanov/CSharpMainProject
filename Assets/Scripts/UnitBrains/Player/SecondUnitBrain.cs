﻿using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;

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
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> targets = GetReachableTargets();
            Vector2Int? lowestTarget = null;
            float lowest = float.MinValue;
            
            foreach (Vector2Int target in targets)
            {
                float distanceToOwnBase = DistanceToOwnBase(target);
                if (distanceToOwnBase > lowest)
                {
                    lowest = distanceToOwnBase;
                    lowestTarget = target;
                }
            }

            if (lowestTarget == null)
            {
                return targets.Count > 0
                    ? new List<Vector2Int> { targets[0] }
                    : new List<Vector2Int>();
            }

            targets.Clear();
            targets.Add((Vector2Int)lowestTarget);
            return targets;
            ///////////////////////////////////////
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