using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.WhatIEat.Core.Win8.Services
{
    public class Win8SettingsService : ISettingsService
    {
        public bool SaveSetting<T>(T value, string key)
        {
            throw new NotImplementedException();
        }

        public T LoadSetting<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool RemoveSetting(string key)
        {
            throw new NotImplementedException();
        }
    }
}
