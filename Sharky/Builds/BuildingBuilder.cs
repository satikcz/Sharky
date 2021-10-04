﻿using SC2APIProtocol;
using Sharky.Builds.BuildingPlacement;
using Sharky.Pathing;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Sharky.Builds
{
    public class BuildingBuilder : IBuildingBuilder
    {
        ActiveUnitData ActiveUnitData;
        TargetingData TargetingData;
        IBuildingPlacement BuildingPlacement;
        SharkyUnitData SharkyUnitData;
        BaseData BaseData;

        BuildingService BuildingService;
        MapDataService MapDataService;

        public BuildingBuilder(ActiveUnitData activeUnitData, TargetingData targetingData, IBuildingPlacement buildingPlacement, SharkyUnitData sharkyUnitData, BaseData baseData, BuildingService buildingService, MapDataService mapDataService)
        {
            ActiveUnitData = activeUnitData;
            TargetingData = targetingData;
            BuildingPlacement = buildingPlacement;
            SharkyUnitData = sharkyUnitData;
            BaseData = baseData;

            BuildingService = buildingService;
            MapDataService = mapDataService;
        }

        public List<Action> BuildBuilding(MacroData macroData, UnitTypes unitType, BuildingTypeData unitData, Point2D generalLocation = null, bool ignoreMineralProximity = false, float maxDistance = 50, List<UnitCommander> workerPool = null, bool requireSameHeight = false, WallOffType wallOffType = WallOffType.None)
        {
            if (unitData.Minerals <= macroData.Minerals && unitData.Gas <= macroData.VespeneGas)
            {
                bool anyBase = false;
                var location = generalLocation;
                if (location == null)
                {
                    anyBase = true;
                    var addOnSwap = macroData.AddOnSwaps.Values.FirstOrDefault(a => a.Started && !a.Completed && a.AddOnBuilder != null && a.AddOnTaker == null && a.DesiredAddOnTaker == unitType);
                    if (addOnSwap != null)
                    {
                        location = new Point2D { X = addOnSwap.AddOnBuilder.UnitCalculation.Position.X, Y = addOnSwap.AddOnBuilder.UnitCalculation.Position.Y };
                    }
                    else
                    {
                        location = GetReferenceLocation(TargetingData.SelfMainBasePoint);
                    }
                }
                var placementLocation = BuildingPlacement.FindPlacement(location, unitType, unitData.Size, ignoreMineralProximity, maxDistance, requireSameHeight, wallOffType);
                
                if (placementLocation == null)
                {
                    placementLocation = BuildingPlacement.FindPlacement(location, unitType, unitData.Size, true, maxDistance, requireSameHeight, wallOffType);
                }
                if (placementLocation == null && anyBase)
                {
                    foreach (var selfBase in BaseData.SelfBases)
                    {
                        placementLocation = BuildingPlacement.FindPlacement(selfBase.Location, unitType, unitData.Size, true, maxDistance, requireSameHeight, wallOffType);
                        if (placementLocation != null)
                        {
                            break;
                        }
                    }
                }
                if (placementLocation != null)
                {
                    var worker = GetWorker(placementLocation, workerPool);
                    if (worker != null)
                    {
                        if (workerPool == null)
                        {
                            worker.UnitRole = UnitRole.Build;
                        }
                        
                        return worker.Order(macroData.Frame, unitData.Ability, placementLocation);
                    }
                }
             }

            return null;
        }

        public List<Action> BuildAddOn(MacroData macroData, TrainingTypeData unitData, Point2D location = null, float maxDistance = 50)
        {
            if (unitData.Minerals <= macroData.Minerals && unitData.Gas <= macroData.VespeneGas)
            {
                var building = ActiveUnitData.Commanders.Where(c => unitData.ProducingUnits.Contains((UnitTypes)c.Value.UnitCalculation.Unit.UnitType) && !c.Value.UnitCalculation.Unit.IsActive && c.Value.UnitCalculation.Unit.BuildProgress == 1 && !c.Value.UnitCalculation.Unit.HasAddOnTag);
                if (building.Count() > 0)
                {
                    if (location != null)
                    {
                        building = building.Where(b => Vector2.DistanceSquared(new Vector2(location.X, location.Y), b.Value.UnitCalculation.Position) <= maxDistance * maxDistance);
                    }
                    if (building.Count() > 0)
                    {
                        // is there room to build the addon?
                        var buildingWithRoom = building.FirstOrDefault(b => HasRoomForAddon(b.Value)).Value;
                        if (buildingWithRoom != null)
                        {
                            var action = building.First().Value.Order(macroData.Frame, unitData.Ability);
                            if (action != null) { return action; }
                        }
                        else
                        {
                            var action = building.First().Value.Order(macroData.Frame, Abilities.LIFT);
                            if (action != null) { return action; }
                        }
                    }
                }
                else
                {
                    var flyingType = UnitTypes.TERRAN_BARRACKSFLYING;
                    if (unitData.ProducingUnits.Contains(UnitTypes.TERRAN_FACTORY)) { flyingType = UnitTypes.TERRAN_FACTORYFLYING; }
                    else if (unitData.ProducingUnits.Contains(UnitTypes.TERRAN_STARPORT)) { flyingType = UnitTypes.TERRAN_STARPORTFLYING; }
                    building = ActiveUnitData.Commanders.Where(c => flyingType == (UnitTypes)c.Value.UnitCalculation.Unit.UnitType);

                    if (location != null)
                    {
                        building = building.Where(b => Vector2.DistanceSquared(new Vector2(location.X, location.Y), b.Value.UnitCalculation.Position) <= maxDistance * maxDistance);
                    }
                    if (building.Count() > 0)
                    {
                        if (location == null)
                        {
                            location = new Point2D { X = building.First().Value.UnitCalculation.Position.X, Y = building.First().Value.UnitCalculation.Position.Y };
                        }
                        var placementLocation = BuildingPlacement.FindPlacement(location, UnitTypes.TERRAN_BARRACKSTECHLAB, 1, true, maxDistance, false, WallOffType.Terran);
                        if (placementLocation != null)
                        {
                            var action = building.First().Value.Order(macroData.Frame, unitData.Ability, placementLocation);
                            if (action != null) { return action; }
                        }
                    }
                }
            }

            return null;
        }

        bool HasRoomForAddon(UnitCommander building)
        {
            var addonY = building.UnitCalculation.Unit.Pos.Y - .5f;
            var addonX = building.UnitCalculation.Unit.Pos.X + 2.5f;
            if (addonX >= 0 && addonY >= 0 && addonX < MapDataService.MapData.MapWidth && addonY < MapDataService.MapData.MapHeight &&
                BuildingService.AreaBuildable(addonX, addonY, .5f) && !BuildingService.Blocked(addonX, addonY, .5f, -.5f))
            {
                return true;
            }
            return false;
        }

        public List<Action> BuildGas(MacroData macroData, BuildingTypeData unitData, Unit geyser)
        {
            if (unitData.Minerals <= macroData.Minerals && unitData.Gas <= macroData.VespeneGas)
            {
                var worker = GetWorker(new Point2D { X = geyser.Pos.X, Y = geyser.Pos.Y });
                if (worker != null)
                {
                    worker.UnitRole = UnitRole.Build;
                    return worker.Order(macroData.Frame, unitData.Ability, null, geyser.Tag);
                }
            }

            return null;
        }

        public Point2D GetReferenceLocation(Point2D buildLocation)
        {
            var nexus = ActiveUnitData.Commanders.Values.Where(c => c.UnitCalculation.UnitClassifications.Contains(UnitClassification.ResourceCenter)).OrderBy(c => Vector2.DistanceSquared(c.UnitCalculation.Position, new Vector2(buildLocation.X, buildLocation.Y))).FirstOrDefault();
            if (nexus != null)
            {
                return new Point2D { X = nexus.UnitCalculation.Unit.Pos.X, Y = nexus.UnitCalculation.Unit.Pos.Y };
            }
            else
            {
                var worker = ActiveUnitData.Commanders.Values.Where(c => c.UnitCalculation.UnitClassifications.Contains(UnitClassification.Worker)).OrderBy(c => c.UnitCalculation.NearbyEnemies.Count()).ThenBy(c => c.UnitCalculation.NearbyAllies.Where(ally => ally.UnitClassifications.Contains(UnitClassification.ArmyUnit)).Count()).FirstOrDefault();
                if (worker != null)
                {
                    return new Point2D { X = worker.UnitCalculation.Unit.Pos.X, Y = worker.UnitCalculation.Unit.Pos.Y };
                }
            }

            return buildLocation;
        }

        private UnitCommander GetWorker(Point2D location, IEnumerable<UnitCommander> workers = null)
        {
            IEnumerable<UnitCommander> availableWorkers;
            if (workers == null)
            {
                availableWorkers = ActiveUnitData.Commanders.Values.Where(c => c.UnitRole == UnitRole.Build && c.UnitCalculation.Unit.UnitType == (uint)UnitTypes.TERRAN_SCV && c.UnitCalculation.Unit.Orders.Any(o => ActiveUnitData.SelfUnits.Values.Any(s => s.Attributes.Contains(Attribute.Structure) && s.Unit.BuildProgress == 1 && o.TargetWorldSpacePos != null && s.Position.X == o.TargetWorldSpacePos.X && s.Position.Y == o.TargetWorldSpacePos.Y))).Concat(
                    ActiveUnitData.Commanders.Values.Where(c => c.UnitCalculation.UnitClassifications.Contains(UnitClassification.Worker) && !c.UnitCalculation.Unit.BuffIds.Any(b => SharkyUnitData.CarryingResourceBuffs.Contains((Buffs)b))).Where(c => (c.UnitRole == UnitRole.PreBuild || c.UnitRole == UnitRole.None || c.UnitRole == UnitRole.Minerals) && !c.UnitCalculation.Unit.Orders.Any(o => SharkyUnitData.BuildingData.Values.Any(b => (uint)b.Ability == o.AbilityId))))
                    .OrderBy(p => Vector2.DistanceSquared(p.UnitCalculation.Position, new Vector2(location.X, location.Y)));
            }
            else
            {
                availableWorkers = workers.Where(c => !c.UnitCalculation.Unit.Orders.Any(o => SharkyUnitData.BuildingData.Values.Any(b => (uint)b.Ability == o.AbilityId))).OrderBy(p => Vector2.DistanceSquared(p.UnitCalculation.Position, new Vector2(location.X, location.Y)));
            }

            if (availableWorkers.Count() == 0)
            {
                return null;
            }
            else
            {
                var closest = availableWorkers.First();
                var pos = closest.UnitCalculation.Position;
                var distanceSquared = Vector2.DistanceSquared(pos, new Vector2(location.X, location.Y));
                if (distanceSquared > 1000)
                {
                    pos = availableWorkers.First().UnitCalculation.Position;

                    if (Vector2.DistanceSquared(new Vector2(pos.X, pos.Y), new Vector2(location.X, location.Y)) > distanceSquared)
                    {
                        return closest;
                    }
                    else
                    {
                        return availableWorkers.First();
                    }
                }
            }
            return availableWorkers.First();
        }
    }
}
