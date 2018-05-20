﻿using System;
using System.Collections.Generic;
using RazzleServer.Common.WzLib.Util;
using RazzleServer.Common.WzLib.WzProperties;
using System.IO;
using System.Globalization;
using System.Xml;
using System.DrawingCore;
using System.DrawingCore.Imaging;

namespace RazzleServer.Common.WzLib.Serialization
{
    public abstract class ProgressingWzSerializer
    {
        protected int total;
        protected int curr;
        public int Total { get { return total; } }
        public int Current { get { return curr; } }

        protected static void CreateDirSafe(ref string path)
        {
            if (path.Substring(path.Length - 1, 1) == @"\")
            {
                {
                    path = path.Substring(0, path.Length - 1);
                }
            }

            string basePath = path;
            int curridx = 0;
            while (Directory.Exists(path) || File.Exists(path))
            {
                curridx++;
                path = basePath + curridx;
            }
            Directory.CreateDirectory(path);
        }
    }

    public abstract class WzXmlSerializer : ProgressingWzSerializer
    {
        protected string indent;
        protected string lineBreak;
        public static NumberFormatInfo formattingInfo;
        protected bool ExportBase64Data;

        protected static char[] amp = "&amp;".ToCharArray();
        protected static char[] lt = "&lt;".ToCharArray();
        protected static char[] gt = "&gt;".ToCharArray();
        protected static char[] apos = "&apos;".ToCharArray();
        protected static char[] quot = "&quot;".ToCharArray();

        static WzXmlSerializer()
        {
            formattingInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = ","
            };
        }

        internal WzXmlSerializer(int indentation, LineBreak lineBreakType)
        {
            switch (lineBreakType)
            {
                case LineBreak.None:
                    lineBreak = "";
                    break;
                case LineBreak.Windows:
                    lineBreak = "\r\n";
                    break;
                case LineBreak.Unix:
                    lineBreak = "\n";
                    break;
            }
            char[] indentArray = new char[indentation];
            for (int i = 0; i < indentation; i++)
            {
                {
                    indentArray[i] = (char)0x20;
                }
            }

            indent = new string(indentArray);
        }

