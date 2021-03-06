﻿using System;
using System.Globalization;
using RazzleServer.Wz.Util;

namespace RazzleServer.Wz.WzProperties
{
    /// <inheritdoc />
    /// <summary>
    /// A property that is stored in the wz file with a byte and possibly followed by a float. If the 
    /// byte is 0, the value is 0, else the value is the float that follows.
    /// </summary>
    public class WzFloatProperty : WzImageProperty
    {
        /// <summary>
        /// Creates a blank WzFloatProperty
        /// </summary>
        public WzFloatProperty()
        {
        }

        /// <summary>
        /// Creates a WzByteFloatProperty with the specified name
        /// </summary>
        /// <param name="name">The name of the property</param>
        public WzFloatProperty(string name) => Name = name;

        /// <summary>
        /// Creates a WzByteFloatProperty with the specified name and value
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value of the property</param>
        public WzFloatProperty(string name, float value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The value of the property
        /// </summary>
        public float Value { get; private set; }

        public override object WzValue => Value;

        public override void SetValue(object value) => Value = (float)value;

        public override WzImageProperty DeepClone() => new WzFloatProperty(Name, Value);

        public override WzPropertyType Type => WzPropertyType.Float;

        public override void WriteValue(WzBinaryWriter writer)
        {
            writer.Write((byte)4);
            if (Math.Abs(Value) < float.Epsilon)
            {
                writer.Write((byte)0);
            }
            else
            {
                writer.Write((byte)0x80);
                writer.Write(Value);
            }
        }

        public override float GetFloat() => Value;

        public override double GetDouble() => Value;

        public override int GetInt() => (int)Value;

        public override short GetShort() => (short)Value;

        public override long GetLong() => (long)Value;

        public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);

        public override void Dispose() => Name = null;
    }
}
