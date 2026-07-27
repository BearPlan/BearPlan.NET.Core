using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BearPlan.Core.Helper;

public static class ConsoleHelper
{
    public static void WriteLine() => Console.Out.WriteLine();

    /// <summary>
    /// 打印控制台信息
    /// </summary>
    /// <param name="str">待打印的字符串</param>
    /// <param name="color">想要打印的颜色</param>
    public static void WriteLine(string str, ConsoleColor color = ConsoleColor.White)
    {
        ConsoleColor currentForeColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(str);
        Console.ForegroundColor = currentForeColor;
    }

    /// <summary>
    /// 配置输出的一个分组：标题 + 一组键值对
    /// </summary>
    public sealed class ConfigSection
    {
        public string Title { get; }
        public IDictionary<string, string> Items { get; }

        public ConfigSection(string title, IDictionary<string, string> items)
        {
            Title = title ?? string.Empty;
            Items = items ?? new Dictionary<string, string>();
        }
    }

    private static readonly string AccentColor = HexAnsi("7c84ff");
    private static readonly string MutedColor = HexAnsi("7a7a90");
    private static readonly string TrueColor = "\x1b[32m";
    private static readonly string FalseColor = "\x1b[31m";

    #region 有框面板（本地 Dev 使用）

    /// <summary>
    /// 以 2×2 四象限面板形式打印配置信息。
    /// 上半部为"品牌带"：左格放 <paramref name="logoLines"/>（居中），右格放 <paramref name="brandLines"/>。
    /// 下半部为"配置区"：左格放 <paramref name="leftSections"/>，右格放 <paramref name="rightSections"/>。
    /// 整体圆角单线边框，边框颜色由 <paramref name="borderHex"/> 指定（默认 #7c84ff）。
    /// 注意：依赖跨行对齐，仅适合本地交互式终端；服务器/docker logs 请用 <see cref="PrintConfigLines"/>。
    /// </summary>
    public static void PrintConfigBoard(
        IReadOnlyList<string> logoLines,
        IReadOnlyList<string> brandLines,
        IReadOnlyList<ConfigSection> leftSections,
        IReadOnlyList<ConfigSection> rightSections,
        string borderHex = "7c84ff",
        string brandHex = "7c84ff")
    {
        logoLines ??= Array.Empty<string>();
        brandLines ??= Array.Empty<string>();
        leftSections ??= Array.Empty<ConfigSection>();
        rightSections ??= Array.Empty<ConfigSection>();

        var border = HexAnsi(borderHex);

        // 左右格目标可视宽度（含 logo 与最长键值的兼容）
        int leftKeyW = SectionsMaxKeyWidth(leftSections);
        int rightKeyW = SectionsMaxKeyWidth(rightSections);
        int leftValW = SectionsMaxValueWidth(leftSections);
        int rightValW = SectionsMaxValueWidth(rightSections);

        int logoW = logoLines.Count == 0 ? 0 : logoLines.Max(GetStringRealLength);
        int leftInner = Math.Max(logoW, leftKeyW + 2 + Math.Max(leftValW, 1));
        int brandW = brandLines.Count == 0 ? 0 : brandLines.Max(GetStringRealLength);
        int rightInner = Math.Max(brandW, rightKeyW + 2 + Math.Max(rightValW, 1));
        rightInner = Math.Max(rightInner, 36);

        var brandColor = HexAnsi(brandHex);

        var sw = new StringWriter();
        void Emit(string s) => sw.WriteLine(s);

        // 顶部边框
        Emit(BorderLine("╭", "┬", "╮", leftInner, rightInner, border));

        // ---- 上半部（logo | 品牌）----
        int topRows = Math.Max(logoLines.Count, brandLines.Count);
        for (int i = 0; i < topRows; i++)
        {
            string leftContent = i < logoLines.Count ? logoLines[i] : "";
            string rightContent = i < brandLines.Count ? brandLines[i] : "";
            Emit(RowLine(
                leftContent, brandColor, true, leftInner,
                rightContent, brandColor, false, rightInner,
                border));
        }

        // 中间分隔横线
        Emit(BorderLine("├", "┼", "┤", leftInner, rightInner, border));

        // ---- 下半部（左配置 | 右配置）----
        var leftBody = ExpandSections(leftSections, leftKeyW, leftValW);
        var rightBody = ExpandSections(rightSections, rightKeyW, rightValW);
        int bottomRows = Math.Max(leftBody.Count, rightBody.Count);

        for (int i = 0; i < bottomRows; i++)
        {
            Row leftRow = i < leftBody.Count ? leftBody[i] : Row.Empty(leftInner);
            Row rightRow = i < rightBody.Count ? rightBody[i] : Row.Empty(rightInner);

            Emit(RowLine(
                leftRow.Text, leftRow.Color, false, leftInner,
                rightRow.Text, rightRow.Color, false, rightInner,
                border));
        }

        // 底部边框
        Emit(BorderLine("╰", "┴", "╯", leftInner, rightInner, border));

        Console.Write(sw.ToString());
    }

