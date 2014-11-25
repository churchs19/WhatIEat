using System;
using System.Windows.Input;
using Shane.Church.WhatIEat.Strings;

namespace Shane.Church.WhatIEat.Core.Universal.Commands
{
	public class SendAnEmailCommand : ICommand
	{
		public bool CanExecute(object parameter)
		{
			return true;
		}

#pragma warning disable 0067
		public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

		public async void Execute(object parameter)
		{
			var mailto = new Uri(String.Format("mailto:?to=shane@s-church.net&subject={0}", Resources.TechnicalSupportEmailSubject));
			await Windows.System.Launcher.LaunchUriAsync(mailto);
		}
	}
}
