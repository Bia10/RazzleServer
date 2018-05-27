﻿using System;

namespace RazzleServer.Common.Crypto
{
    /// <summary>
    /// Cipher class used for encrypting and decrypting maple packet data
    /// </summary>
    public class MapleCipher
    {
        /// <summary>
		/// AES transformer
		/// </summary>
		private FastAes Transformer { get; }

        /// <summary>
        /// General locker to prevent multithreading
        /// </summary>
        private volatile object _locker = new object();

        /// <summary>
        /// Vector to use in the MapleCrypto
        /// </summary>
        private InitializationVector MapleIv { get; set; }

        /// <summary>
        /// Gameversion of the current <see cref="MapleCipher"/> instance
        /// </summary>
        public ushort GameVersion { get; }

        /// <summary>
        /// Bool stating if the current instance received its handshake
        /// </summary>
        public bool Handshaken { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="MapleCipher"/>
        /// </summary>
        /// <param name="currentGameVersion">The current MapleStory version</param>
        /// <param name="aesKey">AESKey for the current MapleStory version</param>
        public MapleCipher(ushort currentGameVersion, ulong aesKey)
        {
            Handshaken = false;
            GameVersion = currentGameVersion;
            Transformer = new FastAes(ExpandKey(aesKey));
        }

        /// <summary>
        /// Encrypts packet data
        /// </summary>
        public ushort? Encrypt(ref byte[] data, bool toClient)
        {
            if (!Handshaken || MapleIv == null)
            {
                return null;
            }

            ushort? ret;


            var newData = new byte[data.Length + 4];

            if (toClient)
            {
                WriteHeaderToClient(newData);
            }
            else
            {
                WriteHeaderToServer(newData);
            }

            EncryptShanda(data);

            lock (_locker)
            {
                Transform(data);
                ret = MapleIv.MustSend ? MapleIv.Loword : null as ushort?;
            }

            Buffer.BlockCopy(data, 0, newData, 4, data.Length);
            data = newData;

            return ret;
        }

        /// <summary>
        /// Decrypts a maple packet contained in <paramref name="data"/>
        /// </summary>
        /// <param name="data">Data to decrypt</param>
        public void Decrypt(ref byte[] data)
        {
            if (!Handshaken || MapleIv == null)
            {
                return;
            }

            var length = GetPacketLength(data);

            var newData = new byte[length];
            Buffer.BlockCopy(data, 4, newData, 0, length);

            lock (_locker)
            {
                Transform(newData);
            }
            DecryptShanda(newData);
            data = newData;
        }

        /// <summary>
        /// Manually sets the vector for the current instance
        /// </summary>
        public void SetIv(uint iv)
        {
            MapleIv = new InitializationVector(iv);
            Handshaken = true;
        }

        /// <summary>
        /// Handles an handshake for the current instance
        /// </summary>
        public void Handshake(ref byte[] data)
        {
            var length = BitConverter.ToUInt16(data, 0);
            var ret = new byte[length];
            Buffer.BlockCopy(data, 2, ret, 0, ret.Length);
            data = ret;
        }

        /// <summary>
        /// Expands the key we store as long
        /// </summary>
        /// <returns>The expanded key</returns>
        private byte[] ExpandKey(ulong aesKey)
        {
            var expand = BitConverter.GetBytes(aesKey);
            var key = new byte[expand.Length * 4];
            for (var i = 0; i < expand.Length; i++)
            {
                key[i * 4] = expand[i];
            }

            return key;
        }

        /// <summary>
        /// Performs Maplestory's AES algo
        /// </summary>
        private void Transform(byte[] buffer)
        {
            int remaining = buffer.Length,
                length = 0x5B0,
                start = 0,
                index;

            byte[] realIv = new byte[sizeof(int) * 4],
                   ivBytes = MapleIv.Bytes;

            while (remaining > 0)
            {
                for (index = 0; index < realIv.Length; ++index)
                {
                    realIv[index] = ivBytes[index % 4];
                }

                if (remaining < length)
                {
                    length = remaining;
                }

                for (index = start; index < start + length; ++index)
                {
                    if ((index - start) % realIv.Length == 0)
                    {
                        Transformer.TransformBlock(realIv);
                    }

                    buffer[index] ^= realIv[(index - start) % realIv.Length];
                }
                start += length;
                remaining -= length;
                length = 0x5B4;
            }
            MapleIv.Shuffle();
        }

        /// <summary>
        /// Creates a packet header for outgoing data
        /// </summary>
        private void WriteHeaderToServer(byte[] data)
        {
            var length = data.Length - 4;
            int a = MapleIv.Hiword;
            a = a ^ GameVersion;
            var b = a ^ length;
            data[0] = (byte)(a % 0x100);
            data[1] = (byte)(a / 0x100);
            data[2] = (byte)(b % 0x100);
            data[3] = (byte)(b / 0x100);
        }

        /// <summary>
        /// Creates a packet header for incoming data
        /// </summary>
        private void WriteHeaderToClient(byte[] data)
        {
            var length = data.Length - 4;
            var a = MapleIv.Hiword ^ -(GameVersion + 1);
            var b = a ^ length;
            data[0] = (byte)(a % 0x100);
            data[1] = (byte)((a - data[0]) / 0x100);
            data[2] = (byte)(b ^ 0x100);
            data[3] = (byte)((b - data[2]) / 0x100);
        }

        /// <summary>
        /// Gets the length of <paramref name="data"/>
        /// </summary>
        /// <param name="data">Data to check</param>
        /// <returns>Length of <paramref name="data"/></returns>
        public int GetPacketLength(byte[] data) => (data[0] + (data[1] << 8)) ^ (data[2] + (data[3] << 8));

        public bool CheckHeader(in byte[] data, bool toClient) => toClient
                    ? CheckHeaderToClient(data)
                    : CheckHeaderToServer(data);

        public bool CheckHeaderToServer(in byte[] data)
        {
            var encodedVersion = (ushort)(data[0] + (data[1] << 8));
            var version = (ushort)(encodedVersion ^ MapleIv.Hiword);
            return version == GameVersion;
        }

        public bool CheckHeaderToClient(in byte[] data)
        {
            var encodedVersion = (ushort)(data[0] + (data[1] << 8));
            var version = (ushort)-((encodedVersion ^ MapleIv.Hiword) + 1);
            return version == GameVersion;
        }

        /// <summary>
        /// Decrypts <paramref name="buffer"/> using the custom MapleStory shanda
        /// </summary>
        private void DecryptShanda(byte[] buffer)
        {
            int length = buffer.Length, i;
            byte xorKey, save, len, temp;
            for (var passes = 0; passes < 3; passes++)
            {
                xorKey = 0;
                save = 0;
                len = (byte)(length & 0xFF);
                for (i = length - 1; i >= 0; --i)
                {
                    temp = (byte)(Rol(buffer[i], 3) ^ 0x13);
                    save = temp;
                    temp = Ror((byte)((xorKey ^ temp) - len), 4);
                    xorKey = save;
                    buffer[i] = temp;
                    --len;
                }

                xorKey = 0;
                len = (byte)(length & 0xFF);
                for (i = 0; i < length; ++i)
                {
                    temp = Rol((byte)~(buffer[i] - 0x48), len & 0xFF);
                    save = temp;
                    temp = Ror((byte)((xorKey ^ temp) - len), 3);
                    xorKey = save;
                    buffer[i] = temp;
                    --len;
                }
            }
        }

        /// <summary>
        /// Encrypts <paramref name="buffer"/> using the custom MapleStory shanda
        /// </summary>
        private void EncryptShanda(byte[] buffer)
        {
            var length = buffer.Length;
            byte xorKey, len, temp;
            int i;
            for (var passes = 0; passes < 3; passes++)
            {
                xorKey = 0;
                len = (byte)(length & 0xFF);
                for (i = 0; i < length; i++)
                {
                    temp = (byte)((Rol(buffer[i], 3) + len) ^ xorKey);
                    xorKey = temp;
                    temp = (byte)((~Ror(temp, len & 0xFF) & 0xFF) + 0x48);
                    buffer[i] = temp;
                    len--;
                }
                xorKey = 0;
                len = (byte)(length & 0xFF);
                for (i = length - 1; i >= 0; i--)
                {
                    temp = (byte)(xorKey ^ (len + Rol(buffer[i], 4)));
                    xorKey = temp;
                    temp = Ror((byte)(temp ^ 0x13), 3);
                    buffer[i] = temp;
                    len--;
                }
            }
        }

        /// <summary>
        /// Bitwise shift left
        /// </summary>
        private byte Rol(byte b, int count)
        {
            var tmp = b << (count & 7);
            return (byte)(tmp | (tmp >> 8));
        }

        /// <summary>
        /// Bitwise shift right
        /// </summary>
        private byte Ror(byte b, int count)
        {
            var tmp = b << (8 - (count & 7));
            return (byte)(tmp | (tmp >> 8));
        }
    }
}