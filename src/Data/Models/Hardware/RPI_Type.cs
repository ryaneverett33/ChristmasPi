using System;
using System.Collections.Generic;
using System.IO;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data.Interfaces;

namespace ChristmasPi.Data.Models.Hardware {
    public enum RPI_Type {
        RPIAPLUS,
        RPIBPLUS,
        RPIZERO,
        RPI3PLUS,
        RPI3APLUS,
        RPI3BPLUS,
        RPICM,
        RPI2B,
        RPI3B,
        RPICM3,
        RPICM3PLUS,
        RPI4B,
        UNKNOWN
    }
    public class RPIType {
        private static Dictionary<int, RPI_Type> typeDict;
        public static int PlaceHolderPin = 18;
        private static RPI_Type? type;
        public static readonly RPI_Type FALLBACK_TYPE = RPI_Type.RPI3BPLUS;

        public static RPI_Type GetHardwareType(bool defaultToRpi = false) {
            if (type != null)
                return type.Value;
            if (!File.Exists("/proc/cpuinfo")) {
                if (defaultToRpi)
                    return RPIType.FALLBACK_TYPE;
                throw new NonLinuxOSException();
            }    
            // check /proc/cpuinfo first then /proc/device-tree/system/linux,revision
            using (StreamReader reader = new StreamReader(File.OpenRead("/proc/cpuinfo"))) {
                string[] lines = reader.ReadToEnd().Split('\n');
                foreach (string line in lines) {
                    if (line.Contains("Revision", StringComparison.CurrentCultureIgnoreCase)) {
                        string[] tabSplit = line.Split('\t');
                        string revisionRaw = tabSplit[1].Trim();
                        UInt32 revision;
                        try {
                            revision = Convert.ToUInt32(revisionRaw, 16);
                        }
                        catch (FormatException) {
                            if (defaultToRpi)
                                return RPIType.FALLBACK_TYPE;
                            throw new Exception("Unable to parse revision number");
                        }
                        if (!typeDict.ContainsKey((int)revision)) {
                            if (defaultToRpi)
                                return RPIType.FALLBACK_TYPE;
                            throw new Exception("Invalid model revision");
                        }
                        type = typeDict[(int)revision];
                        return type.Value;
                    }
                }
                if (defaultToRpi)
                    return RPIType.FALLBACK_TYPE;
                throw new Exception("AARCH64 not supported yet");
            }
        }
        public static bool IsExpandedHeader(RPI_Type type) {
            switch (type) {
                case RPI_Type.RPI2B:
                    return false;
                case RPI_Type.RPI3B:
                    return false;
                case RPI_Type.RPI3PLUS:
                    return true;
                case RPI_Type.RPI3APLUS:
                    return true;
                case RPI_Type.RPI3BPLUS:
                    return true;
                case RPI_Type.RPI4B:
                    return true;
                case RPI_Type.RPIAPLUS:
                    return true;
                case RPI_Type.RPIBPLUS:
                    return false;
                case RPI_Type.RPICM:
                    return true;
                case RPI_Type.RPICM3:
                    return true;
                case RPI_Type.RPICM3PLUS:
                    return true;
                case RPI_Type.RPIZERO:
                    return true;
                default:
                    return false;
            }
        }
        public static Dictionary<int, RPI_Type> GetTypeDictionary() {
            // Using new revision codes from https://www.raspberrypi.org/documentation/hardware/raspberrypi/revision-codes/README.md
            if (typeDict == null) {
                typeDict = new Dictionary<int, RPI_Type> {
                { 0x900021, RPI_Type.RPIAPLUS },
                { 0x900032, RPI_Type.RPIBPLUS },
                { 0x900092, RPI_Type.RPIZERO },
                { 0x900093, RPI_Type.RPIZERO },
                { 0x9000c1, RPI_Type.RPIZERO },
                { 0x9020e0, RPI_Type.RPI3APLUS },
                { 0x920092, RPI_Type.RPIZERO },
                { 0x920093, RPI_Type.RPIZERO },
                { 0x900061, RPI_Type.RPICM },
                { 0xa01040, RPI_Type.RPI2B },
                { 0xa01041, RPI_Type.RPI2B },
                { 0xa02082, RPI_Type.RPI3B },
                { 0xa020a0, RPI_Type.RPICM3 },
                { 0xa020d3, RPI_Type.RPI3BPLUS },
                { 0xa02042, RPI_Type.RPI2B },
                { 0xa21041, RPI_Type.RPI2B },
                { 0xa22042, RPI_Type.RPI2B },
                { 0xa22082, RPI_Type.RPI3B },
                { 0xa220a0, RPI_Type.RPICM3 },
                { 0xa32082, RPI_Type.RPI3B },
                { 0xa52082, RPI_Type.RPI3B },
                { 0xa22083, RPI_Type.RPI3B },
                { 0xa02100, RPI_Type.RPICM3PLUS },
                { 0xa03111, RPI_Type.RPI4B },
                { 0xb03111, RPI_Type.RPI4B },
                { 0xc03111, RPI_Type.RPI4B },
                { 0xc03112, RPI_Type.RPI4B }
                };
            }
            return typeDict;
        }

        public static string PWMValidationString() {
            if (IsExpandedHeader(type.Value))
                return "18,21,24";
            else
                return "18,24";
        }

        public static string PWMImageUrl() {
            if (IsExpandedHeader(type.Value))
                return "/img/hardware/RPI_ExpandedHeader.png";
            else
                return "/img/hardware/RPI_ShortHeader.png";
        }

        public static string PWMPlaceholder() {
            return "18";
        }
    }
}
