﻿using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using RazzleServer.Common;
using RazzleServer.Common.Constants;
using RazzleServer.Common.Data;
using RazzleServer.Common.Exceptions;
using RazzleServer.Common.Util;
using RazzleServer.Data;

namespace RazzleServer.Login.Maple
{
    public sealed class LoginAccount : IMapleSavable
    {
        public LoginClient Client { get; }
        public int Id { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public Gender Gender { get; set; }
        public string Pin { get; set; }
        public BanReasonType BanReason { get; set; }
        public bool IsMaster { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime Creation { get; set; }
        public int MaxCharacters { get; set; }
        public static DateTime PermanentBanDate => DateTime.Now.AddYears(2);
        private bool Assigned { get; set; }
        private readonly ILogger _log = LogManager.CreateLogger<LoginAccount>();

        public LoginAccount(LoginClient client)
        {
            Client = client;
        }

        public void Load()
        {
            using (var dbContext = new MapleDbContext())
            {
                var account = dbContext.Accounts.FirstOrDefault(x => x.Username == Username);

                if (account == null)
                {
                    throw new NoAccountException();
                }

                Id = account.Id;
                Username = account.Username;
                Gender = (Gender)account.Gender;
                Password = account.Password;
                Salt = account.Salt;
                MaxCharacters = account.MaxCharacters;
                Birthday = account.Birthday;
                Creation = account.Creation;
                Pin = account.Pin;
                BanReason = (BanReasonType)account.BanReason;
                IsMaster = account.IsMaster;
            }
        }

        public void Save()
        {
            using (var dbContext = new MapleDbContext())
            {
                var account = dbContext.Accounts.Find(Id);

                if (account == null)
                {
                    _log.LogError($"Account does not exists with Id [{Id}]");
                    return;
                }

                account.Username = Username;
                account.Salt = Salt;
                account.Password = Password;
                account.Gender = (byte)Gender;
                account.Pin = Pin;
                account.Birthday = Birthday;
                account.Creation = Creation;
                account.BanReason = (byte)BanReason;
                account.IsMaster = IsMaster;
                account.MaxCharacters = MaxCharacters;

                dbContext.SaveChanges();
            }
        }

        public void Create()
        {
            using (var dbContext = new MapleDbContext())
            {
                var account = dbContext.Accounts.FirstOrDefault(x => x.Username == Username);

                if (account != null)
                {
                    _log.LogError($"Error creating account - account already exists with username [{Username}]");
                    return;
                }

                account = new AccountEntity
                {
                    Username = Username,
                    Salt = Salt,
                    Password = Password,
                    Gender = (byte)Gender,
                    Pin = Pin,
                    Birthday = Birthday,
                    Creation = Creation,
                    BanReason = (byte)BanReason,
                    IsMaster = IsMaster,
                    MaxCharacters = MaxCharacters
                };

                dbContext.Accounts.Add(account);
                dbContext.SaveChanges();
                Id = account.Id;
            }
        }
    }
}