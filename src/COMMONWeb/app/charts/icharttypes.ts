import { CPUData } from '../classes/cpu';
import { DatabaseHistory } from '../classes/database';
import { Memory } from '../classes/memory';
import { NetworkStatus } from '../classes/network';
import { NICData } from '../classes/nic';
import { ProcessHistory } from '../classes/processes';
import { DiskUsage, DiskPerformance } from '../disk/disk';
import { UPSStatus } from '../classes/ups';

export type IChartTypes =
    CPUData |
    DatabaseHistory |
    Memory |
    NetworkStatus |
    NICData |
    ProcessHistory |
    DiskUsage |
    DiskPerformance |
    UPSStatus;
