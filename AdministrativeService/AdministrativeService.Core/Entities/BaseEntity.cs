namespace AdministrativeService.Core.Entities
{
	/// <summary>
	/// Base entity.
	/// </summary>
	public abstract class BaseEntity
	{
		/// <summary>
		/// Entity identificator.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// User who create.
		/// </summary>
		public User? CreatedBy { get; set; }

		/// <summary>
		/// User id who create.
		/// </summary>
		public Guid CreatedById { get; set; }

		/// <summary>
		/// User who last update.
		/// </summary>
		public User? UpdatedBy { get; set; }

		/// <summary>
		/// User id who last update.
		/// </summary>
		public Guid? UpdatedById { get; set; }

		/// <summary>
		/// Date of creation.
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Date of last update.
		/// </summary>
		public DateTime? UpdatedAt { get; set; }
	}
}
