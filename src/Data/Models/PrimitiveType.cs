using System;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Data.Models {
    public class PrimitiveType {
        private bool isPrimitiveType;
        private PrimType ptype;
        private ExpandType etype;

        public bool IsBoolean => isPrimitiveType ? ptype == PrimType.Bool : false;
        public bool IsString => isPrimitiveType ? ptype == PrimType.String : false;
        public bool IsInteger => isPrimitiveType ? ptype == PrimType.Int : false;
        public bool IsFloat => isPrimitiveType ? ptype == PrimType.Float : false;

        public PrimitiveType (PrimType type) {
            isPrimitiveType = true;
            ptype = type;
        }
        public PrimitiveType (ExpandType type) {
            isPrimitiveType = false;
            etype = type;
        }

        public string AsString(object value) {
            if (isPrimitiveType && ptype == PrimType.String) {
                if (!(value is string))
                    return value.ToString();
                return (string)value;
            }
            else {
                throw new CastToInvalidTypeException();
            }
        }
        public int AsInteger(object value) {
            if (isPrimitiveType && ptype == PrimType.Int) {
                if (value is int)
                    return int.Parse((string)value);
                return (int)value;
            }
            else {
                throw new CastToInvalidTypeException();
            }
        }
        public float AsFloat(object value) {
            if (isPrimitiveType && ptype == PrimType.Float) {
                if (value is string)
                    return float.Parse((string)value);
                //if (value is float || value is Single || value is double)
                //   return (float)value;
                return (float)value;
            }
            else {
                throw new CastToInvalidTypeException();
            }
        }
        public bool AsBoolean(object value) {
            if (isPrimitiveType && ptype == PrimType.Bool) {
                if (value is string)
                    return bool.Parse((string)value);
                return (bool)value;
            }
            else {
                throw new CastToInvalidTypeException();
            }
        }
    }
    // A PrimitiveType object that represents a primitive type
    public enum PrimType {
        String,
        Int,
        Float,
        Bool
    }
    // A PrimitiveType object whos value maps to a primitive type after expansion
    public enum ExpandType {
        ShortTime              // represents a small segment of time, i.e. 2s or 5ms or 15m
    }
}
