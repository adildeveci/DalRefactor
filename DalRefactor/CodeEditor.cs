using DalRefactor.Model;
using System;
using System.IO;
using System.Text;

namespace DalRefactor
{
    public static class CodeEditor
    {

        public static void EditFile(string dir, string fileName)
        {
            string fileNameWithPath = string.Join("\\", dir, fileName);
            string input = string.Empty;
            Encoding currentEncoding;
            using (StreamReader reader = new StreamReader(fileNameWithPath))
            {
                input = reader.ReadToEnd();
                currentEncoding = reader.CurrentEncoding;
            }

            int startIndex = 0;
            bool edited = false;
            EditResult editResult;
            do
            {
                editResult = EditMethod(input, startIndex);
                if (!string.IsNullOrWhiteSpace(editResult.NewInput))
                {
                    input = editResult.NewInput;
                    edited = true;
                }


                if (editResult.MethodLastIndex > startIndex)
                    startIndex = editResult.MethodLastIndex;
                else
                    startIndex++;

            } while (editResult.TryAgain);
            if (edited)
                AddUsingForCollectionsGeneric(ref input);//todo eger degisiklik varsa sartina bagla

            using (StreamWriter writer = new StreamWriter(fileNameWithPath, false, currentEncoding))
            {
                writer.Write(input);
                writer.Close();
            }
        }
        private static void ChangeParametersBlock(ref string input, string arrayName, string newDeclaration, int indexSqlParameterArrayStart, int indexForMethodEndBlock)
        {
            int replaceEndIndex = CodeEditorHelper.GetLastParamEndIndex(input, indexSqlParameterArrayStart, arrayName, indexForMethodEndBlock);
            input = input.Remove(indexSqlParameterArrayStart, replaceEndIndex - indexSqlParameterArrayStart);
            input = input.Insert(indexSqlParameterArrayStart, newDeclaration);
        }
        private static void ChangeRetValue(ref string input, int currentMethodAnyIndex)
        {
            int indexForReadRetValue = input.IndexOf(Constants.READ_RETVALUE_PATTERN, currentMethodAnyIndex);
            int indexForNextMethodStart = input.IndexOf("public ", currentMethodAnyIndex);
            if (indexForReadRetValue > 0 && (indexForNextMethodStart <= 0 || indexForReadRetValue < indexForNextMethodStart))
            {//Ayni method icinde parms[i] kullanilmis ise artik i kullanilmayacagi icin degistirecegiz
                input = input.Remove(indexForReadRetValue, Constants.READ_RETVALUE_PATTERN.Length);
                input = input.Insert(indexForReadRetValue, "(int)parms[parms.Count - 1]");
            }
        }


