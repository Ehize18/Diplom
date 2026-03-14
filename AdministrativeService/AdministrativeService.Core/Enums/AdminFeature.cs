namespace AdministrativeService.Core.Enums
{
	/// <summary>
	/// Admin feature.
	/// </summary>
	[Flags]
	public enum AdminFeature
	{
		None = 0,
		CanSupport = 1,
		CanManageGood = 2,
		CanManageAdmin = 4,
		CanManageAnalytics = 8,
		CanAll = 15
	}
}
