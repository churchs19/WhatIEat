using Microsoft.WindowsAzure.MobileServices;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace Shane.Church.WhatIEat.Core.Win8.Services
{
    public class Win8SyncService : SyncService
    {
        public Win8SyncService(IMobileServiceClient client, ISettingsService settings, IRepository<IEntry> entries)
            : base(client, settings, entries)
        {

        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> IsNetworkConnected()
        {
            throw new NotImplementedException();
        }

        public override Task<Microsoft.WindowsAzure.MobileServices.MobileServiceUser> AuthenticateUser()
        {
            throw new NotImplementedException();
        }
    }
}
