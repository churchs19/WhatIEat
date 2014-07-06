using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;
using Shane.Church.WhatIEat.Strings;

namespace Shane.Church.WhatIEat.Core.WP.Commands
{
	public class SendAnEmailCommand : ICommand
	{
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged
		{
			add { }
			remove { }
		}

		public void Execute(object parameter)
		{
			EmailComposeTask emailTask = new EmailComposeTask();
			emailTask.To = "shane@s-church.net";
			emailTask.Subject = Resources.TechnicalSupportEmailSubject;
			emailTask.Show();
		}
	}
}
