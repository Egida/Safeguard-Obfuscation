using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using Confuser.Core;
using Confuser.Core.Project;
using NDesk.Options;

namespace Confuser.CLI
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static int Main(string[] args)
		{
			ConsoleColor original = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			string originalTitle = Console.Title;
			Console.Title = "ConfuserEx";
			int result;
			try
			{
				bool noPause = false;
				bool debug = false;
				string outDir = null;
				List<string> probePaths = new List<string>();
				List<string> plugins = new List<string>();
				OptionSet p = new OptionSet
				{
					{
						"n|nopause",
						"no pause after finishing protection.",
						delegate(string value)
						{
							noPause = (value != null);
						}
					},
					{
						"o|out=",
						"specifies output directory.",
						delegate(string value)
						{
							outDir = value;
						}
					},
					{
						"probe=",
						"specifies probe directory.",
						delegate(string value)
						{
							probePaths.Add(value);
						}
					},
					{
						"plugin=",
						"specifies plugin path.",
						delegate(string value)
						{
							plugins.Add(value);
						}
					},
					{
						"debug",
						"specifies debug symbol generation.",
						delegate(string value)
						{
							debug = (value != null);
						}
					}
				};
				List<string> files;
				try
				{
					files = p.Parse(args);
					bool flag = files.Count == 0;
					if (flag)
					{
						throw new ArgumentException("No input files specified.");
					}
				}
				catch (Exception ex)
				{
					Console.Write("ConfuserEx.CLI: ");
					Console.WriteLine(ex.Message);
					Program.PrintUsage();
					return -1;
				}
				ConfuserParameters parameters = new ConfuserParameters();
				bool flag2 = files.Count == 1 && Path.GetExtension(files[0]) == ".crproj";
				if (flag2)
				{
					ConfuserProject proj = new ConfuserProject();
					try
					{
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.Load(files[0]);
						proj.Load(xmlDoc);
						proj.BaseDirectory = Path.Combine(Path.GetDirectoryName(files[0]), proj.BaseDirectory);
					}
					catch (Exception ex2)
					{
						Program.WriteLineWithColor(ConsoleColor.Red, "Failed to load project:");
						Program.WriteLineWithColor(ConsoleColor.Red, ex2.ToString());
						return -1;
					}
					parameters.Project = proj;
				}
				else
				{
					bool flag3 = string.IsNullOrEmpty(outDir);
					if (flag3)
					{
						Console.WriteLine("ConfuserEx.CLI: No output directory specified.");
						Program.PrintUsage();
						return -1;
					}
					ConfuserProject proj2 = new ConfuserProject();
					bool flag4 = Path.GetExtension(files[files.Count - 1]) == ".crproj";
					if (flag4)
					{
						ConfuserProject templateProj = new ConfuserProject();
						XmlDocument xmlDoc2 = new XmlDocument();
						xmlDoc2.Load(files[files.Count - 1]);
						templateProj.Load(xmlDoc2);
						files.RemoveAt(files.Count - 1);
						foreach (Rule rule in templateProj.Rules)
						{
							proj2.Rules.Add(rule);
						}
					}
					foreach (string input in files)
					{
						proj2.Add(new ProjectModule
						{
							Path = input
						});
					}
					proj2.BaseDirectory = Path.GetDirectoryName(files[0]);
					proj2.OutputDirectory = outDir;
					foreach (string path in probePaths)
					{
						proj2.ProbePaths.Add(path);
					}
					foreach (string path2 in plugins)
					{
						proj2.PluginPaths.Add(path2);
					}
					proj2.Debug = debug;
					parameters.Project = proj2;
				}
				int retVal = Program.RunProject(parameters);
				bool flag5 = Program.NeedPause() && !noPause;
				if (flag5)
				{
					Console.WriteLine("Press any key to continue...");
					Console.ReadKey(true);
				}
				result = retVal;
			}
			finally
			{
				Console.ForegroundColor = original;
				Console.Title = originalTitle;
			}
			return result;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002530 File Offset: 0x00000730
		private static int RunProject(ConfuserParameters parameters)
		{
			Program.ConsoleLogger logger = new Program.ConsoleLogger();
			parameters.Logger = logger;
			Console.Title = "ConfuserEx - Running...";
			ConfuserEngine.Run(parameters, null).Wait();
			return logger.ReturnValue;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002578 File Offset: 0x00000778
		private static bool NeedPause()
		{
			return Debugger.IsAttached || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROMPT"));
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000025A4 File Offset: 0x000007A4
		private static void PrintUsage()
		{
			Program.WriteLine("Usage:");
			Program.WriteLine("Confuser.CLI -n|noPause <project configuration>");
			Program.WriteLine("Confuser.CLI -n|noPause -o|out=<output directory> <modules>");
			Program.WriteLine("    -n|noPause : no pause after finishing protection.");
			Program.WriteLine("    -o|out     : specifies output directory.");
			Program.WriteLine("    -probe     : specifies probe directory.");
			Program.WriteLine("    -plugin    : specifies plugin path.");
			Program.WriteLine("    -debug     : specifies debug symbol generation.");
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000260C File Offset: 0x0000080C
		private static void WriteLineWithColor(ConsoleColor color, string txt)
		{
			ConsoleColor original = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(txt);
			Console.ForegroundColor = original;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002635 File Offset: 0x00000835
		private static void WriteLine(string txt)
		{
			Console.WriteLine(txt);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000263F File Offset: 0x0000083F
		private static void WriteLine()
		{
			Console.WriteLine();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002648 File Offset: 0x00000848
		public Program()
		{
		}

		// Token: 0x0200000A RID: 10
		private class ConsoleLogger : ILogger
		{
			// Token: 0x0600006F RID: 111 RVA: 0x000042EB File Offset: 0x000024EB
			public ConsoleLogger()
			{
				this.begin = DateTime.Now;
			}

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x06000070 RID: 112 RVA: 0x00004300 File Offset: 0x00002500
			// (set) Token: 0x06000071 RID: 113 RVA: 0x00004308 File Offset: 0x00002508
			public int ReturnValue
			{
				[CompilerGenerated]
				get
				{
					return this.<ReturnValue>k__BackingField;
				}
				[CompilerGenerated]
				private set
				{
					this.<ReturnValue>k__BackingField = value;
				}
			}

			// Token: 0x06000072 RID: 114 RVA: 0x00004311 File Offset: 0x00002511
			public void Debug(string msg)
			{
				Program.WriteLineWithColor(ConsoleColor.Gray, "[DEBUG] " + msg);
			}

			// Token: 0x06000073 RID: 115 RVA: 0x00004326 File Offset: 0x00002526
			public void DebugFormat(string format, params object[] args)
			{
				Program.WriteLineWithColor(ConsoleColor.Gray, "[DEBUG] " + string.Format(format, args));
			}

			// Token: 0x06000074 RID: 116 RVA: 0x00004341 File Offset: 0x00002541
			public void Info(string msg)
			{
				Program.WriteLineWithColor(ConsoleColor.White, " [INFO] " + msg);
			}

			// Token: 0x06000075 RID: 117 RVA: 0x00004357 File Offset: 0x00002557
			public void InfoFormat(string format, params object[] args)
			{
				Program.WriteLineWithColor(ConsoleColor.White, " [INFO] " + string.Format(format, args));
			}

			// Token: 0x06000076 RID: 118 RVA: 0x00004373 File Offset: 0x00002573
			public void Warn(string msg)
			{
				Program.WriteLineWithColor(ConsoleColor.Yellow, " [WARN] " + msg);
			}

			// Token: 0x06000077 RID: 119 RVA: 0x00004389 File Offset: 0x00002589
			public void WarnFormat(string format, params object[] args)
			{
				Program.WriteLineWithColor(ConsoleColor.Yellow, " [WARN] " + string.Format(format, args));
			}

			// Token: 0x06000078 RID: 120 RVA: 0x000043A5 File Offset: 0x000025A5
			public void WarnException(string msg, Exception ex)
			{
				Program.WriteLineWithColor(ConsoleColor.Yellow, " [WARN] " + msg);
				Program.WriteLineWithColor(ConsoleColor.Yellow, "Exception: " + ex);
			}

			// Token: 0x06000079 RID: 121 RVA: 0x000043CE File Offset: 0x000025CE
			public void Error(string msg)
			{
				Program.WriteLineWithColor(ConsoleColor.Red, "[ERROR] " + msg);
			}

			// Token: 0x0600007A RID: 122 RVA: 0x000043E4 File Offset: 0x000025E4
			public void ErrorFormat(string format, params object[] args)
			{
				Program.WriteLineWithColor(ConsoleColor.Red, "[ERROR] " + string.Format(format, args));
			}

			// Token: 0x0600007B RID: 123 RVA: 0x00004400 File Offset: 0x00002600
			public void ErrorException(string msg, Exception ex)
			{
				Program.WriteLineWithColor(ConsoleColor.Red, "[ERROR] " + msg);
				Program.WriteLineWithColor(ConsoleColor.Red, "Exception: " + ex);
			}

			// Token: 0x0600007C RID: 124 RVA: 0x00004429 File Offset: 0x00002629
			public void Progress(int progress, int overall)
			{
			}

			// Token: 0x0600007D RID: 125 RVA: 0x00004429 File Offset: 0x00002629
			public void EndProgress()
			{
			}

			// Token: 0x0600007E RID: 126 RVA: 0x0000442C File Offset: 0x0000262C
			public void Finish(bool successful)
			{
				DateTime now = DateTime.Now;
				string timeString = string.Format("at {0}, {1}:{2:d2} elapsed.", now.ToShortTimeString(), (int)now.Subtract(this.begin).TotalMinutes, now.Subtract(this.begin).Seconds);
				if (successful)
				{
					Console.Title = "ConfuserEx - Success";
					Program.WriteLineWithColor(ConsoleColor.Green, "Finished " + timeString);
					this.ReturnValue = 0;
				}
				else
				{
					Console.Title = "ConfuserEx - Fail";
					Program.WriteLineWithColor(ConsoleColor.Red, "Failed " + timeString);
					this.ReturnValue = 1;
				}
			}

			// Token: 0x04000017 RID: 23
			private readonly DateTime begin;

			// Token: 0x04000018 RID: 24
			[CompilerGenerated]
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private int <ReturnValue>k__BackingField;
		}

		// Token: 0x0200000B RID: 11
		[CompilerGenerated]
		private sealed class <>c__DisplayClass0_0
		{
			// Token: 0x0600007F RID: 127 RVA: 0x00002648 File Offset: 0x00000848
			public <>c__DisplayClass0_0()
			{
			}

			// Token: 0x06000080 RID: 128 RVA: 0x000044DE File Offset: 0x000026DE
			internal void <Main>b__0(string value)
			{
				this.noPause = (value != null);
			}

			// Token: 0x06000081 RID: 129 RVA: 0x000044EB File Offset: 0x000026EB
			internal void <Main>b__1(string value)
			{
				this.outDir = value;
			}

			// Token: 0x06000082 RID: 130 RVA: 0x000044F5 File Offset: 0x000026F5
			internal void <Main>b__2(string value)
			{
				this.probePaths.Add(value);
			}

			// Token: 0x06000083 RID: 131 RVA: 0x00004505 File Offset: 0x00002705
			internal void <Main>b__3(string value)
			{
				this.plugins.Add(value);
			}

			// Token: 0x06000084 RID: 132 RVA: 0x00004515 File Offset: 0x00002715
			internal void <Main>b__4(string value)
			{
				this.debug = (value != null);
			}

			// Token: 0x04000019 RID: 25
			public bool noPause;

			// Token: 0x0400001A RID: 26
			public string outDir;

			// Token: 0x0400001B RID: 27
			public List<string> probePaths;

			// Token: 0x0400001C RID: 28
			public List<string> plugins;

			// Token: 0x0400001D RID: 29
			public bool debug;
		}
	}
}
