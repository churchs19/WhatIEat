using GalaSoft.MvvmLight;
using Shane.Church.Utility.Core.Command;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Shane.Church.WhatIEat.Core.ViewModels
{
	public class SettingsViewModel : ObservableObject
	{
		private ISettingsService _settings;
		private SyncService _syncService;

		public SettingsViewModel(ISettingsService settings, SyncService sync)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");
			_settings = settings;
			if (sync == null)
				throw new ArgumentNullException("sync");
			_syncService = sync;

			ConnectCommand = new AsyncRelayCommand(async (o) =>
			{
				if (!_syncService.IsConnected)
				{
					await _syncService.Authenticate();
				}
				else
				{
					_syncService.Disconnect();
				}
			}, null,
			async () =>
			{
				if (SyncEnabled && _syncService.IsConnected)
					SyncCommand.Execute(null);
				RaisePropertyChanged(() => SyncEnabled);
				RaisePropertyChanged(() => IsConnected);
				RaisePropertyChanged(() => ConnectLabel);
				RaisePropertyChanged(() => OnOffText);
			},
			(ex) =>
			{
				//TODO: Handle Error
			});

			SyncCommand = new AsyncRelayCommand(async (o) =>
			{
				SyncRunning = true;
				await _syncService.Sync();
			}, null,
			async () =>
			{
				SyncRunning = false;
			},
			(ex) =>
			{
				//TODO: Handle Error
				SyncRunning = false;
				throw ex;
			});
		}

		public bool SyncEnabled
		{
			get { return _settings.LoadSetting<bool>("SyncEnabled"); }
			set
			{
				_settings.SaveSetting<bool>(value, "SyncEnabled");
				RaisePropertyChanged(() => SyncEnabled);
				RaisePropertyChanged(() => IsConnected);
				RaisePropertyChanged(() => ConnectLabel);
				RaisePropertyChanged(() => OnOffText);
			}
		}

		public bool IsConnected
		{
			get { return _syncService.IsConnected; }
		}

		public string ConnectLabel
		{
			get
			{
				if (!_syncService.IsConnected)
					return Resources.CoreResources.ConnectLabel;
				else
					return Resources.CoreResources.ConnectedLabel;
			}
		}

		private ICommand _connectCommand;
		public ICommand ConnectCommand
		{
			get { return _connectCommand; }
			set
			{
				Set(() => ConnectCommand, ref _connectCommand, value);
			}
		}

		private ICommand _syncCommand;
		public ICommand SyncCommand
		{
			get { return _syncCommand; }
			set
			{
				Set(() => SyncCommand, ref _syncCommand, value);
			}
		}

		private bool _syncRunning;
		public bool SyncRunning
		{
			get { return _syncRunning; }
			set
			{
				Set(() => SyncRunning, ref _syncRunning, value);
			}
		}

		public string OnOffText
		{
			get
			{
				if (SyncEnabled)
					return Resources.CoreResources.OnText;
				else
					return Resources.CoreResources.OffText;
			}
		}
	}
}
