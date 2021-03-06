﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RazzleServer.Wz.Util;
using RazzleServer.Wz.WzProperties;

namespace RazzleServer.Wz
{
    /// <inheritdoc>
    ///     <cref>WzObject</cref>
    /// </inheritdoc>
    /// <summary>
    /// A .img contained in a wz directory
    /// </summary>
    public class WzImage : WzObject, IPropertyContainer
    {
        private List<WzImageProperty> _properties = new List<WzImageProperty>();

        internal WzBinaryReader Reader { get; }

        [JsonIgnore] public bool ParseEverything { get; private set; }
        internal long TempFileStart { get; set; }
        internal long TempFileEnd { get; set; }

        /// <summary>
        /// Creates a blank WzImage
        /// </summary>
        public WzImage()
        {
        }

        /// <summary>
        /// Creates a WzImage with the given name
        /// </summary>
        /// <param name="name">The name of the image</param>
        public WzImage(string name)
        {
            Name = name;
        }

        public WzImage(string name, Stream dataStream, WzMapleVersionType mapleVersion)
        {
            Name = name;
            Reader = new WzBinaryReader(dataStream, WzTool.GetIvByMapleVersion(mapleVersion));
        }

        internal WzImage(string name, WzBinaryReader reader)
        {
            Name = name;
            Reader = reader;
            BlockStart = (int)reader.BaseStream.Position;
        }

        public override void Dispose()
        {
            Name = null;
            Reader?.Dispose();
            _properties?.ForEach(x => x.Dispose());
            _properties?.Clear();
            _properties = null;
        }

        public override WzFile WzFileParent => Parent?.WzFileParent;

        /// <summary>
        /// Is the object Parsed
        /// </summary>
        [JsonIgnore]
        public bool Parsed { get; set; }

        /// <summary>
        /// Was the image changed
        /// </summary>
        [JsonIgnore]
        public bool Changed { get; set; }

        /// <summary>
        /// The size in the wz file of the image
        /// </summary>
        [JsonIgnore]
        public int BlockSize { get; set; }

        /// <summary>
        /// The checksum of the image
        /// </summary>
        [JsonIgnore]
        public int Checksum { get; set; }

        /// <summary>
        /// The Offset of the image
        /// </summary>
        [JsonIgnore]
        public uint Offset { get; set; }

        [JsonIgnore] public int BlockStart { get; }

        public override WzObjectType ObjectType
        {
            get
            {
                if (Reader != null && !Parsed)
                {
                    ParseImage();
                }

                return WzObjectType.Image;
            }
        }

        /// <summary>
        /// The properties contained in the image
        /// </summary>
        public List<WzImageProperty> WzProperties
        {
            get
            {
                if (Reader != null && !Parsed)
                {
                    ParseImage();
                }

                return _properties;
            }
        }

        public WzImage DeepClone()
        {
            if (Reader != null && !Parsed)
            {
                ParseImage();
            }

            var clone = new WzImage(Name) {Changed = true};
            foreach (var prop in _properties)
            {
                clone.AddProperty(prop.DeepClone());
            }

            return clone;
        }

        /// <summary>
        /// Gets a wz property by it's name
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <returns>The wz property with the specified name</returns>
        public new WzImageProperty this[string name]
        {
            get
            {
                if (Reader != null)
                {
                    if (!Parsed)
                    {
                        ParseImage();
                    }
                }

                foreach (var iwp in _properties)
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

                value.Name = name;
                AddProperty(value);
            }
        }