        protected void WritePropertyToXML(TextWriter tw, string depth, WzImageProperty prop)
        {
            if (prop is WzCanvasProperty property3)
            {
                if (ExportBase64Data)
                {
                    MemoryStream stream = new MemoryStream();
                    property3.PngProperty.GetPNG(false).Save(stream, ImageFormat.Png);
                    byte[] pngbytes = stream.ToArray();
                    stream.Close();
                    tw.Write(string.Concat(new object[] { depth, "<canvas name=\"", XmlUtil.SanitizeText(property3.Name), "\" width=\"", property3.PngProperty.Width, "\" height=\"", property3.PngProperty.Height, "\" basedata=\"", Convert.ToBase64String(pngbytes), "\">" }) + lineBreak);
                }
                else
                {
                    {
                        tw.Write(string.Concat(new object[] { depth, "<canvas name=\"", XmlUtil.SanitizeText(property3.Name), "\" width=\"", property3.PngProperty.Width, "\" height=\"", property3.PngProperty.Height, "\">" }) + lineBreak);
                    }
                }

                string newDepth = depth + indent;
                foreach (WzImageProperty property in property3.WzProperties)
                {
                    {
                        WritePropertyToXML(tw, newDepth, property);
                    }
                }

                tw.Write(depth + "</canvas>" + lineBreak);
            }
            else if (prop is WzIntProperty property4)
            {
                tw.Write(string.Concat(new object[] { depth, "<int name=\"", XmlUtil.SanitizeText(property4.Name), "\" value=\"", property4.Value, "\"/>" }) + lineBreak);
            }
            else if (prop is WzDoubleProperty property5)
            {
                tw.Write(string.Concat(new object[] { depth, "<double name=\"", XmlUtil.SanitizeText(property5.Name), "\" value=\"", property5.Value, "\"/>" }) + lineBreak);
            }
            else if (prop is WzNullProperty property6)
            {
                tw.Write(depth + "<null name=\"" + XmlUtil.SanitizeText(property6.Name) + "\"/>" + lineBreak);
            }
            else if (prop is WzSoundProperty property7)
            {
                if (ExportBase64Data)
                {
                    {
                        tw.Write(string.Concat(new object[] { depth, "<sound name=\"", XmlUtil.SanitizeText(property7.Name), "\" length=\"", property7.Length.ToString(), "\" basehead=\"", Convert.ToBase64String(property7.Header), "\" basedata=\"", Convert.ToBase64String(property7.GetBytes(false)), "\"/>" }) + lineBreak);
                    }
                }
                else
                {
                    {
                        tw.Write(depth + "<sound name=\"" + XmlUtil.SanitizeText(property7.Name) + "\"/>" + lineBreak);
                    }
                }
            }
            else if (prop is WzStringProperty property8)
            {
                string str = XmlUtil.SanitizeText(property8.Value);
                tw.Write(depth + "<string name=\"" + XmlUtil.SanitizeText(property8.Name) + "\" value=\"" + str + "\"/>" + lineBreak);
            }
            else if (prop is WzSubProperty property9)
            {
                tw.Write(depth + "<imgdir name=\"" + XmlUtil.SanitizeText(property9.Name) + "\">" + lineBreak);
                string newDepth = depth + indent;
                foreach (WzImageProperty property in property9.WzProperties)
                {
                    {
                        WritePropertyToXML(tw, newDepth, property);
                    }
                }

                tw.Write(depth + "</imgdir>" + lineBreak);
            }
            else if (prop is WzShortProperty property10)
            {
                tw.Write(string.Concat(new object[] { depth, "<short name=\"", XmlUtil.SanitizeText(property10.Name), "\" value=\"", property10.Value, "\"/>" }) + lineBreak);
            }
            else if (prop is WzLongProperty long_prop)
            {
                tw.Write(string.Concat(new object[] { depth, "<long name=\"", XmlUtil.SanitizeText(long_prop.Name), "\" value=\"", long_prop.Value, "\"/>" }) + lineBreak);
            }
            else if (prop is WzUOLProperty property11)
            {
                tw.Write(depth + "<uol name=\"" + property11.Name + "\" value=\"" + XmlUtil.SanitizeText(property11.Value) + "\"/>" + lineBreak);
            }
            else if (prop is WzVectorProperty property12)
            {
                tw.Write(string.Concat(new object[] { depth, "<vector name=\"", XmlUtil.SanitizeText(property12.Name), "\" x=\"", property12.X.Value, "\" y=\"", property12.Y.Value, "\"/>" }) + lineBreak);
            }
            else if (prop is WzFloatProperty property13)
            {
                string str2 = Convert.ToString(property13.Value, formattingInfo);
                if (!str2.Contains("."))
                {
                    {
                        str2 = str2 + ".0";
                    }
                }

                tw.Write(depth + "<float name=\"" + XmlUtil.SanitizeText(property13.Name) + "\" value=\"" + str2 + "\"/>" + lineBreak);
            }
            else if (prop is WzConvexProperty property14)
            {
                tw.Write(depth + "<extended name=\"" + XmlUtil.SanitizeText(prop.Name) + "\">" + lineBreak);
                string newDepth = depth + indent;
                foreach (WzImageProperty property in property14.WzProperties)
                {
                    {
                        WritePropertyToXML(tw, newDepth, property);
                    }
                }

                tw.Write(depth + "</extended>" + lineBreak);
            }
        }
    }

    public interface IWzFileSerializer
    {
        void SerializeFile(WzFile file, string path);
    }

    public interface IWzDirectorySerializer : IWzFileSerializer
    {
        void SerializeDirectory(WzDirectory dir, string path);
    }

    public interface IWzImageSerializer : IWzDirectorySerializer
    {
        void SerializeImage(WzImage img, string path);
    }

    public interface IWzObjectSerializer
    {
        void SerializeObject(WzObject file, string path);
    }

    public enum LineBreak
    {
        None,
        Windows,
        Unix
    }

