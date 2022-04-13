﻿using Sharky.Builds.BuildingPlacement;

namespace Sharky.Builds
{
    public class BuildOptions
    {
        public bool StrictWorkerCount { get; set; }
        public bool StrictSupplyCount { get; set; }
        public bool EncroachEnemyMainWithExpansions { get; set; }
        public bool StrictGasCount { get; set; }
        public bool StrictWorkersPerGas { get; set; }
        public int StrictWorkersPerGasCount { get; set; }
        public int MaxActiveGasCount { get; set; }
        public ProtossBuildOptions ProtossBuildOptions { get; set; }
        public WallOffType WallOffType { get; set; }

        public BuildOptions()
        {
            StrictWorkerCount = false;
            StrictSupplyCount = false;
            StrictGasCount = false;
            StrictWorkersPerGas = false;
            EncroachEnemyMainWithExpansions = false;
            MaxActiveGasCount = 8;
            StrictWorkersPerGasCount = 3;
            ProtossBuildOptions = new ProtossBuildOptions { PylonsAtDefensivePoint = 0, ShieldsAtDefensivePoint = 0 };
            WallOffType = WallOffType.None;
        }
    }
}
