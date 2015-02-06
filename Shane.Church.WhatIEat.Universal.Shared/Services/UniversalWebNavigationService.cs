using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.System;

namespace Shane.Church.WhatIEat.Universal.Shared.Services
{
	public class UniversalWebNavigationService : IWebNavigationService
	{
		public async void NavigateTo(Uri page)
		{
			await Launcher.LaunchUriAsync(page);
		}
	}
}
