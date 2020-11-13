﻿using SC2APIProtocol;
using Sharky.Managers;
using Sharky.Managers.Protoss;
using System.Collections.Generic;

namespace Sharky.Builds.Protoss
{
    public class ProtossRobo : ProtossSharkyBuild
    {
        SharkyOptions SharkyOptions;

        public ProtossRobo(BuildOptions buildOptions, MacroData macroData, UnitManager unitManager, AttackData attackData, IChatManager chatManager, NexusManager nexusManager, SharkyOptions sharkyOptions) : base(buildOptions, macroData, unitManager, attackData, chatManager, nexusManager)
        {
            SharkyOptions = sharkyOptions;
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            NexusManager.ChronodUpgrades = new HashSet<Upgrades>
            {
                Upgrades.WARPGATERESEARCH
            };

            NexusManager.ChronodUnits = new HashSet<UnitTypes>
            {
                UnitTypes.PROTOSS_IMMORTAL,
            };

            MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_NEXUS] = 1;

            // TODO: be able to add MicroTasks
            //if (EnemyRace == SC2APIProtocol.Race.Protoss)
            //{
            //    var defenseTask = new DefenseSquadTask(MacroData.Main, UnitTypes.PROTOSS_STALKER)
            //}
            //else
            //{
            //    var defenseTask = new DefenseSquadTask(MacroData.Main, UnitTypes.ADEPT)
            //}
            // TODO: EnemyRace
        }

