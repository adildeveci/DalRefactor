namespace DalRefactor
{
    public static class Constants
    {
        public const string READ_RETVALUE_PATTERN = "(int)parms[i]";
        public const string METHOD_START_PATTERN = "SqlParameter[]";
        public const string NEW_ARRAY_DECLARATION_FORMAT = "List<SqlParameter> {0} = new List<SqlParameter>()";
        public const string NEW_INPUT_PARAMETER_FORMAT = @"new SqlParameter(""{0}"", {1}),";
        public const string NEW_NONINPUT_PARAMETER_FORMAT = @"new SqlParameter(""{0}"", SqlDbType.{1}) {{ Direction = ParameterDirection.{2} }},";
        public const string NEW_NONINPUT_PARAMETER_WITHVALUE_FORMAT = @"new SqlParameter(""{0}"", SqlDbType.{1}) {{ Direction = ParameterDirection.{2}, Value = {3} }},";

    }
}
