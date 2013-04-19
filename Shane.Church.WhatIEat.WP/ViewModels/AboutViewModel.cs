using System;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Shane.Church.WhatIEat.WP.Commands;
using GalaSoft.MvvmLight;

namespace Shane.Church.WhatIEat.WP.ViewModels
{
	public class AboutViewModel : ObservableObject
	{
		public ICommand RateThisAppCommand
		{
			get;
			private set;
		}

		public ICommand SendAnEmailCommand
		{
			get;
			private set;
		}

		public AboutViewModel()
		{
			RateThisAppCommand = new RateThisAppCommand();
			SendAnEmailCommand = new SendAnEmailCommand();

			Version = "1.0";
		}

		private string _version;
		public string Version
		{
			get { return "Version " + _version; }
			set
			{
				Set(() => Version, ref _version, value);
			}
		}
	}
}