        private static EditResult EditMethod(string input, int startIndex)
        {
            var result = new EditResult
            {
                MethodLastIndex = startIndex,
            };
            try
            {
                // var stringBuilder = new StringBuilder(input);
                int indexSqlParameterArrayStart = input.IndexOf(Constants.METHOD_START_PATTERN, startIndex);
                if (indexSqlParameterArrayStart <= 0)
                {
                    return result;
                }

                if (input.Substring(indexSqlParameterArrayStart + Constants.METHOD_START_PATTERN.Length).IndexOf("\n", StringComparison.Ordinal) < input.Substring(indexSqlParameterArrayStart + Constants.METHOD_START_PATTERN.Length).IndexOf("=", StringComparison.Ordinal))
                {//SqlParameter[]   =  ; seklinde tanimlama yoksa dokunmadan geciyoruz
                    result.TryAgain = true;
                    return result;
                }

                if (CodeEditorHelper.IsCommentLine(input, indexSqlParameterArrayStart))
                {
                    result.TryAgain = true;
                    return result;
                }

                if (CodeEditorHelper.ExistTermInCurrentBlock(input, indexSqlParameterArrayStart, "changeToLower"))
                {//changeToLower kullanilan methodlari degistirmeyelim
                    result.TryAgain = true;
                    return result;
                }

                int lineStartIndex = input.Substring(0, indexSqlParameterArrayStart).LastIndexOf("\n", StringComparison.Ordinal);
                int columnSpace = indexSqlParameterArrayStart - lineStartIndex - 1;
                int indexForMethodEndBlock = input.IndexOf("parms);", indexSqlParameterArrayStart);//for parameter limit

                if (indexSqlParameterArrayStart <= 0 || indexForMethodEndBlock <= 0)
                {
                    result.TryAgain = true;
                    return result;
                }
                int indexSqlParameterArrayEnd = input.IndexOf(";", indexSqlParameterArrayStart);
                string arrayName = GetArrayVariableName(input, indexSqlParameterArrayStart);

                string newDeclaration = string.Format(Constants.NEW_ARRAY_DECLARATION_FORMAT, arrayName);
                string parms = GetParametersForNewFormat(input, indexSqlParameterArrayEnd, indexForMethodEndBlock, columnSpace);
                if (string.IsNullOrWhiteSpace(parms))
                {
                    result.TryAgain = true;
                    return result;
                }
                newDeclaration = newDeclaration.Replace("()", parms);

                ChangeParametersBlock(ref input, arrayName, newDeclaration, indexSqlParameterArrayStart, indexForMethodEndBlock);
                ChangeRetValue(ref input, indexSqlParameterArrayStart);

                result.MethodLastIndex = indexSqlParameterArrayStart + newDeclaration.Length;
                result.NewInput = input;
                result.TryAgain = true;
                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in EditMethod: {ex.Message}");
                return result;
            }
        }
        private static string GetParametersForNewFormat(string input, int startIndex, int indexForMethodEndBlock, int columnSpace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(' ', columnSpace);
            sb.AppendLine("{");
            string parameterName;
            string parameterValue;
            string parameterDirection;
            string parameterSqlDbType;
            int indexForParameterNameEnd;
            do
            {
                parameterName = GetParameterName(input, startIndex, indexForMethodEndBlock, out indexForParameterNameEnd);
                if (string.IsNullOrWhiteSpace(parameterName) || !CodeEditorHelper.IsOneTerm(parameterName.Trim()))
                    break;

                if (CodeEditorHelper.IsCommentLine(input, indexForParameterNameEnd))
                {
                    if (indexForParameterNameEnd > startIndex)
                        startIndex = indexForParameterNameEnd;
                    else
                        startIndex++;

                    continue;//parametre yorum satirina alinmis ise onu almadan devam et
                }

                parameterValue = GetParameterValue(input, indexForParameterNameEnd, indexForMethodEndBlock);
                parameterDirection = GetParameterDirection(input, indexForParameterNameEnd, indexForMethodEndBlock);

                if ((string.IsNullOrWhiteSpace(parameterValue) && string.IsNullOrWhiteSpace(parameterDirection))
                    || (!CodeEditorHelper.IsOneTerm(parameterDirection))
                    )//value ve direction ikisi birden bos ise veya direction icinde bosluk varsa (handle edilmeis case)
                    return string.Empty;

                if (!string.IsNullOrWhiteSpace(parameterValue) && (string.IsNullOrWhiteSpace(parameterDirection) || parameterDirection.Equals("Input")))
                {//value dolu ise ve direction bos yada input ise sade sekilde kullan
                    sb.Append(' ', columnSpace + 4);
                    sb.AppendLine(string.Format(Constants.NEW_INPUT_PARAMETER_FORMAT, parameterName, parameterValue));
                }
                else if (!string.IsNullOrWhiteSpace(parameterDirection) && !parameterDirection.Equals("Input"))
                {
                    parameterSqlDbType = GetParameterSqlDbType(input, indexForParameterNameEnd, indexForMethodEndBlock);
                    if (!string.IsNullOrWhiteSpace(parameterSqlDbType))
                    {
                        sb.Append(' ', columnSpace + 4);
                        if (string.IsNullOrWhiteSpace(parameterValue))
                        {
                            sb.AppendLine(string.Format(Constants.NEW_NONINPUT_PARAMETER_FORMAT, parameterName, parameterSqlDbType, parameterDirection));
                        }
                        else
                        {
                            sb.AppendLine(string.Format(Constants.NEW_NONINPUT_PARAMETER_WITHVALUE_FORMAT, parameterName, parameterSqlDbType, parameterDirection, parameterValue));
                        }
                    }
                }
                if (indexForParameterNameEnd > startIndex)
                    startIndex = indexForParameterNameEnd;
                else
                    startIndex++;

            }
            while (!string.IsNullOrWhiteSpace(parameterName));
            sb.Append(' ', columnSpace);
            sb.Append("};");

            return sb.ToString();
        }
        private static string GetParameterName(string input, int startIndex, int indexForMethodEndBlock, out int indexForParameterValueEnd)
        {
            indexForParameterValueEnd = 0;
            string patternStart = "] = new SqlParameter(";
            int parameterStartIndex = input.IndexOf(patternStart, startIndex) + 1; //+1 for "

            if (parameterStartIndex <= 0 || parameterStartIndex > indexForMethodEndBlock)
                return string.Empty;

            int parameterEndIndex = input.IndexOf(",", parameterStartIndex) - 1;
            int parameterLength = parameterEndIndex - parameterStartIndex - patternStart.Length;
            indexForParameterValueEnd = parameterEndIndex;
            return input.Substring(parameterStartIndex + patternStart.Length, parameterLength).Trim();
        }
        private static string GetParameterValue(string input, int startIndex, int indexForMethodEndBlock)
        {
            string patternStart = "].Value =";
            int parameterStartIndex = input.IndexOf(patternStart, startIndex);

            if (parameterStartIndex <= 0 || parameterStartIndex > indexForMethodEndBlock)
                return string.Empty;

            if (!CodeEditorHelper.IsCurrentParameter(input, startIndex, parameterStartIndex, indexForMethodEndBlock))
                return string.Empty;

            int parameterEndIndex = input.IndexOf(";", parameterStartIndex);
            int parameterLength = parameterEndIndex - parameterStartIndex - patternStart.Length;
            return input.Substring(parameterStartIndex + patternStart.Length, parameterLength).Trim();
        }
        private static string GetParameterSqlDbType(string input, int startIndex, int indexForMethodEndBlock)
        {
            string patternStart = "SqlDbType.";
            int parameterStartIndex = input.IndexOf(patternStart, startIndex);
            if (parameterStartIndex <= 0 || parameterStartIndex > indexForMethodEndBlock)
                return string.Empty;

            if (!CodeEditorHelper.IsCurrentParameter(input, startIndex, parameterStartIndex, indexForMethodEndBlock))
                return string.Empty;

            int parameterEndIndex = input.IndexOf(");", parameterStartIndex);
            int parameterLength = parameterEndIndex - parameterStartIndex - patternStart.Length;
            return input.Substring(parameterStartIndex + patternStart.Length, parameterLength).Trim();
        }
        private static string GetParameterDirection(string input, int startIndex, int indexForMethodEndBlock)
        {
            string patternStart = "ParameterDirection.";
            int parameterStartIndex = input.IndexOf(patternStart, startIndex);
            if (parameterStartIndex <= 0 || parameterStartIndex > indexForMethodEndBlock)
                return string.Empty;

            if (!CodeEditorHelper.IsCurrentParameter(input, startIndex, parameterStartIndex, indexForMethodEndBlock))
                return string.Empty;

            int parameterEndIndex = input.IndexOf(";", parameterStartIndex);
            int parameterLength = parameterEndIndex - parameterStartIndex - patternStart.Length;
            return input.Substring(parameterStartIndex + patternStart.Length, parameterLength).Trim();
        }
        private static string GetArrayVariableName(string input, int startIndex)
        {
            int startIndexForArrayName = input.IndexOf("SqlParameter[]", startIndex) + "SqlParameter[]".Length + 1; //+1 for space
            int endIndexForArrayName = input.IndexOf("=", startIndexForArrayName);
            return input.Substring(startIndexForArrayName, endIndexForArrayName - startIndexForArrayName).Trim();
        }
        private static void AddUsingForCollectionsGeneric(ref string input)
        {
            if (!input.Contains("using System.Collections.Generic"))
            {
                input = "using System.Collections.Generic;\n" + input;
            }
        }
    }
}
