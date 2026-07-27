using BearPlan.Core.Extensions;

namespace BearPlan.Core.Helper;

/// <summary>
/// 图片操作帮助类
/// </summary>
public static class ImgHelper
{
    /// <summary>
    /// 将图片字节数组转为可直接用于前端 img 标签的 data URL，
    /// 即添加 data:image/jpg;base64, 前缀
    /// </summary>
    /// <param name="bytes">图片字节数据</param>
    /// <returns>带 data:image/jpg;base64, 前缀的 base64 字符串</returns>
    public static string ToBase64StringUrl(byte[] bytes)
    {
        return "data:image/jpg;base64," + bytes.ToBase64String();
    }
}
