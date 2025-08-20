using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Steamworks;

namespace SteamIdChecker
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Steam ID 检查器");
			Console.WriteLine("=================");

			bool steamApiSuccess = false;

			try
			{
				// 尝试使用Steamworks.NET获取SteamID
				Console.WriteLine("\n方法1: 使用Steamworks.NET API");
				Console.WriteLine("正在尝试连接到Steam...");

				// 检查并创建steam_appid.txt文件
				EnsureSteamAppIdFile();

				// 检查steam_api64.dll是否存在
				string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steam_api64.dll");
				Console.WriteLine($"检查steam_api64.dll是否存在: {File.Exists(dllPath)}");
				if (File.Exists(dllPath))
				{
					Console.WriteLine($"steam_api64.dll路径: {dllPath}");
				}

				// 检查steam_appid.txt是否存在
				string appIdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steam_appid.txt");
				Console.WriteLine($"检查steam_appid.txt是否存在: {File.Exists(appIdPath)}");
				if (File.Exists(appIdPath))
				{
					Console.WriteLine($"steam_appid.txt内容: {File.ReadAllText(appIdPath).Trim()}");
				}

				// 尝试初始化Steam API
				if (SteamAPI.Init())
				{
					steamApiSuccess = true;
					Console.WriteLine("成功连接到Steam！");

					// 获取当前用户的SteamID
					var steamId = SteamUser.GetSteamID();
					if (steamId.m_SteamID != 0)
					{
						Console.WriteLine($"当前登录用户的SteamID: {steamId.m_SteamID}");

						// 获取用户名
						var userName = SteamFriends.GetPersonaName();
						Console.WriteLine($"用户名: {userName}");

						// 获取Steam个人资料URL
						var profileUrl = $"https://steamcommunity.com/profiles/{steamId.m_SteamID}";
						Console.WriteLine($"个人资料链接: {profileUrl}");
					}
					else
					{
						Console.WriteLine("无法获取SteamID。请确保您已登录Steam。");
					}

					// 关闭Steam API
					SteamAPI.Shutdown();
				}
				else
				{
					Console.WriteLine("无法初始化Steam API。");
					Console.WriteLine("\n可能的原因:");
					Console.WriteLine("1. Steam客户端未运行");
					Console.WriteLine("2. 未登录Steam账户");
					Console.WriteLine("3. 缺少必要的Steam SDK文件");
					Console.WriteLine("4. 应用程序没有正确的AppID");
					Console.WriteLine("5. 权限问题");

					// 提供更详细的诊断信息
					Console.WriteLine("\n诊断信息:");
					Console.WriteLine($"当前工作目录: {Environment.CurrentDirectory}");
					Console.WriteLine($"应用程序域基础目录: {AppDomain.CurrentDomain.BaseDirectory}");
					Console.WriteLine($"steam_api64.dll存在: {File.Exists(dllPath)}");
					Console.WriteLine($"steam_appid.txt存在: {File.Exists(appIdPath)}");

					// 检查Steam进程
					bool steamRunning = IsSteamRunning();
					Console.WriteLine($"Steam进程运行中: {steamRunning}");

					// 检查Steam安装路径
					string steamPath = GetSteamInstallationPath();
					Console.WriteLine($"Steam安装路径: {steamPath ?? "未找到"}");

					// 检查是否有用户登录Steam
					CheckSteamLoginStatus();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"使用Steamworks.NET时发生错误: {ex.Message}");
				Console.WriteLine($"错误堆栈: {ex.StackTrace}");
			}

			// 如果Steam API失败，尝试其他方法
			if (!steamApiSuccess)
			{
				Console.WriteLine("\n方法2: 检查Steam安装和登录状态");
				CheckSteamInstallation();

				Console.WriteLine("\n方法3: 尝试从注册表获取Steam信息");
				TryGetSteamInfoFromRegistry();

				Console.WriteLine("\n方法4: 尝试从配置文件获取SteamID");
				TryGetSteamIdFromConfigFiles();
			}

			Console.WriteLine("\n使用说明:");
			Console.WriteLine("1. 确保Steam客户端正在运行");
			Console.WriteLine("2. 确保您已登录Steam账户");
			Console.WriteLine("3. 如果问题仍然存在，请尝试重启Steam客户端");
			Console.WriteLine("4. 确保您的计算机已连接到互联网");

			Console.WriteLine("\n按任意键退出...");
			Console.ReadKey();
		}

		static void CheckSteamInstallation()
		{
			try
			{
				// 检查Steam是否安装
				string steamPath = GetSteamInstallationPath();
				if (!string.IsNullOrEmpty(steamPath))
				{
					Console.WriteLine($"Steam安装路径: {steamPath}");

					// 检查Steam进程是否正在运行
					if (IsSteamRunning())
					{
						Console.WriteLine("Steam客户端正在运行");
					}
					else
					{
						Console.WriteLine("Steam客户端未运行");
					}
				}
				else
				{
					Console.WriteLine("未找到Steam安装");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"检查Steam安装时发生错误: {ex.Message}");
			}
		}

		static void TryGetSteamInfoFromRegistry()
		{
			try
			{
				// 尝试从注册表获取Steam信息
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
				{
					if (key != null)
					{
						object steamPath = key.GetValue("SteamPath");
						object activeUser = key.GetValue("ActiveUser");

						if (steamPath != null)
						{
							Console.WriteLine($"Steam路径: {steamPath}");
						}

						if (activeUser != null)
						{
							Console.WriteLine($"当前活跃用户ID: {activeUser}");

							// 尝试获取用户信息
							string loginUsersPath = Path.Combine(steamPath?.ToString() ?? "", "config", "loginusers.vdf");
							if (File.Exists(loginUsersPath))
							{
								Console.WriteLine($"找到登录用户文件: {loginUsersPath}");
								// 这里可以添加解析VDF文件的代码来获取更多信息
							}
						}
						else
						{
							Console.WriteLine("未找到活跃用户信息");
						}
					}
					else
					{
						Console.WriteLine("未找到Steam注册表项");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"从注册表获取Steam信息时发生错误: {ex.Message}");
			}
		}

		static void TryGetSteamIdFromConfigFiles()
		{
			try
			{
				string steamPath = GetSteamInstallationPath();
				if (string.IsNullOrEmpty(steamPath))
				{
					Console.WriteLine("未找到Steam安装路径");
					return;
				}

				// 尝试从配置文件获取SteamID
				string configPath = Path.Combine(steamPath, "config");
				string loginUsersPath = Path.Combine(configPath, "loginusers.vdf");

				if (File.Exists(loginUsersPath))
				{
					Console.WriteLine($"找到登录用户文件: {loginUsersPath}");

					// 读取文件内容尝试查找SteamID
					string[] lines = File.ReadAllLines(loginUsersPath);
					foreach (string line in lines)
					{
						if (line.Contains("SteamID"))
						{
							Console.WriteLine($"找到SteamID信息: {line.Trim()}");
						}
					}
				}
				else
				{
					Console.WriteLine("未找到登录用户文件");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"从配置文件获取SteamID时发生错误: {ex.Message}");
			}
		}

		static string GetSteamInstallationPath()
		{
			try
			{
				// 从注册表获取Steam安装路径
				using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam") ??
											Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam"))
				{
					if (key != null)
					{
						object installPath = key.GetValue("InstallPath");
						return installPath?.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"获取Steam安装路径时发生错误: {ex.Message}");
			}

			return null;
		}

		static bool IsSteamRunning()
		{
			try
			{
				System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("Steam");
				return processes.Length > 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"检查Steam进程时发生错误: {ex.Message}");
				return false;
			}
		}

		static void EnsureSteamAppIdFile()
		{
			try
			{
				string appIdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steam_appid.txt");

				// 如果文件不存在，创建它
				if (!File.Exists(appIdPath))
				{
					// 使用Spacewar的AppID (480)，这是Steam为开发者提供的测试AppID
					File.WriteAllText(appIdPath, "480");
					Console.WriteLine($"已创建steam_appid.txt文件，AppID: 480");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"创建steam_appid.txt文件时发生错误: {ex.Message}");
			}
		}

		static void CheckSteamLoginStatus()
		{
			try
			{
				string steamPath = GetSteamInstallationPath();
				if (string.IsNullOrEmpty(steamPath))
				{
					Console.WriteLine("未找到Steam安装路径");
					return;
				}

				// 检查loginusers.vdf文件
				string loginUsersPath = Path.Combine(steamPath, "config", "loginusers.vdf");
				if (File.Exists(loginUsersPath))
				{
					Console.WriteLine($"找到登录用户文件: {loginUsersPath}");

					// 读取文件内容
					string content = File.ReadAllText(loginUsersPath);

					// 检查是否有"mostrecent"标记，这表示最近登录的用户
					if (content.Contains("\"mostrecent\"\"1\""))
					{
						Console.WriteLine("检测到最近登录的用户");

						// 尝试提取SteamID
						// VDF文件格式示例: "76561197960287930"
						// {
						//   "76561197960287930"
						//   {
						//     "AccountName" "username"
						//     "PersonaName" "Display Name"
						//     "RememberPassword" "1"
						//     "mostrecent" "1"
						//     "Timestamp" "1234567890"
						//   }
						// }

						// 查找所有SteamID
						var steamIdPattern = "\"(\\d+)\"";
						var matches = System.Text.RegularExpressions.Regex.Matches(content, steamIdPattern);

						foreach (System.Text.RegularExpressions.Match match in matches)
						{
							string steamId = match.Groups[1].Value;
							// 检查这个SteamID是否有"mostrecent"标记
							int steamIdIndex = content.IndexOf(steamId);
							if (steamIdIndex != -1)
							{
								// 查找这个SteamID块中是否有"mostrecent"
								int blockStart = content.IndexOf("{", steamIdIndex);
								int blockEnd = content.IndexOf("}", blockStart);
								if (blockStart != -1 && blockEnd != -1)
								{
									string block = content.Substring(blockStart, blockEnd - blockStart + 1);
									if (block.Contains("\"mostrecent\"\"1\""))
									{
										Console.WriteLine($"最近登录的SteamID: {steamId}");
										break;
									}
								}
							}
						}
					}
					else
					{
						Console.WriteLine("未检测到最近登录的用户，可能没有用户登录Steam");
					}
				}
				else
				{
					Console.WriteLine("未找到登录用户文件");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"检查Steam登录状态时发生错误: {ex.Message}");
			}
		}
	}
}
