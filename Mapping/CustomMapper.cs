using System.Reflection;
using BearPlan.Core.Attributes;
using Mapster;

namespace BearPlan.Core.Mapping;

/// <summary>
/// 基于 <see cref="AutoMappingAttribute"/> 自动装配的 Mapster 注册器。
/// </summary>
/// <remarks>
/// 扫描指定类型集合或程序集，按类型上的 <see cref="AutoMappingAttribute"/> 注册 (源类型 → 目标类型) 映射。
/// 支持三种构造方式（按推荐度排序）：
/// <list type="bullet">
///   <item>传 <see cref="Type"/> 数组（推荐，直接复用外部已加载的类型缓存，零反射开销）</item>
///   <item>传 <see cref="Assembly"/> 实例（无外部缓存时使用，类型安全）</item>
///   <item>传程序集名（动态场景，内部按名加载）</item>
/// </list>
/// </remarks>
public class CustomMapper : IRegister
{
    private readonly Type[] _types;

    /// <summary>
    /// 按已加载的类型列表创建注册器（推荐）。
    /// </summary>
    /// <remarks>
    /// 仅 <c>Type[]</c> 重载使用 <see cref="params"/>：零参调用 <c>new CustomMapper()</c> 唯一绑定本签名，
    /// <c>Assembly[]</c> 与 <c>string[]</c> 版本通过首参数类型天然区分，三者互不歧义。
    /// </remarks>
    /// <param name="types">需扫描的类型集合，可传外部已缓存的列表（如 <c>GlobalType.ModelTypes.ToArray()</c>）</param>
    public CustomMapper(params Type[] types)
    {
        // 允许传空，Register 时自然扫描不到任何类型，不抛异常
        _types = types ?? Array.Empty<Type>();
    }

    /// <summary>
    /// 按程序集实例创建注册器，内部反射取 public 类型。
    /// </summary>
    /// <param name="firstAssembly">首项程序集（独立列出避免与 <see cref="CustomMapper(Type[])"/> 零参调用歧义）</param>
    /// <param name="additionalAssemblies">其余程序集</param>
    public CustomMapper(Assembly firstAssembly, params Assembly[] additionalAssemblies)
    {
        if (firstAssembly == null) throw new ArgumentNullException(nameof(firstAssembly));
        var asms = new[] { firstAssembly }.Concat(additionalAssemblies ?? Array.Empty<Assembly>());
        _types = asms.SelectMany(a => a.GetTypes().Where(t => t.IsPublic)).ToArray();
    }

    /// <summary>
    /// 按程序集名创建注册器，内部按短名加载后反射取 public 类型。
    /// </summary>
    /// <param name="assemblyName">第一个程序集名（短名即可，如 <c>BearPlan.Models</c>）</param>
    /// <param name="additionalNames">其余程序集名</param>
    public CustomMapper(string assemblyName, params string[] additionalNames)
    {
        if (assemblyName == null) throw new ArgumentNullException(nameof(assemblyName));
        var names = new[] { assemblyName }.Concat(additionalNames ?? Array.Empty<string>());
        _types = names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(LoadAssembly)
            .SelectMany(a => a.GetTypes().Where(t => t.IsPublic))
            .ToArray();
    }

    /// <inheritdoc />
    public void Register(TypeAdapterConfig config)
    {
        // 扫描范围内所有类型，取其 AutoMappingAttribute 声明的映射关系
        var maps = _types
            .Select(t => t.GetCustomAttribute<AutoMappingAttribute>())
            .Where(a => a != null)
            .Select(a => (a!.SourceType, a.TargetType))
            .ToList();

        maps.ForEach(m => config.NewConfig(m.SourceType, m.TargetType));
    }

    private static Assembly LoadAssembly(string name)
    {
        // 优先复用已加载的程序集，避免重复加载引发版本冲突
        var loaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == name);
        return loaded ?? Assembly.Load(name);
    }
}