        /// <summary>
        /// Gets a WzImageProperty from a path
        /// </summary>
        /// <param name="path">path to object</param>
        /// <returns>the selected WzImageProperty</returns>
        public WzImageProperty GetFromPath(string path)
        {
            if (Reader != null)
            {
                if (!Parsed)
                {
                    ParseImage();
                }
            }

            var segments = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (segments[0] == "..")
            {
                return null;
            }

            WzImageProperty ret = null;
            foreach (var segment in segments)
            {
                var foundChild = false;
                foreach (var iwp in ret == null ? _properties : ret.WzProperties)
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

        /// <summary>
        /// Adds a property to the image
        /// </summary>
        /// <param name="prop">Property to add</param>
        public void AddProperty(WzImageProperty prop)
        {
            prop.Parent = this;
            if (Reader != null && !Parsed)
            {
                ParseImage();
            }

            _properties.Add(prop);
        }

        public void AddProperties(IEnumerable<WzImageProperty> props)
        {
            foreach (var prop in props)
            {
                AddProperty(prop);
            }
        }

        /// <summary>
        /// Removes a property by name
        /// </summary>
        /// <param name="prop">The property to remove</param>
        public void RemoveProperty(WzImageProperty prop)
        {
            if (Reader != null && !Parsed)
            {
                ParseImage();
            }

            prop.Parent = null;
            _properties.Remove(prop);
        }

        public void ClearProperties()
        {
            foreach (var prop in _properties)
            {
                prop.Parent = null;
            }

            _properties.Clear();
        }

        public override void Remove()
        {
            ((WzDirectory)Parent).RemoveImage(this);
        }

        /// <summary>
        /// Parses the image from the wz file
        /// </summary>
        public void ParseImage(bool parseEverything)
        {
            if (Parsed)
            {
                return;
            }

            if (Changed)
            {
                Parsed = true;
                return;
            }

            ParseEverything = parseEverything;
            Reader.BaseStream.Position = Offset;
            var b = Reader.ReadByte();
            if (b != 0x73 || Reader.ReadString() != "Property" || Reader.ReadUInt16() != 0)
            {
                return;
            }

            _properties.AddRange(WzImageProperty.ParsePropertyList(Offset, Reader, this, this));
            Parsed = true;
        }

        /// <summary>
        /// Parses the image from the wz file
        /// </summary>
        public void ParseImage()
        {
            if (Parsed)
            {
                return;
            }

            if (Changed)
            {
                Parsed = true;
                return;
            }

            ParseEverything = false;
            Reader.BaseStream.Position = Offset;
            var b = Reader.ReadByte();
            if (b != 0x73 || Reader.ReadString() != "Property" || Reader.ReadUInt16() != 0)
            {
                return;
            }

            _properties.AddRange(WzImageProperty.ParsePropertyList(Offset, Reader, this, this));
            Parsed = true;
        }

        [JsonIgnore]
        public byte[] DataBlock
        {
            get
            {
                if (Reader == null || BlockSize <= 0)
                {
                    return null;
                }

                var blockData = Reader.ReadBytes(BlockSize);
                Reader.BaseStream.Position = BlockStart;
                return blockData;
            }
        }

        public void UnparseImage()
        {
            Parsed = false;
            _properties = new List<WzImageProperty>();
        }

        internal void SaveImage(WzBinaryWriter writer)
        {
            if (Changed)
            {
                if (Reader != null && !Parsed)
                {
                    ParseImage();
                }

                var imgProp = new WzSubProperty();
                var startPos = writer.BaseStream.Position;
                imgProp.AddProperties(WzProperties);
                imgProp.WriteValue(writer);
                writer.StringCache.Clear();
                BlockSize = (int)(writer.BaseStream.Position - startPos);
            }
            else
            {
                var pos = Reader.BaseStream.Position;
                Reader.BaseStream.Position = Offset;
                writer.Write(Reader.ReadBytes(BlockSize));
                Reader.BaseStream.Position = pos;
            }
        }

        public override IEnumerable<WzObject> GetObjects()
        {
            var objList = new List<WzObject>();
            foreach (var prop in WzProperties)
            {
                objList.Add(prop);
                objList.AddRange(prop.GetObjects());
            }

            return objList;
        }

        public List<string> GetPaths(string curPath)
        {
            var objList = new List<string>();
            foreach (var prop in WzProperties)
            {
                objList.Add(curPath + "/" + prop.Name);
                objList.AddRange(prop.GetPaths(curPath + "/" + prop.Name));
            }

            return objList;
        }
    }
}
