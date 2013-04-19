using Shane.Church.WhatIEat.Core.WP.Data;
using System.Data.Linq;

public class PhoneDataContext : DataContext
{
	// Specify the connection string as a static, used in main page and app.xaml.
	public static string DBConnectionString = "Data Source=isostore:/WhatIEat.sdf";
	public static string DBFileName = "WhatIEat.sdf";

	// Pass the connection string to the base class.
	public PhoneDataContext()
		: base(DBConnectionString)
	{
		if (!this.DatabaseExists())
			this.CreateDatabase();
	}

	public Table<PhoneEntry> Entries;
}