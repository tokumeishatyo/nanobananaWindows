// rule.mdを読むこと
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace nanobananaWindows.Services
{
    /// <summary>
    /// テンプレートエンジン
    /// yaml_templatesフォルダからテンプレートを読み込み、変数を置換してYAMLを生成
    /// </summary>
    public partial class TemplateEngine
    {
        // パーシャル参照のパターン: {{> partial_name key="value"}}
        [GeneratedRegex(@"\{\{>\s*(\w+)([^}]*)\}\}")]
        private static partial Regex PartialPatternRegex();

        // パーシャルパラメータのパターン: key="value"
        [GeneratedRegex(@"(\w+)=""([^""]*)""")]
        private static partial Regex ParamPatternRegex();

        // 変数参照のパターン: {{variable_name}}
        [GeneratedRegex(@"\{\{(\w+)\}\}")]
        private static partial Regex VariablePatternRegex();

        private string? _templatesDirectory;

        /// <summary>
        /// テンプレートをレンダリング
        /// </summary>
        /// <param name="templateName">テンプレートファイル名</param>
        /// <param name="variables">変数辞書</param>
        /// <returns>生成されたYAML文字列</returns>
        public string Render(string templateName, Dictionary<string, string> variables)
        {
            // 1. テンプレートファイルを読み込む
            var template = LoadTemplate(templateName);
            if (template == null)
            {
                return GenerateErrorYaml($"Template not found: {templateName}");
            }

            // 2. パーシャルを展開
            var expanded = ExpandPartials(template, variables);

            // 3. 変数を置換
            var replaced = ReplaceVariables(expanded, variables);

            // 4. 空白値フィールドを削除
            var result = CleanupEmptyFields(replaced);

            return result;
        }

        /// <summary>
        /// テンプレートファイルを読み込む
        /// </summary>
        private string? LoadTemplate(string name)
        {
            var templatesPath = FindTemplatesDirectory();
            if (templatesPath == null) return null;

            var filePath = Path.Combine(templatesPath, name);
            if (!File.Exists(filePath)) return null;

            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// テンプレートディレクトリを検索
        /// </summary>
        private string? FindTemplatesDirectory()
        {
            // キャッシュがあればそれを返す
            if (_templatesDirectory != null && Directory.Exists(_templatesDirectory))
            {
                return _templatesDirectory;
            }

            // 実行ファイルの場所から上位ディレクトリを探索
            var baseDir = AppContext.BaseDirectory;
            var searchDir = new DirectoryInfo(baseDir);

            for (int i = 0; i < 10; i++)
            {
                var templatesDir = Path.Combine(searchDir.FullName, "yaml_templates");
                if (Directory.Exists(templatesDir))
                {
                    _templatesDirectory = templatesDir;
                    return templatesDir;
                }
                if (searchDir.Parent == null) break;
                searchDir = searchDir.Parent;
            }

            // カレントディレクトリからも検索
            var currentDir = Directory.GetCurrentDirectory();
            var currentTemplatesDir = Path.Combine(currentDir, "yaml_templates");
            if (Directory.Exists(currentTemplatesDir))
            {
                _templatesDirectory = currentTemplatesDir;
                return currentTemplatesDir;
            }

            return null;
        }

        /// <summary>
        /// パーシャルを展開（{{> partial_name key="value"}}）
        /// </summary>
        private string ExpandPartials(string template, Dictionary<string, string> variables)
        {
            var result = template;
            var maxIterations = 10;
            var partialPattern = PartialPatternRegex();
            var paramPattern = ParamPatternRegex();

            for (int i = 0; i < maxIterations; i++)
            {
                var matches = partialPattern.Matches(result);
                if (matches.Count == 0) break;

                // 後ろから置換（インデックスずれ防止）
                foreach (Match match in matches.Reverse())
                {
                    var partialName = match.Groups[1].Value;
                    var paramsString = match.Groups[2].Value;

                    // パラメータをパースして変数辞書にマージ
                    var mergedVariables = new Dictionary<string, string>(variables);
                    foreach (Match paramMatch in paramPattern.Matches(paramsString))
                    {
                        mergedVariables[paramMatch.Groups[1].Value] = paramMatch.Groups[2].Value;
                    }

                    // パーシャルを読み込み・置換
                    var partialContent = LoadTemplate($"{partialName}.yaml");
                    if (partialContent != null)
                    {
                        var expandedPartial = ReplaceVariables(partialContent, mergedVariables);
                        result = result.Remove(match.Index, match.Length)
                                      .Insert(match.Index, expandedPartial);
                    }
                    else
                    {
                        // パーシャルが見つからない場合はコメントに置換
                        result = result.Remove(match.Index, match.Length)
                                      .Insert(match.Index, $"# Partial not found: {partialName}");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 変数を置換（{{variable_name}}）
        /// </summary>
        private string ReplaceVariables(string template, Dictionary<string, string> variables)
        {
            var variablePattern = VariablePatternRegex();
            return variablePattern.Replace(template, match =>
            {
                var variableName = match.Groups[1].Value;
                return variables.TryGetValue(variableName, out var value) ? value : match.Value;
            });
        }

        /// <summary>
        /// 空白値フィールドを削除
        /// </summary>
        private string CleanupEmptyFields(string yaml)
        {
            var lines = yaml.Split('\n');
            var result = new List<string>();
            int i = 0;

            while (i < lines.Length)
            {
                var line = lines[i];
                var trimmed = line.TrimEnd();

                // 空の値を持つ行をスキップ（key: "" または key: ''）
                if (IsEmptyValueLine(trimmed))
                {
                    i++;
                    continue;
                }

                // セクションヘッダーの場合、子要素があるかチェック
                if (IsSectionHeader(line, i + 1 < lines.Length ? lines[i + 1] : null))
                {
                    var sectionIndent = GetIndent(line);
                    var hasValidChildren = false;
                    var j = i + 1;

                    while (j < lines.Length)
                    {
                        var childLine = lines[j];
                        var childIndent = GetIndent(childLine);
                        var childTrimmed = childLine.Trim();

                        // 空行やコメント行はスキップ
                        if (string.IsNullOrEmpty(childTrimmed) || childTrimmed.StartsWith('#'))
                        {
                            j++;
                            continue;
                        }

                        // インデントが戻ったらセクション終了
                        if (childIndent <= sectionIndent)
                        {
                            break;
                        }

                        // 空でない値を持つ子要素があるかチェック
                        if (!IsEmptyValueLine(childLine.TrimEnd()))
                        {
                            hasValidChildren = true;
                            break;
                        }
                        j++;
                    }

                    // 有効な子要素がない場合、このセクションヘッダーをスキップ
                    if (!hasValidChildren)
                    {
                        i++;
                        continue;
                    }
                }

                result.Add(line);
                i++;
            }

            // 連続する空行を1つにまとめる
            return ConsolidateEmptyLines(string.Join("\n", result));
        }

        /// <summary>
        /// 空の値を持つ行かどうかをチェック
        /// </summary>
        private static bool IsEmptyValueLine(string line)
        {
            var trimmed = line.Trim();
            return trimmed.EndsWith(": \"\"") || trimmed.EndsWith(": ''");
        }

        /// <summary>
        /// セクションヘッダー（子要素を持つ行）かどうかをチェック
        /// </summary>
        private static bool IsSectionHeader(string line, string? nextLine)
        {
            var trimmed = line.Trim();

            // コメント行や空行はセクションヘッダーではない
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
            {
                return false;
            }

            // "key:" で終わる行（値なし）はセクションヘッダー
            if (trimmed.EndsWith(':') && !trimmed.Contains(": "))
            {
                return true;
            }

            // 次の行がより深いインデントを持つ場合もセクションヘッダー
            if (nextLine != null)
            {
                var currentIndent = GetIndent(line);
                var nextIndent = GetIndent(nextLine);
                var nextTrimmed = nextLine.Trim();
                if (nextIndent > currentIndent && !string.IsNullOrEmpty(nextTrimmed) && !nextTrimmed.StartsWith('#'))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 行のインデントレベルを取得
        /// </summary>
        private static int GetIndent(string line)
        {
            int count = 0;
            foreach (char c in line)
            {
                if (c == ' ')
                {
                    count++;
                }
                else if (c == '\t')
                {
                    count += 2; // タブは2スペースとして扱う
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        /// <summary>
        /// 連続する空行を1つにまとめる
        /// </summary>
        private static string ConsolidateEmptyLines(string text)
        {
            var lines = text.Split('\n');
            var result = new List<string>();
            var previousWasEmpty = false;

            foreach (var line in lines)
            {
                var isEmpty = string.IsNullOrWhiteSpace(line);
                if (isEmpty)
                {
                    if (!previousWasEmpty)
                    {
                        result.Add(line);
                    }
                    previousWasEmpty = true;
                }
                else
                {
                    result.Add(line);
                    previousWasEmpty = false;
                }
            }

            return string.Join("\n", result);
        }

        /// <summary>
        /// エラー用YAMLを生成
        /// </summary>
        private string GenerateErrorYaml(string message)
        {
            var templatesDir = FindTemplatesDirectory() ?? "not found";
            var availableTemplates = "";

            if (Directory.Exists(templatesDir))
            {
                var files = Directory.GetFiles(templatesDir, "*.yaml")
                                    .Select(Path.GetFileName);
                availableTemplates = string.Join(", ", files);
            }

            return $"""
# ====================================================
# Error
# ====================================================
# {message}
#
# テンプレートディレクトリ: {templatesDir}
# 利用可能なテンプレート: {availableTemplates}
# ====================================================
""";
        }
    }
}
