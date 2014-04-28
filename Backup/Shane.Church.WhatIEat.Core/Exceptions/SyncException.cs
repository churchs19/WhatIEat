using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.Core.Exceptions
{
	public class SyncException : Exception
	{
		public SyncException() : base() { }
		public SyncException(string message) : base(message) { }
		public SyncException(string message, Exception innerException) : base(message, innerException) { }
	}
}
