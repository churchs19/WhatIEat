using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.WP
{
	public class LocalizedStrings
	{
		public LocalizedStrings()
		{
		}

		private static Resources.AppResources localizedResources = new Resources.AppResources();

		public Resources.AppResources LocalizedResources { get { return localizedResources; } }
	}
}
