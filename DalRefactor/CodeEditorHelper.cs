using System;

namespace DalRefactor
{
    public static class CodeEditorHelper
    {
        public static bool IsCommentLine(string input, int currentLineIndex)
        {
            int lineStartIndex = input.Substring(0, currentLineIndex).LastIndexOf("\n", StringComparison.Ordinal); //mevcut satiri basindan al
            string currentLine = input.Substring(lineStartIndex).Trim();
            if (currentLine.StartsWith("//"))
            {
                return true;
            }
            return false;
        }
        public static bool IsOneTerm(string input)
        {
            if (input.Contains(" "))
                return false;
            else
                return true;

        }
        public static bool IsCurrentParameter(string input, int currentParameterEndIndex, int currentIndex, int indexForMethodEndBlock)
        {
            int indexForNextParameter = input.IndexOf("new SqlParameter(", currentParameterEndIndex);
            if (indexForNextParameter > 0 && indexForNextParameter < indexForMethodEndBlock)
            {//ayni method icinde baska parametre varsa
                if (currentIndex > indexForNextParameter)
                {//Direction baska parametreye ait ise
                    return false;
                }
            }
            return true;
        }
        public static bool ExistTermInCurrentBlock(string input, int currentMethodAnyIndex, string term)
        {
            int indexForTerm = input.IndexOf(term, currentMethodAnyIndex);
            int indexForNextMethodStart = input.IndexOf("public ", currentMethodAnyIndex);
            if (indexForTerm > 0 && (indexForNextMethodStart <= 0 || indexForTerm < indexForNextMethodStart))
            {//Ayni method icinde ilgili term varsa
                return true;
            }
            return false;
        }

        /// <summary>
        /// method icindeki son parametre kullanilan satir sonunun indexini doner
        /// </summary> 
        public static int GetLastParamEndIndex(string input, int startIndex, string arrayName, int indexForMethodEndBlock)
        {
            int lastIndexForArrayName = 0;
            do
            {
                startIndex = input.IndexOf(arrayName + "[", startIndex);
                if (startIndex <= 0)
                    break;

                if (startIndex < indexForMethodEndBlock)
                {
                    lastIndexForArrayName = ++startIndex;
                }

            } while (startIndex < indexForMethodEndBlock);

            if (lastIndexForArrayName <= 0)
                return 0;

            return input.IndexOf(";", lastIndexForArrayName) + 1;
        }
    }
}
