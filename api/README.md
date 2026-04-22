# CDGService 后端API项目

## 项目介绍

CDGService 是一个血透中心集团信息管理系统的后端API项目，提供用户认证、数据管理、业务逻辑处理等功能。

## 技术栈

- **框架**: ASP.NET Core 6.0
- **数据库**: SQL Server
- **ORM**: Entity Framework Core (Code-First)
- **依赖注入**: Autofac
- **API文档**: Swagger
- **认证**: Token-based认证
- **跨域**: CORS
- **序列化**: Newtonsoft.Json

## 环境要求

- .NET Core SDK 6.0 或更高版本
- SQL Server 2016 或更高版本
- Visual Studio 2022 或 Visual Studio Code (推荐)

## 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/freeqi/tuoguan.git
cd tuoguan/api
```

### 2. 配置数据库连接

编辑 `src/CDGService.WebAPI/appsettings.json` 文件，修改数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "sqlserver": "Server=YOUR_SERVER;Database=CDGService;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

### 3. 初始化数据库

项目使用 Code-First 方式管理数据库，首次运行时会自动创建数据库和表结构。

### 4. 运行项目

#### 使用 Visual Studio

1. 打开 `CDGService.sln` 解决方案
2. 设置 `CDGService.WebAPI` 为启动项目
3. 按 F5 运行项目

#### 使用命令行

```bash
# 进入WebAPI项目目录
cd src/CDGService.WebAPI

# 构建项目
dotnet build

# 运行项目
dotnet run
```

### 5. 访问API文档

项目启动后，可以通过以下地址访问 Swagger API 文档：

```
http://localhost:5000/help
```

## 项目结构

```
api/
├── CDGService.Application/      # 应用层，包含业务逻辑
├── CDGService.Data/             # 数据层，包含实体模型和接口
├── CDGService.Store/            # 数据存储层，实现数据访问
├── CDGService.ThirdPartyLib/     # 第三方库
├── CDGService.Utils/            # 工具类
├── SimpleLogger/                # 日志组件
├── dlls/                        # 第三方dll文件
└── src/
    └── CDGService.WebAPI/       # WebAPI项目
        ├── Controllers/         # 控制器
        ├── DataCore/            # 数据核心逻辑
        ├── Datas/               # 数据模型
        ├── Dto/                 # 数据传输对象
        ├── Extenstions/         # 扩展方法
        └── Properties/          # 项目属性
```

## 打包部署

### 1. 发布到本地文件夹

#### 使用 Visual Studio

1. 右键点击 `CDGService.WebAPI` 项目
2. 选择 "发布"
3. 选择 "文件夹" 作为发布目标
4. 点击 "发布" 按钮

#### 使用命令行

```bash
# 进入WebAPI项目目录
cd src/CDGService.WebAPI

# 发布项目
dotnet publish -c Release -o ./publish
```

### 2. 部署到 IIS

1. 在 IIS 中创建新的网站
2. 将发布文件夹的内容复制到网站根目录
3. 配置应用程序池，设置为 ".NET CLR 版本" 为 "无托管代码"
4. 启动网站

### 3. 部署到 Docker

项目支持 Docker 部署，可根据需要创建 Dockerfile。

## 数据库管理

### 数据迁移

项目使用 Entity Framework Core 的迁移功能管理数据库结构：

```bash
# 进入WebAPI项目目录
cd src/CDGService.WebAPI

# 添加迁移
dotnet ef migrations add MigrationName

# 应用迁移
dotnet ef database update
```

### 初始数据

项目启动时会自动初始化以下数据：
- 基础员工信息
- 初始用户账号
- 过期 token 清理

## 注意事项

1. **安全**：生产环境中请修改默认的数据库连接字符串和初始账号密码
2. **性能**：建议在生产环境中配置合适的数据库索引和缓存策略
3. **日志**：系统会在 `Logs` 目录下生成操作日志和错误日志
4. **跨域**：默认允许所有跨域请求，生产环境中请根据实际情况配置

## 常见问题

### 1. 数据库连接失败

- 检查数据库服务器是否运行
- 验证连接字符串是否正确
- 确保数据库用户有足够的权限

### 2. 项目启动失败

- 检查 .NET Core SDK 是否已正确安装
- 验证项目依赖是否已正确恢复
- 查看日志文件了解具体错误信息

### 3. 登录失败

- 检查用户名和密码是否正确
- 验证用户账号是否已启用
- 查看错误日志了解具体原因

## 联系方式

如有任何问题或建议，请联系项目维护人员。

---

**版本**: 2.0
**更新时间**: 2026-04-22