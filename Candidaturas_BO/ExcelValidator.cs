using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Candidaturas_BO
{
    public class ExcelValidator
    {

        public static Boolean HasExcelExtension(HttpPostedFileBase file)
        {
            return Path.GetExtension(file.FileName) == ".xls" || Path.GetExtension(file.FileName) == ".xlsx";
        }

        public static Boolean HasExcelMimeType(HttpPostedFileBase file)
        {
            return file.ContentType == "application/vnd.ms-excel" || file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }
}