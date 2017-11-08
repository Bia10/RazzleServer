using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Logging;
using RazzleServer.Center;
using RazzleServer.Common.Packet;
using RazzleServer.Data;
using RazzleServer.Game.Maple.Characters;
using RazzleServer.Server;

namespace RazzleServer.Login
{
    public class LoginServer : MapleServer<LoginClient>
    {
        public static Dictionary<ClientOperationCode, List<LoginPacketHandler>> PacketHandlers { get; private set; } = new Dictionary<ClientOperationCode, List<LoginPacketHandler>>();

        internal List<Character> GetCharacters(byte worldID, int accountID)
        {
            using (var dbContext = new MapleDbContext())
            {
                var result = new List<Character>();

                var characters = dbContext.Characters
                                          .Where(x => x.AccountID == accountID)
                                          .Where(x => x.WorldID == worldID);

                characters.ToList()
                          .ForEach(x =>
                          {
                              var c = new Character();
                              c.ID = x.ID;
                              c.Load();
                              result.Add(c);
                          });

                return result;
            }
        }

        public LoginServer(ServerManager manager) : base(manager)
        {
            Port = ServerConfig.Instance.LoginPort;
            Start(new IPAddress(new byte[] { 0, 0, 0, 0 }), Port);
        }

        public static void RegisterPacketHandlers()
        {
            var types = Assembly.GetEntryAssembly()
                                .GetTypes()
                                .Where(x => x.IsSubclassOf(typeof(LoginPacketHandler)));

            var handlerCount = 0;

            foreach (var type in types)
            {
                var attributes = type.GetTypeInfo()
                                     .GetCustomAttributes()
                                     .OfType<PacketHandlerAttribute>()
                                     .ToList();

                foreach (var attribute in attributes)
                {
                    var header = attribute.Header;

                    if (!PacketHandlers.ContainsKey(header))
                    {
                        PacketHandlers[header] = new List<LoginPacketHandler>();
                    }

                    handlerCount++;
                    var handler = (LoginPacketHandler)Activator.CreateInstance(type);
                    PacketHandlers[header].Add(handler);
                }
            }
        }
    }
}