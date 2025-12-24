// rule.mdを読むこと
using System;
using System.IO;
using System.Linq;

namespace nanobananaWindows.Services
{
    /// <summary>
    /// YAML生成共通ユーティリティ
    /// </summary>
    public static class YamlUtilities
    {
        /// <summary>
        /// YAML文字列のエスケープ
        /// </summary>
        /// <param name="str">エスケープする文字列</param>
        /// <returns>エスケープされた文字列</returns>
        public static string EscapeYamlString(string? str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        /// <summary>
        /// 改行をカンマ区切りに変換
        /// </summary>
        /// <param name="str">変換する文字列</param>
        /// <returns>カンマ区切りに変換された文字列</returns>
        public static string ConvertNewlinesToComma(string? str)
        {
            if (string.IsNullOrEmpty(str)) return "";

            var lines = str.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => s.Trim())
                          .Where(s => !string.IsNullOrEmpty(s));

            var joined = string.Join(", ", lines);
            return EscapeYamlString(joined);
        }

        /// <summary>
        /// ファイルパスからファイル名のみを取得
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>ファイル名</returns>
        public static string GetFileName(string? path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            return Path.GetFileName(path);
        }
    }
}
