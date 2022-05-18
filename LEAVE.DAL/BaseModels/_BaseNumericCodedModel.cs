using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LEAVE.DAL
{
    public abstract class _BaseNumericCodedModel : _BaseNumericModel, ICodedModel
    {
        #region Fields

        [MaxLength(CONSTANTS.CODE_FIELD_LENGTH), Required
            , RegularExpression(CONSTANTS.CODE_FIELD_REGEX, ErrorMessage = CONSTANTS.CODE_FIELD_REGEX_ERROR_MESSAGE)]
        public string Code { get; set; }

        #endregion
    }
}
