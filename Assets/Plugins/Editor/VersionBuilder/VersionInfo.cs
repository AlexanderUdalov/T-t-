namespace VersionBuilder
{
	public class VersionInfo
    {
        public string Number;
        public string Comment;

        public VersionInfo(string number)
        {
            Number = number;
        }

        public VersionInfo(string number, string comment)
        {
            Number = number;
            Comment = comment;
        }
    }
}