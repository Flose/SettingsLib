namespace FloseCode.SettingsLib
{
	using System;
	
	[Serializable()]
	public class SettingsFileException : System.Exception
	{
		public SettingsFileException() : base()
		{
		}
		
		protected SettingsFileException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		public SettingsFileException(string message) : base(message)
		{
		}

		public SettingsFileException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