        public override void OnFrame(ResponseObservation observation)
        {
            if (UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) > 0)
            {
                // TODO: MacroData.ShieldsAtEveryExpansion = 2;
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_STALKER] < 4)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_STALKER] = 4;
                }
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_ZEALOT] < 2)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_ZEALOT] = 2;
                }
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_SENTRY] < 1)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_SENTRY] = 1;
                }
            }

            if (UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) >= 2)
            {
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_WARPPRISM] < 1)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_WARPPRISM] = 1;
                }
                if (UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) >= 3 && MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_OBSERVER] < 1)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_OBSERVER] = 1;
                }
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_ROBOTICSFACILITY) > 0)
            {
                MacroData.DesiredGases = 3;
                MacroData.DesiredUpgrades[Upgrades.WARPGATERESEARCH] = true;

                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_IMMORTAL] < 10)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_IMMORTAL] = 10;
                }
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_STALKER] < UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) * 3)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_STALKER] = UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) * 3;
                }
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_ZEALOT] < UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL))
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_ZEALOT] = UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL);
                }
            }

            if (MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_NEXUS] < 2) // start expanding
            {
                MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_NEXUS] = 2;
            }

            if (MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_ROBOTICSFACILITY] < 1)
            {
                MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_ROBOTICSFACILITY] = 1;
            }

            if (MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_ROBOTICSFACILITY] < 2 && UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) > 0)
            {
                MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_ROBOTICSFACILITY] = 2;
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_ROBOTICSFACILITY) >= 2)
            {
                BuildOptions.StrictGasCount = false;
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_ROBOTICSFACILITY) >= 2 && MacroData.DesiredTechCounts[UnitTypes.PROTOSS_FORGE] < 1)
            {
                MacroData.DesiredTechCounts[UnitTypes.PROTOSS_FORGE] = 1;
            }

            // TODO: EnemyManager get EnemyRace
            //if (EnemyRace == SC2APIProtocol.Race.Terran)
            //{
            //    if (UnitManager.Completed(UnitTypes.PROTOSS_ROBOTICSFACILITY) > 0 && MacroData.DesiredTechCounts[UnitTypes.PROTOSS_FORGE] < 1)
            //    {
            //        MacroData.DesiredTechCounts[UnitTypes.PROTOSS_FORGE] = 1;
            //    }
            //}

            if (UnitManager.Completed(UnitTypes.PROTOSS_ROBOTICSFACILITY) >= 2 && UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) > 3)
            {
                if (MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_NEXUS] < 3)
                {
                    MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_NEXUS] = 3;
                }
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_NEXUS) > 2 && UnitManager.Completed(UnitTypes.PROTOSS_FORGE) > 0)
            {
                if (MacroData.DesiredTechCounts[UnitTypes.PROTOSS_TWILIGHTCOUNCIL] < 1)
                {
                    MacroData.DesiredTechCounts[UnitTypes.PROTOSS_TWILIGHTCOUNCIL] = 1;
                }
                MacroData.DesiredUpgrades[Upgrades.BLINKTECH] = true;
                NexusManager.ChronodUpgrades.Add(Upgrades.BLINKTECH);
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_NEXUS) > 2 && UnitManager.Completed(UnitTypes.PROTOSS_TWILIGHTCOUNCIL) > 0)
            {
                if (MacroData.DesiredTechCounts[UnitTypes.PROTOSS_FORGE] < 2)
                {
                    MacroData.DesiredTechCounts[UnitTypes.PROTOSS_FORGE] = 2;
                }
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_TWILIGHTCOUNCIL) > 0 && UnitManager.Completed(UnitTypes.PROTOSS_FORGE) >= 2)
            {
                if (MacroData.DesiredTechCounts[UnitTypes.PROTOSS_TEMPLARARCHIVE] < 1)
                {
                    MacroData.DesiredTechCounts[UnitTypes.PROTOSS_TEMPLARARCHIVE] = 1;
                }
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_TEMPLARARCHIVE) > 0)
            {
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_HIGHTEMPLAR] < 2)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_HIGHTEMPLAR] = 2;
                }
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_ARCHON] < 3)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_ARCHON] = 3;
                }
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_FORGE) > 0)
            {
                MacroData.DesiredUpgrades[Upgrades.PROTOSSGROUNDWEAPONSLEVEL1] = true;
                NexusManager.ChronodUpgrades.Add(Upgrades.PROTOSSGROUNDWEAPONSLEVEL1);
            }
            if (UnitManager.Completed(UnitTypes.PROTOSS_FORGE) > 1)
            {
                MacroData.DesiredUpgrades[Upgrades.PROTOSSGROUNDARMORSLEVEL1] = true;
                NexusManager.ChronodUpgrades.Add(Upgrades.PROTOSSGROUNDARMORSLEVEL1);
            }

            if (observation.Observation.GameLoop > SharkyOptions.FramesPerSecond * 15 * 60 || MacroData.FoodUsed > 125)
            {
                if (MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_STARGATE] < 1)
                {
                    MacroData.DesiredProductionCounts[UnitTypes.PROTOSS_STARGATE] = 1;
                }
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_ORACLE] < 1)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_ORACLE] = 1;
                }
                if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_VOIDRAY] < 1)
                {
                    MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_VOIDRAY] = 1;
                }
            }

            if (MacroData.DesiredTechCounts[UnitTypes.PROTOSS_ROBOTICSBAY] < 1 && UnitManager.Completed(UnitTypes.PROTOSS_ROBOTICSFACILITY) > 1 && UnitManager.Completed(UnitTypes.PROTOSS_NEXUS) > 2)
            {
                MacroData.DesiredTechCounts[UnitTypes.PROTOSS_ROBOTICSBAY] = 1;
            }

            if (UnitManager.Completed(UnitTypes.PROTOSS_ROBOTICSBAY) > 0)
            {
                if (UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) > 3)
                {
                    if (MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_DISRUPTOR] < 3)
                    {
                        MacroData.DesiredUnitCounts[UnitTypes.PROTOSS_DISRUPTOR] = 3;
                    }
                }

                MacroData.DesiredUpgrades[Upgrades.GRAVITICDRIVE] = true;
                NexusManager.ChronodUpgrades.Add(Upgrades.GRAVITICDRIVE);
            }

            // TODO: ProductionBalancer
            //if (UnitManager.Completed(UnitTypes.PROTOSS_NEXUS) > 3)
            //{
            //    ProductionBalancer.BalanceProductionCapacity();
            //    ProductionBalancer.BalanceUnitCounterProduction();
            //    ProductionBalancer.ProduceUntilCapped();
            //    ProductionBalancer.ResearchNeededUpgrades();
            //    ProductionBalancer.ExpandForever();
            //}

            //if (UnitManager.Count(UnitTypes.PROTOSS_IMMORTAL) > 4)
            //{
            //    ProductionBalancer.ExpandForever();
            //}
        }
    }
}