using System;
using System.IO;
using System.Linq;
using BearPlan.Core.Extensions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BearPlan.Core.Helper;

public static class SixLaborsImageHelper
{
    private static readonly Color[] Colors =
    {
        Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown,
        Color.Brown, Color.DarkBlue
    };

    // SixLabors.Fonts 2.x 的 SystemFonts 只扫描系统已安装字体，Linux 容器默认无 Arial 也无 Roboto，
    // 会导致 SystemFonts.Families 为空，调用 .First() 直接抛 Sequence contains no elements。
    // 这里改为在首次使用时显式加载项目打包的 Roboto 字体（csproj 中 Content 拷贝到 bin/Fonts/ 下），
    // 保证跨平台一致可用。FontCollection 是进程级单例，重复 Add 同一文件会抛异常，故用 Lazy 保证只初始化一次。
    private static readonly Lazy<FontFamily> DefaultFontFamily = new(() =>
    {
        var collection = new FontCollection();
        var basePath = Path.Combine(AppContext.BaseDirectory, "Fonts");
        // 优先用 Bold 字重的静态 Roboto（视觉清晰），找不到时回退可变字体
        var boldPath = Path.Combine(basePath, "Roboto", "static", "Roboto-Bold.ttf");
        var variablePath = Path.Combine(basePath, "Roboto", "Roboto-VariableFont_wdth,wght.ttf");
        return File.Exists(boldPath)
            ? collection.Add(boldPath)
            : collection.Add(variablePath);
    });

    private static readonly char[] Chars =
    {
        '2', '3', '4', '5', '6', '8', '9',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y'
    };

    private static string GenCode(int num)
    {
        var code = string.Empty;
        var r = new Random();

        for (int i = 0; i < num; i++)
        {
            code += Chars[r.Next(Chars.Length)].ToString();
        }

        return code;
    }

    public static string BuilEmailCaptcha(int length)
    {
        return CreateValidateCode(length);
    }

    /// <summary>     
    /// 生成验证码     
    /// </summary>     
    /// <param name="length">指定验证码的长度</param>     
    /// <returns></returns>     
    public static string CreateValidateCode(int length)
    {
        int[] randMembers = new int[length];
        int[] validateNums = new int[length];
        string validateNumberStr = "";
        //生成起始序列值     
        int seekSeek = unchecked((int)DateTime.Now.Ticks);
        Random seekRand = new Random(seekSeek);
        int beginSeek = seekRand.Next(0, Int32.MaxValue - length * 10000);
        int[] seeks = new int[length];
        for (int i = 0; i < length; i++)
        {
            beginSeek += 10000;
            seeks[i] = beginSeek;
        }

        //生成随机数字     
        for (int i = 0; i < length; i++)
        {
            Random rand = new Random(seeks[i]);
            int pownum = 1 * (int)Math.Pow(10, length);
            randMembers[i] = rand.Next(pownum, Int32.MaxValue);
        }

        //抽取随机数字     
        for (int i = 0; i < length; i++)
        {
            string numStr = randMembers[i].ToString();
            int numLength = numStr.Length;
            Random rand = new Random();
            int numPosition = rand.Next(0, numLength - 1);
            validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
        }

        //生成验证码     
        for (int i = 0; i < length; i++)
        {
            validateNumberStr += validateNums[i].ToString();
        }

        return validateNumberStr;
    }


    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <param name="width">图像宽度</param>
    /// <param name="height">图像高度</param>
    /// <param name="fontSize">字体大小</param>
    /// <param name="length">验证码长度，如果是0则使用算数验证码</param>
    /// <returns></returns>
    public static (byte[] imgBytes, string code) BuildVerifyCode(int width, int height, int fontSize, int length = 0)
    {
        Random rnd = new Random();
        string expression = "";
        int mathResult = 0; //结果
        var r = new Random();
        using var image = new Image<Rgba32>(width, height);
        // 显式从打包的 Roboto 字体创建指定字号实例（不依赖系统字体，避免 Linux 容器字体缺失导致集合为空）
        var font = DefaultFontFamily.Value.CreateFont(fontSize, FontStyle.Bold);
        image.Mutate(ctx =>
        {
            ////生成3个10以内的整数，用来运算
            int operator1 = rnd.Next(0, 10);
            int operator2 = rnd.Next(0, 10);

            if (length > 0)
            {
                expression = CreateValidateCode(length);
                mathResult = expression.ToInt();
            }
            else
            {
                // 随机组合运算顺序 + - * 
                switch (rnd.Next(0, 3))
                {
                    case 0:
                        mathResult = operator1 + operator2;
                        expression = $"{operator1} + {operator2} = ?";
                        break;
                    case 1:
                        mathResult = operator1 - operator2;
                        expression = $"{operator1} - {operator2} = ?";
                        break;
                    default:
                        mathResult = operator2 * operator1;
                        expression = $"{operator1} x {operator2} = ?";
                        break;
                }
            }

            // 背景色
            ctx.Fill(Color.White);

            // 画验证码
            for (int i = 0; i < expression.Length; i++)
            {
                ctx.DrawText(expression[i].ToString()
                    , font
                    , Colors[r.Next(Colors.Length)]
                    , new PointF(5 + i * 10,
                        rnd.Next(1, 5)));
            }

            // 画干扰线
            for (int i = 0; i < 6; i++)
            {
                var pen = Pens.Solid(Colors[r.Next(Colors.Length)], 1);
                var p1 = new PointF(r.Next(width), r.Next(height));
                var p2 = new PointF(r.Next(width), r.Next(height));

                ctx.DrawLine(pen, p1, p2);
            }

            // 画噪点
            for (int i = 0; i < 60; i++)
            {
                // 创建一个画笔
                var pen = Pens.Solid(Colors[r.Next(Colors.Length)], 1);
                var p1 = new PointF(r.Next(width), r.Next(height));
                var p2 = new PointF(p1.X + 1f, p1.Y + 1f);

                ctx.DrawLine(pen, p1, p2);
            }
        });
        using var ms = new System.IO.MemoryStream();

        //  格式 自定义
        image.SaveAsJpeg(ms);
        return (ms.ToArray(), mathResult.ToString());
    }
}
