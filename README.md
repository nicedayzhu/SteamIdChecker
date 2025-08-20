# Steam ID 检查器

一个用于检查和获取 Steam 用户 ID 的 Windows 平台工具应用程序。

## 功能

- 使用 Steamworks.NET API 获取当前登录用户的 Steam ID
- 检查 Steam 客户端运行状态
- 从注册表获取 Steam 安装信息
- 从配置文件读取 Steam ID 信息
- 提供详细的诊断信息，帮助排查 Steam 连接问题

## 系统要求

- Windows 操作系统
- .NET 9.0 或更高版本
- Steam 客户端
- steam_api64.dll 文件

## 安装和使用

1. 确保您的系统已安装 .NET 9.0 运行时
2. 下载并解压应用程序文件
3. 确保 steam_api64.dll 文件位于应用程序目录中
4. 运行 SteamIdChecker.exe 或使用 `dotnet SteamIdChecker.dll`

## 依赖项

### Steamworks.NET

本项目使用 Steamworks.NET 来与 Steam API 进行交互。有关安装和配置的详细信息，请参阅：
- [Steamworks.NET 安装指南](https://steamworks.github.io/installation/)

### steam_api.dll

steam_api64.dll 文件是 Steamworks SDK 的一部分，可以从以下位置获取：
- [Steamworks SDK redistributable_bin](https://sourceforge.net/p/steamworks-sdk/code/ci/default/tree/redistributable_bin/win64/)

## 项目结构

```
SteamIdChecker/
├── Program.cs          # 主程序文件
├── SteamIdChecker.csproj # 项目配置文件
├── steam_api64.dll     # Steam API 库文件
├── steam_appid.txt     # Steam 应用 ID 文件（自动生成）
└── publish/            # 发布目录
```

## 故障排除

如果遇到问题，请检查以下几点：

1. **Steam 客户端未运行**：确保 Steam 客户端正在运行且您已登录
2. **缺少必要的文件**：确保 steam_api64.dll 和 steam_appid.txt 文件存在
3. **权限问题**：确保应用程序有足够的权限访问 Steam 相关文件和注册表
4. **网络连接**：确保您的计算机已连接到互联网

## 平台支持

本项目目前仅支持 Windows 平台：

- Windows (x64, x86)

在 Windows 平台上，应用程序使用以下方法获取 Steam 信息：

- 使用注册表访问
- 使用 Steam API
- 检查配置文件

## 开发

如果您想修改或扩展此项目，请确保：

1. 安装 .NET 9.0 SDK
2. 添加 Steamworks.NET NuGet 包
3. 确保有适当的 steam_api64.dll 文件用于调试

## 许可证

本项目遵循 MIT 许可证。Steamworks.NET 和 Steam API 的使用受各自许可证的约束。