using System;
using System.ComponentModel.DataAnnotations;

namespace RazzleServer.Data
{
    public class TeleportRockEntity
    {
        [Key]
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public int MapId { get; set; }
        
        public CharacterEntity Character { get; set; }
    }
}
