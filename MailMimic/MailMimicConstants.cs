namespace MailMimic;

/// <summary>
/// MailMimic Constants
/// </summary>
public static class MailMimicConstants
{
    /// <summary>
    /// Content-Disposition specified in RFC 2183.
    /// </summary>
    public static class ContentDispositions
    {
        /// <summary>
        /// The user should take additional action to view the entity.
        /// </summary>
        public const string Attachment = "attachment";

        /// <summary>
        /// Indicates that the entity should be immediately displayed to the user.
        /// </summary>
        public const string Inline = "inline";

        /// <summary>
        /// The filename parameter can be used to suggest a filename for storing the bodypart, if the user wishes to store it in an external file.
        /// </summary>
        public const string Filename = "filename";
    }
}
