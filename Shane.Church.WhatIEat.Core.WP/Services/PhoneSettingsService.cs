﻿using Shane.Church.WhatIEat.Core.Services;
using System;
using System.IO.IsolatedStorage;

namespace Shane.Church.WhatIEat.Core.WP.Services
{
	public class PhoneSettingsService : ISettingsService
	{
		IsolatedStorageSettings _settings;
		ILoggingService _log;

		public PhoneSettingsService(ILoggingService log)
		{
			if (log == null) { throw new ArgumentNullException("log"); }
			_log = log;
			_settings = IsolatedStorageSettings.ApplicationSettings;
		}

		public bool SaveSetting<T>(T value, string key)
		{
			bool valueChanged = false;

			// If the key exists
			if (_settings.Contains(key))
			{
				// If the value has changed
				if (_settings[key] is T)
				{
					T currentVal = (T)_settings[key];
					if (!currentVal.Equals(value))
					{
						// Store the new value
						_settings[key] = value;
						valueChanged = true;
					}
				}
				else
				{
					_settings[key] = value;
					valueChanged = true;
				}
			}
			// Otherwise create the key.
			else
			{
				_settings.Add(key, value);
				valueChanged = true;
			}
			if (valueChanged)
				_settings.Save();
			return valueChanged;
		}

		public T LoadSetting<T>(string key)
		{
			try
			{
				// If the key exists, retrieve the value.
				if (_settings.Contains(key))
				{
					return (T)_settings[key];
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
			if (_settings.Contains(key))
			{
				_settings.Remove(key);
				_settings.Save();
			}
			return removed;
		}
	}
}
