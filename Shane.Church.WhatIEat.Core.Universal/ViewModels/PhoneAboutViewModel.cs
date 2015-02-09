﻿using Shane.Church.WhatIEat.Core.ViewModels;
using Shane.Church.WhatIEat.Core.Universal.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shane.Church.WhatIEat.Core.Universal.ViewModels
{
	public class UniversalAboutViewModel : AboutViewModel
	{
		public override void Initialize()
		{
			RateThisAppCommand = new RateThisAppCommand();
			SendAnEmailCommand = new SendAnEmailCommand();
            OtherAppsCommand = new OtherAppsCommand();

			var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
			Version = versionAttrib.Version.ToString();
		}
	}
}