using Windows.ApplicationModel;
using Shane.Church.WhatIEat.Core.ViewModels;
using Shane.Church.WhatIEat.Universal.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shane.Church.WhatIEat.Universal.Shared.ViewModels
{
	public class UniversalAboutViewModel : AboutViewModel
	{
		public override void Initialize()
		{
			RateThisAppCommand = new RateThisAppCommand();
			SendAnEmailCommand = new SendAnEmailCommand();
            OtherAppsCommand = new OtherAppsCommand();

			PackageVersion version = Package.Current.Id.Version;

			Version = version.ToString();
		}
	}
}
