using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Shane.Church.WhatIEat.Core.Services;
using Ninject;
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
