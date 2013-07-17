using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;

namespace Shane.Church.WhatIEat.WP
{
	public partial class About : PhoneApplicationPage
	{
		public About()
		{
			InitializeComponent();

			this.DataContext = KernelService.Kernel.Get<AboutViewModel>();
		}
	}
}