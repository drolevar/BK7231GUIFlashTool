﻿using System;

namespace BK7231Flasher
{
    internal class RFPartitionUtil
    {
        public static Random r = new Random(DateTime.Now.Millisecond);
        public static byte getRandomByte()
        {
            return (byte)r.Next(0, 255);
        }
        internal static byte[] constructRFDataFor(BKType curType, int sectorSize)
        {
            byte[] correct_rf_config;
            if (curType == BKType.BK7231T)
            {
                correct_rf_config = new byte[]{0x54, 0x4c, 0x56, 0x00, 0xe0, 0x01, 0x00, 0x00, 0x00, 0x11, 0x11, 0x11, 0x5a, 0x00, 0x00, 0x00, 0x01, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x4e, 0x61, 0xbc, 0x00, 0x02, 0x11, 0x11, 0x11,
0x06, 0x00, 0x00, 0x00, 0x84, 0xe3, 0x42, 0xb2, 0x61, 0xbf, 0x03, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x1d, 0x01, 0x00, 0x00, 0x04, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x8e, 0x15,
0x53, 0x01, 0x05, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x1b, 0x00, 0x00, 0x00, 0x06, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x55, 0x00, 0x54, 0x03, 0x07, 0x11, 0x11, 0x11, 0x08, 0x00,
0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x22, 0x22, 0x22, 0xa2, 0x00, 0x00, 0x00, 0x01, 0x22, 0x22, 0x22, 0x04, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x02, 0x22,
0x22, 0x22, 0x0e, 0x00, 0x00, 0x00, 0x0f, 0x8f, 0x8f, 0x8e, 0x8e, 0x8e, 0x8d, 0x8d, 0x8d, 0x8c, 0x8b, 0x8b, 0x0a, 0x8a, 0x03, 0x22, 0x22, 0x22, 0x0e, 0x00, 0x00, 0x00, 0x11, 0x91, 0x91, 0x90,
0x8f, 0x8f, 0x8e, 0x8d, 0x8d, 0x8c, 0x8b, 0x8a, 0x09, 0x89, 0x04, 0x22, 0x22, 0x22, 0x0e, 0x00, 0x00, 0x00, 0x0e, 0x8e, 0x8e, 0x8d, 0x8c, 0x8c, 0x8b, 0x8a, 0x8a, 0x89, 0x88, 0x87, 0x06, 0x86,
0x05, 0x22, 0x22, 0x22, 0x04, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x06, 0x22, 0x22, 0x22, 0x04, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x07, 0x22, 0x22, 0x22, 0x28, 0x00, 0x00, 0x00,
0x90, 0x90, 0x90, 0x90, 0x90, 0x8f, 0x8f, 0x8f, 0x8f, 0x8f, 0x8e, 0x8e, 0x8e, 0x8e, 0x8e, 0x8d, 0x8d, 0x8d, 0x8d, 0x0c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c,
0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x8c, 0x08, 0x22, 0x22, 0x22, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x33, 0x33, 0x33, 0x84, 0x00, 0x00, 0x00, 0x01, 0x33, 0x33, 0x33,
0x04, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x02, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00, 0x0a, 0x00, 0x00, 0x00, 0x03, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00,
0x04, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00, 0x03, 0x02, 0x00, 0x00, 0x05, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00, 0xf8, 0x01, 0x00, 0x00, 0x06, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00,
0xe8, 0x03, 0x00, 0x00, 0x07, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00, 0xff, 0x03, 0x00, 0x00, 0x08, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x09, 0x33, 0x33, 0x33,
0x04, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x0a, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x0b, 0x33, 0x33, 0x33, 0x04, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00,
0x00, 0x44, 0x44, 0x44, 0x40, 0x00, 0x00, 0x00, 0x01, 0x44, 0x44, 0x44, 0x20, 0x00, 0x00, 0x00, 0x80, 0x80, 0x80, 0x80, 0x78, 0x80, 0x78, 0x80, 0x70, 0x84, 0x70, 0x84, 0x60, 0x8c, 0x58, 0x94,
0x56, 0x94, 0x56, 0x94, 0x54, 0x94, 0x54, 0x94, 0x55, 0x95, 0x54, 0x95, 0x55, 0x95, 0x54, 0x95, 0x02, 0x44, 0x44, 0x44, 0x04, 0x00, 0x00, 0x00, 0xee, 0x01, 0x00, 0x00, 0x03, 0x44, 0x44, 0x44,
0x04, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00 };
                // NOTE: MAC 84:e3:42:b2:61:bf
                correct_rf_config[39] = getRandomByte();// replace 0xb2
                correct_rf_config[40] = getRandomByte();// replace 0x61
                correct_rf_config[41] = getRandomByte();// replace 0xbf
            }
            else if (curType == BKType.BK7231N)
            {
                correct_rf_config = new byte[] {0x54, 0x4c, 0x56, 0x00, 0x62, 0x00, 0x00, 0x00, 0x00, 0x11, 0x11, 0x11, 0x5a, 0x00, 0x00, 0x00, 0x01, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x4e, 0x61, 0xbc, 0x00, 0x02, 0x11, 0x11, 0x11
, 0x06, 0x00, 0x00, 0x00, 0x38, 0x1f, 0x8d, 0x38, 0x57, 0x0e, 0x03, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x5e, 0x01, 0x00, 0x00, 0x04, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x8e, 0x15
, 0x53, 0x01, 0x05, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x00, 0x06, 0x11, 0x11, 0x11, 0x04, 0x00, 0x00, 0x00, 0x55, 0x00, 0x54, 0x03, 0x07, 0x11, 0x11, 0x11, 0x08, 0x00
, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff};
                // NOTE: MAC 38:1f:8d:38:57:0e
                correct_rf_config[39] = getRandomByte();// replace 0x38
                correct_rf_config[40] = getRandomByte();// replace 0x57
                correct_rf_config[41] = getRandomByte();// replace 0x0e
            }
            else
            {
                correct_rf_config = null;
            }
            return MiscUtils.padArray(correct_rf_config, sectorSize);
        }
    }
}