    /// <summary>面板中已格式化的一行：文本已补齐到目标可视宽度，颜色为 ANSI 序列（空=默认）</summary>
    private sealed class Row
    {
        public string Text { get; }
        public string Color { get; }

        public Row(string text, string color = null)
        {
            Text = text;
            Color = color;
        }

        public static Row Empty(int width) => new(new string(' ', width));
    }

    /// <summary>
    /// 把一组 sections 展开成 Row 列表。每个 Row 的 Text 已补齐到 (keyW + 2 + valW) 可视宽度。
    /// </summary>
    private static List<Row> ExpandSections(IReadOnlyList<ConfigSection> sections, int keyW, int valW)
    {
        var list = new List<Row>();
        int cellW = keyW + 2 + Math.Max(valW, 1);
        bool first = true;

        foreach (var s in sections)
        {
            if (!first)
                list.Add(new Row(new string('─', cellW), MutedColor));
            first = false;

            string title = "▸ " + s.Title;
            list.Add(new Row(PadRight(title, cellW), AccentColor));

            foreach (var kv in s.Items)
            {
                string keyPart = PadRight(kv.Key, keyW);
                string valPart = PadRight(kv.Value, valW);
                string text = keyPart + "  " + valPart;
                string color = kv.Value is "True" ? TrueColor : kv.Value is "False" ? FalseColor : MutedColor;
                list.Add(new Row(text, color));
            }
        }
        return list;
    }

    private static string RowLine(
        string leftText, string leftColor, bool leftCenter, int leftInner,
        string rightText, string rightColor, bool rightCenter, int rightInner,
        string border)
    {
        string leftPadded = leftCenter ? PadCenter(leftText, leftInner) : PadRight(leftText, leftInner);
        string rightPadded = rightCenter ? PadCenter(rightText, rightInner) : PadRight(rightText, rightInner);

        string leftOut = string.IsNullOrEmpty(leftColor) ? leftPadded : leftColor + leftPadded + AnsiReset();
        string rightOut = string.IsNullOrEmpty(rightColor) ? rightPadded : rightColor + rightPadded + AnsiReset();

        return $"{border}│{AnsiReset()} {leftOut} {border}│{AnsiReset()} {rightOut} {border}│{AnsiReset()}";
    }

    private static string BorderLine(string left, string mid, string right, int leftInner, int rightInner, string color)
        => $"{color}{left}{new string('─', leftInner + 2)}{mid}{new string('─', rightInner + 2)}{right}{AnsiReset()}";

    #endregion 有框面板（本地 Dev 使用）

    #region 无框逐行（服务器 / docker logs 使用）

    /// <summary>
    /// 逐行打印配置信息，无边框、无多列对齐，兼容所有终端（含 docker logs）。
    /// 先输出 <paramref name="headerLines"/>（强调色，常用于框架名/版本），空一行，
    /// 再依次输出每个分组：标题（▸ 标题，强调色）+ 键值行（键右对齐到组内最长键 + ": " + 值，
    /// 值按 True/False 着色），组间空一行分隔。
    /// </summary>
    public static void PrintConfigLines(
        IReadOnlyList<string> headerLines,
        IReadOnlyList<ConfigSection> sections)
    {
        headerLines ??= Array.Empty<string>();
        sections ??= Array.Empty<ConfigSection>();

        var sw = new StringWriter();
        void Emit(string s) => sw.WriteLine(s);

        // 头部信息：强调色逐行输出，结束后空一行
        foreach (var line in headerLines)
            Emit($"{AccentColor}{line}{AnsiReset()}");
        if (headerLines.Count > 0)
            Emit("");

        // 分组：标题 + 键值行，组间空一行
        bool first = true;
        foreach (var section in sections)
        {
            if (!first)
                Emit("");
            first = false;

            Emit($"{AccentColor}▸ {section.Title}{AnsiReset()}");

            // 组内最长键用于右对齐，让冒号在同列，阅读更整齐
            int keyW = section.Items.Count == 0 ? 0 : section.Items.Keys.Max(GetStringRealLength);
            foreach (var kv in section.Items)
            {
                string keyPart = PadLeft(kv.Key, keyW);
                string valColor = kv.Value is "True" ? TrueColor : kv.Value is "False" ? FalseColor : MutedColor;
                Emit($"  {MutedColor}{keyPart}{AnsiReset()}: {valColor}{kv.Value}{AnsiReset()}");
            }
        }

        Console.Write(sw.ToString());
    }

    #endregion 无框逐行（服务器 / docker logs 使用）

    // ---------- 宽度/对齐计算 ----------

    private static int SectionsMaxKeyWidth(IReadOnlyList<ConfigSection> sections)
        => sections.Count == 0 ? 0
            : sections.Max(s => s.Items.Count == 0 ? 0 : s.Items.Keys.Max(GetStringRealLength));

    private static int SectionsMaxValueWidth(IReadOnlyList<ConfigSection> sections)
        => sections.Count == 0 ? 0
            : sections.Max(s => s.Items.Count == 0 ? 0 : s.Items.Values.Max(GetStringRealLength));

