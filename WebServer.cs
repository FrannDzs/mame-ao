﻿using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace Spludlow.MameAO
{
	public class WebServer
	{
		private MameAOProcessor _AO;
		
		private Task _RunTask = null;

		public WebServer(MameAOProcessor ao)
		{
			_AO = ao;
		}

		public void StartListener()
		{
			if (HttpListener.IsSupported == false)
			{
				Console.WriteLine("!!! Http Listener Is not Supported");
				return;
			}

			HttpListener listener = new HttpListener();
			listener.Prefixes.Add(_AO._ListenAddress);
			listener.Start();

			Task listenTask = new Task(() => {

				while (true)
				{
					HttpListenerContext context = listener.GetContext();

					context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

					using (StreamWriter writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
					{
						try
						{
							if (_RunTask != null && _RunTask.Status != TaskStatus.RanToCompletion)
								throw new ApplicationException("I'm busy.");

							string path = context.Request.Url.AbsolutePath.ToLower();

							switch (path)
							{
								case "/":
									Root(context, writer);
									break;

								case "/command":
									Command(context, writer);
									break;

								case "/api/machines":
									ApiMachines(context, writer);
									break;

								default:
									throw new ApplicationException($"404 {path}");
							}
						}
						catch (ApplicationException e)
						{
							writer.WriteLine(e.ToString());
							context.Response.StatusCode = 400;
						}
						catch (Exception e)
						{
							writer.WriteLine(e.ToString());
							context.Response.StatusCode = 500;
						}
					}	
				}
			});

			listenTask.Start();
		}

		private void Root(HttpListenerContext context, StreamWriter writer)
		{
			string html = File.ReadAllText(@"UI.html", Encoding.UTF8);

			context.Response.Headers["Content-Type"] = "text/html";

			writer.WriteLine(html);
		}

		private void Command(HttpListenerContext context, StreamWriter writer)
		{
			string machine = context.Request.QueryString["machine"];
			string software = context.Request.QueryString["software"];
			string arguments = context.Request.QueryString["arguments"];

			if (machine == null)
				throw new ApplicationException("No machine given.");

			Console.WriteLine();
			Tools.ConsoleHeading(_AO._h1, new string[] {
				"Remote command recieved",
				$"machine: {machine}",
				$"software: {software}",
				$"arguments: {arguments}",
			});
			Console.WriteLine();

			_RunTask = new Task(() => {
				_AO.RunLine($"{machine} {software} {arguments}");
			});
			
			_RunTask.Start();
			
			writer.WriteLine("OK");
		}

		private void ApiMachines(HttpListenerContext context, StreamWriter writer)
		{
			string commandText = "SELECT machine.name, machine.description, machine.year, machine.manufacturer, Count(softwarelist.softwarelist_Id) AS CountOfsoftwarelist_Id " +
				"FROM (machine INNER JOIN driver ON machine.machine_Id = driver.machine_Id) LEFT JOIN softwarelist ON machine.machine_Id = softwarelist.machine_Id " +
				"GROUP BY machine.name " +
				"HAVING (((machine.cloneof) Is Null) AND ((driver.status)='good') AND ((machine.runnable)='yes') AND ((machine.isbios)='no') AND ((machine.isdevice)='no') AND ((machine.ismechanical)='no') AND ((COUNT(softwarelist.softwarelist_Id))=0));";

			DataSet dataSet = new DataSet();

			using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(commandText, _AO._MachineConnection))
			{
				adapter.Fill(dataSet);
			}

			StringBuilder json = new StringBuilder();

			json.AppendLine("{ \"results\": [");

			for (int index = 0; index < dataSet.Tables[0].Rows.Count; ++index)
			{
				DataRow row = dataSet.Tables[0].Rows[index];

				bool last = index == (dataSet.Tables[0].Rows.Count - 1);


				string name = (string)row["name"];
				string description = (string)row["description"];

				description = description.Replace("\"", "\\\"");

				string image = $"https://mame.spludlow.co.uk/snap/machine/{name}.jpg";

				json.Append($"{{ \"name\": \"{name}\", \"description\": \"{description}\", \"image\": \"{image}\" }}");

				json.AppendLine(last == true ? "" : ",");

			}

			json.AppendLine("] }");

			context.Response.Headers["Content-Type"] = "application/json";

			writer.WriteLine(json.ToString());

		}



	}
}
