using System.Numerics;
using System.Text;

namespace BearPlan.Core.IdGenerator;

/// <summary>
/// Base62 编码：字节序列转 0-9a-zA-Z 紧凑字符串，用于派生标识等需要不透明短串的场景（编码仅需确定性，不与外部 Base62 实现互操作）
/// </summary>
public static class Base62
{
    /// <summary>
    /// 字符集（0-9 a-z A-Z），62 进制的数字表
    /// </summary>
    private const string Charset = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// 将字节数组编码为 Base62 字符串（BigInteger 大数进制转换，同一输入恒得同一输出）
    /// </summary>
    /// <param name="bytes">待编码字节序列</param>
    /// <param name="minLength">最小输出长度：不足时左侧以字符集首位补齐，用于派生标识固定位数</param>
    public static string Encode(byte[] bytes, int minLength = 0)
    {
        // 尾部补零字节，避免最高位为 1 时 BigInteger 按补码解释为负数
        var value = new BigInteger([.. bytes, (byte)0]);
        if (value.IsZero)
            return Charset[0].ToString();

        var digits = new StringBuilder();
        while (value > 0)
        {
            value = BigInteger.DivRem(value, 62, out var remainder);
            digits.Append(Charset[(int)remainder]);
        }

        // 除余取模法低位在前，反转得到高位在前的可读顺序
        for (int left = 0, right = digits.Length - 1; left < right; left++, right--)
            (digits[left], digits[right]) = (digits[right], digits[left]);

        var encoded = digits.ToString();
        return encoded.Length < minLength
            ? encoded.PadLeft(minLength, Charset[0])
            : encoded;
    }
}
