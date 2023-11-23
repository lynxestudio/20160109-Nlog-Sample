using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;

namespace HelloNlog
{
    class Program
    {
		static NpgsqlConnection conn = null;
		static NpgsqlConnectionStringBuilder builder = null;

        static void Main(string[] args)
        {
			Console.WriteLine ("- A simple demo for NLog trying to connect a PostgreSQL Server -");
			Console.WriteLine (Environment.NewLine);
			bool keepGoing = true;
			do {
				try {
					Console.WriteLine("Enter the Connection String or 'Q' to quit > ");
					string connString = Console.ReadLine();
					if(connString.Equals("Q"))
						keepGoing = false;
					else
					{
						builder = new NpgsqlConnectionStringBuilder(connString);
						conn = new NpgsqlConnection(builder.ConnectionString);
						conn.StateChange += NLoggerWrapper.ConnectionStateChanged;
						conn.Open();
						if(conn.State == ConnectionState.Open)
						{
							Console.WriteLine("Please enter a SELECT query or 'Q' to quit >");
							string commandText = Console.ReadLine();
							if(commandText.Equals("Q"))
								keepGoing = false;
							else
							{
								if(!commandText.ToUpper().StartsWith("SELECT"))
									NLoggerWrapper.Warn("Enter a query with a Select statement");
								else
								{
									using(NpgsqlCommand cmd = new NpgsqlCommand(commandText,conn))
									{
										using(NpgsqlDataReader reader = cmd.ExecuteReader()){
											int columns = reader.FieldCount;
											for(var i = 0;i< columns;i++)
												Console.Write("{0} | ",reader.GetName(i));
											Console.WriteLine(Environment.NewLine);
											while(reader.Read())
											{
												
												for(var i = 0;i < columns;i++)
												{
													Console.Write(reader[i].ToString() + "\t");
													if((i + 1) == columns)
														Console.Write(Environment.NewLine);
												}
											}
										}
									}
								}
							}
						}
					}
				} catch (Exception ex) {
					NLoggerWrapper.LogException (ex);
				} finally {
					if (conn != null) {
						if (conn.State == ConnectionState.Open) {
							conn.Close ();
							keepGoing = false;
						}
					}
				}
			} while(keepGoing == true);
        }
			
    }
}