    /// <summary>
    /// 计算字符串在等宽控制台中的可视列数。
    /// 规则：CJK 中文/全角、全角标点按 2 列；半角符号、Unicode 块元素
    /// （Block Elements U+2580–259F）、制表符（Box Drawing U+2500–257F）等按 1 列。
    /// 注：ANSI 转义序列（颜色、OSC 8 超链接等）不计入可视宽度。
    /// </summary>
    public static int GetStringRealLength(string str)
    {
        if (string.IsNullOrEmpty(str)) return 0;
        int len = 0;
        for (int i = 0; i < str.Length; i++)
        {
            // 跳过 ANSI 转义序列：CSI (\x1b[ ...字母) 和 OSC (\x1b] ... \x07 或 \x1b\\)
            if (str[i] == '\x1b')
            {
                i++;
                if (i < str.Length && str[i] == ']')
                {
                    // OSC 序列：读到 BEL(\x07) 或 ST(\x1b\\) 为止
                    while (i < str.Length - 1 && !(str[i] == '\x07' || (str[i] == '\x1b' && str[i + 1] == '\\')))
                        i++;
                    i++; // 跳过结束符
                }
                else
                {
                    // CSI 序列：读到字母（0x40-0x7E）为止
                    while (i < str.Length && (str[i] < 0x40 || str[i] > 0x7E))
                        i++;
                }
                continue;
            }
            len += IsWideChar(str[i]) ? 2 : 1;
        }
        return len;
    }

    /// <summary>判断字符是否按 2 列宽渲染（CJK 及全角字符）</summary>
    private static bool IsWideChar(char c)
    {
        // 制表符、块元素、几何形状等虽 codepoint > 127，但等宽字体按 1 列渲染
        if (c >= 0x2500 && c <= 0x259F) return false;   // Box Drawing / Block Elements
        if (c >= 0x25A0 && c <= 0x25FF) return false;   // Geometric Shapes
        if (c >= 0x2600 && c <= 0x27BF) return false;   // Misc Symbols / Dingbats（含 emoji）

        // CJK 统一表意文字、全角标点、日韩文按 2 列
        if (c >= 0x1100 && c <= 0x115F) return true;    // Hangul Jamo
        if (c >= 0x2E80 && c <= 0x303E) return true;     // CJK Radicals / 标点
        if (c >= 0x3040 && c <= 0x33BF) return true;     // 日文假名 / 韩文 / CJK 符号
        if (c >= 0x3400 && c <= 0x4DBF) return true;     // CJK Ext A
        if (c >= 0x4E00 && c <= 0x9FFF) return true;     // CJK 统一表意文字（常用中文）
        if (c >= 0xA000 && c <= 0xA4CF) return true;     // 彝文
        if (c >= 0xAC00 && c <= 0xD7AF) return true;     // 韩文音节
        if (c >= 0xF900 && c <= 0xFAFF) return true;     // CJK 兼容表意
        if (c >= 0xFE30 && c <= 0xFE4F) return true;     // CJK 兼容形式
        if (c >= 0xFF00 && c <= 0xFF60) return true;     // 全角 ASCII / 标点
        if (c >= 0xFFE0 && c <= 0xFFE6) return true;     // 全角符号

        return false; // 其余（含扩展区）默认按 1 列
    }

    private static string PadRight(string s, int width)
    {
        int diff = width - GetStringRealLength(s ?? string.Empty);
        return (s ?? string.Empty) + (diff > 0 ? new string(' ', diff) : string.Empty);
    }

    private static string PadLeft(string s, int width)
    {
        int diff = width - GetStringRealLength(s ?? string.Empty);
        return (diff > 0 ? new string(' ', diff) : string.Empty) + (s ?? string.Empty);
    }

    private static string PadCenter(string s, int width)
    {
        int len = GetStringRealLength(s ?? string.Empty);
        int diff = width - len;
        if (diff <= 0) return s ?? string.Empty;
        int l = diff / 2, r = diff - l;
        return new string(' ', l) + (s ?? string.Empty) + new string(' ', r);
    }

    // ---------- ANSI 真彩色 ----------

    /// <summary>
    /// 用 OSC 8 序列把文本包装成可点击的超链接。
    /// 支持的终端（Windows Terminal / VS Code / iTerm2 等）会渲染为可点击链接，
    /// 不支持的终端会原样显示文本（ANSI 序列不可见，不影响阅读）。
    /// </summary>
    public static string Hyperlink(string text, string url)
        => $"\x1b]8;;{url}\x1b\\{text}\x1b]8;;\x1b\\";

    private static string HexAnsi(string hex)
    {
        var (r, g, b) = HexToRgb(hex);
        return $"\x1b[38;2;{r};{g};{b}m";
    }

    private static string AnsiReset() => "\x1b[0m";

    private static (int r, int g, int b) HexToRgb(string hex)
    {
        if (hex.StartsWith("#")) hex = hex[1..];
        if (hex.Length != 6) return (124, 132, 255);
        return (Convert.ToInt32(hex[0..2], 16), Convert.ToInt32(hex[2..4], 16), Convert.ToInt32(hex[4..6], 16));
    }
}
