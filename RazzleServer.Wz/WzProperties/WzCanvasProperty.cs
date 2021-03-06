﻿using System;
using System.Collections.Generic;
using System.Drawing;
using RazzleServer.Wz.Util;

namespace RazzleServer.Wz.WzProperties
{
    /// <summary>
    /// A property that can contain sub properties and has one png image
    /// </summary>
    public class WzCanvasProperty : WzExtended, IPropertyContainer
    {
        public override void SetValue(object value) => PngProperty = (WzPngProperty)value;

        public override WzImageProperty DeepClone()
        {
            var clone = new WzCanvasProperty(Name);
            foreach (var prop in WzProperties)
            {
                clone.AddProperty(prop.DeepClone());
            }

            clone.PngProperty = (WzPngProperty)PngProperty.DeepClone();
            return clone;
        }

        public override object WzValue => PngProperty;

        public override WzPropertyType Type => WzPropertyType.Canvas;

        public override WzImageProperty this[string name]
        {
            get
            {
                if (name == "PNG")
                {
                    return PngProperty;
                }

                foreach (var iwp in WzProperties)
                {
                    if (iwp.Name.ToLower() == name.ToLower())
                    {
                        return iwp;
                    }
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                if (name == "PNG")
                {
                    PngProperty = (WzPngProperty)value;
                    return;
                }

                value.Name = name;
                AddProperty(value);
            }
        }

        public override WzImageProperty GetFromPath(string path)
        {
            var segments = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (segments[0] == "..")
            {
                return ((WzImageProperty)Parent)[path.Substring(Name.IndexOf('/') + 1)];
            }

            WzImageProperty ret = this;
            foreach (var segment in segments)
            {
                var foundChild = false;
                if (segment == "PNG")
                {
                    return PngProperty;
                }

                foreach (var iwp in ret.WzProperties)
                {
                    if (iwp.Name != segment)
                    {
                        continue;
                    }

                    ret = iwp;
                    foundChild = true;
                    break;
                }

                if (!foundChild)
                {
                    return null;
                }
            }

            return ret;
        }

        public override void WriteValue(WzBinaryWriter writer)
        {
            writer.WriteStringValue("Canvas", 0x73, 0x1B);
            writer.Write((byte)0);
            if (WzProperties.Count > 0)
            {
                writer.Write((byte)1);
                WritePropertyList(writer, WzProperties);
            }
            else
            {
                writer.Write((byte)0);
            }

            writer.WriteCompressedInt(PngProperty.Width);
            writer.WriteCompressedInt(PngProperty.Height);
            writer.WriteCompressedInt(PngProperty.Format1);
            writer.Write((byte)PngProperty.Format2);
            writer.Write(0);
            var bytes = PngProperty.GetCompressedBytes(false);
            writer.Write(bytes.Length + 1);
            writer.Write((byte)0);
            writer.Write(bytes);
        }

        /// <summary>
        /// Dispose the object
        /// </summary>
        public override void Dispose()
        {
            Name = null;
            PngProperty.Dispose();
            PngProperty = null;
            WzProperties?.ForEach(x => x.Dispose());
            WzProperties?.Clear();
            WzProperties = null;
        }

        /// <summary>
        /// The png image for this canvas property
        /// </summary>
        public WzPngProperty PngProperty { get; set; }

        /// <summary>
        /// Creates a blank WzCanvasProperty
        /// </summary>
        public WzCanvasProperty() { }

        /// <summary>
        /// Creates a WzCanvasProperty with the specified name
        /// </summary>
        /// <param name="name">The name of the property</param>
        public WzCanvasProperty(string name) => Name = name;

        /// <summary>
        /// Adds a property to the property list of this property
        /// </summary>
        /// <param name="prop">The property to add</param>
        public void AddProperty(WzImageProperty prop)
        {
            prop.Parent = this;
            WzProperties.Add(prop);
        }

        public void AddProperties(IEnumerable<WzImageProperty> props)
        {
            foreach (var prop in props)
            {
                AddProperty(prop);
            }
        }

        /// <summary>
        /// Remove a property
        /// </summary>
        public void RemoveProperty(WzImageProperty prop)
        {
            prop.Parent = null;
            WzProperties.Remove(prop);
        }

        /// <summary>
        /// Clears the list of properties
        /// </summary>
        public void ClearProperties()
        {
            foreach (var prop in WzProperties)
            {
                prop.Parent = null;
            }

            WzProperties.Clear();
        }

        public override Bitmap GetBitmap() => PngProperty.GetPng(false);
    }
}
