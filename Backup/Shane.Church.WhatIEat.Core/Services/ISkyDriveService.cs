using System.Threading.Tasks;

namespace Shane.Church.WhatIEat.Core.Services
{
	public interface ISkyDriveService
	{
		Task<bool> SaveToSkyDrive(SkyDriveSaveArgs args);
	}
}
