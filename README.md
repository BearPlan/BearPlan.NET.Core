# BearPlan.Core

> BearPlan 的通用框架核心库 —— 抽离权限、缓存、日志、ORM 等通用能力，业务无关、开箱即用，可作为任意 .NET 项目的底层基座。

## 定位

`BearPlan.Core` 承载与具体业务无关的框架级基础设施，不依赖任何业务实体或服务接口。任何 .NET / C# 应用程序都可以复用。它作为 [BearPlan](https://gitee.com/ByteXiong/BearPlan) 项目体系的核心层，同时设计为可被任意项目以子模块或 NuGet 包的形式引入。

## 包含内容

| 模块 | 说明 |
| ---- | ---- |
| `App` / `Internal` | 全局应用入口，统一的服务定位器（GetService / GetOptions 等） |
| `Attributes` | 通用特性：缓存、事务、权限、响应格式化、多库路由标记、Redis 订阅等 |
| `Aop` | AOP 拦截器（缓存拦截） |
| `Caches` | 缓存抽象与实现：`ICache`、Redis、分布式缓存、Redis 消息队列 |
| `ConfigOptions` | 强类型配置选项：JWT、Redis、CORS、数据库连接、Serilog、Swagger、中间件开关等 |
| `ClassLibrary` | 通用算法与工具：一致性哈希、MurmurHash、JSON 转换器、同步集合 |
| `Consts` | 通用常量 |
| `DI` | 依赖注入基础设施：拦截器、过滤器、Autofac 辅助、DI 标记接口 |
| `Enums` | 通用枚举：架构、缓存类型、列类型、部门、字典、性别、菜单、触发器、版本等 |
| `Exception` | 通用异常：`BusException`、`BadRequestException` |
| `Extensions` | 扩展方法：字符串、日期、集合、枚举、表达式、字节流、DataTable 等 |
| `Fonts` | 验证码生成所需的字体资源 |
| `Global` | 全局常量与配置：`AppSettings`、`AuthConstants`、`MimeTypes`、`Mapper` |
| `Helper` | 工具类：文件、日期、加密（BCrypt/RSA）、HTTP、IP、日志、树、Excel、雪花 ID 等 |
| `IdGenerator` | 雪花 ID 生成器 |
| `Mapping` | 对象映射配置（Mapster） |
| `Middleware` | 纯中间件：CORS、IP 限流、真实 IP、MiniProfiler、Swagger 授权、404 等 |
| `Model` | 通用模型契约：审计实体接口、分页参数、操作结果 |
| `MultiLanguage` | 多语言契约（`ILocalizationService`、`Localizer` 资源访问器） |
| `Pager` | 分页相关模型 |
| `Serilog` | Serilog 日志输出扩展 |

## 技术栈

- **运行时**：.NET 10
- **ORM 适配**：SqlSugarCore
- **缓存**：StackExchange.Redis
- **日志**：Serilog（含 Async / Elasticsearch Sink）
- **DI**：Autofac（含 DynamicProxy AOP）
- **对象映射**：Mapster
- **图像**：SixLabors.ImageSharp
- **调度**：Quartz
- **其它**：Newtonsoft.Json、NodaTime、MiniProfiler、Swashbuckle

## 使用方式

### 作为 Git Submodule 引入

```bash
git submodule add https://gitee.com/BearPlan/BearPlan.NET.Core.git BearPlan.Core
```

然后在主解决方案中引用 `BearPlan.Core/BearPlan.Core.csproj`。

### 作为 NuGet 包引入

```xml
<PackageReference Include="BearPlan.Core" Version="1.0.0" />
```

## 开发约束

- 本库**不得引用任何业务项目**（Entity / IBusiness / Repository / 业务实现），保持业务中立。
- 不得依赖具体业务实体或服务接口，确保可独立编译发布。
- 新增内容须判断是否真正通用：与业务强相关的代码应留在业务项目，不进入本库。

## 关联项目

- **BearPlan 主项目**：<https://gitee.com/ByteXiong/BearPlan> —— 基于 BearPlan.Core 构建的全栈脚手架。
- **开发文档**：<https://bear.js.org/>

## 联系方式

| 作者微信 |
| :--: |
| <img src="wx.jpg" alt="微信图片" width="200" /> |
| 备注：框架 |

## 开源协议

[MIT](LICENSE)
