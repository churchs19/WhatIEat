using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.Core.Services
{
	public class SkyDriveSaveArgs
	{
		public string Filename { get; set; }
		public string Content { get; set; }
		public Encoding Encoding { get; set; }
	}
}