    public class NoBase64DataException : Exception
    {
        public NoBase64DataException() { }
        public NoBase64DataException(string message) : base(message) { }
        public NoBase64DataException(string message, System.Exception inner) : base(message, inner) { }
        protected NoBase64DataException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }

    public class WzImgSerializer : ProgressingWzSerializer, IWzImageSerializer
    {
        private byte[] SerializeImageInternal(WzImage img)
        {
            MemoryStream stream = new MemoryStream();
            WzBinaryWriter wzWriter = new WzBinaryWriter(stream, ((WzDirectory)img.parent).WzIv);
            img.SaveImage(wzWriter);
            byte[] result = stream.ToArray();
            wzWriter.Close();
            return result;
        }

        private void SerializeImageInternal(WzImage img, string outPath)
        {
            FileStream stream = File.Create(outPath);
            WzBinaryWriter wzWriter = new WzBinaryWriter(stream, ((WzDirectory)img.parent).WzIv);
            img.SaveImage(wzWriter);
            wzWriter.Close();
        }

        public byte[] SerializeImage(WzImage img)
        {
            total = 1; curr = 0;
            return SerializeImageInternal(img);
        }

        public void SerializeImage(WzImage img, string outPath)
        {
            total = 1; curr = 0;
            if (Path.GetExtension(outPath) != ".img")
            {
                {
                    outPath += ".img";
                }
            }

            SerializeImageInternal(img, outPath);
        }

