using Microsoft.Phone.Tasks;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.Core.WP.Services
{
	public class PhoneWebNavigationService : IWebNavigationService
	{
		public void NavigateTo(Uri page)
		{
			WebBrowserTask task = new WebBrowserTask();
			task.Uri = page;
			task.Show();
		}
	}
}
