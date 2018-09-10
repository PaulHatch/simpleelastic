namespace SimpleElastic
{
    /// <summary>
    /// Indicates the type of bulk action to perform.
    /// </summary>
    public enum BulkActionType
    {
        /// <summary>
        /// Indicate index (create or update) action type.
        /// </summary>
        Index,
        /// <summary>
        /// Indicates a create-only action type.
        /// </summary>
        Create,
        /// <summary>
        /// Indicates a delete action type. If this action is used, the documents list
        /// is expected to be a list of keys to be deleted.
        /// </summary>
        Delete,
        /// <summary>
        /// Indicates an update-only action type which will not create documents if they do not exist.
        /// </summary>
        Update
    }
}