using System;
using System.Collections.Generic;
using System.Text;

namespace LEAVE.DAL
{
    public static class CONSTANTS
    {
        public const decimal VERSION = 1.0M;

        /// <summary>
        /// LOCKOUT DATE
        /// </summary>
        public static DateTime DET_VAL = new DateTime(2025, 1, 1);

        public const string SUPER_USER = "admin";
        public const string SUPER_ROLE = "administrator";

        public const int CODE_FIELD_LENGTH = 128;
        public const string CODE_FIELD_REGEX = "^[A-Z0-9_a-z]+$";
        public const string CODE_FIELD_REGEX_ERROR_MESSAGE = "[A-Z0-9_a-z] only!";
    }
}
