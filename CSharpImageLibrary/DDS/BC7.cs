﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpImageLibrary.DDS
{
    internal static class BC7
    {
        #region Tables
        // 3,64,16
        static byte[][][] PartitionTable = new byte[3][][]
        {   // 1 Region case has no subsets (all 0)
            new byte[64][]
            {
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            },

            new byte[64][]
            {   // BC6H/BC7 Partition Set for 2 Subsets
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1 }, // Shape 0
                new byte[16] { 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1 }, // Shape 1
                new byte[16] { 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1 }, // Shape 2
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1 }, // Shape 3
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1 }, // Shape 4
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1 }, // Shape 5
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1 }, // Shape 6
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1 }, // Shape 7
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1 }, // Shape 8
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 9
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1 }, // Shape 10
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1 }, // Shape 11
                new byte[16] { 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 12
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 13
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 14
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 }, // Shape 15
                new byte[16] { 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1 }, // Shape 16
                new byte[16] { 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }, // Shape 17
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0 }, // Shape 18
                new byte[16] { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0 }, // Shape 19
                new byte[16] { 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }, // Shape 20
                new byte[16] { 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0 }, // Shape 21
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0 }, // Shape 22
                new byte[16] { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1 }, // Shape 23
                new byte[16] { 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0 }, // Shape 24
                new byte[16] { 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0 }, // Shape 25
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0 }, // Shape 26
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 0 }, // Shape 27
                new byte[16] { 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0 }, // Shape 28
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 }, // Shape 29
                new byte[16] { 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0 }, // Shape 30
                new byte[16] { 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0 }, // Shape 31

                                                                // BC7 Partition Set for 2 Subsets (second-half)
                new byte[16] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 }, // Shape 32
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 }, // Shape 33
                new byte[16] { 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0 }, // Shape 34
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0 }, // Shape 35
                new byte[16] { 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0 }, // Shape 36
                new byte[16] { 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0 }, // Shape 37
                new byte[16] { 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1 }, // Shape 38
                new byte[16] { 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1 }, // Shape 39
                new byte[16] { 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0 }, // Shape 40
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0 }, // Shape 41
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 0 }, // Shape 42
                new byte[16] { 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0 }, // Shape 43
                new byte[16] { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 }, // Shape 44
                new byte[16] { 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1 }, // Shape 45
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1 }, // Shape 46
                new byte[16] { 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0 }, // Shape 47
                new byte[16] { 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 }, // Shape 48
                new byte[16] { 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0 }, // Shape 49
                new byte[16] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0 }, // Shape 50
                new byte[16] { 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0 }, // Shape 51
                new byte[16] { 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1 }, // Shape 52
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1 }, // Shape 53
                new byte[16] { 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0 }, // Shape 54
                new byte[16] { 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0 }, // Shape 55
                new byte[16] { 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1 }, // Shape 56
                new byte[16] { 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1 }, // Shape 57
                new byte[16] { 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1 }, // Shape 58
                new byte[16] { 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1 }, // Shape 59
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1 }, // Shape 60
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 }, // Shape 61
                new byte[16] { 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0 }, // Shape 62
                new byte[16] { 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1 }  // Shape 63
            },

            new byte[64][]
            {   // BC7 Partition Set for 3 Subsets
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 2, 2, 1, 2, 2, 2, 2 }, // Shape 0
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 2, 2, 1, 1, 2, 2, 2, 1 }, // Shape 1
                new byte[16] { 0, 0, 0, 0, 2, 0, 0, 1, 2, 2, 1, 1, 2, 2, 1, 1 }, // Shape 2
                new byte[16] { 0, 2, 2, 2, 0, 0, 2, 2, 0, 0, 1, 1, 0, 1, 1, 1 }, // Shape 3
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 1, 1, 2, 2 }, // Shape 4
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 2, 2, 0, 0, 2, 2 }, // Shape 5
                new byte[16] { 0, 0, 2, 2, 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 6
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1 }, // Shape 7
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2 }, // Shape 8
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2 }, // Shape 9
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2 }, // Shape 10
                new byte[16] { 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 1, 2 }, // Shape 11
                new byte[16] { 0, 1, 1, 2, 0, 1, 1, 2, 0, 1, 1, 2, 0, 1, 1, 2 }, // Shape 12
                new byte[16] { 0, 1, 2, 2, 0, 1, 2, 2, 0, 1, 2, 2, 0, 1, 2, 2 }, // Shape 13
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 2, 1, 1, 2, 2, 1, 2, 2, 2 }, // Shape 14
                new byte[16] { 0, 0, 1, 1, 2, 0, 0, 1, 2, 2, 0, 0, 2, 2, 2, 0 }, // Shape 15
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 2, 1, 1, 2, 2 }, // Shape 16
                new byte[16] { 0, 1, 1, 1, 0, 0, 1, 1, 2, 0, 0, 1, 2, 2, 0, 0 }, // Shape 17
                new byte[16] { 0, 0, 0, 0, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 2, 2 }, // Shape 18
                new byte[16] { 0, 0, 2, 2, 0, 0, 2, 2, 0, 0, 2, 2, 1, 1, 1, 1 }, // Shape 19
                new byte[16] { 0, 1, 1, 1, 0, 1, 1, 1, 0, 2, 2, 2, 0, 2, 2, 2 }, // Shape 20
                new byte[16] { 0, 0, 0, 1, 0, 0, 0, 1, 2, 2, 2, 1, 2, 2, 2, 1 }, // Shape 21
                new byte[16] { 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 2, 2, 0, 1, 2, 2 }, // Shape 22
                new byte[16] { 0, 0, 0, 0, 1, 1, 0, 0, 2, 2, 1, 0, 2, 2, 1, 0 }, // Shape 23
                new byte[16] { 0, 1, 2, 2, 0, 1, 2, 2, 0, 0, 1, 1, 0, 0, 0, 0 }, // Shape 24
                new byte[16] { 0, 0, 1, 2, 0, 0, 1, 2, 1, 1, 2, 2, 2, 2, 2, 2 }, // Shape 25
                new byte[16] { 0, 1, 1, 0, 1, 2, 2, 1, 1, 2, 2, 1, 0, 1, 1, 0 }, // Shape 26
                new byte[16] { 0, 0, 0, 0, 0, 1, 1, 0, 1, 2, 2, 1, 1, 2, 2, 1 }, // Shape 27
                new byte[16] { 0, 0, 2, 2, 1, 1, 0, 2, 1, 1, 0, 2, 0, 0, 2, 2 }, // Shape 28
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 2, 0, 0, 2, 2, 2, 2, 2 }, // Shape 29
                new byte[16] { 0, 0, 1, 1, 0, 1, 2, 2, 0, 1, 2, 2, 0, 0, 1, 1 }, // Shape 30
                new byte[16] { 0, 0, 0, 0, 2, 0, 0, 0, 2, 2, 1, 1, 2, 2, 2, 1 }, // Shape 31
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 2, 2, 1, 2, 2, 2 }, // Shape 32
                new byte[16] { 0, 2, 2, 2, 0, 0, 2, 2, 0, 0, 1, 2, 0, 0, 1, 1 }, // Shape 33
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 2, 0, 0, 2, 2, 0, 2, 2, 2 }, // Shape 34
                new byte[16] { 0, 1, 2, 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 1, 2, 0 }, // Shape 35
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0 }, // Shape 36
                new byte[16] { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0 }, // Shape 37
                new byte[16] { 0, 1, 2, 0, 2, 0, 1, 2, 1, 2, 0, 1, 0, 1, 2, 0 }, // Shape 38
                new byte[16] { 0, 0, 1, 1, 2, 2, 0, 0, 1, 1, 2, 2, 0, 0, 1, 1 }, // Shape 39
                new byte[16] { 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0, 1, 1 }, // Shape 40
                new byte[16] { 0, 1, 0, 1, 0, 1, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2 }, // Shape 41
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 2, 1, 2, 1, 2, 1 }, // Shape 42
                new byte[16] { 0, 0, 2, 2, 1, 1, 2, 2, 0, 0, 2, 2, 1, 1, 2, 2 }, // Shape 43
                new byte[16] { 0, 0, 2, 2, 0, 0, 1, 1, 0, 0, 2, 2, 0, 0, 1, 1 }, // Shape 44
                new byte[16] { 0, 2, 2, 0, 1, 2, 2, 1, 0, 2, 2, 0, 1, 2, 2, 1 }, // Shape 45
                new byte[16] { 0, 1, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 0, 1 }, // Shape 46
                new byte[16] { 0, 0, 0, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 }, // Shape 47
                new byte[16] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 2, 2, 2, 2 }, // Shape 48
                new byte[16] { 0, 2, 2, 2, 0, 1, 1, 1, 0, 2, 2, 2, 0, 1, 1, 1 }, // Shape 49
                new byte[16] { 0, 0, 0, 2, 1, 1, 1, 2, 0, 0, 0, 2, 1, 1, 1, 2 }, // Shape 50
                new byte[16] { 0, 0, 0, 0, 2, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 2 }, // Shape 51
                new byte[16] { 0, 2, 2, 2, 0, 1, 1, 1, 0, 1, 1, 1, 0, 2, 2, 2 }, // Shape 52
                new byte[16] { 0, 0, 0, 2, 1, 1, 1, 2, 1, 1, 1, 2, 0, 0, 0, 2 }, // Shape 53
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 2, 2, 2, 2 }, // Shape 54
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 2, 2, 1, 1, 2 }, // Shape 55
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2 }, // Shape 56
                new byte[16] { 0, 0, 2, 2, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 2, 2 }, // Shape 57
                new byte[16] { 0, 0, 2, 2, 1, 1, 2, 2, 1, 1, 2, 2, 0, 0, 2, 2 }, // Shape 58
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 2 }, // Shape 59
                new byte[16] { 0, 0, 0, 2, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 1 }, // Shape 60
                new byte[16] { 0, 2, 2, 2, 1, 2, 2, 2, 0, 2, 2, 2, 1, 2, 2, 2 }, // Shape 61
                new byte[16] { 0, 1, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, // Shape 62
                new byte[16] { 0, 1, 1, 1, 2, 0, 1, 1, 2, 2, 0, 1, 2, 2, 2, 0 }  // Shape 63
            }
        };  
        
        
        // 3,64,3
        static byte[][][] FixUpTable = new byte[3][][]  
        {
            new byte[64][]
            {   // No fix-ups for 1st subset for BC6H or BC7
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }
            },

            new byte[64][]
            {   // BC6H/BC7 Partition Set Fixups for 2 Subsets
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 2, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 },
                new byte[3] { 0, 8, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 },
                
                 // BC7 Partition Set Fixups for 2 Subsets (second-half)
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0, 6, 0 }, new byte[3] { 0, 8, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0, 6, 0 },
                new byte[3] { 0, 6, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 6, 0 }, new byte[3] { 0, 8, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0,15, 0 }
            },

            new byte[64][]
            {   // BC7 Partition Set Fixups for 3 Subsets
                new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 }, new byte[3] { 0,15, 8 }, new byte[3] { 0,15, 3 },
                new byte[3] { 0, 8,15 }, new byte[3] { 0, 3,15 }, new byte[3] { 0,15, 3 }, new byte[3] { 0,15, 8 },
                new byte[3] { 0, 8,15 }, new byte[3] { 0, 8,15 }, new byte[3] { 0, 6,15 }, new byte[3] { 0, 6,15 },
                new byte[3] { 0, 6,15 }, new byte[3] { 0, 5,15 }, new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 },
                new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 }, new byte[3] { 0, 8,15 }, new byte[3] { 0,15, 3 },
                new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 }, new byte[3] { 0, 6,15 }, new byte[3] { 0,10, 8 },
                new byte[3] { 0, 5, 3 }, new byte[3] { 0, 8,15 }, new byte[3] { 0, 8, 6 }, new byte[3] { 0, 6,10 },
                new byte[3] { 0, 8,15 }, new byte[3] { 0, 5,15 }, new byte[3] { 0,15,10 }, new byte[3] { 0,15, 8 },
                new byte[3] { 0, 8,15 }, new byte[3] { 0,15, 3 }, new byte[3] { 0, 3,15 }, new byte[3] { 0, 5,10 },
                new byte[3] { 0, 6,10 }, new byte[3] { 0,10, 8 }, new byte[3] { 0, 8, 9 }, new byte[3] { 0,15,10 },
                new byte[3] { 0,15, 6 }, new byte[3] { 0, 3,15 }, new byte[3] { 0,15, 8 }, new byte[3] { 0, 5,15 },
                new byte[3] { 0,15, 3 }, new byte[3] { 0,15, 6 }, new byte[3] { 0,15, 6 }, new byte[3] { 0,15, 8 },
                new byte[3] { 0, 3,15 }, new byte[3] { 0,15, 3 }, new byte[3] { 0, 5,15 }, new byte[3] { 0, 5,15 },
                new byte[3] { 0, 5,15 }, new byte[3] { 0, 8,15 }, new byte[3] { 0, 5,15 }, new byte[3] { 0,10,15 },
                new byte[3] { 0, 5,15 }, new byte[3] { 0,10,15 }, new byte[3] { 0, 8,15 }, new byte[3] { 0,13,15 },
                new byte[3] { 0,15, 3 }, new byte[3] { 0,12,15 }, new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 }
            }
        };

        static int[] AWeights2 = new int[] { 0, 21, 43, 64 };
        static int[] AWeights3 = new int[] { 0, 9, 18, 27, 37, 46, 55, 64 };
        static int[] AWeights4 = new int[] { 0, 4, 9, 13, 17, 21, 26, 30, 34, 38, 43, 47, 51, 55, 60, 64 };
        #endregion Tables

        #region Structs
        struct Mode
        {
            public readonly int Partitions;
            public readonly int PartitionBits;
            public readonly int IndexPrecision;
            public readonly LDRColour RawRGBPrecision;
            public readonly LDRColour RGBPrecisionWithP;
            public readonly int APrecision;
            public readonly int PBits;
            public readonly int RotationBits;
            public readonly int IndexModeBits;

            public Mode(int partitions, int partitionBits, int IndexPrecision, LDRColour rawrgbPrecision, LDRColour RGBPrecisionWithP, int APrecision, int PBits, int RotationBits, int IndexModeBits)
            {
                this.Partitions = partitions;
                this.PartitionBits = partitionBits;
                this.IndexPrecision = IndexPrecision;
                this.RawRGBPrecision = rawrgbPrecision;
                this.RGBPrecisionWithP = RGBPrecisionWithP;
                this.APrecision = APrecision;
                this.PBits = PBits;
                this.RotationBits = RotationBits;
                this.IndexModeBits = IndexModeBits;
            }
        }

        


        public struct LDRColour
        {
            public int R;
            public int G;
            public int B;
            public int A;

            public LDRColour(byte R, byte G, byte B, byte A)
            {
                this.R = R;
                this.B = B;
                this.G = G;
                this.A = A;
            }

            public override string ToString()
            {
                return $"R: {R}, G: {G}, B: {B}, A: {A}";
            }
        }

        public struct HDRColour
        {
            public float R, G, B, A;

            public HDRColour(LDRColour colour)
            {
                R = colour.R * 1f / 255f;
                G = colour.G * 1f / 255f;
                B = colour.B * 1f / 255f;
                A = colour.A * 1f / 255f;
            }

            public override string ToString()
            {
                return $"R: {R}, G: {G}, B: {B}, A: {A}";
            }
        }
        #endregion Structs

        #region Constants
        const int BC67_WEIGHT_MAX = 64;
        const int BC67_WEIGHT_ROUND = 32;
        const int BC67_WEIGHT_SHIFT = 6;
        const int NUM_PIXELS_PER_BLOCK = 16;
        const int BC7_MAX_REGIONS = 3;
        #endregion Constants

        // Mode: Partitions, partitionBits, indexPrecision, rgbPrecision, rgbPrecisionWithP, APrecision, PBits, Rotation, IndexMode
        static Mode[] Modes = new Mode[] {
            /* Mode 0: */ new Mode(2, 4, 3, new LDRColour(4, 4, 4, 0), new LDRColour(5, 5, 5, 0), 0, 6, 0, 0),
            /* Mode 1: */ new Mode(1, 6, 3, new LDRColour(6, 6, 6, 0), new LDRColour(7, 7, 7, 0), 0, 2, 0, 0),
            /* Mode 2: */ new Mode(2, 6, 2, new LDRColour(5, 5, 5, 0), new LDRColour(5, 5, 5, 0), 0, 0, 0, 0),
            /* Mode 3: */ new Mode(1, 6, 2, new LDRColour(7, 7, 7, 0), new LDRColour(8, 8, 8, 0), 0, 4, 0, 0),
            /* Mode 4: */ new Mode(0, 0, 2, new LDRColour(5, 5, 5, 6), new LDRColour(5, 5, 5, 6), 3, 0, 2, 1),
            /* Mode 5: */ new Mode(0, 0, 2, new LDRColour(7, 7, 7, 8), new LDRColour(7, 7, 7, 8), 2, 0, 2, 0),
            /* Mode 6: */ new Mode(0, 0, 4, new LDRColour(7, 7, 7, 7), new LDRColour(8, 8, 8, 8), 0, 2, 0, 0),
            /* Mode 7: */ new Mode(1, 6, 2, new LDRColour(5, 5, 5, 5), new LDRColour(6, 6, 6, 6), 0, 4, 0, 0),
        };


        public static LDRColour[] DecompressBC7(byte[] source, int sourceStart)
        {
            int start = 0;
            while (start < 128 && GetBit(source, sourceStart, ref start) == 0) { }
            int mode = start - 1;

            var outColours = new LDRColour[NUM_PIXELS_PER_BLOCK];

            if (mode < 8)
            {
                int partitions = Modes[mode].Partitions;
                int numEndPoints = (partitions + 1) << 1;
                int indexPrecision = Modes[mode].IndexPrecision;
                int APrecision = Modes[mode].APrecision;

                int[] P = new int[6];
                int shape = GetBits(source, sourceStart, ref start, Modes[mode].PartitionBits);
                int rotation = GetBits(source, sourceStart, ref start, Modes[mode].RotationBits);
                int indexMode = GetBits(source, sourceStart, ref start, Modes[mode].IndexModeBits);

                LDRColour[] c = new LDRColour[6];
                LDRColour RGBPrecision = Modes[mode].RawRGBPrecision;
                LDRColour RGBPrecisionWithP = Modes[mode].RGBPrecisionWithP;

                // Red
                for(int i = 0; i < numEndPoints; i++)
                {
                    if (start + RGBPrecision.R > 128)
                        Debugger.Break();  // Error

                    c[i].R = GetBits(source, sourceStart, ref start, RGBPrecision.R);
                }

                // Green
                for (int i = 0; i < numEndPoints; i++)
                {
                    if (start + RGBPrecision.G > 128)
                        Debugger.Break();  // Error

                    c[i].G = GetBits(source, sourceStart, ref start, RGBPrecision.G);
                }

                // Blue
                for (int i = 0; i < numEndPoints; i++)
                {
                    if (start + RGBPrecision.B > 128)
                        Debugger.Break();  // Error

                    c[i].B = GetBits(source, sourceStart, ref start, RGBPrecision.B);
                }

                // Alpha
                for (int i = 0; i < numEndPoints; i++)
                {
                    if (start + RGBPrecision.A > 128)
                        Debugger.Break();  // Error

                    c[i].A = RGBPrecision.A == 0 ? 255 : GetBits(source, sourceStart, ref start, RGBPrecision.A);
                }

                // P Bits
                for (int i = 0; i < Modes[mode].PBits; i++)
                {
                    if (start > 127)
                    {
                        Debugger.Break();
                        // Error
                    }

                    P[i] = GetBit(source, sourceStart, ref start);
                }


                // Adjust for P bits
                if (Modes[mode].PBits != 0)
                {
                    for (int i = 0; i < numEndPoints; i++)
                    {
                        int pi = i * Modes[mode].PBits / numEndPoints;
                        if (RGBPrecision.R != RGBPrecisionWithP.R)
                            c[i].R = (c[i].R << 1) | P[pi];

                        if (RGBPrecision.G != RGBPrecisionWithP.G)
                            c[i].G = (c[i].G << 1) | P[pi];

                        if (RGBPrecision.B != RGBPrecisionWithP.B)
                            c[i].B = (c[i].B << 1) | P[pi];

                        if (RGBPrecision.A != RGBPrecisionWithP.A)
                            c[i].A = (c[i].A << 1) | P[pi];
                    }
                }

                for (int i = 0; i < numEndPoints; i++)
                    c[i] = Unquantise(c[i], RGBPrecisionWithP);

                int[] w1 = new int[NUM_PIXELS_PER_BLOCK];
                int[] w2 = new int[NUM_PIXELS_PER_BLOCK];

                // Read colour indicies
                for (int i = 0; i < NUM_PIXELS_PER_BLOCK; i++)
                {
                    int numBits = IsFixUpOffset(Modes[mode].Partitions, shape, i) ? indexPrecision - 1 : indexPrecision;
                    if (start + numBits > 128)
                    {
                        Debugger.Break();
                        // Error
                    }
                    w1[i] = GetBits(source, sourceStart, ref start, numBits);
                }

                // Read Alpha
                if (APrecision != 0)
                {
                    for (int i = 0; i < NUM_PIXELS_PER_BLOCK; i++)
                    {
                        int numBits = i != 0 ? APrecision : APrecision - 1;
                        if (start + numBits > 128)
                        {
                        Debugger.Break();
                            // Error
                        }
                        w2[i] = GetBits(source, sourceStart, ref start, numBits);
                    }
                }


                for (int i = 0; i < NUM_PIXELS_PER_BLOCK; i++)
                {
                    int region = PartitionTable[partitions][shape][i];
                    LDRColour outPixel;
                    if (APrecision == 0)
                        outPixel = Interpolate(c[region << 1], c[(region << 1) + 1], w1[i], w1[i], indexPrecision, indexPrecision);
                    else
                    {
                        if (indexMode == 0)
                            outPixel = Interpolate(c[region << 1], c[(region << 1) + 1], w1[i], w2[i], indexPrecision, APrecision);
                        else
                            outPixel = Interpolate(c[region << 1], c[(region << 1) + 1], w2[i], w1[i], APrecision, indexPrecision);
                    }

                    switch (rotation)
                    {
                        case 1:
                            int temp = outPixel.R;
                            outPixel.R = outPixel.A;
                            outPixel.A = temp;
                            break;
                        case 2:
                            temp = outPixel.G;
                            outPixel.G = outPixel.A;
                            outPixel.A = temp;
                            break;
                        case 3:
                            temp = outPixel.B;
                            outPixel.B = outPixel.A;
                            outPixel.A = temp;
                            break;
                    }

                    outColours[i] = outPixel;
                }
                return outColours;
            }
            else
            {
                return outColours;
            }
        }


        #region Decompression Helpers
        private static LDRColour Interpolate(LDRColour lDRColour1, LDRColour lDRColour2, int wc, int wa, int wcPrec, int waPrec)
        {
            LDRColour temp = InterpolateRGB(lDRColour1, lDRColour2, wc, wcPrec);
            temp.A = InterpolateA(lDRColour1, lDRColour2, wa, waPrec);
            return temp;
        }

        private static int InterpolateA(LDRColour lDRColour1, LDRColour lDRColour2, int wa, int waPrec)
        {
            int[] weights = null;
            switch (waPrec)
            {
                case 2:
                    weights = AWeights2;
                    break;
                case 3:
                    weights = AWeights3;
                    break;
                case 4:
                    weights = AWeights4;
                    break;
                default:
                    return 0;
            }
            return (lDRColour1.A * (BC67_WEIGHT_MAX - weights[wa]) + lDRColour2.A * weights[wa] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
        }

        private static LDRColour InterpolateRGB(LDRColour lDRColour1, LDRColour lDRColour2, int wc, int wcPrec)
        {
            LDRColour temp = new LDRColour();
            int[] weights = null;
            switch (wcPrec)
            {
                case 2:
                    weights = AWeights2;
                    break;
                case 3:
                    weights = AWeights3;
                    break;
                case 4:
                    weights = AWeights4;
                    break;
                default:
                    return temp;
            }
            temp.R = (lDRColour1.R * (BC67_WEIGHT_MAX - weights[wc]) + lDRColour2.R * weights[wc] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
            temp.G = (lDRColour1.G * (BC67_WEIGHT_MAX - weights[wc]) + lDRColour2.G * weights[wc] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
            temp.B = (lDRColour1.B * (BC67_WEIGHT_MAX - weights[wc]) + lDRColour2.B * weights[wc] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
            return temp;
        }

        static bool IsFixUpOffset(int partitions, int shape, int offset)
        {
            for (int i = 0; i <= partitions; i++)
            {
                if (offset == FixUpTable[partitions][shape][i])
                    return true;
            }

            return false;
        }

        static LDRColour Unquantise(LDRColour colour, LDRColour rGBPrecisionWithP)
        {
            LDRColour temp = new LDRColour()
            {
                R = Unquantise(colour.R, rGBPrecisionWithP.R),
                G = Unquantise(colour.G, rGBPrecisionWithP.G),
                B = Unquantise(colour.B, rGBPrecisionWithP.B),
                A = rGBPrecisionWithP.A > 0 ? Unquantise(colour.A, rGBPrecisionWithP.A) : 255
            };
            return temp;
        }

        private static int Unquantise(int r1, int r2)
        {
            int temp = r1 << (8 - r2);
            return temp | (temp >> r2);
        }

        static int GetBit(byte[] source, int sourceStart, ref int start)
        {
            int uIndex = start >> 3;
            int ret = (source[sourceStart + uIndex] >> (start - (uIndex << 3))) & 0x01;
            start++;
            return ret;
        }

        static int GetBits(byte[] source, int sourceStart, ref int start, int length)
        {
            if (length == 0)
                return 0;

            int uIndex = start >> 3;
            int uBase = start - (uIndex << 3);
            int ret = 0;

            if (uBase + length > 8)
            {
                int firstIndexBits = 8 - uBase;
                int nextIndexBits = length - firstIndexBits;
                ret = (source[sourceStart + uIndex] >> uBase) | ((source[sourceStart + uIndex + 1] & ((1 << nextIndexBits) - 1)) << firstIndexBits);
            }
            else
                ret = (source[sourceStart + uIndex] >> uBase) & ((1 << length) - 1);

            
            start += length;
            return ret;
        }
        #endregion Decompression Helpers
    }
}