        public void SerializeDirectory(WzDirectory dir, string outPath)
        {
            total = dir.CountImages();
            curr = 0;
            if (!Directory.Exists(outPath))
            {
                {
                    ProgressingWzSerializer.CreateDirSafe(ref outPath);
                }
            }

            if (outPath.Substring(outPath.Length - 1, 1) != @"\")
            {
                {
                    outPath += @"\";
                }
            }

            foreach (WzDirectory subdir in dir.WzDirectories)
            {
                {
                    SerializeDirectory(subdir, outPath + subdir.Name + @"\");
                }
            }

            foreach (WzImage img in dir.WzImages)
            {
                {
                    SerializeImage(img, outPath + img.Name);
                }
            }
        }

        public void SerializeFile(WzFile f, string outPath)
        {
            SerializeDirectory(f.WzDirectory, outPath);
        }
    }


    public class WzImgDeserializer : ProgressingWzSerializer
    {
        private readonly bool freeResources;

        public WzImgDeserializer(bool freeResources)
        {
            this.freeResources = freeResources;
        }

        public WzImage WzImageFromIMGBytes(byte[] bytes, WzMapleVersion version, string name, bool freeResources)
        {
            byte[] iv = WzTool.GetIvByMapleVersion(version);
            MemoryStream stream = new MemoryStream(bytes);
            WzBinaryReader wzReader = new WzBinaryReader(stream, iv);
            WzImage img = new WzImage(name, wzReader)
            {
                BlockSize = bytes.Length,
                Checksum = 0
            };
            foreach (byte b in bytes)
            {
                {
                    img.Checksum += b;
                }
            }

            img.Offset = 0;
            if (freeResources)
            {
                img.ParseImage(true);
                img.Changed = true;
                wzReader.Close();
            }
            return img;
        }

        public WzImage WzImageFromIMGFile(string inPath, byte[] iv, string name)
        {
            FileStream stream = File.OpenRead(inPath);
            WzBinaryReader wzReader = new WzBinaryReader(stream, iv);
            WzImage img = new WzImage(name, wzReader)
            {
                BlockSize = (int)stream.Length,
                Checksum = 0
            };
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            stream.Position = 0;
            foreach (byte b in bytes)
            {
                {
                    img.Checksum += b;
                }
            }

            img.Offset = 0;
            if (freeResources)
            {
                img.ParseImage(true);
                img.Changed = true;
                wzReader.Close();
            }
            return img;
        }
    }


    public class WzPngMp3Serializer : ProgressingWzSerializer, IWzImageSerializer, IWzObjectSerializer
    {
        //List<WzImage> imagesToUnparse = new List<WzImage>();
        private string outPath;

        public void SerializeObject(WzObject obj, string outPath)
        {
            //imagesToUnparse.Clear();
            total = 0; curr = 0;
            this.outPath = outPath;
            if (!Directory.Exists(outPath))
            {
                {
                    CreateDirSafe(ref outPath);
                }
            }

            if (outPath.Substring(outPath.Length - 1, 1) != @"\")
            {
                {
                    outPath += @"\";
                }
            }

            total = CalculateTotal(obj);
            ExportRecursion(obj, outPath);
            /*foreach (WzImage img in imagesToUnparse)
                img.UnparseImage();
            imagesToUnparse.Clear();*/
        }

        public void SerializeFile(WzFile file, string path)
        {
            SerializeObject(file, path);
        }

        public void SerializeDirectory(WzDirectory file, string path)
        {
            SerializeObject(file, path);
        }

        public void SerializeImage(WzImage file, string path)
        {
            SerializeObject(file, path);
        }

        private int CalculateTotal(WzObject currObj)
        {
            int result = 0;
            if (currObj is WzFile)
            {
                {
                    result += ((WzFile)currObj).WzDirectory.CountImages();
                }
            }
            else if (currObj is WzDirectory)
            {
                {
                    result += ((WzDirectory)currObj).CountImages();
                }
            }

            return result;
        }

        private void ExportRecursion(WzObject currObj, string exportOutPath)
        {
            if (currObj is WzFile)
            {
                {
                    ExportRecursion(((WzFile)currObj).WzDirectory, exportOutPath);
                }
            }
            else if (currObj is WzDirectory)
            {
                exportOutPath += currObj.Name + @"\";
                if (!Directory.Exists(exportOutPath))
                {
                    {
                        Directory.CreateDirectory(exportOutPath);
                    }
                }

                foreach (WzDirectory subdir in ((WzDirectory)currObj).WzDirectories)
                {
                    {
                        ExportRecursion(subdir, exportOutPath + subdir.Name + @"\");
                    }
                }

                foreach (WzImage subimg in ((WzDirectory)currObj).WzImages)
                {
                    {
                        ExportRecursion(subimg, exportOutPath + subimg.Name + @"\");
                    }
                }
            }
            else if (currObj is WzCanvasProperty)
            {
                Bitmap bmp = ((WzCanvasProperty)currObj).PngProperty.GetPNG(false);
                string path = exportOutPath + currObj.Name + ".png";
                bmp.Save(path, ImageFormat.Png);
                //curr++;
            }
            else if (currObj is WzSoundProperty)
            {
                string path = exportOutPath + currObj.Name + ".mp3";
                ((WzSoundProperty)currObj).SaveToFile(path);
            }
            else if (currObj is WzImage)
            {
                exportOutPath += currObj.Name + @"\";
                if (!Directory.Exists(exportOutPath))
                {
                    {
                        Directory.CreateDirectory(exportOutPath);
                    }
                }

                bool parse = ((WzImage)currObj).Parsed || ((WzImage)currObj).Changed;
                if (!parse)
                {
                    {
                        ((WzImage)currObj).ParseImage();
                    }
                }

                foreach (WzImageProperty subprop in ((IPropertyContainer)currObj).WzProperties)
                {
                    {
                        ExportRecursion(subprop, exportOutPath);
                    }
                }

                if (!parse)
                {
                    {
                        ((WzImage)currObj).UnparseImage();
                    }
                }

                curr++;
            }
            else if (currObj is IPropertyContainer)
            {
                exportOutPath += currObj.Name + ".";
                foreach (WzImageProperty subprop in ((IPropertyContainer)currObj).WzProperties)
                {
                    {
                        ExportRecursion(subprop, exportOutPath);
                    }
                }
            }
            else if (currObj is WzUOLProperty)
            {
                {
                    ExportRecursion(((WzUOLProperty)currObj).LinkValue, exportOutPath);
                }
            }
        }
    }

    public class WzClassicXmlSerializer : WzXmlSerializer, IWzImageSerializer
    {
        public WzClassicXmlSerializer(int indentation, LineBreak lineBreakType, bool exportbase64)
            : base(indentation, lineBreakType)
        { ExportBase64Data = exportbase64; }

        private void ExportXmlInternal(WzImage img, string path)
        {
            bool parsed = img.Parsed || img.Changed;
            if (!parsed)
            {
                {
                    img.ParseImage();
                }
            }

            curr++;
            TextWriter tw = new StreamWriter(path);
            tw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" + lineBreak);
            tw.Write("<imgdir name=\"" + XmlUtil.SanitizeText(img.Name) + "\">" + lineBreak);
            foreach (WzImageProperty property in img.WzProperties)
            {
                {
                    WritePropertyToXML(tw, indent, property);
                }
            }

            tw.Write("</imgdir>" + lineBreak);
            tw.Close();
            if (!parsed)
            {
                {
                    img.UnparseImage();
                }
            }
        }

        private void ExportDirXmlInternal(WzDirectory dir, string path)
        {
            if (!Directory.Exists(path))
            {
                {
                    CreateDirSafe(ref path);
                }
            }

            if (path.Substring(path.Length - 1) != @"\")
            {
                {
                    path += @"\";
                }
            }

            foreach (WzDirectory subdir in dir.WzDirectories)
            {
                {
                    ExportDirXmlInternal(subdir, path + subdir.name + @"\");
                }
            }

            foreach (WzImage subimg in dir.WzImages)
            {
                {
                    ExportXmlInternal(subimg, path + subimg.Name + ".xml");
                }
            }
        }

        public void SerializeImage(WzImage img, string path)
        {
            total = 1; curr = 0;
            if (Path.GetExtension(path) != ".xml")
            {
                {
                    path += ".xml";
                }
            }

            ExportXmlInternal(img, path);
        }

        public void SerializeDirectory(WzDirectory dir, string path)
        {
            total = dir.CountImages(); curr = 0;
            ExportDirXmlInternal(dir, path);
        }

        public void SerializeFile(WzFile file, string path)
        {
            SerializeDirectory(file.WzDirectory, path);
        }
    }

    public class WzNewXmlSerializer : WzXmlSerializer
    {
        public WzNewXmlSerializer(int indentation, LineBreak lineBreakType)
            : base(indentation, lineBreakType)
        { }

        internal void DumpImageToXML(TextWriter tw, string depth, WzImage img)
        {
            bool parsed = img.Parsed || img.Changed;
            if (!parsed)
            {
                {
                    img.ParseImage();
                }
            }

            curr++;
            tw.Write(depth + "<wzimg name=\"" + XmlUtil.SanitizeText(img.Name) + "\">" + lineBreak);
            string newDepth = depth + indent;
            foreach (WzImageProperty property in img.WzProperties)
            {
                {
                    WritePropertyToXML(tw, newDepth, property);
                }
            }

            tw.Write(depth + "</wzimg>");
            if (!parsed)
            {
                {
                    img.UnparseImage();
                }
            }
        }

        internal void DumpDirectoryToXML(TextWriter tw, string depth, WzDirectory dir)
        {
            tw.Write(depth + "<wzdir name=\"" + XmlUtil.SanitizeText(dir.Name) + "\">" + lineBreak);
            foreach (WzDirectory subdir in dir.WzDirectories)
            {
                {
                    DumpDirectoryToXML(tw, depth + indent, subdir);
                }
            }

            foreach (WzImage img in dir.WzImages)
            {
                {
                    DumpImageToXML(tw, depth + indent, img);
                }
            }

            tw.Write(depth + "</wzdir>" + lineBreak);
        }

        public void ExportCombinedXml(List<WzObject> objects, string path)
        {
            total = 1; curr = 0;
            if (Path.GetExtension(path) != ".xml")
            {
                {
                    path += ".xml";
                }
            }

            foreach (WzObject obj in objects)
            {
                if (obj is WzImage)
                {
                    {
                        total++;
                    }
                }
                else if (obj is WzDirectory)
                {
                    {
                        total += ((WzDirectory)obj).CountImages();
                    }
                }
            }

            ExportBase64Data = true;
            TextWriter tw = new StreamWriter(path);
            tw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" + lineBreak);
            tw.Write("<xmldump>" + lineBreak);
            foreach (WzObject obj in objects)
            {
                if (obj is WzDirectory)
                {
                    {
                        DumpDirectoryToXML(tw, indent, (WzDirectory)obj);
                    }
                }
                else if (obj is WzImage)
                {
                    {
                        DumpImageToXML(tw, indent, (WzImage)obj);
                    }
                }
                else if (obj is WzImageProperty)
                {
                    {
                        WritePropertyToXML(tw, indent, (WzImageProperty)obj);
                    }
                }
            }
            tw.Write("</xmldump>" + lineBreak);
            tw.Close();
        }
    }

    public class WzXmlDeserializer : ProgressingWzSerializer
    {
        public static NumberFormatInfo formattingInfo;

        private bool useMemorySaving;
        private byte[] iv;
        private WzImgDeserializer imgDeserializer = new WzImgDeserializer(false);

        public WzXmlDeserializer(bool useMemorySaving, byte[] iv)
        {
            this.useMemorySaving = useMemorySaving;
            this.iv = iv;
        }

        #region Public Functions
        public List<WzObject> ParseXML(string path)
        {
            List<WzObject> result = new List<WzObject>();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement mainElement = (XmlElement)doc.ChildNodes[1];
            curr = 0;
            if (mainElement.Name == "xmldump")
            {
                total = CountImgs(mainElement);
                foreach (XmlElement subelement in mainElement)
                {
                    if (subelement.Name == "wzdir")
                    {
                        {
                            result.Add(ParseXMLWzDir(subelement));
                        }
                    }
                    else if (subelement.Name == "wzimg")
                    {
                        {
                            result.Add(ParseXMLWzImg(subelement));
                        }
                    }
                    else
                    {
                        {
                            throw new InvalidDataException("unknown XML prop " + subelement.Name);
                        }
                    }
                }
            }
            else if (mainElement.Name == "imgdir")
            {
                total = 1;
                result.Add(ParseXMLWzImg(mainElement));
                curr++;
            }
            else
            {
                {
                    throw new InvalidDataException("unknown main XML prop " + mainElement.Name);
                }
            }

            return result;
        }
        #endregion

        #region Internal Functions
        internal int CountImgs(XmlElement element)
        {
            int result = 0;
            foreach (XmlElement subelement in element)
            {
                if (subelement.Name == "wzimg")
                {
                    {
                        result++;
                    }
                }
                else if (subelement.Name == "wzdir")
                {
                    {
                        result += CountImgs(subelement);
                    }
                }
            }
            return result;
        }

        internal WzDirectory ParseXMLWzDir(XmlElement dirElement)
        {
            WzDirectory result = new WzDirectory(dirElement.GetAttribute("name"));
            foreach (XmlElement subelement in dirElement)
            {
                if (subelement.Name == "wzdir")
                {
                    {
                        result.AddDirectory(ParseXMLWzDir(subelement));
                    }
                }
                else if (subelement.Name == "wzimg")
                {
                    {
                        result.AddImage(ParseXMLWzImg(subelement));
                    }
                }
                else
                {
                    {
                        throw new InvalidDataException("unknown XML prop " + subelement.Name);
                    }
                }
            }
            return result;
        }

        internal WzImage ParseXMLWzImg(XmlElement imgElement)
        {
            string name = imgElement.GetAttribute("name");
            WzImage result = new WzImage(name);
            foreach (XmlElement subelement in imgElement)
            {
                {
                    result.WzProperties.Add(ParsePropertyFromXMLElement(subelement));
                }
            }

            result.Changed = true;
            if (this.useMemorySaving)
            {
                string path = Path.GetTempFileName();
                WzBinaryWriter wzWriter = new WzBinaryWriter(File.Create(path), iv);
                result.SaveImage(wzWriter);
                wzWriter.Close();
                result.Dispose();
                result = imgDeserializer.WzImageFromIMGFile(path, iv, name);
            }
            return result;
        }

        internal WzImageProperty ParsePropertyFromXMLElement(XmlElement element)
        {
            switch (element.Name)
            {
                case "imgdir":
                    WzSubProperty sub = new WzSubProperty(element.GetAttribute("name"));
                    foreach (XmlElement subelement in element)
                    {
                        {
                            sub.AddProperty(ParsePropertyFromXMLElement(subelement));
                        }
                    }

                    return sub;

                case "canvas":
                    WzCanvasProperty canvas = new WzCanvasProperty(element.GetAttribute("name"));
                    if (!element.HasAttribute("basedata"))
                    {
                        {
                            throw new NoBase64DataException("no base64 data in canvas element with name " + canvas.Name);
                        }
                    }

                    canvas.PngProperty = new WzPngProperty();
                    MemoryStream pngstream = new MemoryStream(Convert.FromBase64String(element.GetAttribute("basedata")));
                    canvas.PngProperty.SetPNG((Bitmap)Image.FromStream(pngstream));
                    foreach (XmlElement subelement in element)
                    {
                        {
                            canvas.AddProperty(ParsePropertyFromXMLElement(subelement));
                        }
                    }

                    return canvas;

                case "int":
                    WzIntProperty compressedInt = new WzIntProperty(element.GetAttribute("name"), int.Parse(element.GetAttribute("value"), formattingInfo));
                    return compressedInt;

                case "double":
                    WzDoubleProperty doubleProp = new WzDoubleProperty(element.GetAttribute("name"), double.Parse(element.GetAttribute("value"), formattingInfo));
                    return doubleProp;

                case "null":
                    WzNullProperty nullProp = new WzNullProperty(element.GetAttribute("name"));
                    return nullProp;

                case "sound":
                    if (!element.HasAttribute("basedata") || !element.HasAttribute("basehead") || !element.HasAttribute("length"))
                    {
                        {
                            throw new NoBase64DataException("no base64 data in sound element with name " + element.GetAttribute("name"));
                        }
                    }

                    WzSoundProperty sound = new WzSoundProperty(element.GetAttribute("name"),
                        int.Parse(element.GetAttribute("length")),
                        Convert.FromBase64String(element.GetAttribute("basehead")),
                        Convert.FromBase64String(element.GetAttribute("basedata")));
                    return sound;

                case "string":
                    WzStringProperty stringProp = new WzStringProperty(element.GetAttribute("name"), element.GetAttribute("value"));
                    return stringProp;

                case "short":
                    WzShortProperty shortProp = new WzShortProperty(element.GetAttribute("name"), short.Parse(element.GetAttribute("value"), formattingInfo));
                    return shortProp;

                case "long":
                    WzLongProperty longProp = new WzLongProperty(element.GetAttribute("name"), long.Parse(element.GetAttribute("value"), formattingInfo));
                    return longProp;

                case "uol":
                    WzUOLProperty uol = new WzUOLProperty(element.GetAttribute("name"), element.GetAttribute("value"));
                    return uol;

                case "vector":
                    WzVectorProperty vector = new WzVectorProperty(element.GetAttribute("name"), new WzIntProperty("x", Convert.ToInt32(element.GetAttribute("x"))), new WzIntProperty("y", Convert.ToInt32(element.GetAttribute("y"))));
                    return vector;

                case "float":
                    WzFloatProperty floatProp = new WzFloatProperty(element.GetAttribute("name"), float.Parse(element.GetAttribute("value"), formattingInfo));
                    return floatProp;

                case "extended":
                    WzConvexProperty convex = new WzConvexProperty(element.GetAttribute("name"));
                    foreach (XmlElement subelement in element)
                    {
                        {
                            convex.AddProperty(ParsePropertyFromXMLElement(subelement));
                        }
                    }

                    return convex;
            }
            throw new InvalidDataException("unknown XML prop " + element.Name);
        }
        #endregion
    }
}