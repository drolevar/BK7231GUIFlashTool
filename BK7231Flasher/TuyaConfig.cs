﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace BK7231Flasher
{
    public class TuyaKeysText
    {
        public string textDescription;
    }
    public class TuyaConfig
    {
        // thanks to kmnh/bk7231 tools for figuring out this format
        static string KEY_MASTER = "qwertyuiopasdfgh";
        static int SECTOR_SIZE = 4096;
        static uint MAGIC_FIRST_BLOCK = 0x13579753;
        static uint MAGIC_NEXT_BLOCK = 0x98761234;
        static byte[] KEY_PART_1 = Encoding.ASCII.GetBytes("8710_2M");
        static byte[] KEY_PART_2 = Encoding.ASCII.GetBytes("HHRRQbyemofrtytf");
        static byte[] MAGIC_CONFIG_START = new byte[] { 0x46, 0xDC, 0xED, 0x0E, 0x67, 0x2F, 0x3B, 0x70, 0xAE, 0x12, 0x76, 0xA3, 0xF8, 0x71, 0x2E, 0x03 };

        MemoryStream decrypted;
        byte[] descryptedRaw;
        // Dictionary<string, string> parms = new Dictionary<string, string>();
        List<KeyValue> parms = new List<KeyValue>();

        public bool fromFile(string fname)
        {
            using (var fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[fs.Length]; 
                fs.Read(buffer, 0, buffer.Length);
                return fromBytes(buffer);
            }
        }
        public byte []myDecrypt(byte [] data, int baseOffset, int blockIndex, byte [] key)
        {
            byte[] decryptedData = null;
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.ECB;
                    aes.Key = key;
                    aes.Padding = PaddingMode.Zeros;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream msDecrypt = new MemoryStream(data, baseOffset + SECTOR_SIZE * blockIndex, SECTOR_SIZE))
                    {
                        using (MemoryStream msPlain = new MemoryStream())
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] buffer = new byte[1024];
                                int bytesRead;
                                while ((bytesRead = csDecrypt.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    msPlain.Write(buffer, 0, bytesRead);
                                }
                            }
                            decryptedData = msPlain.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return decryptedData;
        }
        public bool fromBytes(byte[] data)
        {
            int needle = MiscUtils.indexOf(data, MAGIC_CONFIG_START);
            needle -= 32;

            byte[] key = null;

            decrypted = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(decrypted);
            byte[] first = myDecrypt(data, needle, 0, Encoding.ASCII.GetBytes(KEY_MASTER));
            using (BinaryReader br = new BinaryReader(new MemoryStream(first)))
            {
                UInt32 mag = br.ReadUInt32();
                if (mag != MAGIC_FIRST_BLOCK)
                {
                    return true;
                }
                uint crc = br.ReadUInt32();
                key = br.ReadBytes(16);
                if (checkCRC(crc, key, 0, key.Length) == false)
                {
                    return true;
                }
                key = makeSecondaryKey(key);
                bw.Write(first, 8, first.Length - 8);
            }
            for (int blockIndex = 1; blockIndex < 500; blockIndex++)
            {
                byte[] next = myDecrypt(data, needle, blockIndex, key);
                if (next == null)
                {
                    break;
                }
                using (BinaryReader br = new BinaryReader(new MemoryStream(next)))
                {
                    UInt32 mag = br.ReadUInt32();
                    if (mag != MAGIC_NEXT_BLOCK)
                    {
                        return true;
                    }
                    uint crc = br.ReadUInt32();
                    if (checkCRC(crc, next, 8, next.Length - 8) == false)
                    {
                        return true;
                    }
                    bw.Write(next, 8, next.Length - 8);
                }
            }
            descryptedRaw = decrypted.ToArray();
            File.WriteAllBytes("lastRawDecryptedStrings.bin", descryptedRaw);
            return false;
        }
        public string getKeysHumanReadable()
        {
            string desc = "";
            for (int i = 0; i < parms.Count; i++)
            {
                var p = parms[i];
                string key = p.Key;
                string value = p.Value;
                if (Regex.IsMatch(key, "^led\\d+_pin$"))
                {
                    int number = int.Parse(Regex.Match(key, "\\d+").Value);
                    desc += "- LED (channel " + number + ") on P" + value + Environment.NewLine;
                }
                else if (Regex.IsMatch(key, "^netled\\d+_pin$"))
                {
                    // some devices have netled1_pin, some have netled_pin
                    int number = int.Parse(Regex.Match(key, "\\d+").Value);
                    desc += "- WiFi LED on P" + value + Environment.NewLine;
                }
                else if (Regex.IsMatch(key, "^rl\\d+_pin$"))
                {
                    int number = int.Parse(Regex.Match(key, "\\d+").Value);
                    desc += "- Relay (channel " + number + ") on P" + value + Environment.NewLine;
                }
                else if (Regex.IsMatch(key, "^bt\\d+_pin$"))
                {
                    int number = int.Parse(Regex.Match(key, "\\d+").Value);
                    desc += "- Button (channel " + number + ") on P" + value + Environment.NewLine;
                }
                else if(Regex.IsMatch(key, "^door\\d+_magt_pin$"))
                {
                    int number = int.Parse(Regex.Match(key, "\\d+").Value);
                    desc += "- Door Sensor (channel " + number + ") on P" + value + Environment.NewLine;
                }
                else if(Regex.IsMatch(key, "^onoff\\d+$"))
                {
                    int number = int.Parse(Regex.Match(key, "\\d+").Value);
                    desc += "- TglChannelToggle (channel " + number + ") on P" + value + Environment.NewLine;
                }
                else if (key == "netled_pin")
                {
                    // some devices have netled1_pin, some have netled_pin
                    desc += "- WiFi LED on P" + value + Environment.NewLine;
                }
                else if(key == "ele_pin")
                {
                    desc += "- BL0937 ELE on P" + value + Environment.NewLine;
                }
                else if (key == "vi_pin")
                {
                    desc += "- BL0937 VI on P" + value + Environment.NewLine;
                }
                else if (key == "sel_pin_pin")
                {
                    desc += "- BL0937 SEL on P" + value + Environment.NewLine;
                }
                else if (key == "r_pin")
                {
                    desc += "- LED Red (Channel 1) on P" + value + Environment.NewLine;
                }
                else if (key == "g_pin")
                {
                    desc += "- LED Green (Channel 2) on P" + value + Environment.NewLine;
                }
                else if (key == "b_pin")
                {
                    desc += "- LED Blue (Channel 3) on P" + value + Environment.NewLine;
                }
                else if (key == "c_pin")
                {
                    desc += "- LED Cool (Channel 4) on P" + value + Environment.NewLine;
                }
                else if (key == "w_pin")
                {
                    desc += "- LED Warm (Channel 5) on P" + value + Environment.NewLine;
                }
                else if (key == "ctrl_pin")
                {
                    desc += "- Control Pin (TODO) on P" + value + Environment.NewLine;
                }
                else if (key == "total_bt_pin")
                {
                    desc += "- Pair/Toggle All Pin on P" + value + Environment.NewLine;
                }
                else
                {

                }
            }
            // LED
            string iicscl = getKeyValue("iicscl");
            string iicsda = getKeyValue("iicsda");
            if (iicscl.Length > 0 && iicsda.Length > 0)
            {
                string iicr = getKeyValue("iicr");
                string iicg = getKeyValue("iicg");
                string iicb = getKeyValue("iicb");
                string iicc = getKeyValue("iicc");
                string iicw = getKeyValue("iicw");
                string map = "" + iicr + " " + iicg + " " + iicb + " " + iicc + " " + iicw;
                string ledType = "Unknown";
                string ehccur = getKeyValue("ehccur");
                string dccur = getKeyValue("dccur");
                string cjwcur = getKeyValue("cjwcur");
                string _2235ccur = getKeyValue("2235ccur");
                string _2335ccur = getKeyValue("2335ccur");
                // use current (color/cw) setting
                if (ehccur.Length>0)
                {
                    ledType = "SM2135";
                }
                else if (dccur.Length > 0)
                {
                    ledType = "BP5758D_";
                }
                else if (cjwcur.Length > 0)
                {
                    ledType = "BP1658CJ_";
                }
                else if (_2235ccur.Length > 0)
                {
                    ledType = "SM2235";
                }
                else if (_2335ccur.Length > 0)
                {
                    // fallback
                    ledType = "SM2235";
                }
                else
                {

                }
                string dat_name = ledType + "DAT";
                string clk_name = ledType + "CLK";
                desc += "- " + dat_name + " on P" + iicsda + Environment.NewLine;
                desc += "- " + clk_name + " on P" + iicscl + Environment.NewLine;
                desc += "- LED remap is " + map + Environment.NewLine;
            }
            if (desc.Length > 0)
            {
                desc = "Device configuration, as extracted from Tuya: " + Environment.NewLine + desc;
            }
            else
            {
                desc = "Sorry, no meaningful data found. This device may be TuyaMCU or a custom one with no Tuya config data." + Environment.NewLine;
            }
            return desc;
        }
        public string getKeyValue(string key)
        {
            for (int i = 0; i < parms.Count; i++)
            {
                if (parms[i].Key == key)
                {
                    return parms[i].Value;
                }
            }
            return "";
        }
        public string getKeysAsJSON()
        {
            string r = "{" + Environment.NewLine;
            for (int i = 0; i < parms.Count; i++)
            {
                var p = parms[i];
                r += "\t\""+p.Key + "\":\"" + p.Value + "\"";
                if (i != parms.Count-1)
                {
                    r += ",";
                }
                r += Environment.NewLine;
            }
            r += "}" + Environment.NewLine;
            return r;
        }
        public bool extractKeys() { 
            int keys_at = MiscUtils.indexOf(descryptedRaw, Encoding.ASCII.GetBytes("ap_s{"));
            if (keys_at == -1)
            {
                return true;
            }
            int stopAT = MiscUtils.findFirst(descryptedRaw, (byte)'}', keys_at);
            if (stopAT == -1)
            {
                return true;
            }
            int first_at = keys_at + 5;
            byte[] str = MiscUtils.subArray(descryptedRaw, first_at, stopAT - first_at);
            string asciiString;
            // There is still some kind of Tuya paging here,
            // let's skip it in a quick and dirty way
#if false
            str = MiscUtils.stripBinary(str);
            asciiString = Encoding.ASCII.GetString(str);
#else
            asciiString = "";
            for(int i = 0; i < str.Length; i++)
            {
                byte b = str[i];
                if (b < 32)
                    continue;
                if (b > 127)
                    continue;
                char ch = (char)b;
                asciiString += ch;
            }
#endif
            string[] pairs = asciiString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < pairs.Length; i++)
            {
                string []kp = pairs[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                string skey = kp[0];
                string svalue = kp[1];
                skey = skey.Trim(new char[] { '"' });
                svalue = svalue.Trim(new char[] { '"' });
                //parms.Add(skey, svalue);
                KeyValue kv = new KeyValue(skey, svalue);
                parms.Add(kv);
            }
            return false;
        }
        byte[] makeSecondaryKey(byte[] innerKey)
        {
            byte[] key = new byte[0x10];
            for (int i = 0; i < 16; i++)
            {
                key[i] = (byte)(KEY_PART_1[i & 3] + KEY_PART_2[i]);
            }
            for (int i = 0; i < 16; i++)
            {
                key[i] = (byte)((key[i] + innerKey[i]) % 256);
            }
            return key;
        }
        bool checkCRC(uint expected, byte [] dat, int ofs, int len)
        {
            uint n = 0;
            for(int i = 0; i < len; i++)
            {
                n += dat[ofs + i];
            }
            n = n & 0xFFFFFFFF;
            if (n == expected)
                return true;
            return false;
        }
    }
}