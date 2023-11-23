using System;
using System.Data;
using NLog;
using Npgsql;

namespace HelloNlog
{
	public static class NLoggerWrapper
	{
		static Logger logger = LogManager.GetCurrentClassLogger();
		public static void ConnectionStateChanged(object sender,StateChangeEventArgs args)
		{
			logger.Info ("Connection Original state: " + args.OriginalState);
			logger.Info ("Connection Current state: " + args.CurrentState);
		}

		public static void Warn(string message){
			logger.Warn (message);
		}

		public static void Trace(string message){
			logger.Trace (message);
		}

		public static void LogException(Exception ex)
		{
			if (ex is ArgumentException)
				logger.Warn (ex.Message);
			else
			if (ex is NpgsqlException)
				logger.Error (ex.Message);
			else
				logger.Fatal (ex.Message);
		}
	}
}

