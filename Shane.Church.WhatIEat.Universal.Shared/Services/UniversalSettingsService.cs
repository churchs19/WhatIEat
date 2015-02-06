using Shane.Church.WhatIEat.Core.Services;
using System;
using Windows.Storage;

namespace Shane.Church.WhatIEat.Universal.Shared.Services
{
	public class UniversalSettingsService : ISettingsService
	{
		ApplicationDataContainer _settings;
		ILoggingService _log;

		public UniversalSettingsService(ILoggingService log)
		{
			if (log == null) { throw new ArgumentNullException("log"); }
			_log = log;
			_settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
		}

		public bool SaveSetting<T>(T value, string key)
		{
			bool valueChanged = false;

			// If the key exists
			if (_settings.Values.ContainsKey(key))
			{
				// If the value has changed
				if (_settings.Values[key] is T)
				{
					T currentVal = (T)_settings.Values[key];
					if (!currentVal.Equals(value))
					{
						// Store the new value
						_settings.Values[key] = value;
						valueChanged = true;
					}
				}
				else
				{
					_settings.Values[key] = value;
					valueChanged = true;
				}
			}
			// Otherwise create the key.
			else
			{
				_settings.Values.Add(key, value);
				valueChanged = true;
			}
			return valueChanged;
		}

		public T LoadSetting<T>(string key)
		{
			try
			{
				// If the key exists, retrieve the value.
				if (_settings.Values.ContainsKey(key))
				{
					return (T)_settings.Values[key];
				}
				// Otherwise, use the default value.
				else
				{
					return default(T);
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "LoadSetting Exception");
				return default(T);
			}
		}

		public bool RemoveSetting(string key)
		{
			var removed = false;
			if (_settings.Values.ContainsKey(key))
			{
				_settings.Values.Remove(key);
			}
			return removed;
		}
	}
}
