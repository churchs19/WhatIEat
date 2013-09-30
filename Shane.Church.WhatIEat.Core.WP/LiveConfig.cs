using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.Core.WP
{
	public static class LiveConfig
	{
		public static readonly string ClientId = "00000000440EEC7F";
		public static readonly string[] Scopes = new string[3] { "wl.basic", "wl.offline_access", "wl.skydrive_update" };
	}
}
